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
    
    public partial class game_assets
    {
        public int id { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public int game_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string youtube_id { get; set; }
        public Nullable<int> clip_duration { get; set; }
        public string thumb_url { get; set; }
        public string full_url { get; set; }
    
        public virtual game game { get; set; }
    }
}