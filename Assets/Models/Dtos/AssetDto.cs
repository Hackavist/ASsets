using System;
using Assets.Models.DataModels;
using Assets.Models.Enums;

namespace Assets.Models.Dtos
{
    public class AssetDto
    {
        public string AssetId { get; }
        public string AssetNumber { get; }
        public string AssetName { get; }
        public string PMVCode { get; }
        public string CurrentLocation { get; }
        public DateTime DateOfPurchase { get; }
        public string PoNumber { get; }
        public string PlateSerialNumber { get; }
        public double PurchaseCostOfAsset { get; }
        public double NetBookValue { get; }
        public double MonthlyDepreciationDueDate { get; }
        public int MonthsToDepreciation { get; }
        public Status AssetStatus { get; }
        public string ToolType { get; }
        public string CalibrationCertificationNumber { get; }
        public DateTime CalibrationCertificationDate { get; }

        public AssetDto(Asset asset)
        {
            AssetId = asset.AssetId;
            AssetNumber = asset.AssetNumber;
            AssetName = asset.AssetName;
            PMVCode = asset.PMVCode;
            CurrentLocation = asset.CurrentLocation;
            DateOfPurchase = asset.DateOfPurchase;
            PoNumber = asset.PoNumber;
            PlateSerialNumber = asset.PlateSerialNumber;
            PurchaseCostOfAsset = asset.PurchaseCostOfAsset;
            MonthsToDepreciation = asset.MonthsToDepreciation;
            AssetStatus = asset.AssetStatus;
            ToolType = asset.ToolType;
            CalibrationCertificationNumber = asset.CalibrationCertificationNumber;
            CalibrationCertificationDate = asset.CalibrationCertificationDate;
            NetBookValue = PurchaseCostOfAsset - PurchaseCostOfAsset / MonthsToDepreciation *
                (DateTime.Today.Month - DateOfPurchase.Month + 1);
            if (NetBookValue > 0.0)
                MonthlyDepreciationDueDate = PurchaseCostOfAsset / MonthsToDepreciation;
            else
                MonthlyDepreciationDueDate = 1;
        }
    }
}