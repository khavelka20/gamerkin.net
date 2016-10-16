using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamerKin.ViewModels
{
    public class HomeViewModel
    {
        public Gamer gamer { get; set; }
        public List<GamerGame> gamer_games { get; set; }
    }
}