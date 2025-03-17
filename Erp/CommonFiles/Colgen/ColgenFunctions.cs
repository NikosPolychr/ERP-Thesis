using Erp.Model.Colgen;
using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Thesis.CrewScheduling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ILOG.Concert;
using ILOG.CPLEX;
using Gurobi;
using Erp.Repositories;
using Erp.DataBase;
using Erp.Model;

namespace Erp.CommonFiles.Colgen
{
    public class ColgenFunctions: RecordBaseModel
    {
        //---| CPLEX Objects
        //---| Gurobi Objects

        #region Gurobi Data Initialization

        GRBEnv MasterEnv_Gb = null;  // Gurobi environment for the master problem
        GRBEnv SCtoSPEnv_Gb = null;  // Gurobi environment for the SC-to-SP conversion

        GRBModel MasterLp_Gb = null;  // Gurobi model for the master linear program
        GRBModel SCtoSP_Gb = null;    // Gurobi model for the SC-to-SP conversion

        // Set up Gurobi WLS credentials
        string wlsAccessId = "044e1ea7-f383-47f4-99b3-8bd25c2f9367";
        string wlsSecret = "e7bd7d8a-533f-4e50-95f8-3b1a56afee5b";
        string licenseId = "2612362";



        #endregion

        #region Data Initialization
        // Event for property change notifications
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        //---| Instance Parameters 
        // Number of crew members of the same rank
        private int _n;
        public int N
        {
            get => _n;
            set { _n = value; OnPropertyChanged(); }
        }

        // Number of flight routes to be covered in the planning horizon
        private int _r;
        public int R
        {
            get => _r;
            set { _r = value; OnPropertyChanged(); }
        }

        // Number of days in planning horizon
        private int _days;
        public int Days
        {
            get => _days;
            set { _days = value; OnPropertyChanged(); }
        }

        //---| Data Generation Parameters 
        // Minimum number of flight hours of a route
        private int _minRouteDuration;
        public int MinRouteDuration
        {
            get => _minRouteDuration;
            set { _minRouteDuration = value; OnPropertyChanged(); }
        }

        // Maximum number of flight hours of a route
        private int _maxRouteDuration;
        public int MaxRouteDuration
        {
            get => _maxRouteDuration;
            set { _maxRouteDuration = value; OnPropertyChanged(); }
        }

        // Maximum number of flight hours in the planning horizon that a crew member could take without a penalty
        private int _maxFlightHoursCM;
        public int MaxFlightHoursCM
        {
            get => _maxFlightHoursCM;
            set { _maxFlightHoursCM = value; OnPropertyChanged(); }
        }

        // The range of the flight hour window of a crew member
        private int _windowRangeCM;
        public int WindowRangeCM
        {
            get => _windowRangeCM;
            set { _windowRangeCM = value; OnPropertyChanged(); }
        }

        // By subtracting/adding this parameter from/to the average flight hours per crew member,
        // we get a lower/upper bound for the flight hours of the crew member
        private int _cmBoundDiffAver;
        public int CMBoundDiffAver
        {
            get => _cmBoundDiffAver;
            set { _cmBoundDiffAver = value; OnPropertyChanged(); }
        }

        //---| Roster Legality Rules' Parameters
        // Minimum days in the planning horizon (month) that a crew member is resting (does not have duty)
        private int _daysOff;
        public int DaysOff
        {
            get => _daysOff;
            set { _daysOff = value; OnPropertyChanged(); }
        }

        // Minimum hours of break between consecutive duties in order to be considered a day off
        private int _minHoursBetween;
        public int MinHoursBetween
        {
            get => _minHoursBetween;
            set { _minHoursBetween = value; OnPropertyChanged(); }
        }

        // Latest arrival time of the first of two consecutive duties so that the next day of the first duty can be considered a day off
        private int _latestArrivalTime;
        public int LatestArrivalTime
        {
            get => _latestArrivalTime;
            set { _latestArrivalTime = value; OnPropertyChanged(); }
        }

        // Earliest departure time of the second of two consecutive duties so that the previous day of the second duty can be considered a day off
        private int _earliestDepartureTime;
        public int EarliestDepartureTime
        {
            get => _earliestDepartureTime;
            set { _earliestDepartureTime = value; OnPropertyChanged(); }
        }

        // Rule XHoursBreak / YFlightHours: The number of hours a crew member must have as a break without a flight
        private int _xHoursBreak;
        public int XHoursBreak
        {
            get => _xHoursBreak;
            set { _xHoursBreak = value; OnPropertyChanged(); }
        }

        // Rule XHoursBreak / YFlightHours: The total number of flight hours after which the crew member must have a break
        private int _yFlightHours;
        public int YFlightHours
        {
            get => _yFlightHours;
            set { _yFlightHours = value; OnPropertyChanged(); }
        }

        // Maximum sum of flight hours that a crew member is allowed to fly in the planning horizon
        private int _maximumFlightHours;
        public int MaximumFlightHours
        {
            get => _maximumFlightHours;
            set { _maximumFlightHours = value; OnPropertyChanged(); }
        }

        //---| Coverage and Balance Cost Parameters
        // Cost for not covering a flight leg
        private int _undercoverCost;
        public int UndercoverCost
        {
            get => _undercoverCost;
            set { _undercoverCost = value; OnPropertyChanged(); }
        }

        // Cost per hour violation of the time window of a crew member
        private int _hourPenalty;
        public int HourPenalty
        {
            get => _hourPenalty;
            set { _hourPenalty = value; OnPropertyChanged(); }
        }

        //---| Numeric Parameters 
        // A very large double number
        private double _maxDouble;
        public double MaxDouble
        {
            get => _maxDouble;
            set { _maxDouble = value; OnPropertyChanged(); }
        }

        // A very small number
        private double _eps;
        public double Eps
        {
            get => _eps;
            set { _eps = value; OnPropertyChanged(); }
        }

        //---| Column Generation Performance Parameters
        // Number of buckets of each Route node
        private int _buckets;
        public int Buckets
        {
            get => _buckets;
            set { _buckets = value; OnPropertyChanged(); }
        }

        // Number of buckets of Fictitious node [Define this between 10 and 50]
        private int _fictBuckets;
        public int FictBuckets
        {
            get => _fictBuckets;
            set { _fictBuckets = value; OnPropertyChanged(); }
        }

        // If the Reduced Cost of a candidate roster is less than this parameter, it is added to Master
        private double _reducedCostCut = -1;
        public double ReducedCostCut
        {
            get => _reducedCostCut;
            set { _reducedCostCut = value; OnPropertyChanged(); }
        }

        // Percentage of the total IDs for whom at least one roster was added to Master in a single column generation iteration
        private double _nidrai;
        public double NIDRAI
        {
            get => _nidrai;
            set { _nidrai = value; OnPropertyChanged(); }
        }

        // If the user wishes to utilize the manual update of the duals scheme, set this value to 1; otherwise, set to 0
        private int _updateDualsManuallyScheme;
        public int UpdateDualsManuallyScheme
        {
            get => _updateDualsManuallyScheme;
            set { _updateDualsManuallyScheme = value; OnPropertyChanged(); }
        }

        //---| Branching Parameters
        // Minimum absolute difference between the objective value of a master problem and the optimal objective value
        private int _absBacktrack;
        public int AbsBacktrack
        {
            get => _absBacktrack;
            set { _absBacktrack = value; OnPropertyChanged(); }
        }

        // Minimum percentage difference between the objective value of a master problem and the optimal objective value
        private double _perceBacktrack;
        public double PerceBacktrack
        {
            get => _perceBacktrack;
            set { _perceBacktrack = value; OnPropertyChanged(); }
        }

        // Minimum absolute difference between the optimal objective value of an integer solution and the optimal objective value
        private int _absMIP;
        public int AbsMIP
        {
            get => _absMIP;
            set { _absMIP = value; OnPropertyChanged(); }
        }

        // Minimum percentage difference between the optimal objective value of an integer solution and the optimal objective value
        private double _perceMIP;
        public double PerceMIP
        {
            get => _perceMIP;
            set { _perceMIP = value; OnPropertyChanged(); }
        }

        // Maximum number of backtracks allowed after an integer solution has been found
        private int _numberOfBacktracksLimit;
        public int NumberOfBacktracksLimit
        {
            get => _numberOfBacktracksLimit;
            set { _numberOfBacktracksLimit = value; OnPropertyChanged(); }
        }

        // Maximum number of TreeNodes created allowed after an integer solution has been found
        private int _numberOfTreeNodesLimit = 100 * 100; // Assuming N is default 100
        public int NumberOfTreeNodesLimit
        {
            get => _numberOfTreeNodesLimit;
            set { _numberOfTreeNodesLimit = value; OnPropertyChanged(); }
        }

        // Minimum absolute difference between the MasterObjVals of two TreeNodes for sorting in the Treelist
        private int _treelistSortingIdsFixed;
        public int TreelistSortingIdsFixed
        {
            get => _treelistSortingIdsFixed;
            set { _treelistSortingIdsFixed = value; OnPropertyChanged(); }
        }

        #region Lists
        private int[] _CrewMemberFlightHoursLB;
        public int[] CrewMemberFlightHoursLB
        {
            get => _CrewMemberFlightHoursLB;
            set { _CrewMemberFlightHoursLB = value; OnPropertyChanged(); }
        }
        #endregion

        #endregion



        public void Calculate_Colgen(CSInputData InputData)
        {
            try
            {
                if (InputData.RandomDataFlag == false)
                {
                }
                else
                {
                    #region Random Data

                    R = InputData.ColgenSettings.R;
                    N = InputData.ColgenSettings.N;
                    MinRouteDuration = InputData.ColgenSettings.MinRouteDuration;
                    MaxRouteDuration = InputData.ColgenSettings.MaxRouteDuration;
                    MaxFlightHoursCM = InputData.ColgenSettings.MaxFlightHoursCM;
                    WindowRangeCM = InputData.ColgenSettings.WindowRangeCM;
                    CMBoundDiffAver = InputData.ColgenSettings.CMBoundDiffAver;
                    DaysOff = InputData.ColgenSettings.DaysOff;
                    MinHoursBetween = InputData.ColgenSettings.MinHoursBetween;
                    LatestArrivalTime = InputData.ColgenSettings.LatestArrivalTime;
                    EarliestDepartureTime = InputData.ColgenSettings.EarliestDepartureTime;
                    XHoursBreak = InputData.ColgenSettings.XHoursBreak;
                    YFlightHours = InputData.ColgenSettings.YFlightHours;
                    MaximumFlightHours = InputData.ColgenSettings.MaximumFlightHours;
                    UndercoverCost = InputData.ColgenSettings.UndercoverCost;
                    HourPenalty = InputData.ColgenSettings.HourPenalty;
                    MaxDouble = InputData.ColgenSettings.MaxDouble;

                    //Colgen Data
                    Buckets = InputData.ColgenSettings.Buckets;
                    FictBuckets = InputData.ColgenSettings.FictBuckets;
                    ReducedCostCut = InputData.ColgenSettings.ReducedCostCut;
                    NIDRAI = InputData.ColgenSettings.NIDRAI;
                    AbsBacktrack = InputData.ColgenSettings.AbsBacktrack;
                    PerceBacktrack = InputData.ColgenSettings.PerceBacktrack;
                    AbsMIP = InputData.ColgenSettings.AbsMIP;
                    PerceMIP = InputData.ColgenSettings.PerceMIP;
                    NumberOfTreeNodesLimit = InputData.ColgenSettings.NumberOfTreeNodesLimit;
                    TreelistSortingIdsFixed = InputData.ColgenSettings.TreelistSortingIdsFixed;

                    // ---| Global Variables
                    int[] RouteDepartDay = new int[R];
                    int[] RouteArrivalDay = new int[R];
                    int[] RouteDepartTime = new int[R];
                    int[] RouteArrivalTime = new int[R];
                    int[] RouteFlightHours = new int[R];

                    CrewMemberFlightHoursLB = new int[N];
                    int[] CrewMemberFlightHoursUB = new int[N];

                    int[,] ValidDayOff = new int[Days, R];

                    double OptimalLPObjValue;

                    // The structures in C are often replaced by classes in C#
                    TreeNode Treelist = null;
                    AuxList first = null;

                    #region Filepath

                    string relativePath = Path.Combine("Colgen", "Data Input");
                    Directory.CreateDirectory(relativePath);

                    #endregion

                    GenerateRouteData();
                    GenerateCrewMemberData();

                    //Print Options
                    bool PrntProgramsParametersToAFile = true;
                    bool PrntRouteAndCrewMemberDataToConsole = false, PrntRouteDataToAFile = true, PrntCrewMemberDataToAFile = true;
                    //-------------------------//

                    if (PrntProgramsParametersToAFile == true) PrintProgramsParametersToAFile();
                    if (PrntRouteAndCrewMemberDataToConsole == true) PrintRouteAndCrewMemberDataToConsole();
                    if (PrntRouteDataToAFile == true) PrintRouteDataToAFile();
                    if (PrntCrewMemberDataToAFile == true) PrintCrewMemberDataToAFile();


                    void GenerateRouteData()
                    {
                        int i, random_number, j, temp;
                        int index = 0;
                        int Hours = Days * 24, MinsPerDay = 24 * 60;
                        double min;
                        int MinRouteDuration = InputData.ColgenSettings.MinRouteDuration;
                        int MaxRouteDuration = InputData.ColgenSettings.MinRouteDuration;
                        double MaxDouble = InputData.ColgenSettings.MaxDouble;

                        // Initialize the arrays
                        for (i = 0; i <= R - 1; i++)
                        {
                            RouteDepartDay[i] = 0;
                            RouteArrivalDay[i] = 0;
                            RouteDepartTime[i] = 0;
                            RouteArrivalTime[i] = 0;
                            RouteFlightHours[i] = 0;
                        }

                        // Generate random values
                        for (i = 0; i <= R - 1; i++)
                        {
                            random_number = new Random().Next(0, Days); // random number between 0 and Days
                            RouteDepartDay[i] = random_number;

                            random_number = new Random().Next(0, MinsPerDay); // random number between 0 and MinsPerDay
                            random_number = random_number - (random_number % 10);
                            RouteDepartTime[i] = (RouteDepartDay[i] * MinsPerDay) + random_number;
                            random_number = new Random().Next(MinRouteDuration, MaxRouteDuration + 1); // random between MinRouteDuration and MaxRouteDuration
                            RouteFlightHours[i] = random_number;

                            while (RouteDepartTime[i] + (RouteFlightHours[i] * 60) > Days * MinsPerDay)
                            {
                                RouteDepartTime[i] = RouteDepartTime[i] - MaxRouteDuration;
                                random_number = new Random().Next(MinRouteDuration, MaxRouteDuration + 1);
                                RouteFlightHours[i] = random_number;
                            }

                            RouteArrivalTime[i] = RouteDepartTime[i] + RouteFlightHours[i] * 60;

                            RouteArrivalDay[i] = RouteArrivalTime[i] / MinsPerDay;
                        }

                        // Sort arrays
                        for (i = 0; i <= R - 1; i++)
                        {
                            min = MaxDouble;
                            for (j = i; j <= R - 1; j++)
                            {
                                if (RouteDepartTime[j] <= min)
                                {
                                    min = RouteDepartTime[j];
                                    index = j;
                                }
                            }

                            // Swaps
                            temp = RouteDepartTime[i];
                            RouteDepartTime[i] = RouteDepartTime[index];
                            RouteDepartTime[index] = temp;

                            temp = RouteArrivalTime[i];
                            RouteArrivalTime[i] = RouteArrivalTime[index];
                            RouteArrivalTime[index] = temp;

                            temp = RouteDepartDay[i];
                            RouteDepartDay[i] = RouteDepartDay[index];
                            RouteDepartDay[index] = temp;

                            temp = RouteArrivalDay[i];
                            RouteArrivalDay[i] = RouteArrivalDay[index];
                            RouteArrivalDay[index] = temp;

                            temp = RouteFlightHours[i];
                            RouteFlightHours[i] = RouteFlightHours[index];
                            RouteFlightHours[index] = temp;
                        }
                    }

                    void GenerateCrewMemberData()
                    {
                        int i, AverageFlHrs, SumRtFlHrs = 0, AverageFlHrsMin, AverageFlHrsMax, random_number;
                        int CMBoundDiffAver = InputData.ColgenSettings.CMBoundDiffAver;
                        int MaxFlightHoursCM = InputData.ColgenSettings.MaxFlightHoursCM;
                        int WindowRangeCM = InputData.ColgenSettings.WindowRangeCM;

                        // Calculate total flight hours of all routes
                        for (i = 0; i <= R - 1; i++)
                        {
                            SumRtFlHrs = SumRtFlHrs + RouteFlightHours[i];
                        }

                        AverageFlHrs = SumRtFlHrs / N;
                        AverageFlHrsMin = AverageFlHrs - CMBoundDiffAver;
                        AverageFlHrsMax = AverageFlHrs + CMBoundDiffAver;

                        // If there are too many flight hours for this number of crew members, then put an upper limit to the maximum hours each crew member can fly
                        if (AverageFlHrsMax > MaxFlightHoursCM)
                        {
                            AverageFlHrsMax = MaxFlightHoursCM;
                            AverageFlHrsMin = MaxFlightHoursCM - CMBoundDiffAver;
                        }

                        for (i = 0; i <= N - 1; i++) // for each crew member
                        {
                            random_number = new Random().Next(AverageFlHrsMin, AverageFlHrsMax + 1);
                            CrewMemberFlightHoursLB[i] = random_number - WindowRangeCM;
                            CrewMemberFlightHoursUB[i] = random_number + WindowRangeCM;
                        }
                    }

                    void PrintRouteAndCrewMemberDataToConsole()
                    {
                        // Prints Crew Member Data
                        Console.WriteLine("\n|-- Crew Member Data");
                        for (int i = 0; i <= N - 1; i++)
                        {
                            Console.WriteLine($"Crew Member {i} :  LowerBound = {CrewMemberFlightHoursLB[i]} , UpperBound = {CrewMemberFlightHoursUB[i]}");
                        }

                        // Prints Route Data
                        Console.WriteLine("\n\n|-- Route Data");
                        for (int i = 0; i <= R - 1; i++)
                        {
                            Console.WriteLine($"Route {i} : DepartureDay = {RouteDepartDay[i]} - ArrivalDay = {RouteArrivalDay[i]} , DepartureTime = {RouteDepartTime[i]} - ArrivalTime = {RouteArrivalTime[i]} , FlightHours = {RouteFlightHours[i]}");
                        }

                        Console.WriteLine(); // Add a final newline for better formatting
                    }

                    void PrintProgramsParametersToAFile()
                    {
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(Path.Combine(relativePath, "ProgramParameters_CrewScheduling.lp")))
                            {
                                writer.WriteLine("                                                      |-----------------  PROGRAM PARAMETERS  -----------------|");

                                writer.WriteLine("\n|--- Instance Parameters  \n");
                                writer.WriteLine($"|- N = {N},   Number of crew members of the same rank");
                                writer.WriteLine($"|- R = {R},   Number of flight routes to be covered in the planning horizon");
                                writer.WriteLine($"|- Days = {Days}, Number of days in planning horizon \n");

                                writer.WriteLine("\n|--- Data Generation Parameters  \n");
                                writer.WriteLine($"|- MinRouteDuration = {InputData.ColgenSettings.MinRouteDuration}, Minimum number of flight hours of a route");
                                writer.WriteLine($"|- MaxRouteDuration = {InputData.ColgenSettings.MaxRouteDuration}, Maximum number of flight hours of a route");
                                writer.WriteLine($"|- MaxFlightHoursCM = {InputData.ColgenSettings.MaxFlightHoursCM}, Maximum number of flight hours in the planning horizon that a crew member could take without a penalty [ It is an upper limit to the upper bounds of the crew members' flight hours windows]");
                                writer.WriteLine($"|- WindowRangeCM    = {InputData.ColgenSettings.WindowRangeCM}, The range of the flight hour window of a crew member");
                                writer.WriteLine($"|- CMBoundDiffAver  = {InputData.ColgenSettings.CMBoundDiffAver}, By subtracting/adding this parameter from/to the average flight hours per crew member we get a lower/upper bound for the flight hours of the crew member \n");

                                writer.WriteLine("\n|--- Roster Legality Rules' Parameters  \n");
                                writer.WriteLine($"|- DaysOff               = {InputData.ColgenSettings.DaysOff}, Minimum days in the planning horizon (month) that a crew member is resting (does not have duty)");
                                writer.WriteLine($"|- MinHoursBetween       = {InputData.ColgenSettings.MinHoursBetween}, Minimum hours of break between consecutive duties in order to be considered a day off");
                                writer.WriteLine($"|- LatestArrivalTime     = {InputData.ColgenSettings.LatestArrivalTime}, Latest arrival time of the first of two consecutive duties so that the next day of the first duty can be considered a day off [ 23 : 00 ]");
                                writer.WriteLine($"|- EarliestDepartureTime = {InputData.ColgenSettings.EarliestDepartureTime}, Earliest departure time of the second of two consecutive duties so that the previous day of the second duty can be considered a day off [ 06 : 00 ]");
                                writer.WriteLine($"|- XHoursBreak           = {InputData.ColgenSettings.XHoursBreak}, Rule XHoursBreak / YFlightHours");
                                writer.WriteLine($"|- YFlightHours          = {InputData.ColgenSettings.YFlightHours}, Rule XHoursBreak / YFlightHours demands that a crew member has X hours of break without a flight in between, each time he/she completes Y hours of flight");
                                writer.WriteLine($"|- MaximumFlightHours    = {InputData.ColgenSettings.MaximumFlightHours}, Maximum sum flight hours that a crew member is allowed to fly in the planning horizon\n");

                                writer.WriteLine("\n|--- Coverage and Balance Cost Parameters \n");
                                writer.WriteLine($"|- UndercoverCost  = {InputData.ColgenSettings.UndercoverCost}, Cost for not covering a flight leg");
                                writer.WriteLine($"|- HourPenalty     = {InputData.ColgenSettings.HourPenalty}, Cost per hour violation of the time window of a crew member \n");

                                writer.WriteLine("\n|--- Numeric Parameters  \n");
                                writer.WriteLine($"|- MaxDouble = {InputData.ColgenSettings.MaxDouble} , A very large double number ");
                                writer.WriteLine($"|- eps = {InputData.ColgenSettings.Eps} , A very small double number \n");

                                writer.WriteLine("\n|--- Column Generation Performance Parameters \n");
                                writer.WriteLine($"|- buckets                       = {InputData.ColgenSettings.Buckets}, Number of buckets of each Route node");
                                writer.WriteLine($"|- fictbuckets                   = {InputData.ColgenSettings.FictBuckets}, Number of buckets of Fictitious node [Define this between 10 and 50]");
                                writer.WriteLine($"|- ReducedCostCut                = {InputData.ColgenSettings.ReducedCostCut}, If the Reduced Cost of a candidate roster is less than the ReducedCostCut parameter then it is added to Master");
                                writer.WriteLine($"|- NIDRAI                        = {InputData.ColgenSettings.NIDRAI}, Percentage of the total IDs for whom at least one roster was added to Master in a single column generation iteration, before Master is solved and Duals are updated [Define this between 0.01 and 1]");
                                writer.WriteLine($"|- UpdateDualsManuallyScheme     = {InputData.ColgenSettings.UpdateDualsManuallyScheme}, If the user wishes to utilize the manual update of the duals scheme, set this value to 1. Otherwise, set it to 0 (duals update by solving the master with CPLEX each time all the variables for a crew member are inserted to master)\n");

                                writer.WriteLine("\n|--- Branching Parameters \n");
                                writer.WriteLine($"|- AbsBacktrack               = {InputData.ColgenSettings.AbsBacktrack}, Minimum absolute difference between the objective value of a master problem of a TreeNode (during the branching process) and the optimal objective value of the lp problem for which the flow backtracks. If the difference is less than the parameter, the flow does not backtrack");
                                writer.WriteLine($"|- PerceBacktrack             = {InputData.ColgenSettings.PerceBacktrack}, Minimum percentage difference between the objective value of a master problem of a TreeNode (during the branching process) and the optimal objective value of the lp problem for which the flow backtracks. If the difference is less than the parameter, the flow does not backtrack");
                                writer.WriteLine($"|- AbsMIP                     = {InputData.ColgenSettings.AbsMIP}, Minimum absolute difference between the optimal objective value of an integer solution and the optimal objective value of the lp problem for which the program continues to find a better integer solution. If the difference is less than the parameter, the program terminates");
                                writer.WriteLine($"|- PerceMIP                   = {InputData.ColgenSettings.PerceMIP}, Minimum percentage difference between the optimal objective value of an integer solution and the optimal objective value of the lp problem for which the program continues to find a better integer solution. If the difference is less than the parameter, the program terminates");
                                writer.WriteLine($"|- NumberOfBacktracksLimit    = {InputData.ColgenSettings.NumberOfBacktracksLimit}, Maximum number of backtracks allowed, after an integer solution has been found");
                                writer.WriteLine($"|- NumberOfTreeNodesLimit     = {InputData.ColgenSettings.NumberOfTreeNodesLimit}, Maximum number of TreeNodes created allowed, after an integer solution has been found");
                                writer.WriteLine($"|- TreelistSortingIdsFixed    = {InputData.ColgenSettings.TreelistSortingIdsFixed}, Minimum absolute difference between the MasterObjVals of two TreeNodes in the Treelist for which it is checked which of the two has more Ids fixed and the one with the most fixed is put in the front, in MIPGap loop, after an integer solution is found\n");
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine($"Error creating data file: {ex.Message}");
                        }
                    }

                    void PrintRouteDataToAFile()
                    {
                        try
                        {


                            // Create or open the file for writing
                            using (StreamWriter writer = new StreamWriter(Path.Combine(relativePath, "RouteData_CrewScheduling.lp")))
                            {
                                writer.WriteLine("                                                      |-----------------  ROUTE DATA  -----------------|");
                                writer.WriteLine();
                                writer.WriteLine($"|--- Number of Routes = {R}");
                                writer.WriteLine();

                                // Write route data
                                for (int i = 0; i <= R - 1; i++)
                                {
                                    writer.WriteLine($"|- RT = {i} :   DepartureDay = {RouteDepartDay[i]} - ArrivalDay = {RouteArrivalDay[i]} ,  DepartureTime = {RouteDepartTime[i]} - ArrivalTime = {RouteArrivalTime[i]} ,  FlightHours = {RouteFlightHours[i]}");
                                }
                            }

                            Console.WriteLine("Route data written to RouteData_CrewScheduling.lp successfully.");
                        }
                        catch (System.Exception ex)
                        {
                            // Handle any errors that occur during file creation or writing
                            Console.WriteLine($"Error creating data file: {ex.Message}");
                            Console.ReadLine();
                            Environment.Exit(0);
                        }
                    }

                    void PrintCrewMemberDataToAFile()
                    {
                        try
                        {


                            // Create or open the file for writing
                            using (StreamWriter writer = new StreamWriter(Path.Combine(relativePath, "CrewMemberData_CrewScheduling.lp")))

                            {
                                writer.WriteLine("                                                      |-----------------  CREW MEMBER DATA  -----------------|");
                                writer.WriteLine();
                                writer.WriteLine($"|--- Number of Crew Members = {N}");
                                writer.WriteLine();

                                // Write crew member data
                                for (int i = 0; i <= N - 1; i++)
                                {
                                    writer.WriteLine($"|- Crew Member = {i} :   LB = {CrewMemberFlightHoursLB[i]} ,   UB = {CrewMemberFlightHoursUB[i]}");
                                }
                            }

                            Console.WriteLine("Crew member data written to CrewMemberData_CrewScheduling.lp successfully.");
                        }
                        catch (System.Exception ex)
                        {
                            // Handle any errors that occur during file creation or writing
                            Console.WriteLine($"Error creating data file: {ex.Message}");
                            Console.ReadLine();
                            Environment.Exit(0);
                        }
                    }


                    #endregion

                    #region Optimize 


                    //---| Settings 
                    //Print Options
                    bool PrntLPSolutionToConsole = false;
                    bool PrntInitialMasterToAFile = true, PrntLPSolutionToAFile = false;
                    bool PrntInitialNetworkListToConsole = false;
                    bool PrntALLIntProblemsToAFile = false;
                    //-------------------------//

                    OutputData = new CSOutputData();
                    InputData.CSType = BasicEnums.CSType.Set_Covering;

                    if (InputData.CSType == BasicEnums.CSType.Set_Covering)//Change To SetCOVER
                    {
                        // Set Cover
                        if (!Line1_SetCoverFlow_Gb(PrntLPSolutionToConsole, PrntInitialMasterToAFile, PrntLPSolutionToAFile, PrntInitialNetworkListToConsole, PrntALLIntProblemsToAFile, InputData))
                        {
                            Console.WriteLine("\n\n\n Line1_SetCoverFlow: A problem occurred while trying to solve the problem... \n Press any key to exit... \n");
                            //Console.ReadKey();

                        }
                    }
                    else if (InputData.CSType == BasicEnums.CSType.Set_Partition)
                    {
                        //// Set Partition
                        //if (!Line2_SetPartitionFlow(PrntLPSolutionToConsole, PrntInitialMasterToAFile, PrntLPSolutionToAFile, PrntInitialNetworkListToConsole, PrntALLIntProblemsToAFile))
                        //{
                        //    Console.WriteLine("\n\n\n Line2_SetPartitionFlow: A problem occurred while trying to solve the problem... \n Press any key to exit... \n");
                        //    Console.ReadKey();
                        //    Environment.Exit(0);
                        //}
                    }
                    else if (InputData.CSType == BasicEnums.CSType.Hybrid)
                    {
                        //// Hybrid Flow
                        //if (!Line3_HybridFlow(PrntLPSolutionToConsole, PrntInitialMasterToAFile, PrntLPSolutionToAFile, PrntInitialNetworkListToConsole, PrntALLIntProblemsToAFile))
                        //{
                        //    Console.WriteLine("\n\n\n Line3_HybridFlow: A problem occurred while trying to solve the problem... \n Press any key to exit... \n");
                        //    Console.ReadKey();
                        //    Environment.Exit(0);
                        //}
                    }



                    #endregion
                }

            }
            catch (GRBException ex)
            {
                dsp_grb_error("Line1_SetCoverFlow_Gb", ex);
                Gurobi_LogError(ex, "Line1_SetCoverFlow_Gb");
            }
            catch (System.Exception ex)
            {
                System_LogError(ex, "Line1_SetCoverFlow_Gb", "Notes");
            }

        }

        #region Gurobi

        #region SetCover,SetPartition, Hybrid
        public bool Line1_SetCoverFlow_Gb(bool PrntLPSolutionToConsole, bool PrntInitialMasterToAFile, bool PrntLPSolutionToAFile, bool PrntInitialNetworkListToConsole, bool PrntALLIntProblemsToAFile, CSInputData InputData)
        {
            try
            {
                // Variable Declaration
                int start;
                bool SetCover = true, SCtoSPv = false;
                TreeNode BestIntegerSolNodeCarrier = null;

                // Create a TreeNode
                CreateTreeNode_Gb(ref BestIntegerSolNodeCarrier);

                // Record the start time
                start = Environment.TickCount;

                // Procedure
                if (!CreateMaster_Gb(PrntInitialMasterToAFile, SetCover, SCtoSPv, InputData))
                    return false;

                //if (!LPCoverage(PrntInitialNetworkListToConsole, SetCover, start))
                //    return false;

                //if (!IPCoverage(ref BestIntegerSolNodeCarrier, SetCover, start, PrntALLIntProblemsToAFile))
                //    return false;

                //// Turn the Set Cover into a Set Partition Solution
                //SCtoSPv = true;
                //CreateAndInitialize_AuxList();

                //if (!SetCover_To_SetPartition_Conversion(ref BestIntegerSolNodeCarrier, SCtoSPv, start))
                //    return false;

                //// Free memory
                //FreeTreeList();
                //FreeTreeNode(ref BestIntegerSolNodeCarrier, false);

                //if (!Free_CPLEXEnvironment(ref MasterEnv))
                //    return false;

                //if (Line == 1)
                //{
                //    FreeAuxList();

                //    if (!FreeCPLEXOptimizationProblem_FreeCPLEXEnvironment(ref SCtoSPEnv, ref SCtoSP))
                //        return false;
                //}

                return true;
            }
            catch (GRBException ex)
            {
                dsp_grb_error("Line1_SetCoverFlow_Gb", ex);
                Gurobi_LogError(ex, "Line1_SetCoverFlow_Gb");
                return false;
            }
            catch (System.Exception ex)
            {
                System_LogError(ex, "Line1_SetCoverFlow_Gb", "Notes");
                return false;
            }

        }

        public void CreateTreeNode_Gb(ref TreeNode node)
        {
            try
            {
                // Initialize a new TreeNode
                node = new TreeNode(N + R)
                {
                    Lp = null,
                    CGPerformed = false,
                    UId = 1,
                    MasterObjVal = MaxDouble,
                    TimesToCover = new int[N + R], // Create and initialize the timestocover array
                    NetworkList = null,
                    NumberOfNetworkRouteNodes = R + 1,
                    RightNode = null
                };

                // Initialize the timestocover array to 1
                for (int i = 0; i < N + R; i++)
                {
                    node.TimesToCover[i] = 1;
                }
            }
            catch (System.Exception ex)
            {
                System_LogError(ex, "CreateTreeNode_Gb", "Notes");
            }

        }

        public bool CreateMaster_Gb(bool PrntInitialMasterToAFile, bool SetCover, bool SCtoSPv, CSInputData InputData)
        {
            try
            {
                // Initialize CPLEX environment
                if (!Initialize_GurobiEnvironment(out MasterEnv_Gb)) return false;

                // Initialize CPLEX optimization problem
                if (!Initialize_GurobiOptimizationProblem(MasterEnv_Gb, out MasterLp_Gb, "InitialMasterProblem")) return false;

                // Create crew member constraints
                if (!CreateCrewMemberConstraints_Gb(MasterEnv_Gb, MasterLp_Gb)) return false;

                // Create route constraints
                if (!CreateRouteConstraints_Gb(MasterEnv_Gb, MasterLp_Gb, SetCover)) return false;

                // Create undercover variables
                if (!CreateUndercoverVariables_Gb(MasterEnv_Gb, MasterLp_Gb, SCtoSPv)) return false;

                var a = 1;

                // Handle Line 3 specific logic
                if (InputData.CSType == BasicEnums.CSType.Hybrid && !SetCover)
                {
                    //if (!Line3_CreateInitRosters(MasterEnv_Gb, MasterLp_Gb)) return false;
                }
                else
                {
                    if (!CreateEmptyRosters_Gb(MasterEnv_Gb, MasterLp_Gb, SetCover, SCtoSPv)) return false;
                }

                // Print the initial master problem to a file if requested
                if (PrntInitialMasterToAFile)
                {
                    if (SetCover)
                    {
                        if (!PrintProblemToFile_Gb(MasterEnv_Gb, MasterLp_Gb, "SetCover_InitialMasterProblem.lp")) return false;
                    }
                    else
                    {
                        if (!PrintProblemToFile_Gb(MasterEnv_Gb, MasterLp_Gb, "SetPartition_InitialMasterProblem.lp")) return false;
                    }
                }

                return true;
            }
            catch (GRBException ex)
            {
                dsp_grb_error("Line1_SetCoverFlow_Gb", ex);
                Gurobi_LogError(ex, "Line1_SetCoverFlow_Gb");
                return false;
            }
            catch (System.Exception ex)
            {
                System_LogError(ex, "CreateMaster_Gb", "Notes");
                return false;

            }

        }

        #endregion

        #region GurobiLib
        void dsp_grb_error(string name, GRBException exception)
        {
            Console.WriteLine($"{name} failed...");
            Console.WriteLine($"   Error code = {exception.ErrorCode}");
            Console.WriteLine($"   Message = {exception.Message}");
            Console.WriteLine("\n Press any key to continue...");
            Console.Read();
        }

        public bool Initialize_GurobiEnvironment(out GRBEnv env)
        {
            env = null;
            try
            {
                env = new GRBEnv();
                return true;
            }
            catch (GRBException ex)
            {
                dsp_grb_error("Initialize_GurobiEnvironment", ex);
                Gurobi_LogError(ex, "Initialize_GurobiEnvironment");
                return false;
            }

        }

        public bool Initialize_GurobiOptimizationProblem(GRBEnv env, out GRBModel lp, string name)
        {
            lp = null;
            try
            {
                lp = new GRBModel(env) { ModelName = name };
                return true;
            }
            catch (GRBException ex)
            {
                dsp_grb_error("Initialize_GurobiOptimizationProblem", ex);
                Gurobi_LogError(ex, "Initialize_GurobiOptimizationProblem");
                return false;
            }
        }

        public bool NewRows_Gb(GRBEnv env, GRBModel lp, int rcnt, double[] rhs, char[] sense, string[] rowname)
        {
            try
            {
                for (int i = 0; i < rcnt; i++)
                {
                    // Create a linear expression for the constraint (initially empty)
                    GRBLinExpr expr = new GRBLinExpr();

                    // Placeholder: Add terms to the linear expression if required
                    // Example: expr.AddTerm(coefficient, variable); Modify this based on your use case

                    // Add the constraint to the model
                    lp.AddConstr(expr, sense[i], rhs[i], rowname[i]);
                }

                return true;
            }
            catch (GRBException ex)
            {
                Console.WriteLine($"Gurobi error: {ex.Message}");
                Gurobi_LogError(ex, "NewRows_Gb");

                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                System_LogError(ex, "NewRows_Gb");

                return false;
            }
        }

        // Fixed CreateCrewMemberConstraints_Gb
        public bool CreateCrewMemberConstraints_Gb(GRBEnv env, GRBModel lp)
        {
            try
            {
                double[] rhs = { 1.0 }; // Define as an array
                char[] sense = { 'E' }; // Define as an array

                for (int i = 0; i < N; i++)
                {
                    string[] rowName = { $"CrewMember{i}" }; // Define as an array
                    if (!NewRows_Gb(env, lp, 1, rhs, sense, rowName)) return false;
                }

                return true;
            }
            catch (GRBException ex)
            {
                dsp_grb_error("CreateCrewMemberConstraints_Gb", ex);
                Console.WriteLine($"Gurobi error: {ex.Message}");
                Gurobi_LogError(ex, "CreateCrewMemberConstraints_Gb");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                System_LogError(ex, "CreateCrewMemberConstraints_Gb");

                return false;
            }
        }

        public bool CreateRouteConstraints_Gb(GRBEnv env, GRBModel lp, bool SetCover)
        {
            try
            {
                double[] rhs = { 1.0 }; // Define the RHS as an array
                char[] sense = { SetCover ? GRB.GREATER_EQUAL : GRB.EQUAL }; // Use Gurobi constraint senses as an array

                for (int i = 0; i < R; i++)
                {
                    string[] rowName = { $"Route{i}" }; // Define the row name as an array

                    // Add constraints using the NewRows_Gb function
                    if (!NewRows_Gb(env, lp, 1, rhs, sense, rowName))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (GRBException ex)
            {
                dsp_grb_error("CreateRouteConstraints_Gb", ex);
                Console.WriteLine($"Gurobi error: {ex.Message}");
                Gurobi_LogError(ex, "CreateRouteConstraints_Gb");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                System_LogError(ex, "CreateRouteConstraints_Gb");

                return false;
            }
        }

        public bool CreateEmptyRosters_Gb(GRBEnv env, GRBModel lp, bool SetCover, bool SCtoSPv)
        {
            try
            {
                double lb = 0.0;
                double ub;
                char xctype;

                // Determine variable type and upper bound based on SCtoSPv
                if (SCtoSPv == false)
                {
                    ub = double.MaxValue; // Equivalent to CPX_INFBOUND
                    xctype = 'C'; // Continuous variable
                }
                else
                {
                    ub = 1.0;
                    xctype = 'B'; // Binary variable
                }

                for (int i = 0; i < N; i++)
                {
                    // Set the objective coefficient based on SetCover
                    double obj = SetCover ? 0.0 : CrewMemberFlightHoursLB[i] * HourPenalty;

                    // Add a new column (variable) to the model
                    if (!NewColumns(env, lp, 1, new double[] { obj }, new double[] { lb }, new double[] { ub }, new char[] { xctype }, null))
                        return false;

                    // Get the number of columns (variables) in the model
                    int numcol = GetNumberOfColumns(env, lp);
                    if (numcol == 0) return false;

                    // Update the coefficient of the new variable in the current constraint
                    if (!ChangeConstraintCoefficient(env, lp, i, numcol - 1, 1.0))
                        return false;
                }

                return true; // Indicate success
            }
            catch (GRBException ex)
            {
                dsp_grb_error("CreateEmptyRosters_Gb", ex);
                Console.WriteLine($"Gurobi error: {ex.Message}");
                Gurobi_LogError(ex, "CreateEmptyRosters_Gb");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                System_LogError(ex, "CreateEmptyRosters_Gb");

                return false;
            }
        }

        public bool CreateUndercoverVariables_Gb(GRBEnv env, GRBModel lp, bool SCtoSPv)
        {
            try
            {
                double obj = UndercoverCost; // Objective coefficient
                double lb = 0.0; // Lower bound
                double ub; // Upper bound
                char xctype; // Variable type

                // Determine upper bound and variable type based on SCtoSPv
                if (SCtoSPv == false)
                {
                    ub = double.MaxValue; // Equivalent to CPX_INFBOUND
                    xctype = 'C'; // Continuous variable
                }
                else
                {
                    ub = 1.0;
                    xctype = 'B'; // Binary variable
                }

                // Loop through R variables
                for (int i = 0; i < R; i++)
                {
                    // Add a new variable (column) to the model
                    if (!NewColumns(env, lp, 1, new double[] { obj }, new double[] { lb }, new double[] { ub }, new char[] { xctype }, null))
                        return false;

                    // Get the current number of columns (variables) in the model
                    int numcol = GetNumberOfColumns(env, lp);
                    if (numcol == 0) return false;

                    // Change the coefficient of the variable in the constraint
                    if (!ChangeConstraintCoefficient(env, lp, N + i, numcol - 1, 1.0))
                        return false;
                }

                return true; // Indicate success
            }
            catch (GRBException ex)
            {
                dsp_grb_error("CreateUndercoverVariables_Gb", ex);
                Console.WriteLine($"Gurobi error: {ex.Message}");
                Gurobi_LogError(ex, "CreateUndercoverVariables_Gb");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                System_LogError(ex, "CreateUndercoverVariables_Gb");

                return false;
            }
        }

        public bool NewColumns(GRBEnv env, GRBModel lp, int ccnt, double[] obj, double[] lb, double[] ub, char[] xctype, string[] colname)
        {
            try
            {
                for (int i = 0; i < ccnt; i++)
                {
                    // Add the new variable to the model
                    GRBVar newVar = lp.AddVar(
                        lb[i],               // Lower bound
                        ub[i],               // Upper bound
                        obj[i],              // Objective coefficient
                        xctype[i] == 'C' ? GRB.CONTINUOUS : GRB.BINARY, // Variable type
                        colname != null ? colname[i] : null             // Column name
                    );
                }

                return true;
            }
            catch (GRBException ex)
            {
                dsp_grb_error("NewColumns", ex);
                Console.WriteLine($"Gurobi error: {ex.Message}");
                Gurobi_LogError(ex, "NewColumns");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                System_LogError(ex, "NewColumns");

                return false;
            }
        }

        public int GetNumberOfColumns(GRBEnv env, GRBModel lp)
        {
            try
            {
                // Get the number of variables (columns) in the model
                int numColumns = lp.NumVars;

                return numColumns;
            }
            catch (GRBException ex)
            {
                dsp_grb_error("GetNumberOfColumns", ex);
                Console.WriteLine($"Gurobi error: {ex.Message}");
                Gurobi_LogError(ex, "GetNumberOfColumns");

                return -1;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                System_LogError(ex, "GetNumberOfColumns");

                return -1;
            }
        }

        public int GetNumberOfRows(GRBEnv env, GRBModel lp)
        {
            try
            {
                // Get the number of constraints (rows) in the model
                int numRows = lp.NumConstrs;

                return numRows;
            }
            catch (GRBException ex)
            {
                dsp_grb_error("GetNumberOfRows", ex);
                Console.WriteLine($"Gurobi error: {ex.Message}");
                Gurobi_LogError(ex, "GetNumberOfRows");
                return -1;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                System_LogError(ex, "GetNumberOfRows");

                return -1;
            }
        }

        public bool ChangeConstraintCoefficient(GRBEnv env, GRBModel lp, int rowIndex, int colIndex, double newCoef)
        {
            try
            {
                // Get the constraint (row) by index
                GRBConstr constr = lp.GetConstrs()[rowIndex];

                // Get the variable (column) by index
                GRBVar variable = lp.GetVars()[colIndex];

                // Change the coefficient of the variable in the constraint
                lp.ChgCoeff(constr, variable, newCoef);

                return true; // Indicate success
            }
            catch (GRBException ex)
            {
                dsp_grb_error("ChangeConstraintCoefficient", ex);
                Console.WriteLine($"Gurobi error: {ex.Message}");
                Gurobi_LogError(ex, "ChangeConstraintCoefficient");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                System_LogError(ex, "ChangeConstraintCoefficient");

                return false;
            }
        }

        public bool PrintProblemToFile_Gb(GRBEnv env, GRBModel lp, string filename)
        {

            string relativePath = Path.Combine("Colgen", "Optimization", filename);
            try
            {
                lp.Write(relativePath);
                return true;
            }
            catch (GRBException ex)
            {
                Console.WriteLine($"Error writing model to file: {ex.Message}");
                return false;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error writing model to file: {ex.Message}");
                System_LogError(ex, "PrintProblemToFile_Gb");

                return false;
            }

        }

        #endregion
        #endregion

        #region CPLEX

        #region SetCover,SetPartition, Hybrid
        //public bool Line1_SetCoverFlow(bool PrntLPSolutionToConsole, bool PrntInitialMasterToAFile, bool PrntLPSolutionToAFile, bool PrntInitialNetworkListToConsole, bool PrntALLIntProblemsToAFile, CSInputData InputData)
        //{
        //    // Variable Declaration
        //    int start;
        //    bool SetCover = true, SCtoSPv = false;
        //    TreeNode BestIntegerSolNodeCarrier = null;

        //    // Create a TreeNode
        //    CreateTreeNode(ref BestIntegerSolNodeCarrier);

        //    // Record the start time
        //    start = Environment.TickCount;

        //    // Procedure
        //    if (!CreateMaster(PrntInitialMasterToAFile, SetCover, SCtoSPv,InputData))
        //        return false;

        //    if (!LPCoverage(PrntInitialNetworkListToConsole, SetCover, start))
        //        return false;

        //    if (!IPCoverage(ref BestIntegerSolNodeCarrier, SetCover, start, PrntALLIntProblemsToAFile))
        //        return false;

        //    // Turn the Set Cover into a Set Partition Solution
        //    SCtoSPv = true;
        //    CreateAndInitialize_AuxList();

        //    if (!SetCover_To_SetPartition_Conversion(ref BestIntegerSolNodeCarrier, SCtoSPv, start))
        //        return false;

        //    // Free memory
        //    FreeTreeList();
        //    FreeTreeNode(ref BestIntegerSolNodeCarrier, false);

        //    if (!Free_CPLEXEnvironment(ref MasterEnv))
        //        return false;

        //    if (Line == 1)
        //    {
        //        FreeAuxList();

        //        if (!FreeCPLEXOptimizationProblem_FreeCPLEXEnvironment(ref SCtoSPEnv, ref SCtoSP))
        //            return false;
        //    }

        //    return true;
        //}

        //public void CreateTreeNode(ref TreeNode node)
        //{
        //    // Initialize a new TreeNode
        //    node = new TreeNode(N+R)
        //    {
        //        Lp = null,
        //        CGPerformed = false,
        //        UId = 1,
        //        MasterObjVal = MaxDouble,
        //        TimesToCover = new int[N + R], // Create and initialize the timestocover array
        //        NetworkList = null,
        //        NumberOfNetworkRouteNodes = R + 1,
        //        RightNode = null
        //    };

        //    // Initialize the timestocover array to 1
        //    for (int i = 0; i < N + R; i++)
        //    {
        //        node.TimesToCover[i] = 1;
        //    }
        //}

        //public bool CreateMaster(bool PrntInitialMasterToAFile, bool SetCover, bool SCtoSPv, CSInputData InputData)
        //{
        //    // Initialize CPLEX environment
        //    if (!Initialize_CPLEXEnvironment(ref MasterEnv)) return false;

        //    // Initialize CPLEX optimization problem
        //    if (!Initialize_CPLEXOptimizationProblem(MasterEnv, ref MasterLp, "InitialMasterProblem")) return false;

        //    // Create crew member constraints
        //    if (!CreateCrewMemberConstraints(MasterEnv, MasterLp)) return false;

        //    // Create route constraints
        //    if (!CreateRouteConstraints(MasterEnv, MasterLp, SetCover)) return false;

        //    // Create undercover variables
        //    if (!CreateUndercoverVariables(MasterEnv, MasterLp, SCtoSPv)) return false;

        //    // Handle Line 3 specific logic
        //    if (InputData.CSType == BasicEnums.CSType.Hybrid && !SetCover)
        //    {
        //        if (!Line3_CreateInitRosters(MasterEnv, MasterLp)) return false;
        //    }
        //    else
        //    {
        //        if (!CreateEmptyRosters(MasterEnv, MasterLp, SetCover, SCtoSPv)) return false;
        //    }

        //    // Print the initial master problem to a file if requested
        //    if (PrntInitialMasterToAFile)
        //    {
        //        if (SetCover)
        //        {
        //            if (!PrintProblem_ToFile(MasterEnv, MasterLp, "SetCover_InitialMasterProblem.lp")) return false;
        //        }
        //        else
        //        {
        //            if (!PrintProblem_ToFile(MasterEnv, MasterLp, "SetPartition_InitialMasterProblem.lp")) return false;
        //        }
        //    }

        //    return true;
        //}

        #endregion

        #region CplexLib
        //void dsp_cpx_error(string name, int status)
        //{
        //    Console.WriteLine($"{name} failed...");
        //    Console.WriteLine($"   status = {status}");
        //    Console.WriteLine("\n Press any key to continue...");
        //    Console.ReadKey();
        //}

        //public bool Initialize_CPLEXEnvironment(out IntPtr env) // initializes a CPLEX environment
        //{
        //    int status;
        //    env = CPXopenCPLEX(out status);

        //    if (status != 0) // status check
        //    {
        //        dsp_cpx_error("CPXopenCPLEX", status);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool Initialize_CPLEXOptimizationProblem(IntPtr env, out IntPtr lp, string name) // initializes a CPLEX optimization problem
        //{
        //    int status;
        //    lp = CPXcreateprob(env, out status, name);

        //    if (status != 0) // status check
        //    {
        //        dsp_cpx_error("CPXcreateprob", status);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool CreateCrewMemberConstraints(IntPtr env, IntPtr lp)
        //{
        //    try
        //    {
        //        double rhs = 1.0;
        //        char sense = 'E';

        //        for (int i = 0; i < N; i++)
        //        {
        //            string rowName = $"CrewMember{i}";
        //            if (!NewRows(env, lp, 1, rhs, sense, rowName)) return false;
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false; // Ensure false is returned on any exception
        //    }
        //}

        //public bool CreateRouteConstraints(IntPtr env, IntPtr lp, bool SetCover)
        //{
        //    try
        //    {
        //        double rhs = 1.0;
        //        char sense = SetCover ? 'G' : 'E';

        //        for (int i = 0; i < R; i++)
        //        {
        //            string rowName = $"Route{i}";
        //            if (!NewRows(env, lp, 1, rhs, sense, rowName)) return false;
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false; // Ensure false is returned on any exception
        //    }
        //}

        //public bool CreateEmptyRosters(IntPtr env, IntPtr lp, bool SetCover, bool SCtoSPv)
        //{
        //    try
        //    {
        //        double lb = 0.0;
        //        double ub;
        //        char xctype;

        //        if (!SCtoSPv)
        //        {
        //            ub = double.MaxValue; // Equivalent to CPX_INFBOUND
        //            xctype = 'C'; // Continuous variable
        //        }
        //        else
        //        {
        //            ub = 1.0;
        //            xctype = 'B'; // Binary variable
        //        }

        //        for (int i = 0; i < N; i++)
        //        {
        //            double obj = SetCover ? 0.0 : CrewMemberFlightHoursLB[i] * HourPenalty;

        //            if (!NewColumns(env, lp, 1, obj, lb, ub, xctype, null)) return false;

        //            int numcol = GetNumberOfColumns(env, lp);
        //            if (numcol == 0) return false;

        //            if (!ChangeConstraintCoefficient(env, lp, i, numcol - 1, 1)) return false;
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false; // Ensure false is returned on any exception
        //    }
        //}

        //public bool CreateUndercoverVariables(IntPtr env, IntPtr lp, bool SCtoSPv)
        //{
        //    try
        //    {
        //        double obj = UndercoverCost;
        //        double lb = 0.0;
        //        double ub;
        //        char xctype;

        //        if (!SCtoSPv)
        //        {
        //            ub = double.MaxValue; // Equivalent to CPX_INFBOUND
        //            xctype = 'C'; // Continuous variable
        //        }
        //        else
        //        {
        //            ub = 1.0;
        //            xctype = 'B'; // Binary variable
        //        }

        //        for (int i = 0; i < R; i++)
        //        {
        //            if (!NewColumns(env, lp, 1, obj, lb, ub, xctype, null)) return false;

        //            int numcol = GetNumberOfColumns(env, lp);
        //            if (numcol == 0) return false;

        //            if (!ChangeConstraintCoefficient(env, lp, N + i, numcol - 1, 1)) return false;
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false; // Ensure false is returned on any exception
        //    }
        //}



        //// Actual CPLEX function declarations (using P/Invoke)
        //[DllImport("cplex.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern IntPtr CPXopenCPLEX(out int status);

        //[DllImport("cplex.dll", CallingConvention = CallingConvention.Cdecl)]
        //private static extern IntPtr CPXcreateprob(IntPtr env, out int status, string name);
        #endregion

        #endregion


        public static void System_LogError(System.Exception ex, string methodName, string additionalInfo = "")
        {
            using (var dbContext = new ErpDbContext(ErpDbContext.DbOptions))
            {
                dbContext.Loge.Add(new Log
                {
                    ExceptionType = ex.GetType().ToString(),
                    ExceptionMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source,
                    MethodName = methodName,
                    OccurredAt = DateTime.Now,
                    AdditionalInfo = additionalInfo
                });

                dbContext.SaveChanges();
            }
        }
        public static void Gurobi_LogError(GRBException ex, string methodName, string additionalInfo = "")
        {
            using (var dbContext = new ErpDbContext(ErpDbContext.DbOptions))
            {
                dbContext.Loge.Add(new Log
                {
                    ExceptionType = ex.GetType().ToString(),
                    ExceptionMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source,
                    MethodName = methodName,
                    OccurredAt = DateTime.Now,
                    AdditionalInfo = additionalInfo
                });

                dbContext.SaveChanges();
            }
        }

    }
}
