using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Runtime
{
    public interface IRuntimeImplementation
    {
        object Execute(object[] input);
    }
}
