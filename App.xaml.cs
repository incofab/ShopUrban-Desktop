using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ShopUrban
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //defer other startup processing to base class
            base.OnStartup(e);

            registerExceptionHandlers();
        }

        private void registerExceptionHandlers()
        {
            //Console.WriteLine("Errrr = App started");

            //define application exception handler
            Application.Current.DispatcherUnhandledException += (sender, e) => {
                e.Handled = true;
                handleException(e.Exception);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
                handleException(e.ExceptionObject as Exception);
            };

            //Dispatcher.UnhandledException += UnhandledException;

            TaskScheduler.UnobservedTaskException += (sender, e) => {
                handleException(e.Exception);
            };
        }

        private void handleException(Exception e)
        {
            Helpers.log("Errrr = Application_DispatcherUnhandledException()", true);
            MessageBox.Show("An unhandled exception just occurred: " + e.Message);
            //MessageBox.Show("An unhandled exception just occurred: " 
            //    + e.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

    }

}
