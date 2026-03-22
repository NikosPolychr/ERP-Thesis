using Erp.CommonFiles;
using Erp.Model.BasicFiles;
using Erp.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Erp.Model.Thesis;
using Syncfusion.UI.Xaml.Grid;
using Erp.Model.Thesis.Filters;
using Erp.ViewSearch;
using Erp.Helper;
using Erp.CustomControls;

namespace Erp.ViewModelSearch.Thesis
{
    public class AirportSearchViewModel : ViewModelBaseSearch
    {

        #region DataProperties

        private ObservableCollection<AirportData> data;
        public ObservableCollection<AirportData> Data
        {
            get { return data; }
            set
            {
                data = value;
                INotifyPropertyChanged(nameof(Data));


            }
        }
        private AirportsFilterData filterdata;
        public AirportsFilterData FilterData
        {
            get { return filterdata; }
            set
            {
                filterdata = value;
                INotifyPropertyChanged(nameof(FilterData));


            }
        }
        private Columns sfGridColumns;
        public Columns SfGridColumns
        {
            get { return sfGridColumns; }
            set
            {
                this.sfGridColumns = value;
                INotifyPropertyChanged("SfGridColumns");
            }
        }

        #endregion

        #region Commands

        #region  Filter Commands

        #region CRUD Commands

        #region Clear

        private ViewModelCommand clearCommand;

        public ICommand ClearCommand
        {
            get
            {
                if (clearCommand == null)
                {
                    clearCommand = new ViewModelCommand(ExecuteClearCommand);
                }

                return clearCommand;
            }
        }

        private void ExecuteClearCommand(object commandParameter)
        {
            ResetAirportsViewmodelData();

        }
        public void ResetAirportsViewmodelData()
        {
            FilterData = new AirportsFilterData();
            FilterData.City = new CityData();
        }
        #endregion

        #region Search

        private ViewModelCommand searchCommand;

        public ICommand SearchCommand
        {
            get
            {
                if (searchCommand == null)
                {
                    searchCommand = new ViewModelCommand(ExecuteSearchCommand);
                }

                return searchCommand;
            }
        }

        private void ExecuteSearchCommand(object commandParameter)
        {
            Data = new ObservableCollection<AirportData>();
            Data = CommonFunctions.GetAirportsFilterData(FilterData.ShowDeleted,FilterData);
            // Automatically close the popup
            RaiseRequestClose(true);
        }

        #endregion


        #endregion

        #region F7 Commands

        public ICommand ShowCitiesGridCommand { get; }
        public ICommand ShowCountriesGridCommand { get; }
        public ICommand ShowPrefecturesGridCommand { get; }

        private void ExecuteShowPrefecturesGridCommand(object obj)
        {

            var f7Data = F7Common.F7City(ShowDeleted);
            f7Data.F7Title = "Select Prefecture";

            var popup = new F7PopupWindow(f7Data, ChangeCanExecute);
            bool? dialogResult = popup.ShowDialog();

        }
        private void ExecuteShowCitiesGridCommand(object obj)
        {

            var f7Data = F7Common.F7City(ShowDeleted);
            f7Data.F7Title = "Select City";

            var popup = new F7PopupWindow(f7Data, ChangeCanExecute);
            bool? dialogResult = popup.ShowDialog();

        }
        private void ExecuteShowCountriesGridCommand(object obj)
        {

            var f7Data = F7Common.F7Country(ShowDeleted);
            f7Data.F7Title = "Select Country";

            var popup = new F7PopupWindow(f7Data, ChangeCanExecute);
            bool? dialogResult = popup.ShowDialog();

        }
        public void ChangeCanExecute(object obj)
        {

            var selectedItemProperty = obj.GetType().GetProperty("SelectedItem");
            object selectedItem = selectedItemProperty?.GetValue(obj);

            if (selectedItem is AirportsFilterData airport)
            {
                FilterData = new AirportsFilterData();
                FilterData = airport;
            }
            if (selectedItem is CityData city)
            {
                FilterData.City = new CityData();
                FilterData.City = city;

            }
        }

        private ICommand rowDataCommand { get; set; }
        public ICommand RowDataCommand
        {
            get
            {
                return rowDataCommand;
            }
            set
            {
                rowDataCommand = value;
            }
        }

        protected void ClearColumns()
        {

            var ColumnsCount = this.SfGridColumns.Count();
            if (ColumnsCount != 0)
            {
                for (int i = 0; i < ColumnsCount; i++)
                {
                    this.sfGridColumns.RemoveAt(0);
                }
            }
        }
        #endregion

        #endregion

        #endregion


        public AirportSearchViewModel()
        {

            ResetAirportsViewmodelData();

            #region Commands Initialization

            ShowCitiesGridCommand = new RelayCommand2(ExecuteShowCitiesGridCommand);
            ShowCountriesGridCommand = new RelayCommand2(ExecuteShowCountriesGridCommand);
            ShowPrefecturesGridCommand = new RelayCommand2(ExecuteShowPrefecturesGridCommand);
            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            #endregion

            FilterData = new AirportsFilterData();
            Data = CommonFunctions.GetAirportsData(false);
        }

    }

}
