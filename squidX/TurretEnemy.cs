using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace squidX
{
    public class TurretEnemy: Enemy
      {
        static public Texture2D t; //its texture, public, static
        static public Texture2D bTexture;
        static public Color bulletColor = Color.Orange;
        float maxTurn;
        Vector2 velocity;
  
        SpriteElement target;
        public TurretEnemy(SquidGame game) : base(t, 28, game)
        {
            Vector2[] path = new Vector2[]{
                new Vector2(25, 7),
                new Vector2(44, 18),
                new Vector2(44, 39),
                new Vector2(26, 29),
                new Vector2(9,39),
                new Vector2(9,19),
                new Vector2(25,7)
            };
            this.center = new Vector2(26, 22);
            shape = new Shape(path);
            
            shape.transform(Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)));
            targetLocation = getRandomUnoccupiedLocation();

            maxTurn = .04f;
            bPath = new Vector2[]{
                new Vector2(1, -5),
                new Vector2(5, 1), 
                new Vector2(5, 8),
                new Vector2(-5, 8),
                new Vector2(-5, 1),
                new Vector2(1,-5)
            };

            angle = (float)Math.Atan2(targetLocation.Y - location.Y, targetLocation.X - location.X);
            velocity = targetLocation - location;
            velocity *= 6f / velocity.Length();
        }
        public void fireBullet()
        { 
             
             game.bullets.Add(new Bullet(bTexture, 6.5f, .00f, new Vector2(location.X + 30f * (float)Math.Cos(angle), location.Y + 30f * (float)Math.Sin(angle)), angle, 600, bulletColor,bPath, new Vector2(bTexture.Width/2,10), game));
        }

        private void selectTarget()
        {
            target = getClosestTarget();
            //if (game.players.Count > 0) target = game.players.ElementAt(random.Next(game.players.Count));
            //else target = null;
        }

        int bulletcounter = 0;
        bool reached = false;
        public override void Update()
        {
            angle = angleFix(angle);

            if (bulletcounter > 170)
            {
                fireBullet();
                bulletcounter = 0;

                selectTarget();
            }

            if (!reached)
            {
                if ((targetLocation-location).LengthSquared()<= velocity.LengthSquared())
                {
                    reached = true;
                    selectTarget();
                }
                location += velocity;

            }
            else
            {
                
                bulletcounter++;
              
            }
            if (target != null)
                angle += smallestBoundedAngleDifference(target.location,  maxTurn);

            base.Update();
        }
    }

}
