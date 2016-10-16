using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamerKin.ViewModels
{
    public class GamePrices
    {
        public long id { get; set; }
        public int game_id { get; set; }
        public string source { get; set; }
        public DateTime updated_at { get; set; }
        public int price { get; set; }
    }
}
