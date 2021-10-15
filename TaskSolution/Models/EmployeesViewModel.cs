using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static TaskSolution.Models.Enums;

namespace TaskSolution.Models
{
    public class EmployeesViewModel
    {
        public string Category { get; set; }
        public string Surname { get; set; }
        public SortCriteria Criteria { get; set; }
        public SortOrder Order { get; set; }
        public List<EmployeeViewModel> Employees { get; set; }
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}