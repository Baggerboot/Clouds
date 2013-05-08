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

		CloudLayer alpha = new CloudLayer(64, 48, 800, 480, 1001);
		CloudLayer beta = new CloudLayer(32, 24, 800, 480, 564);
		CloudLayer gamma = new CloudLayer(16, 12, 800, 480, 423);
		CloudLayer delta = new CloudLayer(8, 6, 800, 480, 134);
		CloudLayer epsilon = new CloudLayer(4, 3, 800, 480, 482);

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

			tracedSize = GraphicsDevice.PresentationParameters.Bounds;
			canvas = new Texture2D(GraphicsDevice, tracedSize.Width, tracedSize.Height, false, SurfaceFormat.Color);
			pixels = new UInt32[tracedSize.Width * tracedSize.Height];

			Console.WriteLine("Precalculating frames...");
			alpha.Precalculate(1);
			Console.WriteLine("Alpha.");
			beta.Precalculate(1);
			Console.WriteLine("Beta.");
			gamma.Precalculate(1);
			Console.WriteLine("Gamma.");
			delta.Precalculate(1);
			Console.WriteLine("Delta.");
			epsilon.Precalculate(1);
			Console.WriteLine("Epsilon.");
			Console.WriteLine("Done.");

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

		private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();


		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.DeepSkyBlue);

			GraphicsDevice.Textures[0] = null;

			sw.Start();
			alpha.Update();
			beta.Update();
			gamma.Update();
			delta.Update();
			epsilon.Update();
			for (int i = 0; i < 800; i++) {
				for (int j = 0; j < 480; j++) {

					float total = alpha.CurrentNoiseMap[i, j] + 0.5F * beta.CurrentNoiseMap[i, j] + 0.25F * gamma.CurrentNoiseMap[i,j] + 0.125F * delta.CurrentNoiseMap[i,j] + 0.06F * epsilon.CurrentNoiseMap[i,j];

					byte cloudCover = (byte)(total/1.935F*255);
					if (cloudCover < 128) {
						cloudCover = 0;
					} else {
						cloudCover = (byte)((cloudCover - 128)*2);
					}
					//cloudCover = CloudExpCurve(cloudCover);

					pixels[j * 800 + i] = (uint)(cloudCover << 24) + 0x00FFFFFF;
				}
			}
			float calctime = sw.ElapsedTicks / (float)System.Diagnostics.Stopwatch.Frequency;
			sw.Reset();

			canvas.SetData<UInt32>(pixels, 0, tracedSize.Width * tracedSize.Height);

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			spriteBatch.Draw(canvas, new Rectangle(0, 0, tracedSize.Width, tracedSize.Height), Color.White);
			spriteBatch.DrawString(font, "Calculation time: " + (int) (calctime * 1000) + "ms", new Vector2(25, 25), Color.Red);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
