using ShopUrban.Model;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopUrban.Services
{
    public class LoginHelper
    {
        public static void logout()
        {
            DateTime.Now.ToString(KStrings.TIME_FORMAT);

            Setting.update(new
            {
                key = Setting.KEY_REMEMBER_LOGIN,
                display_name = "Remeber Login",
                value = "",
                type = "boolean"
            }, Setting.KEY_REMEMBER_LOGIN);

            Setting.update(new
            {
                key = Setting.KEY_LOGGED_IN_STAFF_ID,
                display_name = "Logged in Staff ID",
                value = "",
                type = "integer"
            }, Setting.KEY_LOGGED_IN_STAFF_ID);

            MyEventBus.post(new EventMessage(EventMessage.EVENT_LOGOUT, null));
        }

    }
}
