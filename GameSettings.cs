using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Diagnostics;
using System.IO;

namespace Mono_Ether {
    public static class GameSettings {

        public const string SettingsFilename = "settings.txt"; // TODO rename to Settings.txt
        public static Vector2 ScreenSize = new Vector2(1800, 900); // TODO allow this to change
        public static bool DebugMode = true;
        //public static bool VSync = true;
        public static bool ShowFps = true;
        public static bool AllowWindowResizing = false;
        public static float MasterVolume;
        public static float MusicVolume;
        public static float SoundEffectVolume;
        public static void ApplyChanges() {
            ApplyVolumeChanges();
            GameRoot.Instance.Graphics.PreferredBackBufferWidth = (int)ScreenSize.X;
            GameRoot.Instance.Graphics.PreferredBackBufferHeight = (int)ScreenSize.Y;

            //GameRoot.Instance.Graphics.SynchronizeWithVerticalRetrace = VSync;
            //GameRoot.Instance.IsFixedTimeStep = VSync;

            GameRoot.Instance.Window.AllowUserResizing = AllowWindowResizing;
            GameRoot.Instance.Graphics.ApplyChanges();
        }
        public static void ApplyVolumeChanges(float multiplier = 1f) {
            SoundEffect.MasterVolume = MasterVolume * SoundEffectVolume * multiplier;
            MediaPlayer.Volume = MasterVolume * MusicVolume * multiplier;
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
        public static void OnScreenResize(object sender, EventArgs e) {
            ScreenSize.X = GameRoot.Instance.Window.ClientBounds.Width;
            ScreenSize.Y = GameRoot.Instance.Window.ClientBounds.Height;
        }
    }
}
