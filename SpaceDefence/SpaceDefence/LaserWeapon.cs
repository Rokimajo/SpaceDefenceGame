using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence;

public class LaserWeapon : Weapon
{
    private int _laserLength = 400;
    public LaserWeapon(Texture2D texture) : base(texture) {}

    public override void Shoot()
    {
        var laser = new Laser(new LinePieceCollider(GetTurretExit(), _parent.GetTarget().ToVector2()), _laserLength);
        SetProjectile(laser);
        base.Shoot();
    }
}