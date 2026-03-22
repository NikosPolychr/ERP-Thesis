using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Erp.DataBase.Τhesis
{
    public class OptimizerSettingsDataEntity
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Code { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Descr { get; set; }


        #region Roster Legality Rules Parameters

        [Column(TypeName = "int")]
        public int? DaysOff { get; set; }

        [Column(TypeName = "int")]
        public int? MinHoursBetween { get; set; }

        [Column(TypeName = "int")]
        public int? LatestArrivalTime { get; set; }

        [Column(TypeName = "int")]
        public int? EarliestDepartureTime { get; set; }

        [Column(TypeName = "int")]
        public int? XHoursBreak { get; set; }

        [Column(TypeName = "int")]
        public int? YFlightHours { get; set; }

        [Column(TypeName = "int")]
        public int? MaximumFlightHours { get; set; }

        #endregion


        #region Branching Parameters

        [Column(TypeName = "int")]
        public int? AbsBacktrack { get; set; }

        [Column(TypeName = "float")]
        public double? PerceBacktrack { get; set; }

        [Column(TypeName = "int")]
        public int? AbsMIP { get; set; }

        [Column(TypeName = "float")]
        public double? PerceMIP { get; set; }

        [Column(TypeName = "int")]
        public int? NumberOfBacktracksLimit { get; set; }

        [Column(TypeName = "int")]
        public int? NumberOfTreeNodesLimit { get; set; }

        [Column(TypeName = "int")]
        public int? TreelistSortingIdsFixed { get; set; }

        #endregion

        #region ColgenPerformanceParams

        [Column(TypeName = "int")]
        public int? Buckets { get; set; }


        [Column(TypeName = "int")]
        public int? Fictbuckets { get; set; }


        [Column(TypeName = "int")]
        public int? ReducedCostCut { get; set; }

        [Column(TypeName = "float")]
        public double? NIDRAI { get; set; }

        #endregion

        #region Numeric Parameters

        [Column(TypeName = "float")]
        public double? Eps { get; set; }

        [Column(TypeName = "float")]
        public double? MaxDouble { get; set; }

        #endregion

        #region Penalties

        [Column(TypeName = "int")]
        public int? UnderCoverPenalty { get; set; }

        [Column(TypeName = "int")]
        public int? HourPenalty { get; set; }

        [Column(TypeName = "int")]
        public int? MinMaxPenalty { get; set; }

        [Column(TypeName = "int")]
        public int? WithWithoutPenalty { get; set; }

        [Column(TypeName = "int")]
        public int? GenderPenalty { get; set; }

        #endregion

        #region SpecialConstraintsParameters


        [Column(TypeName = "bit")]
        public bool? MinMaxAct { get; set; }

        [Column(TypeName = "bit")]
        public bool? WithAct { get; set; }

        [Column(TypeName = "bit")]
        public bool? WithoutAct { get; set; }

        [Column(TypeName = "bit")]
        public bool? GenderAct { get; set; }

        #region Soft


        [Column(TypeName = "bit")]
        public bool? MinMaxSoft { get; set; }

        [Column(TypeName = "bit")]
        public bool? WithSoft { get; set; }

        [Column(TypeName = "bit")]
        public bool? WithoutSoft { get; set; }

        [Column(TypeName = "bit")]
        public bool? GenderSoft { get; set; }

        #endregion

        #region Priority

        [Column(TypeName = "bit")]
        public bool? WithoutPriority { get; set; }

        [Column(TypeName = "bit")]
        public bool? GenderPriority { get; set; }

        #endregion

        #endregion

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }
    }
}
