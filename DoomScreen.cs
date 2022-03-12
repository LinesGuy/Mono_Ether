using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mono_Ether.Ether {
    public class DoomScreen : GameState {
        private GameMode _mode = GameMode.Playing;
        private List<PlayerShip> _players = EntityManager.Instance.Players;
        private readonly PauseWindow _pauseWindow = new PauseWindow();
        private readonly TileMap _tileMap = new TileMap(Level.Secret, false);
        private TimeSpan _timeRemaining = TimeSpan.FromSeconds(60);
        public DoomScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {
        }
        public override void Initialize() {
            /*MediaPlayer.IsRepeating = true; TODO doom music
            MediaPlayer.Play(Sounds.Music);*/
            _players.ForEach(p => p.DoomMode = true);
        }
        public override void Suspend() {
            throw new NotImplementedException();
        }

        public override void Resume() {
            throw new NotImplementedException();
        }
        public override void LoadContent(ContentManager content) {
        }
        public override void UnloadContent() {
            EntityManager.Instance.Players.ForEach(p => p.DoomMode = false);
        }
        public override void Update(GameTime gameTime) {
            if (!GameRoot.Instance.IsActive)
                return;
            // Esc to toggle pause
            _timeRemaining -= gameTime.ElapsedGameTime;
            if (_timeRemaining <= TimeSpan.Zero) {
                ScreenManager.RemoveScreen();
            }
            if (Input.WasKeyJustDown(Keys.Escape)) {
                switch (_mode) {
                    case GameMode.Playing:
                        _mode = GameMode.Paused;
                        _pauseWindow.Pause();
                        break;
                    case GameMode.Paused:
                        _mode = GameMode.Playing;
                        _pauseWindow.UnPause();
                        break;
                }
            }
            _tileMap.Update();
            if (_mode != GameMode.Paused) {
                _players.ForEach(p => p.Update(gameTime));
            }
            _pauseWindow.Update(gameTime);

        }
        public override void Draw(SpriteBatch batch) {
            GraphicsDevice.Clear(Color.Black);
            batch.Begin();
            Doom.Draw(batch, _players[0]);
            batch.DrawStringCentered(GlobalAssets.NovaSquare24, "Find a red block to continue", new Vector2(GameSettings.ScreenSize.X / 2f, GameSettings.ScreenSize.Y / 3f), Color.White);
            if (_pauseWindow.Visible)
                _pauseWindow.Draw(batch);
            batch.Draw(GameScreen.GameCursor, Input.Mouse.Position.ToVector2() - GameScreen.GameCursor.Size() / 2f, Color.White);
            if (!GameRoot.Instance.IsActive)
                batch.DrawString(GlobalAssets.NovaSquare24, "GAME IS UNFOCUSED, CLICK ANYWHERE TO FOCUS WINDOW", GameSettings.ScreenSize / 4f, Color.White);
            batch.End();
        }
    }
    public static class Doom {
        private const float Fov = MathF.PI * 0.8f;
        private const float StepDist = 4;
        private const int MaxSteps = 512;
        private const float MaxDist = StepDist * MaxSteps;
        public static void Draw(SpriteBatch spriteBatch, PlayerShip player) {
            spriteBatch.Draw(GlobalAssets.Pixel, new Rectangle(0, 0, (int)(player.PlayerCamera.ScreenSize.X), (int)(player.PlayerCamera.ScreenSize.Y / 2f)), Color.CornflowerBlue);
            spriteBatch.Draw(GlobalAssets.Pixel, new Rectangle(0, (int)(player.PlayerCamera.ScreenSize.Y / 2f), (int)(player.PlayerCamera.ScreenSize.X), (int)(player.PlayerCamera.ScreenSize.Y / 2f)), new Color(32, 128, 32));
            Vector2 p1Pos = player.Position;
            float p1Angle = player.Orientation;
            List<float> dists = new List<float>();
            List<float> texXs = new List<float>();
            List<int> Ids = new List<int>();
            for (int i = 0; i < (int)player.PlayerCamera.ScreenSize.X; i++) {
                dists.Add(0);
                texXs.Add(-1f);
                Ids.Add(0);
            }
            Parallel.For(0, (int)player.PlayerCamera.ScreenSize.X, i => {
                float deltaAngle = MyUtils.Interpolate(-Fov / 2f, Fov / 2f, (player.PlayerCamera.ScreenSize.X - i) / player.PlayerCamera.ScreenSize.X);
                float rayAngle = p1Angle - deltaAngle;
                int Id = -1;
                int step;
                Vector2 currentRayPos = p1Pos;
                Vector2 stepVector = MyUtils.FromPolar(rayAngle, StepDist);
                float dist = 0f;
                for (step = 0; step <= MaxSteps; step++) {

                    Id = TileMap.Instance.GetTileFromWorld(currentRayPos).Id;
                    if (Id > 0) {
                        // Collision! Let's find the nearest intersection between the ray and tile
                        var tile = TileMap.Instance.GetTileFromWorld(currentRayPos);
                        Vector2 nearestCollision = Vector2.Zero;
                        float nearestDistSquared = float.MaxValue;

                        float gradient = stepVector.Y / stepVector.X;
                        float m = -gradient;
                        float c = -currentRayPos.Y + gradient * currentRayPos.X;

                        // Left wall
                        float ty = -c - m * tile.Left.X;
                        Tile upperTile = TileMap.Instance.GetTileFromMap(tile.Pos - new Vector2(0, 1));
                        Tile lowerTile = TileMap.Instance.GetTileFromMap(tile.Pos + new Vector2(0, 1));
                        if (upperTile.SolidWalls[0] && ty < tile.Top.Y) {

                            Vector2 collision = new Vector2(upperTile.Left.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - upperTile.Top.Y) / (upperTile.Bottom.Y - upperTile.Top.Y);
                                Id = upperTile.Id;
                            }
                        } else if (tile.SolidWalls[0] && tile.Bottom.Y > ty && ty > tile.Top.Y) {
                            Vector2 collision = new Vector2(tile.Left.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - tile.Top.Y) / (tile.Bottom.Y - tile.Top.Y);
                                Id = tile.Id;
                            }
                        } else if (lowerTile.SolidWalls[0] && tile.Bottom.Y < ty) {

                            Vector2 collision = new Vector2(lowerTile.Left.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - lowerTile.Top.Y) / (lowerTile.Bottom.Y - lowerTile.Top.Y);
                                Id = lowerTile.Id;
                            }
                        }
                        // Top wall
                        float tx = (c + tile.Top.Y) / gradient;
                        Tile leftTile = TileMap.Instance.GetTileFromMap(tile.Pos - new Vector2(1, 0));
                        Tile rightTile = TileMap.Instance.GetTileFromMap(tile.Pos + new Vector2(1, 0));
                        if (leftTile.SolidWalls[1] && tile.Left.X > tx) {
                            Vector2 collision = new Vector2(tx, leftTile.Top.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - leftTile.Right.X) / (leftTile.Left.X - leftTile.Right.X);
                                Id = leftTile.Id;
                            }
                        } else if (tile.SolidWalls[1] && tile.Left.X < tx && tx < tile.Right.X) {
                            Vector2 collision = new Vector2(tx, tile.Top.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - tile.Right.X) / (tile.Left.X - tile.Right.X);
                                Id = tile.Id;
                            }
                        } else if (rightTile.SolidWalls[1] && tx > tile.Right.X) {
                            Vector2 collision = new Vector2(tx, rightTile.Top.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - rightTile.Right.X) / (rightTile.Left.X - rightTile.Right.X);
                                Id = rightTile.Id;
                            }
                        }
                        // Right wall
                        ty = -c - m * tile.Right.X;
                        upperTile = TileMap.Instance.GetTileFromMap(tile.Pos - new Vector2(0, 1));
                        lowerTile = TileMap.Instance.GetTileFromMap(tile.Pos + new Vector2(0, 1));
                        if (upperTile.SolidWalls[2] && ty < tile.Top.Y) {
                            Vector2 collision = new Vector2(upperTile.Right.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - upperTile.Top.Y) / (upperTile.Bottom.Y - upperTile.Top.Y);
                                Id = upperTile.Id;
                            }
                        } else if (tile.SolidWalls[2] && tile.Bottom.Y > ty && ty > tile.Top.Y) {
                            Vector2 collision = new Vector2(tile.Right.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - tile.Top.Y) / (tile.Bottom.Y - tile.Top.Y);
                                Id = tile.Id;
                            }
                        } else if (lowerTile.SolidWalls[2] && tile.Bottom.Y < ty) {
                            Vector2 collision = new Vector2(lowerTile.Right.X, ty);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.Y - lowerTile.Top.Y) / (lowerTile.Bottom.Y - lowerTile.Top.Y);
                                Id = lowerTile.Id;
                            }
                        }
                        // Bottom wall
                        tx = (c + tile.Bottom.Y) / gradient;
                        leftTile = TileMap.Instance.GetTileFromMap(tile.Pos - new Vector2(1, 0));
                        rightTile = TileMap.Instance.GetTileFromMap(tile.Pos + new Vector2(1, 0));
                        if (leftTile.SolidWalls[3] && tile.Left.X > tx) {
                            Vector2 collision = new Vector2(tx, leftTile.Bottom.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - leftTile.Right.X) / (leftTile.Left.X - leftTile.Right.X);
                                Id = leftTile.Id;
                            }
                        } else if (tile.SolidWalls[3] && tile.Left.X < tx && tx < tile.Right.X) {
                            Vector2 collision = new Vector2(tx, tile.Bottom.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - tile.Right.X) / (tile.Left.X - tile.Right.X);
                                Id = tile.Id;
                            }
                        } else if (rightTile.SolidWalls[3] && tx > tile.Right.X) {
                            Vector2 collision = new Vector2(tx, rightTile.Bottom.Y);
                            float distSquared = Vector2.DistanceSquared(p1Pos, collision);
                            if (distSquared < nearestDistSquared) {
                                nearestCollision = collision;
                                nearestDistSquared = distSquared;
                                texXs[(int)i] = (collision.X - rightTile.Right.X) / (rightTile.Left.X - rightTile.Right.X);
                                Id = rightTile.Id;
                            }
                        }
                        dist = MathF.Sqrt(nearestDistSquared);
                        dists[(int)i] = dist;
                        Ids[(int)i] = Id;
                        break;
                    }
                    currentRayPos += stepVector;
                }
            });
            for (int i = 0; i < (int)player.PlayerCamera.ScreenSize.X; i++) {
                float dist = dists[i];
                float texX = texXs[i];
                int Id = Ids[i];
                float ed = dist / 32f;
                if (texX >= 0) {
                    Texture2D img = Tile.Textures[0];
                    if (Id == 2)
                        img = Tile.Textures[1];
                    if (Id == 3)
                        img = Tile.Textures[2];
                    if (Id == 4)
                        img = Tile.Textures[3];
                    int c = (int)(255f * (1 - dist / MaxDist));
                    spriteBatch.Draw(img, new Vector2(i, player.PlayerCamera.ScreenSize.Y / 2f), new Rectangle((int)(Tile.Textures[0].Width * texX), 0, 1, Tile.Textures[0].Height), new Color(255, 255, 255, c), 0f, new Vector2(32f, 32f), new Vector2(1, 16f / ed), 0, 0);
                }
            }
        }
    }
}
