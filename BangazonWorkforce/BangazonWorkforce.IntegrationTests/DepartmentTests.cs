using AngleSharp.Dom.Html;
using BangazonWorkforce.IntegrationTests.Helpers;
using BangazonWorkforce.Models.ViewModels;
using BangazonWorkforce.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using AngleSharp.Dom;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace BangazonWorkforce.IntegrationTests
{
    public class DepartmentTests :
        IClassFixture<WebApplicationFactory<BangazonWorkforce.Startup>>
    {
        private readonly HttpClient _client;

        public DepartmentTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_CreateDepartmentForm()
        {
            //Arrange
            string url = "/department/create";

            //Act
            HttpResponseMessage response = await _client.GetAsync(url);

            //Assert
            response.EnsureSuccessStatusCode();
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(response);

            Assert.Contains(
                createPage.QuerySelectorAll("input"),
                i => i.Id == "Name");

            Assert.Contains(
               createPage.QuerySelectorAll("input"),
               i => i.Id == "Budget");
        }

        // *Author*: Madison Peper
        // *Purpose*: Testing the department index to see if the "Sales" department with a budget of "100000" is on the page

        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            string url = "/department";
            
            // Act
            HttpResponseMessage response = await _client.GetAsync(url);
            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);


            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            Assert.Contains(indexPage.QuerySelectorAll("td"), d => 
            d.TextContent.Contains("Sales"));

            Assert.Contains(indexPage.QuerySelectorAll("td"), d =>
            d.TextContent.Contains("100000"));
        }



        [Fact]
        // David Taylor
        // Department Details displays all employees for department
        public async Task Get_DetailsDisplayEmployees() {

            // Arrange
            // Creates variables to represent data to be tested

            Employee employee = (await GetAllEmployees()).First();

            string url = $"/department/details/1";
            string employeeFirstName = employee.FirstName;
            string employeeLastName = employee.LastName;
            
            
            // Act
            // Gets HTTP response for data represented above

            HttpResponseMessage response = await _client.GetAsync(url);


            // Assert
            // Checks if there is any data represented on details 
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            
            // Checks if data displayed represents data in database
            IHtmlDocument detailPage = await HtmlHelpers.GetDocumentAsync(response);
            IHtmlCollection<IElement> viewData = detailPage.QuerySelectorAll("h2");
            Assert.Contains(viewData, h2 => h2.TextContent.Trim() == "Marketing");
            IHtmlCollection<IElement> lis = detailPage.QuerySelectorAll("li");
            Assert.Contains(lis, li => li.TextContent.Trim() == employeeFirstName + " " + employeeLastName); 
        }

        [Fact]
        public async Task Post_CreateAddsDepartment()
        {
            // Arrange
            string url = "/department/create";
            HttpResponseMessage createPageResponse = await _client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);

            string newDepartmentName = StringHelpers.EnsureMaxLength("Dept-" + Guid.NewGuid().ToString(), 55);
            string newDepartmentBudget = new Random().Next().ToString();


            // Act
            HttpResponseMessage response = await _client.SendAsync(
                createPage,
                new Dictionary<string, string>
                {
                    {"Name", newDepartmentName},
                    {"Budget", newDepartmentBudget}
                });


            // Assert
            response.EnsureSuccessStatusCode();

            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);
            Assert.Contains(
                indexPage.QuerySelectorAll("td"), 
                td => td.TextContent.Contains(newDepartmentName));
            Assert.Contains(
                indexPage.QuerySelectorAll("td"), 
                td => td.TextContent.Contains(newDepartmentBudget));
        }

        private async Task<List<Employee>> GetAllEmployees()
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
