using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BangazonWorkforce.Models.ViewModels
{
    public class CreateEmployeeViewModel
    {
        //  List: Departments - Dropdown options
        public List<SelectListItem> Departments { get; set; }

   
        // A single employee. When we render the form (i.e. make a GET request to Employees/Create) this will be null.
        // When we submit the form (i.e. make a POST request to Employees/Create), this will hold the data from the form.
        public Employee employee { get; set; }

        // Connection to the database
        protected string _connectionString;

        protected SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        // Empty constructor so that we can create a new instance of this view model when we make our POST request (in which case it won't need a connection string)
        public CreateEmployeeViewModel() { }

        // This is an example of method overloading! We have one constructor with no parameter and another constructor that's expecting a connection string. We can call either one!
        public CreateEmployeeViewModel(string connectionString)
        {
            _connectionString = connectionString;

            // When we create a new instance of this view model, we'll call the internal methods to get all the departments from the database
            // Then we'll map over them and convert the list of departments to a list of select list items
            Departments = GetAllDepartments()
                .Select(department => new SelectListItem()
                {
                    Text = department.Name,
                    Value = department.Id.ToString()

                })
                .ToList();

            // Add an option with instructions for how to use the dropdown
            Departments.Insert(0, new SelectListItem
            {
                Text = "Choose a department",
                Value = "0"
            });

        }

        // Internal method -- connects to DB, gets all departments, returns list of departments
        protected List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, Budget FROM Department";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        });
                    }

                    reader.Close();

                    return departments;
                }
            }
        }
    }
}

