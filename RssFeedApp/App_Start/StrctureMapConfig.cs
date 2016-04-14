using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using StructureMap;

namespace RssFeedApp
{
    public class StrctureMapConfig : IDependencyResolver
    {
        private readonly IContainer _container;

        public StrctureMapConfig(IContainer container)
        {
            _container = container;
        }

        public IDependencyScope BeginScope()
        {
            return new StrctureMapConfig(_container.GetNestedContainer());
        }

        public void Dispose()
        {
            _container?.TransientTracking.DisposeAndClear();
            //BeginScope()?.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return serviceType.IsAbstract || serviceType.IsInterface
                 ? _container.TryGetInstance(serviceType)
                 : _container.GetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.GetAllInstances(serviceType).Cast<object>();
        }
    }
}