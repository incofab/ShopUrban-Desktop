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
        const string UPDATE_URL = @"C:\Users\User\Desktop\Project Files\Ogaboss\release";

        private static SquirrelHelper instance;
        private UpdateManager manager;

        public SquirrelHelper()
        {
            manager = new UpdateManager(UPDATE_URL);
        }

        public static SquirrelHelper getInstance()
        {
            if (instance == null) instance = new SquirrelHelper();

            return instance;
        }

        public async Task<bool> checkForUpdate()
        {
            var updateInfo = await manager.CheckForUpdate();

            return updateInfo.ReleasesToApply.Count > 0;
        }

        public async void update()
        {
            await manager.UpdateApp();
        }


        

    }
}
