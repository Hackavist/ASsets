using System;
using Assets.Models.DataModels;

namespace Assets.Models.Dtos
{
    public class RepairDto
    {
        public DateTime RepairDate { get; set; }
        public double LaborAmount { get; set; }
        public double SparePartsAmount { get; set; }
        public double TotalAmount => SparePartsAmount + LaborAmount;
        public string Location { get; set; }

        public RepairDto(Repair repair)
        {
            RepairDate = repair.RepairDate;
            LaborAmount = repair.LaborAmount;
            SparePartsAmount = repair.SparePartsAmount;
            Location = repair.Location;
        }
    }

}
