using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace squidX
{
    public class WheelEnemy: Enemy
    {
        static public Texture2D t;
        static public Texture2D bTexture;
        static public Texture2D enteringTexture;
        static Color bulletColor = new Color(75, 255, 75);
        Vector2 velocity;

        //Holds bullets before they are released so they can be updated and drawn by this wheel enemy
        private List<Bullet> bullets = new List<Bullet>();

		public WheelEnemy(SquidGame g)
            : base(t, 18, g)
        {
            explosionColor = new Color(70, 255, 70);

            #region path
            Vector2[] path = new Vector2[61];
            for (int i = 0, c = 0; i <= 360; i += 6)
            {
                path[c] = new Vector2(radius * (float)Math.Cos(i * Math.PI / 180) + 24, radius * (float)Math.Sin(i * Math.PI / 180) + 24);
                c++;
            }
            bPath = new Vector2[] { new Vector2(-8, 13), new Vector2(0, 0), new Vector2(8, 13), new Vector2(-8, 13) };
            center = new Vector2(radius + 7, radius + 7);
            shape = new Shape(path);
            shape.transform(Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)));
            #endregion

            targetLocation = getRandomUnoccupiedLocation();

            //place outside game in line with randomly selected target location
            location = targetLocation;
            int edge = random.Next(4);
            if (edge == 0) location.Y = -radius * 2;                    //Top
            else if (edge == 1) location.Y = game.height + radius * 2;  //Bottom
            else if (edge == 2) location.X = -radius * 2;               //Left
            else location.X = game.width + radius * 2;                  //right
            velocity = targetLocation - location;
            velocity *= 2f / velocity.Length();

            //Create and hold onto all the bullets, putting them at the target location
            //where they will be realeased; Bullets are drawn after entering and not updated here
            for (int i = 0; i < 8; i++)
            {
                float angle = (i + .5f) * MathHelper.PiOver4;
                bullets.Add(new WheelBullet(targetLocation, 
                    Vector2.Zero//new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) //Velocity once fired
                    , angle, bPath, game));
            }

        }

        public void fireBullet()
        {
            //Release wedge closest to the closest player
            if (game.players.Count == 0) return;
            SpriteElement closestPlayer = getClosestTarget();
            WheelBullet toBeFired = null;
            float closestdsq = float.MaxValue;
            foreach (WheelBullet s in bullets)
            {
                float xd = s.location.X - closestPlayer.location.X;
                float yd = s.location.Y - closestPlayer.location.Y;
                float dsq = xd * xd + yd * yd;
                if (dsq < closestdsq)
                {
                    toBeFired = s;
                    closestdsq = dsq;
                }
            }
            bullets.Remove(toBeFired);//Untether from this enemy
            toBeFired.velocity *= 2;
            game.bullets.Add(toBeFired);    //Let game call it's update and draw; already has right velocity
        }

        int waitCounter = 0; //To track time between releasing wedges
        bool entering = true;

        public override void Update()
        {
            if (entering)
            {
                location += velocity;
                angle += .02f;
                if ((targetLocation - location).LengthSquared() <= velocity.LengthSquared())
                {
                    entering = false;
                    location = targetLocation;
                    //particle effect for transforming...
                    angle = 0;
                }
            }
            else
            {
                //angle += .07f;
                //foreach (WheelBullet b in bullets) b.angle += .07f;
                if (bullets.Count == 0) game.enemiesToExplode.Add(this);
                //shoot wedge
                waitCounter++;
                if (waitCounter == 99)
                {
                    fireBullet();
                    waitCounter = 0;
                }
            }
            base.Update();
        }

    
        public override void Draw(SpriteBatch s)
        {
            //if (entering)
            //{
            //    if (velocity.Y == 0)    //Horizontal
            //    {
            //        int a = (int)location.X % d;
            //        for (int i = -n; i < n+1; i++)
            //        {
            //            s.Draw(enteringTexture,
            //                new Vector2(location.X + d * i - a, location.Y),
            //                null, new Color(Color.Green, (byte)(255 - Math.Abs(i) * 255 / n)),
            //                angle, new Vector2(enteringTexture.Width / 2, enteringTexture.Height / 2), 1, SpriteEffects.None, 0);
                        
            //        }
            //    }
            //    else                    //Vertical
            //    {
            //        int a = (int)location.Y % d;
            //        for (int i = -n; i < n+1; i++)
            //        {
            //            s.Draw(enteringTexture,
            //                new Vector2(location.X, location.Y + d * i - a),
            //                null, new Color(Color.Green, (byte)(255 - Math.Abs(i) * 255 / n)),
            //                angle, new Vector2(enteringTexture.Width / 2, enteringTexture.Height / 2), 1, SpriteEffects.None, 0);

            //        }
            //    }
            //}
            //else
            {
                base.Draw(s);                                   //Draws the wheel texture
                if (!entering)
                    foreach (WheelBullet b in bullets) b.Draw(s);
                //Draw bullets as they are attached
                
            }
        } 

        private class WheelBullet : Bullet
        {
            Vector2 velocity;       //Hides superclass float velocity since here we need components
            Vector2 accel;
            Vector2 magnetForce;
            float maxVelocity = 2f;

            public WheelBullet(Vector2 loc, Vector2 firedVelocity, float angle, Vector2[] path, SquidGame g) : base(bTexture, 0, .04f, loc, angle, 600, bulletColor, path, new Vector2(15, 24), g)
            {
                velocity = firedVelocity;
                
                shape.transform(Matrix.CreateRotationZ(MathHelper.PiOver2));
            }

            public override void addForce(Vector2 Force)
            {
                magnetForce += Force;
            }

            /// <summary>
            /// Overloaded from Bullet; Only called from game list because bullets in wheelEnemy list are 
            /// updated appropriately in the WheelEnemy update
            /// </summary>
            public override void Update()
            {
                age++;
                if (age > lifetime) explode();
                
                //Turn and accelerate towards closest player
                closestPlayer = getClosestTarget();
                if (closestPlayer != null)
                {
                    angle += base.smallestBoundedAngleDifference(closestPlayer.location, maxTurn);
                    accel = closestPlayer.location - location;
                    accel.Normalize();
                    accel *= .02f;
                    accel += magnetForce*4;
                }
                velocity += accel;

                //Limit velocity to max velocity
                if (velocity.LengthSquared() > maxVelocity*maxVelocity)
                {
                    velocity.Normalize();
                    velocity *= maxVelocity;
                }

                location += velocity;

                magnetForce = Vector2.Zero; //Reset this because magnet forces are added in each player update

                //Check for collisions
                base.detectCollisions();
                CurrentTransformMatrix = Matrix.CreateRotationZ(angle + (float)Math.PI / 2) * Matrix.CreateTranslation(new Vector3(location.X, location.Y, 0));

            }

            public override void Draw(SpriteBatch s)
            {
                s.Draw(texture, location, null, Color.White, angle + MathHelper.PiOver2 + MathHelper.Pi, center, 1, SpriteEffects.None, 0);
            }

        }
        
    }
}
