using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence;

public class AnimationPlayer : GameObject
{
    private GameManager _gm;
    private int _time;
    private GameObject _parent; // Add optional parent
                                // If no parent, assume player is being added to Game Manager as object (one off animations).
                                // If parent, assume it's a child of another object (repeating animations).
    private Texture2D _spriteSheet;
    private Rectangle _destRect; // rectangle to dictate where to draw the sprite
    private string _spriteName; // could add name as arg in Load to remove this field
    private int _frameTime;
    private bool _isPlaying;
    private bool _isFinished;
    private bool _loopAnimation;
    private int _frameAmount;
    private int _frameSize;
    private int _currentFrame;
     
    
    public AnimationPlayer(string spritename, int frametime, Rectangle dest, GameObject parent = null, bool loop = false)
    {
        _gm = GameManager.GetGameManager();
        _spriteName = spritename;
        _frameTime = frametime;
        _loopAnimation = loop;
        _destRect = dest;
        _isPlaying = false;
        _isFinished = false;
        _parent = parent;
        _time = 0;
    }
    public AnimationPlayer(Texture2D texture2D, int frametime, Rectangle dest, GameObject parent = null, bool loop = false)
    {
        _spriteSheet = texture2D;
        _gm = GameManager.GetGameManager();
        _frameTime = frametime;
        _loopAnimation = loop;
        _destRect = dest;
        _isPlaying = false;
        _isFinished = false;
        _parent = parent;
        _time = 0;
    }

    public void SetParent(GameObject obj) => _parent = obj;

    public void ToggleAnimationPlaying() => _isPlaying = !_isPlaying;
    public void PlayAnimation() => _isPlaying = true;
    public void ToggleLoop() => _loopAnimation = !_loopAnimation;
    public bool IsFinished() => _isFinished;
    public void Reset()
    {
        _isPlaying = true;
        _isFinished = false;
        _time = 0;
    }
    

    public void UpdateSpriteLocation(Rectangle dest) => _destRect = dest;
    
    public override void Load(ContentManager content)
    {
        base.Load(content);
        _spriteSheet ??= content.Load<Texture2D>(_spriteName);
        _frameSize = _spriteSheet.Height;
        _frameAmount = _spriteSheet.Width / _frameSize;
    }
    
        
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);
        if (_isFinished)
        {
            // Remove animation player from GM if it has no parent, just return if it does.
            if (_parent == null)
            {
                _gm.RemoveGameObject(this);
            }
            return;
        }
        
        if (_isPlaying)
        {
            _time += gameTime.ElapsedGameTime.Milliseconds;
        } 
        _currentFrame = _time / _frameTime;
        if (_loopAnimation)
            _currentFrame %= _frameAmount;
        else if (_currentFrame >= _frameAmount)
            _isFinished = true;
        
        var sourceRec = new Rectangle(_currentFrame * _frameSize, 0, _frameSize,_frameSize);
        spriteBatch.Draw(_spriteSheet, new Rectangle(_destRect.X, _destRect.Y, _frameSize, _frameSize), sourceRec, Color.White);
    }
}