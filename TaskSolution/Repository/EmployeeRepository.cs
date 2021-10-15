using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaskSolution.Entities;

namespace TaskSolution.Repository
{
    public class EmployeeRepository
    {
        private StaffManagementDBEntities db;

        public EmployeeRepository()
        {
            db = new StaffManagementDBEntities();
        }

        //Retrieve all records
        public IQueryable<Employee> GetAll()
        {
            return db.Employees;
        }

        //Add multiple records
        public void InsertEmployees(List<Employee> employees)
        {
            db.Employees.AddRange(employees);
            db.SaveChanges();
        }

        //Add one new record
        public void Create(Employee employees)
        {
            db.Employees.Add(employees);
            db.SaveChanges();
        }

        //Find record by its Id number
        public Employee GetById(int id)
        {
            return GetAll().SingleOrDefault(x => x.ID == id);
        }

        //Remove record
        public void Delete(int id)
        {
            var currentEmployee = GetById(id);
            db.Employees.Remove(currentEmployee);
            db.SaveChanges();
        }

        //Edit record's information 
        public void Update(Employee employee)
        {
            var retrieveEmployee = GetById(employee.ID);
            retrieveEmployee.Surname = employee.Surname;
            retrieveEmployee.Forename = employee.Forename;
            retrieveEmployee.Email = employee.Email;
            retrieveEmployee.DateOfBirth = employee.DateOfBirth;
            retrieveEmployee.Telephone = employee.Telephone;
            retrieveEmployee.Mobile = employee.Mobile;
            retrieveEmployee.Address = employee.Address;
            retrieveEmployee.Address2 = employee.Address2;
            retrieveEmployee.Postcode = employee.Postcode;
            retrieveEmployee.StartDate = employee.StartDate;
            retrieveEmployee.PayrollNumber = employee.PayrollNumber;            
            db.SaveChanges();
        }

    }
}