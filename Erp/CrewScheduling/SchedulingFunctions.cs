using Erp.CommonFiles;
using Erp.Model.Enums;
using Erp.Model.Thesis.CrewScheduling;
using Erp.Repositories;
using ILOG.Concert;
using ILOG.CPLEX;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Erp.CrewScheduling
{
    public class SchedulingFunctions : RepositoryBase
    {
        public TreeNode CurrentTreeNode { get; set; }
        public Stopwatch Clock_cs { get; set; }

        #region Create Master Functions

        public void CreateCrewMemberConstraints(CSInputData InputData, Cplex model)
        {

        }
        public void CreateRouteConstraints(CSInputData InputData, Cplex model)
        {

        }
        public void CreateUnderCoverVariables(CSInputData InputData, Cplex model)
        {

        }
        public void CreateMinMaxConstraints(CSInputData InputData, Cplex model)
        {

        }
        public void CreateWithWithoutConstraints(CSInputData InputData,Cplex model)
        {

        }
        public void CreateGenderConstraints(CSInputData InputData, Cplex model)
        {

        }
        #endregion
        public CSOutputData CalculateCrewScheduling_SetPartition_CPLEX(CSInputData InputData)
        {
            #region FilePath Initialization
            string relativePath = Path.Combine("OptimizationResults", "CPLEX", "Thesis", "CS_Set_Partition");
            Directory.CreateDirectory(relativePath);
            CSOutputData Data = new CSOutputData();

            #endregion

            // Save the files
            try
            {
                #region Indexes

                int NumberOfEmps = InputData.I;
                int NumberOfRoutes = InputData.F;
                int NumberOfWithoutCons = 0;
                int NumberOfGenderCons = 0;
                int NumberOfVars = NumberOfEmps + NumberOfRoutes + NumberOfWithoutCons + NumberOfGenderCons;
                int RiMax = InputData.Ri.Values.Max(list => list.Count);

                #endregion

                #region Parameters

                var HourCost = InputData.OptimizerSettingsData.PenaltiesParams.HourPenalty;
                var UnderCoverCost = InputData.OptimizerSettingsData.PenaltiesParams.UnderCoverPenalty;

                #endregion

                //Model Initialization
                Cplex model = new Cplex();

                #region Decision Variables

                INumVar[] X = new INumVar[NumberOfVars];

                for (int i = 0; i < NumberOfVars; i++)
                {
                    string varNameX = $"X{i + 1}";

                    X[i] = model.NumVar(0, 1, NumVarType.Float, varNameX);
                }
                #endregion

                #region Objective Function

                ILinearNumExpr objective = model.LinearNumExpr();

                for (int i = 0; i < NumberOfEmps; i++)
                {
                    var emp = InputData.Employees[i];
                    var HourBalanceCost = HourCost * emp.EmpCrSettings.LowerBound;
                    objective.AddTerm(HourBalanceCost, X[NumberOfRoutes + NumberOfWithoutCons +NumberOfGenderCons + i]);
 
                }
                for (int i = 0; i < NumberOfRoutes; i++)
                {
                    objective.AddTerm(UnderCoverCost, X[i]);
                }

                model.AddMinimize(objective);

                #endregion

                #region Constraints

                //// #13. Con13
                //for (int i = 0; i < I; i++)
                //{
                //    ILinearNumExpr expr = model.LinearNumExpr();
                //    List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();
                //    foreach (int j in RosterPerEmployee_List)
                //    {
                //        expr.AddTerm(1, X[i, j]);

                //    }
                //    model.AddEq(expr, 1, "CON2_" + (i + 1));
                //}

                //// #14. Con14 
                //for (int f = 0; f < F; f++)
                //{
                //    ILinearNumExpr expr = model.LinearNumExpr();
                //    expr.AddTerm(1, Y[f]);
                //    for (int i = 0; i < I; i++)
                //    {
                //        List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                //        foreach (int j in RosterPerEmployee_List)
                //        {
                //            expr.AddTerm(Aijf[(i, j, f)], X[i, j]);
                //        }
                //    }
                //    model.AddEq(expr, Bf[f], "CON3_" + (f + 1));
                //}

                #endregion

                #region Get Results

                // Solve the model
                model.Solve();
                bool solution = (model.GetStatus() == Cplex.Status.Optimal);
                if (solution)
                {
                    Data.ObjValue = model.ObjValue;

                    // Export Model
                    model.ExportModel(Path.Combine(relativePath, "CS_Set_Partition.lp"));
                    model.ExportModel(Path.Combine(relativePath, "CS_Set_Partition.mps"));
                    model.WriteSolution(Path.Combine(relativePath, "CS_Set_Partition.sol"));
                }

                model.End();

                #endregion
            }
            catch (ILOG.Concert.Exception ex)
            {
                Console.WriteLine("A CPLEX error occurred: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("A System error occurred: " + ex.Message);
            }

            return Data;
        }

        public CSOutputData CreateMaster(CSInputData InputData)
        {
            #region FilePath Initialization

            string relativePath = Path.Combine("OptimizationResults", "CPLEX", "Thesis", "CS_Set_Partition");
            Directory.CreateDirectory(relativePath);
            CSOutputData Data = new CSOutputData();

            #endregion
            // Save the files
            try
            {
                #region Indexes

                int I = InputData.I;
                int F = InputData.F;
                int RiMax = InputData.Ri.Values.Max(list => list.Count);

                #endregion

                #region Parameters

                var h = InputData.OptimizerSettingsData.PenaltiesParams.UnderCoverPenalty;
                var c = InputData.OptimizerSettingsData.PenaltiesParams.HourPenalty;
                Dictionary<int, int> Bf = InputData.RoutesCompl_Dict;
                Dictionary<(int, int), double> Cij_Hours = InputData.Cij_Hours;
                Dictionary<(int, int, int), int> Aijf = InputData.Aijf;
                Dictionary<int, List<int>> Ri = InputData.Ri;

                #endregion

                //Model Initialization
                Cplex model = new Cplex();

                #region Decision Variables

                INumVar[,] X = new INumVar[I, RiMax];
                INumVar[] Y = new INumVar[F];

                for (int i = 0; i < I; i++)
                {
                    List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                    foreach (int j in RosterPerEmployee_List)
                    {
                        string varNameX = $"X{i + 1}_{j + 1}";

                        X[i, j] = model.NumVar(0, 1, NumVarType.Bool, varNameX);
                    }
                }
                for (int f = 0; f < F; f++)
                {
                    string varNameY = $"Y{f + 1}";

                    Y[f] = model.NumVar(0, 1, NumVarType.Bool, varNameY);

                }
                #endregion

                #region Objective Function

                ILinearNumExpr objective = model.LinearNumExpr();

                for (int i = 0; i < I; i++)
                {
                    List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                    foreach (int j in RosterPerEmployee_List)
                    {
                        var Cij_Cost = Cij_Hours[(i, j)] * c;
                        objective.AddTerm(Cij_Cost, X[i, j]);
                    }
                }
                for (int f = 0; f < F; f++)
                {
                    objective.AddTerm(h, Y[f]);
                }

                model.AddMinimize(objective);

                #endregion

                #region Constraints

                // #13. Con13
                for (int i = 0; i < I; i++)
                {
                    ILinearNumExpr expr = model.LinearNumExpr();
                    List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();
                    foreach (int j in RosterPerEmployee_List)
                    {
                        expr.AddTerm(1, X[i, j]);

                    }
                    model.AddEq(expr, 1, "CON2_" + (i + 1));
                }

                // #14. Con14 
                for (int f = 0; f < F; f++)
                {
                    ILinearNumExpr expr = model.LinearNumExpr();
                    expr.AddTerm(1, Y[f]);
                    for (int i = 0; i < I; i++)
                    {
                        List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                        foreach (int j in RosterPerEmployee_List)
                        {
                            expr.AddTerm(Aijf[(i, j, f)], X[i, j]);
                        }
                    }
                    model.AddEq(expr, Bf[f], "CON3_" + (f + 1));
                }

                #endregion

                #region Get Results

                // Solve the model
                model.Solve();
                bool solution = (model.GetStatus() == Cplex.Status.Optimal);
                if (solution)
                {
                    Data.ObjValue = model.ObjValue;

                    // Export Model
                    model.ExportModel(Path.Combine(relativePath, "CS_Set_Partition.lp"));
                    model.ExportModel(Path.Combine(relativePath, "CS_Set_Partition.mps"));
                    model.WriteSolution(Path.Combine(relativePath, "CS_Set_Partition.sol"));
                }

                model.End();

                #endregion
            }
            catch (ILOG.Concert.Exception ex)
            {
                Console.WriteLine("A CPLEX error occurred: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("A System error occurred: " + ex.Message);
            }

            return Data;
        }

        public void InitializeData (CSInputData InputData)
        {
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
        }

        public CSOutputData MainFunction(CSInputData InputData)
        {
            CSOutputData Output = new CSOutputData();
            Clock_cs = new Stopwatch();
            Clock_cs.Start();

            InitializeData(InputData);

            if (InputData.CSType == Model.Enums.BasicEnums.CSType.Set_Partition)
                CalculateCrewScheduling_SetPartition_CPLEX(InputData);
            else
                //CalculateCrewScheduling_SetCover_CPLEX(InputData);


            Clock_cs.Stop();
  
            return Output;
        }
    }
}
