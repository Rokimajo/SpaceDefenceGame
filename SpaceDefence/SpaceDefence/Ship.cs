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
        public float points;
        private Texture2D ship_body;
        private Texture2D base_turret;
        private Texture2D laser_turret;
        private float buffTimer = 10;
        private float buffDuration = 10f;
        private RectangleCollider _rectangleCollider;
        private Point target;
        private float move_speed = 2f;
        private Point oldPosition;
        private float current_speedX = 0;
        private float current_speedY = 0;
        private float lastShipAngle = 0; // Save last ship angle incase of no movement

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
            points = 0;
        }

        public override void Load(ContentManager content)
        {
            // Ship sprites from: https://zintoki.itch.io/space-breaker
            ship_body = content.Load<Texture2D>("ship_body");
            base_turret = content.Load<Texture2D>("base_turret");
            laser_turret = content.Load<Texture2D>("laser_turret");
            _rectangleCollider.shape.Size = ship_body.Bounds.Size;
            _rectangleCollider.shape.Location -= new Point(ship_body.Width/2, ship_body.Height/2);
            base.Load(content);
        }

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
            {
                Vector2 aimDirection = LinePieceCollider.GetDirection(GetPosition().Center, target);
                Vector2 turretExit = _rectangleCollider.shape.Center.ToVector2() + aimDirection * base_turret.Height / 2f;
                if (buffTimer <= 0)
                {
                    GameManager.GetGameManager().AddGameObject(new Bullet(turretExit, aimDirection, 150));
                }
                else
                {
                    GameManager.GetGameManager().AddGameObject(new Laser(new LinePieceCollider(turretExit, target.ToVector2()),400));
                }
            }

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
            
            CheckScreenWarp();
            
            base.Update(gameTime);
        }

        private float GetShipAngle()
        {
            var angle = LinePieceCollider.GetAngle(LinePieceCollider.GetDirection(oldPosition, _rectangleCollider.shape.Location));
            if (double.IsNaN(angle)) return lastShipAngle;
            lastShipAngle = angle;
            return angle;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float shipAngle = GetShipAngle();
            spriteBatch.Draw(ship_body, _rectangleCollider.shape.Center.ToVector2(), null, Color.White, shipAngle, _rectangleCollider.shape.Size.ToVector2() / 2f, Vector2.One, SpriteEffects.None, 0);
            float aimAngle = LinePieceCollider.GetAngle(LinePieceCollider.GetDirection(GetPosition().Center, target));
            if (buffTimer <= 0)
            {
                Rectangle turretLocation = base_turret.Bounds;
                turretLocation.Location = _rectangleCollider.shape.Center;
                spriteBatch.Draw(base_turret, turretLocation, null, Color.White, aimAngle, turretLocation.Size.ToVector2() / 2f, SpriteEffects.None, 0);
            }
            else
            {
                Rectangle turretLocation = laser_turret.Bounds;
                turretLocation.Location = _rectangleCollider.shape.Center;
                spriteBatch.Draw(laser_turret, turretLocation, null, Color.White, aimAngle, turretLocation.Size.ToVector2() / 2f, SpriteEffects.None, 0);
            }
            base.Draw(gameTime, spriteBatch);
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
