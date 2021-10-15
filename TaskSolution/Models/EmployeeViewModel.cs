using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskSolution.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeePayrollNumber { get; set; }
        public string EmployeeForename { get; set; }
        public string EmployeeSurname { get; set; }
        public DateTime EmployeeDateOfBirth { get; set; }
        public int EmployeeTelephone { get; set; }
        public int EmployeeMobile { get; set; }
        public string EmployeeAddress { get; set; }
        public string EmployeeAddress2 { get; set; }
        public string EmployeePostCode { get; set; }
        public string EmployeeEmail { get; set; }
        public DateTime EmployeeStartDate { get; set; }
    }
}