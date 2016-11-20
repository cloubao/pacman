using DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Dependency injection mechanism inspired by Dagger2 (Dependency Injection for Android/Java)
// The Component hold injectors for injectable classes
namespace DependencyInjection
{
    public class Component<M> where M: IModule
    {
        private M module;
        private Dictionary<string, object> injectors;

        public Component(M module)
        {
            this.module = module;
            this.injectors = new Dictionary<string, object>();
        }

        public void addInjector<I,T>(string typeName, I injector) where I: Injector<M,T> 
        {
            injectors.Add(typeName, injector);
        }

        public void inject<T>(T mObject)
        {
            if(injectors.ContainsKey(mObject.GetType().Name))
            {
                ((Injector<M,T>)injectors[mObject.GetType().Name]).inject(module, mObject);
            }
            else
            {
                throw new ArgumentException("No injector found");
            }
        }
    }
}
