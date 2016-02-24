using System;
using HarnishBalanceSheet.Models;

namespace HarnishBalanceSheet.DataAccess
{
    public interface IRepository
    {
        /// <summary>
        /// Gets a balance sheet.
        /// </summary>
        /// <param name="date"> Date to get the balance sheet. </param>
        /// <returns> Snapshot closest to given date. </returns>
        Snapshot GetBalanceSheet(DateTime date);

        /// <summary>
        /// Gets past balance sheets. 
        /// </summary>
        /// <param name="start"> Start date. </param>
        /// <param name="end"> End date. </param>
        /// <returns> Array of snapshots. </returns>
        Snapshot[] GetHistoryBalanceSheets(DateTime start, DateTime end);

        /// <summary>
        /// Saves a balance sheet. 
        /// </summary>
        /// <param name="snapshot"> Snapshot to save. </param>
        /// <returns> File name of saved snapshot. </returns>
        string SaveBalanceSheet(Snapshot snapshot);

    }
}
