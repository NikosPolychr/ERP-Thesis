using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Erp.DataBase.Τhesis
{
    public class MinMaxDataEntity
    {
        [Key]
        public int RuleId { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Code { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Descr { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Position { get; set; }

        public int? CrewCatId { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string RouteCateg { get; set; }

        [Column(TypeName = "int")]
        public int rhs { get; set; }

        [Column(TypeName = "int")]
        public int PenaltyPoints { get; set; }

        [Column(TypeName = "bit")]
        public bool IsMin { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsAct { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        [ForeignKey("CrewCatId")]
        public virtual CrewCategDataEntity CrewCateg { get; set; }
    }
}
