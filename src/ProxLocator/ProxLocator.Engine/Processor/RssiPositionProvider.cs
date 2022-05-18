using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxLocator.Engine.Entities;

namespace ProxLocator.Engine.Processor
{
    public class RssiPositionProvider : IPositionProvider
    {
        public Position GetPosition(LocatorMessage[] dataRow)
        {
            var distance1 = DistanceFromRssi(dataRow[0].Rssi);
            var distance2 = DistanceFromRssi(dataRow[0].Rssi);

            var x = Math.Pow(distance1, 2) / (2 * distance2);
            var y = Math.Sqrt(Math.Pow(distance1, 2) - Math.Pow(x, 2));

            return new Position
            {
                X = x,
                Y = y,
                TimeStamp = DateTime.Now
            };
        }

        private double DistanceFromRssi(double rssi)
        {
            var txPower = -59; //hard coded power value. Usually ranges between -59 to -65

            if (rssi == 0)
            {
                return -1.0;
            }

            var ratio = rssi * 1.0 / txPower;
            if (ratio < 1.0)
            {
                return Math.Pow(ratio, 10);
            }
            else
            {
                var distance = (0.89976) * Math.Pow(ratio, 7.7095) + 0.111;
                return distance;
            }
        }

    }
}
