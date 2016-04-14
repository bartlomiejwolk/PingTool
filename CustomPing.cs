using UnityEngine;
using System.Diagnostics;

namespace PingTool
{
    public sealed class CustomPing
    {
        private Ping ping;
        private Stopwatch stopWatch;

        /// <summary>
        /// Returns current ping time in miliseconds.
        /// </summary>
        public long TimeSinceCreation {
            get
            {
                return stopWatch.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// Ping travel time after round trip.
        /// </summary>
        public int RoundTripTime
        {
            get
            {
                return ping.time;
            }
        }

        public bool IsDone {
            get
            {
                return ping.isDone;
            }
        }

        public CustomPing(string remoteIp)
        {
            ping = new Ping(remoteIp);
            stopWatch = new Stopwatch();

            stopWatch.Start();
        }
    }
}
