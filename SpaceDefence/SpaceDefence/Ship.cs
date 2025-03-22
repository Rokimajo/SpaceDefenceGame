using System;
using SpaceDefence.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class Ship : GameObject
    {
        private Weapon _weapon;
        private int _points;
        private SpriteFont _font;
        private Texture2D ship_body;
        private Texture2D base_turret;
        private Texture2D laser_turret;
        private AnimationPlayer _animPlayer;
        private float buffTimer = 10;
        private float buffDuration = 10f;
        private RectangleCollider _rectangleCollider;
        private Point target;
        private float move_speed = 2f;
        private Point oldPosition;
        private float current_speedX = 0;
        private float current_speedY = 0;
        private float lastShipAngle = 0; // Save last ship angle incase of no movement
        private bool _hasCargo;

        /// <summary>
        /// The player character
        /// </summary>
        /// <param name="Position">The ship's starting position</param>
        public Ship(Point Position)
        {
            _rectangleCollider = new RectangleCollider(new Rectangle(Position, Point.Zero));
            var x = new Rectangle(0, 0, 0, 0);
            oldPosition = Position;
            SetCollider(_rectangleCollider);
            _points = 0;
            _hasCargo = false;
        }

        public override void Load(ContentManager content)
        {
            // Ship sprites from: https://zintoki.itch.io/space-breaker
            ship_body = content.Load<Texture2D>("ship_body");
            base_turret = content.Load<Texture2D>("base_turret");
            laser_turret = content.Load<Texture2D>("laser_turret");
            _font = content.Load<SpriteFont>("PixelFont");
            _rectangleCollider.shape.Size = ship_body.Bounds.Size;
            _rectangleCollider.shape.Location -= new Point(ship_body.Width/2, ship_body.Height/2);
            _animPlayer = new AnimationPlayer("Explosion", 15, _rectangleCollider.shape);
            _animPlayer.Load(content);
            _weapon = new LaserWeapon(laser_turret);
            base.Load(content);
        }

        public Point GetTarget() => target;

        public bool HasCargo() => _hasCargo;
        public void AddCargo() => _hasCargo = true;
        public void RemoveCargo() => _hasCargo = false;
        public void AddPoints(int points) => _points += points;

        private void Move(InputManager inputManager)
        {
            if (inputManager.IsKeyPress(Keys.W))
                current_speedY -= move_speed;
            if (inputManager.IsKeyPress(Keys.S))
                current_speedY += move_speed;
            if (inputManager.IsKeyPress(Keys.A))
                current_speedX -= move_speed;
            if (inputManager.IsKeyPress(Keys.D))
                current_speedX += move_speed;
            current_speedX = Math.Clamp(current_speedX, -move_speed, move_speed);
            current_speedY = Math.Clamp(current_speedY, -move_speed, move_speed);
            
            oldPosition = _rectangleCollider.shape.Location;
            _rectangleCollider.shape.Location += new Point((int)current_speedX, (int)current_speedY);
        }

        public override void HandleInput(InputManager inputManager)
        {
            base.HandleInput(inputManager);
            target = inputManager.CurrentMouseState.Position;
            if(inputManager.LeftMousePress())
                _weapon.Shoot();
            
            Move(inputManager);
        }

        private void CheckScreenWarp()
        {
            var bounds = GameManager.GetGameManager().Game.GraphicsDevice.Viewport.Bounds;
            if (_rectangleCollider.shape.Left > bounds.Right)
            {
                _rectangleCollider.shape.X = bounds.Left;
            }
            if (_rectangleCollider.shape.Right < bounds.Left)
            {
                _rectangleCollider.shape.X = bounds.Right - _rectangleCollider.shape.Width;
            }
            if (_rectangleCollider.shape.Top > bounds.Bottom)
            {
                _rectangleCollider.shape.Y = bounds.Top - _rectangleCollider.shape.Height;
            }
            if (_rectangleCollider.shape.Bottom < bounds.Top)
            {
                _rectangleCollider.shape.Y = bounds.Bottom;
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Update the Buff timer
            if (buffTimer > 0)
                buffTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Check if buff expired and weapon is still Laser
            if (_weapon is LaserWeapon && buffTimer <= 0)
                SetBulletWeapon();
            
            CheckScreenWarp();
            
            base.Update(gameTime);
        }

        public void SetWeapon(Weapon weapon) => _weapon = weapon;
        public void SetBulletWeapon() => _weapon = new BulletWeapon(base_turret);
        public void SetLaserWeapon() => _weapon = new LaserWeapon(laser_turret);

        private float GetShipAngle()
        {
            var angle = LinePieceCollider.GetAngle(LinePieceCollider.GetDirection(oldPosition, _rectangleCollider.shape.Location));
            if (double.IsNaN(angle)) return lastShipAngle;
            lastShipAngle = angle;
            return angle;
        }

        public override void Destroy()
        {
            _animPlayer.Reset();
            _animPlayer.UpdateSpriteLocation(_rectangleCollider.shape);
            GameManager.GetGameManager().AddGameObject(_animPlayer);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float shipAngle = GetShipAngle();
            spriteBatch.Draw(ship_body, _rectangleCollider.shape.Center.ToVector2(), null, Color.White, shipAngle, _rectangleCollider.shape.Size.ToVector2() / 2f, Vector2.One, SpriteEffects.None, 0);
            _weapon.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
        }

        public override void DrawHUD(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.DrawHUD(gameTime, spriteBatch);
            var gm = GameManager.GetGameManager();
            var pointsString = $"POINTS: {_points}";
            var cargoString = _hasCargo ? "CARGO: YES" : "CARGO: NO";
            // draw HUD in top right
            var bounds = gm.Game.GraphicsDevice.Viewport.Bounds;
            var hudLoc = new Vector2(bounds.Width - Math.Max(_font.MeasureString(pointsString).Length(), _font.MeasureString(cargoString).Length()), 70);
            spriteBatch.DrawString(_font, pointsString, hudLoc, Color.White, 0, _font.MeasureString(pointsString) / 2, 1.0f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(_font, cargoString, new Vector2(hudLoc.X, hudLoc.Y - 30), Color.White, 0, _font.MeasureString(cargoString) / 2, 1.0f, SpriteEffects.None, 1f);
        }

        public void Buff()
        {
            buffTimer = buffDuration;
        }

        public Rectangle GetPosition()
        {
            return _rectangleCollider.shape;
        }
    }
}
