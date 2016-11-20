using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    public interface Injector<M, T> where M: IModule
    {
        void inject(M module, T mObject); 
    }
}
