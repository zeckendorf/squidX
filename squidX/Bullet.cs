using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace squidX
{
   public class Bullet : SpriteElement
    {   //oriented around center
        public float velocity;
        public Vector2 vel;
        public float maxTurn;
        public int age;
        public int lifetime;
        public Color explosionColor;
        float rsq = 5f;
        public bool fired = false;
        public float maxVelocity;
        public SpriteElement closestPlayer = null; //One that is heat seeked

        public Bullet(Texture2D t, float velocity, float maxTurn, Vector2 location, float angle, int lifetime,
		              Color explosionColor, Vector2[] path, Vector2 center, SquidGame game)
            : base(t, location, game)
        {
            this.center = center;
           
            shape = new Shape(path);
            this.lifetime = lifetime;
            this.explosionColor = explosionColor;

            this.velocity = velocity;
            this.maxVelocity = velocity;
            this.maxTurn = maxTurn;
            this.angle = angle;
            if (velocity > 0) fired = true;
            vel = new Vector2(velocity * (float)Math.Cos(angle), velocity * (float)Math.Sin(angle));
        }
        public virtual void addForce(Vector2 Force)
        {
            vel += Force;
            velocity = vel.Length();
            angle = (float)Math.Atan2(vel.Y, vel.X);
        }
        
        protected void bounceOffWalls()
        {
            if (location.X + vel.X > SquidGame.border.Right || location.X +vel.X < SquidGame.border.Left)
            {
                angle = MathHelper.PiOver2-(angle -MathHelper.PiOver2);
                game.pe.createExplosion(location, 2f, explosionColor, 20,120);
                velocity *= .8f;
            }
            if (location.Y + vel.Y > SquidGame.border.Bottom || location.Y + vel.Y< SquidGame.border.Top)
            {
                angle = MathHelper.PiOver2 - angle - MathHelper.PiOver2;
               game.pe.createExplosion(location, 2f, explosionColor, 20, 120);
                velocity *= .8f;
            }
           
        }

        public override void Update()
        {
			// always increase age
            age++;

			// ensure that the bullet stays within age and speed params
            if (age > lifetime) explode();
            if (velocity > 2.5f * maxVelocity) velocity = 2.5f * maxVelocity;
            if (velocity > maxVelocity) velocity -= .05f;
                
			// if this bullet should bounce, ensure that we bounce
            if (this.texture == TurretEnemy.bTexture) bounceOffWalls();

			// add x and y vel components
            vel.X = velocity * (float)Math.Cos(angle);
            vel.Y = velocity * (float)Math.Sin(angle);

			// select target
            closestPlayer = getClosestTarget();
            if (closestPlayer != null)
                angle+=base.smallestBoundedAngleDifference(closestPlayer.location, maxTurn);

			// add velocity to location
            location += vel;

            // do the sprite element update
            base.Update();

			// finally, if we are collided, handle it
            detectCollisions();
            


       }

        protected void detectCollisions()
        {

            //Explode intersecting enemies
            foreach (Enemy e in game.enemies)
            {
                if (e.GetType() != typeof(WheelEnemy))
                {
                    float dx = e.location.X - location.X;
                    float dy = e.location.Y - location.Y;
                  if ((dx * dx + dy * dy) < rsq + e.shape.radsq)
                    {
                        Shape eshape = e.shape.transformed(e.CurrentTransformMatrix);
                        for (int i = 0; i < eshape.path.Length - 1; i++)
                        {
                            if (intersects(eshape.path[i].X, eshape.path[i].Y, eshape.path[i + 1].X, eshape.path[i + 1].Y))
                            {
                                game.enemiesToExplode.Add(e);
                                game.score += 100 * game.scoreMultiplier;
                                game.scoreMultiplier+=4;
                                explode();
                                return;
                            }
                        }
                    }
                }
            }
         
            foreach (SpriteElement e in game.targets)
            {

                float dx = e.location.X - location.X;
                float dy = e.location.Y - location.Y;
                if (e.GetType() == typeof(Flare))
                {
                     if (shape.intersectsCircle(CurrentTransformMatrix, e.location.X, e.location.Y, 100))
                     {
                         explode();
                     }
                }
                else
                if ((dx * dx + dy * dy) < rsq + e.shape.radsq)
                {
                    Shape eshape = e.shape.transformed(e.CurrentTransformMatrix);
                    for (int i = 0; i < eshape.path.Length - 1; i++)
                    {
                        if (intersects(eshape.path[i].X, eshape.path[i].Y, eshape.path[i + 1].X, eshape.path[i + 1].Y))
                        {
                            Player temp = e as Player;
                            temp.health -= 10;
                            game.score -= 10;
                            if (temp.health <= 0)
                            {
                                temp.vibCounter = 0;
                            }
                            else temp.vibCounter = 10;
                            game.scoreMultiplier = 1;
                            explode();
                            return;
                        }
                    }
                }
            }
        }

        public void explode()
        {
            game.bulletsToRemove.Add(this);
            game.score += 10 * game.scoreMultiplier;
        }

      

        
    }
}
