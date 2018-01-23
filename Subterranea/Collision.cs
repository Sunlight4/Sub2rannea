using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace Subterranea {
    public class Collision {
        public Polygon s1;
        public Polygon s2;
        public float overlap;
        public Vector2 axis;
        public Collision(Polygon s1, Polygon s2, float overlap, Vector2 axis) {
            this.s1 = s1;
            this.s2 = s2;
            this.overlap = overlap;
            this.axis = axis;
        }

        public Vector2 offset { get => axis*overlap; }
        public Polygon getPolygon(Polygon shape) {
            if (ReferenceEquals(shape, s1)) {
                return s2;
            }
            else {
                return s1;
            }
        }
    }
}
