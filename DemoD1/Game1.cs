using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace DemoD1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // --- Deliverable 1 Variables ---
        private Texture2D _pixel;
        private Vector2 _playerPos;

        private float _normalSpeed = 300f;
        private float _slowSpeed = 120f;
        private bool _isSlowMode = false;

        private double _gameTimer = 0;
        private List<Vector2> _enemies = new();
        private List<Vector2> _bosses = new();
        private Random _rng = new();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            // FORCE window size so nothing is off-screen
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Start player near bottom-center
            _playerPos = new Vector2(384, 500);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // 1x1 pixel texture
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _gameTimer += dt;

            HandlePlayerMovement(dt);
            HandleSpawningLogic(dt);

            base.Update(gameTime);
        }

        private void HandlePlayerMovement(float dt)
        {
            var k = Keyboard.GetState();

            _isSlowMode = k.IsKeyDown(Keys.LeftShift);
            float speed = _isSlowMode ? _slowSpeed : _normalSpeed;

            Vector2 dir = Vector2.Zero;

            if (k.IsKeyDown(Keys.W) || k.IsKeyDown(Keys.Up)) dir.Y -= 1;
            if (k.IsKeyDown(Keys.S) || k.IsKeyDown(Keys.Down)) dir.Y += 1;
            if (k.IsKeyDown(Keys.A) || k.IsKeyDown(Keys.Left)) dir.X -= 1;
            if (k.IsKeyDown(Keys.D) || k.IsKeyDown(Keys.Right)) dir.X += 1;

            if (dir != Vector2.Zero)
            {
                dir.Normalize();
                _playerPos += dir * speed * dt;
            }

            // Clamp to screen
            _playerPos.X = MathHelper.Clamp(_playerPos.X, 0, 800 - 32);
            _playerPos.Y = MathHelper.Clamp(_playerPos.Y, 0, 600 - 32);
        }

        private void HandleSpawningLogic(float dt)
        {
            // Spawn regular enemies every 2s for first 48s
            if (_gameTimer < 48 && _gameTimer % 2 < dt)
            {
                _enemies.Add(new Vector2(_rng.Next(0, 775), -30));
            }
            // Spawn mid-boss once at 48s
            else if (_gameTimer >= 48 && _gameTimer < 48 + dt)
            {
                _bosses.Add(new Vector2(360, -100));
            }

            // Enemy movement
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i] = new Vector2(
                    _enemies[i].X,
                    _enemies[i].Y + 150f * dt);
            }

            // Boss movement
            for (int i = 0; i < _bosses.Count; i++)
            {
                if (_bosses[i].Y < 120)
                {
                    _bosses[i] = new Vector2(
                        _bosses[i].X,
                        _bosses[i].Y + 50f * dt);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // NOT the MonoGame default blue — proves Draw() is running
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Player
            _spriteBatch.Draw(
                _pixel,
                new Rectangle((int)_playerPos.X, (int)_playerPos.Y, 32, 32),
                Color.Green);

            // Slow-mode hitbox
            if (_isSlowMode)
            {
                _spriteBatch.Draw(
                    _pixel,
                    new Rectangle(
                        (int)_playerPos.X + 14,
                        (int)_playerPos.Y + 14,
                        4, 4),
                    Color.White);
            }

            // Enemies
            foreach (var e in _enemies)
            {
                _spriteBatch.Draw(
                    _pixel,
                    new Rectangle((int)e.X, (int)e.Y, 25, 25),
                    Color.Red);
            }

            // Bosses
            foreach (var b in _bosses)
            {
                _spriteBatch.Draw(
                    _pixel,
                    new Rectangle((int)b.X, (int)b.Y, 80, 80),
                    Color.Blue);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
