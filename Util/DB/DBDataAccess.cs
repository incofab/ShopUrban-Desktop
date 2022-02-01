using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopUrban.Util
{
    class DBDataAccess
    {
        private readonly DBCreator dBCreator;
        //SQLiteConnection dbConnection;
        //SQLiteCommand command;
        //string sqlCommand;

        public DBDataAccess(DBCreator dBCreator)
        {
            this.dBCreator = dBCreator;
        }

        public void create()
        {
            using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            {
                //cnn.Query<>();
            }
        }


    }
}
