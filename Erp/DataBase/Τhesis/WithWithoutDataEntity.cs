using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Erp.DataBase.Τhesis
{
    public class WithWithoutDataEntity
    {
        [Key]
        public int RuleId { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Code { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Descr { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Position1 { get; set; }

        public int? CrewCatId1 { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Position2 { get; set; }
        public int? CrewCatId2 { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string RouteCateg { get; set; }

        [Column(TypeName = "int")]
        public int PenaltyPoints { get; set; }

        [Column(TypeName = "bit")]
        public bool IsWith { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsAct { get; set; }
        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        [ForeignKey("CrewCatId1")]
        public virtual CrewCategDataEntity CrewCateg1 { get; set; }
        [ForeignKey("CrewCatId2")]
        public virtual CrewCategDataEntity CrewCateg2 { get; set; }
    }
}
