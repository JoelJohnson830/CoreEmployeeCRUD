//Completion update
using CoreEmployeeCRUD.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CoreEmployeeCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;
        public EmployeeController(IConfiguration config)
        {
            _config = config;
        }

        //####################################DISPLAY ALL EMPLOYEES ROUTE########################################
        [HttpGet]
        [Route("/GetAllEmployee/withQuery")]
        public ActionResult GetAllEmployeewithQuery()
        {
            try
            {
                SqlConnection connection = new SqlConnection(_config.GetConnectionString("EmployeeDB"));
                var employees = connection.Query("SELECT Emp_firstname AS Firstname,EMp_surname AS Lastname,Emp_email AS EmailID,Emp_dept AS Department FROM Emp_details");
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("/GetAllEmployee/withStoredProcedure")]
        public async Task<IEnumerable<EmployeeModel>> GetAllEmployeewithStoredProcedure()
        {
            IEnumerable<EmployeeModel> employees;
            try
            {
                using(SqlConnection connection = new SqlConnection(_config.GetConnectionString("EmployeeDB")))
                {
                    await connection.OpenAsync();
                    employees = await connection.QueryAsync<EmployeeModel>("Emp_procedure", 
                        new { Action = "select" }, 
                        commandType: System.Data.CommandType.StoredProcedure);
                }
                return employees;
            }
            catch
            {
                return new List<EmployeeModel>();
            }
        }
        
        private static async Task<IEnumerable<EmployeeModel>> SelectAllEmployees(SqlConnection connection)
        {
            return await connection.QueryAsync<EmployeeModel>("Emp_procedure",
                        new { Action = "select" },
                        commandType: System.Data.CommandType.StoredProcedure);
        }
        //########################################################################################################
        //##################################DISPLAY PERTICULAR EMPLOYEE ROUTE#####################################
        [HttpGet("/GetEmployee/withQuery/{email}")]
        public async Task<ActionResult<List<EmployeeModel>>> GetEmployeewithQuery(string EmployeeId)
        {
            try
            {
                using var connection = new SqlConnection(_config.GetConnectionString("EmployeeDB"));
                var Employee = await connection.QueryFirstAsync<EmployeeModel>($"SELECT Emp_firstname AS Firstname,  Emp_surname AS Surname, Emp_email AS Email, Emp_dept AS Department FROM Emp_details WHERE Emp_email = '{EmployeeId}'");
                return Ok(Employee);
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("/GetEmployee/withStoredProcedure/")]
        public async Task<ActionResult<EmployeeModel>> GetEmployeewithStoredProcedure(string EmployeeId)
        {
                using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("EmployeeDB")))
                {
                    await connection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Action", "find");
                    parameters.Add("@search", EmployeeId);
                    var employee = await connection.QueryAsync<EmployeeModel>("Emp_procedure", parameters, commandType: System.Data.CommandType.StoredProcedure);
                    return Ok(employee);
                }
        }
        //########################################################################################################
        //########################################EMPLOYEE CREATE ROUTE###########################################
        [HttpPost]
        [Route("/CreateEmployee/withQuery")]
        public async Task<ActionResult<List<EmployeeModel>>> CreateEmployeewithQuery(EmployeeModel Emp)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("EmployeeDB"));
            await connection.ExecuteAsync($"INSERT INTO Emp_details(Emp_firstname, Emp_surname, Emp_email, Emp_password, Emp_dept) values('{Emp.firstname}', '{Emp.surname}', '{Emp.email}', '{Emp.password}', '{Emp.department}')");
            return Ok(await SelectAllEmployees(connection));
        }
        
        [HttpPost]
        [Route("/CreateEmployee/withStoredProcedure")]
        public async Task<ActionResult<EmployeeModel>> CreateEmployeewithStoredProcedure(EmployeeModel Emp)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("EmployeeDB")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", "insert");
                parameters.Add("@fname", Emp.firstname);
                parameters.Add("@sname", Emp.surname);
                parameters.Add("@email", Emp.email);
                parameters.Add("@password", Emp.password);
                parameters.Add("@department", Emp.department);
                var employee = await connection.QueryAsync<EmployeeModel>("Emp_procedure", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return Ok(await SelectAllEmployees(connection));
            }
        }
        //########################################################################################################
        //###########################################EMPLOYEE UPDATE ROUTE########################################
        [HttpPut]
        [Route("/EmployeeUpdate/withQuery")]
        public async Task<ActionResult<List<EmployeeModel>>> EmployeeUpdatewithQuery(string fname, string lname, string email, string dept)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("EmployeeDB"));
            await connection.ExecuteAsync($"UPDATE Emp_details SET Emp_firstname = '{fname}',Emp_surname = '{lname}',Emp_dept = '{dept}' WHERE Emp_email = '{email}'");
            return Ok(await SelectAllEmployees(connection));
        }
        
        [HttpPut]
        [Route("/EmployeeUpdate/withStoredProcedure")]
        public async Task<ActionResult<EmployeeModel>> EmployeeUpdatewithStoredProcedure(EmployeeModel Emp)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("EmployeeDB")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", "update");
                parameters.Add("@fname", Emp.firstname);
                parameters.Add("@sname", Emp.surname);
                parameters.Add("@email", Emp.email);
                parameters.Add("@department", Emp.department);
                var employee = await connection.QueryAsync<EmployeeModel>("Emp_procedure", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return Ok(await SelectAllEmployees(connection));
            }
        }
        //########################################################################################################
        //###########################################EMPLOYEE DELETE ROUTE########################################
        [HttpDelete]
        [Route("EMployeeDelete/withQuery/")]
        public async Task<ActionResult<EmployeeModel>> EmployeeDelete(string empid)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("EmployeeDB"));
            await connection.ExecuteAsync($"DELETE FROM Emp_details WHERE Emp_email = '{empid}'");
            return Ok(await SelectAllEmployees(connection));
        }
        
        [HttpDelete]
        [Route("EMployeeDelete/withStoredProcedure/")]
        public async Task<ActionResult<EmployeeModel>> EmployeeDeletewithStoredProcedure(string empid)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("EmployeeDB")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", "delete");
                parameters.Add("email", empid);
                var employee = await connection.QueryAsync<EmployeeModel>("Emp_procedure", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return Ok(await SelectAllEmployees(connection));
            }
        }
        //########################################################################################################
        [HttpGet]
        [Route("/login/withStoredProcedure/{email}/{password}")]
        public async Task<ActionResult<EmployeeModel>> LoginwithStoredProcedure(string email, string password)
        {
            using (SqlConnection connection = new SqlConnection(_config.GetConnectionString("EmployeeDB")))
            {
                await connection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@Action", "login");
                parameters.Add("@var_email", email);
                parameters.Add("@var_pswd", password);
                var employee = await connection.QueryAsync<EmployeeModel>("Emp_procedure", parameters, commandType: System.Data.CommandType.StoredProcedure);
                if(employee != null)
                {
                    return Ok(employee);
                }
                else
                {
                    return BadRequest("Invalid EMployee Credentials");
                }
            }
        }
    }
}