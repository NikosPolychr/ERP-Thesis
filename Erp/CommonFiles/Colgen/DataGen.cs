using Erp.Model.Colgen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILOG.Concert;
using ILOG.CPLEX;

namespace Erp.CommonFiles.Colgen
{

    public class DataGen
    {
        //public ColgenSettings Settings. = new ColgenSettings();
        //#region Entry Code 
        ////--------------------------------------------------------------------------------------------------------------------------------------//

        //// Performance Settings
        //bool BacktrackOnlyAfterIPFound = false;     // If this is true, the program backtracks only if an infeasible problem was produced or only after one integer solution was found. Else, the program checks Backtrack tolerances even before finding the first integer solution

        //// ------|| Global Variables and Pointers

        //// ---| CPLEX Pointers
        //// Note: In C#, instead of using pointers, we typically use objects or handles for external libraries
        //CPXENVptr MasterEnv = null, SCtoSPEnv = null;  // CPLEX environment pointers
        //CPXLPptr MasterLp = null, SCtoSP = null;      // CPLEX problem pointers

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

        //#endregion
        //void GenerateRouteData()
        //{
        //    int i, random_number, index, j, temp;
        //    int Hours = Days * 24, MinsPerDay = 24 * 60;
        //    double min;

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

    }
}
