using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSD.Toolkit.Logger.Messages;

namespace DSD.Toolkit.Logger;

public interface ILogger
{
    void Log(LogMessage message);
}
