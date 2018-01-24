using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace Subterranea {
    public class GameObject : CollisionObject {
        protected Vector2 position;
        public bool hasMoved = false;
        public override Vector2 GetPosition() => position;
        public List<Tile> tiles = new List<Tile>(); 

        public void Update(GameTime delta) {

        }
        public override void ResolveCollision(CollisionInfo info) {
            
        }

        public override Vector2 SetPosition(Vector2 val)
        {
            position = val;
        }
    }
