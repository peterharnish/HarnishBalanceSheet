using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using HarnishBalanceSheet.Models;

namespace HarnishBalanceSheet.BusinessRules
{
    public class BusinessRules
    {
        private const string Coins = "Coins";

        private readonly string[] AssetTypes = { "Cash", "Precious Metals", "Real Estate", "Bonds", "Stocks" };

        public Snapshot Snapshot { get; set; }

        /// <summary>
        /// Performs business rules calculations. 
        /// </summary>
        /// <param name="snapshot"> Snapshot entity. </param>
        public void CalculateBusinessRules()
        {
            Trace.TraceInformation("Entering BusinessRules.CalculateBusinessRules.");

            this.Snapshot.NetWorth = CalculateNetWorth();
            CalculateAssetGroupTotals();
            CalculateTargetData();

            Trace.TraceInformation("Exiting BusinessRules.CalculateBusinessRules.");
        }

        /// <summary>
        /// Calculates the total liabilities. 
        /// </summary>
        /// <returns> Total liabilities. </returns>
        public decimal CalculateTotalLiabilities()
        {
            return decimal.Round(this.Snapshot.Liabilities.Select(x => x.Value).Sum(), 2);
        }

        /// <summary>
        /// Calculates the net worth of this snapshot. 
        /// </summary>
        /// <returns> Net worth. </returns>
        public decimal CalculateNetWorth()
        {
            Trace.TraceInformation("Entering BusinessRules.CalculateNetWorth.");

            CalculateCoinTotalValue();
            this.Snapshot.TotalAssets = this.Snapshot.Assets.Select(x => x.Value).Sum();
            this.Snapshot.TotalLiabilities = CalculateTotalLiabilities();
            decimal netWorth = this.Snapshot.TotalAssets - this.Snapshot.TotalLiabilities;

            Trace.TraceInformation(string.Format("Exiting BusinessRules.CalculateNetWorth with netWorth = {0}.", netWorth.ToString()));
            return decimal.Round(netWorth, 2);
        }    

        /// <summary>
        /// Calculates actual and difference from targets. 
        /// </summary>
        private void CalculateTargetData()
        {
            Trace.TraceInformation("Entering BusinessRules.CalculateTargetData.");

            foreach (var target in this.Snapshot.Targets)
            {
                target.Actual = this.Snapshot.Groups.Where(x => x.Type == target.Asset).First().Total
                    / this.Snapshot.TotalAssets;

                target.Difference = (target.Actual - target.Target) / target.Target;
            }

            Trace.TraceInformation("Exiting BusinessRules.CalculateTargetData.");
        }

        /// <summary>
        /// Calculates the sum of all the assets in each type. 
        /// </summary>
        private void CalculateAssetGroupTotals()
        {
            Trace.TraceInformation("Entering BusinessRules.CalculateAssetGroupTotals.");

            this.Snapshot.Groups = new Group[AssetTypes.Length];

            for (int i = 0; i < AssetTypes.Length; i++)
            {
                Group group = new Group() { Type = AssetTypes[i] };

                List<NameValuePair> pairs = new List<NameValuePair>();
                List<Asset> assets = this.Snapshot.Assets.Where(x => x.Type == AssetTypes[i]).ToList();

                foreach (var asset in assets)
                {
                    pairs.Add(new NameValuePair() { Name = asset.Name, Value = asset.Value });
                }

                foreach (var asset in Snapshot.Assets)
                {
                    if (asset.Fractions != null) { 
                    foreach (var fraction in asset.Fractions)
                    {
                        if (fraction.Type == AssetTypes[i])
                        {
                            if (fraction.Fraction > 0) fraction.Value = asset.Value * fraction.Fraction;
                            pairs.Add(new NameValuePair() { Name = asset.Name, Value = fraction.Value });
                        }
                    }
                    }
                }

                group.Pairs = pairs.ToArray();
                group.Total = group.Pairs.Select(x => x.Value).Sum();
                this.Snapshot.Groups[i] = group;
            }

            Trace.TraceInformation("Exiting BusinessRules.CalculateAssetGroupTotals.");
        }

        /// <summary>
        /// Calculates total value of coins. 
        /// </summary>
        private void CalculateCoinTotalValue()
        {
            Trace.TraceInformation("Entering BusinessRules.CalculateCoinTotalValue.");

            Asset coins = this.Snapshot.Assets.Where(x => x.Name == Coins).First();

            foreach (var element in coins.Elements)
            {
                element.TotalValue = element.Ounces * element.Price;
            }

            coins.Value = coins.Elements.Select(x => x.TotalValue).Sum();

            Trace.TraceInformation("Exiting BusinessRules.CalculateCoinTotalValue.");
        }
    }
}