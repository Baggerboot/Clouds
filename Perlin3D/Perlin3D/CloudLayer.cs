		using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perlin3D
{
	class CloudLayer
	{
		private Random rand = new Random(100);
		private float[,] lowerBoundMap;
		private float[,] upperBoundMap;
		private int octave;
		private int ttl;
		private int frame;
		private int totalFrame;
		private int width, height;
		public float[,] CurrentNoiseMap { get; private set; }

		private bool precalculate;

		private List<float[,]> precalculatedFrames = new List<float[,]>();

		public CloudLayer(int octave, int ttl, int width, int height, int seed)
		{
			this.octave = octave;
			this.ttl = ttl;
			this.width = width;
			this.height = height;
			upperBoundMap = GenerateNoiseMap();
			rand = new Random(seed);
		}

		public void Precalculate(int KeyFrameCount)
		{
			float[,] firstMap = upperBoundMap;

			for (int i = 0; i < KeyFrameCount-1; i++) {
				GenerateNoiseMap();
				for (int j = 0; j < ttl; j++) {
					Update();
					precalculatedFrames.Add(CurrentNoiseMap);
				}
			}
			for (int i = 0; i < ttl; i++) {
				Update(firstMap);
				precalculatedFrames.Add(CurrentNoiseMap);
			}

			precalculate = true;
		}

		public void Update(float[,] noiseMap = null)
		{
			frame = frame % ttl;
			if (precalculate) {
				if (totalFrame == precalculatedFrames.Count) totalFrame = 0;
				CurrentNoiseMap = precalculatedFrames[totalFrame];
			}else{
				if (frame == 0) {
					lowerBoundMap = upperBoundMap;
					upperBoundMap = noiseMap == null ? GenerateNoiseMap() : noiseMap;
				}
				CurrentNoiseMap = new float[width, height];
				for (int i = 0; i < width; i++) {
					for (int j = 0; j < height; j++) {
						CurrentNoiseMap[i, j] = JMath.LinInterpolate(lowerBoundMap[i, j], upperBoundMap[i, j], frame / (float)ttl);
					}
				}
			}
			frame++;
			totalFrame++;
		}

		private float[,] GenerateNoiseMap()
		{
			if (precalculate) {
				return precalculatedFrames[totalFrame];
			}
			int seed = rand.Next(1000);
			Perlin2D p = new Perlin2D(seed, octave);
			int xIt = (int)Math.Ceiling(800D / octave);
			int yIt = (int)Math.Ceiling(480D / octave);
			float[,] map = new float[xIt * octave, yIt * octave];
			for (int x = 0; x < xIt; x++) {
				for (int y = 0; y < yIt; y++) {

					float[,] noises = p.getNoiseLevelsAtOctave(x, y);
					for (int i = 0; i < octave; i++) {
						for (int j = 0; j < octave; j++) {
							float noisef = (noises[i, j]);

							byte noise = (byte)(noisef * 255);

							uint pix = 0xFF000000;
							pix += (uint)(noise << 16);
							pix += (uint)(noise << 8);
							pix += (uint)noise;

							int a = x * octave + i;
							int b = y * octave + j;

							map[a, b] = noisef;

						}
					}
				}
			}
			return map;
		}
	}
}
