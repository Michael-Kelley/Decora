/*
	Copyright © 2014, Forge Development
	All rights reserved.
	http://forge-dev.com


	This file is part of Decora.

	Decora is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	any later version.

	Decora is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Decora.  If not, see <http://www.gnu.org/licenses/>.
*/

#region Includes

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;

using NAudio.Wave;
using WPFSoundVisualizationLib;

#endregion

namespace Decora
{
	class NAudioEngine : INotifyPropertyChanged, ISpectrumPlayer, IWaveformPlayer, IDisposable
	{
		#region Fields

		static NAudioEngine instance;
		readonly DispatcherTimer positionTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
		readonly BackgroundWorker waveformGenerateWorker = new BackgroundWorker();
		bool disposed;
		bool canPlay, canPause, canStop;
		bool isPlaying;
		bool inChannelTimerUpdate;
		double channelLength;
		double channelPosition;
		bool inChannelSet;
		WaveOut waveOutDevice;
		WaveStream activeStream;
		WaveChannel32 inputStream;
		SampleAggregator sampleAggregator;
		SampleAggregator waveformAggregator;
		string pendingWaveformPath;
		float[] fullLevelData;
		float[] waveformData;
		TagLib.File fileTag;
		long repeatStart, repeatEnd;

		#endregion

		#region Constants

		const int waveformCompressedPointCount = 2000;

		#endregion

		#region Singleton Pattern

		public static NAudioEngine Instance
		{
			get
			{
				if (instance == null)
					instance = new NAudioEngine();
				return instance;
			}
		}

		#endregion

		#region Constructor

		NAudioEngine()
		{
			positionTimer.Interval = TimeSpan.FromMilliseconds(500);
			positionTimer.Tick += positionTimer_Tick;

			waveformGenerateWorker.DoWork += waveformGenerateWorker_DoWork;
			waveformGenerateWorker.RunWorkerCompleted += waveformGenerateWorker_RunWorkerCompleted;
			waveformGenerateWorker.WorkerSupportsCancellation = true;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					StopAndCloseStream();
				}

				disposed = true;
			}
		}

		#endregion

		#region ISpectrumPlayer

		public bool GetFFTData(float[] fftDataBuffer)
		{
			sampleAggregator.GetFFTResults(fftDataBuffer);
			return isPlaying;
		}

		public int GetFFTFrequencyIndex(int frequency)
		{
			double maxFrequency;
			if (ActiveStream != null)
				maxFrequency = ActiveStream.WaveFormat.SampleRate / 2.0d;
			else
				maxFrequency = 22050; // Assume a default 44.1 kHz sample rate.
			return (int)((frequency / maxFrequency) * 2048);
		}

		#endregion

		#region IWaveformPlayer

		public void SetRepeatRange(double start, double stop)
		{
			repeatStart = (long)((start / ActiveStream.TotalTime.TotalSeconds) * ActiveStream.Length);
			repeatEnd = (long)((stop / ActiveStream.TotalTime.TotalSeconds) * ActiveStream.Length);
		}

		public void ClearRepeatRange()
		{
			repeatStart = -1;
			repeatEnd = -1;
		}

		public float[] WaveformData
		{
			get { return waveformData; }
			protected set
			{
				float[] oldValue = waveformData;
				waveformData = value;
				if (oldValue != waveformData)
					NotifyPropertyChanged("WaveformData");
			}
		}

		public double ChannelLength
		{
			get { return channelLength; }
			protected set
			{
				double oldValue = channelLength;
				channelLength = value;
				if (oldValue != channelLength)
					NotifyPropertyChanged("ChannelLength");
			}
		}

		public double ChannelPosition
		{
			get { return channelPosition; }
			set
			{
				if (!inChannelSet)
				{
					inChannelSet = true; // Avoid recursion
					double oldValue = channelPosition;
					double position = Math.Max(0, Math.Min(value, ChannelLength));
					if (!inChannelTimerUpdate && ActiveStream != null)
						ActiveStream.Position = (long)((position / ActiveStream.TotalTime.TotalSeconds) * ActiveStream.Length);
					channelPosition = position;
					if (oldValue != channelPosition)
						NotifyPropertyChanged("ChannelPosition");
					inChannelSet = false;
				}
			}
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(info));
		}

		#endregion

		#region Waveform Generation

		class WaveformGenerationParams
		{
			public WaveformGenerationParams(int points, string path)
			{
				Points = points;
				Path = path;
			}

			public int Points { get; protected set; }
			public string Path { get; protected set; }
		}

		void GenerateWaveformData(string path)
		{
			if (waveformGenerateWorker.IsBusy)
			{
				pendingWaveformPath = path;
				waveformGenerateWorker.CancelAsync();
				return;
			}

			if (!waveformGenerateWorker.IsBusy && waveformCompressedPointCount != 0)
				waveformGenerateWorker.RunWorkerAsync(new WaveformGenerationParams(waveformCompressedPointCount, path));
		}

		void waveformGenerateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
				if (!waveformGenerateWorker.IsBusy && waveformCompressedPointCount != 0)
					waveformGenerateWorker.RunWorkerAsync(new WaveformGenerationParams(waveformCompressedPointCount, pendingWaveformPath));
		}

		void waveformGenerateWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			WaveformGenerationParams waveformParams = e.Argument as WaveformGenerationParams;
			Mp3FileReader waveformMp3Stream = new Mp3FileReader(waveformParams.Path);
			WaveChannel32 waveformInputStream = new WaveChannel32(waveformMp3Stream);
			waveformInputStream.Sample += waveStream_Sample;

			int frameLength = (int)((20.0d / waveformInputStream.TotalTime.TotalMilliseconds) * waveformInputStream.Length); // Sample 20ms of data.
			int frameCount = (int)((double)waveformInputStream.Length / (double)frameLength);
			int waveformLength = frameCount * 2;
			byte[] readBuffer = new byte[frameLength];
			waveformAggregator = new SampleAggregator(frameLength);

			float maxLeftPointLevel = float.MinValue;
			float maxRightPointLevel = float.MinValue;
			int currentPointIndex = 0;
			float[] waveformCompressedPoints = new float[waveformParams.Points];
			List<float> waveformData = new List<float>();
			List<int> waveMaxPointIndexes = new List<int>();

			for (int i = 1; i <= waveformParams.Points; i++)
				waveMaxPointIndexes.Add((int)Math.Round(waveformLength * ((double)i / (double)waveformParams.Points), 0));

			int readCount = 0;

			while (currentPointIndex * 2 < waveformParams.Points)
			{
				waveformInputStream.Read(readBuffer, 0, readBuffer.Length);

				waveformData.Add(waveformAggregator.LeftMaxVolume);
				waveformData.Add(waveformAggregator.RightMaxVolume);

				if (waveformAggregator.LeftMaxVolume > maxLeftPointLevel)
					maxLeftPointLevel = waveformAggregator.LeftMaxVolume;
				if (waveformAggregator.RightMaxVolume > maxRightPointLevel)
					maxRightPointLevel = waveformAggregator.RightMaxVolume;

				if (readCount > waveMaxPointIndexes[currentPointIndex])
				{
					waveformCompressedPoints[(currentPointIndex * 2)] = maxLeftPointLevel;
					waveformCompressedPoints[(currentPointIndex * 2) + 1] = maxRightPointLevel;
					maxLeftPointLevel = float.MinValue;
					maxRightPointLevel = float.MinValue;
					currentPointIndex++;
				}
				if (readCount % 3000 == 0)
				{
					float[] clonedData = (float[])waveformCompressedPoints.Clone();
					App.Current.Dispatcher.Invoke(new Action(() =>
					{
						WaveformData = clonedData;
					}));
				}

				if (waveformGenerateWorker.CancellationPending)
				{
					e.Cancel = true;
					break;
				}
				readCount++;
			}

			float[] finalClonedData = (float[])waveformCompressedPoints.Clone();

			App.Current.Dispatcher.Invoke(new Action(() =>
			{
				fullLevelData = waveformData.ToArray();
				WaveformData = finalClonedData;
			}));

			waveformInputStream.Close();
			waveformInputStream.Dispose();
			waveformInputStream = null;
			waveformMp3Stream.Close();
			waveformMp3Stream.Dispose();
			waveformMp3Stream = null;
		}

		#endregion

		#region Private Utility Methods

		void StopAndCloseStream()
		{
			if (waveOutDevice != null)
			{
				waveOutDevice.Stop();
			}
			if (activeStream != null)
			{
				inputStream.Close();
				inputStream = null;
				ActiveStream.Close();
				ActiveStream = null;
			}
			if (waveOutDevice != null)
			{
				waveOutDevice.Dispose();
				waveOutDevice = null;
			}
		}

		#endregion

		#region Public Methods

		public void Stop()
		{
			if (waveOutDevice != null)
			{
				waveOutDevice.Stop();
			}
			IsPlaying = false;
			CanStop = false;
			CanPlay = true;
			CanPause = false;
		}

		public void Pause()
		{
			if (IsPlaying && CanPause)
			{
				waveOutDevice.Pause();
				IsPlaying = false;
				CanPlay = true;
				CanPause = false;
			}
		}

		public void Play()
		{
			if (CanPlay)
			{
				waveOutDevice.Play();
				IsPlaying = true;
				CanPause = true;
				CanPlay = false;
				CanStop = true;
			}
		}

		public void OpenFile(string path)
		{
			Stop();
			StopAndCloseStream();

			if (ActiveStream != null)
			{
				ClearRepeatRange();
				ChannelPosition = 0;
			}

			if (System.IO.File.Exists(path))
			{
				try
				{
					waveOutDevice = new WaveOut()
					{
						DesiredLatency = 100
					};
					ActiveStream = new Mp3FileReader(path);
					inputStream = new WaveChannel32(ActiveStream);
					sampleAggregator = new SampleAggregator(4096);
					inputStream.Sample += inputStream_Sample;
					waveOutDevice.Init(inputStream);
					ChannelLength = inputStream.TotalTime.TotalSeconds;
					FileTag = TagLib.File.Create(path);
					GenerateWaveformData(path);
					CanPlay = true;
				}
				catch
				{
					ActiveStream = null;
					CanPlay = false;
				}
			}
		}

		public void OpenStream(System.IO.Stream stream)
		{
			Stop();
			StopAndCloseStream();

			if (ActiveStream != null)
			{
				ClearRepeatRange();
				ChannelPosition = 0;
			}

			try
			{
				waveOutDevice = new WaveOut()
				{
					DesiredLatency = 100
				};
				ActiveStream = new Mp3FileReader(stream);
				inputStream = new WaveChannel32(ActiveStream);
				sampleAggregator = new SampleAggregator(4096);
				inputStream.Sample += inputStream_Sample;
				waveOutDevice.Init(inputStream);
				ChannelLength = inputStream.TotalTime.TotalSeconds;
				CanPlay = true;
			}
			catch
			{
				ActiveStream = null;
				CanPlay = false;
			}
		}

		#endregion

		#region Public Properties

		public TagLib.File FileTag
		{
			get { return fileTag; }
			set
			{
				TagLib.File oldValue = fileTag;
				fileTag = value;
				if (oldValue != fileTag)
					NotifyPropertyChanged("FileTag");
			}
		}

		public WaveStream ActiveStream
		{
			get { return activeStream; }
			protected set
			{
				WaveStream oldValue = activeStream;
				activeStream = value;
				if (oldValue != activeStream)
					NotifyPropertyChanged("ActiveStream");
			}
		}

		public bool CanPlay
		{
			get { return canPlay; }
			protected set
			{
				bool oldValue = canPlay;
				canPlay = value;
				if (oldValue != canPlay)
					NotifyPropertyChanged("CanPlay");
			}
		}

		public bool CanPause
		{
			get { return canPause; }
			protected set
			{
				bool oldValue = canPause;
				canPause = value;
				if (oldValue != canPause)
					NotifyPropertyChanged("CanPause");
			}
		}

		public bool CanStop
		{
			get { return canStop; }
			protected set
			{
				bool oldValue = canStop;
				canStop = value;
				if (oldValue != canStop)
					NotifyPropertyChanged("CanStop");
			}
		}


		public bool IsPlaying
		{
			get { return isPlaying; }
			protected set
			{
				bool oldValue = isPlaying;
				isPlaying = value;
				if (oldValue != isPlaying)
					NotifyPropertyChanged("IsPlaying");
				positionTimer.IsEnabled = value;
			}
		}

		#endregion

		#region Event Handlers

		void inputStream_Sample(object sender, SampleEventArgs e)
		{
			sampleAggregator.Add(e.Left, e.Right);
			if (repeatEnd != -1 && ActiveStream.Position >= repeatEnd)
			{
				sampleAggregator.Clear();
				ActiveStream.Position = repeatStart;
			}
		}

		void waveStream_Sample(object sender, SampleEventArgs e)
		{
			waveformAggregator.Add(e.Left, e.Right);
		}

		void positionTimer_Tick(object sender, EventArgs e)
		{
			inChannelTimerUpdate = true;
			ChannelPosition = ((double)ActiveStream.Position / (double)ActiveStream.Length) * ActiveStream.TotalTime.TotalSeconds;
			inChannelTimerUpdate = false;
		}

		#endregion
	}
}