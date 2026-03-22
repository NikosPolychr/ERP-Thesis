using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using Syncfusion.Data.Extensions;
using Erp.Model.Enums;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Syncfusion.UI.Xaml.Grid;
using Erp.Helper;
using Erp.Model.Thesis.CrewScheduling;
using Erp.Model.Thesis;
using OxyPlot;
using Erp.CustomControls;
using System.Runtime.InteropServices.ComTypes;
using Erp.CrewScheduling;
using ControlzEx.Standard;
using System.IO;
using System.Threading.Tasks;
using static Erp.Model.Enums.BasicEnums;
using static ILOG.CPLEX.Cplex;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.FSharp.Collections;
using System.Data;
using Accord.Math;


namespace Erp.ViewModel.Thesis
{
    public class CrewSchedulingViewModel : ViewModelBase
    {
        #region DataProperties

        private int _selectedTabIndex;

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    INotifyPropertyChanged(nameof(SelectedTabIndex));
                }
            }
        }
        private ICollectionView collectionviewD;

        public ICollectionView CollectionViewD
        {
            get
            {
                return collectionviewD;
            }
            set
            {
                collectionviewD = value;
                INotifyPropertyChanged("CollectionViewD");
            }
        }

        private CSInputData inputdata;
        public CSInputData InputData
        {
            get { return inputdata; }
            set
            {
                inputdata = value;
                INotifyPropertyChanged(nameof(InputData));


            }
        }
        private CSOutputData _OutputData;
        public CSOutputData OutputData
        {
            get { return _OutputData; }
            set
            {
                _OutputData = value;
                INotifyPropertyChanged(nameof(OutputData));


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

        #region Enums

        public BasicEnums.EmployeeType[] EmployeeTypes
        {
            get { return (BasicEnums.EmployeeType[])Enum.GetValues(typeof(BasicEnums.EmployeeType)); }
        }

        public BasicEnums.CSType[] CSTypes
        {
            get { return (BasicEnums.CSType[])Enum.GetValues(typeof(BasicEnums.CSType)); }
        }
        #endregion

        #endregion

        #region Commands

        #region Data_Grid Commands
        public ICommand ShowCrewSchedulingGridCommand { get; }
        public ICommand ShowEmployeesGridCommand { get; }
        public ICommand ShowFlightRoutestGridCommand { get; }

        public ICommand ShowOptimizerSettingsGridCommand { get; set; }
        private void ExecuteShowOptimizerSettingsGridCommand(object obj)
        {

            var f7Data = F7Common.F7OptimizerSettings(ShowDeleted);
            f7Data.F7Title = "Select Optimizer Settings";

            var popup = new F7PopupWindow(f7Data, ChangeCanExecute);
            bool? dialogResult = popup.ShowDialog();
            F7key = "OptSettings";
        }

        public void ExecuteShowCrewSchedulingGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7CrewScheduling(ShowDeleted);
            F7key = F7input.F7key;
            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        public void ExecuteShowFlightRoutestGridCommand(object obj)
        {

            ClearColumns();

            var F7input = F7Common.F7CSFlightRoutes(InputData);
            F7key = F7input.F7key;
            var Data = CommonFunctions.GetCSFlightRoutesData(false, InputData);

            CollectionView = CollectionViewSource.GetDefaultView(Data);
            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }
        public void ExecuteShowEmployeesGridCommand(object obj)
        {
            ClearColumns();

            var F7input = F7Common.F7CSEmployee(InputData);
            F7key = F7input.F7key;

            var Data = CommonFunctions.GetEmployeesByTypeData(InputData.Position, false);
            foreach(var row in Data)
            {
                row.IsSelected = true;
            }


            CollectionView = CollectionViewSource.GetDefaultView(Data);

            var a = F7input.SfGridColumns;
            foreach (var item in a)
            {
                this.sfGridColumns.Add(item);
            }

        }

        public void ChangeCanExecute(object obj)
        {
            var selectedItemProperty = obj.GetType().GetProperty("SelectedItem");
            object selectedItem = selectedItemProperty?.GetValue(obj);

            if (F7key == "CSCODE")
            {
                InputData = (SelectedItem as CSInputData);
            }
            else if (selectedItem is OptimizerSettingsData optsettings)
            {
                InputData.OptimizerSettingsData = new OptimizerSettingsData();
                InputData.OptimizerSettingsData = optsettings;
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

        #region CRUD  Commands

        #region Input 
        #region ADD
        private ViewModelCommand _AddCommand;

        public ICommand AddCommand
        {
            get
            {
                if (_AddCommand == null)
                {
                    _AddCommand = new ViewModelCommand(ExecuteAddCommand);
                }

                return _AddCommand;
            }
        }

        private void ExecuteAddCommand(object obj)
        {
            if (string.IsNullOrWhiteSpace(InputData.Code) || string.IsNullOrWhiteSpace(InputData.Descr))
            {
                MessageBox.Show("Insert Code and Description");
            }

            else
            {
                int Flag = CommonFunctions.AddCSInputData(InputData);
                if (Flag == 0)
                {
                    MessageBox.Show($"A New Crew Scheduling saved with Code: {InputData.Code}");
                    ExecuteShowCrewSchedulingGridCommand(obj);
                    InputData.Id = 0;
                    ExecuteRefreshInputCommand(obj);
                }
                else if (Flag == 1)
                {
                    MessageBox.Show($"The Crew Scheduling with Code : {InputData.Code} already exists");

                }
                else if (Flag == 2)
                {
                    MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        #endregion

        #region Clear

        private ViewModelCommand _ClearInputCommand;

        public ICommand ClearInputCommand
        {
            get
            {
                if (_ClearInputCommand == null)
                {
                    _ClearInputCommand = new ViewModelCommand(ExecuteClearInputCommand);
                }

                return _ClearInputCommand;
            }
        }

        private void ExecuteClearInputCommand(object commandParameter)
        {

            InputData = new CSInputData();
        }

        #endregion

        #region Save


        private ViewModelCommand _SaveInputCommand;

        public ICommand SaveInputCommand
        {
            get
            {
                if (_SaveInputCommand == null)
                {
                    _SaveInputCommand = new ViewModelCommand(ExecuteSaveInputCommand);
                }

                return _SaveInputCommand;
            }
        }

        private void ExecuteSaveInputCommand(object obj)
        {
            int Flag = CommonFunctions.SaveCSInputData(InputData);


            if (Flag == 1)
            {
                MessageBox.Show($"Save/Update Completed for Crew Scheduling with Code : {InputData.Code}");
                ExecuteShowCrewSchedulingGridCommand(obj);
                ExecuteRefreshInputCommand(obj);
            }
            else if (Flag == -1)
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        #endregion

        #region Refresh

        private ViewModelCommand _RefreshInputCommand;

        public ICommand RefreshInputCommand
        {
            get
            {
                if (_RefreshInputCommand == null)
                {
                    _RefreshInputCommand = new ViewModelCommand(ExecuteRefreshInputCommand);
                }

                return _RefreshInputCommand;
            }
        }

        private void ExecuteRefreshInputCommand(object commandParameter)
        {


            InputData = CommonFunctions.GetCSInputChooserData(InputData.Id, InputData.Code,InputData);
        }

        #endregion
        #endregion

        #region Output 



        #endregion

        #endregion

        #region Crew Scheduling Optimization

        #region Gurobi
        public ICommand CalculateCS_GB { get; }
        private void ExecuteCalculateCS_Gurobi(object obj)
        {
            //Na diksw autes tis Sunarthseis
            InputData.Employees = CommonFunctions.GetEmployeesByTypeData(InputData.Position, false);
            InputData.FlightRoutesData = CommonFunctions.GetCSFlightRoutesData(false, InputData);

            #region Dictionaires,Indexes Initialization

            InputData.T = new int(); // DATES
            InputData.I = new int(); // EMPLOYEES
            InputData.F = new int(); // ROUTES

            InputData.DatesIndexMap = new Dictionary<int, DateTime>();
            InputData.EmployeesIndexMap = new Dictionary<int, string>();
            InputData.RoutesIndexMap = new Dictionary<int, string>();
            InputData.RoutesCompl_Dict = new Dictionary<int, int>();

            InputData.RoutesDates_Dict = new Dictionary<int, (DateTime, DateTime)>();
            InputData.RoutesDay_Dict = new Dictionary<int, (int, int)>();
            InputData.RoutesTime_Dict = new Dictionary<int, (int, int)>();
            InputData.EmpBounds_Dict = new Dictionary<int, (double, double)>();

            InputData.Ri = new Dictionary<int, List<int>>(); 
            InputData.Cij_Hours = new Dictionary<(int, int), double>(); 
            InputData.Aijf = new Dictionary<(int, int, int), int>();
            #endregion

            #region Populate Dictionaries,Indexes

           

            InputData.F = InputData.FlightRoutesData.Count;
            InputData.I = InputData.Employees.Count;

            int EmployeeCounter = 0;
            foreach (var emp in InputData.Employees)
            {
                InputData.EmployeesIndexMap.Add(EmployeeCounter, emp.Code);
                InputData.EmpBounds_Dict.Add(EmployeeCounter, (emp.EmpCrSettings.LowerBound, emp.EmpCrSettings.UpperBound));

                #region Cij,Ri,Aijf Initialization 
                //Create a List with the Empty Roster
                List<int> RostersForEmployee_List = new List<int>();
                RostersForEmployee_List.Add(0);

                //Insert into Ri
                InputData.Ri.Add(EmployeeCounter, RostersForEmployee_List);

                //Add the Roster to Cij with Cost = 0
                InputData.Cij_Hours.Add((EmployeeCounter, 0), 0);
                #endregion
                EmployeeCounter++;

            }
            int RouteCounter = 0;
            foreach (var Route in InputData.FlightRoutesData)
            {
                #region RoutesDates, RoutesDay, RouteTime 

                int StartDayIndex = InputData.DatesIndexMap.FirstOrDefault(x => x.Value.Date.Date == Route.StartDate.Date).Key;
                int EndDayIndex = InputData.DatesIndexMap.FirstOrDefault(x => x.Value.Date.Date == Route.EndDate.Date).Key;
                int StartTime = Route.StartDate.Minute >= 30 ? Route.StartDate.Hour + 1 : Route.StartDate.Hour;
                int EndTime = Route.EndDate.Minute >= 30 ? Route.EndDate.Hour + 1 : Route.EndDate.Hour;

                //RoutesDay
                InputData.RoutesDay_Dict.Add(RouteCounter, (StartDayIndex, EndDayIndex));
                //RoutesTime
                InputData.RoutesTime_Dict.Add(RouteCounter, (StartTime, EndTime));
                //RoutesDate
                InputData.RoutesDates_Dict.Add(RouteCounter, (Route.StartDate, Route.EndDate));

                #endregion

                #region RoutesCompl

                #region Retrieve Complement based on Position/EmployeeType
                int Complement = 0;
                if (InputData.Position == BasicEnums.EmployeeType.Captain)
                {
                    Complement = Route.Complement_Captain;
                }
                if (InputData.Position == BasicEnums.EmployeeType.FO)
                {
                    Complement = Route.Complement_FO;
                }
                if (InputData.Position == BasicEnums.EmployeeType.Cabin_Manager)
                {
                    Complement = Route.Complement_Cabin_Manager;
                }
                if (InputData.Position == BasicEnums.EmployeeType.Flight_Attendant)
                {
                    Complement = Route.Complement_Flight_Attendant;
                }
                #endregion

                //Insert Complement
                InputData.RoutesCompl_Dict.Add(RouteCounter, Complement);

                #endregion

                for(int i = 0; i<InputData.I; i++)
                {
                    InputData.Aijf[(i, 0, RouteCounter)] = 0;

                }

                RouteCounter++;
            }
            #endregion

            #region Optimize 
            OutputData = new CSOutputData();

            
            if(InputData.CSType == BasicEnums.CSType.Set_Partition)
            {
                //Set-Partition
                OutputData = CommonFunctions.CalculateCrewScheduling_SetPartition_GB(InputData);
            }
            else if (InputData.CSType == BasicEnums.CSType.Set_Covering)
            {
                //Set Cover
                OutputData = CommonFunctions.CalculateCrewScheduling_SetCover_GB(InputData);
            }


            #endregion

            #region Print Messages To Screen

            if (OutputData != null)
            {
                MessageBox.Show($"Crew Scheduling Succeeded");
                SelectedTabIndex = 1;
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            #endregion

            #region Extra

            InputData.T = (int)Math.Ceiling((InputData.DateTo - InputData.DateFrom).TotalDays);

            int dateCounter = 0;
            for (var date = InputData.DateFrom; date <= InputData.DateTo; date = date.AddDays(1))
            {
                InputData.DatesIndexMap.Add(dateCounter, date.Date);
                dateCounter++;
            }

            #endregion

        }
        #endregion

        #region CPLEX
        public ICommand CalculateCS_CPLEX { get; }

        private void ExecuteCalculateCS_CPLEX(object obj)
        {
            //Na diksw autes tis Sunarthseis
            InputData.Employees = CommonFunctions.GetEmployeesByTypeData(InputData.Position, false);
            InputData.FlightRoutesData = CommonFunctions.GetCSFlightRoutesData(false, InputData);

            #region Dictionaires,Indexes Initialization

            InputData.T = new int(); // DATES
            InputData.I = new int(); // EMPLOYEES
            InputData.F = new int(); // ROUTES

            InputData.DatesIndexMap = new Dictionary<int, DateTime>();
            InputData.EmployeesIndexMap = new Dictionary<int, string>();
            InputData.RoutesIndexMap = new Dictionary<int, string>();
            InputData.RoutesCompl_Dict = new Dictionary<int, int>();

            InputData.RoutesDates_Dict = new Dictionary<int, (DateTime, DateTime)>();
            InputData.RoutesDay_Dict = new Dictionary<int, (int, int)>();
            InputData.RoutesTime_Dict = new Dictionary<int, (int, int)>();
            InputData.EmpBounds_Dict = new Dictionary<int, (double, double)>();

            InputData.Ri = new Dictionary<int, List<int>>();
            InputData.Cij_Hours = new Dictionary<(int, int), double>();
            InputData.Aijf = new Dictionary<(int, int, int), int>();
            #endregion

            #region Populate Dictionaries,Indexes



            InputData.F = InputData.FlightRoutesData.Count;
            InputData.I = InputData.Employees.Count;

            int EmployeeCounter = 0;
            foreach (var emp in InputData.Employees)
            {
                InputData.EmployeesIndexMap.Add(EmployeeCounter, emp.Code);
                InputData.EmpBounds_Dict.Add(EmployeeCounter, (emp.EmpCrSettings.LowerBound, emp.EmpCrSettings.UpperBound));

                #region Cij,Ri,Aijf Initialization 
                //Create a List with the Empty Roster
                List<int> RostersForEmployee_List = new List<int>();
                RostersForEmployee_List.Add(0);

                //Insert into Ri
                InputData.Ri.Add(EmployeeCounter, RostersForEmployee_List);

                //Add the Roster to Cij with Cost = 0
                InputData.Cij_Hours.Add((EmployeeCounter, 0), 0);
                #endregion
                EmployeeCounter++;

            }
            int RouteCounter = 0;
            foreach (var Route in InputData.FlightRoutesData)
            {
                #region RoutesDates, RoutesDay, RouteTime 

                int StartDayIndex = InputData.DatesIndexMap.FirstOrDefault(x => x.Value.Date.Date == Route.StartDate.Date).Key;
                int EndDayIndex = InputData.DatesIndexMap.FirstOrDefault(x => x.Value.Date.Date == Route.EndDate.Date).Key;
                int StartTime = Route.StartDate.Minute >= 30 ? Route.StartDate.Hour + 1 : Route.StartDate.Hour;
                int EndTime = Route.EndDate.Minute >= 30 ? Route.EndDate.Hour + 1 : Route.EndDate.Hour;

                //RoutesDay
                InputData.RoutesDay_Dict.Add(RouteCounter, (StartDayIndex, EndDayIndex));
                //RoutesTime
                InputData.RoutesTime_Dict.Add(RouteCounter, (StartTime, EndTime));
                //RoutesDate
                InputData.RoutesDates_Dict.Add(RouteCounter, (Route.StartDate, Route.EndDate));

                #endregion

                #region RoutesCompl

                #region Retrieve Complement based on Position/EmployeeType
                int Complement = 0;
                if (InputData.Position == BasicEnums.EmployeeType.Captain)
                {
                    Complement = Route.Complement_Captain;
                }
                if (InputData.Position == BasicEnums.EmployeeType.FO)
                {
                    Complement = Route.Complement_FO;
                }
                if (InputData.Position == BasicEnums.EmployeeType.Cabin_Manager)
                {
                    Complement = Route.Complement_Cabin_Manager;
                }
                if (InputData.Position == BasicEnums.EmployeeType.Flight_Attendant)
                {
                    Complement = Route.Complement_Flight_Attendant;
                }
                #endregion

                //Insert Complement
                InputData.RoutesCompl_Dict.Add(RouteCounter, Complement);

                #endregion

                for (int i = 0; i < InputData.I; i++)
                {
                    InputData.Aijf[(i, 0, RouteCounter)] = 0;
                }

                RouteCounter++;
            }
            #endregion

            #region Optimize 
            OutputData = new CSOutputData();


            if (InputData.CSType == BasicEnums.CSType.Set_Partition)
            {
                //Set-Partition
                OutputData = CplexFunctions.CalculateCrewScheduling_SetPartition_CPLEX(InputData);
            }
            else if (InputData.CSType == BasicEnums.CSType.Set_Covering)
            {
                //Set Cover
                OutputData = CplexFunctions.CalculateCrewScheduling_SetCover_CPLEX(InputData);
            }


            #endregion

            #region Print Messages To Screen

            if (OutputData != null)
            {
                MessageBox.Show($"{InputData.CSType} completed for crew scheduling with code : {InputData.Code}", "", MessageBoxButton.OK, MessageBoxImage.Information);
                SelectedTabIndex = 1;
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            #endregion

            #region Extra

            InputData.T = (int)Math.Ceiling((InputData.DateTo - InputData.DateFrom).TotalDays);

            int dateCounter = 0;
            for (var date = InputData.DateFrom; date <= InputData.DateTo; date = date.AddDays(1))
            {
                InputData.DatesIndexMap.Add(dateCounter, date.Date);
                dateCounter++;
            }

            #endregion

        }

        private void ExecuteCalculateCS_CPLEX1(object obj)
        {
            //Na diksw autes tis Sunarthseis
            InputData.Employees = CommonFunctions.GetEmployeesByTypeData(InputData.Position, false);
            InputData.FlightRoutesData = CommonFunctions.GetCSFlightRoutesData(false, InputData);

            if (InputData.OptimizerSettingsData.SpecialConsParams.MinMaxAct)
            {
                InputData.MinMaxList = CommonFunctions.GetMinMaxData(false).ToList();
            }


            #region Optimize 
            OutputData = new CSOutputData();


            if (InputData.CSType == BasicEnums.CSType.Set_Partition)
            {
                //Set-Partition

                OutputData = SchedulingFunctions.MainFunction(InputData);
            }
            else if (InputData.CSType == BasicEnums.CSType.Set_Covering)
            {
                //Set Cover
                OutputData = CplexFunctions.CalculateCrewScheduling_SetCover_CPLEX(InputData);
            }


            #endregion

            #region Print Messages To Screen

            if (OutputData != null)
            {
                MessageBox.Show($"{InputData.CSType} completed for crew scheduling with code : {InputData.Code}", "", MessageBoxButton.OK, MessageBoxImage.Information);
                SelectedTabIndex = 1;
            }
            else
            {
                MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            #endregion

            #region Extra

            InputData.T = (int)Math.Ceiling((InputData.DateTo - InputData.DateFrom).TotalDays);

            int dateCounter = 0;
            for (var date = InputData.DateFrom; date <= InputData.DateTo; date = date.AddDays(1))
            {
                InputData.DatesIndexMap.Add(dateCounter, date.Date);
                dateCounter++;
            }

            #endregion

        }

        #endregion

        #region C++ Cplex
        public ICommand CalculateCS_Cplusplus { get; }

        private void ExecuteCalculateCS_Cplusplus(object obj)
        {
            #region Retrieve Data from Database

            if (InputData.OptimizerSettingsData.SpecialConsParams.MinMaxAct)
            {
                InputData.MinMaxList = CommonFunctions.GetMinMaxData(false).ToList();
            }
            if (InputData.OptimizerSettingsData.SpecialConsParams.WithAct || InputData.OptimizerSettingsData.SpecialConsParams.WithoutAct)
            {
                InputData.WithWithoutList = CommonFunctions.GetWithWithoutData(false).ToList();
            }
            InputData.Employees = CommonFunctions.GetEmployeesByTypeData(InputData.Position, false);
            InputData.FlightRoutesData = CommonFunctions.GetCSFlightRoutesData(false, InputData);

            #endregion


            // ONE base path - calculated automatically from solution
            string solutionDir = Path.GetFullPath(Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"..\..\..\..\"));

            // All paths derived from solutionDir
            string cppProjectDir = Path.Combine(solutionDir, @"Colgen c++ Nikos\ColgenNikosC++\ColgenNikosC++");
            string settingsPath_MainSettings = Path.Combine(cppProjectDir, "MainSettings.h");
            string ExperimentFolderPath = Path.Combine(cppProjectDir, "Experiments");




            string MainSettingsH = Generate_MainSettingsH(settingsPath_MainSettings);
            Generate_CrewRouteData(ExperimentFolderPath, MainSettingsH);

            string exeDebugPath = Path.Combine(solutionDir, @"ERP-Thesis\x64\Debug\ColgenNikosC++.exe");
            string exeReleasePath = Path.Combine(solutionDir, @"ERP-Thesis\x64\Release\ColgenNikosC++.exe");
            //ExecuteRunCplusplus(exeDebugPath, exeReleasePath);
        }
        private string Generate_MainSettingsH(string settingsPath)
        {
            try
            {
                var MainSettingsH = new System.Text.StringBuilder();
                var Params = InputData.OptimizerSettingsData;

                int Line = 2;
                if (InputData.CSType == BasicEnums.CSType.Set_Partition)
                    Line = 2;
                else if (InputData.CSType == BasicEnums.CSType.Set_Covering)
                    Line = 1;
                
                int N = InputData.Employees.Count;
                int R = InputData.FlightRoutesData.Count;
                int Days = (int)Math.Ceiling((InputData.DateTo - InputData.DateFrom).TotalDays);

                bool RandomData = false;
                int MinRouteDuration = 5;
                int MaxRouteDuration = 30;
                int MaxFlightHoursCM = 90;
                int WindowRangeCM = 3;
                int CMBoundDiffAver = 30;
                int GenderConPercent = 50;

                #region TxtFile

                MainSettingsH.AppendLine("//------|| Program's Parameters");
                MainSettingsH.AppendLine();

                MainSettingsH.AppendLine("//---| Select Line");
                MainSettingsH.AppendLine($"#define  Line    {Line}   // Select your preferred Line (design). (Line 1 for Set Cover), (Line 2 for Set Partition), (Line 3 for Hybrid Set Cover - Set Partition)");
                MainSettingsH.AppendLine();

                MainSettingsH.AppendLine("//---| Instance Parameters");
                MainSettingsH.AppendLine($"#define  N       {N}   // Number of crew members of the same rank");
                MainSettingsH.AppendLine($"#define  R       {R}   // Number of flight routes to be covered in the planning horizon");
                MainSettingsH.AppendLine($"#define  Days    {Days}   // Number of days in planning horizon");
                MainSettingsH.AppendLine();

                MainSettingsH.AppendLine("//---| Data Generation Parameters");
                MainSettingsH.AppendLine($"#define  RandomData          {RandomData.ToString().ToLower()}   // False => data will be the same for every run, True => generates random seed");
                MainSettingsH.AppendLine($"#define  MinRouteDuration    {MinRouteDuration}   // Minimum number of flight hours of a route");
                MainSettingsH.AppendLine($"#define  MaxRouteDuration    {MaxRouteDuration}   // Maximum number of flight hours of a route");
                MainSettingsH.AppendLine($"#define  MaxFlightHoursCM    {MaxFlightHoursCM}   // Maximum number of flight hours in the planning horizon that a crew member could take without a penalty");
                MainSettingsH.AppendLine($"#define  WindowRangeCM       {WindowRangeCM}   // The range of the flight hour window of a crew member");
                MainSettingsH.AppendLine($"#define  CMBoundDiffAver     {CMBoundDiffAver}   // By subtracting/adding this parameter from/to the average flight hours per crew member we get a lower/upper bound");
                MainSettingsH.AppendLine($"#define  GenderConPercent    {GenderConPercent}   // % of eligible routes that are subject to gender constraints (0-100)");
                MainSettingsH.AppendLine();

                MainSettingsH.AppendLine("//---| Roster Legality Rules' Parameters");
                MainSettingsH.AppendLine($"#define  DaysOff               {Params.LegalityRulesParams.DaysOff}   // Minimum days in the planning horizon (month) that a crew member is resting (does not have duty)");
                MainSettingsH.AppendLine($"#define  MinHoursBetween       {Params.LegalityRulesParams.MinHoursBetween}   // Minimum hours of break between consecutive duties in order to be considered a day off");
                MainSettingsH.AppendLine($"#define  LatestArrivalTime     {Params.LegalityRulesParams.LatestArrivalTime}   // Latest arrival time of the first of two consecutive duties [ 23 : 00 ]");
                MainSettingsH.AppendLine($"#define  EarliestDepartureTime {Params.LegalityRulesParams.EarliestDepartureTime}   // Earliest departure time of the second of two consecutive duties [ 06 : 00 ]");
                MainSettingsH.AppendLine($"#define  XHoursBreak           {Params.LegalityRulesParams.XHoursBreak}   // Rule XHoursBreak / YFlightHours");
                MainSettingsH.AppendLine($"#define  YFlightHours          {Params.LegalityRulesParams.YFlightHours}   // Rule XHoursBreak / YFlightHours demands that a crew member has X hours of break without a flight in between");
                MainSettingsH.AppendLine($"#define  MaximumFlightHours    {Params.LegalityRulesParams.MaximumFlightHours}   // Maximum sum flight hours that a crew member is allowed to fly in the planning horizon");
                MainSettingsH.AppendLine();

                MainSettingsH.AppendLine("//---| Coverage and Balance Cost Parameters");
                MainSettingsH.AppendLine($"#define  UndercoverCost        {Params.PenaltiesParams.UnderCoverPenalty}   // Cost for not covering a flight leg");
                MainSettingsH.AppendLine($"#define  HourPenalty           {Params.PenaltiesParams.HourPenalty}   // Cost per hour violation of the time window of a crew member");
                MainSettingsH.AppendLine($"#define  GenderPenalty         {Params.PenaltiesParams.GenderPenalty}   // Cost per unsatisfied gender constraint");
                MainSettingsH.AppendLine($"#define  SoftGenderConstraints {false.ToString().ToLower()}");
                MainSettingsH.AppendLine();

                MainSettingsH.AppendLine("//---| Numeric Parameters");
                MainSettingsH.AppendLine($"#define  MaxDouble  {Params.NumericParams.MaxDouble}   // A very large double number");
                MainSettingsH.AppendLine($"#define  eps        {Params.NumericParams.Eps}   // A very small number");
                MainSettingsH.AppendLine();

                MainSettingsH.AppendLine("//---| Column Generation Performance Parameters");
                MainSettingsH.AppendLine($"#define  buckets                    {Params.ColgenParams.Buckets}   // Number of buckets of each Route node");
                MainSettingsH.AppendLine($"#define  fictbuckets                {Params.ColgenParams.Fictbuckets}   // Number of buckets of Fictitious node [Define this between 10 and 50]");
                MainSettingsH.AppendLine($"#define  ReducedCostCut             {Params.ColgenParams.ReducedCostCut}   // If the Reduced Cost of a candidate roster is less than the ReducedCostCut parameter then it is added to Master");
                MainSettingsH.AppendLine($"#define  NIDRAI                     {Params.ColgenParams.NIDRAI}   // Percentage of the total IDs for whom at least one roster was added to Master in a single column generation iteration");
                MainSettingsH.AppendLine($"#define  UpdateDualsManuallyScheme  {false.ToString().ToLower()}   // set to 1 for manual update, 0 for CPLEX update");
                MainSettingsH.AppendLine();

                MainSettingsH.AppendLine("//---| Branching Parameters");
                MainSettingsH.AppendLine($"#define  AbsBacktrack               {Params.BranchingParams.AbsBacktrack}   // Minimum absolute difference for backtrack");
                MainSettingsH.AppendLine($"#define  PerceBacktrack             {Params.BranchingParams.PerceBacktrack}   // Minimum percentage difference for backtrack");
                MainSettingsH.AppendLine($"#define  AbsMIP                     {Params.BranchingParams.AbsMIP}   // Minimum absolute difference for MIP termination");
                MainSettingsH.AppendLine($"#define  PerceMIP                   {Params.BranchingParams.PerceMIP}   // Minimum percentage difference for MIP termination");
                MainSettingsH.AppendLine($"#define  NumberOfBacktracksLimit    {Params.BranchingParams.NumberOfBacktracksLimit}   // Maximum number of backtracks allowed");
                MainSettingsH.AppendLine($"#define  NumberOfTreeNodesLimit     {Params.BranchingParams.NumberOfTreeNodesLimit * N}   // Maximum number of TreeNodes created allowed");
                MainSettingsH.AppendLine($"#define  TreelistSortingIdsFixed    {Params.BranchingParams.TreelistSortingIdsFixed}   // Minimum absolute difference between MasterObjVals of two TreeNodes");
                MainSettingsH.AppendLine();

                MainSettingsH.AppendLine("//---| Activation Parameters");
                MainSettingsH.AppendLine($"#define  ActivateMinMax   {Params.SpecialConsParams.MinMaxAct.ToString().ToLower()}");
                MainSettingsH.AppendLine($"#define  ActivateWith     {Params.SpecialConsParams.WithAct.ToString().ToLower()}");
                MainSettingsH.AppendLine($"#define  ActivateWithout  {Params.SpecialConsParams.WithoutAct.ToString().ToLower()}");
                MainSettingsH.AppendLine($"#define  ActivateGender   {Params.SpecialConsParams.GenderAct.ToString().ToLower()}");
                MainSettingsH.AppendLine();

                MainSettingsH.AppendLine("//---| Priority Parameters");
                MainSettingsH.AppendLine($"#define  WithoutVarPriority {Params.SpecialConsParams.WithoutPriority.ToString().ToLower()}   // Without auxiliary variables get strict priority when branching");
                MainSettingsH.AppendLine($"#define  GenderVarPriority  {Params.SpecialConsParams.GenderPriority.ToString().ToLower()}   // Gender auxiliary variables get strict priority when branching");
                MainSettingsH.AppendLine();

                #endregion

                File.WriteAllText(settingsPath, MainSettingsH.ToString());
                return MainSettingsH.ToString(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Settings: {ex.Message}");
                return null;
            }
        }
        private void Generate_CrewRouteData(string ExperimentFolderPath, string MainSettingsh)
        {
            try
            {
                int N = InputData.Employees.Count;
                int R = InputData.FlightRoutesData.Count;

                #region Experiment Folder
                // ---| Get Next Test ID from Log |---
                string logFilePath = Path.Combine(ExperimentFolderPath, "Experiments_Log.txt");
                int nextTestId = 1;

                if (File.Exists(logFilePath))
                {
                    var existingLines = File.ReadAllLines(logFilePath)
                        .Where(l => l.StartsWith("test", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (existingLines.Any())
                    {
                        // Find the last Test number
                        var lastTest = existingLines
                            .Select(l => l.Split('|')[0].Trim())
                            .Where(l => l.StartsWith("test", StringComparison.OrdinalIgnoreCase))
                            .Select(l => int.TryParse(l.Replace("test", ""), out int num) ? num : 0) // ← 2 arguments
                            .Max();
                        nextTestId = lastTest + 1;
                    }
                }

                string ExperFolderName = $"test{nextTestId}";
                string ExperimentName = ExperFolderName; // ← πρόσθεσε αυτό

                // ---| Create Folder |---
                string ExperFolderPath = Path.Combine(ExperimentFolderPath, ExperFolderName);
                if (!Directory.Exists(ExperFolderPath))
                    Directory.CreateDirectory(ExperFolderPath);

                // ---| Save MainSettings.h inside Experiment Folder |---
                string mainSettingsInExperFolder = Path.Combine(ExperFolderPath, "MainSettings.h");
                File.WriteAllText(mainSettingsInExperFolder, MainSettingsh.ToString());
                #endregion

                #region Experiment Log
                // ---| Create log file with header only if not exists |---
                if (!File.Exists(logFilePath))
                {
                    var header = "TEST_ID".PadRight(20) + "| N".PadRight(6) + "| R".PadRight(6) +
                                 "| MinMax".PadRight(10) + "| With".PadRight(8) + "| Without".PadRight(10) +
                                 "| Gender".PadRight(10) + "| Without Priority".PadRight(16) + "| Gender Priority";
                    var separator = new string('-', header.Length);
                    File.WriteAllText(logFilePath, header + Environment.NewLine + separator + Environment.NewLine);
                }

                // ---| Always append new entry |---
                string existingLog = File.ReadAllText(logFilePath);
                var Params = InputData.OptimizerSettingsData;
                string tick = "Yes";
                string cross = "No";

                var entry = ExperFolderName.PadRight(20) +
                            $"| {N}".PadRight(6) +
                            $"| {R}".PadRight(6) +
                            $"| {(Params.SpecialConsParams.MinMaxAct ? tick : cross)}".PadRight(10) +
                            $"| {(Params.SpecialConsParams.WithAct ? tick : cross)}".PadRight(8) +
                            $"| {(Params.SpecialConsParams.WithoutAct ? tick : cross)}".PadRight(10) +
                            $"| {(Params.SpecialConsParams.GenderAct ? tick : cross)}".PadRight(10) +
                            $"| {(Params.SpecialConsParams.GenderPriority ? tick : cross)}".PadRight(16) +
                            $"| {(Params.SpecialConsParams.WithoutPriority ? tick : cross)}";

                File.AppendAllText(logFilePath, entry + Environment.NewLine);
                #endregion

                var IdxCrewCat = new List<(string, int)>();
                var CrewCategories = CommonFunctions.GetCrewCategData(false).ToList();

                for (int i = 0; i < CrewCategories.Count; i++)
                {
                    IdxCrewCat.Add((CrewCategories[i].CrewCatCode, i + 1));
                }
                Generate_CrewData(ExperFolderPath, ExperimentName, IdxCrewCat, CrewCategories);
                Generate_RouteData(ExperFolderPath, ExperimentName, IdxCrewCat, CrewCategories);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Crew Route Data: {ex.Message}");
            }
        }

        public void Generate_CrewData(string ExperimentPath, string ExperimentName, List<(string, int)> IdxCrewCat, List<CrewCategData> CrewCategories)
        {
            try
            {
                string logFilePath = Path.Combine(ExperimentPath, $"ProblemData_{ExperimentName}_CrewMembers.txt");

                var CrewMemberTxt = new System.Text.StringBuilder();



                #region Fill TXT
                CrewMemberTxt.AppendLine($"{InputData.Employees.Count}");

                // ---| Line 1: All LowerBounds |---
                CrewMemberTxt.AppendLine(string.Join(" ", InputData.Employees.Select(emp => emp.EmpCrSettings.LowerBound)));

                // ---| Line 2: All UpperBounds |---
                CrewMemberTxt.AppendLine(string.Join(" ", InputData.Employees.Select(emp => emp.EmpCrSettings.UpperBound)));

                // ---| Line 3: All CrewCatIdx |---
                var crewCatIdxList = new List<int>();
                foreach (var emp in InputData.Employees)
                {
                    var EMPCrewCatData = CommonFunctions.GetEMPCrewCategData(emp.Code, false).ToList();
                    var empCrewCat = EMPCrewCatData.FirstOrDefault();
                    int crewCatIdx = 0;
                    if (empCrewCat != null)
                    {
                        var match = IdxCrewCat.FirstOrDefault(x => x.Item1 == empCrewCat.CrewCateg.CrewCatCode);
                        crewCatIdx = match != default ? match.Item2 : 0;
                    }
                    crewCatIdxList.Add(crewCatIdx);
                }
                CrewMemberTxt.AppendLine(string.Join(" ", crewCatIdxList));

                // ---| Line 4: All Genders |---
                CrewMemberTxt.AppendLine(string.Join(" ", InputData.Employees.Select(emp => emp.Gender == BasicEnums.Gender.Male ? 1 : 0)));

                #endregion

                File.WriteAllText(logFilePath, CrewMemberTxt.ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Crew Member Data: {ex.Message}");
            }

        }

        public void Generate_RouteData(string ExperimentPath, string ExperimentName,List<(string, int)> IdxCrewCat, List<CrewCategData> CrewCategories)
        {
            try
            {
                string logFilePath = Path.Combine(ExperimentPath, $"ProblemData_{ExperimentName}_Routes.txt");

                var RouteTxt = new System.Text.StringBuilder();

                var Routes = InputData.FlightRoutesData.ToList();

                var experimentStartDate = InputData.DateFrom;
                var experimentEndDate = InputData.DateTo;

                #region Fill TXT
                RouteTxt.AppendLine($"{Routes.Count}");

                // ---| Line 1: All Start Days (days from DateFrom) |---

                RouteTxt.AppendLine(string.Join(" ", Routes.Select(route =>
                    (int)(route.StartDate - InputData.DateFrom).TotalDays)));
                // ---| Line 2: All End Day |---

                RouteTxt.AppendLine(string.Join(" ", Routes.Select(route =>
                    (int)(route.EndDate - InputData.DateFrom).TotalDays)));

                // ---| Line 3: DepartTime |---
                var departTimeList = new List<int>();
                foreach (var route in Routes)
                {
                    int DateStartIdx = (int)(route.StartDate - InputData.DateFrom).TotalDays;
                    int dayMinutes = DateStartIdx * 24 * 60;
                    int timeMinutes = (int)route.StartDate.TimeOfDay.TotalMinutes;
                    int departTime = dayMinutes + timeMinutes;
                    departTimeList.Add(departTime);
                }
                RouteTxt.AppendLine(string.Join(" ", departTimeList));

                // ---| Line 4: ArrivalTime |---
                var arrivalTimeList = new List<int>();
                foreach (var route in Routes)
                {
                    int DateEndIdx = (int)(route.EndDate - InputData.DateFrom).TotalDays;
                    int dayMinutes = DateEndIdx * 24 * 60;
                    int timeMinutes = (int)route.EndDate.TimeOfDay.TotalMinutes;
                    int arrivalTime = dayMinutes + timeMinutes;
                    arrivalTimeList.Add(arrivalTime);
                }
                RouteTxt.AppendLine(string.Join(" ", arrivalTimeList));

                // ---| Line 5: FlightHours |---
                var flightHoursList = new List<int>();
                foreach (var route in Routes)
                {
                    int flightHours = arrivalTimeList[Routes.IndexOf(route)] - departTimeList[Routes.IndexOf(route)];
                    flightHoursList.Add((int)Math.Round(flightHours / 60.0));
                }
                RouteTxt.AppendLine(string.Join(" ", flightHoursList));

                // ---| Line 5: Complement |---
                #region Complement
                if (InputData.Position == EmployeeType.Captain)
                {
                    RouteTxt.AppendLine(string.Join(" ", Routes.Select(route =>
                        (int)(route.Complement_Captain))));

                }
                else if (InputData.Position == EmployeeType.FO)
                {
                    RouteTxt.AppendLine(string.Join(" ", Routes.Select(route =>
                        (int)(route.Complement_FO))));

                }
                else if (InputData.Position == EmployeeType.Cabin_Manager)
                {
                    RouteTxt.AppendLine(string.Join(" ", Routes.Select(route =>
                        (int)(route.Complement_Cabin_Manager))));

                }
                else if (InputData.Position == EmployeeType.Flight_Attendant)
                {
                    RouteTxt.AppendLine(string.Join(" ", Routes.Select(route =>
                        (int)(route.Complement_Flight_Attendant))));

                }

                #endregion

                bool specialcon_act = false;
                var maxCrewcatList = new List<int>();
                var maxBoundsList = new List<int>();
                var withDataCrewcatList1 = new List<int>();
                var withDataCrewcatList2 = new List<int>();
                var withoutDataCrewcatList1 = new List<int>();
                var withoutDataCrewcatList2 = new List<int>();
                var genderList = new List<int>();

                var maxRouteIndices = new List<int>();
                var withRouteIndices = new List<int>();
                var withoutRouteIndices = new List<int>();
                var genderRouteIndices = new List<int>();
                   


                if (InputData.OptimizerSettingsData.SpecialConsParams.MinMaxAct) //Mono ulopoihsh gia max
                {
                    // ---| Line 6: Max Crew Category |---
                    // ---| Line 7: Bounds |---
                    specialcon_act = true;
                    var MinMaxData = InputData.MinMaxList.Where(x => x.IsAct == true).ToList(); 
                    bool RuleFound = false;

                    for(int  i = 0; i < Routes.Count; i++)
                    {
                        var route = Routes[i];
                         RuleFound = false;

                        foreach (var rule in MinMaxData)
                        {
                            if (route.RouteCateg == rule.RouteCateg)
                            {

                                var match = IdxCrewCat.FirstOrDefault(x => x.Item1 == rule.CrewCat.CrewCatCode);
                                var crewCatIdx = match != default ? match.Item2 : -1;
                                if(crewCatIdx == match.Item2)
                                {
                                    maxCrewcatList.Add(crewCatIdx);
                                    maxBoundsList.Add(rule.Rhs);
                                    maxRouteIndices.Add(i);
                                    RuleFound = true;
                                    break;

                                }
                            }
                        }
                        if (!RuleFound)
                        {
                            maxCrewcatList.Add(-1);
                            maxBoundsList.Add(-1);
                        }
                    }

                    // ---| Max CrewCat per Route |---
                    RouteTxt.AppendLine(string.Join(" ", maxCrewcatList));
                    // ---| Max Bounds per Route |---
                    RouteTxt.AppendLine(string.Join(" ", maxBoundsList));
                }
                else
                {
                    for (int i = 0; i < Routes.Count; i++)
                    {
                        maxCrewcatList.Add(-1);
                        maxBoundsList.Add(-1);

                    }
                    RouteTxt.AppendLine(string.Join(" ", maxCrewcatList));
                    RouteTxt.AppendLine(string.Join(" ", maxBoundsList));

                }
                if (InputData.OptimizerSettingsData.SpecialConsParams.WithAct)
                {
                    specialcon_act = true;
                    var withData = InputData.WithWithoutList.Where(x => x.IsWith == true && x.IsAct == true).ToList();
                    bool RuleFound = false;

                    for (int i = 0; i < Routes.Count; i++)
                    {
                        var route = Routes[i];
                        RuleFound = false;

                        foreach (var rule in withData)
                        {
                            if (route.RouteCateg == rule.RouteCateg)
                            {
                                var match1 = IdxCrewCat.FirstOrDefault(x => x.Item1 == rule.CrewCat1.CrewCatCode);
                                var crewCatIdx1 = match1 != default ? match1.Item2 : -1;

                                var match2 = IdxCrewCat.FirstOrDefault(x => x.Item1 == rule.CrewCat2.CrewCatCode);
                                var crewCatIdx2 = match2 != default ? match2.Item2 : -1;

                                withDataCrewcatList1.Add(crewCatIdx1);
                                withDataCrewcatList2.Add(crewCatIdx2);
                                withRouteIndices.Add(i);
                                RuleFound = true;
                                break;
                            }
                        }
                        if (!RuleFound)
                        {
                            withDataCrewcatList1.Add(-1);
                            withDataCrewcatList2.Add(-1);
                        }
                    }

                    if (withDataCrewcatList1.Count > 0)
                    {
                        RouteTxt.AppendLine(string.Join(" ", withDataCrewcatList1));
                        RouteTxt.AppendLine(string.Join(" ", withDataCrewcatList2));
                    }
                }
                else
                {
                    for (int i = 0; i < Routes.Count; i++)
                    {
                        withDataCrewcatList1.Add(-1);
                        withDataCrewcatList2.Add(-1);

                    }
                    RouteTxt.AppendLine(string.Join(" ", withDataCrewcatList1));
                    RouteTxt.AppendLine(string.Join(" ", withDataCrewcatList2));

                }
                if (InputData.OptimizerSettingsData.SpecialConsParams.WithoutAct)
                {
                    specialcon_act = true;
                    var withoutData = InputData.WithWithoutList.Where(x => x.IsWith == false && x.IsAct == true).ToList();
                    bool RuleFound = false;

                    for (int i = 0; i < Routes.Count; i++)
                    {
                        var route = Routes[i];
                        RuleFound = false;

                        foreach (var rule in withoutData)
                        {
                            if (route.RouteCateg == rule.RouteCateg)
                            {
                                var match1 = IdxCrewCat.FirstOrDefault(x => x.Item1 == rule.CrewCat1.CrewCatCode);
                                var crewCatIdx1 = match1 != default ? match1.Item2 : -1;

                                var match2 = IdxCrewCat.FirstOrDefault(x => x.Item1 == rule.CrewCat2.CrewCatCode);
                                var crewCatIdx2 = match2 != default ? match2.Item2 : -1;

                                withoutDataCrewcatList1.Add(crewCatIdx1);
                                withoutDataCrewcatList2.Add(crewCatIdx2);
                                withoutRouteIndices.Add(i);
                                RuleFound = true;
                                break;
                            }
                        }
                        if (!RuleFound)
                        {
                            withoutDataCrewcatList1.Add(-1);
                            withoutDataCrewcatList2.Add(-1);
                        }
                    }

                    if (withoutDataCrewcatList1.Count > 0)
                    {
                        RouteTxt.AppendLine(string.Join(" ", withoutDataCrewcatList1));
                        RouteTxt.AppendLine(string.Join(" ", withoutDataCrewcatList2));
                    }
                }
                else
                {
                    for (int i = 0; i < Routes.Count; i++)
                    {
                        withoutDataCrewcatList1.Add(-1);
                        withoutDataCrewcatList2.Add(-1);

                    }
                    RouteTxt.AppendLine(string.Join(" ", withoutDataCrewcatList1));
                    RouteTxt.AppendLine(string.Join(" ", withoutDataCrewcatList2));

                }

                if (InputData.OptimizerSettingsData.SpecialConsParams.GenderAct)
                {
                    specialcon_act = true;

                }
                else
                {
                    for (int i = 0; i < Routes.Count; i++)
                        genderList.Add(-1);
                    RouteTxt.AppendLine(string.Join(" ", genderList));

                }
                #endregion

      
                    // ---| Line: Counts |---
                    RouteTxt.AppendLine($"{maxRouteIndices.Count} {withRouteIndices.Count} {withoutRouteIndices.Count} {genderRouteIndices.Count}");

                if (specialcon_act)
                {
                    // ---| Max Routes |---
                    if (maxRouteIndices.Count > 0)
                        RouteTxt.AppendLine(string.Join(" ", maxRouteIndices));

                    // ---| With Routes |---
                    if (withRouteIndices.Count > 0)
                        RouteTxt.AppendLine(string.Join(" ", withRouteIndices));

                    // ---| Without Routes |---
                    if (withoutRouteIndices.Count > 0)
                        RouteTxt.AppendLine(string.Join(" ", withoutRouteIndices));

                    // ---| Gender Routes |---
                    if (genderRouteIndices.Count > 0)
                        RouteTxt.AppendLine(string.Join(" ", genderRouteIndices));
                }


                File.WriteAllText(logFilePath, RouteTxt.ToString());

            }
 
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Crew Member Data: {ex.Message}");
            }

        }

        #region Execute Run C++
        private async void ExecuteRunCplusplus(string debugPath, string releasePath)
        {
            try
            {
                // ---| Step 1: Get exe path |---
#if DEBUG
                string exePath = debugPath;
#else
        string exePath = releasePath;
#endif

                // ---| Step 2: Start C++ Process |---
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.CreateNoWindow = false;
                process.Start();

#if DEBUG
                // ---| Step 3: Attach Debugger to C++ Process |---
                await AttachDebuggerToCplusplus();
#endif

                // ---| Step 4: Wait for C++ to finish |---
                await Task.Run(() => process.WaitForExit());
                MessageBox.Show("C++ execution completed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error running C++: {ex.Message}");
            }
        }

#if DEBUG
        private async Task AttachDebuggerToCplusplus()
        {
            // ---| Wait for process to start |---
            await Task.Delay(1000);

            // ---| Get Visual Studio DTE with retry |---
            EnvDTE.DTE dte = null;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    dte = (EnvDTE.DTE)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE");
                    break;
                }
                catch
                {
                    await Task.Delay(500);
                }
            }

            if (dte == null) return;

            // ---| Attach to ColgenNikosC++ process with retry |---
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    foreach (EnvDTE.Process proc in dte.Debugger.LocalProcesses)
                    {
                        if (proc.Name.Contains("ColgenNikosC++"))
                        {
                            proc.Attach();
                            return; // success
                        }
                    }
                    break;
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    await Task.Delay(500); // VS busy - retry
                }
            }
        }
#endif

        #endregion

        #endregion

        #endregion

        #region Extra
        private double CalculateCijCost(EmployeeData emp, FlightRoutesData route)
        {
            var LowerBound = emp.EmpCrSettings.LowerBound;
            var UpperBound = emp.EmpCrSettings.UpperBound;

            double targetFlightHours = (LowerBound + UpperBound) / 2; // Example target value from the image


            // Calculate actual flight hours for the route
            double actualFlightHours = (route.EndDate - route.StartDate).TotalHours;

            // Calculate variance
            double variance = Math.Abs(actualFlightHours - targetFlightHours);

            // Calculate penalty deviation
            double penaltyDeviation = 0;
            if (actualFlightHours > UpperBound)
            {
                penaltyDeviation = actualFlightHours - UpperBound;
            }
            else if (actualFlightHours < LowerBound)
            {
                penaltyDeviation = LowerBound - actualFlightHours;
            }

            // Calculate Cij_Cost
            double Cij_Hours = penaltyDeviation;
            return Cij_Hours;
        }
        #endregion
        #endregion


        public CrewSchedulingViewModel()
        {
            #region Data Initialization

            InputData = new CSInputData();
            InputData.OptimizerSettingsData = new OptimizerSettingsData();
            InputData.Code = " ";
            InputData.Position = BasicEnums.EmployeeType.Captain;
            InputData.DateFrom = new DateTime(2024, 6, 1);
            InputData.DateTo = new DateTime(2024, 6, 30);
            InputData.RoutesPenalty = 1000000;
            InputData.BoundsPenalty = 100;
            InputData.CSType = BasicEnums.CSType.Set_Partition;
            InputData.FlightRoutesData = new ObservableCollection<FlightRoutesData>();
            InputData.Employees = new ObservableCollection<EmployeeData>();
            this.sfGridColumns = new Columns();

            #endregion

            #region Commands Initialization

            CalculateCS_GB = new RelayCommand2(ExecuteCalculateCS_Gurobi);
            CalculateCS_CPLEX = new RelayCommand2(ExecuteCalculateCS_CPLEX);
            CalculateCS_Cplusplus = new RelayCommand2(ExecuteCalculateCS_Cplusplus);
            ShowEmployeesGridCommand = new RelayCommand2(ExecuteShowEmployeesGridCommand);
            ShowFlightRoutestGridCommand = new RelayCommand2(ExecuteShowFlightRoutestGridCommand);
            ShowCrewSchedulingGridCommand = new RelayCommand2(ExecuteShowCrewSchedulingGridCommand);
            ShowOptimizerSettingsGridCommand = new RelayCommand2(ExecuteShowOptimizerSettingsGridCommand);

            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            #endregion

        }

    }
}
