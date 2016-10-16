using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamerKin.ViewModels
{
    public class GameSearch : BaseDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Min search length is 3.")]
        public string Input { get; set; }

        public List<GameOverview> Results { get; set; }
    }
}
