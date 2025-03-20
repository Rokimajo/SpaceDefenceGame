using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    internal class Alien : GameObject
    {
        private GameManager _gm;
        private CircleCollider _circleCollider;
        private AnimationPlayer _animPlayer;
        private Texture2D _texture;
        private float playerClearance = 100;
        private float alienKillRadius = 75;
        private float move_speed = 125f;
        private float speed_increase = 10f;

        public Alien()
        {
            _gm = GameManager.GetGameManager();
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _texture = content.Load<Texture2D>("Alien");
            _circleCollider = new CircleCollider(Vector2.Zero, _texture.Width / 2);
            _animPlayer = new AnimationPlayer("Explosion", 15, _circleCollider.GetBoundingBox());
            _animPlayer.Load(content);
            SetCollider(_circleCollider);
            RandomMove();
        }

        public void PlayAnimation()
        {
            _animPlayer.Reset();
            _animPlayer.UpdateSpriteLocation(_circleCollider.GetBoundingBox());
            _gm.AddGameObject(_animPlayer);
        }

        public override void OnCollision(GameObject other)
        {
            // Increase speed on alien death (don't count collisions with other aliens)
            if (other is not Alien)
            {
                move_speed += speed_increase;
                PlayAnimation();
                RandomMove();
                base.OnCollision(other);
            }
        }

        public void RandomMove()
        {
            GameManager gm = GameManager.GetGameManager();
            _circleCollider.Center = gm.RandomScreenLocation();

            Vector2 centerOfPlayer = gm.Player.GetPosition().Center.ToVector2();
            while ((_circleCollider.Center - centerOfPlayer).Length() < playerClearance)
                _circleCollider.Center = gm.RandomScreenLocation();
        }

        public bool CheckAlienRange()
        {
            GameManager gm = GameManager.GetGameManager();
            Vector2 centerOfPlayer = gm.Player.GetPosition().Center.ToVector2();
            if ((_circleCollider.Center - centerOfPlayer).Length() < alienKillRadius)
            {
                return true;
            }

            return false;
        }

        public void Move(GameTime gameTime)
        {
            GameManager gm = GameManager.GetGameManager();
            var playerPos =  gm.Player.GetPosition().Center.ToVector2();
            float x = _circleCollider.Center.X;
            float y = _circleCollider.Center.Y;
            var rD2 = _circleCollider.Radius;
            if (_circleCollider.Center.X < playerPos.X - rD2)
            {
                x += (int)(move_speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else if (_circleCollider.Center.X > playerPos.X + rD2)
            {
                x -= (int)(move_speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (_circleCollider.Center.Y < playerPos.Y - rD2)
            {
                y += (int)(move_speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (_circleCollider.Center.Y > playerPos.Y + rD2)
            {
                y -= (int)(move_speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            _circleCollider.Center = new Vector2(x, y);
        }

        public override void Update(GameTime gameTime)
        {
            Move(gameTime);
            base.Update(gameTime);
            // Game Over if alien is within range
            if (CheckAlienRange())
            {
                var gm = GameManager.GetGameManager();
                gm.RemoveGameObject(gm.Player);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _circleCollider.GetBoundingBox(), Color.White);
            _animPlayer.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
