using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamerKin.ViewModels
{
    public class GameVideo
    {
        public int id { get; set; }
        public int game_id { get; set; }
        public string youtube_id { get; set; }
        public int clip_duration { get; set; }
    }
}
