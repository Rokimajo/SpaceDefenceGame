using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceDefence.Collision;

namespace SpaceDefence;

public class Button : GameObject
{
    private SpriteFont _textFont;
    private Texture2D _buttonTexture;
    private Color _buttonColor;
    private RectangleCollider _buttonBounds;
    private string _message;
    private Vector2 _splashLoc;
    private int buttonWidth = 100;
    private int buttonHeight = 40;
    
    public Rectangle shape => _buttonBounds.shape;
    
    public Button(RectangleCollider collider, string message, Color color)
    {
        _buttonBounds = collider;
        _message = message;
        _buttonColor = color;
    }

    public override void Load(ContentManager content)
    {
        var gm = GameManager.GetGameManager();
        base.Load(content);
        _textFont = content.Load<SpriteFont>("PixelFont");
        _buttonTexture = new Texture2D(gm.Game.GraphicsDevice, 1, 1);
        _buttonTexture.SetData(new[] { _buttonColor });
    }

    public bool Contains(Vector2 v) => _buttonBounds.Contains(v);
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
        spriteBatch.Draw(_buttonTexture, shape, null, Color.White, 0,
            Vector2.Zero, SpriteEffects.None, 0);
        spriteBatch.DrawString(_textFont, _message, shape.Center.ToVector2(), Color.White, 0, _textFont.MeasureString(_message) / 2, 1.0f, SpriteEffects.None, 1f);

    }
}