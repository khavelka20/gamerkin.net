using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamerKin.Models.Steam
{
    public class PlayerSummary
    {
        public long steamid { get; set; }
        public int communityvisibilitystate { get; set; }
        public int profilestate { get; set; }
        public string personaname { get; set; }
        public long lastlogoff { get; set; }
        public string profileurl { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public int personastate { get; set; }
        public string realname { get; set; }
        public long primaryclanid { get; set; }
        public long timecreated { get; set; }
        public int personastateflags { get; set; }
        public string loccountrycode { get; set; }
    }
}
