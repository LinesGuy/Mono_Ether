using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;

namespace Mono_Ether {
    public static class GameSettings {

        public const string SettingsFilename = "settings.txt"; // TODO rename to Settings.txt
        public static Vector2 ScreenSize = new Vector2(1366, 768); // TODO allow this to change
        public static bool DebugMode = false;
        public static bool VSync = true;
        public static bool ShowFPS = true;
        public static bool AllowWindowResizing = true;
        public static float MasterVolume;
        public static float MusicVolume;
        public static float SoundEffectVolume;
        public static void ApplyChanges() {
            SoundEffect.MasterVolume = GameSettings.MasterVolume * GameSettings.SoundEffectVolume;
            MediaPlayer.Volume = GameSettings.MasterVolume * GameSettings.MusicVolume;
            GameRoot.Instance.Graphics.PreferredBackBufferWidth = (int)ScreenSize.X;
            GameRoot.Instance.Graphics.PreferredBackBufferHeight = (int)ScreenSize.Y;
            
            if (VSync) {
                GameRoot.Instance.Graphics.SynchronizeWithVerticalRetrace = true;
                GameRoot.Instance.IsFixedTimeStep = true;
            } else {
                GameRoot.Instance.Graphics.SynchronizeWithVerticalRetrace = false;
                GameRoot.Instance.IsFixedTimeStep = false;
            }
            GameRoot.Instance.Window.AllowUserResizing = AllowWindowResizing;
            GameRoot.Instance.Graphics.ApplyChanges();
        }
        public static void LoadSettings() {
            // TODO Use for-loop and dictionary to read settings
            // TODO add ScreenSize to settings.txt
            if (File.Exists(SettingsFilename)) {
                string[] lines = File.ReadAllText(SettingsFilename).Split("\n");
                float.TryParse(lines[0], out MasterVolume);
                float.TryParse(lines[1], out MusicVolume);
                float.TryParse(lines[2], out SoundEffectVolume);
            } else {
                // Default settings
                File.WriteAllText(SettingsFilename, "0.05\n0.05\n0.8");
                LoadSettings();
            }
            ApplyChanges();
        }
        public static void SaveSettings() {
            // TODO Use for-loop and dictionary to save settings
            File.WriteAllText(SettingsFilename, $"{MasterVolume}\n{MusicVolume}\n{SoundEffectVolume}");
        }

        public static void OnScreenResize(Object sender, EventArgs e) {
            ScreenSize.X = GameRoot.Instance.Window.ClientBounds.Width;
            ScreenSize.Y = GameRoot.Instance.Window.ClientBounds.Height;
        }
    }
}
