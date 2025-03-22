using System.Buffers.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence;

public class BulletWeapon : Weapon
{
    private int _bulletSpeed = 150;
    public BulletWeapon(Texture2D texture) : base(texture) {}
    
    public override void Shoot()
    {
        var bullet = new Bullet(GetTurretExit(), GetAimDirection(), _bulletSpeed);
        SetProjectile(bullet);
        base.Shoot();
    }
}