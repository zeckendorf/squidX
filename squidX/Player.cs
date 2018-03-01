using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace squidX
{
    public class Player : SpriteElement
    {

        public HealthBar healthBar;
        public static Texture2D playerTexture;
        public static Texture2D[] thrustTextures = new Texture2D[5];
        public float maxVelocity = 6f;
        public float velocity = 0f;
        public byte alpha;
        float accel = .55f;
		float decel = .55f;
        public float numFlares = 0;
        float angularVelocity = .22f;
        public int playerNumber;
        public int vibCounter;
        public KeyboardState keyState;
        public KeyboardState oldKeyState;
        public GamePadState gamePadState;
        public GamePadState oldGamePadState;
        // public static Keys[] defaultKeys = new Keys[] { Keys.Up, Keys.Down, Keys.Left, Keys.Right };
        public static Color[] explosionColors = new Color[4];
        public TrailDebris trailRoot;

        //health stuff
        public int health;
        public int maxhealth = 100;
     
        //Keys - 0 up, 1 left, 2 right
        public Keys[] PCcontrols;

        public PlayerIndex playerIndex;

        public List<IPowerup> powerups = new List<IPowerup>();

        //If the controller is connected when the player is made, default to controller input so it can pause if disconnected
        public bool controllerDefaultInput = false;

        public Player(int playerNum, Vector2 loc, SquidGame game, Keys[] PCcontrols, PlayerIndex playerIndex)
            : base(playerTexture, loc, game)
        {
            
            this.playerIndex = playerIndex;
            this.PCcontrols = PCcontrols;
            health = 100;

			// healthbar location depends on player number
			if (playerNum == 0)
				healthBar = new HealthBar(new Vector2(25, 15), explosionColors[playerNum],  this);
			if (playerNum == 1)
				healthBar = new HealthBar(new Vector2(game.width - 270, 15), explosionColors[playerNum], this);
			if (playerNum == 2)
				healthBar = new HealthBar(new Vector2(25, game.height - 60), explosionColors[playerNum], this);
			if (playerNum == 3)
				healthBar = new HealthBar(new Vector2(game.width - 270, game.height - 60), explosionColors[playerNum], this);

            float healthy = SquidGame.border.Y/1.2f;
            playerNumber = playerNum;
            Vector2[] path = new Vector2[]{
                new Vector2(10, 4),
                new Vector2(10, 11),
                new Vector2(20, 11),
                new Vector2(20, 4),
                new Vector2(25, 6),
                new Vector2(25, 25),
                new Vector2(4, 25),
                new Vector2(4, 6),
                new Vector2(10, 4)};
            shape = new Shape(path);
            center = new Vector2(playerTexture.Width/2f, playerTexture.Height/2f);
            shape.transform(Matrix.CreateTranslation(new Vector3(-15, -15, 0)));
            
            gamePadState = GamePad.GetState(playerIndex);
            oldGamePadState = gamePadState;
            controllerDefaultInput = gamePadState.IsConnected;

        }

        public override void Draw(SpriteBatch s)
        {
            if (alpha < 255) alpha++;
            s.Draw(texture, location, null, new Color(explosionColors[playerNumber], alpha), angle + (float)Math.PI / 2, center, 1, SpriteEffects.None, 0);
            foreach (IPowerup pu in powerups)
                pu.drawSelected(s);
        }

        public bool thrust = false;
        float thrustIntensity;
        float thrusterBias = .5f;
        public void move()
        {
            
            location.X += velocity * (float)Math.Cos(angle);
            location.Y += velocity * (float)Math.Sin(angle);

            #region GamePad controls
            if (GamePad.GetState(playerIndex).IsConnected)
            {
                thrustIntensity = 0;
                angularVelocity = .6f;
                Vector2 stickPosition = gamePadState.ThumbSticks.Left;
                Vector2 stickPositionR = gamePadState.ThumbSticks.Right;

               
                velocity = (stickPosition.Length()) * maxVelocity;
                float dtheta = 0;
                thrust = true;
                stickPosition = new Vector2(stickPosition.X, -stickPosition.Y);
                if (stickPosition.LengthSquared() >= .064)
                {
                    dtheta = smallestBoundedAngleDifference(
                        (float)Math.Atan2(stickPosition.Y, stickPosition.X), angularVelocity);

                }
                else { velocity = 0; thrust = false; }
                angle += dtheta;



                thrustIntensity = 1f;
                thrusterBias = .5f - dtheta / angularVelocity * .5f;
            #endregion
            }
            else
            {
                #region PC PCcontrols

               

                if (keyState.IsKeyDown(PCcontrols[0]))
                {
                    thrusterBias = .5f;
                    velocity += accel;
                    if (velocity >= maxVelocity) velocity = maxVelocity;

                    thrustIntensity = 1f; //should be 1 or 2

                }
                else
                {

                    velocity -= decel;
                    if (velocity <= 0) velocity = 0;


                }
                if (keyState.IsKeyDown(PCcontrols[2]))
                {
                    angle += angularVelocity;
                    if (keyState.IsKeyDown(PCcontrols[0]))
                    {
                        thrusterBias = .1f;
                        thrustIntensity = .5f;
                    }
                    else
                    {
                        thrusterBias = 0f;
                        thrustIntensity = .25f;
                    }
                }
                else if (keyState.IsKeyDown(PCcontrols[1]))
                {
                    if (keyState.IsKeyDown(PCcontrols[0]))
                    {
                        thrusterBias = .9f;
                        thrustIntensity = 1f;
                    }
                    else
                    {
                        thrusterBias = 1f;
                        thrustIntensity = .25f;
                    }
                    angle -= angularVelocity;
                }

                if (keyState.IsKeyDown(PCcontrols[0]) || keyState.IsKeyDown(PCcontrols[2]) || keyState.IsKeyDown(PCcontrols[1])) thrust = true;
                else thrust = false;
                #endregion
            }

          
        }

        public void updateInputStates()
        {
            oldKeyState = keyState;
            oldGamePadState = gamePadState;

            //Get input states
            keyState = Keyboard.GetState();
            gamePadState = GamePad.GetState(playerIndex);

            if (controllerDefaultInput && !gamePadState.IsConnected) game.st = SquidGame.state.paused;
        }

        //PCcontrols which thruster has more thrust based on turning
        
        public override void Update()
        {
            if (vibCounter > 0)
            {
                GamePad.SetVibration(playerIndex, 1f, 1f);
                vibCounter--;
            }
            else GamePad.SetVibration(playerIndex, 0f, 0f);

            move();

            //Calculate health bar
            healthBar.health = health;
      
            //Update trails
            createTrail();


            //Update location
            if (location.X < SquidGame.border.Left+5) location.X = SquidGame.border.Left+5;
            if (location.Y < SquidGame.border.Top+5) location.Y = SquidGame.border.Top+5;
            if (location.X > SquidGame.border.Right-5) location.X = SquidGame.border.Right-5;
            if (location.Y > SquidGame.border.Bottom-5) location.Y = SquidGame.border.Bottom-5;


            //Update powerups
            for (int i = 0; i < powerups.Count(); i++)
            {
                if (powerups[i].updateSelected(this))
                {
                    powerups.RemoveAt(i);
                    i--;
                }
            }

            if (health <= 0)
                game.playersToExplode.Add(this);

            oldGamePadState = gamePadState;
            oldKeyState = keyState;
            base.Update();
        }

        public void createTrail() //creates the fiery trail behind players
        {
           if(thrust)
            {
                TrailDebris newdebris;
                Vector2 source = new Vector2();

                for (int i = 0; i < thrustIntensity; i++)
                {
                    float v = 0f + (maxVelocity / 3.5f) * (float)random.NextDouble();
                    float divergance = 3f / maxVelocity;     //PCcontrols spread of ion spray (should be 2 or 3)
                    double trailAngle = angle + random.NextDouble() * divergance - divergance / 2 + Math.PI;
                    float vx = v * (float)Math.Cos(trailAngle);
                    float vy = v * (float)Math.Sin(trailAngle);
                    if (random.NextDouble() >= thrusterBias)
                    {
                        source.X = shape.path[6].X + 4;
                        source.Y = shape.path[6].Y + 4;
                        source = Vector2.Transform(source, (CurrentTransformMatrix));
                    }
                    else
                    {
                        source.X = shape.path[5].X - 2;
                        source.Y = shape.path[5].Y + 4;
                        source = Vector2.Transform(source, CurrentTransformMatrix);
                    }
                    newdebris = new TrailDebris(explosionColors[playerNumber], source.X, source.Y, vx, vy,
                            (int)(75 - random.NextDouble() * 30), //75
                            (int)random.Next(thrustTextures.Length));
                    game.pe.add(newdebris);

                    //update the most recent particle a little
                    float randomTimeUpToOne = (float)random.NextDouble();
                    newdebris.x += randomTimeUpToOne * newdebris.vx;
                    newdebris.y += randomTimeUpToOne * newdebris.vy;

                }
            }
        }

        public class TrailDebris : ParticleEngine.Particle
        {   
            private Texture2D texture;
            private Color color;
            private float angle;

            public TrailDebris(Color c, float x, float y, float vx, float vy, int lifetime, int textureNumber)
            {
                color = c;
                this.x = x;
                this.y = y;
                this.vx = vx;
                this.vy = vy;
                this.angle =  (float)(Math.Atan2(vy, vx) + Math.PI / 2);
                this.lifetime = lifetime;
                this.texture = thrustTextures[textureNumber];
            }

            public override void Update()
            {
                bounceOffWalls();
                base.Update();
            }


            public override void Draw(SpriteBatch s)
            {
                //Fades to blue over time
                byte alpha = (byte)(200 * (lifetime - age) / lifetime);
                byte r = (byte)(color.R * (lifetime - age) / lifetime);
                byte g = (byte)(color.G * (lifetime - age) / lifetime);
                byte b = (byte)(200);
                color = new Color(r, g, b, alpha);

                s.Draw(texture,
                    new Rectangle((int)(x), (int)(y), texture.Width+4, texture.Height+5),
                    null, color, angle,
                    new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);

                
                
            }

        }

    }
}
