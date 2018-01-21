using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Subterranea {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TileManager tileManager;
        Texture2D pixel;
        Texture2D slope;
        Texture2D circle;
        public Vector2 camera = new Vector2(0, 0);
        public Vector2 cameraSize = new Vector2(40, 30);
        public int windowWidth = 1280;
        Vector2 pivot = new Vector2(0.5f);
        const float root2 = 1.41421356237f;
        public float ppu; // Pixels per unit, assuming square world scaling
        public MainGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            tileManager = new TileManager();
        }
        float a = 0;
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        Rectangle Mult(Rectangle r, float s) { // Scales rectangle by factor s
            return new Rectangle((int)(r.X * s), (int)(r.Y * s), (int)(r.Width * s), (int)(r.Height * s));

        }
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = windowWidth;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = (int)(windowWidth * (cameraSize.Y / cameraSize.X));   // set this value to the desired height of your window
            graphics.ApplyChanges();
            ppu = GraphicsDevice.Viewport.Width / cameraSize.X;
            base.Initialize();
            tileManager.Generate();
            tileManager.UpdateSlopes();
        }
        public static float NormalToRotation(Vector2 nrm) {
            float opp = nrm.Y;
            float adj = nrm.X;
            return (float)(Math.Atan2(opp, adj) * 57.295779513);
        }
        public static float SlopeNormalToRotation(Vector2 nrm) {
            float x = nrm.X;
            float y = nrm.Y;
            if (x == 1 && y == 1) {
                return 90;
            }
            if (x == 1 && y == -1) {
                return -90;
            }
            if (x == -1 && y == -1) {
                return -180;
            }
            if (x == -1 && y == 1) {
                return -270;
            }
            return 0;
        }
        public void DrawLine(Vector2 from, Vector2 to, Color tint, float thickness = 0.1f) {
            Vector2 rotVector = (to - from);
            DrawSprite(pixel, new Bounding(from.X, from.Y, rotVector.Length(), thickness),
                       tint,
                       (float)Math.Atan2(rotVector.Y, rotVector.X));
        }
        public void DrawCircle(Vector2 point, float radius, Color tint) {
            DrawSprite(circle,new Bounding(point.X,point.Y,radius*2,radius*2),tint);
        }
        public void DrawLines(Vector2[] points, Color tint, float thickness = 0.1f) {
            Vector2 lastPoint = points[0];
            DrawCircle(lastPoint,thickness/2,tint);
            for (int i = 1;i<points.Length;i++) {
                DrawLine(lastPoint,points[i],tint,thickness);
                DrawCircle(points[i], thickness / 2, tint);
                lastPoint = points[i];
            }
        }
        public void DrawSprite(Texture2D tex, Bounding bounds, Color tint, float rot = 0, Vector2? _pivot=null) { // Draws a sprite in world space
            Vector2 pivot;
            if (_pivot==null) {
                pivot = new Vector2(tex.Width / 2f, tex.Height / 2f);
            } else {
                pivot = (Vector2)_pivot;

            }
            spriteBatch.Draw(tex,
                             bounds.Scaled(ppu).Translated((-camera * ppu)).ToRectangle(),
                             null,
                             tint,
                             rot * 0.01745329251f,
                             pivot,
                             SpriteEffects.None,
                             1
                            );
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData<Color>(new Color[] { Color.White });
            // TODO: use this.Content to load your game content here
            slope = Content.Load<Texture2D>("slope");
            circle = Content.Load<Texture2D>("circle");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            tileManager.Update(gameTime);

            // TODO: Add your update logic here
            base.Update(gameTime);
        }
        public float[] ScreenToTile(Vector2 screenPos) {
            Vector2 changeOne = screenPos / ppu + camera;
            return new float[] { changeOne.X, changeOne.Y };
        }
        public Vector2 ScreenToWorld(Vector2 screenPos) {
            Vector2 changeOne = screenPos / ppu + camera;
            return new Vector2( changeOne.X, changeOne.Y );
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            float tileWidth = ppu;
            float tileHeight = ppu;
            spriteBatch.Begin();
            Vector2 screenPos = Mouse.GetState().Position.ToVector2();
            float[] pos = ScreenToTile(screenPos);
            if (Mouse.GetState().LeftButton== ButtonState.Pressed) {
                if (tileManager.GetAt((int)pos[0],(int)pos[1]).Filled) {
                    tileManager.Destroy((int)pos[0], (int)pos[1]);
                }

            }
            for (int i = (int)camera.X; i <= camera.X + cameraSize.X + 1; i++) {
                for (int j = (int)camera.Y; j <= camera.Y + cameraSize.Y + 1; j++) {
                    Tile tile = tileManager.GetAt(i, j);
                    if (tile.isnull || !tile.Filled) {

                    }
                    else {
                        //spriteBatch.Draw(pixel, new Rectangle((int)((i - camera.X) * tileWidth), (int)((j - camera.Y) * tileHeight), (int)tileWidth, (int)tileHeight), Color.SandyBrown);
                        if (tile.sloped == false) {
                            DrawSprite(pixel, new Bounding(i, j, 1, 1), Color.SandyBrown, 0);

                        }
                        else {
                            DrawSprite(slope, new Bounding(i, j, 1, 1), Color.SandyBrown, (int)tile.Slope);

                        }
                        if (tileManager.NeighborsAt(i, j) != 4) {
                            
                        }

                    }
                }
            }
            a++;
            DrawSprite(pixel, new Bounding(pos[0], pos[1], 0.2f, 0.2f), Color.Red, 0);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
