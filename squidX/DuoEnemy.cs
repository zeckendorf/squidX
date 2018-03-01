using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace squidX
{
    public class DuoEnemy : Enemy
    {
        static public Texture2D t;
        static public Texture2D bTexture;
        static Color bulletColor = new Color(100, 255, 100);
        bool go;
        float angularVelocity = .4f;
        float maxTurn = .09f;
        Vector2 velocity;

        public DuoEnemy(SquidGame g)
            : base(t, 20, g)
        {
            explosionColor = new Color(80, 230, 160);
            Vector2[] path = new Vector2[]{
                new Vector2(23, 6),
                new Vector2(40, 35),
                new Vector2(6, 35),
                new Vector2(23, 6),
            };
            center = new Vector2(texture.Width / 2f, texture.Height / 2f);
            shape = new Shape(path);
            bPath = new Vector2[] { new Vector2(-5, 5), new Vector2(5, 5), new Vector2(5, -5), new Vector2(-5, -5), new Vector2(0, 0) };
            shape.transform(Matrix.CreateTranslation(new Vector3(-23, -25, 0)));

            
            targetLocation = getRandomUnoccupiedLocation();
            velocity = targetLocation - location;
            velocity.Normalize();
            velocity *= 2f;

            go = true;
        }

      

        public void fireBullet()
        {
            game.bullets.Add(new Bullet(bTexture, 4.1f, .055f, new Vector2(location.X + 25 * (float)Math.Cos(angle + MathHelper.Pi / 6), location.Y + 40 * (float)Math.Sin(angle + MathHelper.Pi / 6)), angle, 700, bulletColor, bPath, new Vector2(bTexture.Width / 2f, bTexture.Height / 2f), game));
            game.bullets.Add(new Bullet(bTexture, 4.1f, .055f, new Vector2(location.X - 25 * (float)Math.Cos(angle + MathHelper.Pi / 6), location.Y - 40 * (float)Math.Sin(angle + MathHelper.Pi / 6)), angle + MathHelper.Pi, 700, bulletColor, bPath, new Vector2(bTexture.Width / 2f, bTexture.Height / 2f), game));
           // game.bullets.Add(new Bullet(bTexture, 3.5f, .02f, new Vector2(location.X, location.Y - 33f), (float)Math.PI * -1 / 2, 700, bulletColor, bPath, new Vector2(bTexture.Width / 2f, bTexture.Height / 2f), game));
        }
        public override void Draw(SpriteBatch s)
        {
            s.Draw(texture, location, null, Color.White, angle-MathHelper.Pi, center, 1, SpriteEffects.None, 0);
        }
        
        public override void Update()
        {
            if (!go)
            {
                angle -= angularVelocity;
                angularVelocity -= .003f;
                if (angularVelocity <= 0) { go = true; angularVelocity = .5f; }

            }
            else
            {
                velocity.X = 2.2f * (float)Math.Cos(angle);
                velocity.Y = 2.2f * (float)Math.Sin(angle);
                location += velocity;
                
                angle += base.smallestBoundedAngleDifference(targetLocation, maxTurn);

                if (go&&(targetLocation - location).LengthSquared() < 1600)
                {
                    go = false;
                    fireBullet();
                    
                    game.pe.createExplosion(new Vector2(location.X + 25 * (float)Math.Cos(angle + MathHelper.Pi / 6), location.Y + 40 * (float)Math.Sin(angle + MathHelper.Pi / 6)), 2, Color.Aquamarine, 12, 50);
                    game.pe.createExplosion(new Vector2(location.X - 25 * (float)Math.Cos(angle + MathHelper.Pi / 6), location.Y - 40 * (float)Math.Sin(angle + MathHelper.Pi / 6)), 2, Color.Aquamarine, 12, 50);
                    //New target and velocity
                    targetLocation = getRandomUnoccupiedLocation();
                   
                    
                }
            }
            base.Update();
        }



    }
}
