using System;
using Microsoft.Xna.Framework;
namespace Subterranea
{
    public interface CollisionObject
    {
        Vector2 GetPosition();
        int Solid();
    }
}
