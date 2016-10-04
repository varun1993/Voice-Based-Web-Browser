using System;
using UWIC.FinalProject.Common.svcUpdateService;

namespace UWIC.FinalProject.Common
{
    public class UpdateManager
    {
        readonly WebBrowserUpdateServiceClient _updateServiceClient = new WebBrowserUpdateServiceClient();

        public bool CheckForUpdates(Version currentVersion, out string url)
        {
            try
            {
                var result = _updateServiceClient.CheckForUpdates(currentVersion);
                url = Convert.ToBoolean(result["Available"]) ? result["url"].ToString() : "";
                return (Convert.ToBoolean(result["Available"]));
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}
