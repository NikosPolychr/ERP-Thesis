using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Colgen
{
    public class ColgenSettings : INotifyPropertyChanged
    {
        // Event for property change notifications
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //------|| Program's Parameters

        //---| Select Line
        // Select your preferred Line (design).
        // (Line 1 for Set Cover), (Line 2 for Set Partition), (Line 3 for Hybrid Set Cover - Set Partition)
        private int _line = 1;
        public int Line
        {
            get => _line;
            set { _line = value; OnPropertyChanged(); }
        }

        //---| Instance Parameters 
        // Number of crew members of the same rank
        private int _n = 100;
        public int N
        {
            get => _n;
            set { _n = value; OnPropertyChanged(); }
        }

        // Number of flight routes to be covered in the planning horizon
        private int _r = 400;
        public int R
        {
            get => _r;
            set { _r = value; OnPropertyChanged(); }
        }

        // Number of days in planning horizon
        private int _days = 30;
        public int Days
        {
            get => _days;
            set { _days = value; OnPropertyChanged(); }
        }

        //---| Data Generation Parameters 
        // Minimum number of flight hours of a route
        private int _minRouteDuration = 5;
        public int MinRouteDuration
        {
            get => _minRouteDuration;
            set { _minRouteDuration = value; OnPropertyChanged(); }
        }

        // Maximum number of flight hours of a route
        private int _maxRouteDuration = 30;
        public int MaxRouteDuration
        {
            get => _maxRouteDuration;
            set { _maxRouteDuration = value; OnPropertyChanged(); }
        }

        // Maximum number of flight hours in the planning horizon that a crew member could take without a penalty
        private int _maxFlightHoursCM = 90;
        public int MaxFlightHoursCM
        {
            get => _maxFlightHoursCM;
            set { _maxFlightHoursCM = value; OnPropertyChanged(); }
        }

        // The range of the flight hour window of a crew member
        private int _windowRangeCM = 3;
        public int WindowRangeCM
        {
            get => _windowRangeCM;
            set { _windowRangeCM = value; OnPropertyChanged(); }
        }

        // By subtracting/adding this parameter from/to the average flight hours per crew member,
        // we get a lower/upper bound for the flight hours of the crew member
        private int _cmBoundDiffAver = 30;
        public int CMBoundDiffAver
        {
            get => _cmBoundDiffAver;
            set { _cmBoundDiffAver = value; OnPropertyChanged(); }
        }

        //---| Roster Legality Rules' Parameters
        // Minimum days in the planning horizon (month) that a crew member is resting (does not have duty)
        private int _daysOff = 10;
        public int DaysOff
        {
            get => _daysOff;
            set { _daysOff = value; OnPropertyChanged(); }
        }

        // Minimum hours of break between consecutive duties in order to be considered a day off
        private int _minHoursBetween = 34;
        public int MinHoursBetween
        {
            get => _minHoursBetween;
            set { _minHoursBetween = value; OnPropertyChanged(); }
        }

        // Latest arrival time of the first of two consecutive duties so that the next day of the first duty can be considered a day off
        private int _latestArrivalTime = 23;
        public int LatestArrivalTime
        {
            get => _latestArrivalTime;
            set { _latestArrivalTime = value; OnPropertyChanged(); }
        }

        // Earliest departure time of the second of two consecutive duties so that the previous day of the second duty can be considered a day off
        private int _earliestDepartureTime = 6;
        public int EarliestDepartureTime
        {
            get => _earliestDepartureTime;
            set { _earliestDepartureTime = value; OnPropertyChanged(); }
        }

        // Rule XHoursBreak / YFlightHours: The number of hours a crew member must have as a break without a flight
        private int _xHoursBreak = 34;
        public int XHoursBreak
        {
            get => _xHoursBreak;
            set { _xHoursBreak = value; OnPropertyChanged(); }
        }

        // Rule XHoursBreak / YFlightHours: The total number of flight hours after which the crew member must have a break
        private int _yFlightHours = 68;
        public int YFlightHours
        {
            get => _yFlightHours;
            set { _yFlightHours = value; OnPropertyChanged(); }
        }

        // Maximum sum of flight hours that a crew member is allowed to fly in the planning horizon
        private int _maximumFlightHours = 100;
        public int MaximumFlightHours
        {
            get => _maximumFlightHours;
            set { _maximumFlightHours = value; OnPropertyChanged(); }
        }

        //---| Coverage and Balance Cost Parameters
        // Cost for not covering a flight leg
        private int _undercoverCost = 1000000;
        public int UndercoverCost
        {
            get => _undercoverCost;
            set { _undercoverCost = value; OnPropertyChanged(); }
        }

        // Cost per hour violation of the time window of a crew member
        private int _hourPenalty = 1000;
        public int HourPenalty
        {
            get => _hourPenalty;
            set { _hourPenalty = value; OnPropertyChanged(); }
        }

        //---| Numeric Parameters 
        // A very large double number
        private double _maxDouble = 1000000000000;
        public double MaxDouble
        {
            get => _maxDouble;
            set { _maxDouble = value; OnPropertyChanged(); }
        }

        // A very small number
        private double _eps = 0.001;
        public double Eps
        {
            get => _eps;
            set { _eps = value; OnPropertyChanged(); }
        }

        //---| Column Generation Performance Parameters
        // Number of buckets of each Route node
        private int _buckets = 3;
        public int Buckets
        {
            get => _buckets;
            set { _buckets = value; OnPropertyChanged(); }
        }

        // Number of buckets of Fictitious node [Define this between 10 and 50]
        private int _fictBuckets = 20;
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
        private double _nidrai = 0.3;
        public double NIDRAI
        {
            get => _nidrai;
            set { _nidrai = value; OnPropertyChanged(); }
        }

        // If the user wishes to utilize the manual update of the duals scheme, set this value to 1; otherwise, set to 0
        private int _updateDualsManuallyScheme = 0;
        public int UpdateDualsManuallyScheme
        {
            get => _updateDualsManuallyScheme;
            set { _updateDualsManuallyScheme = value; OnPropertyChanged(); }
        }

        //---| Branching Parameters
        // Minimum absolute difference between the objective value of a master problem and the optimal objective value
        private int _absBacktrack = 900000;
        public int AbsBacktrack
        {
            get => _absBacktrack;
            set { _absBacktrack = value; OnPropertyChanged(); }
        }

        // Minimum percentage difference between the objective value of a master problem and the optimal objective value
        private double _perceBacktrack = 0.2;
        public double PerceBacktrack
        {
            get => _perceBacktrack;
            set { _perceBacktrack = value; OnPropertyChanged(); }
        }

        // Minimum absolute difference between the optimal objective value of an integer solution and the optimal objective value
        private int _absMIP = 5000;
        public int AbsMIP
        {
            get => _absMIP;
            set { _absMIP = value; OnPropertyChanged(); }
        }

        // Minimum percentage difference between the optimal objective value of an integer solution and the optimal objective value
        private double _perceMIP = 0.1;
        public double PerceMIP
        {
            get => _perceMIP;
            set { _perceMIP = value; OnPropertyChanged(); }
        }

        // Maximum number of backtracks allowed after an integer solution has been found
        private int _numberOfBacktracksLimit = 100;
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
        private int _treelistSortingIdsFixed = 10;
        public int TreelistSortingIdsFixed
        {
            get => _treelistSortingIdsFixed;
            set { _treelistSortingIdsFixed = value; OnPropertyChanged(); }
        }
    }
}
