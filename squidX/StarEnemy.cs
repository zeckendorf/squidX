using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace squidX
{
    public class StarEnemy : Enemy
    {
        static public Texture2D t;
        static public Texture2D bTexture;
        static public Color bulletColor = new Color(150, 150, 255);
        private Vector2 velocity = Vector2.Zero;
        private Vector2 accel = Vector2.Zero;
       
        private float maxVelocity = 2.33f;
        float rsq = 100;
        float age = 0;
        float lifetime = 1000;
       
        public StarEnemy(SquidGame g)
            : base(t, 20, g)
        {
            
            explosionColor = new Color(150, 150, 255);
            lifetime += SquidGame.randy.Next(100);
            Vector2[] path = new Vector2[]{
                new Vector2(24, -4),
                new Vector2(38, 5),
                new Vector2(34, 25),
                new Vector2(10,25),
                new Vector2(6,5),
                 new Vector2(24, -4),
            };
            shape = new Shape(path);
            bPath = new Vector2[] { new Vector2(1, -12),
                        new Vector2(8, -6), new Vector2(7,5), new Vector2(-6,5), new Vector2(-7,-6),new Vector2(1,-12) };
            center = new Vector2(texture.Width / 2f, texture.Height / 2f);
            shape.transform(Matrix.CreateTranslation(new Vector3(-23, -10, 0)));
            targetLocation = getRandomUnoccupiedLocation();
        }
        public void fireBullet()
        {
            game.bullets.Add(new StarBullet(new Vector2(location.X + 16 * (float)Math.Cos(angle), location.Y + 16 * (float)Math.Sin(angle)), new Vector2(6 * (float)Math.Cos(angle), 6 * (float)Math.Sin(angle)), angle, bPath, game));
        }

        int bulletCounter = 0;
        public override void Update()
        {
           
            if (age > lifetime) game.enemiesToExplode.Add(this);
                age++;
                bulletCounter++;
            //  Turn and accelerate towards closest player
            if (!(location.X + rsq > SquidGame.border.Right ||
                location.X - rsq < SquidGame.border.Left ||
                location.Y + rsq > SquidGame.border.Bottom ||
                location.Y - rsq < SquidGame.border.Top))
            {
                 if (bulletCounter >= 100)
                {
                fireBullet();
                bulletCounter = 0;
                }
            }
           

            if((targetLocation-location).LengthSquared()>1600)targetLocation = getRandomUnoccupiedLocation();



           // angle += base.smallestBoundedAngleDifference(targetLocation, maxTurn);
            if (targetLocation != null)
            {
                angle+=.008f;
                accel = targetLocation - location;
                accel.Normalize();
                accel *=.03f;
              
            }
            velocity += accel;
            if (velocity.X > maxVelocity) velocity.X = maxVelocity;
            if (velocity.Y > maxVelocity) velocity.Y = maxVelocity;
        location+=velocity;

            base.Update();

        }
    

    }
       public class StarBullet : Bullet
        {
            Vector2 velocity;       //Hides superclass float velocity since here we need components
            
            float rotationalVelocity = .02f;
            float rsq = 10;

            public StarBullet(Vector2 loc, Vector2 firedVelocity, float angle, Vector2[] path, SquidGame g) : base(StarEnemy.bTexture, firedVelocity.Length(), 0f, loc, angle, 600, Color.Orange, path, new Vector2(StarEnemy.bTexture.Width/2, StarEnemy.bTexture.Height/2), g)
            {
                velocity = firedVelocity;

                lifetime = 1500;
                shape = new Shape(path);
               
            }

            public override void addForce(Vector2 Force)
            {
                if (velocity.LengthSquared() > 0)
                    velocity += Force;
            }

            /// <summary>
            /// Overloaded from Bullet; Only called from game list because bullets in wheelEnemy list are 
            /// updated appropriately in the WheelEnemy update
            /// </summary>
            public override void Update()
            {

                if (age > lifetime) explode();

                //  Turn and accelerate towards closest player
                if (location.X + rsq > SquidGame.border.Right ||
                    location.X - rsq < SquidGame.border.Left ||
                    location.Y + rsq > SquidGame.border.Bottom ||
                    location.Y - rsq < SquidGame.border.Top)
                {
                    age++;
                    velocity = Vector2.Zero;
                }

                else{

                  
                    
                location += velocity;
                angle+=rotationalVelocity;


                //Check for collisions
               
                CurrentTransformMatrix = Matrix.CreateRotationZ(angle + (float)Math.PI / 2) * Matrix.CreateTranslation(new Vector3(location.X, location.Y, 0));
                }
                 base.detectCollisions();
            }

            public override void Draw(SpriteBatch s)
            {
                base.Draw(s);
            }

        }
        
    }

