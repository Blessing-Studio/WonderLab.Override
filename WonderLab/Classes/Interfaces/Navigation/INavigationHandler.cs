using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderLab.Classes.Interfaces.Navigation {
    public interface INavigationHandler {
        bool CanGoBack { get; }

        bool CanGoForward { get; }

        INavigationHandler? Parent { get; }

        INavigationProvider NavigationProvider { get; }

        void GoBack();

        void GoForward();

        void NavigateTo(string key, object? parameter = null);

        void InitializeNavigation(INavigationProvider navigationProvider, IServiceScope scope, INavigationHandler? parent);
    }
}
