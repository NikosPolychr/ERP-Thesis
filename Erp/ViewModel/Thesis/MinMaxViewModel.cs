using Erp.Helper;
using Erp.Model.Thesis;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using Syncfusion.UI.Xaml.Grid;
using Erp.Model.BasicFiles;
using Erp.CustomControls;
using Erp.Model.Thesis.CrewScheduling;
using Erp.Model.Enums;
using System;

namespace Erp.ViewModel.Thesis
{
    public class MinMaxViewModel : ViewModelBase
    {
        #region DataProperties

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


        private MinMaxData flatData;
        public MinMaxData FlatData
        {
            get { return flatData; }
            set
            {
                flatData = value;
                INotifyPropertyChanged(nameof(FlatData));
            }
        }

        #region Enums 

        public BasicEnums.EmployeeType[] Positions
        {
            get { return (BasicEnums.EmployeeType[])Enum.GetValues(typeof(BasicEnums.EmployeeType)); }
        }

        public BasicEnums.RouteCategory[] RouteCategories
        {
            get { return (BasicEnums.RouteCategory[])Enum.GetValues(typeof(BasicEnums.RouteCategory)); }
        }

        #endregion
        #endregion

        #region Commands

        #region CRUD Commands

        #region 1st Tab
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
            ResetViewmodelData();

        }
        public void ResetViewmodelData()
        {
            FlatData = new MinMaxData();
            FlatData.CrewCat = new CrewCategData();
        }
        #endregion
        #region Save


        private ViewModelCommand savecommand;

        public ICommand SaveCommand
        {
            get
            {
                if (savecommand == null)
                {
                    savecommand = new ViewModelCommand(ExecuteSaveCommand);
                }

                return savecommand;
            }
        }

        private void ExecuteSaveCommand(object obj)
        {
            int Flag = CommonFunctions.SaveMinMaxData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Saving/Updating Completed for the Airport with Code : {FlatData.Code}");
                //ExecuteShowMinMaxGridCommand(obj);

            }
            else if (Flag == -1)
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
        #region Refresh

        private ViewModelCommand refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new ViewModelCommand(ExecuteRefreshCommand);
                }

                return refreshCommand;
            }
        }

        private void ExecuteRefreshCommand(object commandParameter)
        {
            FlatData = CommonFunctions.GetMinMaxChooserData(FlatData.RuleId, FlatData.Code);
        }

        #endregion
        #region Add

        public ICommand AddMinMaxDataCommand { get; }

        private void ExecuteAddMinMaxDataCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.Code) || string.IsNullOrWhiteSpace(FlatData.Descr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddMinMaxData(FlatData);
                if (Flag == 0)

                {
                    MessageBox.Show($"A new MinMax was saved with Code: {FlatData.Code}");

                    //ExecuteShowMinMaxGridCommand(obj);
                    FlatData.RuleId = 0;
                    ExecuteRefreshCommand(obj);

                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The MinMax with Code : {FlatData.Code} already exists");

                }
                else if (Flag == 2)
                {
                    MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        #endregion

        #endregion

        #endregion

        #region Data_Grid Commands

        public ICommand ShowMinMaxGridCommand { get; set; }
        public ICommand ShowCrewCategGridCommand { get; }
        private void ExecuteShowMinMaxGridCommand(object obj)
        {

            var f7Data = F7Common.F7MinMax(ShowDeleted);
            //f7Data.F7Title = "Select MinMAX";

            var popup = new F7PopupWindow(f7Data, ChangeCanExecute);
            bool? dialogResult = popup.ShowDialog();
        }
        private void ExecuteShowCrewCategGridCommand(object obj)
        {

            var f7Data = F7Common.F7CrewCat(ShowDeleted);
            //f7Data.F7Title = "Select CrewCateg";

            var popup = new F7PopupWindow(f7Data, ChangeCanExecute);
            bool? dialogResult = popup.ShowDialog();

        }
        public void ChangeCanExecute(object obj)
        {

            var selectedItemProperty = obj.GetType().GetProperty("SelectedItem");
            object selectedItem = selectedItemProperty?.GetValue(obj);

            if (selectedItem is MinMaxData minmax)
            {
                FlatData = new MinMaxData();
                FlatData = minmax;
            }
            if (selectedItem is CrewCategData crewCateg)
            {
                FlatData.CrewCat = new CrewCategData();
                FlatData.CrewCat = crewCateg;
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

        public MinMaxViewModel()
        {
            #region Data Initialization

            ResetViewmodelData();
            this.sfGridColumns = new Columns();

            #endregion

            #region Commands Initialization

            ShowMinMaxGridCommand = new RelayCommand2(ExecuteShowMinMaxGridCommand);
            ShowCrewCategGridCommand = new RelayCommand2(ExecuteShowCrewCategGridCommand);
            AddMinMaxDataCommand = new RelayCommand2(ExecuteAddMinMaxDataCommand);
            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            #endregion

        }

    }
}
