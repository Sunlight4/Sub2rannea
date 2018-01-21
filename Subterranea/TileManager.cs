using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Subterranea {
    public class TileManager {
        public KeyboardState lastState;
        public const int MAPX = 1000; // Fixed size of map
        public const int MAPY = 1000; // Fixed size of map
        public Tile nulltile;
        public Tile[,] tiles; // Tile data: 0 - empty  1 - filled  2 - out of bounds
        Random rand = new Random(6); // RNG
        public static int[][] sideOffsets = new int[][] {
            new int[] {1, 0},
            new int[] {0, 1},
            new int[] {-1, 0},
            new int[] {0, -1}
        }; // Tile offsets for side-neighboring tiles
        public bool IsKeyPressed(Keys key) {
            return Keyboard.GetState().IsKeyDown(key) && !lastState.IsKeyDown(key);
        }
        public TileManager() {
            nulltile = new Tile();
        }
        public Vector2 GetInput() {
            if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                return new Vector2(-1, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                return new Vector2(1, 0);
            }
            return new Vector2();
        }
        public void Update(GameTime delta) {
            lastState = Keyboard.GetState();
        }
        public static int Sign(float n) { // Taking code from Nested Dungeon's Player.cs
            if (n < 0) {
                return -1;
            }
            if (n == 0) {
                return 0;
            }
            return 1;
        }
        public bool IsValid(int x, int y) { // Is tile within map bounds
            if (x < 0 || y < 0 || x >= MAPX || y >= MAPY) {
                return false;
            }
            return true;
        }
        public bool IsOutside(int x, int y) { // Shortcut
            return !IsValid(x, y);
        }
        public bool SetAt(int x, int y, bool filled) { // Safe setter method. Returns false if unsuccessful, but will not crash
            if (IsOutside(x, y)) {
                return false;
            }
            tiles[x, y] = new Tile(this, filled, new Vector2(x, y));

            return true;
        }
        public Tile GetAt(int x, int y) { // Returns 2 if out of bounds
            if (IsOutside(x, y)) {
                return nulltile;
            }
            return tiles[x, y];
        }
        public void BatchSet(List<int[]> batch,bool value) {
            foreach (int[] tile in batch) {
                SetAt(tile[0],tile[1],value);
            }
        }
        
        public void Smooth(int minNeighbors = 2) {
            List<int[]> toRemove = new List<int[]>();
            for (int x = 0; x < MAPX; x++) {
                for (int y = 0; y < MAPY; y++) {
                    int neighbors = 0;
                    foreach (int[] offset in sideOffsets) {
                        if (GetAt(offset[0] + x, offset[1] + y).Filled) {
                            neighbors++;
                        
                        }
                    }
                    if (neighbors < minNeighbors) {
                        toRemove.Add(new int[] { x, y });
                    }
                }
            }
            BatchSet(toRemove, false);
          
        }
        public void Generate() {
            tiles = new Tile[MAPX, MAPY];
            System.Console.Write("Filling map");

            // First pass - Fill map
            for (int x = 0; x < MAPX; x++) {
                System.Console.Write(".");
                for (int y = 0; y < MAPY; y++) {
                    SetAt(x, y, y>=4?true:false);
                }
            }
            System.Console.WriteLine("Done.");
            System.Console.Write("Generating caves... ");
            // Second pass - Generate caves;
            for (int x = 0; x < MAPX; x++) {
                for (int y = 0; y < MAPY; y++) {
                    if (rand.Next(1, Global.CAVEINDEX) == 1) { // 1 in 50 tiles are seeded for caves
                        Expand(x, y, rand.Next(Global.MINCAVESIZE, Global.MAXCAVESIZE));
                    }

                }
            }
            System.Console.WriteLine("Done.");
            System.Console.Write("Smoothing terrain... ");
            // Third pass - Remove floating and hanging blocks
            for (int i = 0; i < 5;i++) {
                Smooth(2);

            }
            SetAt(0,0,false);
            System.Console.WriteLine("Done.");
        }
        public void Expand(int x, int y, int life) { // Recursive function for generating caves
            Tile thisTile = GetAt(x, y);
            if (!thisTile.Filled || life == 0) {
                return;
            }
            SetAt(x, y, false);
            foreach (int[] offset in sideOffsets) {
                if (rand.Next(1, 4) == 1) { // Adds rough edges to cave walls
                    continue;
                }
                Expand(x + offset[0], y + offset[1], life - 1);
            }
        }
        public bool IsTile(int x, int y) {
            return GetAt(x, y).Filled;
        }
        public bool Destroy(int x, int y) {
            bool success = SetAt(x, y, false);
            UpdateTile(x - 1, y);
            UpdateTile(x + 1, y);
            UpdateTile(x, y - 1);
            UpdateTile(x, y + 1);
            return success;
        }

        public void UpdateTile(int x, int y) { // Apply normal to given tile
            if (!(GetAt(x, y).Filled)) {
                return;
            }
            int? slope = null;
            if (!IsTile(x+1, y) && !IsTile(x-1, y) && !IsTile(x, y-1) && !IsTile(x, y+1)) {
                Destroy(x, y);
            }
            if (IsTile(x + 1, y) && !IsTile(x - 1, y)) {
                if (IsTile(x, y + 1) && !IsTile(x, y - 1)) {
                    slope = 0;
                }
                if (!IsTile(x, y + 1) && IsTile(x, y - 1)) {
                    slope = 270;
                }
            }
            if (!IsTile(x + 1, y) && IsTile(x - 1, y)) {
                if (IsTile(x, y + 1) && !IsTile(x, y - 1)) {
                    slope = 90;
                }
                if (!IsTile(x, y + 1) && IsTile(x, y - 1)) {
                    slope = 180;

                }
            }
            if (slope != null) {
                Tile tile = GetAt(x, y);
                tile.Slope = (int)slope;
            }



        }
        public void UpdateSlopes(Rectangle bounds) { // Updates side values for a given area
            for (int x = bounds.X; x < bounds.Width + 1; x++) {
                for (int y = bounds.Y; y < bounds.Height + 1; y++) {
                    UpdateTile(x, y);
                }
            }
        }
        public void UpdateSlopes() {
            UpdateSlopes(new Rectangle(1, 1, MAPX - 1, MAPY - 1));
        }

        public int NeighborsAt(int x, int y) {
            int n = 0;
            foreach (int[] offset in sideOffsets) {
                if (GetAt(x+offset[0],y+offset[1]).Filled) {
                    n++;
                }
            }
            return n;
        }
    }

}