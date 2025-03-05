using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence;

public class GameOverScreen : GameObject
{
    private SpriteFont _font;
    private Rectangle _gameBounds;
    private string _message = "GAME OVER!";
    
    public GameOverScreen()
    {
        var gm = GameManager.GetGameManager();
        _gameBounds = gm.Game.GraphicsDevice.Viewport.Bounds;
    }

    public override void Load(ContentManager content)
    {
        base.Load(content);
        _font = content.Load<SpriteFont>("GameOverFont");
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font, _message, new Vector2(_gameBounds.Width / 2f, _gameBounds.Height / 2f), Color.White, 0, _font.MeasureString(_message) / 2, 1.0f, SpriteEffects.None, 1f);
        base.Draw(gameTime, spriteBatch);
    }
}