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
    public class WithWithoutViewModel : ViewModelBase
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


        private WithWithoutData flatData;
        public WithWithoutData FlatData
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
            FlatData = new WithWithoutData();
            FlatData.CrewCat1 = new CrewCategData();
            FlatData.CrewCat2 = new CrewCategData();

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
            int Flag = CommonFunctions.SaveWithWithoutData(FlatData);

            if (Flag == 1)
            {
                MessageBox.Show($"Saving/Updating Completed for the WithWithout with Code : {FlatData.Code}");
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
            FlatData = CommonFunctions.GetWithWithoutChooserData(FlatData.RuleId, FlatData.Code);
        }

        #endregion
        #region Add

        public ICommand AddWithWithoutDataCommand { get; }

        private void ExecuteAddWithWithoutDataCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(FlatData.Code) || string.IsNullOrWhiteSpace(FlatData.Descr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddWithWithoutData(FlatData);
                if (Flag == 0)

                {
                    MessageBox.Show($"A new WithWithout was saved with Code: {FlatData.Code}");

                    //ExecuteShowMinMaxGridCommand(obj);
                    FlatData.RuleId = 0;
                    ExecuteRefreshCommand(obj);

                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The WithWithout with Code : {FlatData.Code} already exists");

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

        public ICommand ShowWithWithoutGridCommand { get; set; }
        public ICommand ShowCrewCategGridCommand1 { get; }
        public ICommand ShowCrewCategGridCommand2 { get; }

        private void ExecuteShowWithWithoutGridCommand(object obj)
        {

            var f7Data = F7Common.F7WithWithout(ShowDeleted);
            //f7Data.F7Title = "Select MinMAX";

            var popup = new F7PopupWindow(f7Data, ChangeCanExecute);
            bool? dialogResult = popup.ShowDialog();
        }
        private void ExecuteShowCrewCategGridCommand1(object obj)
        {
            var f7Data = F7Common.F7CrewCat(ShowDeleted);
            //f7Data.F7Title = "Select CrewCateg";
            F7key = "C1";

            var popup = new F7PopupWindow(f7Data, ChangeCanExecute);
            bool? dialogResult = popup.ShowDialog();
        }
        private void ExecuteShowCrewCategGridCommand2(object obj)
        {
            var f7Data = F7Common.F7CrewCat(ShowDeleted);
            //f7Data.F7Title = "Select CrewCateg";

            F7key = "C2";
            var popup = new F7PopupWindow(f7Data, ChangeCanExecute);
            bool? dialogResult = popup.ShowDialog();
        }
        public void ChangeCanExecute(object obj)
        {

            var selectedItemProperty = obj.GetType().GetProperty("SelectedItem");
            object selectedItem = selectedItemProperty?.GetValue(obj);

            if (selectedItem is WithWithoutData withwithout)
            {
                FlatData = new WithWithoutData();
                FlatData = withwithout;
            }
            else if ( F7key == "C1" && selectedItem is CrewCategData crewCateg1)
            {
                FlatData.CrewCat1 = new CrewCategData();
                FlatData.CrewCat1 = crewCateg1;
            }
            else if(F7key == "C2" && selectedItem is CrewCategData crewCateg2)
            {
                FlatData.CrewCat2 = new CrewCategData();
                FlatData.CrewCat2 = crewCateg2;
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

        public WithWithoutViewModel()
        {
            #region Data Initialization

            ResetViewmodelData();
            this.sfGridColumns = new Columns();

            #endregion

            #region Commands Initialization

            ShowWithWithoutGridCommand = new RelayCommand2(ExecuteShowWithWithoutGridCommand);
            ShowCrewCategGridCommand1 = new RelayCommand2(ExecuteShowCrewCategGridCommand1);
            ShowCrewCategGridCommand2 = new RelayCommand2(ExecuteShowCrewCategGridCommand2);
            AddWithWithoutDataCommand = new RelayCommand2(ExecuteAddWithWithoutDataCommand);
            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            #endregion

        }

    }

}
