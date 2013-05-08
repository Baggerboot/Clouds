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

		

		public Perlin2D(int seed, int octave)
		{
			this.seed = seed;
			this.octave = octave;
			octave_f = octave;
			rand = new Random();
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
				float botInterv = JMath.FastCosineInterpolate(r00, r10, i / octave_f);
				bottom[i] = botInterv;
				ret[i,0] = botInterv;
				float topInterv = JMath.FastCosineInterpolate(r01, r11, i / octave_f);
				top[i] = topInterv;
				ret[i,octave-1] = topInterv;

			}

			for (int i = 0; i < octave; i++) {
				for (int j = 1; j < octave-1; j++) {
					ret[i, j] = JMath.FastCosineInterpolate(bottom[i], top[i], j / octave_f);
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


			float x1 = JMath.FastCosineInterpolate(ra, rb, (x - xmin * octave) / octave_f);
			float x2 = JMath.FastCosineInterpolate(rd, rc, (x - xmin * octave) / octave_f);
			//float x1 = 0.5f;
			//float x2 = 0.5f;
			float ret = JMath.FastCosineInterpolate(x1, x2, (y - ymin * octave_f) / octave);

			//float ret = 0.4F;

			return ret;
		}

		public float getRandomAtPosition(Vector2 coord)
		{
			return JMath.Rand((int)(10000 * (Math.Sin(coord.X) + Math.Cos(coord.Y) + seed)));
			//return Rand2(((int)coord.X << 16) + (int)coord.Y);
		}
	}
}
