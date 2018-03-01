using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace squidX
{
    public class CircleEnemy : Enemy
    {
        static public Texture2D t;
        static public Texture2D bTexture;
        static public Color bulletColor = new Color(150, 150, 255);
        private Vector2 velocity = Vector2.Zero;
        private Vector2 accel = Vector2.Zero;
        private float maxTurn = .02f;
       
        float age = 0;
        float lifetime = 1000;
      
        public CircleEnemy(SquidGame g)
            : base(t, 20, g)
        {
            
            explosionColor = new Color(150, 150, 255);
            lifetime += SquidGame.randy.Next(100);
            Vector2[] path = new Vector2[20];
            bPath = new Vector2[20]; 
            float pihelper = MathHelper.TwoPi/20f;
            for(int i = 0; i<20;i++)
            {
                path[i] = new Vector2(23+17*(float)Math.Cos(i*pihelper),10+17*(float)Math.Sin(i*pihelper));
                bPath[i] = new Vector2(5*(float)Math.Cos(i*pihelper),5*(float)Math.Sin(i*pihelper));
            }
            shape = new Shape(path);
            
            center = new Vector2(texture.Width / 2f, texture.Height / 2f);
            shape.transform(Matrix.CreateTranslation(new Vector3(-23, -10, 0)));
            targetLocation = getRandomUnoccupiedLocation();
        }
        public void fireBullet()
        {
            game.bullets.Add(new CircleBullet(new Vector2(location.X - 16 * (float)Math.Cos(angle), location.Y - 16 * (float)Math.Sin(angle)),bPath,game));
        }

        int bulletCounter = 0;
        public override void Update()
        {
            age++;
            if (age > lifetime) game.enemiesToExplode.Add(this);
           
           
                bulletCounter++;
                 if (bulletCounter >= 70)
                 {
                     fireBullet();
                      bulletCounter = 0;
                 }
            
            

           if((targetLocation-location).LengthSquared()<200) targetLocation = getRandomUnoccupiedLocation();

           

            angle += base.smallestBoundedAngleDifference(targetLocation, maxTurn);
            velocity.X = 1.9f*(float)Math.Cos(angle);
            velocity.Y = 1.9f*(float)Math.Sin(angle);
        location+=velocity;

            base.Update();

        }
    

    }
       public class CircleBullet : Bullet
        {
            Vector2 velocity;       //Hides superclass float velocity since here we need components
        
       
           
          

            public CircleBullet(Vector2 loc, Vector2[] path, SquidGame g) : base(CircleEnemy.bTexture,0, 0f, loc, 0, 300, Color.Purple, path, new Vector2(CircleEnemy.bTexture.Width/2, CircleEnemy.bTexture.Height/2), g)
            {
                velocity = Vector2.Zero;


                shape = new Shape(path);
               
            }

            public override void addForce(Vector2 Force)
            {
               
            }

            /// <summary>
            /// Overloaded from Bullet; Only called from game list because bullets in wheelEnemy list are 
            /// updated appropriately in the WheelEnemy update
            /// </summary>
            public override void Update()
            {

                if (age > lifetime) explode();

                age++;


                //Check for collisions
                base.detectCollisions();
                CurrentTransformMatrix = Matrix.CreateRotationZ(angle + (float)Math.PI / 2) * Matrix.CreateTranslation(new Vector3(location.X, location.Y, 0));
                }
                
            
            public override void Draw(SpriteBatch s)
            {
                base.Draw(s);
            }

        }
        
    }



