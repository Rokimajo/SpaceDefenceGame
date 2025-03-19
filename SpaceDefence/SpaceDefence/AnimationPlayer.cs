using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence;

public class AnimationPlayer : GameObject
{
    private string _spriteName;
    private int _frameTime;
     
    
    public AnimationPlayer(string spritename, int frametime)
    {
        _spriteName = spritename;
        _frameTime = frametime;
    }

    public override void Load(ContentManager content)
    {
        base.Load(content);
    }
    
        
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
    }
}