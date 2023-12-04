using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MyApps.Utilities
{
    public static class DispatcherHelper
    {
        public static void Delay(this Dispatcher dispatcher, int delay, Action<object> action, object param = null)
        {
            Task.Delay(delay).ContinueWith(_ => { dispatcher.Invoke(action, param); });
        }
        
        public static void CheckBeginInvokeOnUI(Action action)
        {
            if (action == null)
                return;

            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.BeginInvoke(action);
        }
    }
}
