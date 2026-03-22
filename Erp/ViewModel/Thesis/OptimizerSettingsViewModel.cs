using Erp.Model.BasicFiles;
using Erp.Model.Thesis;
using Erp.Model.Thesis.CrewScheduling;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Erp.CustomControls;
using Erp.Helper;

namespace Erp.ViewModel.Thesis
{
    public class OptimizerSettingsViewModel : ViewModelBase
    {

        #region Data Properties

        private OptimizerSettingsData flatData;
        public OptimizerSettingsData FlatData
        {
            get { return flatData; }
            set
            {
                flatData = value;
                INotifyPropertyChanged(nameof(FlatData));
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
            FlatData = new OptimizerSettingsData();
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
            int Flag = CommonFunctions.SaveOptimizerSettingsData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Saving/Updating Completed for the Optimizer Settings with Code : {FlatData.Code}");
                ExecuteShowOptimizerSettingsGridCommand(obj);

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
            FlatData = CommonFunctions.GetOptimizerSettingsChooserData(FlatData.Id, FlatData.Code);
        }

        #endregion
        #region Add

        public ICommand AddDataCommand { get; }

        private void ExecuteAddDataCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.Code) || string.IsNullOrWhiteSpace(FlatData.Descr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddOptimizerSettingsData(FlatData);
                if (Flag == 0)

                {
                    MessageBox.Show($"New settings were saved with Code: {FlatData.Code}");

                    ExecuteShowOptimizerSettingsGridCommand(obj);
                    FlatData.Id = 0;
                    ExecuteRefreshCommand(obj);

                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Optimizer Settings with Code : {FlatData.Code} already exists");

                }
                else if (Flag == 2)
                {
                    MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        #endregion

        #endregion

        #region Data_Grid Commands

        public ICommand ShowOptimizerSettingsGridCommand { get; set; }
        private void ExecuteShowOptimizerSettingsGridCommand(object obj)
        {

            var f7Data = F7Common.F7OptimizerSettings(ShowDeleted);
            f7Data.F7Title = "Select Settings";

            var popup = new F7PopupWindow(f7Data, ChangeCanExecute);
            bool? dialogResult = popup.ShowDialog();
        }
        public void ChangeCanExecute(object obj)
        {

            var selectedItemProperty = obj.GetType().GetProperty("SelectedItem");
            object selectedItem = selectedItemProperty?.GetValue(obj);

            if (selectedItem is OptimizerSettingsData settings)
            {
                FlatData = new OptimizerSettingsData();
                FlatData = settings;
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

        public OptimizerSettingsViewModel()
        {
            #region Data Initialization

            ResetViewmodelData();
            this.sfGridColumns = new Columns();

            #endregion

            #region Commands Initialization

            ShowOptimizerSettingsGridCommand = new RelayCommand2(ExecuteShowOptimizerSettingsGridCommand);
            AddDataCommand = new RelayCommand2(ExecuteAddDataCommand);
            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            #endregion
        }
    }
}
