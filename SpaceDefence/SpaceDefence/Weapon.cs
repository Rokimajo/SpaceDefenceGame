using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence;

public class Weapon : GameObject
{
    private string _textureName;
    protected GameManager _gm;
    protected Texture2D _texture;
    protected GameObject _projectile;
    protected Ship _parent;

    public Weapon(Texture2D texture)
    {
        _gm = GameManager.GetGameManager();
        _parent = _gm.Player;
        _texture = texture;
    }
    
    protected void SetProjectile(GameObject projectile) => _projectile = projectile;
    
    protected float GetAngle() =>
        LinePieceCollider.GetAngle(LinePieceCollider.GetDirection(_parent.GetPosition().Center, _parent.GetTarget()));

    
    protected Vector2 GetAimDirection() => 
        LinePieceCollider.GetDirection(_parent.GetPosition().Center, _parent.GetTarget());

    protected Vector2 GetTurretExit() => _parent.GetPosition().Center.ToVector2() +
                                       GetAimDirection() * _texture.Height / 2f;

    public virtual void Shoot()
    {
        if (_projectile != null)
            _gm.AddGameObject(_projectile);
    }
    
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        Rectangle turretLocation = _texture.Bounds;
        turretLocation.Location = _parent.GetPosition().Center;
        spriteBatch.Draw(_texture, turretLocation, null, Color.White, GetAngle(), turretLocation.Size.ToVector2() / 2f, SpriteEffects.None, 0);
        base.Draw(gameTime, spriteBatch);
    }
}