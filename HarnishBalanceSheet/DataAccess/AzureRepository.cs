using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HarnishBalanceSheet.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;


namespace HarnishBalanceSheet.DataAccess
{
    public class AzureRepository : IRepository
    {
        /// <summary>
        /// Gets a balance sheet from Azure storage. 
        /// </summary>
        /// <param name="date"> Date for which to get the balance sheet. </param>
        /// <returns> A snapshot. </returns>
        public Snapshot GetBalanceSheet(DateTime date)
        {
            Trace.TraceInformation("Entering AzureRepository.GetCurrentBalanceSheet.");

            CloudBlobContainer container = GetCloudBlobContainer();
            DateTime result;

            CloudBlockBlob blob = (CloudBlockBlob)container
                .ListBlobs()
                .Where(x => DateTime.TryParse(((CloudBlockBlob)x).Name.Split('.')[0], out result))
                .Where(x => DateTime.Parse(((CloudBlockBlob)x).Name.Split('.')[0]) <= date)
                .OrderByDescending(x => ((CloudBlockBlob)x).Properties.LastModified)
                .First();

            string text = blob.DownloadText();
            Snapshot snapshot = JsonConvert.DeserializeObject<Snapshot>(text);
            snapshot.Date = DateTime.Parse(blob.Name.Split('.')[0]);

            CloudBlockBlob targets = container.GetBlockBlobReference(System.Configuration.ConfigurationManager.AppSettings["targets"]);
            text = targets.DownloadText();
            snapshot.Targets = JsonConvert.DeserializeObject<Targ[]>(text);

            Trace.TraceInformation("Exiting AzureRepository.GetCurrentBalanceSheet.");
            return snapshot;
        }        

        /// <summary>
        /// Gets the balance sheets between 2 given dates. 
        /// </summary>
        /// <param name="start"> Start date. </param>
        /// <param name="end"> End date. </param>
        /// <returns> Array of snapshots. </returns>
        public Snapshot[] GetHistoryBalanceSheets(DateTime start, DateTime end)
        {
            Trace.TraceInformation(string.Format("Entering AzureRepository.GetHistoryBalanceSheets with start = {0} and end = {1}.", start.ToString(), end.ToString()));

            CloudBlobContainer container = GetCloudBlobContainer();
            DateTime result;

            var blobs = container
                .ListBlobs()
                .Where(x => DateTime.TryParse(((CloudBlockBlob)x).Name.Split('.')[0], out result))
                .Where(x => DateTime.Parse(((CloudBlockBlob)x).Name.Split('.')[0]) >= start && DateTime.Parse(((CloudBlockBlob)x).Name.Split('.')[0]) <= end);

            int count = blobs.Count();
            Snapshot[] snapshots = new Snapshot[count];

            for (int i = 0; i < count; i++)
            {
                CloudBlockBlob blob = (CloudBlockBlob)blobs.ElementAt(i);
                string text = blob.DownloadText();
                snapshots[i] = JsonConvert.DeserializeObject<Snapshot>(text);
                snapshots[i].Date = DateTime.Parse(blob.Name.Split('.')[0]);
            }  

            Trace.TraceInformation("Exiting AzureRepository.GetHistoryBalanceSheets.");
            return snapshots;
        }

        /// <summary>
        /// Saves a balance sheet as Azure blob. 
        /// </summary>
        /// <param name="snapshot"> Snapshot entity. </param>
        /// <returns> Name of saved blob. </returns>
        public string SaveBalanceSheet(Snapshot snapshot)
        {
            Trace.TraceInformation("Entering AzureRepository.SaveBalanceSheet.");

            CloudBlobContainer container = GetCloudBlobContainer();
            string bName = string.Format("{0}.json", DateTime.Now.ToString("yyyy-MM-dd"));
            string text = JsonConvert.SerializeObject(snapshot);
            CloudBlockBlob blob = container.GetBlockBlobReference(bName);
            blob.UploadText(text);

            Trace.TraceInformation("Exiting AzureRepository.SaveBalanceSheet.");
            return bName;
        }

        /// <summary>
        /// Gets the cloud blob container. 
        /// </summary>
        /// <returns> Cloud blob container object. </returns>
        private CloudBlobContainer GetCloudBlobContainer()
        {
            Trace.TraceInformation("Entering AzureRepository.GetCloudBlobContainer.");

            CloudStorageAccount account = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["AzureStorageConnection"]);
            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(System.Configuration.ConfigurationManager.AppSettings["container"]);

            Trace.TraceInformation("Exiting AzureRepository.GetCloudBlobContainer.");
            return container;
        }
    }
}
