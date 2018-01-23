using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace Subterranea {
    public class GameObject : CollisionObject {
        protected Vector2 position;

        public override Vector2 GetPosition() => position;

        public void Update(GameTime delta) {

        }
        public override void ResolveCollision(CollisionInfo info) {
            
        }
}
