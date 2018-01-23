using System;
using Microsoft.Xna.Framework;
namespace Subterranea
{
    public struct CollisionInfo
    {
        public CollisionObject other;
        public float overlap;
        public Vector2 axis;
    }
}
