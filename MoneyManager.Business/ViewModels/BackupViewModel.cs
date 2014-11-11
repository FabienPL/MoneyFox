﻿#region

using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using GalaSoft.MvvmLight;
using Microsoft.Live;
using MoneyManager.Business.Helper;
using MoneyManager.Business.Logic;
using MoneyManager.Foundation;

#endregion

namespace MoneyManager.Business.ViewModels
{
    public class BackupViewModel : ViewModelBase
    {
        private const string BackupFolderName = "MoneyFoxBackup";
        private const string DbName = "moneyfox.sqlite";

        private LiveConnectClient LiveClient { get; set; }

        public async void LogInToOneDrive()
        {
            LiveClient = await BackupLogic.LogInToOneDrive();

            if (LiveClient == null)
            {
                await ShowNotLoggedInMessage();
            }
        }

        private static async Task ShowNotLoggedInMessage()
        {
            var dialog = new MessageDialog(Translation.GetTranslation("NotLoggedInMessage"),
                Translation.GetTranslation("NotLoggedIn"));
            dialog.Commands.Add(new UICommand(Translation.GetTranslation("OkLabel")));

            await dialog.ShowAsync();
        }
        
        public async void CreateBackup()
        {
            var folderId = await BackupLogic.GetFolderId(LiveClient, BackupFolderName);

            if (String.IsNullOrEmpty(folderId))
            {
                folderId = await BackupLogic.CreateBackupFolder(LiveClient, BackupFolderName);
            }

            await ShowOverwriteInfo();

            await BackupLogic.UploadBackup(LiveClient, folderId, DbName);
        }

        private async Task<bool> ShowOverwriteInfo()
        {
            var dialog = new MessageDialog(Translation.GetTranslation("OverwriteBackupMessage"),
                Translation.GetTranslation("OverwriteBackup"));
            dialog.Commands.Add(new UICommand(Translation.GetTranslation("YesLabel")));
            dialog.Commands.Add(new UICommand(Translation.GetTranslation("NoLabel")));

            var result = await dialog.ShowAsync();

            return result.Label == Translation.GetTranslation("YesLabel");
        }
    }
}
