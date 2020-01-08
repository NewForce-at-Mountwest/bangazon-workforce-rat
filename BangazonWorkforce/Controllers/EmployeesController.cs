using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforce.Controllers
{
    public class EmployeesController : Controller
    {
        // GET: Employee
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
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                     SELECT e.Id,
                     e.FirstName,
					 e.LastName,
                    d.Name
                    as 'Department'

                    FROM Employee e LEFT JOIN Department d ON e.DepartmentId = d.id";
;
                    //SELECT e.Id,
                    // e.FirstName,
                    //e.LastName
                    //FROM Employee e

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("Department")))
                        {
                            Department department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                               
                            };
                            if (employees.Any(e => e.Id == employee.Id))
                            {
                                Employee employeeToReference = employees.Where(e => e.Id == employee.Id).FirstOrDefault();
                                if (!employeeToReference.CurrentDepartment.Any(s => s.Id == department.Id))
                                
                                    employeeToReference.CurrentDepartment.Add(department);
                                }
                            }
                            else
                            {
                                employee.CurrentDepartment.Add(department);
                                employees.Add(employee);
                            }
                        }

              

                    //List<Department> departments = new List<Department>();
                    //while (reader.Read())
                    //{
                    //    Department department = new Department
                    //    {
                    //        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    //        Name = reader.GetString(reader.GetOrdinal("Name")),
                    //    };

                    //    departments.Add(department);
                   //}

                    reader.Close();

                    return View(employees);
                }
            }
        }
    }
}

       