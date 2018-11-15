using AngleSharp.Dom.Html;
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
            string firstName = "Madison";
            string lastName = "Peper";
            string dept = "IT";
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
        public async Task Post_EditWillUpdateEmployee()
        {
            // Arrange
            Employee employee = (await GetAllEmloyees()).Last();
            Department department = (await GetAllDepartments()).Last();

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


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                editPage,
                new Dictionary<string, string>
                {
                    {"Employee_FirstName", firstName},
                    {"Employee_LastName", lastName},
                    {"Employee_IsSupervisor", isSupervisor},
                    {"Employee_DepartmentId", departmentId}
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            var lastRow = indexPage.QuerySelector("tbody tr:last-child");

            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(firstName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(lastName));
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(departmentName));

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
    }
}
