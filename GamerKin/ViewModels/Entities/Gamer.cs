using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamerKin.ViewModels
{
    public class Gamer
    {
        public int id { get; set; }
        public long steam_id { get; set; }
        public string avatar { get; set; }
        public string avatar_medium { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string name { get; set; }
    }
}
