using Playnite.SDK.Models;
using System;

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
        public string Platform { get; set; }        
        public string AgeRating { get; set; }
        public string Region { get; set; }
        public string Tag { get; set; }
        public Game OriginalGame { get; set; }
        public FakeGame(Game aGame)
        {
            OriginalGame = aGame;
        }

    }
}
