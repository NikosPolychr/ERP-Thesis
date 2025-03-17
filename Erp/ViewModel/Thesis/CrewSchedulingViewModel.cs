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
using Erp.Model.Colgen;
using System.IO;


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

            if (F7key == "CSCODE")
            {
                InputData = (SelectedItem as CSInputData);
                InputData.ColgenSettings = new ColgenSettings();

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
                SelectedTabIndex = 2;
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

        #region Colgen

        public ICommand CalculateColgen { get; }
        private void ExecuteCalculateColgen(object obj)
        {
            if (InputData.RandomDataFlag == false) 
            {
                #region Specific Data
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

                #endregion
            }
            else
            {
                #region Random Data

                //int R = InputData.ColgenSettings.R;
                //int N = InputData.ColgenSettings.N;
                //int Days = InputData.ColgenSettings.N;

                //// ---| Global Variables
                //int[] RouteDepartDay = new int[R];
                //int[] RouteArrivalDay = new int[R];
                //int[] RouteDepartTime = new int[R];
                //int[] RouteArrivalTime = new int[R];
                //int[] RouteFlightHours = new int[R];

                //int[] CrewMemberFlightHoursLB = new int[N];
                //int[] CrewMemberFlightHoursUB = new int[N];

                //int[,] ValidDayOff = new int[Days, R];
                //double OptimalLPObjValue;

                //// The structures in C are often replaced by classes in C#
                //TreeNode Treelist = null;
                //AuxList first = null;

                //#region Filepath

                //string relativePath = Path.Combine("Colgen", "Data Input");
                //Directory.CreateDirectory(relativePath);

                //#endregion

                //GenerateRouteData();
                //GenerateCrewMemberData();

                ////Print Options
                //bool PrntProgramsParametersToAFile = true;
                //bool PrntRouteAndCrewMemberDataToConsole = false, PrntRouteDataToAFile = true, PrntCrewMemberDataToAFile = true;
                ////-------------------------//

                //if (PrntProgramsParametersToAFile == true) PrintProgramsParametersToAFile();
                //if (PrntRouteAndCrewMemberDataToConsole == true) PrintRouteAndCrewMemberDataToConsole();
                //if (PrntRouteDataToAFile == true) PrintRouteDataToAFile();
                //if (PrntCrewMemberDataToAFile == true) PrintCrewMemberDataToAFile();


                //void GenerateRouteData()
                //{
                //    int i, random_number, j, temp;
                //    int index = 0;
                //    int Hours = Days * 24, MinsPerDay = 24 * 60;
                //    double min;
                //    int MinRouteDuration = InputData.ColgenSettings.MinRouteDuration;
                //    int MaxRouteDuration = InputData.ColgenSettings.MinRouteDuration;
                //    double MaxDouble = InputData.ColgenSettings.MaxDouble;

                //    // Initialize the arrays
                //    for (i = 0; i <= R - 1; i++)
                //    {
                //        RouteDepartDay[i] = 0;
                //        RouteArrivalDay[i] = 0;
                //        RouteDepartTime[i] = 0;
                //        RouteArrivalTime[i] = 0;
                //        RouteFlightHours[i] = 0;
                //    }

                //    // Generate random values
                //    for (i = 0; i <= R - 1; i++)
                //    {
                //        random_number = new Random().Next(0, Days); // random number between 0 and Days
                //        RouteDepartDay[i] = random_number;

                //        random_number = new Random().Next(0, MinsPerDay); // random number between 0 and MinsPerDay
                //        random_number = random_number - (random_number % 10);
                //        RouteDepartTime[i] = (RouteDepartDay[i] * MinsPerDay) + random_number;
                //        random_number = new Random().Next(MinRouteDuration, MaxRouteDuration + 1); // random between MinRouteDuration and MaxRouteDuration
                //        RouteFlightHours[i] = random_number;

                //        while (RouteDepartTime[i] + (RouteFlightHours[i] * 60) > Days * MinsPerDay)
                //        {
                //            RouteDepartTime[i] = RouteDepartTime[i] - MaxRouteDuration;
                //            random_number = new Random().Next(MinRouteDuration, MaxRouteDuration + 1);
                //            RouteFlightHours[i] = random_number;
                //        }

                //        RouteArrivalTime[i] = RouteDepartTime[i] + RouteFlightHours[i] * 60;

                //        RouteArrivalDay[i] = RouteArrivalTime[i] / MinsPerDay;
                //    }

                //    // Sort arrays
                //    for (i = 0; i <= R - 1; i++)
                //    {
                //        min = MaxDouble;
                //        for (j = i; j <= R - 1; j++)
                //        {
                //            if (RouteDepartTime[j] <= min)
                //            {
                //                min = RouteDepartTime[j];
                //                index = j;
                //            }
                //        }

                //        // Swaps
                //        temp = RouteDepartTime[i];
                //        RouteDepartTime[i] = RouteDepartTime[index];
                //        RouteDepartTime[index] = temp;

                //        temp = RouteArrivalTime[i];
                //        RouteArrivalTime[i] = RouteArrivalTime[index];
                //        RouteArrivalTime[index] = temp;

                //        temp = RouteDepartDay[i];
                //        RouteDepartDay[i] = RouteDepartDay[index];
                //        RouteDepartDay[index] = temp;

                //        temp = RouteArrivalDay[i];
                //        RouteArrivalDay[i] = RouteArrivalDay[index];
                //        RouteArrivalDay[index] = temp;

                //        temp = RouteFlightHours[i];
                //        RouteFlightHours[i] = RouteFlightHours[index];
                //        RouteFlightHours[index] = temp;
                //    }
                //}

                //void GenerateCrewMemberData()
                //{
                //    int i, AverageFlHrs, SumRtFlHrs = 0, AverageFlHrsMin, AverageFlHrsMax, random_number;
                //    int CMBoundDiffAver = InputData.ColgenSettings.CMBoundDiffAver;
                //    int MaxFlightHoursCM = InputData.ColgenSettings.MaxFlightHoursCM;
                //    int WindowRangeCM = InputData.ColgenSettings.WindowRangeCM;

                //    // Calculate total flight hours of all routes
                //    for (i = 0; i <= R - 1; i++)
                //    {
                //        SumRtFlHrs = SumRtFlHrs + RouteFlightHours[i];
                //    }

                //    AverageFlHrs = SumRtFlHrs / N;
                //    AverageFlHrsMin = AverageFlHrs - CMBoundDiffAver;
                //    AverageFlHrsMax = AverageFlHrs + CMBoundDiffAver;

                //    // If there are too many flight hours for this number of crew members, then put an upper limit to the maximum hours each crew member can fly
                //    if (AverageFlHrsMax > MaxFlightHoursCM)
                //    {
                //        AverageFlHrsMax = MaxFlightHoursCM;
                //        AverageFlHrsMin = MaxFlightHoursCM - CMBoundDiffAver;
                //    }

                //    for (i = 0; i <= N - 1; i++) // for each crew member
                //    {
                //        random_number = new Random().Next(AverageFlHrsMin, AverageFlHrsMax + 1);
                //        CrewMemberFlightHoursLB[i] = random_number - WindowRangeCM;
                //        CrewMemberFlightHoursUB[i] = random_number + WindowRangeCM;
                //    }
                //}

                //void PrintRouteAndCrewMemberDataToConsole()
                //{
                //    // Prints Crew Member Data
                //    Console.WriteLine("\n|-- Crew Member Data");
                //    for (int i = 0; i <= N - 1; i++)
                //    {
                //        Console.WriteLine($"Crew Member {i} :  LowerBound = {CrewMemberFlightHoursLB[i]} , UpperBound = {CrewMemberFlightHoursUB[i]}");
                //    }

                //    // Prints Route Data
                //    Console.WriteLine("\n\n|-- Route Data");
                //    for (int i = 0; i <= R - 1; i++)
                //    {
                //        Console.WriteLine($"Route {i} : DepartureDay = {RouteDepartDay[i]} - ArrivalDay = {RouteArrivalDay[i]} , DepartureTime = {RouteDepartTime[i]} - ArrivalTime = {RouteArrivalTime[i]} , FlightHours = {RouteFlightHours[i]}");
                //    }

                //    Console.WriteLine(); // Add a final newline for better formatting
                //}

                //void PrintProgramsParametersToAFile()
                //{
                //    try
                //    {
                //        using (StreamWriter writer = new StreamWriter(Path.Combine(relativePath, "ProgramParameters_CrewScheduling.lp")))
                //        {
                //            writer.WriteLine("                                                      |-----------------  PROGRAM PARAMETERS  -----------------|");

                //            writer.WriteLine("\n|--- Instance Parameters  \n");
                //            writer.WriteLine($"|- N = {N},   Number of crew members of the same rank");
                //            writer.WriteLine($"|- R = {R},   Number of flight routes to be covered in the planning horizon");
                //            writer.WriteLine($"|- Days = {Days}, Number of days in planning horizon \n");

                //            writer.WriteLine("\n|--- Data Generation Parameters  \n");
                //            writer.WriteLine($"|- MinRouteDuration = {InputData.ColgenSettings.MinRouteDuration}, Minimum number of flight hours of a route");
                //            writer.WriteLine($"|- MaxRouteDuration = {InputData.ColgenSettings.MaxRouteDuration}, Maximum number of flight hours of a route");
                //            writer.WriteLine($"|- MaxFlightHoursCM = {InputData.ColgenSettings.MaxFlightHoursCM}, Maximum number of flight hours in the planning horizon that a crew member could take without a penalty [ It is an upper limit to the upper bounds of the crew members' flight hours windows]");
                //            writer.WriteLine($"|- WindowRangeCM    = {InputData.ColgenSettings.WindowRangeCM}, The range of the flight hour window of a crew member");
                //            writer.WriteLine($"|- CMBoundDiffAver  = {InputData.ColgenSettings.CMBoundDiffAver}, By subtracting/adding this parameter from/to the average flight hours per crew member we get a lower/upper bound for the flight hours of the crew member \n");

                //            writer.WriteLine("\n|--- Roster Legality Rules' Parameters  \n");
                //            writer.WriteLine($"|- DaysOff               = {InputData.ColgenSettings.DaysOff}, Minimum days in the planning horizon (month) that a crew member is resting (does not have duty)");
                //            writer.WriteLine($"|- MinHoursBetween       = {InputData.ColgenSettings.MinHoursBetween}, Minimum hours of break between consecutive duties in order to be considered a day off");
                //            writer.WriteLine($"|- LatestArrivalTime     = {InputData.ColgenSettings.LatestArrivalTime}, Latest arrival time of the first of two consecutive duties so that the next day of the first duty can be considered a day off [ 23 : 00 ]");
                //            writer.WriteLine($"|- EarliestDepartureTime = {InputData.ColgenSettings.EarliestDepartureTime}, Earliest departure time of the second of two consecutive duties so that the previous day of the second duty can be considered a day off [ 06 : 00 ]");
                //            writer.WriteLine($"|- XHoursBreak           = {InputData.ColgenSettings.XHoursBreak}, Rule XHoursBreak / YFlightHours");
                //            writer.WriteLine($"|- YFlightHours          = {InputData.ColgenSettings.YFlightHours}, Rule XHoursBreak / YFlightHours demands that a crew member has X hours of break without a flight in between, each time he/she completes Y hours of flight");
                //            writer.WriteLine($"|- MaximumFlightHours    = {InputData.ColgenSettings.MaximumFlightHours}, Maximum sum flight hours that a crew member is allowed to fly in the planning horizon\n");

                //            writer.WriteLine("\n|--- Coverage and Balance Cost Parameters \n");
                //            writer.WriteLine($"|- UndercoverCost  = {InputData.ColgenSettings.UndercoverCost}, Cost for not covering a flight leg");
                //            writer.WriteLine($"|- HourPenalty     = {InputData.ColgenSettings.HourPenalty}, Cost per hour violation of the time window of a crew member \n");

                //            writer.WriteLine("\n|--- Numeric Parameters  \n");
                //            writer.WriteLine($"|- MaxDouble = {InputData.ColgenSettings.MaxDouble} , A very large double number ");
                //            writer.WriteLine($"|- eps = {InputData.ColgenSettings.Eps} , A very small double number \n");

                //            writer.WriteLine("\n|--- Column Generation Performance Parameters \n");
                //            writer.WriteLine($"|- buckets                       = {InputData.ColgenSettings.Buckets}, Number of buckets of each Route node");
                //            writer.WriteLine($"|- fictbuckets                   = {InputData.ColgenSettings.FictBuckets}, Number of buckets of Fictitious node [Define this between 10 and 50]");
                //            writer.WriteLine($"|- ReducedCostCut                = {InputData.ColgenSettings.ReducedCostCut}, If the Reduced Cost of a candidate roster is less than the ReducedCostCut parameter then it is added to Master");
                //            writer.WriteLine($"|- NIDRAI                        = {InputData.ColgenSettings.NIDRAI}, Percentage of the total IDs for whom at least one roster was added to Master in a single column generation iteration, before Master is solved and Duals are updated [Define this between 0.01 and 1]");
                //            writer.WriteLine($"|- UpdateDualsManuallyScheme     = {InputData.ColgenSettings.UpdateDualsManuallyScheme}, If the user wishes to utilize the manual update of the duals scheme, set this value to 1. Otherwise, set it to 0 (duals update by solving the master with CPLEX each time all the variables for a crew member are inserted to master)\n");

                //            writer.WriteLine("\n|--- Branching Parameters \n");
                //            writer.WriteLine($"|- AbsBacktrack               = {InputData.ColgenSettings.AbsBacktrack}, Minimum absolute difference between the objective value of a master problem of a TreeNode (during the branching process) and the optimal objective value of the lp problem for which the flow backtracks. If the difference is less than the parameter, the flow does not backtrack");
                //            writer.WriteLine($"|- PerceBacktrack             = {InputData.ColgenSettings.PerceBacktrack}, Minimum percentage difference between the objective value of a master problem of a TreeNode (during the branching process) and the optimal objective value of the lp problem for which the flow backtracks. If the difference is less than the parameter, the flow does not backtrack");
                //            writer.WriteLine($"|- AbsMIP                     = {InputData.ColgenSettings.AbsMIP}, Minimum absolute difference between the optimal objective value of an integer solution and the optimal objective value of the lp problem for which the program continues to find a better integer solution. If the difference is less than the parameter, the program terminates");
                //            writer.WriteLine($"|- PerceMIP                   = {InputData.ColgenSettings.PerceMIP}, Minimum percentage difference between the optimal objective value of an integer solution and the optimal objective value of the lp problem for which the program continues to find a better integer solution. If the difference is less than the parameter, the program terminates");
                //            writer.WriteLine($"|- NumberOfBacktracksLimit    = {InputData.ColgenSettings.NumberOfBacktracksLimit}, Maximum number of backtracks allowed, after an integer solution has been found");
                //            writer.WriteLine($"|- NumberOfTreeNodesLimit     = {InputData.ColgenSettings.NumberOfTreeNodesLimit}, Maximum number of TreeNodes created allowed, after an integer solution has been found");
                //            writer.WriteLine($"|- TreelistSortingIdsFixed    = {InputData.ColgenSettings.TreelistSortingIdsFixed}, Minimum absolute difference between the MasterObjVals of two TreeNodes in the Treelist for which it is checked which of the two has more Ids fixed and the one with the most fixed is put in the front, in MIPGap loop, after an integer solution is found\n");
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine($"Error creating data file: {ex.Message}");
                //    }
                //}

                //void PrintRouteDataToAFile()
                //{
                //    try
                //    {


                //        // Create or open the file for writing
                //        using (StreamWriter writer = new StreamWriter(Path.Combine(relativePath, "RouteData_CrewScheduling.lp")))
                //        {
                //            writer.WriteLine("                                                      |-----------------  ROUTE DATA  -----------------|");
                //            writer.WriteLine();
                //            writer.WriteLine($"|--- Number of Routes = {R}");
                //            writer.WriteLine();

                //            // Write route data
                //            for (int i = 0; i <= R - 1; i++)
                //            {
                //                writer.WriteLine($"|- RT = {i} :   DepartureDay = {RouteDepartDay[i]} - ArrivalDay = {RouteArrivalDay[i]} ,  DepartureTime = {RouteDepartTime[i]} - ArrivalTime = {RouteArrivalTime[i]} ,  FlightHours = {RouteFlightHours[i]}");
                //            }
                //        }

                //        Console.WriteLine("Route data written to RouteData_CrewScheduling.lp successfully.");
                //    }
                //    catch (Exception ex)
                //    {
                //        // Handle any errors that occur during file creation or writing
                //        Console.WriteLine($"Error creating data file: {ex.Message}");
                //        Console.ReadLine();
                //        Environment.Exit(0);
                //    }
                //}

                //void PrintCrewMemberDataToAFile()
                //{
                //    try
                //    {


                //        // Create or open the file for writing
                //        using (StreamWriter writer = new StreamWriter(Path.Combine(relativePath, "CrewMemberData_CrewScheduling.lp")))

                //        {
                //            writer.WriteLine("                                                      |-----------------  CREW MEMBER DATA  -----------------|");
                //            writer.WriteLine();
                //            writer.WriteLine($"|--- Number of Crew Members = {N}");
                //            writer.WriteLine();

                //            // Write crew member data
                //            for (int i = 0; i <= N - 1; i++)
                //            {
                //                writer.WriteLine($"|- Crew Member = {i} :   LB = {CrewMemberFlightHoursLB[i]} ,   UB = {CrewMemberFlightHoursUB[i]}");
                //            }
                //        }

                //        Console.WriteLine("Crew member data written to CrewMemberData_CrewScheduling.lp successfully.");
                //    }
                //    catch (Exception ex)
                //    {
                //        // Handle any errors that occur during file creation or writing
                //        Console.WriteLine($"Error creating data file: {ex.Message}");
                //        Console.ReadLine();
                //        Environment.Exit(0);
                //    }
                //}


                #endregion

                ColgenFunctions.Calculate_Colgen(InputData);

            }



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

            //#region Extra

            //InputData.T = (int)Math.Ceiling((InputData.DateTo - InputData.DateFrom).TotalDays);

            //int dateCounter = 0;
            //for (var date = InputData.DateFrom; date <= InputData.DateTo; date = date.AddDays(1))
            //{
            //    InputData.DatesIndexMap.Add(dateCounter, date.Date);
            //    dateCounter++;
            //}

            //#endregion

        }



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
            InputData.Code = " ";
            InputData.Position = BasicEnums.EmployeeType.Captain;
            InputData.DateFrom = new DateTime(2024, 6, 1);
            InputData.DateTo = new DateTime(2024, 6, 30);
            InputData.RoutesPenalty = 1000000;
            InputData.BoundsPenalty = 100;
            InputData.CSType = BasicEnums.CSType.Set_Covering;
            InputData.FlightRoutesData = new ObservableCollection<FlightRoutesData>();
            InputData.Employees = new ObservableCollection<EmployeeData>();
            InputData.ColgenSettings = new ColgenSettings();
            this.sfGridColumns = new Columns();

            #endregion

            #region Commands Initialization

            CalculateCS_GB = new RelayCommand2(ExecuteCalculateCS_Gurobi);
            CalculateCS_CPLEX = new RelayCommand2(ExecuteCalculateCS_CPLEX);
            CalculateColgen = new RelayCommand2(ExecuteCalculateColgen);
            ShowEmployeesGridCommand = new RelayCommand2(ExecuteShowEmployeesGridCommand);
            ShowFlightRoutestGridCommand = new RelayCommand2(ExecuteShowFlightRoutestGridCommand);
            ShowCrewSchedulingGridCommand = new RelayCommand2(ExecuteShowCrewSchedulingGridCommand);
            rowDataCommand = new RelayCommand2(ChangeCanExecute);

            #endregion

        }

    }
}
