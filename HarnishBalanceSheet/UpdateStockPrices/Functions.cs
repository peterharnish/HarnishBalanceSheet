using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Azure.WebJobs;
using SendGrid;
using Stocks.BusinessRules;
using Stocks.Entity;

namespace UpdateStockPrices
{
    public class Functions
    {
        /// <summary>
        /// Updates the database with current stock prices. If any prices are at or below target sale price, sends email notification to sell. 
        /// </summary>
        [NoAutomaticTrigger]
        public static void UpdateStockPricesAndSendEmail()
        {
            Trace.TraceInformation("Entering Functions.UpdateStockPricesAndSendEmail.");
            try
            {
                string[] symbols = GetCurrentStockPrices();

                if (symbols.Length > 0)
                {
                    SendSellAlertEmail(symbols);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            Trace.TraceInformation("Exiting Functions.UpdateStockPricesAndSendEmail.");
        }

        /// <summary>
        /// Sends an alert email to sell the given positions.
        /// </summary>
        /// <param name="symbols"> Array of stock ticker symbols. </param>
        private static void SendSellAlertEmail(string[] symbols)
        {
            Trace.TraceInformation("Entering Functions.SendSellAlertEmail.");

            var myMessage = new SendGridMessage();
            myMessage.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["emailFrom"]);

            List<String> recipients = new List<String>(System.Configuration.ConfigurationManager.AppSettings["recipients"].Split(','));

            myMessage.AddTo(recipients);
            myMessage.Subject = "Sell stock";

            foreach (var symbol in symbols)
            {
                myMessage.Text = string.Concat(myMessage.Text, "Sell ", symbol, "\r\n");
            }

            var userName = System.Configuration.ConfigurationManager.AppSettings["sendGridUserName"];
            var password = System.Configuration.ConfigurationManager.AppSettings["sendGridPassword"];
            var credentials = new NetworkCredential(userName, password);
            var transportWeb = new Web(credentials);

            transportWeb.DeliverAsync(myMessage);

            Trace.TraceInformation("Exiting Functions.SendSellAlertEmail.");
        }

        /// <summary>
        /// Gets the current stock prices from an XML feed. 
        /// </summary>
        /// <returns> Array of stock ticker symbols where the current price is less than the target sale price. </returns>
        private static string[] GetCurrentStockPrices()
        {
            Trace.TraceInformation("Entering Functions.GetCurrentStockPrices.");

            string[] belowTarget;
            
            Type t = Type.GetType(System.Configuration.ConfigurationManager.AppSettings["Repository"]);
            Assembly a = Assembly.GetAssembly(t);
            Stocks.DataAccess.IRepository repository = (Stocks.DataAccess.IRepository)a.CreateInstance(t.FullName);
            BR br = new BR(repository);
            List<Position> positions = br.GetCurrent().Where(x => x.ID > 0).ToList<Position>();

            foreach (var position in positions)
            {
                try
                {
                    decimal price = FetchCurrentPrice(position.Symbol);

                    if (price > 0)
                    {
                        position.CurrentPrice = price;
                    }

                    br.UpdateStockPrice(position);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }

            belowTarget = positions.Where(x => x.CurrentPrice <= x.TargetSalePrice).Select(x => x.Symbol).ToArray();

            Trace.TraceInformation("Exiting Functions.GetCurrentStockPrices.");
            return belowTarget;
        }

        /// <summary>
        /// Makes a web request to fetch the current stock price. 
        /// </summary>
        /// <param name="symbol"> Stock ticker symbol. </param>
        /// <returns> Stock price as decimal. </returns>
        public static decimal FetchCurrentPrice(string symbol)
        {
            Trace.TraceInformation(string.Format("Entering Functions.FetchCurrentPrice with symbol = {0}.", symbol));

            string url = string.Format(System.Configuration.ConfigurationManager.AppSettings["url"], symbol);
            decimal price = 0;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            WebResponse response = request.GetResponse();
            XmlDocument document = new XmlDocument();
            document.Load(response.GetResponseStream());
            XmlNodeList list = document.GetElementsByTagName("price");

            if (list.Count > 0)
            {
                price = decimal.Parse(list[0].InnerText);
            }

            Trace.TraceInformation(string.Format("Exiting Functions.FetchCurrentPrice with price = {0}.", price.ToString()));
            return price;
        }
    }
}
