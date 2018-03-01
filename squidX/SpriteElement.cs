using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace squidX
{
   public class SpriteElement
    {
        protected static Random random = new Random();
        public SquidGame game;
        public Vector2 location;
        public float angle;
        public Texture2D texture;
        public Vector2 center; //With respect to the top left of the Texture2D
        
        public Shape shape;
        public Matrix CurrentTransformMatrix;

        public SpriteElement() { }

        public SpriteElement(Texture2D t, Vector2 loc, SquidGame g)
        {
            texture = t;
            location = loc;
            game = g;
        }

        public virtual void Update()
        {
           CurrentTransformMatrix = Matrix.CreateRotationZ(angle+(float)Math.PI/2)*Matrix.CreateTranslation(new Vector3(location.X, location.Y,0));
        }

        public virtual void Draw(SpriteBatch s)
        {
            s.Draw(texture, location, null, Color.White, angle+(float)Math.PI/2, center, 1, SpriteEffects.None, 0);
        	
		}

        public virtual void drawShape(SpriteBatch s)
        {
            Vector2[] v2 = {
                               new Vector2(location.X-1, location.Y-1),
                               new Vector2(location.X+1, location.Y-1),
                               new Vector2(location.X+1, location.Y+1),
                               new Vector2(location.X-1, location.Y+1)};
            
            game.DrawLine(s, v2, Color.White);

            if (shape == null) return;

            shape.draw(game, s, CurrentTransformMatrix);
        }

        #region Some geometry Methods

        public bool intersects(float ax, float ay, float bx, float by)
        {
            return shape.intersectsLine(ax, ay, bx, by, CurrentTransformMatrix);
            //this.shape.intersects(
        }

       

        protected float smallestBoundedAngleDifference(Vector2 otherLocation, float maxAngle)
        {
            return smallestBoundedAngleDifference(
                (float)Math.Atan2(otherLocation.Y - location.Y, otherLocation.X - location.X), maxAngle);
        }

        protected float smallestBoundedAngleDifference(float angleTowards, float maxAngle)
        {
            float rawDifference = angleFix(angleTowards) - angleFix(angle);
            if (rawDifference > 0)
            {
                if (rawDifference < MathHelper.Pi)
                    return smaller(maxAngle, rawDifference);
                else
                    return -smaller(MathHelper.TwoPi - rawDifference, maxAngle);
            }
            else
            {
                if (-rawDifference < MathHelper.Pi)
                    return -smaller(-rawDifference, maxAngle);
                else
                    return smaller(MathHelper.TwoPi + rawDifference, maxAngle);
            }
        }

        private float smaller(float a, float b)
        {
            if (a < b) return a;
            else return b;
        }

        public static float angleFix(float b)
        {
            float a = b;
            while (a < 0) a += MathHelper.TwoPi;
            while (a > MathHelper.TwoPi) a -= MathHelper.TwoPi;
            return a;
        }

        #endregion

        /// <summary>
        /// Returns the sprite element whose location is closest to this sprite element's location
        /// </summary>
        public SpriteElement closestElement(List<SpriteElement> l)
        {
            SpriteElement closest = null;
            float closestDistanceSquared = float.MaxValue;
            foreach (SpriteElement s in l)
            {
                float xd = s.location.X - location.X;
                float yd = s.location.Y - location.Y;
                float dsq = xd * xd + yd * yd;
                if (dsq < closestDistanceSquared)
                {
                    closest = s;
                    closestDistanceSquared = dsq;
                }
            }
            return closest;
        }

        /// <summary>
        /// Uses the game.players list to find which player is closest
        /// </summary>
        /// <returns></returns>
        public SpriteElement getClosestTarget()
        {
            SpriteElement closestPlayer = null;
            float closestDistanceSquared = float.MaxValue;
            
            foreach (SpriteElement s in game.targets)
            {
                float xd = s.location.X - location.X;
                float yd = s.location.Y - location.Y;
                float dsq = xd * xd + yd * yd;
                if (dsq < closestDistanceSquared)
                {
                    closestPlayer = s;
                    closestDistanceSquared = dsq;
                }
            }
            return closestPlayer;
        }

    }

}

