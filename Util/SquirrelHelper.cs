using ShopUrban.Model;
using ShopUrban.View.Receipt;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace ShopUrban.Util
{
    public class SquirrelHelper
    {
        const string UPDATE_URL = "https://github.com/incofab/shopurban-desktop-squirrel-update";
        //const string UPDATE_URL = "https://github.com/incofab/ShopUrban-Desktop";
            //@"C:\Users\User\Desktop\Project Files\Ogaboss\release";

        private static SquirrelHelper instance;
        private UpdateManager manager;

        public SquirrelHelper()
        {
            //manager = new UpdateManager(UPDATE_URL);
            //initManager();
        }

        //private async void initManager()
        //{
        //    manager = await UpdateManager.GitHubUpdateManager(UPDATE_URL);
        //}

        public static SquirrelHelper getInstance()
        {
            if (instance == null) instance = new SquirrelHelper();

            return instance;
        }

        public async Task<bool> checkForUpdate()
        {
            if (manager == null)
            {
                manager = await UpdateManager.GitHubUpdateManager(UPDATE_URL);
            }

            var updateInfo = await manager.CheckForUpdate();

            return updateInfo.ReleasesToApply.Count > 0;
        }

        public async Task<ReleaseEntry> update()
        {
            if (manager == null)
            {
                manager = await UpdateManager.GitHubUpdateManager(UPDATE_URL);
            }

            ReleaseEntry releaseEntry = await manager.UpdateApp();

            return releaseEntry;
        }

        public void dispose()
        {
            if(manager != null)
            {
                manager.Dispose();
                manager = null;
            }
        }

        

    }
}
