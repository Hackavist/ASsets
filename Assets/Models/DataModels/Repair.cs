using System;

namespace Assets.Models.DataModels
{
    public class Repair : BaseModel
    {
        public DateTime RepairDate { get; set; }
        public double LaborAmount { get; set; }
        public double SparePartsAmount { get; set; }
        public string Location { get; set; }
        public int AssetId { get; set; }
        public virtual Asset Asset { get; set; }
    }
}