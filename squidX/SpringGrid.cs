using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;


namespace squidX
{
    public class SpringGrid
    {
        SquidGame g;

        static int horizontalNodes = 80;
        static int verticalNodes = 60;
        Color c1 = new Color(0, 0, 245, 140);
        Color c2 = new Color(0, 170, 170, 140);
        float restingHorizontalDistance;
        float restingVerticalDistance;
        public static Texture2D lineTexture;

        Effect effect;
        RenderTarget2D olive;
        VertexPositionTexture[] rectangle = new VertexPositionTexture[4];

        Vector2[] gridArray = new Vector2[horizontalNodes * verticalNodes];
        Vector2[][] drawNodes;

        Texture2D x1;
        Texture2D original;
        Texture2D bulletTexture;

        public SpringGrid(SquidGame g)
        {
            this.g = g;
            effect = g.Content.Load<Effect>("SpringGrid.fx");

			olive = new RenderTarget2D(g.GraphicsDevice, horizontalNodes, verticalNodes, false, SurfaceFormat.Vector2, DepthFormat.Depth16, 1, RenderTargetUsage.PreserveContents);
			x1 = new Texture2D(g.GraphicsDevice, horizontalNodes, verticalNodes, false, SurfaceFormat.Vector2);
			original = new Texture2D(g.GraphicsDevice, horizontalNodes, verticalNodes, false, SurfaceFormat.Vector2);

            rectangle[0] = new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1));
            rectangle[1] = new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0));
            rectangle[2] = new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1));
            rectangle[3] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0));

			restingHorizontalDistance = SquidGame.border.Width / (horizontalNodes - 1f);
            restingVerticalDistance = SquidGame.border.Height / (verticalNodes - 1f);
            effect.Parameters["numBullets"].SetValue(0);
            effect.Parameters["restingHorizontalDistance"].SetValue(restingHorizontalDistance);
            effect.Parameters["restingVerticalDistance"].SetValue(restingVerticalDistance);
            effect.Parameters["hNodes"].SetValue(1f / x1.Width);
            effect.Parameters["vNodes"].SetValue(1f / x1.Height);
            effect.Parameters["width"].SetValue(x1.Width);
            effect.Parameters["height"].SetValue(x1.Height);

            drawNodes = new Vector2[horizontalNodes][];
            for (int i = 0; i < drawNodes.Length; i++) drawNodes[i] = new Vector2[verticalNodes];
            int counter = 0;
            for (int r = 0; r < verticalNodes; r++)
            {
                for (int c = 0; c < horizontalNodes; c++)
                {
                    gridArray[counter] = new Vector2(c * restingHorizontalDistance + SquidGame.border.Left, 
					                                 r * restingVerticalDistance + SquidGame.border.Top);
                    counter++;
                }
            }
			// Console.WriteLine("left, top: " + Game.border.Left + ", " + Game.border.Top);
			x1.SetData<Vector2>(gridArray, 0, gridArray.Length);
        
            effect.Parameters["x1"].SetValue(x1);
            effect.Parameters["x2"].SetValue(x1);



        }


        Vector2[] originalNodes = new Vector2[horizontalNodes * verticalNodes];
        Vector2[] bulletPositions;
        public void Update()
        {
            GraphicsDevice d = g.GraphicsDevice;
            effect.CurrentTechnique = effect.Techniques["FluidTechnique"];
            //bullets
            if (g.bullets.Count > 0)
            {
				bulletTexture = new Texture2D(g.GraphicsDevice, 1, g.bullets.Count, false, SurfaceFormat.Vector2);
                bulletPositions = new Vector2[g.bullets.Count];

                for (int i = 0; i < g.bullets.Count; i++)
                    bulletPositions[i] = g.bullets[i].location;

                bulletTexture.SetData<Vector2>(bulletPositions, 0, bulletPositions.Length);
                effect.Parameters["btext"].SetValue(bulletTexture);
                effect.Parameters["invBullets"].SetValue(1f / g.bullets.Count);
                effect.Parameters["numBullets"].SetValue(g.bullets.Count);
            }
           

          //  effect.Parameters["vdown"].SetValue(false);
          

            //Store current x1 which will become x2 later
			d.SetRenderTarget(null);
            x1.GetData<Vector2>(originalNodes);


            //Calculate relaxed position with the first pass
            d.SetRenderTarget(olive);

            d.Clear(Color.Black);
  
            EffectPass pass = effect.CurrentTechnique.Passes["Relax"];
			pass.Apply();
            d.DrawUserPrimitives(PrimitiveType.TriangleStrip, rectangle, 0, 2);


            //Give back the graphics card the partially completed calculation so it can finish with second pass
            d.SetRenderTarget(null);
            d.Textures[0] = null;

            olive.GetData<Vector2>(nodes);
            x1.SetData<Vector2>(nodes, 0, nodes.Length);
            effect.Parameters["x1"].SetValue(x1);

            //Calculate final node position with verlet integration pass
            d.SetRenderTarget(olive);
            d.Clear(Color.Black);
            pass = effect.CurrentTechnique.Passes["Verlet"];
            pass.Apply();
            d.DrawUserPrimitives(PrimitiveType.TriangleStrip, rectangle, 0, 2);


            d.SetRenderTarget(null); //Resets the device to render on the screen
            d.Textures[0] = null;
            d.Textures[1] = null;
            original.SetData<Vector2>(originalNodes, 0, originalNodes.Length);
            olive.GetData<Vector2>(nodes);
            x1.SetData<Vector2>(nodes);


            effect.Parameters["x1"].SetValue(x1);
            effect.Parameters["x2"].SetValue(original);

        }



        Vector2[] nodes = new Vector2[horizontalNodes * verticalNodes];
        public void Draw(SpriteBatch s)
        {
            x1.GetData<Vector2>(nodes);


            // Console.WriteLine(nodes[0] + ", " + nodes[1]);
            int counter = 0;
            float locsum = 0;
            for (int r = 0; r < verticalNodes; r++)
            {
                for (int c = 0; c < horizontalNodes; c++)
                {
                    drawNodes[c][r] = nodes[counter];
                    counter++;
                    locsum += drawNodes[c][r].X;
                    locsum += drawNodes[c][r].Y;
                }

            }

            byte alpha = 70;
            for (int i = 0; i < horizontalNodes; i++)
            {
				Color c = new Color((byte)10, (byte)10, (byte)200, alpha);
                for (int j = 0; j < verticalNodes; j++)
                {
                    Vector2 gn = drawNodes[i][j];

                    if (i < horizontalNodes - 1) g.DrawLine(s, gn.X, gn.Y, drawNodes[i + 1][j].X, drawNodes[i + 1][j].Y, c);
                    if (j < verticalNodes - 1) g.DrawLine(s, gn.X, gn.Y, drawNodes[i][j + 1].X, drawNodes[i][j + 1].Y, c);

                }
            }



        }

    }
}
