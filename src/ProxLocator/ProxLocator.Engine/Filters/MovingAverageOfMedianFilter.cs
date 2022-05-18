using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxLocator.Engine.Entities;
using ProxLocator.Engine.Utils;

namespace ProxLocator.Engine.Filters
{
    public class MovingAverageOfMedianFilter : IFilter
    {
        private readonly MedianFilter _medianFilter;
        private readonly MovingAverageFilter _movingAverageFilter;

        public MovingAverageOfMedianFilter(int averagePeriod, int medianPeriod)
        {
            _medianFilter = new MedianFilter(medianPeriod);
            _movingAverageFilter = new MovingAverageFilter(averagePeriod);
        }

        public LocatorMessage Filter(LocatorMessage message)
        {
            return _movingAverageFilter
                .Filter(_medianFilter
                    .Filter(message));
        }

        public void Reset()
        {
            _medianFilter.Reset();
            _movingAverageFilter.Reset();
        }
    }
}
