using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace squidX
{
    /// <summary>
    /// Manages a particle system with a linked list
    /// </summary>
    public class ParticleEngine
    {
        Particle root;
        public SquidGame g;

        Particle temp;    //Used for iterating while updating, deleting, drawing

        public static Texture2D ExplosionTexture;

        public ParticleEngine(SquidGame g)
        {
            this.g = g;
        }

        public void update()
        {
            //Update each particle
            temp = root;
            while (temp != null)
            {
                temp.Update();
                temp = temp.next;
            }


            //Remove expired particles
            while (root != null && root.age > root.lifetime)            //Removes expired particles adjacent to an expired root
                root = root.next;
            temp = root; //Root at this point must be an unexpired or null particle
            while (temp != null)
            {
                if (temp.next != null &&
                    temp.next.age > temp.next.lifetime) temp.next = temp.next.next;
                else temp = temp.next;
            }
        }

        public void Draw(SpriteBatch s)
        {
            temp = root;
            while (temp != null)
            {
                temp.Draw(s);
                temp = temp.next;
            }

        }

        public void add(Particle p)
        {
            p.next = root;
            root = p;
        }

        public void createExplosion(Vector2 loc, float initialRadius, Color c, int particleCount, int lifetime)
        {
            for (int i = 0; i < particleCount; i++)
            {
                float xd;
                float yd;
                do
                {
                    xd = 2 * initialRadius * ((float)SquidGame.randy.NextDouble() - .5f);
                    yd = 2 * initialRadius * ((float)SquidGame.randy.NextDouble() - .5f);
                } while (xd * xd + yd * yd > initialRadius * initialRadius);
                float t = (float)SquidGame.randy.NextDouble();
                float x0 = xd * t;
                float y0 = yd * t;
                add(new ExplosionParticulate(SquidGame.randy.Next(lifetime) + lifetime / 2, loc.X + x0, loc.Y + y0, xd - x0, yd - y0, c, true));
            }
        }
        public void createPlayerExplosion(Vector2 loc, float initialRadius, Color c, int lifetime, float velocity)
        {
            for (int i = 0; i < 180; i++)
            {
                float angle = i * 2 * MathHelper.Pi / 180f;
                add(new ExplosionParticulate(lifetime, loc.X, loc.Y, velocity * (float)Math.Cos(angle), velocity * (float)Math.Sin(angle), Color.Lavender, false));

            }
        }
        public void createPlayerIntro(Vector2 location)
        {
            for (int i = 0; i < 5; i++)
            {
                float xpoint = (float)(Math.Pow(-1, 5 + SquidGame.randy.Next(1, 3)) * SquidGame.randy.NextDouble() * 300);
                float ypoint = (float)(Math.Pow(-1, 5 + SquidGame.randy.Next(1, 3)) * SquidGame.randy.NextDouble() * 300);
                if (xpoint * xpoint + ypoint * ypoint < 300 * 300)
                    g.pe.add(new particle6(g,location, 120, location.X + xpoint, location.Y + ypoint, (float)SquidGame.randy.NextDouble() * MathHelper.TwoPi, 10, Color.LightGreen, 0));
                else i--;
            }
            //  pe.createExplosion(location, 10, Color.Green, 200, 130);

        }
        /// <summary>
        /// A single particle of ParticleEngine that is intrinsically linked
        /// </summary>
        public abstract class Particle
        {
            public Particle next;

            public int age, lifetime;
            public float x, y, vx, vy;

            public Particle()
            {

            }

            public Particle(int lifetime, float x, float y, float vx, float vy)
            {
                age = 0;
                this.lifetime = lifetime;
                this.x = x;
                this.y = y;
                this.vx = 2.2f*vx;
                this.vy = 2.2f*vy;
            }

            /// <summary>
            /// Ages the particle and adds the velocities to the current location.
            /// </summary>
            public virtual void Update()
            {
                age++;
                x += vx;
                y += vy;
            }

            /// <summary>
            /// Call this before base.Update for particals
            /// </summary>
            protected void bounceOffWalls()
            {
                if (x + vx > SquidGame.border.Right || x + vx < SquidGame.border.Left)
                {
                    vx *= -1;
                }
                if (y + vy > SquidGame.border.Bottom || y + vy < SquidGame.border.Top)
                {
                    vy *= -1;
                }
            }

            /// <summary>
            /// Don't use this. It does nothing. You should make it at least draw a point just so it makes a little sense.
            /// </summary>
            /// <param name="s"></param>
            public virtual void Draw(SpriteBatch s)
            {

            }

        }
        public class particle6 : ParticleEngine.Particle
        {
            public static Texture2D glowyLine;
            int generation;
            float angle;
            float length;
            Color color;
            Vector2 origin;
            Vector2 velocity;
            float alpha = 0;
            SquidGame g;
            public particle6(SquidGame g, Vector2 origin, int lifetime, float x, float y, float angle, float length, Color color, int generation)
                : base(lifetime, x, y, 0, 0)
            {
                this.angle = angle;
                this.length = length;
                this.generation = generation;
                this.color = color;
                this.origin = origin;
                Vector2 direction = origin - new Vector2(x, y);
                velocity = direction / 30;
                this.g = g;
            }

            public override void Update()
            {
                x += velocity.X;
                y += velocity.Y;

                if ((Math.Abs(origin.X - x) < 1))
                {
                    age = lifetime;
                    g.pe.createExplosion(origin, 1, Color.GreenYellow, 1, 70);
                }
                if ((Math.Abs(origin.Y - y) < 1))
                {
                    age = lifetime;
                   g.pe.createExplosion(origin, 1, Color.GreenYellow, 1, 70);
                }

                age++;
            }

            public override void Draw(SpriteBatch s)
            {
                alpha += 15;
                byte a = (byte)alpha;
               // s.Draw(glowyLine, new Rectangle((int)x, (int)y, 12, 12), null, new Color(color, a),0, new Vector2(5, 5), SpriteEffects.None, 1);
                s.Draw(glowyLine, new Rectangle((int)x,(int)y,10,10), new Color(color, a));
            }

        }
        private class ExplosionParticulate : Particle
        {
            private Color color;
            private float angle;    //So it doesn't have to be recalculated every time it draws
            private float length;
            private bool bounce;
            private float vxi, vyi;

            public ExplosionParticulate(int lifetime, float x, float y, float vx, float vy, Color color, bool bounce)
                : base(lifetime, x, y, vx, vy)
            {
                this.bounce = bounce;
                this.color = color;
                this.vxi = vx;
                this.vyi = vy;
                this.angle = (float)Math.Atan2(vy, vx) + (float)Math.PI / 2;
                this.length = (float)Math.Sqrt(vx * vx + vy * vy);

            }


            public override void Update()
            {
                float velocityMultiplier = (lifetime - age + 40f) / (lifetime + 100f);
                vx = vxi * velocityMultiplier;
                vy = vyi * velocityMultiplier;
                byte alpha = (byte)(255 * (lifetime - age) / (lifetime + .0001f));
                color = new Color(color, alpha);

                if (bounce)
                {
                    if ((x < SquidGame.border.Right && x + vx > SquidGame.border.Right) ||
                        (x > SquidGame.border.Left && x + vx < SquidGame.border.Left))
                    {
                        vx *= -.23f;
                        vxi *= -.23f;
                        angle = MathHelper.Pi - angle;
                    }
                    if ((x > SquidGame.border.Right && x + vx < SquidGame.border.Right) ||
                      (x < SquidGame.border.Left && x + vx > SquidGame.border.Left))
                    {
                        vx *= -.23f;
                        vxi *= -.23f;
                        angle = MathHelper.Pi - angle;
                    }

                    if ((y < SquidGame.border.Bottom && y + vy > SquidGame.border.Bottom) ||
                        (y > SquidGame.border.Top && y + vy < SquidGame.border.Top))
                    {
                        vy *= -.23f;
                        vyi *= -.23f;
                        angle = MathHelper.TwoPi - angle;
                    }
                    if ((y > SquidGame.border.Bottom && y + vy < SquidGame.border.Bottom) ||
                      (y < SquidGame.border.Top && y + vy > SquidGame.border.Top))
                    {
                        vy *= -.23f;
                        vyi *= -.23f;
                        angle = MathHelper.TwoPi - angle;
                    }
                }
                base.Update();
            }

            public override void Draw(SpriteBatch s)
            {
                s.Draw(ExplosionTexture,
                    new Rectangle((int)(x), (int)(y), ExplosionTexture.Width, (int)((vx * vx + vy * vy) * 3) + 33),
                    null, color, angle,
                    new Vector2(ExplosionTexture.Width / 2, ExplosionTexture.Height / 2), SpriteEffects.None, 0);
            }

        }

        internal void clear()
        {
            root = null;
            temp = null;
        }
    }

}
    

   
    

