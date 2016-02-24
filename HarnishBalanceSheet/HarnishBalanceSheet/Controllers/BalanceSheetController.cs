using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using HarnishBalanceSheet.DataAccess;
using HarnishBalanceSheet.Models;
using Newtonsoft.Json;

namespace HarnishBalanceSheet.Controllers
{
    public class BalanceSheetController : Controller
    {
        /// <summary>
        /// Data repository.
        /// </summary>
        private IRepository repository;

        /// <summary>
        /// Grams to ounces conversion factor. 
        /// </summary>
        private const decimal convFactor = 31.1034768m;

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="repository"> Data repository. </param>
        public BalanceSheetController(IRepository rep)
        {
            this.repository = rep;
        }

        /// <summary>
        /// Gets a balance sheet for the given date.
        /// </summary>
        /// <returns> Balance sheet view. </returns>
        public ActionResult Details(DateTime? date)
        {
            Trace.TraceInformation(string.Format("Entering BalanceSheetController.Details with date = {0}.", date));

            try {
                if (date == null) date = DateTime.Now;

                Snapshot snapshot = this.repository.GetBalanceSheet((DateTime)date);
                BusinessRules.BusinessRules br = new BusinessRules.BusinessRules() { Snapshot = snapshot };
                br.CalculateBusinessRules();

                Trace.TraceInformation("Exiting BalanceSheetController.Details.");
                return View(snapshot);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets date/value pairs of total liabilities between two dates. 
        /// </summary>
        /// <param name="start"> Start date. </param>
        /// <param name="end"> End date. </param>
        /// <returns> Json result of date/value pairs.</returns>
        [HttpGet]
        public ActionResult Liabilities(DateTime? start, DateTime? end)
        {
            Trace.TraceInformation(string.Format("Entering BalanceSheetController.Liabilities with start = {0} and end = {1}.", start, end));

            try
            {
                if (start == null) start = DateTime.Now - new TimeSpan(365, 0, 0, 0);
            if (end == null) end = DateTime.Now;

            Snapshot[] snapshots = this.repository.GetHistoryBalanceSheets((DateTime)start, (DateTime)end);
                ChartData cd = new ChartData();
                cd.labels = new string[snapshots.Length];
                cd.data = new decimal[snapshots.Length];
                int i = 0;

            foreach (var snapshot in snapshots)
            {
                BusinessRules.BusinessRules br = new BusinessRules.BusinessRules() { Snapshot = snapshot };
                    cd.labels[i] = snapshot.Date.ToString("MMMM d yyyy");
                    cd.data[i] = br.CalculateTotalLiabilities();
                    i++;

            }

            Trace.TraceInformation("Exiting BalanceSheetController.Liabilities.");
            return Json(JsonConvert.SerializeObject(cd), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets date/value pairs of net worth.
        /// </summary>
        /// <param name="start"> Start date. </param>
        /// <param name="end"> End date. </param>
        /// <returns> Date/Net worth pairs as Json. </returns>
        [HttpGet]
        public ActionResult NetWorth(DateTime? start, DateTime? end)
        {
            Trace.TraceInformation(string.Format("Entering BalanceSheetController.NetWorth with start = {0} and end = {1}.", start, end));

            try { 
            if (start == null) start = DateTime.Now - new TimeSpan(365, 0, 0, 0);
            if (end == null) end = DateTime.Now;

            Snapshot[] snapshots = this.repository.GetHistoryBalanceSheets((DateTime)start, (DateTime)end);
                ChartData cd = new ChartData();
                cd.labels = new string[snapshots.Length];
                cd.data = new decimal[snapshots.Length];
                int i = 0;

                foreach (var snapshot in snapshots)
            {
                BusinessRules.BusinessRules br = new BusinessRules.BusinessRules() { Snapshot = snapshot };
                cd.labels[i] = snapshot.Date.ToString("MMMM d yyyy");
                    cd.data[i] = br.CalculateNetWorth();
                    i++;
                }

            Trace.TraceInformation("Exiting BalanceSheetController.NetWorth.");
            return Json(JsonConvert.SerializeObject(cd), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets the Edit view. 
        /// </summary>
        /// <returns> Edit view. </returns>
        [HttpGet]
        public ActionResult Edit()
        {
            Trace.TraceInformation("Entering BalanceSheetController.Edit.");

            try { 
            Snapshot snapshot = this.repository.GetBalanceSheet(DateTime.Now);
                BusinessRules.BusinessRules br = new BusinessRules.BusinessRules() { Snapshot = snapshot };
                br.CalculateBusinessRules();

                Trace.TraceInformation("Exiting BalanceSheetController.Edit.");
            return View(snapshot);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Saves a snapshot to the repository. 
        /// </summary>
        /// <param name="snapshot"> Snapshot to save. </param>
        /// <returns> Redirects to Details view.</returns>
        [HttpPost]
        public ActionResult Edit(Snapshot snapshot)
        {
            Trace.TraceInformation("Entering BalanceSheetController.Edit.");

            try {
                var form = Request.Form;
                string[] assetNames = form["asset.Name"].Split(',');
                decimal[] assetValues = form["asset.FormattedValue"].Split(',').Select(x => decimal.Parse(x)).ToArray();
                string[] types = form["asset.Type"].Split(',');
                Object[] fractions = GetDeserializedArray<Fract[]>(form["asset.FormattedFractions"]);
                Object[] elements = GetDeserializedArray<Elem[]>(form["asset.FormattedElements"]);
                snapshot.Assets = new Asset[assetNames.Length];

                for (int i = 0; i < snapshot.Assets.Length; i++)
                {
                    snapshot.Assets[i] = new Asset();
                    snapshot.Assets[i].Name = assetNames[i];
                    snapshot.Assets[i].Value = assetValues[i];
                    snapshot.Assets[i].Type = types[i];
                    snapshot.Assets[i].Fractions = (Fract[])fractions[i];
                    snapshot.Assets[i].Elements = (Elem[])elements[i];
                }

                string[] liabilityNames = form["liability.Name"].Split(',');
                decimal[] liabilityValues = form["liability.Value"].Split(',').Select(x => decimal.Parse(x)).ToArray();
                snapshot.Liabilities = new Liability[liabilityNames.Length];

                for (int i = 0; i < snapshot.Liabilities.Length; i++)
                {
                    snapshot.Liabilities[i] = new Liability();
                    snapshot.Liabilities[i].Name = liabilityNames[i];
                    snapshot.Liabilities[i].Value = liabilityValues[i];
                }
               
                GetPreciousMetalsPrices(snapshot);
            this.repository.SaveBalanceSheet(snapshot);

            Trace.TraceInformation("Exiting BalanceSheetController.Edit.");
            return RedirectToAction("Details");
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets an array of deserialized arrays.
        /// </summary>
        /// <param name="v"> String to deserialize. </param>
        /// <param name="type"> Type to deserialize to. </param>
        /// <returns> Array of arrays of given type. </returns>
        private object[] GetDeserializedArray<T>(string v)
        {
            Trace.TraceInformation("Entering BalanceSheetController.GetDeserializedArray.");

            int index = 0;
            List<object> list = new List<object>();

            while (index < v.Length)
            {
                int nextIndex = v.IndexOfAny(new char[] { ',', '[' }, index);
                if (nextIndex == -1) nextIndex = v.Length;
                char myChar = ' ';
                
                if (nextIndex < v.Length) myChar = v[nextIndex];
                string str = null;

                switch (myChar)
                {
                    case ',':
                        str = v.Substring(index, nextIndex - index);
                        break;
                    case ' ':
                        goto case ',';
                    case '[':
                        int endIndex = v.IndexOf(']', nextIndex);
                        str = v.Substring(nextIndex, endIndex - nextIndex + 1);
                        nextIndex = endIndex + 1;
                        break;
                }

                Object obj = JsonConvert.DeserializeObject<T>(str);
                list.Add(obj);               

                index = nextIndex + 1;
            }

            Trace.TraceInformation("Exiting BalanceSheetController.GetDeserializedArray.");
            return list.ToArray();
        }

        /// <summary>
        /// Gets current precious metals prices. 
        /// </summary>
        /// <param name="snapshot"> Snapshot entity. </param>
        private void GetPreciousMetalsPrices(Snapshot snapshot)
        {
            Trace.TraceInformation("Entering BalanceSheetController.GetPreciousMetalsPrices.");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["metalsUrl"]);
            WebResponse response = request.GetResponse();
            XmlDocument document = new XmlDocument();
            document.Load(response.GetResponseStream());
            
            XmlNodeList list = document.SelectNodes("/prices/currency[@access='usd']");
            XmlNode node = list.Item(0);

            foreach (XmlNode child in node.ChildNodes)
            {
                string metal = child.Attributes[0].Value.ToUpper();

                Elem element = snapshot
                    .Assets
                    .Where(x => x.Name == "Coins")
                    .First()
                    .Elements.Where(x => x.Element.ToUpper() == metal)
                    .First();

                element.Price = decimal.Parse(child.InnerText) * convFactor;
            }

            Trace.TraceInformation("Exiting BalanceSheetController.GetPreciousMetalsPrices.");
        }
    }

    public class ChartData
    {
        public string[] labels { get; set; }
        public decimal[] data { get; set; }
    }
}