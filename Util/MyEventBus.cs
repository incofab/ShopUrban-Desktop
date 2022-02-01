using ShopUrban.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopUrban.Util
{
    class MyEventBus
    {
        public delegate void ExecuteDel(object param);

        public ExecuteDel allSubscriptions;

        public List<Action<EventMessage>> actions = new List<Action<EventMessage>>();

        public MyEventBus()
        {

        }

        public static MyEventBus instance;
        public static MyEventBus getInstance()
        {
            if(instance == null)
            {
                instance = new MyEventBus();
            }
            return instance;
        }

        public static void subscribe(Action<EventMessage> action)
        {
            if (getInstance().actions.Contains(action))
            {
                throw new Exception("Duplicate action: This action already exits");
            }
            getInstance().actions.Add(action);
        }

        public static void unSubscribe(Action<EventMessage> action)
        {
            if (getInstance().actions.Contains(action))
            {
                getInstance().actions.Remove(action);
            }
        }

        public static void post(EventMessage param)
        {
            //getInstance().allSubscriptions(param);

            List<Action<EventMessage>> actionsList = getInstance().actions;

            for (int i = 0; i < actionsList.Count(); i++)
            {
                actionsList[i](param);
            }
        }


    }


}
