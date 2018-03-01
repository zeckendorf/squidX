using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace squidX
{
    public class Shape
    {
        public Vector2[] path;
        public Vector2[] drawPath;
        public float radsq = 0;
        public Shape(Vector2[] points)
        {
            path = points;
            drawPath = new Vector2[path.Length];
            for (int i = 0; i < drawPath.Length; i++)
                drawPath[i] = new Vector2(path[i].X, path[i].Y);

        }
        public void transform(Matrix transform)
        {
            
            radsq = 0;
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = Vector2.Transform(path[i], transform);
                float rsqtemp = path[i].X * path[i].X + path[i].Y * path[i].Y;
                if (rsqtemp > radsq) radsq = rsqtemp;
            }
        }


        public bool intersectsLine(float ax, float ay, float bx, float by, Matrix CurrentTransformMatrix)
        {

            Vector2[] tempPath = transformed(CurrentTransformMatrix).path;
            for (int i = 0; i < tempPath.Length - 1; i++)
            {
                float ua = (tempPath[i+1].X - tempPath[i].X) * (ay - tempPath[i].Y) - (tempPath[i+1].Y - tempPath[i].Y) * (ax - tempPath[i].X);
                float ub = (bx - ax) * (ay - tempPath[i].Y) - (by - ay) * (ax - tempPath[i].X);
                float denominator = (tempPath[i+1].Y - tempPath[i].Y) * (bx - ax) - (tempPath[i+1].X - tempPath[i].X) * (by - ay);



                if (Math.Abs(denominator) <= 0.00001f)
                {
                    if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f)
                    {
                        return true;

                    }
                }
                else
                {
                    ua /= denominator;
                    ub /= denominator;

                    if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                    {
                        return true;

                    }
                }
            }
            return false;
    
            //Vector2[] tempPath = transformed(CurrentTransformMatrix).path;
            //for (int i = 0; i < path.Length - 1; i++)
            //{
            //    if
            //   ((ccw(ax, ay, bx, by, tempPath[i].X, tempPath[i].Y) != ccw(ax, ay, bx, by, tempPath[i + 1].X, tempPath[i + 1].Y))
            //     && (ccw(tempPath[i].X, tempPath[i].Y, tempPath[i + 1].X, tempPath[i + 1].Y, ax, ay) != ccw(tempPath[i].X, shape.path[i].Y, tempPath[i + 1].X, tempPath[i + 1].Y, bx, by))) return true;

            //}
            //return false;

        }
        public bool intersectsCircle(Matrix tr, float x, float y, float rsq)
        {
            Shape temp = transformed(tr);
            Vector2 c = new Vector2(x, y);
            for (int i = 0; i < path.Length-1; i++)
            {
                    Vector2 p1 = temp.path[i];
                    Vector2 p2 = temp.path[i + 1];
                    Vector2 dir = p2 - p1;
                    Vector2 diff = c - p1;
                    
                    float t = Vector2.Dot(diff,dir) / Vector2.Dot(dir,dir);
                    if (t < 0.0f)
                        t = 0.0f;
                    if (t > 1.0f)
                        t = 1.0f;
                    Vector2 closest = p1 + t * dir;
                    Vector2 d = c - closest;
                    float distsqr = Vector2.Dot(d,d);
                    if (distsqr < rsq) return true;
            }


            return false;
        }

        private bool lineIntersectsCircle(float x, float y, float ax, float ay, float bx, float by, float rsq)
        {

            float px = x-ax;
            float py = y-ay;

            float lx = bx - ax;
            float ly = by - ay;
            float ll = (float)Math.Sqrt(lx * lx + ly * ly);
            float lxu = lx / ll;
            float lyu = ly / ll;

            float dot = px * lxu + py * lyu;
            float lxProjection = lxu * dot;
            float lyProjection = lyu * dot;
            float dx, dy;
            if (lxProjection > lx)
            {
                dx = bx - x;
                dy = by - y;
            }
            else if (lxProjection < 0)
            {
                dx = ax - x;
                dy = ay - y;
            }
            else
            {
                dx = ax + lxProjection - x;
                dy = ay + lyProjection - y;
            }
            
            if( dx * dx + dy * dy < rsq) 
                return true;
            return false;
            
        }

       
  
        public static bool ccw(float ax, float ay, float bx, float by, float cx, float cy)
        {
            //Slightly deficient function to determine if the two lines p1,p2 and
            //p2, p3 turn in counter clockwise direction

            float dx1, dx2, dy1, dy2;

            dx1 = bx - ax;
            dy1 = by - ay;
            dx2 = cx - bx;
            dy2 = cy - by;

            if (dy1 * dx2 < dy2 * dx1) return true;
            else return false;
        }
        public void draw(SquidGame g,SpriteBatch s, Matrix transformation)
        {


            for (int i = 0; i < drawPath.Length; i++)
             {
                 drawPath[i].X = path[i].X;
                 drawPath[i].Y = path[i].Y;
                
                 drawPath[i]= Vector2.Transform(drawPath[i],transformation);
             
             }

            g.DrawLine(s, drawPath, Color.White);
        }
        public Shape transformed(Matrix transform)
        {
            Vector2[] v = new Vector2[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                v[i] = new Vector2(path[i].X, path[i].Y);
                v[i] = Vector2.Transform(v[i], transform);
            }
            return new Shape(v);
        }
    }
}
