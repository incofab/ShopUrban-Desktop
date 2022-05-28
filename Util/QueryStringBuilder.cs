using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopUrban.Util
{
    class QueryStringBuilder
    {
        private readonly List<KeyValuePair<string, string>> _list;

        public QueryStringBuilder(List<KeyValuePair<string, string>> l = null)
        {
            _list = l == null ? new List<KeyValuePair<string, string>>() : l;
        }

        public void Add(string name, string value)
        {
            _list.Add(new KeyValuePair<string, string>(name, value));
        }

        public override string ToString()
        {
            return String.Join("&", 
                _list.Select(kvp => String.Concat(Uri.EscapeDataString(kvp.Key), 
                "=", Uri.EscapeDataString(kvp.Value))));
        }

    }
}
