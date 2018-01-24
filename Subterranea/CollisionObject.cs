using System;
using Microsoft.Xna.Framework;
namespace Subterranea
{
    public abstract class CollisionObject
    {
        public abstract Vector2 GetPosition();
        public abstract Vector2 SetPosition(Vector2 val);
        public Polygon polygon;
        public virtual bool IsStatic() => false;
        public virtual void ResolveCollision(CollisionInfo info) {
            
        }
    }
}
