using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace squidX
{
    public class Starfield:SpriteElement
    {
        Star[] stars;
        //Color[] starColors;
        public static Texture2D[] starTextures;
        
        Color color;
        int brightness=255;
        int gwidth, gheight;
        public Starfield(float starDensity, int width, int height)
        {
            color = Color.White;
           // Console.WriteLine(starDensity);
            stars = new Star[(int)(width*height*starDensity)];
            gwidth = width*4;
            gheight = height*4;
            //Console.WriteLine((int)(width*height/500));
            for (int i = 0; i <stars.Length; i++)
            {
               
                byte r = color.R;
                byte g = color.G;
                byte b = color.B;
                color = new Color(r, g, b, (byte)random.Next(brightness));
                stars[i] = new Star(starTextures[random.Next(starTextures.Length)],getRandomLocation(), (float)random.NextDouble()*200,color);
                int randdif = random.Next(9);
                if (randdif > 4) stars[i].dif = 1;
                else stars[i].dif = -1;
           
            }
            
        }
        public Vector2 getRandomLocation()
        {
           return new Vector2(gwidth * 3 * (float)random.NextDouble() - gwidth, gheight * 3 * (float)random.NextDouble() - gheight);
        }
        public override void Update()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                byte alpha = stars[i].color.A;
                byte r = (byte)255;
                byte g = (byte)255;
                byte b = (byte)255;
                if (alpha <= 0)
                {
                    stars[i].dif = 1;
                    stars[i].location = getRandomLocation();
                    stars[i].z = (float)random.NextDouble() * 2500;
                }
                else if (alpha >= brightness) stars[i].dif = -1;
                    stars[i].color = new Color(r, g, b,(byte)( alpha + stars[i].dif));
            }
        }
        public override void Draw(SpriteBatch s)
        {

            for (int i = 0; i < stars.Length; i++)
                stars[i].Draw(s); //s.Draw(starTextures[random.Next(starLocations.Length)], starLocations[i], starColors[i]);
        }
        private class Star 
        {
            public int dif = 1;
            public Vector2 location;
            public Color color;
            public float z;
            private Texture2D texture;
            public Star(Texture2D texture,Vector2 location, float z, Color col)
            {
                color = col;
                this.location = location;
                this.texture = texture;
                this.z = z;
            }
            public void Draw(SpriteBatch s)
            {
                s.Draw(texture, new Rectangle((int)((location.X - SquidGame.cam.Position.X) * SquidGame.cameraZ / (z + SquidGame.cameraZ) ), (int)((location.Y 
                    - SquidGame.cam.Position.Y) * SquidGame.cameraZ / (z + SquidGame.cameraZ) )
                    , texture.Width / 7, texture.Height / 7), color);
            }
        }
    }
}
