using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.DataBase.Τhesis
{
    public class EmpCrewCategsDataEntity
    {
        [Key]
        [Column(TypeName = "int")]
        public int ECId { get; set; }

        [Required]
        [ForeignKey("Employees")]
        [Column(TypeName = "int")]
        public int EmpId { get; set; }

        [Required]
        [ForeignKey("CrewCateg")]
        [Column(TypeName = "int")]
        public int CrewCategId { get; set; }

        public virtual EmployeeDataEntity Employees { get; set; }
        public virtual CrewCategDataEntity CrewCateg { get; set; }
    }
}
