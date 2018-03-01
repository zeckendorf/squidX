using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace squidX
{
    public interface IPowerup
    {
        bool updateUnselected();
        bool updateSelected(Player p);
        void drawUnselected(SpriteBatch s);
        void drawSelected(SpriteBatch s);
        bool isActive();
        void checkActivation(Player p);
    }

    public class HealthPU : SpriteElement, IPowerup
    {
        public static Texture2D healthTexture;

        int age = 0;
        float alpha = 1;
        Color c;

        public HealthPU(Vector2 loc, SquidGame g)
            : base(healthTexture, loc, g)
        {

        }

        public bool updateUnselected()
        {
            if (age > 1000) alpha -= .002f;
            if (alpha <= 0) return true;

            //Check for intersection with players
            foreach (Player p in game.players)
            {
                if (p.shape.intersectsCircle(p.CurrentTransformMatrix, location.X, location.Y, 100))
                {
                    p.health += 20;
                    if (p.health > p.maxhealth) p.health = p.maxhealth;
                    //to do: add particles

                    for (int i = 0; i < 15; i++)
                    {
                        float randvelx = .15f * ((float)SquidGame.randy.NextDouble() - .5f);
                        float randvely = .15f * ((float)SquidGame.randy.NextDouble() - .5f);
                        game.pe.add(new PowerParticulate(300, location.X, location.Y, randvelx, randvely, Color.Lime));
                    }
                    return true;
                }
            }

            age++;
            return false;
        }
        public void checkActivation(Player p) { }
        public bool updateSelected(Player p)
        {
            return true;
        }

        public void drawUnselected(SpriteBatch s)
        {
            c = new Color(1f, 1f, 1f, alpha);
            int size = Math.Min(age * 3 / 2, healthTexture.Width);
            s.Draw(healthTexture, new Rectangle((int)(location.X - size / 2), (int)(location.Y - size / 2 ), size, size), c);
        }

        public void drawSelected(SpriteBatch s)
        {

        }

        public bool isActive() { return false; }





    }

    public class PowerParticulate : ParticleEngine.Particle
    {
        public static Texture2D healthParticleTexture;
        private Color color;
        private float angle;    //So it doesn't have to be recalculated every time it draws
        private float length;
        private float x2, y2, ax, ay;
        private byte alpha = 255;
        public PowerParticulate(int lifetime, float x, float y, float vx, float vy, Color color)
            : base(lifetime, x, y, vx, vy)
        {

            this.color = color;
            this.x2 = x - vx;
            this.y2 = y - vy;
            ax = -vx / 200;
            ay = -vy / 200;
            this.angle = (float)Math.Atan2(vy, vx) + (float)Math.PI / 2;
            this.length = (float)Math.Sqrt(vx * vx + vy * vy);

        }


        public override void Update()
        {

            //Randomly change acceleration
            if (SquidGame.randy.Next(70) == 5)
            {
                ax = .08f * ((float)SquidGame.randy.NextDouble());
                ay = .04f * ((float)SquidGame.randy.NextDouble() - .6f - (float)(Math.Sign(y - y2)));
            }

            float tempx = x;
            float tempy = y;

            //verlet integrate
            x = 1.80f * x - .80f * x2 + ax;
            y = 1.80f * y - .80f * y2 + ay;

            x2 = tempx;
            y2 = tempy;

            alpha = (byte)(255 * (lifetime - age) / lifetime);
            color = new Color(color, alpha);


            base.Update();
        }

        public override void Draw(SpriteBatch s)
        {
            s.Draw(healthParticleTexture,
                new Rectangle((int)(x), (int)(y), healthParticleTexture.Width, healthParticleTexture.Height),
                null, color, angle,
                new Vector2(healthParticleTexture.Width / 2, healthParticleTexture.Height / 2), SpriteEffects.None, 0);
        }

    }

    public class DeflectorPU : SpriteElement, IPowerup
    {
        public int age = 0;
        float alpha = 1;
        Color c;
        public int lifetime = 500;
        public static Texture2D DeflectorTexture;
        public static Texture2D UnselectedTexture;
        private bool active = false;
        public DeflectorPU(Vector2 loc, SquidGame g)
            : base(DeflectorTexture, loc, g)
        {
            Vector2[] path = new Vector2[]{
                new Vector2(-15,0),
            new Vector2(15,0),
            new Vector2(15,3),
            new Vector2(-15,3),
            new Vector2(-15,0)};
            shape = new Shape(path);
            center = new Vector2(DeflectorTexture.Width / 2, 0);
        }
       
        public bool updateUnselected()
        {

            age++;
            base.Update();
            if (age > lifetime) alpha -= .002f;
            if (alpha <= 0) return true;


            foreach (Player e in game.players)
            {

                Shape eshape = e.shape.transformed(e.CurrentTransformMatrix);
                float dx = e.location.X - location.X;
                float dy = e.location.Y - location.Y;
                DeflectorPU tdefl = null;
                foreach (IPowerup p in e.powerups)
                {

                    if (p.GetType() == typeof(DeflectorPU))
                    {

                        tdefl = p as DeflectorPU;
                    }

                }
                if ((dx * dx + dy * dy) < 700 + e.shape.radsq)
                    for (int i = 0; i < eshape.path.Length - 1; i++)
                    {
                        if (intersects(eshape.path[i].X, eshape.path[i].Y, eshape.path[i + 1].X, eshape.path[i + 1].Y))
                        {

                            for (int c = 0; c < 15; c++)
                            {
                                float randvelx = .15f * ((float)SquidGame.randy.NextDouble() - .5f);
                                float randvely = .15f * ((float)SquidGame.randy.NextDouble() - .5f);
                                game.pe.add(new PowerParticulate(300, location.X, location.Y, randvelx, randvely, Color.Sienna));
                            }
                            if (tdefl == null)
                            {
                                e.powerups.Add(this);
                                this.angle = e.angle;
                                age = 0;
                            }
                            else
                            {
                                tdefl.age -= 1000;
                                if (age < 0) age = 0;
                            }

                            return true;
                        }



                    }

            }


            return false;
        }

        public bool updateSelected(Player p)
        {
              angle = p.angle;
            location = new Vector2(p.location.X + 35 * (float)Math.Cos(angle), p.location.Y + 35 * (float)Math.Sin(angle));
            checkActivation(p);
            if (active)
            {
                age++;
                game.pe.add(new FlareSpark(p.location.X, p.location.Y, .4f * p.velocity * (float)Math.Cos(angle) + (float)SquidGame.randy.NextDouble() * (float)Math.Cos(angle + SquidGame.randy.NextDouble() * 1.2f * (float)Math.Pow(-1, SquidGame.randy.Next(0, 4))),
                .4f * p.velocity * (float)Math.Sin(angle) + (float)SquidGame.randy.NextDouble() * (float)Math.Sin(angle +
                SquidGame.randy.NextDouble() * 1.2f * (float)Math.Pow(-1, SquidGame.randy.Next(0, 4))), Color.White));
               // game.pe.add(new FlareSpark(p.location.X, p.location.Y, (float)Game.randy.NextDouble() * 4f * (float)Math.Cos(angle + Game.randy.NextDouble() * 1.2f * Game.randy.Next(0, 2) * -1), (float)Game.randy.NextDouble() * 4f * (float)Math.Sin(angle + Game.randy.NextDouble() * 1.2f * Game.randy.Next(0, 2) * -1), Color.White));
                
               //game.pe.add(new PowerParticulate(20, p.location.X, p.location.Y, .07f * p.velocity *(float)Math.Cos(angle)+ .4f*(float)Math.Cos(age),
               // .07f * p.velocity*(float)Math.Sin(angle) +.4f* (float)Math.Sin(age), Color.White));

                foreach (Bullet e in game.bullets)
                {

                    Vector2 d = e.location - p.location;
                    float angleBetween = (float)Math.Atan2(e.location.Y - p.location.Y, e.location.X - p.location.X);
                    float angleDif = smallestBoundedAngleDifference(angleBetween, MathHelper.TwoPi);
                    float maxAngle = MathHelper.Pi / 2;

                    if (Math.Abs(angleDif) < maxAngle)
                    {
                        float Force = 700 * (maxAngle - angleDif) / d.LengthSquared();
                        Vector2 force = (e.location - p.location);
                        force.Normalize();
                        e.addForce(force * Force);
                    }
                }

               
                    

                if (age > lifetime) return true;
                if (age < 0) age = 0;
                base.Update();
            }
            return false;
        }

        public void drawUnselected(SpriteBatch s)
        {
            //shape.draw(game, s, CurrentTransformMatrix);
            c = new Color(Color.White, alpha);
            int size = Math.Min(3*age / 2, HealthPU.healthTexture.Width);
            s.Draw(DeflectorPU.UnselectedTexture, new Rectangle((int)(location.X - size / 2 ), (int)(location.Y - size / 2), size, size), c);
        }

        public void drawSelected(SpriteBatch s)
        {

            //shape.draw(game, s, CurrentTransformMatrix);
            byte a = (byte)(40+(float)(215*(lifetime - age)) / (float)lifetime);
           
            Color aWhite = new Color(Color.White, a);
            if (active) s.Draw(texture, new Rectangle((int)(location.X), (int)(location.Y), texture.Width, texture.Height), null, aWhite, angle + MathHelper.PiOver2, center, SpriteEffects.None, 0);
        }

        public bool isActive()
        {
            return active;
        }
        public void checkActivation(Player p)
        {
            if (!active)
            {
                if (p.oldGamePadState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.LeftTrigger))
                {
                    if (p.gamePadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftTrigger))
                        active = true;
                }


            }
            if (active)
            {
                if (p.oldGamePadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftTrigger))
                    if (p.gamePadState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.LeftTrigger))
                        active = false;


            }
            
            if (!active)
            {
                if (p.oldKeyState.IsKeyUp(p.PCcontrols[4]))
                {
                    if (p.keyState.IsKeyDown(p.PCcontrols[4]))
                        active = true;
                }


            }
            if (active)
            {
                if (p.oldKeyState.IsKeyDown(p.PCcontrols[4]))
                {
                    if (p.keyState.IsKeyUp(p.PCcontrols[4]))
                        active = false;
                }


            }
            
        }

    }

    public class FlarePU : SpriteElement, IPowerup
    {


        public static Texture2D UnselectedFlareTexture;
        
        int age = 0;
        float alpha = 1;
        Color c;
        public List<Flare> flares = new List<Flare>();
        public FlarePU(Vector2 loc, SquidGame g)
            : base(UnselectedFlareTexture, loc, g)
        {
            
        }

        public bool updateUnselected()
        {
            if (age > 800) alpha -= .002f;
            if (alpha <= 0) return true;

            //Check for intersection with players
            foreach (Player p in game.players)
            {
                if (p.shape.intersectsCircle(p.CurrentTransformMatrix, location.X, location.Y, 100))
                { 
                    p.numFlares++;
                    FlarePU temp = null;
                    foreach (IPowerup pu in p.powerups)
                        if (pu.GetType() == typeof(FlarePU)) 
                        {
                            temp = pu as FlarePU;
                            temp.addFlares(p);
                           
                            break;
                        }
                    if (temp == null)
                    {
                       
                        p.powerups.Add(this);
                        addFlares(p);

                    }
                    //to do: add particles

                    for (int i = 0; i < 15; i++)
                    {
						float randvelx = .15f * ((float)SquidGame.randy.NextDouble() - .5f);
                        float randvely = .15f * ((float)SquidGame.randy.NextDouble() - .5f);
                        game.pe.add(new PowerParticulate(300, location.X, location.Y, randvelx, randvely, Color.OrangeRed));
                    }
                    return true;
                }
            }

            age++;
            return false;
        }

        public bool updateSelected(Player p)
        {

            if(flares.Count<=0)return true;
            checkActivation(p);
                for(int i = 0; i<flares.Count;)
                {
                    if (flares[i].alpha <= 0) 
                    {
                        game.targets.Remove(flares[i]);
                        flares.Remove(flares[i]);
                      
                    }
                    else
                    {
                        flares[i].Update();
                        i++;
                    }
                }
            return false;
        }

        public void drawUnselected(SpriteBatch s)
        {
            c = new Color(1f, 1f, 1f, alpha);
            int size = Math.Min(age * 3 / 2, UnselectedFlareTexture.Width);
            s.Draw(UnselectedFlareTexture, new Rectangle((int)(location.X - size / 2), (int)(location.Y - size / 2 ), size, size), c);
        }

        public void drawSelected(SpriteBatch s)
        {
            foreach (Flare f in flares)
                f.Draw(s);
        }

        public bool isActive() { return false; }
        public void checkActivation(Player p)
        {
            
           
                if (p.oldGamePadState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.RightTrigger))
                    if (p.gamePadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.RightTrigger))
                    {


                        int count = 0;
                         for(int i = 0; i<flares.Count;i++)
                             {
                                
                                 if (count > 1) break;
                                 if (!flares[i].fired)
                                 {
                                     
                                     
                                  
                                     flares[i] = new Flare(p.location, game, new Vector2(2 * (float)Math.Cos(p.angle + MathHelper.Pi), 2 * (float)Math.Sin(p.angle + MathHelper.Pi)),
                                        (int)Math.Pow(-1,count));

                                     flares[i].fired = true;
                                     game.targets.Add(flares[i]);
                                     if (count == 0) p.numFlares--;
                                     count++;
                                    
                                 }
                                 
                                
                                
                            }
                          
                        
                    }
                    else if (p.oldKeyState.IsKeyUp(p.PCcontrols[4]))
                        if(p.keyState.IsKeyDown(p.PCcontrols[4]))
                        {
                            
                        int count = 0;
                         for(int i = 0; i<flares.Count;i++)
                             {
                                 
                                 if (count > 1) break;
                                 if (!flares[i].fired)
                                 {
                                     
                                     
                                  
                                     flares[i] = new Flare(p.location, game, new Vector2(2 * (float)Math.Cos(p.angle + MathHelper.Pi), 2 * (float)Math.Sin(p.angle + MathHelper.Pi)),
                                        (int)Math.Pow(-1,count));

                                     flares[i].fired = true;
                                     
                                     game.targets.Add(flares[i]);
                                     if (count == 0) p.numFlares--;
                                     count++;
                                    
                                 }
                                 
                                
                                
                            }
                        }
        }
        
        private void addFlares(Player p)
        {
            
            flares.Add(new Flare(location, game,Vector2.Zero,0));
            flares.Add(new Flare(location, game, Vector2.Zero, 0));
          
        }


    }
    public class Flare : SpriteElement
    {
        public static Texture2D FlareMissile;
        public Vector2 velocity;
        public Vector2 bias;
        public float spinAngle;
        public float dspinAngle;
        public bool fired = false;
        public int age = 0;
        public byte alpha = 255;
        public Flare(Vector2 location, SquidGame g, Vector2 velocity, int bias)
            : base(FlareMissile, location, g)
        {
            this.velocity = velocity;
            this.velocity *= (.4f + .5f*(float)random.NextDouble());
            this.bias = Vector2.Normalize(velocity);
            this.bias = new Vector2(bias*this.bias.Y, -1*bias*this.bias.X);
            this.bias *= .055f;
            spinAngle = (float)random.NextDouble()*MathHelper.TwoPi;
            dspinAngle = .1f;

        }

        public override void Update()
        {
            if (fired)
            {
                age++;
                spinAngle += dspinAngle;
                dspinAngle -= .0002f;
                if (dspinAngle <= .008)
                {
                    if (dspinAngle < 0) dspinAngle = 0;
                    alpha--;
                }

                location += velocity;
                velocity += bias;

                Vector2 vacceleration = Vector2.Normalize(velocity);
                vacceleration *= .008f;
                Vector2 bacceleration = Vector2.Normalize(bias);
                bacceleration *= .008f;

                velocity -= vacceleration;
                bias -= bacceleration;
                if (age >= 1000)
                {
                    game.targets.Remove(this);
                }

                //particle engine


                int particleCount = (int)(2*(age+1) / (100));
                if (particleCount < 1) particleCount = 1;
                float divergence = 0;
                if (age > 400) divergence = .2f * age / 1000;

                if (age % particleCount == 0)
                {
                    game.pe.add(new FlareSpark( location.X + 10 * (float)Math.Cos(spinAngle + MathHelper.PiOver2), location.Y + 10 * (float)Math.Sin(spinAngle + MathHelper.PiOver2),
                       .3f * (float)Math.Cos(spinAngle + MathHelper.PiOver2 + divergence - 2*divergence * random.Next()), .3f * (float)Math.Sin(spinAngle + MathHelper.PiOver2 + divergence - 2*divergence * random.Next()), Color.White));
                    game.pe.add(new FlareSpark( location.X - 10 * (float)Math.Cos(spinAngle + MathHelper.PiOver2), location.Y - 10 * (float)Math.Sin(spinAngle + MathHelper.PiOver2),
                      -.3f * (float)Math.Cos(spinAngle + MathHelper.PiOver2 + divergence - 2*divergence * random.Next()), -.3f * (float)Math.Sin(spinAngle + MathHelper.PiOver2 +divergence - 2*divergence * random.Next()), Color.White));
                }

                base.Update();
            }
        }
        public override void Draw(SpriteBatch s)
        {
            if (fired)s.Draw(texture,
                new Rectangle((int)(location.X), (int)(location.Y), texture.Width, texture.Height),
                null, new Color(Color.Red,alpha), spinAngle,
                new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);
            
        }

    }


    public class FlareSpark : ParticleEngine.Particle
    {
        
        private Color color;
        
       
        public static int lifetime = 80;
        private byte alpha = 255;
        public FlareSpark( float x, float y, float vx, float vy, Color color)
            : base(lifetime, x, y, vx, vy)
        {

            this.color = color;
           

        }


        public override void Update()
        {

            //verlet integrate

            //bounceOffWalls();

            alpha = (byte)(255 * (lifetime - age) / lifetime);
            color = new Color(color, alpha);

            


            base.Update();
        }

        public override void Draw(SpriteBatch s)
        {
            s.Draw(PowerParticulate.healthParticleTexture,
                new Rectangle((int)(x), (int)(y), PowerParticulate.healthParticleTexture.Width/2, PowerParticulate.healthParticleTexture.Height/2),
                null, color, 0,
                new Vector2(PowerParticulate.healthParticleTexture.Width / 2, PowerParticulate.healthParticleTexture.Height / 2), SpriteEffects.None, 0);
        }

    }

}