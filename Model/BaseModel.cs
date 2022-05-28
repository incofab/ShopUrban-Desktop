using Dapper;
using ShopUrban.Util;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ShopUrban.Model
{
    public class BaseModel: INotifyPropertyChanged
    {
        public const string TIME_STAMPS_STR =
            "created_at VARCHAR(255), " +
            "updated_at VARCHAR(255)";
        public const string TIME_STAMPS =
            " created_at DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL, " +
            " updated_at DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL";

        public static string prepareEqQuery(object c)
        {
            var query = "";
            
            if (c == null) return query;

            var i = -1;
            PropertyInfo[] propertyInfos = c.GetType().GetProperties();
            foreach (PropertyInfo prop in propertyInfos)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var propValue = prop.GetValue(c, null);
                var value = propValue == null ? null : propValue.ToString();
                var name = prop.Name;
                //if (string.IsNullOrEmpty(value)) continue;
                i++;

                query += $" {(i==0 ? "WHERE" : "AND")}  {name} = @{name} ";
            }

            return query;
        }

        public static string prepareInsertQuery(string table, object c, string[] fillable = null)
        {
            var query = "";
            var queryValues = "";
            var i = -1;

            PropertyInfo[] propertyInfos = c.GetType().GetProperties();

            foreach (PropertyInfo prop in propertyInfos)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var propValue = prop.GetValue(c, null);
                var value = propValue == null ? null : propValue.ToString();
                var name = prop.Name;
                //if (string.IsNullOrEmpty(value)) continue;

                if (fillable != null && !fillable.Contains(name)) continue;

                i++;

                query += $" {name},";
                queryValues += $" @{name},";
            }

            query = query.Trim(',');
            queryValues = queryValues.Trim(',');

            query = $"INSERT INTO {table} ({query}) VALUES ({queryValues})";

            return query;
        }

        public static string prepareUpdateQuery(string table, object c, object where = null, string[] fillable = null)
        {
            var query = "";

            PropertyInfo[] propertyInfos = c.GetType().GetProperties();

            foreach (PropertyInfo prop in propertyInfos)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var propValue = prop.GetValue(c, null);
                var value = propValue == null ? null : propValue.ToString();
                var name = prop.Name;

                if (fillable != null && !fillable.Contains(name)) continue;

                query += $" {name} = @{name},";
            }

            query = query.Trim(',');

            var whereQuery = prepareEqQuery(where);

            query = $"UPDATE {table} SET {query} {whereQuery}";

            return query;
        }

        public static string deleteQuery(string table, object where)
        {
            return $"DELETE FROM {table} " + prepareEqQuery(where);
        }

        public static void execute(string query, object obj)
        {
            Helpers.log(query);

            var cnn = DBCreator.getConn();
            //using (IDbConnection cnn = new SQLiteConnection(DBCreator.dbConnectionString))
            //{
            cnn.Execute(query, obj);
            //}
        }

        public override string ToString()
        {
            var str = "";

            PropertyInfo[] propertyInfos = this.GetType().GetProperties();

            foreach (PropertyInfo prop in propertyInfos)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var propValue = prop.GetValue(this, null);
                //var value = propValue == null ? null : propValue.ToString();
                var name = prop.Name;

                str += $"type||{name} = {type}||{propValue}, -- ";
            }

            return str;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }

}
