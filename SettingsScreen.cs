using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;

namespace Mono_Ether {
    public class SettingsScreen : GameState
    {
        private ButtonManager buttonManager = new ButtonManager();
        private Song _music;
        public SettingsScreen(GraphicsDevice graphicsDevice) : base(graphicsDevice) {

        }
        public override void Initialize() {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_music);
        }

        public override void LoadContent(ContentManager content) {
            _music = content.Load<Song>("Songs/Settings");
        }

        public override void UnloadContent() {
            _music.Dispose();
        }

        public override void Update(GameTime gameTime) {
            /* Reset scene if user pressed R */
            
        }
        public override void Draw(SpriteBatch batch) {
            
        }
    }
}
