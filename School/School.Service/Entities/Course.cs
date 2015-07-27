using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace School.Service.Entities
{
    public class Course
    {
        public Course ()
        {
            this.Instructors = new HashSet<Instructor>();
        }
        // Primary key 
        public int CourseID { get; set; }

        public string Title { get; set; }
        public int Credits { get; set; }

        // Foreign key 
        public int DepartmentID { get; set; }

        // Navigation properties 
        public virtual Department Department { get; set; }
        public virtual ICollection<Instructor> Instructors { get; private set; }
    }
}
