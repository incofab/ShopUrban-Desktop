using ShopUrban.Model;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopUrban.Services
{
    public class LoginHelper
    {
        public static void logout(bool deleteStaffData = false)
        {
            DateTime.Now.ToString(KStrings.TIME_FORMAT);

            Setting.update(new
            {
                key = Setting.KEY_REMEMBER_LOGIN,
                display_name = "Remeber Login",
                value = "",
                type = "boolean"
            }, Setting.KEY_REMEMBER_LOGIN);

            if (deleteStaffData)
            {
                int.TryParse(Setting.getValue(Setting.KEY_LOGGED_IN_STAFF_ID), out int staffId);

                if(staffId > 0) 
                    Staff.deleteById(staffId);
            }

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
