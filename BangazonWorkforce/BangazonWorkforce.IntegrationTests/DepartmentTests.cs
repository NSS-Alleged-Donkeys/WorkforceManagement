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

        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            string url = "/department";
            
            // Act
            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }



        [Fact]
        public async Task Get_DetailsDisplayEmployees() {

            // Arrange
            
            Department department = (await GetAllDepartments()).First();

            int id = department.Id;
            string url = $"/department/details/{id}";
            string deptName = department.Name;
            
            
            // Act

            HttpResponseMessage response = await _client.GetAsync(url);

            // Assert

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            IHtmlDocument detailPage = await HtmlHelpers.GetDocumentAsync(response);
            IHtmlCollection<IElement> viewData = detailPage.QuerySelectorAll("h2");
            Assert.Contains(viewData, h2 => h2.TextContent.Trim() == deptName);
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
