using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Perlin3D
{
	class Perlin2D
	{
		private int seed;
		private Random rand;
		private float octave_f;
		private int octave;
		public int Octave
		{
			get
			{
				return octave;
			}
		}
		private readonly float PI;

		private float[] lookup;
		
		/// <summary>
		/// max 3.141592
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private float Cos(float value)
		{
			return lookup[(int)(value * 1000)];
		}

		static float Rand(int seed)
		{
			ulong next = unchecked((ulong)seed);
			next = next * 1103515245 + 12345;
			int result = (int)(next / 65536) % 32768;
			return result / 32740F;
		}

		static float Rand2(int seed)
		{
			ulong next = unchecked((ulong)seed);
			next = next * 1103515245 + 12345;
			uint result = (uint)(next / 65536) % 32768;
			float ret = result / 32768F;
			return ret;
		}

		public Perlin2D(int seed, int octave)
		{
			lookup = new float[1000];
			for (int i = 0; i < 1000; i++) {
				lookup[i] = ((1 - (float)Math.Cos(Math.PI / 1000 * i)) * 0.5f);
			}

			this.seed = seed;
			this.octave = octave;
			octave_f = octave;
			rand = new Random();
			PI = (float)Math.PI;
		}

		/// <summary>
		/// Returns the noise levels as a two-dimensional array in the area [octave*x,octave*y;octave*x+octave,octave*y+octave]
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public float[,] getNoiseLevelsAtOctave(int x, int y)
		{
			float r00 = getRandomAtPosition(new Vector2(x, y));
			float r10 = getRandomAtPosition(new Vector2(x+1, y));
			float r11 = getRandomAtPosition(new Vector2(x+1, y+1));
			float r01 = getRandomAtPosition(new Vector2(x, y+1));

			float[,] ret = new float[octave, octave];

			float[] top = new float[octave];
			float[] bottom = new float[octave];

			for(int i = 0; i < octave; i++)
			{
				float botInterv = cosineInterpolate(r00, r10, i / octave_f);
				bottom[i] = botInterv;
				ret[i,0] = botInterv;
				float topInterv = cosineInterpolate(r01, r11, i / octave_f);
				top[i] = topInterv;
				ret[i,octave-1] = topInterv;

			}

			for (int i = 0; i < octave; i++) {
				for (int j = 1; j < octave-1; j++) {
					ret[i,j] = cosineInterpolate(bottom[i], top[i], j / octave_f);
				}
			}
			return ret;
		}

		public float getNoiseLevelAtPosition(int x, int y)
		{
			int xmin = (int)(x / octave);
			int ymin = (int)(y / octave);

			Vector2 a = new Vector2(xmin, ymin);
			Vector2 b = new Vector2(xmin+1, ymin);
			Vector2 c = new Vector2(xmin+1, ymin+1);
			Vector2 d = new Vector2(xmin, ymin+1);

			float ra = getRandomAtPosition(a);
			float rb = getRandomAtPosition(b);
			float rc = getRandomAtPosition(c);
			float rd = getRandomAtPosition(d);


			float x1 = fastCosineInterpolate(ra, rb, (x - xmin * octave) / octave_f);
			float x2 = fastCosineInterpolate(rd, rc, (x - xmin * octave) / octave_f);
			//float x1 = 0.5f;
			//float x2 = 0.5f;
			float ret = fastCosineInterpolate(x1, x2, (y - ymin * octave_f) / octave);

			//float ret = 0.4F;

			return ret;
		}

		private float linInterpolate(float a, float b, float x)
		{
			return a * (1 - x) + b * x;
		}

		private float cosineInterpolate(float a, float b, float x)
		{
			float ft = (float)(x * PI);
			float f = (float)((1f - Math.Cos(ft)) * .5f);
			float ret = a * (1f - f) + b * f;
			return ret;
		}
		private float fastCosineInterpolate(float a, float b, float x)
		{
			float f = Cos(x);
			return a * (1 - f) + b * f;
		}

		public float getRandomAtPosition(Vector2 coord)
		{
			return Rand2((int)(10000 * (Math.Sin(coord.X) + Math.Cos(coord.Y) + seed)));
			//return Rand2(((int)coord.X << 16) + (int)coord.Y);
		}
	}
}
