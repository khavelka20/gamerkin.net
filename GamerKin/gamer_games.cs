//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GamerKin
{
    using System;
    using System.Collections.Generic;
    
    public partial class gamer_games
    {
        public long id { get; set; }
        public int gamer_id { get; set; }
        public int game_id { get; set; }
        public Nullable<int> rating { get; set; }
        public System.DateTime created_at { get; set; }
        public System.DateTime updated_at { get; set; }
        public Nullable<int> time_played { get; set; }
        public Nullable<int> include { get; set; }
    
        public virtual game game { get; set; }
    }
}
