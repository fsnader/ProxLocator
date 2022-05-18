using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxLocator.Engine.Entities;

namespace ProxLocator.Engine.Processor
{
    public interface IProcessor
    {
        Position LastPosition { get; }
        void ProcessMessage(LocatorMessage message);
    }
}
