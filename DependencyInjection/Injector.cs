using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection

{
    // Dependency injection mechanism inspired by Dagger2 (Dependency Injection for Android/Java)
    // The injector initialize the dependencies of an object by using the module as a provider
    // TODO: Use delegate methods instead of injector objects for the injection
    public interface Injector<M, T> where M: IModule
    {
        void inject(M module, T mObject); 
    }
}
