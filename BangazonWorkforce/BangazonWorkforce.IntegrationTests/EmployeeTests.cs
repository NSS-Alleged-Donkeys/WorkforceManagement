﻿using AngleSharp.Dom.Html;
using BangazonWorkforce.IntegrationTests.Helpers;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Dapper;
using AngleSharp.Dom;

namespace BangazonWorkforce.IntegrationTests
{
    public class EmployeeTests :
        IClassFixture<WebApplicationFactory<BangazonWorkforce.Startup>>
    {
        private readonly HttpClient _client;

        public EmployeeTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            string url = "/employee";
            
            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        //David Taylor
        //Checks if employees display correctly from the index view
        public async Task Get_IndexDisplaysCorrectEmployees()
        {
            // Arrange
            // Create variables to represent data to be tested
            string url = "/employee";
            string firstName = "Taylor";
            string lastName = "Gulley";
            string dept = "Marketing";
            string fullName = firstName + " " + lastName;

			// Act
			// Get HTTP response from variable defined above
			HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            // Check if there is any data is displayed on index 
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            
            //Check if data displayed matches data in database
            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            IHtmlCollection<IElement> tds = indexPage.QuerySelectorAll("td");
            Assert.Contains(tds, td => td.TextContent.Trim() == fullName);
            Assert.Contains(tds, td => td.TextContent.Trim() == dept);
        }

		[Fact]
		//Mark Hale
		//Checks if employees details correctly
		public async Task GET_DetailsDisplaysCorrectEmployeeInformation()
		{
			// Arrange
			// Create variables to represent data to be tested
			string url = "/employee/Details/2";
			string firstName = "Taylor";
			string lastName = "Gulley";
			string dept = "Marketing";
			string fullName = firstName + " " + lastName;
			string computerManufacturer = "Schmicrosoft";
			string computerMake = "Schmurface Pro";
			string wholeComputer = computerManufacturer + " " +computerMake;
			string firstTrainingProgramName = "POS Training";

			// Act
			// Get HTTP response from variable defined above
			HttpResponseMessage response = await _client.GetAsync(url);

			// Assert
			// Check if there is any data is displayed on details/1 
			response.EnsureSuccessStatusCode(); // Status Code 200-299
			Assert.Equal("text/html; charset=utf-8",
				response.Content.Headers.ContentType.ToString());

			//Check if data displayed matches data in database
			IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
			IHtmlCollection<IElement> dds = indexPage.QuerySelectorAll("dd");
			Assert.Contains(dds, dd => dd.TextContent.Trim() == fullName);
			Assert.Contains(dds, dd => dd.TextContent.Trim() == dept);
			Assert.Contains(dds, dd => dd.TextContent.Trim() == wholeComputer);
			Assert.Contains(dds, dd => dd.TextContent.Trim() == firstTrainingProgramName);
		}

		[Fact]
        public async Task Post_CreateAddsEmployee()
        {
            // Arrange
            Department department = (await GetAllDepartments()).First();

            string url = "/employee/create";

            HttpResponseMessage createPageResponse = await _client.GetAsync(url);

            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);

            string newFirstName = "FirstName-" + Guid.NewGuid().ToString();
            string newLastName = "LastName-" + Guid.NewGuid().ToString();
            string isSupervisor = "true";
            string departmentId = department.Id.ToString();
            string departmentName = department.Name;


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                createPage,
                new Dictionary<string, string>
                {
                    {"Employee_FirstName", newFirstName},
                    {"Employee_LastName", newLastName},
                    {"Employee_IsSupervisor", isSupervisor},
                    {"Employee_DepartmentId", departmentId}
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            var lastRow = indexPage.QuerySelector("tbody tr:last-child");

            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newFirstName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newLastName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(departmentName));

        }

        [Fact]
        public async Task Get_EditEmployeeForm()
        {
            //Arrange
            string url = "/employee/edit/1";

            //Act
            HttpResponseMessage response = await _client.GetAsync(url);

            //Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(response);


            Assert.Contains(
               createPage.QuerySelectorAll(".form-control"),
               fc => fc.Id == "LastName");


            Assert.Contains(
               createPage.QuerySelectorAll(".form-control"),
               i => i.Id == "DepartmentId");

            Assert.Contains(
              createPage.QuerySelectorAll(".form-control"),
              i => i.Id == "ComputerId");

            Assert.Contains(
                createPage.QuerySelectorAll(".form-control"),
                fc => fc.Id == "SelectedTrainingPrograms");

        }

        [Fact]
        public async Task Post_EditWillUpdateEmployee()
        {
            // Arrange
            Employee employee = (await GetAllEmloyees()).First();
            Department department = (await GetAllDepartments()).First();
            TrainingProgram TrainingPrograms = (await GetEmployeeTrainingPrograms(employee.Id)).First();
            Computer computer = (await GetAllComputers()).First();

            string url = $"employee/edit/{employee.Id}";
            HttpResponseMessage editPageResponse = await _client.GetAsync(url);
            IHtmlDocument editPage = await HtmlHelpers.GetDocumentAsync(editPageResponse);

            string firstName = StringHelpers.EnsureMaxLength(
                employee.FirstName + Guid.NewGuid().ToString(), 55);
            string lastName = StringHelpers.EnsureMaxLength(
                employee.LastName + Guid.NewGuid().ToString(), 55);
            string isSupervisor = employee.IsSupervisor ? "false" : "true";
            string departmentId = department.Id.ToString();
            string departmentName = department.Name;
            string trainingProgramId = TrainingPrograms.Id.ToString();
            string computerId = computer.Id.ToString();


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                editPage,
                new Dictionary<string, string>
                {
                    {"LastName", lastName},
                    {"DepartmentId", departmentId},
                    {"ComputerId", computerId},
                    {"SelectedTrainingPrograms", trainingProgramId }
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            var lastRow = indexPage.QuerySelector("tbody tr:first-child");

           
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(lastName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(departmentName));
        }


        [Fact]
        public async Task Get_CreateEmployeeForm()
        {
            //Arrange
            string url = "/employee/create";

            //Act
            HttpResponseMessage response = await _client.GetAsync(url);

            //Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(response);

            Assert.Contains(
                createPage.QuerySelectorAll("input"),
                i => i.Id == "Employee_FirstName");

            
            Assert.Contains(
               createPage.QuerySelectorAll("input"),
               i => i.Id == "Employee_LastName");

             
            Assert.Contains(
               createPage.QuerySelectorAll("input"),
               i => i.Id == "Employee_IsSupervisor");

               
            Assert.Contains(
               createPage.QuerySelectorAll("select"),
               i => i.Id == "Employee_DepartmentId");
            
        }

        private async Task<List<Employee>> GetAllEmloyees()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionSring))
            {
                IEnumerable<Employee> allEmployees =
                    await conn.QueryAsync<Employee>(@"SELECT Id, FirstName, LastName, 
                                                              IsSupervisor, DepartmentId 
                                                         FROM Employee
                                                     ORDER BY Id");
                return allEmployees.ToList();
            }
        }

        private async Task<List<Department>> GetAllDepartments()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionSring))
            {
                IEnumerable<Department> allDepartments =
                    await conn.QueryAsync<Department>(@"SELECT Id, Name, Budget FROM Department");
                return allDepartments.ToList();
            }
        }

        private async Task<List<Computer>> GetAllComputers()
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionSring))
            {
                IEnumerable<Computer> allComputers =
                    await conn.QueryAsync<Computer>(@"SELECT Id, Make, Manufacturer FROM Computer");
                return allComputers.ToList();
            }
        }

        private async Task<List<TrainingProgram>> GetEmployeeTrainingPrograms(int id)
        {
            using (IDbConnection conn = new SqlConnection(Config.ConnectionSring))
            {
                IEnumerable<TrainingProgram> allDepartments =
                    await conn.QueryAsync<TrainingProgram>($"SELECT Id FROM TrainingProgram ");
                return allDepartments.ToList();
            }
        }
    }
}
