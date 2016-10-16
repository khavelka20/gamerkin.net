using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamerKin.ViewModels
{
    public class SideBarViewModel : BaseViewModel
    {
        public List<Genre> Genres { get; set; }
        public GameSearch Search { get; set; }

    }
}
