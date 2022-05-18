using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxLocator.Engine.Entities;

namespace ProxLocator.Engine.Filters
{
    interface IFilter
    {
        LocatorMessage Filter(LocatorMessage message);
        void Reset();
    }
}
