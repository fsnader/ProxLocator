using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSockets.Core
{
    public interface DataReceiver
    {
        void StartListening();
        void Dispose();
    }
}
