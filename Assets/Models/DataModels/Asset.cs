using System;
using System.Collections.Generic;
using Assets.Models.Enums;

namespace Assets.Models.DataModels
{
    public class Asset : BaseModel
    {
        public string AssetId { get; set; }
        public string AssetNumber { get; set; }
        public string AssetName { get; set; }
        public string PMVCode { get; set; }
        public string CurrentLocation { get; set; }
        public DateTime DateOfPurchase { get; set; }
        public string PoNumber { get; set; }
        public string PlateSerialNumber { get; set; }
        public double PurchaseCostOfAsset { get; set; }
        public int MonthsToDepreciation { get; set; }
        public Status AssetStatus { get; set; }
        public string ToolType { get; set; }
        public string CalibrationCertificationNumber { get; set; }
        public DateTime CalibrationCertificationDate { get; set; }
        public string CalibrationCertificationPictureBase64 { get; set; }
        public string CalibrationCertificationPictureFormat{ get; set; }
        public string AssetPictureBase64 { get; set; }
        public string AssetPictureFormat { get; set; }
        public virtual ICollection<Repair> Repairs { get; set; }
        public virtual ICollection<Repositions> Repositions { get; set; }
    }
}