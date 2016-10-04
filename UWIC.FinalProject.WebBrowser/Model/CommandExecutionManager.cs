using System;
using System.Collections.Generic;
using System.Linq;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.WebBrowser.ViewModel;

namespace UWIC.FinalProject.WebBrowser.Model
{
    public class CommandExecutionManager
    {
        private static TabItemViewModel _tabItemViewModel;

        public static void SetTabItemViewModel(TabItemViewModel tabItemViewModel)
        {
            _tabItemViewModel = tabItemViewModel;
        }

        private static void ExecuteGoCommand(string identifiedWebSite)
        {
            if (!identifiedWebSite.Contains(".com"))
                identifiedWebSite += ".com";
            var websiteName = "http://www." + identifiedWebSite;
            Uri url;
            if (Uri.TryCreate(websiteName, UriKind.RelativeOrAbsolute, out url))
            {
                //_bcViewModel.NavigateToURL(url);
            }
        }

        private static void ExecuteBackCommand()
        {
            //_bcViewModel.MoveBackward();
        }

        private static void ExecuteForwardCommand()
        {
            //_bcViewModel.MoveForward();
        }

        private static void ExecuteRefershCommand()
        {
            //_bcViewModel.RefreshBrowserWindow();
        }

        private static void ExecuteStopCommand()
        {
            //_bcViewModel.StopBrowser();
        }
    }
}
