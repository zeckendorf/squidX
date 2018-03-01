using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace squidX
{
    public class BlueShip : Enemy
    {
        static public Texture2D t;
        static public Texture2D bTexture;
        static public Color bulletColor = new Color(150, 150, 255);
        private float maxTurn, velocity;

        public BlueShip(SquidGame g)
            : base(t, 30, g)
        {
            maxTurn = .033f;
            explosionColor = new Color(150, 150, 255);
            velocity = 1.65f;
            Vector2[] path = new Vector2[]{
                new Vector2(24, 6),
                new Vector2(44, 22),
                new Vector2(25, 18),
                new Vector2(4,22),
                new Vector2(24,6)
            };
            shape = new Shape(path);
            bPath = new Vector2[] { new Vector2(-5, 5),
                        new Vector2(5, 5), new Vector2(5, -5), new Vector2(-5, -5), 
                        new Vector2(0, 0) };
            center = new Vector2(texture.Width/2f, texture.Height/2f);
            shape.transform(Matrix.CreateTranslation(new Vector3(-23, -10, 0)));
            targetLocation = getRandomUnoccupiedLocation();
        }
        public void fireBullet()
        {
            game.bullets.Add(new Bullet(bTexture, 3.2f, .15f, 
                new Vector2(location.X + 16f * (float)Math.Cos(angle), 
                    location.Y + 16f * (float)Math.Sin(angle)), angle, 
                    600, bulletColor, bPath, new Vector2(bTexture.Width/2f, bTexture.Height/2f), 
                        game));
        }

        int bulletCounter = 0;
        public override void Update()
        {
            bulletCounter++;
            if (bulletCounter >= 200)
            {
                fireBullet();
                bulletCounter = 0;
            }

            //Pick a new random target location if this one has been reached
            if ((targetLocation - location).LengthSquared() <= velocity * velocity)
                targetLocation = getRandomUnoccupiedLocation();

            angle += base.smallestBoundedAngleDifference(targetLocation, maxTurn);

            location.X += (float)(velocity * Math.Cos(angle));
            location.Y += (float)(velocity * Math.Sin(angle));

            base.Update();

        }



    }
}
