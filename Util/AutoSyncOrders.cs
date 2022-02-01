using ShopUrban.Util.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopUrban.Util
{
    class AutoSyncOrders
    {
        private IInterruptable timerObj;

        private static AutoSyncOrders instance;

        public AutoSyncOrders()
        {
            instance = this;
        }

        public static AutoSyncOrders getInstance()
        {
            if (instance == null) new AutoSyncOrders();

            return instance;
        }

        public void start()
        {
            if (instance.timerObj != null) return;

            timerObj = TimerHelper.SetInterval(KStrings.DEV ? 20000 : 1 * 60 * 1000, syncOrders);
        }

        public void stop()
        {
            if (timerObj == null) return;

            timerObj.Stop();

            timerObj = null;
        }

        private async void syncOrders()
        {
            BaseResponse baseResponse = await ProductDownloader.getInstance().uploadOrders(false);

            if (baseResponse == null) return;

            stop();
        }
    }

}
