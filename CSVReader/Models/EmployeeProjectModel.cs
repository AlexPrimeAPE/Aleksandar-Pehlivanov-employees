using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSVReader.Models
{
    public class EmployeeProjectModel
    {
        public int EmpID { get; set; }
        public int ProjectID { get; set; }
        public DateTime DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
    }
}
