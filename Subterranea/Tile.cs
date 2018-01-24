using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Subterranea {
    public class Tile : CollisionObject
    {
        public bool sloped;
        protected int slopeRotation;
        protected bool filled = false;
        protected Vector2 position;
        public override bool IsStatic() => true;
        public List<GameObject> objects = new List<GameObject>();
        public Vector2 Position
        {
            get => position;
            set {
                throw new FieldAccessException();
            }
        }
        public bool isnull = false;
        public void Fill() {
            filled = true;
        }
        public Boolean Filled => filled;
        public void Unfill() {
            filled = false;
        }

        public override Vector2 GetPosition() => position;

        public Tile() {
            isnull = true;
        }
        public int Slope {
            get => slopeRotation;
            set {
                sloped = true;
                slopeRotation = value;
                polygon = Polygon.RightTriangle(this, slopeRotation, 0.5f);
            }
        }
        public Tile(TileManager mng, bool filled, Vector2 pos) {
            sloped = false;
            slopeRotation = 0;
            position = pos;
            this.filled = filled;
            polygon = Polygon.AABB(this, 0.5f, 0.5f); 
        }
    }
}
