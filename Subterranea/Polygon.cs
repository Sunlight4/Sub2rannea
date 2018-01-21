using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Subterranea {
    public class Polygon {
        private Rectangle bounds;
        private HashSet<Vector2> axes;
        public HashSet<Vector2> normals;
        public Vector2[] points;
        public Vector2 Position;
        public Polygon(Vector2[] points) {
            axes = new HashSet<Vector2>();
            normals = new HashSet<Vector2>();
            UpdatePoints(points);
        }
        public static Polygon AABB(float hx, float hy) { // Creates a rectangle collider
            return new Polygon(new Vector2[] { new Vector2(-hx, -hy), new Vector2(hx, -hy), new Vector2(hx, hy), new Vector2(-hx, hy) });
        }
        public static Polygon RightTriangle(int rotation, float side) {
            List<Vector2> points = new List<Vector2>();
            if (rotation != 0) {
                points.Add(new Vector2(-side, -side));
            }
            
            if (rotation != 90) {
                points.Add(new Vector2(side, -side));
            }
            if (rotation != 180) {
                points.Add(new Vector2(side, side));
            }
            if (rotation != 270) {
                points.Add(new Vector2(-side, side));
            }


            Polygon poly = new Polygon(points.ToArray());
            return poly;
        }
        public void UpdatePoints(Vector2[] points) {
            //POINTS MUST GO CLOCKWISE
            int minx = 100;
            int miny = 100;
            int maxx = -100;
            int maxy = -100;
            for (int i = 0; i < points.Length; i++) {
                Vector2 point = points[i];
                Vector2 point2 = new Vector2();
                if (i > 0) {

                    point2 = points[i - 1];
                }
                else {
                    point2 = points[points.Count<Vector2>() - 1];
                }
                //System.Console.WriteLine(point.ToString()+point2.ToString());
                Vector2 edge = point - point2;
                Vector2 normal = Global.Rotate90(edge, 1);
                
                normals.Add(normal);
                axes.Add(Global.RefVector(normal));
                if (point.X<minx) {
                    minx = (int) (point.X+0.5f);
                }
                if (point.X > maxx) {
                    maxx = (int) (point.X+0.5f);
                }
                if (point.Y < miny) {
                    miny = (int) (point.Y+0.5f);
                }
                if (point.Y > maxy) {
                    maxy = (int) (point.Y+0.5f);
                }
            }
            bounds = new Rectangle(minx, miny, maxx - minx, maxy - miny);
            this.points = points;
        }
        public Rectangle GetBounds() {
            if (bounds==null) {
                UpdatePoints(points);
            }
            Rectangle rect = new Rectangle((int) (bounds.X+Position.X), (int) (bounds.Y+Position.Y), bounds.Width, bounds.Height);
            return rect;
        }

        public HashSet<Vector2> Axes(Vector2 otherPos) {
            if (axes==null) {
                UpdatePoints(points);
            }
            return axes;
        }

        public double[] GetMinMax(Vector2 axis) {
            double min = 100;
            double max = -100;
            foreach (Vector2 point in points) {
                double proj = Global.Project(point+Position, axis);
                if (proj<min) {
                    min = proj;
                }
                if (proj>max) {
                    max = proj;
                }
            }
            return new double[] { min, max };
        }
    }
}
