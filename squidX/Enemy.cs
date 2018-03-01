using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace squidX
{
   public class Enemy : SpriteElement
    {
        public Vector2 targetLocation;
        public Color explosionColor = Color.Wheat;
        public Vector2[] bPath;
        public int radius;
        public float age=0;
		public Enemy(Texture2D t, int radius, SquidGame game)
            : base(t, Vector2.Zero, game) { this.radius = radius; placeRandomlyAlongOutside(); }
        
        /// <summary>
        /// Sets TargetLocation to a random, empty spot inside game bounds
        /// </summary>
        public Vector2 getRandomUnoccupiedLocation()
        {
           
            Vector2 loc;
            do
            {
                loc = new Vector2(random.Next(SquidGame.border.Left + radius, SquidGame.border.Right - radius),
                    random.Next(SquidGame.border.Top + radius, SquidGame.border.Bottom - radius));

            } while (!areaEmpty(loc));
            return loc;
        }
        
        /// <summary>
       /// Sets the location at a random spot outside the edge of the game
       /// </summary>
        public void placeRandomlyAlongOutside()
        {
            int edge = random.Next(4);
            if (edge == 0)          //Place along top
            {
                location.X = random.Next(SquidGame.border.Left, SquidGame.border.Right);
                location.Y = -radius * 4;

            }
            else if (edge == 1)     //Place along left
            {
                location.X = -radius * 4;
                location.Y = random.Next(SquidGame.border.Top, SquidGame.border.Bottom);
            }
            else if (edge == 2)     //Place along right
            {
                location.X = game.width + radius * 4;
                location.Y = random.Next(SquidGame.border.Top, SquidGame.border.Bottom);
            }
            else                    //Place along bottom
            {
                location.X = random.Next(SquidGame.border.Left, SquidGame.border.Right);
                location.Y = game.height + radius * 8;
            }
        }
        
        /// <summary>
       /// Returns true if bounding circle of this enemy doesn't intersect with any other enemies
       /// </summary>
       /// <param name="loc"></param>
       /// <returns></returns>
        private bool areaEmpty(Vector2 loc)
        {
            foreach (Enemy e in game.enemies)
            {
                float dx = e.targetLocation.X - loc.X;
                float dy = e.targetLocation.Y - loc.Y;
                if (dx * dx + dy * dy < radius * radius + e.radius * e.radius)
                    return false;
            }
            return true;
        }
        
        public override void drawShape(SpriteBatch s)
        {
            //Draw the radius used in selecting empty locations
            game.DrawCircle(s, location, radius, 20, Color.White);

            base.drawShape(s);
        }
        public override void Draw(SpriteBatch s)
        {   
            
            if (3 * age / 2 <= texture.Width)
            {
                age++;
                int size = Math.Min(3 * (byte)age / 2, texture.Width);
                Color c = new Color(Color.White,(byte)(255*(age/texture.Width)));
                s.Draw(texture, new Rectangle((int)(location.X), (int)(location.Y), size, size), null, c, angle + MathHelper.PiOver2, center, SpriteEffects.None, 0);
            }
            else s.Draw(texture, location, null, Color.White, angle + MathHelper.PiOver2, center, 1, SpriteEffects.None, 0);
        }

    }
}
