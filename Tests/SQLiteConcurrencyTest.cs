using ShopUrban.Model;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopUrban.Tests
{
    public class SQLiteConcurrencyTest
    {

        public void run()
        {
            Helpers.log("*********First Dummy started**************");
            Task.Run(() => {
                //Helpers.runOnUiThread(() => { 
                    //createDummyCartDraft();
                    loopSetting();
                //});
            });

            Helpers.log("*********Second Dummy started**************");

            Task.Run(() => {
                //Helpers.runOnUiThread(() => {
                    //createDummyStaff();
                    getCountStaff();
                //});
            });
        }

        private void loopSetting()
        {
            for (int i = 0; i < 200; i++)
            {
                var staffId = Setting.getValue(Setting.KEY_LOGGED_IN_STAFF_ID);
                
                Helpers.log($"No {i}  KEY_LOGGED_IN_STAFF_ID {staffId}");
            }
        }
        private void getCountStaff()
        {
            for (int i = 0; i < 200; i++)
            {
                List<Staff> l = Staff.all();
                
                Helpers.log(l != null ? $"No {i}  Staff {l.Count}" : "Staff empty");
            }
        }

        private void createDummyCartDraft()
        {
            for (int i = 0; i < 200; i++)
            {
                var c = new CartDraft() { amount = i, quantity = i, status = "Active" };

                CartDraft.save(c);

                Helpers.log("No = " + i + ", CartDraft");
            }
        }

        private void createDummyStaff()
        {
            for (int i = 0; i < 200; i++)
            {
                List<Staff> staffs = Staff.all();
                
                Helpers.log($"Staff {i} = "+staffs[0].name);

                continue;

                var name = $"name = {i}";
                var token = Helpers.md5(name);

                var c = new Staff()
                {
                    name = name,
                    username = name,
                    password = token,
                    token = token,
                    phone = $"Phone - {i}",
                    pin = $"{i}",
                    user_id = (i + 2)
                };

                Staff.save(c);

                Helpers.log("No = " + i + ", Staff");
            }
        }

    }
}
