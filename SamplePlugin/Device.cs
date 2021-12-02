﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buttplug;

namespace AetherSenseRedux
{
    internal class Device
    {
        public readonly ButtplugClientDevice ClientDevice;
        public List<IPattern> Patterns;
        public string Name { get => ClientDevice.Name; }
        private double _lastIntensity;
        private bool _active;

        public Device(ButtplugClientDevice clientDevice)
        {
            ClientDevice = clientDevice;
            Patterns = new List<IPattern>();
            _lastIntensity = 0;
            _active = true;
        }

        public async Task Run()
        {
            while (_active)
            {
                await OnTick();
                await Task.Delay(10);
            }
        }

        public void Stop()
        {
            _active = false;
            Patterns.Clear();

            var t = Task.Run(() => WriteAsync(0));
            t.Wait();
        }

        private async Task OnTick()
        {
            List<double> intensities = new List<double>();
            foreach (var pattern in Patterns)
            {
                try
                {
                    intensities.Add(pattern.GetIntensityAtTime(DateTime.UtcNow));
                }
                catch (Exception)
                {
                    Patterns.Remove(pattern);
                }
            }

            //TODO: Allow different merge modes besides average
            double intensity = (intensities.Any()) ? intensities.Average() : 0;

            await WriteAsync(intensity);
        }

        private async Task WriteAsync(double intensity)
        {
            double clampedIntensity = Clamp(intensity, 0, 1);

            if (_lastIntensity == clampedIntensity)
            {
                return;
            }

            _lastIntensity = clampedIntensity;

            await ClientDevice.SendVibrateCmd(clampedIntensity);
        }
        private static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}