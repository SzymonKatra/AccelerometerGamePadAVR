using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccelerometerGamePadUtility
{
    public class DeviceNotFoundException : Exception
    {
        public DeviceNotFoundException()
        {
        }
        public DeviceNotFoundException(string message)
            : base(message)
        {
        }
    }
}
