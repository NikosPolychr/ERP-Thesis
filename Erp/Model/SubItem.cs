using System;
using System.Windows.Controls;

namespace Erp.Model
{
    public class SubItem
    {
        private object _sharedVm; // cache so Screen + Filter share same VM

        public SubItem(
            string name,
            Func<UserControl> screenFactory = null,
            Func<UserControl> filterFactory = null,
            string searchKey = null,
            Func<object> viewModelFactory = null)
        {
            Name = name;
            SearchKey = searchKey ?? name;
            ViewModelFactory = viewModelFactory;

            // Wrap ScreenFactory
            ScreenFactory = () =>
            {
                var vm = GetOrCreateVm();
                var screen = screenFactory?.Invoke();
                if (screen != null)
                    screen.DataContext = vm;
                return screen;
            };

            // Wrap FilterFactory
            FilterFactory = () =>
            {
                var vm = GetOrCreateVm();
                var filter = filterFactory?.Invoke();
                if (filter != null)
                    filter.DataContext = vm;
                return filter;
            };
        }

        public string Name { get; }
        public string SearchKey { get; }

        public Func<UserControl> ScreenFactory { get; }
        public Func<UserControl> FilterFactory { get; }

        public Func<object> ViewModelFactory { get; }

        private object GetOrCreateVm()
        {
            if (_sharedVm == null && ViewModelFactory != null)
                _sharedVm = ViewModelFactory();
            return _sharedVm;
        }

        /// <summary>
        /// Reset the shared VM so a new one will be created next time.
        /// </summary>
        public void ResetVm() => _sharedVm = null;
    }
}
