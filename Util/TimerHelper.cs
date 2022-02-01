using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace ShopUrban.Util
{
    public class TimerHelper
    {
        public static IInterruptable SetInterval(int intervalMillis, Action function)
        {
            return StartTimer(intervalMillis, function, true);
        }

        public static IInterruptable SetTimeout(int intervalMillis, Action function)
        {
            return StartTimer(intervalMillis, function, false);
        }

        private static IInterruptable StartTimer(int interval, Action function, bool autoReset)
        {
            Action functionCopy = (Action)function.Clone();

            System.Timers.Timer timer = new System.Timers.Timer { Interval = interval, AutoReset = autoReset };
            timer.Elapsed += (sender, e) => functionCopy();
            timer.Start();

            return new TimerInterrupter(timer);
        }

    }
    public class TimerInterrupter : IInterruptable
    {
        private readonly System.Timers.Timer _timer;

        public TimerInterrupter(System.Timers.Timer timer)
        {
            if (timer == null) throw new ArgumentNullException(nameof(timer));
            _timer = timer;
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }

    public interface IInterruptable
    {
        void Stop();
    }

}
