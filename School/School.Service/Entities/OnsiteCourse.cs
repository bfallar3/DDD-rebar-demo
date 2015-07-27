using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace School.Service.Entities
{
    public partial class OnsiteCourse : Course
    {
        public OnsiteCourse ()
        {
            Details = new Details();
        }

        public Details Details { get; set; } 
    }
}
