using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WE.Core.Time
{
    public class TimeManager
    {
        private readonly Stopwatch _stopwatch = new();
        private double _lastTime = 0;

        public double TotalTime => _stopwatch.Elapsed.TotalSeconds;

        public void Start()
        {
            _stopwatch.Start();
            _lastTime = _stopwatch.Elapsed.TotalSeconds;
        }

        /// <summary>
        /// Calculates and returns Delta Time (the time elapsed since the last call to this method)
        /// </summary>
        /// <returns></returns>
        public double GetDeltaTime()
        {
            double currentTime = _stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentTime - _lastTime;
            _lastTime = currentTime;
            return deltaTime;
        }
    }
}
