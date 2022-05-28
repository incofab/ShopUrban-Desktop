using ShopUrban.Model;
using System;

namespace ShopUrban.Util
{
    public class Toast
    {
        public static void showInformation(string message)
        {
            MyEventBus.post(new EventMessage(EventMessage.EVENT_TOAST_INFO, message));
        }

        public static void showSuccess(string message)
        {
            MyEventBus.post(new EventMessage(EventMessage.EVENT_TOAST_SUCCESS, message));
        }

        public static void showWarning(string message)
        {
            MyEventBus.post(new EventMessage(EventMessage.EVENT_TOAST_WARNING, message));
        }

        public static void showError(string message)
        {
            MyEventBus.post(new EventMessage(EventMessage.EVENT_TOAST_ERROR, message));
        }

    }
}
