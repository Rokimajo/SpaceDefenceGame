using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    internal class Alien : GameObject
    {
        private CircleCollider _circleCollider;
        private Texture2D _texture;
        private float playerClearance = 100;
        private float move_speed = 100f;
        private float speed_increase = 25f;

        public Alien() 
        {
            
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _texture = content.Load<Texture2D>("Alien");
            _circleCollider = new CircleCollider(Vector2.Zero, _texture.Width / 2);
            SetCollider(_circleCollider);
            RandomMove();
        }

        public override void OnCollision(GameObject other)
        {
            RandomMove();
            base.OnCollision(other);
        }

        public void RandomMove()
        {
            GameManager gm = GameManager.GetGameManager();
            _circleCollider.Center = gm.RandomScreenLocation();

            Vector2 centerOfPlayer = gm.Player.GetPosition().Center.ToVector2();
            while ((_circleCollider.Center - centerOfPlayer).Length() < playerClearance)
                _circleCollider.Center = gm.RandomScreenLocation();
        }

        public void Move(GameTime gameTime)
        {
            GameManager gm = GameManager.GetGameManager();
            var playerPos =  gm.Player.GetPosition().Center.ToVector2();
            Console.WriteLine("playerPos: " + playerPos);
            float x = _circleCollider.Center.X;
            float y = _circleCollider.Center.Y;
            Console.WriteLine("x: " + x + " y: " + y);
            if (_circleCollider.Center.X < playerPos.X)
            {
                Console.WriteLine("move_speed added to x: " + (int)(move_speed * (float)gameTime.ElapsedGameTime.TotalSeconds));
                x += (int)(move_speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else if (_circleCollider.Center.X > playerPos.X)
            {
                x -= (int)(move_speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (_circleCollider.Center.Y < playerPos.Y)
            {
                y += (int)(move_speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (_circleCollider.Center.Y > playerPos.Y)
            {
                y -= (int)(move_speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            Console.WriteLine("-- AFTER CHANGING-- x: " + x + " y: " + y);
            Console.WriteLine("moving alien: " + _circleCollider.Center + ", " + _circleCollider.Radius);
            _circleCollider.Center = new Vector2(x, y);
        }

        public override void Update(GameTime gameTime)
        {
            Move(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _circleCollider.GetBoundingBox(), Color.White);
            base.Draw(gameTime, spriteBatch);
        }


    }
}
