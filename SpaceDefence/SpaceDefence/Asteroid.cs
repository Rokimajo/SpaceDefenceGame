using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence;

public class Asteroid : GameObject
{
    private CircleCollider _circleCollider;
        private Texture2D _texture;
        private float playerClearance = 200;

        public Asteroid() 
        {
            
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _texture = content.Load<Texture2D>("asteroid");
            _circleCollider = new CircleCollider(Vector2.Zero, _texture.Width / 2);
            SetCollider(_circleCollider);
            RandomMove();
        }
        
        public void RandomMove()
        {
            GameManager gm = GameManager.GetGameManager();
            _circleCollider.Center = gm.RandomScreenLocation();

            Vector2 centerOfPlayer = gm.Player.GetPosition().Center.ToVector2();
            while ((_circleCollider.Center - centerOfPlayer).Length() < playerClearance)
                _circleCollider.Center = gm.RandomScreenLocation();
        }

        public override void OnCollision(GameObject other)
        {
            // ugly way to not make an asteroid destroy a planet,
            // in actual game would probably make a proper spawning system based on all other objects
            if (other is Planet)
                return;
            base.OnCollision(other);
            var gm = GameManager.GetGameManager();
            // Destroy both the asteroid and the other game object on collision
            gm.RemoveGameObject(this);
            gm.RemoveGameObject(other);
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _circleCollider.GetBoundingBox(), Color.White);
            base.Draw(gameTime, spriteBatch);
        }


}