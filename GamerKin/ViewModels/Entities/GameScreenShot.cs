using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamerKin.ViewModels
{
    public class GameScreenShot
    {
        public int id { get; set; }
        public int game_id { get; set; }
        public string thumb_url { get; set; }
        public string full_url { get; set; }
        public bool active { get; set; } = false;
    }
}
