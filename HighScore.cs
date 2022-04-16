using System.IO;

namespace Mono_Ether {
    public static class HighScore {
        private const string HighScoreFilename = "highscore.txt";

        public static int Score {
            get => LoadHighScore();
            set => SaveHighScore(Score);
            }
        private static int LoadHighScore() {
            // Return saved score if it exists, or return 0 if there is none
            return File.Exists(HighScoreFilename) && int.TryParse(File.ReadAllText(HighScoreFilename), out var score) ? score : 0;
        }
        private static void SaveHighScore(int score) {
            // Saves the score to the highscore file, note that this does not check the saved score is greater than the new score.
            File.WriteAllText(HighScoreFilename, score.ToString());
        }
    }
}
