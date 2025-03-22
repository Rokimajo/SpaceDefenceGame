using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceDefence.Collision;

namespace SpaceDefence;

public class StartScreen : GameObject
{
    private SpriteFont _splashFont;
    private Rectangle _gameBounds;
    private string _message = "SPACE DEFENSE";
    private Vector2 _splashLoc;
    private Button _startButton;
    private Button _quitButton;
    private int buttonWidth = 100; 
    private int buttonHeight = 40;
    private int padding = 25;
    
    public StartScreen()
    {
        var gm = GameManager.GetGameManager();
        _gameBounds = gm.Game.GraphicsDevice.Viewport.Bounds;
        _splashLoc = new Vector2(_gameBounds.Width / 2f, _gameBounds.Height / 2f);
        _startButton = new Button(new RectangleCollider(new Rectangle(new Point((int) _splashLoc.X - buttonWidth / 2, (int)(_splashLoc.Y + buttonHeight + padding)), new Point(buttonWidth, buttonHeight))), 
            "start", Color.DarkGreen);
        _quitButton = new Button(new RectangleCollider(new Rectangle(
            new Point((int) _splashLoc.X - buttonWidth / 2, _startButton.shape.Y + buttonHeight + padding), new Point(buttonWidth, buttonHeight))), 
            "quit", Color.DarkRed);
    }

    public override void Load(ContentManager content)
    {
        base.Load(content);
        _splashFont = content.Load<SpriteFont>("GameOverFont");
        _startButton.Load(content);
        _quitButton.Load(content);
    }
    
    public override void HandleInput(InputManager inputManager)
    {
        base.HandleInput(inputManager);
        var target = inputManager.CurrentMouseState.Position;
        if(inputManager.LeftMousePress())
        {
            if (_startButton.Contains(target.ToVector2()))
            {
                var gm = GameManager.GetGameManager();
                gm.RemoveGameObject(this);
                gm.AddGameObject(gm.Player);
                gm.AddGameObject(new Alien());
                gm.AddGameObject(new Asteroid());
                gm.AddGameObject(new Asteroid());
                gm.AddGameObject(new Supply());
                // Generate planets on fixed positions based on screen size
                gm.AddGameObject(new Planet("earth_planet", new Vector2(50, 50), true));
                var bounds = gm.Game.GraphicsDevice.Viewport.Bounds;
                gm.AddGameObject(new Planet("alien_planet", new Vector2(bounds.Width - 75, bounds.Height - 100), false));
                gm.SetGameState(GameState.GameRunning);
            }

            if (_quitButton.Contains(target.ToVector2()))
            {
                GameManager.GetGameManager().Game.Exit();
            }
        }
        
    }
    
    public override void Update(GameTime gameTime)
    {
        
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_splashFont, _message, _splashLoc, Color.White, 0, _splashFont.MeasureString(_message) / 2, 1.0f, SpriteEffects.None, 1f);
        _startButton.Draw(gameTime, spriteBatch);
        _quitButton.Draw(gameTime, spriteBatch);
        base.Draw(gameTime, spriteBatch);
    }
}