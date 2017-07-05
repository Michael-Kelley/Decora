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
using NAudio.Dsp;

#endregion

namespace Decora
{
	public class SampleAggregator
	{
		float volumeLeftMaxValue, volumeLeftMinValue;
		float volumeRightMaxValue, volumeRightMinValue;
		Complex[] channelData;
		int channelDataPosition;

		public SampleAggregator(int bufferSize)
			{ channelData = new Complex[bufferSize]; }

		public void Clear()
		{
			volumeLeftMaxValue = float.MinValue;
			volumeRightMaxValue = float.MinValue;
			volumeLeftMinValue = float.MaxValue;
			volumeRightMinValue = float.MaxValue;
			channelDataPosition = 0;
		}

		/// <summary>Add a sample value to the aggregator.</summary>
		/// <param name="value">The value of the sample.</param>
		public void Add(float leftValue, float rightValue)
		{
			if (channelDataPosition == 0)
			{
				volumeLeftMaxValue = float.MinValue;
				volumeRightMaxValue = float.MinValue;
				volumeLeftMinValue = float.MaxValue;
				volumeRightMinValue = float.MaxValue;
			}

			// Make stored channel data stereo by averaging left and right values.
			channelData[channelDataPosition].X = (leftValue + rightValue) / 2.0f;
			channelData[channelDataPosition].Y = 0;
			channelDataPosition++;

			volumeLeftMaxValue = Math.Max(volumeLeftMaxValue, leftValue);
			volumeLeftMinValue = Math.Min(volumeLeftMinValue, leftValue);
			volumeRightMaxValue = Math.Max(volumeRightMaxValue, rightValue);
			volumeRightMinValue = Math.Min(volumeRightMinValue, rightValue);

			if (channelDataPosition >= channelData.Length)
				channelDataPosition = 0;
		}

		/// <summary>Performs an FFT calculation on the channel data upon request.</summary>
		/// <param name="fftBuffer">A buffer where the FFT data will be stored.</param>
		public void GetFFTResults(float[] fftBuffer)
		{
			Complex[] channelDataClone = new Complex[4096];
			channelData.CopyTo(channelDataClone, 0);
			// 4096 = 2^12
			FastFourierTransform.FFT(true, 12, channelDataClone);
			for (int i = 0; i < channelDataClone.Length / 2; i++)
				fftBuffer[i] = (float)Math.Sqrt(channelDataClone[i].X * channelDataClone[i].X + channelDataClone[i].Y * channelDataClone[i].Y); // Calculate actual intensities for the FFT results.
		}

		public float LeftMaxVolume
			{ get { return volumeLeftMaxValue; } }

		public float LeftMinVolume
			{ get { return volumeLeftMinValue; } }

		public float RightMaxVolume
			{ get { return volumeRightMaxValue; } }

		public float RightMinVolume
			{ get { return volumeRightMinValue; } }
	}
}