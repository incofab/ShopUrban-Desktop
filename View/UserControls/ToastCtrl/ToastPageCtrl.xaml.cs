using ShopUrban.Model;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShopUrban.View.UserControls.ToastCtrl
{
    /// <summary>
    /// Interaction logic for ToastPageCtrl.xaml
    /// </summary>
    public partial class ToastPageCtrl : UserControl
    {
        public ToastPageCtrl()
        {
            InitializeComponent();

            this.Loaded += UserControl_Loaded;

            MyEventBus.subscribe(handleEvent);
        }

        void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Closing += window_Closing;
        }
        void window_Closing(object sender, global::System.ComponentModel.CancelEventArgs e)
        {
            //do something before the window is closed...
            MyEventBus.unSubscribe(handleEvent);
        }

        public void showToast(string message, SingleToastCtrl.ToastSetting toastSetting)
        {
            var toastView = new SingleToastCtrl(message, toastSetting);

            toastList.Children.Add(toastView);

            TimerHelper.SetTimeout(5000, () => {

                Application.Current.Dispatcher.Invoke(new Action(() => {

                    toastList.Children.Remove(toastView);

                }));

            });
        }
        public void success(string message)
        {
            showToast(message, new SingleToastCtrl.ToastSetting
            {
                icon = MaterialDesignThemes.Wpf.PackIconKind.CheckCircleOutline,
                background = Brushes.Green,
            });
        }
        public void error(string message)
        {
            showToast(message, new SingleToastCtrl.ToastSetting
            {
                icon = MaterialDesignThemes.Wpf.PackIconKind.ErrorOutline,
                background = Brushes.DarkRed,
            });
        }

        public void info(string message)
        {
            showToast(message, new SingleToastCtrl.ToastSetting
            {
                icon = MaterialDesignThemes.Wpf.PackIconKind.EnvelopeOutline,
                background = Brushes.DarkBlue,
            });
        }

        public void warning(string message)
        {
            showToast(message, new SingleToastCtrl.ToastSetting
            {
                icon = MaterialDesignThemes.Wpf.PackIconKind.Exclamation,
                background = Brushes.Orange,
            });
        }

        private void handleEvent(EventMessage eventMessage)
        {
            switch (eventMessage.eventId)
            {
                case EventMessage.EVENT_TOAST_SUCCESS:
                    success((string)eventMessage.data);
                    break;
                case EventMessage.EVENT_TOAST_ERROR:
                    error((string)eventMessage.data);
                break;
                case EventMessage.EVENT_TOAST_INFO:
                    info((string)eventMessage.data);
                break;
                case EventMessage.EVENT_TOAST_WARNING:
                    warning((string)eventMessage.data);
                break;
            }
        }
    }
}
