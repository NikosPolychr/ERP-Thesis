using Erp.CustomControls;
using Erp.Helper;
using Erp.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Erp.View
{
    public partial class UserControlMenuItem : UserControl
    {
        private readonly MainView _context;

        public UserControlMenuItem(ItemMenu itemMenu, MainView context)
        {
            InitializeComponent();

            _context = context;

            ExpanderMenu.Visibility = itemMenu.SubItems == null ? Visibility.Collapsed : Visibility.Visible;
            ListViewItemMenu.Visibility = itemMenu.SubItems == null ? Visibility.Visible : Visibility.Collapsed;

            DataContext = itemMenu;

            FirstButtonCommand = new RelayCommand<SubItem>(ExecuteFirstButton);
            SecondButtonCommand = new RelayCommand<SubItem>(ExecuteSecondButton);
        }

        #region Button Commands
        public ICommand FirstButtonCommand { get; }
        public ICommand SecondButtonCommand { get; }

        private void ExecuteFirstButton(SubItem subItem)
        {
            if (subItem?.ScreenFactory == null)
            {
                MessageBox.Show($"No screen available for {subItem?.Name ?? "Unknown"}",
                    "Navigation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var screen = subItem.ScreenFactory();
            _context.SwitchScreen2(screen, subItem.Name);
            _context.ClearSelectionExcept(this);
        }
        private void ExecuteSecondButton(SubItem subItem)
        {
            if (subItem == null) return;

            // Find matching search item by SearchKey
            var searchItem = _context.SubItemsSearch
                                     .FirstOrDefault(s => s.SearchKey == subItem.SearchKey);

            if (searchItem == null)
            {
                MessageBox.Show($"No search screen registered for {subItem.Name}",
                    "Search", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (searchItem.ScreenFactory == null)
            {
                MessageBox.Show($"Search screen factory missing for {searchItem.Name}",
                    "Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var screen = searchItem.ScreenFactory();
            _context.SwitchScreen2(screen, searchItem.Name);
            _context.ClearSelectionExcept(this);

            if (searchItem.FilterFactory != null)
            {
                var popup = new FlatSearchWindow(searchItem.FilterFactory())
                {
                    Owner = Application.Current.MainWindow
                };
                popup.ShowDialog();
            }
        }

        #endregion

        public void ClearSelection()
        {
            ListViewMenu.SelectedItem = null;
        }
    }
}
