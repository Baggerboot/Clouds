using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perlin3D
{
	class JMath
	{
		public static readonly float PI;

		private static float[] lookup;

		static JMath()
		{
			lookup = new float[1000];
			for (int i = 0; i < 1000; i++) {
				lookup[i] = ((1 - (float)Math.Cos(Math.PI / 1000 * i)) * 0.5f);
			}

			PI = (float)Math.PI;
		}

		private static float FastCos(float value)
		{
			return lookup[(int)(value * 1000)];
		}

		public static float FastCosineInterpolate(float a, float b, float x)
		{
			float f = FastCos(x);
			return a * (1 - f) + b * f;
		}
		public static float LinInterpolate(float a, float b, float x)
		{
			return a * (1 - x) + b * x;
		}
	}
}
