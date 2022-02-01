using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ShopUrban.Util
{
    /// <summary>
    /// @deprecated Use TimerHelper instead
    /// </summary>
    class Timer
    {
        private long duration;
        private long counter;

        private DispatcherTimer dispatcherTimer;
        private bool counterRunning;

        public Timer(Action<long> eventHandler)
        {

            dispatcherTimer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, 
                delegate{
                    eventHandler(this.duration - counter);
                    counter++;
                }, Application.Current.Dispatcher);

        }

        public void start(long duration)
        {
            this.duration = duration;
            dispatcherTimer.Start();
            counterRunning = true;
        }

        public void pause()
        {
            dispatcherTimer.Stop();
            counterRunning = false;
        }
        public void resume()
        {
            dispatcherTimer.Start();
            counterRunning = true;
        }
        public void stop()
        {
            dispatcherTimer.Stop();
            counterRunning = false;
            counter = 0;
        }

        public bool isRunning() { return counterRunning;  } 

    }
}
