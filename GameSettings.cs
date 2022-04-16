using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Diagnostics;
using System.IO;

namespace Mono_Ether {
    public static class GameSettings {

        public const string SettingsFilename = "Settings.txt";
        public static Vector2 ScreenSize = new Vector2(1800, 900);
        public static bool DebugMode;
        public static bool VSync;
        public static bool ShowFps;
        public static bool AllowWindowResizing;
        public static float MasterVolume;
        public static float MusicVolume;
        public static float SoundEffectVolume;
        public static void ApplyChanges() {
            ApplyVolumeChanges();
            // Set window size
            GameRoot.Instance.Graphics.PreferredBackBufferWidth = (int)ScreenSize.X;
            GameRoot.Instance.Graphics.PreferredBackBufferHeight = (int)ScreenSize.Y;
            // Set vsync
            GameRoot.Instance.Graphics.SynchronizeWithVerticalRetrace = VSync;
            GameRoot.Instance.IsFixedTimeStep = VSync;
            // Set window resizing
            GameRoot.Instance.Window.AllowUserResizing = AllowWindowResizing;
            // Apply changes
            GameRoot.Instance.Graphics.ApplyChanges();
        }
        public static void ApplyVolumeChanges(float multiplier = 1f) {
            SoundEffect.MasterVolume = MasterVolume * SoundEffectVolume * multiplier;
            MediaPlayer.Volume = MasterVolume * MusicVolume * multiplier;
        }
        public static void LoadSettings() {
            // Loads settings from the settings file if it exists, otherwise creates a new settings file.
            if (File.Exists(SettingsFilename)) {
                string[] lines = File.ReadAllText(SettingsFilename).Split("\n");
                float.TryParse(lines[0], out MasterVolume);
                float.TryParse(lines[1], out MusicVolume);
                float.TryParse(lines[2], out SoundEffectVolume);
                DebugMode = lines[3] == "true";
                VSync = lines[4] == "true";
                ShowFps = lines[5] == "true";
                AllowWindowResizing = lines[6] == "true";
            } else {
                // Default settings
                File.WriteAllText(SettingsFilename, "0.05\n0.05\n0.8\ntrue\ntrue\ntrue\nfalse");
                LoadSettings();
            }
            ApplyChanges();
        }
        public static void SaveSettings() {
            // Save settings to the settings file (if the file doesn't exist it will still create a new one)
            File.WriteAllText(SettingsFilename, $"{MasterVolume}\n" +
                                                $"{MusicVolume}\n" +
                                                $"{SoundEffectVolume}\n" +
                                                (DebugMode ? "true" : "false") + "\n" +
                                                (VSync ? "true" : "false") + "\n" +
                                                (ShowFps ? "true" : "false") + "\n" +
                                                (AllowWindowResizing ? "true" : "false"));
        }
        public static void OnScreenResize(object sender, EventArgs e) {
            // If the user resizes the window, update the ScreenSize variable with the new screen size.
            ScreenSize.X = GameRoot.Instance.Window.ClientBounds.Width;
            ScreenSize.Y = GameRoot.Instance.Window.ClientBounds.Height;
        }
    }
}
