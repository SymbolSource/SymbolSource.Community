using System;

namespace SymbolSource.Gateway.NuGet.Core
{
    public static class ServiceLocator
    {
        public static IServiceLocator Locator { get; set; }

         public static T Resolve<T>()
         {
             if (Locator == null)
                 throw new ArgumentException("Locator is not set");
             return Locator.Resolve<T>();
         }
    }

    public interface IServiceLocator
    {
        T Resolve<T>();
    }

    public class SimpleServiceLocator : IServiceLocator
    {
        private readonly Func<Type, object> resolver;

        public SimpleServiceLocator(Func<Type, object> resolver)
        {
            this.resolver = resolver;
        }

        public T Resolve<T>()
        {
            return (T) resolver(typeof (T));
        }
    }
}