using System;

namespace Assets.Models.DataModels
{
    public class Repositions : BaseModel
    {
        public string OldPosition { get; set; }
        public string NewPosition { get; set; }
        public DateTime RepositionDate { get; set; }
        public int AssetId { get; set; }
        public virtual Asset Asset { get; set; }
    }
}