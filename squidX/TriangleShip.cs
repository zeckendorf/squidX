using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace squidX
{
    public class TriangleShip : Enemy
    {
        static public Texture2D t;
        static public Texture2D bTexture;
        static Color bulletColor = new Color(255, 100, 100);
        bool go;
        Vector2 velocity;

        public TriangleShip(SquidGame g)
            : base(t, 20, g)
        {
            explosionColor = new Color(255, 40, 40);
            Vector2[] path = new Vector2[]{
                new Vector2(23, 6),
                new Vector2(40, 35),
                new Vector2(6, 35),
                new Vector2(23, 6),
            };
            center = new Vector2(texture.Width/2f, texture.Height/2f);
            shape = new Shape(path);
            bPath = new Vector2[] { new Vector2(-5, 5), new Vector2(5, 5), new Vector2(5, -5), new Vector2(-5, -5), new Vector2(0, 0) };
            shape.transform(Matrix.CreateTranslation(new Vector3(-23, -25, 0)));
            shape.transform(Matrix.CreateRotationZ(-MathHelper.PiOver2));
            targetLocation = getRandomUnoccupiedLocation();
            velocity = targetLocation - location;
            velocity.Normalize();
            velocity *= 2f;

            go = true;
        }

        public override void Draw(SpriteBatch s)
        {
            s.Draw(texture, location, null, Color.White, 0, center, 1, SpriteEffects.None, 0);
        }

        public void fireBullet()
        {
            game.bullets.Add(new Bullet(bTexture,6.2f, .03f, new Vector2(location.X - 22f, location.Y + 30f), (float)Math.PI * 7 / 6, 700, bulletColor, bPath, new Vector2(bTexture.Width/2f, bTexture.Height/2f), game));
            game.bullets.Add(new Bullet(bTexture, 6.2f, .03f, new Vector2(location.X + 22f, location.Y + 30f), (float)Math.PI * -1 / 6, 700, bulletColor, bPath, new Vector2(bTexture.Width / 2f, bTexture.Height / 2f), game));
            game.bullets.Add(new Bullet(bTexture, 6.2f, .03f, new Vector2(location.X, location.Y - 33f), (float)Math.PI * -1 / 2, 700, bulletColor, bPath, new Vector2(bTexture.Width / 2f, bTexture.Height / 2f), game));
        }

        int waitCounter = 0; //for triangle, waits, fires, goes
        public override void Update()
        {
            if (!go)
            {
                waitCounter++;
                if (waitCounter == 50)
                    fireBullet();
                if (waitCounter == 100)
                {
                    go = true;
                    waitCounter = 0;
                }

            }
            else
            {
                location += velocity;
                //Check if target location has been reached
                if ((targetLocation - location).LengthSquared() < velocity.LengthSquared())
                {
                    go = false;
                    //New target and velocity
                    targetLocation = getRandomUnoccupiedLocation();
                    velocity = targetLocation - location;
                    velocity.Normalize();
                    velocity *= 4f;
                }
            }
            base.Update();
        }



    }
}
