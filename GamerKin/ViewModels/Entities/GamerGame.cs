using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamerKin.ViewModels
{
    public class GamerGame
    {
        public long id { get; set; }
        public int gamer_id { get; set; }
        public int game_id { get; set; }
        public int rating { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int time_played { get; set; }
        public int include { get; set; }
        public Game game { get; set; }
    }
}
