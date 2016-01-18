﻿using System;
using Windows.ApplicationModel.Background;
using MoneyManager.Core.Manager;
using MoneyManager.Core.Repositories;
using MoneyManager.DataAccess;
using MoneyManager.Foundation;
using MoneyManager.Windows.Concrete.Shortcut;
using MvvmCross.Plugins.Sqlite.WindowsUWP;
using Xamarin;

namespace MoneyManager.Tasks.Windows
{
    public sealed class ClearTransactionBackgroundTask : IBackgroundTask
    {
        private readonly PaymentManager paymentManager;

        public ClearTransactionBackgroundTask()
        {
            var insightKey = "599ff6bfdc79368ff3d5f5629a57c995fe93352e";

#if DEBUG
            insightKey = Insights.DebugModeKey;
#endif
            if (!Insights.IsInitialized)
            {
                Insights.Initialize(insightKey);
            }

            var sqliteConnectionCreator = new SqliteConnectionCreator(new WindowsSqliteConnectionFactory());

            paymentManager = new PaymentManager(
                new PaymentRepository(new TransactionDataAccess(sqliteConnectionCreator),
                    new RecurringTransactionDataAccess(sqliteConnectionCreator)),
                new AccountRepository(new AccountDataAccess(sqliteConnectionCreator)),
                null);
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                paymentManager.ClearPayments();
                Tile.UpdateMainTile();
            } 
            catch (Exception ex)
            {
                Insights.Report(ex);
            }
        }
    }
}