using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeoCoordinator
{
    public static class GeoCoordinateWatcherExtensions
    {
        public static GeoCoordinate GetCurrentLocation(this GeoCoordinateWatcher watcher, TimeSpan timeout)
        {
            // PositionChangedイベントを監視し、イベント発生時に待機中のスレッドを再開する
            watcher.PositionChanged += (sender, eventArgs) =>
            {
                Monitor.Enter(watcher);
                try
                {
                    Monitor.PulseAll(watcher);
                }
                finally
                {
                    Monitor.Exit(watcher);
                }
            };

            Monitor.Enter(watcher);
            try
            {
                // GeoCoordinateWatcherを起動し、PositionChangedイベントが発生するか
                // タイムアウトまで待機する
                watcher.Start();
                Monitor.Wait(watcher, timeout);
            }
            finally
            {
                Monitor.Exit(watcher);
            }
            watcher.Stop();
            return watcher.Position.Location;
        }
    }
}
