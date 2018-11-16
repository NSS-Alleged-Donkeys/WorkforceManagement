using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Dapper;
using BangazonWorkforce.Models.ViewModels;

namespace BangazonWorkforce.Controllers
{
    public class EmployeeController : Controller
    {
        private IConfiguration _config;
        private IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public EmployeeController(IConfiguration config)
        {
            _config = config;
        }

        // Get employees
        public async Task<IActionResult> Index()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = @"SELECT e.Id, 
                                      e.FirstName,
                                      e.LastName, 
                                      e.IsSupervisor,
                                      e.DepartmentId,
                                      d.Id,
                                      d.Name,
                                      d.Budget
                                 FROM Employee e JOIN Department d on e.DepartmentId = d.Id
                             ORDER BY e.Id";
                IEnumerable<Employee> employees = await conn.QueryAsync<Employee, Department, Employee>(
                    sql,
                    (employee, department) => {
                        employee.Department = department;
                        return employee;
                    });

                EmployeeIndexViewModel viewModel = new EmployeeIndexViewModel();
                viewModel.Employees = employees;
                return View(viewModel);
            }
        }

        // Get employee by Id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employee employee = await GetById(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // GET: Employee/Create
        public async Task<IActionResult> Create()
        {
            List<Department> allDepartments = await GetAllDepartments();
            EmployeeAddEditViewModel viewmodel = new EmployeeAddEditViewModel
            {
                AllDepartments = allDepartments
            };
            return View(viewmodel);
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeAddEditViewModel viewmodel)
        {
            if (!ModelState.IsValid)
            {
                List<Department> allDepartments = await GetAllDepartments();
                viewmodel.AllDepartments = allDepartments;
                return View(viewmodel);
            }

            Employee employee = viewmodel.Employee;

            using (IDbConnection conn = Connection)
            {
                string sql = $@"INSERT INTO Employee (
                                    FirstName, LastName, IsSupervisor, DepartmentId
                                ) VALUES (
                                    '{employee.FirstName}', '{employee.LastName}',
                                    {(employee.IsSupervisor ? 1 : 0)}, {employee.DepartmentId}
                                );";

                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Department> allDepartments = await GetAllDepartments();
            List<Computer> EmployeeComputer = await GetEmployeeComputerId(id.Value);
            List<Computer> allComputers = await GetAllComputers();
            List<TrainingProgram> EmployeeTrainingPrograms = await GetEmployeeTrainingPrograms(id.Value);
            List<TrainingProgram> allTrainingPrograms = await GetAllTrainingPrograms();
            
            Employee employee = await GetById(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            EmployeeEditViewModel viewmodel = new EmployeeEditViewModel();

            viewmodel.LastName = employee.LastName;
            viewmodel.EmployeeId = employee.Id;
            viewmodel.DepartmentId = employee.DepartmentId;
            viewmodel.ComputerId = EmployeeComputer.First().Id;
            viewmodel.AllComputers = allComputers;
            viewmodel.AllDepartments = allDepartments;
            viewmodel.EmployeeTrainingPrograms = EmployeeTrainingPrograms;
            viewmodel.AllTrainingPrograms = allTrainingPrograms;
                
            

            return View(viewmodel);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel viewmodel)
        {
            if (id != viewmodel.EmployeeId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                List<Department> allDepartments = await GetAllDepartments();
                List<Computer> allComputers = await GetAllComputers();
                List<TrainingProgram> allTrainingPrograms = await GetAllTrainingPrograms();
                viewmodel.AllDepartments = allDepartments;
                viewmodel.AllComputers = allComputers;
                return View(viewmodel);
            }

            EmployeeEditViewModel employee = viewmodel;

            List<int> SelectedPrograms = employee.SelectedTrainingPrograms;

            using (IDbConnection conn = Connection)
            {
                string sql = $@"UPDATE Employee 
                                   SET LastName = '{employee.LastName}',
                                       DepartmentId = {employee.DepartmentId}
                                 WHERE id = {id}
                                ;

                                UPDATE ComputerEmployee
                                   SET ComputerId = { employee.ComputerId}
                                 WHERE EmployeeId = { id }
                                ;";

                string resettp = $@" DELETE FROM EmployeeTraining WHERE EmployeeId={employee.EmployeeId};";

                string tpsql = "";

                

                foreach (var p in SelectedPrograms)
                {
                    {
                        tpsql = tpsql + $@"
                        INSERT INTO EmployeeTraining
                        (EmployeeId, TrainingProgramId)
                        VALUES
                        ({employee.EmployeeId}, {p});";
                    }
                }

                sql = sql + resettp + tpsql;

                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employee employee = await GetById(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"DELETE FROM Employee WHERE id = {id}";
                await conn.ExecuteAsync(sql);
                return RedirectToAction(nameof(Index));
            }
        }


        private async Task<Employee> GetById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT e.Id, 
                                       e.FirstName,
                                       e.LastName, 
                                       e.IsSupervisor,
                                       e.DepartmentId,
                                       d.Id,
                                       d.Name,
                                       d.Budget
                                  FROM Employee e JOIN Department d on e.DepartmentId = d.Id
                                 WHERE e.id = {id}";
                IEnumerable<Employee> employees = await conn.QueryAsync<Employee, Department, Employee>(
                    sql,
                    (employee, department) => {
                        employee.Department = department;
                        return employee;
                    });

                return employees.SingleOrDefault();
            }
        }

        private async Task<List<Department>> GetAllDepartments()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT Id, Name, Budget FROM Department";

                IEnumerable<Department> departments = await conn.QueryAsync<Department>(sql);
                return departments.ToList();
            }
        }

        private async Task<List<Computer>> GetAllComputers()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT c.Id, 
                                       c.Make,
                                       c.Manufacturer, 
                                       c.PurchaseDate
                                  FROM Computer c
                             ";

                IEnumerable<Computer> computers = await conn.QueryAsync<Computer>(sql);
                return computers.ToList();
            }
        }
        

        private async Task<List<Computer>> GetEmployeeComputerId(int num)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT c.Id, 
                                       c.Make,
                                       c.Manufacturer, 
                                       c.PurchaseDate
                                FROM Computer c
                                JOIN ComputerEmployee ce ON c.Id = ce.ComputerId
                                WHERE ce.EmployeeId = {num}
                             ;";

                IEnumerable<Computer> computers = await conn.QueryAsync<Computer>(sql);
                return computers.ToList();
            }
        }

        private async Task<List<TrainingProgram>> GetAllTrainingPrograms()
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT Id, 
                                       Name,
                                       StartDate, 
                                       EndDate,
                                       MaxAttendees
                                  FROM TrainingProgram
                             ";

                IEnumerable<TrainingProgram> TrainingPrograms = await conn.QueryAsync<TrainingProgram>(sql);
                return TrainingPrograms.ToList();
            }
        }

        private async Task<List<TrainingProgram>> GetEmployeeTrainingPrograms(int num)
        {
            using (IDbConnection conn = Connection)
            {
                string sql = $@"SELECT tp.Id, 
                                       tp.Name,
                                       tp.StartDate, 
                                       tp.EndDate,
                                       tp.MaxAttendees
                                FROM TrainingProgram tp
                                JOIN EmployeeTraining et ON tp.Id = et.TrainingProgramId
                                WHERE et.EmployeeId = {num}
                             ;";

                IEnumerable<TrainingProgram> trainingPrograms = await conn.QueryAsync<TrainingProgram>(sql);
                return trainingPrograms.ToList();
            }
        }
    }
}