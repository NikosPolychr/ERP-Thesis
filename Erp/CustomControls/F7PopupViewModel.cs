using System;
using System.ComponentModel;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid;
using System.Windows.Data;
using Erp.Model;
using Erp.Helper;

namespace Erp.CustomControls
{
    public class F7PopupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private Columns _sfGridColumns;
        public Columns SfGridColumns
        {
            get => _sfGridColumns;
            set
            {
                _sfGridColumns = value;
                OnPropertyChanged(nameof(SfGridColumns));
            }
        }

        private ICollectionView _collectionView;
        public ICollectionView CollectionView
        {
            get => _collectionView;
            set
            {
                _collectionView = value;
                OnPropertyChanged(nameof(CollectionView));
            }
        }

        private object _selectedItem;
        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public string F7key { get; set; }

        public ICommand RowDoubleClickCommand { get; }

        // Event to notify the popup host about the selected item
        public event Action<object> ItemSelected;

        public F7PopupViewModel(F7Data f7Data)
        {
            SfGridColumns = f7Data.SfGridColumns;
            CollectionView = f7Data.CollectionView;
            F7key = f7Data.F7key;

            RowDoubleClickCommand = new RelayCommand<object>(OnRowDoubleClick);
        }

        private void OnRowDoubleClick(object parameter)
        {
            if (parameter != null)
            {
                ItemSelected?.Invoke(parameter);
            }
        }


    }
}
