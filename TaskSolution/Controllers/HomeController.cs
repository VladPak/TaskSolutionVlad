using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskSolution.Entities;
using TaskSolution.Models;
using TaskSolution.Repository;
using static TaskSolution.Models.Enums;

namespace TaskSolution.Controllers
{
    public class HomeController : Controller
    {
        //Get method for retrieving all data from database and show in Index view
        public ActionResult Index(string category, string surname, SortCriteria? criteria, SortOrder? order, int? page)
        {
            var model = new EmployeesViewModel
            {
                CurrentPage = page ?? 1,
                Category = category,
                Surname = surname,
                Criteria = criteria ?? SortCriteria.Surname,
                Order = order ?? SortOrder.ASC
            };

            var employees = new EmployeeRepository().GetAll();

            if (!string.IsNullOrEmpty(category))
                employees = employees.Where(x => x.Category.Equals(category));

            if (!string.IsNullOrEmpty(surname))
                employees = employees.Where(x => x.Surname.ToLower().Contains(surname.ToLower()));

            if (criteria == SortCriteria.Surname)
            {
                if (order == SortOrder.DESC)
                    employees = employees.OrderByDescending(x => x.Surname);
                else
                    employees = employees.OrderBy(x => x.Surname);
            }
            else
            {
                if (order == SortOrder.DESC)
                    employees = employees.OrderByDescending(x => x.Surname);
                else
                    employees = employees.OrderBy(x => x.Surname);
            }

            //pagination
            int pageSize = 4;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            model.TotalPages = (int)Math.Ceiling(((double)employees.Count()) / ((double)pageSize));
            model.Employees = employees.Select(MapToModel).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return View(model);
        }

    #region Upload csv file and Save it to the database
        //Post method for uploading csv to database
        [HttpPost]
        public ActionResult Index(EmployeesViewModel empModel, int? page)
        {
            empModel.CurrentPage = page ?? 1;

            var file = Request.Files["fileToImport"];

            if (file == null)
            {
                ViewBag.Result = "File is missing";
                return View();
            }

            var employees = new List<EmployeeViewModel>();
            var errorMessage = new List<string>();
            var lineNum = 0;
            using (var reader = new StreamReader(file.InputStream))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        lineNum++;
                        if (line.StartsWith("Personnel_Records"))
                        {
                            continue;
                        }

                        var tokens = line.Split(',');
                        var emps = new EmployeeViewModel();

                        emps.EmployeePayrollNumber = tokens[0].ToString();
                        emps.EmployeeForename = tokens[1].ToString();
                        emps.EmployeeSurname = tokens[2].ToString();
                        emps.EmployeeDateOfBirth = DateTime.ParseExact(tokens[3], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        emps.EmployeeTelephone = int.Parse(tokens[4]);
                        emps.EmployeeMobile = int.Parse(tokens[5]);
                        emps.EmployeeAddress = tokens[6].ToString();
                        emps.EmployeeAddress2 = tokens[7].ToString();
                        emps.EmployeePostCode = tokens[8].ToString();
                        emps.EmployeeEmail = tokens[9].ToString();
                        emps.EmployeeStartDate = DateTime.ParseExact(tokens[10], "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        employees.Add(emps);
                    }
                    catch (Exception)
                    {
                        errorMessage.Add("Data format error in line " + lineNum.ToString() + ". This line will be not imported.");
                        continue;
                    }

                }
            }
            var totalMsg = "Total lines read from file: " + lineNum.ToString() + ". Successfully imported - " + employees.Count.ToString() + ". Failed records - " + errorMessage.Count.ToString();
            errorMessage.Add(totalMsg);
            if (employees.Count > 0)
            {
                errorMessage = SaveEmployeesToDataBase(employees, errorMessage);
            }

            ViewBag.Result = errorMessage;

            int pageSize = 4;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            empModel.TotalPages = (int)Math.Ceiling(((double)employees.Count()) / ((double)pageSize));
            //empModel.Employees = employees.Select(MapToModel).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            empModel.Employees = GetDataToView().Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(); ;

            return View(empModel);
        }

        private List<string> SaveEmployeesToDataBase(List<EmployeeViewModel> model, List<string> errors)
        {
            try
            {
                var mappedEmployees = model.Select(MapFromModel).ToList();
                var repo = new EmployeeRepository();
                repo.InsertEmployees(mappedEmployees);
            }
            catch (Exception ex)
            {
                errors.Add("Errors inserting employees : " + ex.Message);
            }
            return errors;

        }

        private List<EmployeeViewModel> GetDataToView()
        {
            var repository = new EmployeeRepository();
            var emps = repository.GetAll();
            return emps.Select(MapToModel).ToList();
        }
        #endregion

        //Add new employee to the database
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EmployeeViewModel employeeModel)
        {
            if (ModelState.IsValid)
            {
                new EmployeeRepository().Create(MapFromModel(employeeModel));
                return RedirectToAction("Index");
            }
            return View(employeeModel);
        }

        //Edit chose employee data
        public ActionResult Edit(int id)
        {
            if (new EmployeeRepository().GetById(id) == null)
            {
                return Content("Employee is not found");
            }
            else
            {
                return View(MapToModel(new EmployeeRepository().GetById(id)));
            }
        }

        [HttpPost]
        public ActionResult Edit(EmployeeViewModel model)
        {
            EmployeeRepository employeeRepository  = new EmployeeRepository();
            employeeRepository.Update(MapFromModel(model));
            return RedirectToAction("Index");
        }

        //Function for deleting empoyee from database (works)
        public ActionResult Delete(int id)
        {
            EmployeeRepository employeeRepository = new EmployeeRepository();
            var employee = employeeRepository.GetById(id);
            employeeRepository.Delete(id);
            return RedirectToAction("Index");
        }

        //To allow the database model and project's viewmodel communicate two private methods were created (MapFromModel and MapToModel)
        private Employee MapFromModel(EmployeeViewModel model)
        {
            return new Employee
            {
                ID = model.EmployeeId,
                PayrollNumber = model.EmployeePayrollNumber,
                Forename = model.EmployeeForename,
                Surname = model.EmployeeSurname,
                DateOfBirth = model.EmployeeDateOfBirth,
                Telephone = model.EmployeeTelephone,
                Mobile = model.EmployeeMobile,
                Address = model.EmployeeAddress,
                Address2 = model.EmployeeAddress2,
                Postcode = model.EmployeePostCode,
                Email = model.EmployeeEmail,
                StartDate = model.EmployeeStartDate
            };
        }

        private EmployeeViewModel MapToModel(Employee employees)
        {
            return new EmployeeViewModel
            {
                EmployeeId = employees.ID,
                EmployeePayrollNumber = employees.PayrollNumber,
                EmployeeForename = employees.Forename,
                EmployeeSurname = employees.Surname,
                EmployeeDateOfBirth = employees.DateOfBirth,
                EmployeeTelephone = employees.Telephone,
                EmployeeMobile = employees.Mobile,
                EmployeeAddress = employees.Address,
                EmployeeAddress2 = employees.Address2,
                EmployeePostCode = employees.Postcode,
                EmployeeEmail = employees.Email,
                EmployeeStartDate = employees.StartDate
            };
        }

        //About page 
        public ActionResult About()
        {
            ViewBag.Message = "Description Page";

            return View();
        }

        //Contact page
        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Page";

            return View();
        }
    }
}