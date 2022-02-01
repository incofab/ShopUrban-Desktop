using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopUrban.Model
{
    class EventMessage
    {
        public const int EVENT_ADD_TO_CART = 1;
        public const int EVENT_ORDER_CREATED = 2;
        public const int EVENT_LOGOUT = 3;
        public const int EVENT_OPEN_PRODUCT_SYNC = 4;
        public const int EVENT_PRODUCT_SYNC_COMPLETED = 5;
        public const int EVENT_DRAFT_CREATED = 6;
        public const int EVENT_DRAFT_DELETED = 7;
        public const int EVENT_DRAFT_EDIT = 8;

        public int eventId
        {
            get;
            private set;
        }
        public object data
        {
            get;
            private set;
        }
        public string message
        {
            get;
            private set;
        }

        public EventMessage(int eventId, object data)
        {
            this.eventId = eventId;
            this.data = data;
        }
        public EventMessage(int eventId, object data, string message)
        {
            this.eventId = eventId;
            this.data = data;
            this.message = message;
        }
        
    }
}
