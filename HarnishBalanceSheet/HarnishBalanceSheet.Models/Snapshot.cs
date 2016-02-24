using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace HarnishBalanceSheet.Models
{
    public class Snapshot
    {
        public DateTime Date;
        public string DateFormatted { get { return Date.ToShortDateString(); } }
        public Asset[] Assets;
        public Liability[] Liabilities;
        public decimal TotalAssets;
        public decimal TotalLiabilities;
        public decimal NetWorth;
        public Group[] Groups;
        public Targ[] Targets;
    }

    public class Asset
    {
        public string Name;
        public decimal Value;
        public string FormattedValue { get { return Value.ToString("F2"); } }
        public string Type;
        public Fract[] Fractions;
        public string FormattedFractions { get { return JsonConvert.SerializeObject(Fractions); } }
        public Elem[] Elements;
        public string FormattedElements { get { return JsonConvert.SerializeObject(Elements); } }
    }

    public class Liability
    {
        public string Name;
        public decimal Value;
    }

    public class Fract
    {
        public string Type;
        public decimal Fraction;
        public decimal Value;
    }    

    public class Elem
    {
        public string Element;
        public int Ounces;
        public decimal Price;
        public decimal TotalValue;
    }

    public class Group
    {
        public string Type;
        public NameValuePair[] Pairs;
        public decimal Total;
    }

    public class NameValuePair
    {
        public string Name;
        public decimal Value;
    }

    public class Targ
    {
        public string Asset;
        public decimal Target;
        public decimal Actual;
        public decimal Difference;
    }
}