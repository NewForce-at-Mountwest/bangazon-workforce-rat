using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BangazonWorkforce.Models;

namespace BangazonWorkforce.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Employees
        public ActionResult Index()
        {
            return View();
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                         SELECT Employee.Id, Employee.FirstName, Employee.LastName, Employee.IsSupervisor, Employee.DepartmentId AS 'Department Id', ComputerEmployee.AssignDate, ComputerEmployee.UnassignDate, Computer.Make, Computer.Manufacturer, TrainingProgram.[Name] AS 'Training Program Name', TrainingProgram.Id AS 'Training Program Id'
FROM Employee
 
 LEFT JOIN EmployeeTraining on EmployeeTraining.EmployeeId = Employee.Id
 LEFT JOIN TrainingProgram on EmployeeTraining.TrainingProgramId = TrainingProgram.Id
LEFT JOIN ComputerEmployee on ComputerEmployee.EmployeeId = Employee.Id 
LEFT JOIN Computer ON ComputerEmployee.ComputerId = Computer.Id 
WHERE Employee.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    // Create a new employee and set it equal to null.
                    Employee employee = null;
                    // Create a while loop so that additional training programs will be added. 
                    while (reader.Read())
                    {

                        //If there isn't already an employee, then one will be created now.
                        if (employee == null)
                        {

                            employee = new Employee()

                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("Department Id")),
                                TrainingPrograms = new List<TrainingProgram>()
                            };
                        }
                        //If the UnassignDate is equal to null for the employee's computer, that means that they have a current computer and a current computer needs to be created. If they do not have a current computer, then it will be set to null.
                        if (reader.IsDBNull(reader.GetOrdinal("UnassignDate")))
                        {
                            employee.CurrentComputer = new Computer()
                            {
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            };
                        }
                        else
                        {
                            employee.CurrentComputer = null;
                        }
                        
                            //If the employee has any training programs linked to them, then a new list of training programs will be created.

                            if (!reader.IsDBNull(reader.GetOrdinal("Training Program Id")))
                            {
                           
                                TrainingProgram trainingProgram = new TrainingProgram()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Training Program Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Training Program Name"))
                                };
                            //If the training program id doesn't match any of the training program id's already added, then add it. This makes sure the programs are only added one time.

                            if (!employee.TrainingPrograms.Any(e => e.Id == trainingProgram.Id))
                            {
                                employee.TrainingPrograms.Add(trainingProgram);
                            }
                            }



                          
                        
                    }
                //Close the reader and add the employee details to the view.
                        reader.Close();

                        return View(employee);
                    }
                }
            }
       
        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}