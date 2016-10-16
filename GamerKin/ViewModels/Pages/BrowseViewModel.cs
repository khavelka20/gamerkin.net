using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamerKin.ViewModels
{
    public class BrowseViewModel
    {
        public List<Game> Games { get; set; }
        public Genre SelectedGenre { get; set; }
    }
}
