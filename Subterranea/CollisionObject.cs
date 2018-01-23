using System;
using Microsoft.Xna.Framework;
namespace Subterranea
{
    public abstract class CollisionObject
    {
        public abstract Vector2 GetPosition();
        public Polygon polygon;
        public virtual bool IsStatic() => false;
        public virtual void ResolveCollision(CollisionInfo info) {
            
        }
    }
}
