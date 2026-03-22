using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling
{
    public class InputParamsMainSettingsH
    {
        //---| Select Line
        public int Line { get; set; }

        //---| Instance Parameters
        public int N { get; set; }
        public int R { get; set; }
        public int Days { get; set; }

        //---| Data Generation Parameters
        public bool RandomData { get; set; }
        public int MinRouteDuration { get; set; }
        public int MaxRouteDuration { get; set; }
        public int MaxFlightHoursCM { get; set; }
        public int WindowRangeCM { get; set; }
        public int CMBoundDiffAver { get; set; }
        public int GenderConPercent { get; set; }

        //---| Roster Legality Rules' Parameters
        public int DaysOff { get; set; }
        public int MinHoursBetween { get; set; }
        public int LatestArrivalTime { get; set; }
        public int EarliestDepartureTime { get; set; }
        public int XHoursBreak { get; set; }
        public int YFlightHours { get; set; }
        public int MaximumFlightHours { get; set; }

        //---| Coverage and Balance Cost Parameters
        public int UndercoverCost { get; set; }
        public int HourPenalty { get; set; }
        public int GenderPenalty { get; set; }
        public bool SoftGenderConstraints { get; set; }

        //---| Numeric Parameters
        public double MaxDouble { get; set; }
        public double Eps { get; set; }

        //---| Column Generation Performance Parameters
        public int Buckets { get; set; }
        public int FictBuckets { get; set; }
        public int ReducedCostCut { get; set; }
        public double NIDRAI { get; set; }
        public int UpdateDualsManuallyScheme { get; set; }

        //---| Branching Parameters
        public int AbsBacktrack { get; set; }
        public double PerceBacktrack { get; set; }
        public int AbsMIP { get; set; }
        public double PerceMIP { get; set; }
        public int NumberOfBacktracksLimit { get; set; }
        public int TreelistSortingIdsFixed { get; set; }

        //---| Activation Parameters
        public bool ActivateMinMax { get; set; }
        public bool ActivateWith { get; set; }
        public bool ActivateWithout { get; set; }
        public bool ActivateGender { get; set; }

        //---| Priority Parameters
        public bool WithoutVarPriority { get; set; }
        public bool GenderVarPriority { get; set; }
    }
}
