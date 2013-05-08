using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Perlin3D
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		SpriteFont font;

		int offset = 0;

		Texture2D canvas;
		Rectangle tracedSize;
		UInt32[] pixels;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			lowerBoundMap = GenerateNoiseMap();
			tracedSize = GraphicsDevice.PresentationParameters.Bounds;
			canvas = new Texture2D(GraphicsDevice, tracedSize.Width, tracedSize.Height, false, SurfaceFormat.Color);
			pixels = new UInt32[tracedSize.Width * tracedSize.Height];

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			font = Content.Load<SpriteFont>("font");

			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			

			

			

			base.Update(gameTime);
		}

		private Random rand = new Random(100);
		private float[,] lowerBoundMap;
		private float[,] upperBoundMap;

		private float[,] GenerateNoiseMap(int octave = 64)
		{
			int seed = rand.Next(1000);
			Perlin2D p = new Perlin2D(seed, octave);
			int xIt = (int)Math.Ceiling(800D / octave);
			int yIt = (int)Math.Ceiling(480D / octave);
			float[,] map = new float[xIt*octave, yIt*octave];
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

							map[a,b]  = noisef;

						}
					}
				}
			}
			return map;
		}

		private int frame;
		private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();


		protected override void Draw(GameTime gameTime)
		{
			frame = frame % 120;


			GraphicsDevice.Clear(Color.SkyBlue);

			GraphicsDevice.Textures[0] = null;

			sw.Start();

			if (frame == 0) {
				if (upperBoundMap != null) {
					lowerBoundMap = upperBoundMap;
				}
				upperBoundMap = GenerateNoiseMap();
			}

			
			for (int i = 0; i < 800; i++) {
				for (int j = 0; j < 480; j++) {
					float interpNoise = JMath.LinInterpolate(lowerBoundMap[i, j], upperBoundMap[i, j], frame / 120F);

					byte cloudCover = (byte)(interpNoise*255);
					if (cloudCover < 128) {
						cloudCover = 0;
					} else {
						cloudCover = (byte)((cloudCover - 128)*2);
					}
					//cloudCover = CloudExpCurve(cloudCover);

					pixels[j * 800 + i] = (uint)(cloudCover << 24) + 0x00FFFFFF;
				}
			}

			/*for (int i = 0; i < 800; i++) {
				for (int j = 0; j < 480; j++) {
					// A B G R
					byte v1 = (byte)Math.Round(p0.getNoiseLevelAtPosition(i + offset, j) * 255);
					byte v2 = (byte)Math.Round(p1.getNoiseLevelAtPosition(i + offset, j) *127);
					byte v3 = (byte)Math.Round(p2.getNoiseLevelAtPosition(i + offset, j) * 63);
					byte v4 = (byte)Math.Round(p3.getNoiseLevelAtPosition(i + offset, j) * 31);

					val = (byte)((v1+v2+v3+v4) / 1.875f);

					uint pix = 0xFF000000;
					pix += (uint)(val << 16);
					pix += (uint)(val << 8);
					pix += (uint)val;

					pixels[j * 800 + i] = pix;
					//pixels[j * 800 + i] = (uint)(0xFF000000 + (val << 4) + (val << 2) + val);
				}
			}*/

			float calctime = sw.ElapsedTicks / (float)System.Diagnostics.Stopwatch.Frequency;
			sw.Reset();
			offset++;

			canvas.SetData<UInt32>(pixels, 0, tracedSize.Width * tracedSize.Height);

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			spriteBatch.Draw(canvas, new Rectangle(0, 0, tracedSize.Width, tracedSize.Height), Color.White);
			spriteBatch.DrawString(font, "Calculation time: " + (int) (calctime * 1000) + "ms", new Vector2(25, 25), Color.Red);
			spriteBatch.End();

			frame++;
			base.Draw(gameTime);
		}
	}
}
