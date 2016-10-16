using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamerKin.ViewModels
{
    public class Game
    {
        public int id { get; set; }
        public long steam_id { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string steam_user_rating { get; set; }
        public string metacritic_rating { get; set; }
        public string description { get; set; }
        public int steam_user_rating_value { get; set; }
        public string developer { get; set; }
        public double predicted_score { get; set; }
        public bool show_header { get; set; } = true;
        public bool show_preview { get; set; } = false;
        public List<GameScreenShot> screen_shots { get; set; }
        public List<GameVideo> videos { get; set; }
        public List<Genre> genres { get; set; }
        public List<GamePlatform> platforms { get; set; }
        public List<GamePrices> prices { get; set; }
    }
}
