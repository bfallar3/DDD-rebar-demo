using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace School.Service.Entities
{
    public class OfficeAssignment
    {
        // Specifying InstructorID as a primary 
        [Key]
        public Int32 InstructorID { get; set; }

        public string Location { get; set; }

        // When the Entity Framework sees Timestamp attribute 
        // it configures ConcurrencyCheck and DatabaseGeneratedPattern=Computed. 
        [Timestamp]
        public Byte[] Timestamp { get; set; }

        // Navigation property 
        public virtual Instructor Instructor { get; set; } 
    }
}
