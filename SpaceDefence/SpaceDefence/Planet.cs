using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence;

public class Planet : GameObject
{
    private CircleCollider _circleCollider;
    private Vector2 _planetLoc;
    private string _textureName;
    private Texture2D _texture;
    private AnimationPlayer _animPlayer;
    // True if planet gives cargo, false if it takes cargo.
    private bool _isCargoPickup;
    private int _cargoPoints = 500;

    public Planet(string textureName, Vector2 loc, bool isCargo) 
    {
        _textureName = textureName;
        _planetLoc = loc;
        _isCargoPickup = isCargo;
    }

    public override void Load(ContentManager content)
    {
        base.Load(content);
        _texture = content.Load<Texture2D>(_textureName);
        // Grab height due to planet being a spritesheet
        _circleCollider = new CircleCollider(Vector2.Zero, _texture.Height / 2);
        _animPlayer = new AnimationPlayer(_texture, 100, _circleCollider.GetBoundingBox(), this, true);
        _animPlayer.Load(content);
        _animPlayer.PlayAnimation();
        SetCollider(_circleCollider);
        _circleCollider.Center = _planetLoc;
    }
    

    public override void OnCollision(GameObject other)
    {
        base.OnCollision(other);
        if (other is Ship player)
        {
            if (_isCargoPickup && !player.HasCargo())
            {
                player.AddCargo();
            }
            else if (!_isCargoPickup && player.HasCargo())
            {
                player.RemoveCargo();
                player.AddPoints(_cargoPoints);
            }
        }
    }
        
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
        // Could be removed since planets have a static location
        _animPlayer.UpdateSpriteLocation(_circleCollider.GetBoundingBox());
        _animPlayer.Draw(gameTime, spriteBatch);
    }


}