using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace squidX
{
	/// <summary>
	/// This is the main type for your game
	/// 
	/// </summary>


	public class SquidGame : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D linePixel;

		// global parameters for the game
		public int width = 1280;
		public int height = 720;
		public int score = 0;
		public int highScore = 0;
		public int scoreMultiplier = 1;
		public int level = 1;
		public int numPlayers = 2;
		public bool fullscreen = true;
		public bool showFPS = true;
		public bool showBoundingShapes = false;
		public int introCounter = 0;
		private static bool renderSpringGrid = false;

		// global shared values for the game
		public static Random randy = new Random(459364); // random number generator
		public enum state { splash, menu, intro, running, paused, gameOver };
		public static SpriteFont font;
		public static SpriteFont fontBig;
		public static SpriteFont fontSmall;
		public static string scoresfile = "squidscores";

		// game state definition
		public state st = state.menu;
		public enum GameType { singlePlayer, multiPlayerCoop };
		public GameType gameType = GameType.singlePlayer;

		// data structures for maintaining game elements
		public List<Player> players = new List<Player>();
		public List<Enemy> enemies = new List<Enemy>();
		public List<Bullet> bullets = new List<Bullet>();
		public List<SpriteElement> targets = new List<SpriteElement>();
		public List<Player> playersToExplode = new List<Player>();
		public List<Bullet> bulletsToRemove = new List<Bullet>();
		public List<Enemy> enemiesToExplode = new List<Enemy>();
		public List<SpriteElement> everything = new List<SpriteElement>();
		private List<IPowerup> unselectedPowerups = new List<IPowerup>();

		// params for game area
		public static Rectangle border;
		public Texture2D borderCorner;
		public Texture2D borderLine;
		public static Camera2D cam;
		public static float cameraZ = 250f;
		public ParticleEngine pe;

		// obj
		KeyboardState old;
		KeyboardState news;
		GraphicsDevice device;
		public Color fontColor;
		public string FramesPerSecond = "0";
		public Starfield stars;
		public static string[] gametypes = new string[] { "Squid" };
		public List<int> topTenScores = new List<int>();
		public SpringGrid springGrid;


		//media
		public static Song themeSong;
		public static Song menuSong;
		//Video introvid;
		//VideoPlayer videoPlayer;

		public static Keys[][] playerKeyControls =
			{
				new Keys[] {Keys.Up, Keys.Left, Keys.Right, Keys.Q, Keys.E}, //5 and 6 are right analog left and right
                new Keys[] {Keys.W, Keys.A, Keys.D, Keys.Delete, Keys.PageDown},
				new Keys[] {Keys.I, Keys.J, Keys.L, Keys.Delete, Keys.PageDown},
				new Keys[] {Keys.G, Keys.V, Keys.N, Keys.Delete, Keys.PageDown}
			};

		List<Text> hiscoreText = new List<Text>();
		/* Filter what highscores are visible. -1 means all types.*/

		int gametypeFilter = -1;

		public SquidGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			Window.Title = "Squid";

			//videoPlayer = new VideoPlayer();
			//savegame stuff
			//store = Highscores2.Storage.Attach(this, true);
			/*
			if (!store.StartLoading())
			{
				store.ReselectStorage();
			}*/

			Menu.g = this;
			Menu.create();

		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			device = graphics.GraphicsDevice;
			PresentationParameters presentParams = device.PresentationParameters;

			graphics.PreferMultiSampling = true;
			//graphics.GraphicsProfile     = GraphicsProfile.HiDef;
			device.PresentationParameters.MultiSampleCount = 4;
			device.BlendState = BlendState.NonPremultiplied;
			device.DepthStencilState = DepthStencilState.DepthRead;


			if (fullscreen)
			{
				this.graphics.PreferredBackBufferWidth = width;
				this.graphics.PreferredBackBufferHeight = height;
				this.graphics.IsFullScreen = true;
				//presentParams.SwapEffect = SwapEffect.Flip;
			}
			else
			{
				this.graphics.PreferredBackBufferWidth = width;
				this.graphics.PreferredBackBufferHeight = height;
				this.graphics.IsFullScreen = false;
				//presentParams.SwapEffect = SwapEffect.Discard;
			}


			graphics.ApplyChanges();
			old = Keyboard.GetState();

			if (GraphicsDevice.DisplayMode.Width > 700)
				border = new Rectangle(device.Viewport.Width / 10, device.Viewport.Height / 10,
					device.Viewport.Width * 8 / 10, device.Viewport.Height * 8 / 10);
			else border = new Rectangle(-device.Viewport.Width / 8, -device.Viewport.Height / 8, 9 * device.Viewport.Width / 8, 9 * device.Viewport.Height / 8);

			pe = new ParticleEngine(this);
			//springGrid = new SpringGrid(this);
			cam = new Camera2D(this);

			fontColor = Color.WhiteSmoke * .62f;


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

			MediaPlayer.IsRepeating = true;
			//introvid = Content.Load<Video>("intro");
			font = Content.Load<SpriteFont>("fontRegular");
			fontBig = Content.Load<SpriteFont>("fontBig");
			fontSmall = Content.Load<SpriteFont>("fontSmall");
			Menu.fontMenu = Content.Load<SpriteFont>("fontMenu");

			Player.playerTexture = Content.Load<Texture2D>("player");
			HealthBar.hTexture = Content.Load<Texture2D>("flare");

			Starfield.starTextures = new Texture2D[1];
			Starfield.starTextures[0] = Content.Load<Texture2D>("star glow");

			MainMenu.gameLogo = Content.Load<Texture2D>("logo symbol transparent");
			MainMenu.screenLine = Content.Load<Texture2D>("horizontal screen line");
			MainMenu.screenLineDark = Content.Load<Texture2D>("horizontal screen line dark");
			MainMenu.shortLine = Content.Load<Texture2D>("shortLine");
			MainMenu.shortLineDark = Content.Load<Texture2D>("shortLineDark");
			MainMenu.bottomOptionOutline = Content.Load<Texture2D>("bottomOutline");
			MainMenu.bottomOptionOutlineDark = Content.Load<Texture2D>("bottomOutlineDark");
			MainMenu.tinyLine = Content.Load<Texture2D>("tinyLine");
			MainMenu.tinyLineDark = Content.Load<Texture2D>("tinyLineDark");
			MainMenu.halfTinyLine = Content.Load<Texture2D>("tinyHalfLine");
			MainMenu.halfTinyLineDark = Content.Load<Texture2D>("tinyHalfLineDark");
			MainMenu.blueBlocker = Content.Load<Texture2D>("blueblocker");
			MainMenu.InstructionsScreen.instructions = Content.Load<Texture2D>("instruction");
			MainMenu.CreditsScreen.credits = Content.Load<Texture2D>("credits");
			MainMenu.MultiplayerMenu.bigGrayPlayerImage = Content.Load<Texture2D>("grayMPship");
			Menu.sMenu = Content.Load<SoundEffect>("menuOver");

			StarEnemy.t = Content.Load<Texture2D>("star enemy");
			StarEnemy.bTexture = Content.Load<Texture2D>("star bullet");
			CircleEnemy.t = Content.Load<Texture2D>("Circle Enemy");
			CircleEnemy.bTexture = Content.Load<Texture2D>("Circle Bullet");
			DuoEnemy.t = Content.Load<Texture2D>("Duo Enemy");
			DuoEnemy.bTexture = Content.Load<Texture2D>("Duo Bullet");
			BlueShip.t = Content.Load<Texture2D>("blueship");
			BlueShip.bTexture = Content.Load<Texture2D>("bluebullet");
			TriangleShip.t = Content.Load<Texture2D>("TriangleEnemy");
			TriangleShip.bTexture = Content.Load<Texture2D>("TriangleBullet");
			TurretEnemy.t = Content.Load<Texture2D>("enemy");
			TurretEnemy.bTexture = Content.Load<Texture2D>("turretbullet");
			WheelEnemy.t = Content.Load<Texture2D>("WheelEnemy");
			WheelEnemy.bTexture = Content.Load<Texture2D>("WheelBullet");
			WheelEnemy.enteringTexture = Content.Load<Texture2D>("WheelEnemyRing");
			ParticleEngine.ExplosionTexture = Content.Load<Texture2D>("explosion line");
			Player.explosionColors[0] = new Color(255, 255, 60);
			Player.explosionColors[1] = new Color(215, 90, 185);
			Player.explosionColors[2] = new Color(140, 220, 240);
			Player.explosionColors[3] = new Color(220, 170, 140);

			Player.thrustTextures[0] = Content.Load<Texture2D>("thrust textures/thrust turbulence 1");
			Player.thrustTextures[1] = Content.Load<Texture2D>("thrust textures/thrust turbulence 2");
			Player.thrustTextures[2] = Content.Load<Texture2D>("thrust textures/thrust turbulence 3");
			Player.thrustTextures[3] = Content.Load<Texture2D>("thrust textures/thrust turbulence 4");
			Player.thrustTextures[4] = Content.Load<Texture2D>("thrust textures/thrust turbulence 5");

			HealthPU.healthTexture = Content.Load<Texture2D>("Health powerup");
			DeflectorPU.UnselectedTexture = Content.Load<Texture2D>("Magnet powerup");
			PowerParticulate.healthParticleTexture = Content.Load<Texture2D>("particle");
			DeflectorPU.DeflectorTexture = Content.Load<Texture2D>("Energy Deflector");
			FlarePU.UnselectedFlareTexture = Content.Load<Texture2D>("Flare powerup");
			Flare.FlareMissile = Content.Load<Texture2D>("flare");
			ParticleEngine.particle6.glowyLine = Content.Load<Texture2D>("particle");
			themeSong = Content.Load<Song>("squid 411");
			menuSong = Content.Load<Song>("menu");

			stars = new Starfield(.0015f, device.Viewport.Width, device.Viewport.Height);

			borderCorner = Content.Load<Texture2D>("border");


			//Line texture
			linePixel = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
			Color[] pd = { Color.White };
			linePixel.SetData<Color>(pd);


			for (int i = 0; i < 10; i++)
				topTenScores.Add(0);

			//videoPlayer.Play(introvid);
	

		}

		private void clearScore()
		{
			//store.ClearLoaded();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>

		bool once = true;
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__ && !__TVOS__
						if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
							Exit();
#endif

			if (once)
			{
				loadScore();
				once = false;
				MediaPlayer.Stop();
				MediaPlayer.Play(menuSong);
			}



			stars.Update();
			//if (renderSpringGrid) springGrid.Update();

			//Update input
			old = news;
			news = Keyboard.GetState();
			foreach (Player p in players) p.updateInputStates();

			if (news.IsKeyDown(Keys.Escape)) this.Exit();
			if (news.IsKeyDown(Keys.C)) this.clearScore();
	

			if (news.IsKeyDown(Keys.P) && old.IsKeyUp(Keys.P))
			{
				if (st == state.running) st = state.paused;
				else if (st == state.paused)
				{
					InGameMenu.displayInstructions = false;
					st = state.running;
				}
			}

			foreach (Player p in players)
			{
				if (p.gamePadState.IsButtonDown(Buttons.A)) Console.WriteLine("A");
				if (p.gamePadState.IsButtonDown(Buttons.B)) Console.WriteLine("B");
				if (p.gamePadState.IsButtonDown(Buttons.X)) Console.WriteLine("X");
				if (p.gamePadState.IsButtonDown(Buttons.Y)) Console.WriteLine("Y");
				if (p.gamePadState.IsButtonDown(Buttons.Back)) Console.WriteLine("back");
				if (p.gamePadState.IsButtonDown(Buttons.Start)) Console.WriteLine("start");
				if (p.gamePadState.IsButtonDown(Buttons.LeftShoulder)) Console.WriteLine("ls");
				if (p.gamePadState.IsButtonDown(Buttons.RightShoulder)) Console.WriteLine("rs");

				if (p.gamePadState.IsButtonDown(Buttons.Start) && p.oldGamePadState.IsButtonUp(Buttons.Start))
					if (st == state.running) st = state.paused;
					else if (st == state.paused)
					{
						InGameMenu.displayInstructions = false;
						st = state.running;
					}
			}


			if (st == state.running || st == state.gameOver || st == state.intro)
			{
				//Update game elements
				foreach (SpriteElement p in enemies)
					everything.Add(p);
				foreach (SpriteElement e in players)
					everything.Add(e);
				#region levels
				if (enemies.Count == 0 && st == state.running)
				{


					if (level <= 3)
						for (int i = 0; i < level; i++)
							enemies.Add(new BlueShip(this));


					else if (level == 4)
					{
						enemies.Add(new BlueShip(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new TurretEnemy(this));

					}

					else if (level == 5)
					{
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new StarEnemy(this));
					}

					else if (level == 6)
					{

						enemies.Add(new WheelEnemy(this));
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new CircleEnemy(this));
					}
					else if (level == 7)
					{

						enemies.Add(new CircleEnemy(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new WheelEnemy(this));
					}
					else if (level == 8)
					{
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new DuoEnemy(this));
					}
					else if (level == 9)
					{

						enemies.Add(new BlueShip(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new WheelEnemy(this));
					}
					else if (level == 10)
					{

						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TriangleShip(this));

					}
					else if (level == 11)
					{
						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));

					}
					else if (level == 12)
					{
						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new DuoEnemy(this));

					}
					else if (level == 13)
					{
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TurretEnemy(this));
					}
					else if (level == 14)
					{
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new DuoEnemy(this));
					}
					else if (level == 15)
					{

						enemies.Add(new BlueShip(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new DuoEnemy(this));

					}
					else if (level == 16)
					{

						enemies.Add(new BlueShip(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));

					}

					else if (level == 17)
					{

						enemies.Add(new DuoEnemy(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));


					}
					else if (level == 18)
					{

						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new CircleEnemy(this));

					}
					else if (level == 19)
					{

						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new WheelEnemy(this));

					}
					else if (level == 20)
					{

						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TurretEnemy(this));
					}
					else if (level == 21)
					{

						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
					}
					else if (level == 22)
					{

						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new BlueShip(this));
						enemies.Add(new TurretEnemy(this));
						enemies.Add(new TurretEnemy(this));
					}
					else if (level >= 23)
					{

						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new TriangleShip(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new CircleEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new StarEnemy(this));
						enemies.Add(new WheelEnemy(this));
						enemies.Add(new DuoEnemy(this));
						enemies.Add(new DuoEnemy(this));
					}
					level++;
				}
				#endregion


				foreach (Player p in players)
				{
					if (st == state.running) p.Update();
				}
				foreach (Enemy p in enemies)
					p.Update();
				foreach (Bullet p in bullets)
				{
					p.Update();

				}

				for (int i = 0; i < unselectedPowerups.Count(); i++)
					if (unselectedPowerups[i].updateUnselected())
					{
						unselectedPowerups.RemoveAt(i);
						i--;
					}

				foreach (Player p in playersToExplode)
				{
					//pe.createExplosion(p.location, 22f, Player.explosionColors[p.playerNumber], 900,300);
					pe.createExplosion(p.location, 12f, Player.explosionColors[p.playerNumber], 500, 190);
					players.Remove(p);
					targets.Remove(p);
					if (players.Count == 0)
					{
						st = state.gameOver;
						targets.Clear();
						saveScore();
					}

				}
				foreach (Enemy e in enemiesToExplode)
				{
					pe.createExplosion(e.location, 12f, e.explosionColor, 200, 200);
					enemies.Remove(e);
					//unselectedPowerups.Add(new DeflectorPU(e.location,this));
					int rand = randy.Next(0, 18);
					if (rand < 4)
						dropPowerUp(e.location);
				}
				foreach (Bullet b in bulletsToRemove)
				{
					pe.createExplosion(b.location, 8f, b.explosionColor, 40, 190);
					bullets.Remove(b);
				}

				playersToExplode.Clear();
				enemiesToExplode.Clear();
				bulletsToRemove.Clear();


				if (highScore < score) highScore = score;
			}

			if (st != state.paused) pe.update();

			//Update the camera
			#region camera
			Vector2 focus = new Vector2(device.Viewport.Width / 2, device.Viewport.Height / 2);//new Vector2(device.Viewport.Width/2,device.Viewport.Height/2);
			Vector2 longest = Vector2.Zero;
			Vector2 t1 = Vector2.Zero;
			Vector2 t2 = Vector2.Zero;
			if (players.Count > 0)
			{
				foreach (Player p in players)
				{
					foreach (SpriteElement e in everything)
					{
						Vector2 temp = p.location - e.location;
						if (temp.LengthSquared() > longest.LengthSquared())
						{
							longest = temp;

							t1 = p.location;
							t2 = e.location;

						}

					}
				}
				foreach (SpriteElement p in everything)
				{
					foreach (SpriteElement e in everything)
					{
						Vector2 temp = p.location - e.location;
						if (temp.LengthSquared() > longest.LengthSquared())
						{
							longest = temp; //find longest distance for zoom

						}

					}
				}


				{
					focus = Vector2.Zero;
					foreach (Player p in players)
						focus += p.location;

					focus /= players.Count;
				}

			}

			if (players.Count > 1)
				cam.scaleTarget = 1f;
			else cam.scaleTarget = .04f + .6f * Math.Min(Math.Abs((float)device.Viewport.Width / (longest.X + .01f)), Math.Abs((float)device.Viewport.Height / (longest.Y + .01f)));


			cam.Focus2 = focus;
			cam.Update(gameTime);


			everything.Clear();
			#endregion


			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>

		float deltaFPSTime = 0;
		int counter = 0;    //Counts. Has nothing to do with the frames each second
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, cam.Transform);

		
			if (st == state.splash)
			{


				Rectangle screen = new Rectangle(0,
					0,
					device.Viewport.Width,
					device.Viewport.Height);

				// Draw the video, if we have a texture to draw.


				//spriteBatch.Draw(videoPlayer.GetTexture(), screen, Color.White);
			}
			else
			{
				if (st == state.menu)
				{
					Menu.updateDrawMainMenu(spriteBatch);
				}

				else if (st == state.gameOver)
				{
					Menu.updateDrawGameOverMenu(spriteBatch);
					spriteBatch.DrawString(font, "Score: " + score, new Vector2(Menu.g.width / 2 - 100, Menu.g.height / 3 + (int)(1.5f*Menu.fontMenu.LineSpacing)), Color.Red);
					if (counter == 100)
					{
						enemiesToExplode.AddRange(enemies);
						bulletsToRemove.AddRange(bullets);
						unselectedPowerups.Clear();
						GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
						GamePad.SetVibration(PlayerIndex.Two, 0f, 0f);
						GamePad.SetVibration(PlayerIndex.Three, 0f, 0f);
						GamePad.SetVibration(PlayerIndex.Four, 0f, 0f);
					}
					counter++;
				}
				if (st == state.running || st == state.gameOver || st == state.intro || st == state.paused)
				{

			
					if (st == state.intro)
					{



						if (introCounter < 110)
						{
							foreach (Player p in players)
								pe.createPlayerIntro(p.location);

							introCounter++;
						}
						else
						{

							st = state.running;
							introCounter = 0;
						}

					}

					//draw border
					Vector2[] borderVertices = new Vector2[]{
					new Vector2(border.X - borderCorner.Width / 2+24 , border.Y - borderCorner.Height / 2+24),
					new Vector2(border.X - borderCorner.Width / 2+24, border.Bottom - borderCorner.Height / 2+24),
					new Vector2(border.Right - borderCorner.Width / 2+24, border.Bottom - borderCorner.Height / 2+24),
					new Vector2(border.Right - borderCorner.Width / 2+24 , border.Y - borderCorner.Height / 2+24),
					new Vector2(border.X - borderCorner.Width / 2 +24, border.Y - borderCorner.Height / 2+24)
				};
					spriteBatch.Draw(borderCorner, new Rectangle((int)borderVertices[0].X - 24, (int)borderVertices[0].Y - 24, borderCorner.Width, borderCorner.Height), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipVertically, 0);
					spriteBatch.Draw(borderCorner, new Rectangle((int)borderVertices[1].X - 24, (int)borderVertices[1].Y - 24, borderCorner.Width, borderCorner.Height), Color.White);
					spriteBatch.Draw(borderCorner, new Rectangle((int)borderVertices[3].X - 24, (int)borderVertices[3].Y - 24, borderCorner.Width, borderCorner.Height), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0);
					spriteBatch.Draw(borderCorner, new Rectangle((int)borderVertices[2].X - 24, (int)borderVertices[2].Y - 24, borderCorner.Width, borderCorner.Height), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);

					DrawLine(spriteBatch, borderVertices, new Color(Color.WhiteSmoke, 20));
					if (showBoundingShapes)
					{
						foreach (Player p in players) p.drawShape(spriteBatch);
						foreach (Enemy en in enemies) en.drawShape(spriteBatch);
						foreach (SpriteElement b in bullets) b.drawShape(spriteBatch);
					}

					foreach (IPowerup pu in unselectedPowerups) pu.drawUnselected(spriteBatch);

					foreach (Player p in players)
						p.Draw(spriteBatch);        //Draw Players
					foreach (Enemy y in enemies)
						y.Draw(spriteBatch);        //Draw Enemies
					foreach (Bullet b in bullets)
						b.Draw(spriteBatch);         //Draw Bullets

					stars.Draw(spriteBatch);        //Draw Stars
					pe.Draw(spriteBatch);           //Draw Particles
					if (renderSpringGrid) springGrid.Draw(spriteBatch); // draw spring
				}


				spriteBatch.End();
				spriteBatch.Begin(); //HUD
									 //healthbars

				if (st == state.paused)
					Menu.updateDrawInGameMenu(spriteBatch);
				
				if (st == state.running || st == state.intro || st == state.paused)
				{
					foreach (Player p in players)
						p.healthBar.Draw(spriteBatch);


					#region FPS counter
					float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

					float fps = 1.0f / elapsed;
					deltaFPSTime += elapsed;
					if (deltaFPSTime > 1)
					{

						FramesPerSecond = "" + fps.ToString().Substring(0, 4);
						deltaFPSTime -= 1;
					}
					if (showFPS)
						spriteBatch.DrawString(font, "FPS: " +
							  FramesPerSecond, new Vector2(20, 20), Color.White);

					#endregion
					//scoredisplay
					spriteBatch.DrawString(font, "Score: " + score, new Vector2(60, 60), fontColor);
					//spriteBatch.DrawString(font, "HighScore: " + highScore, new Vector2(width - 400, 60), fontColor);
					spriteBatch.DrawString(font, "Multiplier: x" + scoreMultiplier, new Vector2(60, height - 20), fontColor);
					//endscoredisplay


					if (InGameMenu.displayInstructions)
					{
						spriteBatch.Draw(linePixel, new Rectangle(0, 0, width, height), new Color(5, 5, 40, 210));
						spriteBatch.Draw(MainMenu.InstructionsScreen.instructions, new Vector2(0, 45), Color.White);
						spriteBatch.DrawString(fontBig, "Instructions", new Vector2(400, 100), Color.LightGray);
					}
				}
			}
			spriteBatch.End();

			base.Draw(gameTime);
		}

		public void saveScore()
		{
			// add highest score and remove next bottom one
			topTenScores.Add(highScore);
			topTenScores.Sort();
			topTenScores.Reverse();
			if (topTenScores.Count > 10)
			{
				topTenScores.RemoveAt(topTenScores.Count - 1);
			}

			var store = IsolatedStorageFile.GetUserStoreForDomain();
			if (store.FileExists(scoresfile))
			{
				var fs = store.OpenFile(scoresfile, FileMode.Open);
				using (StreamWriter sw = new StreamWriter(fs))
				{
					
					sw.Write(string.Join(",",topTenScores.ToArray()));
				}
			}
		}

		public void loadScore()
		{

			var store = IsolatedStorageFile.GetUserStoreForDomain();

			if (store.FileExists(scoresfile))
			{
				var fs = store.OpenFile(scoresfile, FileMode.Open);
				using (StreamReader sr = new StreamReader(fs))
				{
					// parse string as comma delimited set of ints	
					topTenScores = sr.ReadLine().Split(',').Select(Int32.Parse).ToList();
					highScore = topTenScores.First();
				}
			}
			else
			{
				var fs = store.CreateFile(scoresfile);
				using (StreamWriter sw = new StreamWriter(fs))
				{
					sw.Write("0");
				}
                highScore = 0;
			}
			/*
			if (once && store.HasLoaded)
			{
				Highscores2.Highscore[] scores = store.QueryHighscores(null,
							gametypeFilter == -1 ? null : gametypes[gametypeFilter], null, null, 20);
				hiscoreText.Add(new Text(new Vector2(200, 60), gametypeFilter == -1 ? "All Highscores"
				  : gametypes[gametypeFilter] + " Highscores"));

				once = false;

				if (scores.Length > 0)
				{
					highScore = scores[0].Score;
					for (int i = 0; i < scores.Length && i < 10; i++)
						topTenScores[i] = scores[i].Score;

				}


			}*/
		}

		private void dropPowerUp(Vector2 location)
		{

			int random = randy.Next(0, 7);
			if (random < 2)
			{
				unselectedPowerups.Add(new HealthPU(location, this));
				return;
			}
			if (random < 5)
			{
				unselectedPowerups.Add(new FlarePU(location, this));
				return;
			}

			unselectedPowerups.Add(new DeflectorPU(location, this));
			return;

		}


		#region Extra Drawing Methods
		public void DrawLine(SpriteBatch s, float x, float y, float x2, float y2, Color c)
		{
			// create vector from coordinates
			float xd = x2 - x;
			float yd = y2 - y;
			Vector2 Line = new Vector2(xd, yd);

			// bastardize the draw method to stretch the pixel into the dimensinos of the vector
			s.Draw(linePixel, new Rectangle((int)x, (int)y, (int)Line.Length(), (int)2), null, c, (float)Math.Atan2(Line.Y, Line.X), Vector2.Zero, SpriteEffects.None, 0);

		}

		public void DrawLine(SpriteBatch s, Vector2[] locs, Color c)
		{

			// go through a list of points (such as a boundary) and use the previously defined drawline method
			for (int i = 0; i < locs.Length - 1; i++)
			{
				float xd = locs[i + 1].X - locs[i].X;
				float yd = locs[i + 1].Y - locs[i].Y;
				Vector2 Line = new Vector2(xd, yd);
				DrawLine(s, locs[i].X, locs[i].Y, locs[i + 1].X, locs[i + 1].Y, c);

			}
		}

		public void DrawCircle(SpriteBatch s, Vector2 loc, float r, int segments, Color c)
		{
			float x1 = loc.X + r;   //Starting x
			float y1 = loc.Y;       //Starting y
			float x2, y2;
			float dtheta = MathHelper.TwoPi / segments; //Change in theta with each segment
			for (int i = 0; i < segments; i++)
			{
				float angle = dtheta * (i + 1);
				x2 = r * (float)Math.Cos(angle) + loc.X;
				y2 = r * (float)Math.Sin(angle) + loc.Y;
				DrawLine(s, x1, y1, x2, y2, c);
				x1 = x2;
				y1 = y2;
			}
		}

		#endregion

		#region cameraFuncs
		public void clearEverythingExceptCamera()
		{
			players.Clear();
			enemies.Clear();
			bullets.Clear();
			targets.Clear();
			unselectedPowerups.Clear();
			introCounter = 0;
			level = 1;
			counter = 0;
			score = 0;
			MediaPlayer.Stop();
			MediaPlayer.Play(menuSong);
		}

		public void resetCamera()
		{
			cam = new Camera2D(this);
			cameraZ = 250f;
		}
		#endregion

	}

	public class Camera2D
	{
		private Vector2 _position;

		protected float _viewportHeight;
		protected float _viewportWidth;
		public float scaleTarget = 1;
		public Camera2D(Game game)
		{
			_viewportWidth = game.GraphicsDevice.Viewport.Width;
			_viewportHeight = game.GraphicsDevice.Viewport.Height;
			ScreenCenter = new Vector2(_viewportWidth / 2, _viewportHeight / 2);
			Scale = 1f;
			MoveSpeed = 1.1f;
			Position = ScreenCenter;
			Focus = ScreenCenter;
			Focus2 = Focus;
		}
		#region CamProperties
		public Vector2 Position
		{
			get { return _position; }
			set { _position = value; }
		}

		public float Rotation { get; set; }
		public Vector2 Origin { get; set; }
		public float Scale { get; set; }
		public Vector2 ScreenCenter { get; protected set; }
		public Matrix Transform { get; set; }
		public Vector2 Focus;
		public Vector2 Focus2;
		public float MoveSpeed { get; set; }
		#endregion

		//spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.SaveState, cam.Position.Transform);
		public void Update(GameTime gameTime)
		{
			//Create the Transform used by any
			//spritebatch process
			if (scaleTarget > 1) scaleTarget = 1;
			else if (scaleTarget < .6) scaleTarget = .6f;
			if (Scale < scaleTarget)
				Scale += .022f * (scaleTarget - Scale) / scaleTarget;
			else if (Scale > scaleTarget)
				Scale += .022f * (scaleTarget - Scale) / scaleTarget;

			if (Focus.X < Focus2.X)
				Focus.X += .03f * (Focus2.X - Focus.X);/// Focus2.X;
			if (Focus.X > Focus2.X)
				Focus.X += .03f * (Focus2.X - Focus.X);/// Focus2.X;
			if (Focus.Y < Focus2.Y)
				Focus.Y += .03f * (Focus2.Y - Focus.Y);// / Focus2.Y;
			if (Focus.Y > Focus2.Y)
				Focus.Y += .03f * (Focus2.Y - Focus.Y);// / Focus2.Y;
													   //else if (scaleTarget < 1) scaleTarget = 1;

			Transform = Matrix.Identity *
						Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
						Matrix.CreateRotationZ(Rotation) *
						Matrix.CreateTranslation(Origin.X, Origin.Y, 0) *
						Matrix.CreateScale(new Vector3(Scale, Scale, Scale));

			Origin = ScreenCenter / Scale;

			// Move the Camera to the position that it needs to go
			var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

			_position += (Focus - Position) * MoveSpeed * delta;

		}

		/// <summary>
		/// Determines whether the target is in view given the specified position.
		/// This can be used to increase performance by not drawing objects
		/// directly in the viewport
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="texture">The texture.</param>
		/// <returns>
		///     <c>true</c> if [is in view] [the specified position]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsInView(Vector2 position, Texture2D texture)
		{
			// If the object is not within the horizontal bounds of the screen

			if ((position.X + texture.Width) < (Position.X - Origin.X) || (position.X) > (Position.X + Origin.X))
				return false;

			// If the object is not within the vertical bounds of the screen
			if ((position.Y + texture.Height) < (Position.Y - Origin.Y) || (position.Y) > (Position.Y + Origin.Y))
				return false;

			// In View
			return true;
		}

	}
	public class Text
	{
		public Text(Vector2 p, string s)
		{
			Pos = p;
			Str = s;
		}
		public Color Color = Color.White;
		public Vector2 Pos;
		public string Str;
	}

}


