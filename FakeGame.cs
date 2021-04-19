using Playnite.SDK.Models;

namespace HtmlExporterPlugin
{
    public class FakeGame
    {
        public string Genre { get; set; }
        public string Developer { get; set; }
        public string Feature { get; set; }
        public string Publisher { get; set; }
        public string Serie { get; set; }
        public string Category { get; set; }
        public string Library { get; set; }
        public Game OriginalGame { get; set; }

        public FakeGame(Game aGame)
        {
            OriginalGame = aGame;
        }

    }
}
