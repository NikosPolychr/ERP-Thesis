using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase.Τhesis
{
    public class CrewCategDataEntity
    {
        [Key]
        public int CrewCatId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string CrewCatCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string CrewCatDescr { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }
    }
}
