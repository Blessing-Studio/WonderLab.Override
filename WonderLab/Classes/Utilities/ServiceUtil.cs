using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;

namespace WonderLab.Classes.Utilities {
    public static class ServiceUtil {
        public static void AddWindowFactory<T>(this IServiceCollection services) where T : class {
            services.AddTransient<T>();
            services.AddSingleton((Func<IServiceProvider, Func<T>>) delegate (IServiceProvider s) {
                IServiceProvider s2 = s;
                return () => s2.GetService<T>()!;
            });

            services.AddSingleton<IAbstractFactory<T>, AbstractFactory<T>>();
        }
    }

    public class AbstractFactory<T> : IAbstractFactory<T> where T : class {
        private readonly Func<T> _factory;

        public AbstractFactory(Func<T> factory) {
            _factory = factory;
        }

        public T Create() {
            return _factory();
        }
    }
}
