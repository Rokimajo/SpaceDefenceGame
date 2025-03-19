using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class GameManager
    {
        private static GameManager gameManager;

        private GameState _gameState;
        private List<GameObject> _gameObjects;
        private List<GameObject> _toBeRemoved;
        private List<GameObject> _toBeAdded;
        private ContentManager _content;
        private float _diffScaleTimer;

        public Random RNG { get; private set; }
        public Ship Player { get; private set; }
        public InputManager InputManager { get; private set; }
        public Game Game { get; private set; }

        public static GameManager GetGameManager()
        {
            if(gameManager == null)
                gameManager = new GameManager();
            return gameManager;
        }
        public GameManager()
        {
            _gameObjects = new List<GameObject>();
            _toBeRemoved = new List<GameObject>();
            _toBeAdded = new List<GameObject>();
            InputManager = new InputManager();
            RNG = new Random();
            _gameState = GameState.GameStart;
            _diffScaleTimer = 0.5f;
        }

        public void Initialize(ContentManager content, Game game, Ship player)
        {
            Game = game;
            _content = content;
            Player = player;
        }

        public void Load(ContentManager content)
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Load(content);
            }
        }

        public void SetGameState(GameState gameState)
        {
            _gameState = gameState;
        }

        public void HandleInput(InputManager inputManager)
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.HandleInput(this.InputManager);
            }
        }

        public void CheckCollision()
        {
            // Checks once for every pair of 2 GameObjects if the collide.
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                for (int j = i+1; j < _gameObjects.Count; j++)
                {
                    if (_gameObjects[i].CheckCollision(_gameObjects[j]))
                    {
                        _gameObjects[i].OnCollision(_gameObjects[j]);
                        _gameObjects[j].OnCollision(_gameObjects[i]);
                    }
                }
            }
            
        }

        public void CheckDiffScaling(GameTime gameTime)
        {
            // Check if elapsed game time matches the scale timer field, spawn more enemies if so
            // e.g.: field is 2, every 2 minutes: spawn more enemies
            if (_gameState == GameState.GameRunning && gameTime.TotalGameTime.TotalSeconds >= (_diffScaleTimer * 60))
            {
                _diffScaleTimer += _diffScaleTimer;
                // 25% chance to spawn another alien, 75% to spawn asteroid
                if (RNG.Next(0, 4) == 0)
                {
                    AddGameObject(new Alien());
                }
                else
                {
                    AddGameObject(new Asteroid());
                }
            }
        }

        public void CheckGameOver()
        {
            if (_gameState == GameState.GameRunning && _gameObjects.Find(x => x is Ship) == null)
            {
                SetGameState(GameState.GameOver);
                RemoveAllGameObjects();
                AddGameObject(new GameOverScreen());
            }
        }
        
        public void Update(GameTime gameTime) 
        {
            CheckGameOver();
            CheckDiffScaling(gameTime);
            
            InputManager.Update();
            // Check input on GM update and pause the game if the key is pressed.
            if (InputManager.IsKeyPress(Keys.Space) && _gameState == GameState.GameRunning)
            {
                SetGameState(GameState.GamePaused);
                AddGameObject(new PauseScreen());
            }
                
            switch (_gameState)
            {
                case GameState.GameStart:
                {
                    GameStart_Update(gameTime);
                    break;
                }
                case GameState.GameRunning:
                {
                    GameRunning_Update(gameTime);
                    break;
                }
                case GameState.GamePaused:
                {
                    GamePaused_Update(gameTime);
                    break;
                }
                case GameState.GameOver:
                {
                    GameOver_Update(gameTime);
                    break;
                }
                default:
                {
                    throw new NotImplementedException();
                }
            }
        }

        public void GameOver_Update(GameTime gameTime)
        {
            // only add game objects that need to be added (such as the game over screen), do nothing else.
            foreach (GameObject gameObject in _toBeAdded)
            {
                gameObject.Load(_content);
                _gameObjects.Add(gameObject);
            }
            _toBeAdded.Clear();
        }

        public void GamePaused_Update(GameTime gameTime)
        {
            foreach (GameObject gameObject in _toBeAdded)
            {
                gameObject.Load(_content);
                _gameObjects.Add(gameObject);
            }
            _toBeAdded.Clear();
            
            var pauseScreen = _gameObjects.Find(gameObject => gameObject is PauseScreen);
            if (pauseScreen != null)
            {
                pauseScreen.HandleInput(InputManager);
                pauseScreen.Update(gameTime);
            }
            
            foreach (GameObject gameObject in _toBeRemoved)
            {
                gameObject.Destroy();
                _gameObjects.Remove(gameObject);
            }
            _toBeRemoved.Clear();
        }

        public void GameStart_Update(GameTime gameTime)
        {
            HandleInput(InputManager);
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Update(gameTime);
            }
            foreach (GameObject gameObject in _toBeAdded)
            {
                gameObject.Load(_content);
                _gameObjects.Add(gameObject);
            }
            _toBeAdded.Clear();

            foreach (GameObject gameObject in _toBeRemoved)
            {
                gameObject.Destroy();
                _gameObjects.Remove(gameObject);
            }
            _toBeRemoved.Clear();
        }

        public void GameRunning_Update(GameTime gameTime)
        {

            // Handle input
            HandleInput(InputManager);
            
            // Update
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Update(gameTime);
            }

            // Check Collision
            CheckCollision();

            foreach (GameObject gameObject in _toBeAdded)
            {
                gameObject.Load(_content);
                _gameObjects.Add(gameObject);
            }
            _toBeAdded.Clear();

            foreach (GameObject gameObject in _toBeRemoved)
            {
                gameObject.Destroy();
                _gameObjects.Remove(gameObject);
            }
            _toBeRemoved.Clear();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin();
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Add a new GameObject to the GameManager. 
        /// The GameObject will be added at the start of the next Update step. 
        /// Once it is added, the GameManager will ensure all steps of the game loop will be called on the object automatically. 
        /// </summary>
        /// <param name="gameObject"> The GameObject to add. </param>
        public void AddGameObject(GameObject gameObject)
        {
            _toBeAdded.Add(gameObject);
        }

        /// <summary>
        /// Remove GameObject from the GameManager. 
        /// The GameObject will be removed at the start of the next Update step and its Destroy() mehtod will be called.
        /// After that the object will no longer receive any updates.
        /// </summary>
        /// <param name="gameObject"> The GameObject to Remove. </param>
        public void RemoveGameObject(GameObject gameObject)
        {
            _toBeRemoved.Add(gameObject);
        }

        public void DestroyAllGameObjects()
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                _toBeRemoved.Add(gameObject);
            }
        }

        public void RemoveAllGameObjects() => _gameObjects.Clear();

        /// <summary>
        /// Get a random location on the screen.
        /// </summary>
        public Vector2 RandomScreenLocation()
        {
            return new Vector2(
                RNG.Next(0, Game.GraphicsDevice.Viewport.Width),
                RNG.Next(0, Game.GraphicsDevice.Viewport.Height));
        }

    }
}
