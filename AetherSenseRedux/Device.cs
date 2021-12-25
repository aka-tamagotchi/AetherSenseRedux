using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AetherSenseRedux.Pattern;
using Buttplug;
using Dalamud.Logging;
using System.Threading;

namespace AetherSenseRedux
{
    internal class Device : IDisposable
    {
        public readonly ButtplugClientDevice ClientDevice;
        public List<IPattern> Patterns;
        public string Name { get => ClientDevice.Name; }
        public double UPS
        {
            get
            {
                // The actual UPS counter is derived from this internal average time per update value
                return 1000 / _ups;
            }
        }

        private double _ups = 16;       // we initialize this to the target time per update value just to avoid confusing users
        private double _lastIntensity;
        private bool _active;
        private int frameTime = 16;     // The target time per update, in this case 16ms = ~60 ups, and also a pipe dream for BLE toys.

        public Device(ButtplugClientDevice clientDevice)
        {
            ClientDevice = clientDevice;
            Patterns = new List<IPattern>();
            _lastIntensity = 0;
            _active = true;
        }

        public void Dispose()
        {
            _active = false;
            Patterns.Clear();
            ClientDevice.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            Task.Run(MainLoop).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task MainLoop()
        {
            Stopwatch timer = new();
            while (_active)
            {
                timer.Restart();
                await OnTick();
                var t = timer.ElapsedMilliseconds;
                if (t < frameTime)
                {
                    // We use this instead of Task.Delay because Task.Delay seems to emulate the old 15.6ms
                    // system timers and often delays for up to three cycles longer than requested whereas
                    // modern Windows 10 systems have a system timer resolution of more like 1.56ms, which
                    // is much preferable since buttplug requires real-time messaging.
                    // Worst case it performs just as poorly as Task.Delay, but best case we get accuracy
                    // to every other millisecond. Which is less cpu intensive than a SpinWait but also more
                    // than good enough for butts.
                    while (timer.ElapsedMilliseconds < frameTime)
                    {
                        Thread.Sleep(1);
                    }
                } 
                else
                {
                    PluginLog.Verbose("OnTick for device {0} took {1}ms too long!", Name, t - frameTime);
                }
                _ups = _ups * 0.9 + timer.ElapsedMilliseconds * 0.1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            _active = false;
            Patterns.Clear();

            var t = Task.Run(() => Write(0));
            t.Wait();
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task OnTick()
        {

            List<double> intensities = new();
            DateTime t = DateTime.UtcNow;
            var patternsToRemove = new List<IPattern>();

            lock (Patterns)
            {
                foreach (var pattern in Patterns)
                {
                    try
                    {
                        intensities.Add(pattern.GetIntensityAtTime(t));
                    }
                    catch (PatternExpiredException)
                    {
                        patternsToRemove.Add(pattern);
                    }
                }
            }
            foreach (var pattern in patternsToRemove)
            {
                lock (Patterns)
                {
                    Patterns.Remove(pattern);
                }
            }
            //TODO: Allow different merge modes besides average
            double intensity = (intensities.Any()) ? intensities.Average() : 0;

            await Write(intensity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intensity"></param>
        private async Task Write(double intensity)
        {
            // clamp intensity before comparing to reduce unnecessary writes to device
            double clampedIntensity = Clamp(intensity, 0, 1);

            if (_lastIntensity == clampedIntensity)
            {
                return;
            }

            _lastIntensity = clampedIntensity;

            try
            {
                await ClientDevice.SendVibrateCmd(clampedIntensity);

            } catch (Exception)
            {
                // Connecting to an intiface server on Linux will spam the log with bluez errors
                // so we just ignore all exceptions from this statement. Good? Probably not. Necessary? Yes.
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
