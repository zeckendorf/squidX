using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace squidX
{
    public class HealthBar
    {
        public static Texture2D hTexture;
        public int health=100;
        private Vector2 location;
        private HealthNode[] healthNodes;
        private Color c;
        public Player owner;
        public HealthBar(Vector2 location, Color c, Player owner)
        {
            this.location = location;
            this.c = c;
            healthNodes = new HealthNode[10];
            for (int i = 0; i < health/10; i++)
                healthNodes[i] = new HealthNode(new Vector2(location.X + (15) * i, location.Y), hTexture);
            this.owner = owner; 
            
        }

      
        public void Draw(SpriteBatch s)
        {
            for (int i = 0; i < 10; i++)
            {
                
                if (i < health/10)
                    healthNodes[i].active = true;
                else healthNodes[i].active = false;

            }
            foreach (HealthNode h in healthNodes)
            {
                if (h.active) h.Draw(s, c);
                else h.Draw(s, Color.Gray);
            }
			s.Draw(FlarePU.UnselectedFlareTexture, new Rectangle((int)location.X + 153, (int)location.Y - 5, (int)(1.5f * FlarePU.UnselectedFlareTexture.Width), (int)(1.5f * FlarePU.UnselectedFlareTexture.Height)), Color.White * .5f);
            s.DrawString(SquidGame.fontSmall, "x" + owner.numFlares, new Vector2(location.X + 203, location.Y + 21), Color.White * .5f);

			float percent = 20f;
            foreach (IPowerup i in owner.powerups)
                if (i.GetType() == typeof(DeflectorPU))
                {
                    percent = 20+235f * (((float)(i as DeflectorPU).lifetime - (i as DeflectorPU).age) / (float)(i as DeflectorPU).lifetime);
					
                }

			float poweralpha = percent / 255.0f;

            s.Draw(DeflectorPU.UnselectedTexture, new Rectangle((int)location.X + 211, (int)location.Y - 5, (int)(1.5f * FlarePU.UnselectedFlareTexture.Width),
			                                                    (int)(1.5f * FlarePU.UnselectedFlareTexture.Width)), Color.White * poweralpha);
        }
        public class HealthNode
        {
            private Texture2D hText;
            private Vector2 loc;
            public bool active;
          
            public HealthNode(Vector2 loc, Texture2D hText)
            {
                this.loc = loc;
                this.hText = hText;
            }
            public void Draw(SpriteBatch s, Color color)
            {
                s.Draw(hText, new Rectangle((int)loc.X+50, (int)loc.Y+5, (int)(hText.Width * 1.3), (int)(hText.Height+3)), null, new Color(color,150), MathHelper.PiOver2, Vector2.Zero, SpriteEffects.None, 0);
                //s.Draw(hText, loc, color);
                
            }
            
            
        }
    }
}
