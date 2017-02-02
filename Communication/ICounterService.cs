using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication
{
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface ICounterService : IService
    {
        Task<CounterResult> GetValue();
    }
}
