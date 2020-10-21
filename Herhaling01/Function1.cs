using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Herhaling01.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Herhaling01
{
    public static class Function1
    {
        [FunctionName("GetRegs")]
        public static async Task<IActionResult> GetRegs(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "garbage/registrations")] HttpRequest req,
            ILogger log)
        {
            try
            {
                List<GarbageRegistration> regs = new List<GarbageRegistration>();

                string connectionString = Environment.GetEnvironmentVariable("DBConString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM GarbageRegistration";

                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            GarbageRegistration reg = new GarbageRegistration();
                            reg.GarbageRegistrationId = Guid.Parse(reader["GarbageRegistrationId"].ToString());
                            reg.Name = reader["Name"].ToString();
                            reg.Email = reader["Email"].ToString();
                            reg.Description = reader["Description"].ToString();
                            reg.GarbageTypeId = Guid.Parse(reader["GarbageTypeId"].ToString());
                            reg.CityId = Guid.Parse(reader["CityId"].ToString());
                            reg.Street = reader["Street"].ToString();
                            reg.Weight = Convert.ToDouble(reader["Weight"]);
                            reg.Lat = Convert.ToDouble(reader["Lat"]);
                            reg.Long = Convert.ToDouble(reader["Long"]);
                            reg.Timestamp = Convert.ToDateTime(reader["Timestamp"]);
                            regs.Add(reg);
                        }
                    }
                }

                return new OkObjectResult(regs);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }


        [FunctionName("GetReg")]
        public static async Task<IActionResult> GetReg(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "garbage/registrations/{id}")] HttpRequest req,
            string id, ILogger log)
        {
            try
            {
                GarbageRegistration reg = new GarbageRegistration();
                string connectionString = Environment.GetEnvironmentVariable("DBConString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM GarbageRegistration WHERE GarbageRegistrationId = @regid";
                        command.Parameters.AddWithValue("@regid", id);

                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            reg.GarbageRegistrationId = Guid.Parse(reader["GarbageRegistrationId"].ToString());
                            reg.Name = reader["Name"].ToString();
                            reg.Email = reader["Email"].ToString();
                            reg.Description = reader["Description"].ToString();
                            reg.GarbageTypeId = Guid.Parse(reader["GarbageTypeId"].ToString());
                            reg.CityId = Guid.Parse(reader["CityId"].ToString());
                            reg.Street = reader["Street"].ToString();
                            reg.Weight = Convert.ToDouble(reader["Weight"]);
                            reg.Lat = Convert.ToDouble(reader["Lat"]);
                            reg.Long = Convert.ToDouble(reader["Long"]);
                            reg.Timestamp = Convert.ToDateTime(reader["Timestamp"]);
                        }
                    }
                }

                return new OkObjectResult(reg);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }


        [FunctionName("AddReg")]
        public static async Task<IActionResult> AddReg(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "garbage/registrations")] HttpRequest req,
            ILogger log)
        {

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                GarbageRegistration reg = JsonConvert.DeserializeObject<GarbageRegistration>(requestBody);

                reg.GarbageRegistrationId = Guid.NewGuid(); //uniqueidentifier nummer genereren
                string connectionString = Environment.GetEnvironmentVariable("DBConString"); //de value van de "SQLServer" setting in local.settings.json ophalen

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO GarbageRegistration VALUES " +
                            "(@garbageregistrationid,@name,@email,@description," +
                            "@garbagetypeid,@cityid,@street,@weight,@lat,@long,@timestamp)";
                        command.Parameters.AddWithValue("@garbageregistrationid", reg.GarbageRegistrationId);
                        command.Parameters.AddWithValue("@name", reg.Name);
                        command.Parameters.AddWithValue("@email", reg.Email);
                        command.Parameters.AddWithValue("@description", reg.Description);
                        command.Parameters.AddWithValue("@garbagetypeid", reg.GarbageTypeId);
                        command.Parameters.AddWithValue("@cityid", reg.CityId);
                        command.Parameters.AddWithValue("@street", reg.Street);
                        command.Parameters.AddWithValue("@weight", reg.Weight);
                        command.Parameters.AddWithValue("@lat", reg.Lat);
                        command.Parameters.AddWithValue("@long", reg.Long);
                        command.Parameters.AddWithValue("@timestamp", reg.Timestamp);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return new OkObjectResult(reg);

            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }


        [FunctionName("DelReg")]
        public static async Task<IActionResult> DelReg(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "garbage/registrations/{id}")] HttpRequest req,
            string id, ILogger log)
        {

            try
            {
                string connectionString = Environment.GetEnvironmentVariable("DBConString"); //de value van de "SQLServer" setting in local.settings.json ophalen

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "DELETE FROM GarbageRegistration WHERE GarbageRegistrationId = @id";
                        command.Parameters.AddWithValue("@id", id);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return new OkObjectResult("");

            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }


        [FunctionName("EditReg")]
        public static async Task<IActionResult> EditReg(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "garbage/registrations/{id}")] HttpRequest req,
            string id, ILogger log)
        {

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                GarbageRegistration reg = JsonConvert.DeserializeObject<GarbageRegistration>(requestBody);

                string connectionString = Environment.GetEnvironmentVariable("DBConString"); //de value van de "SQLServer" setting in local.settings.json ophalen

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "UPDATE GarbageRegistration SET " +
                            "GarbageRegistrationId = @id," +
                            "Name = @name," +
                            "Email = @email," +
                            "Description = @description," +
                            "GarbageTypeId = @typeid," +
                            "CityId = @cityid," +
                            "Street = @street," +
                            "Weight = @weight," +
                            "Lat = @lat," +
                            "Long = @long," +
                            "Timestamp = @timestamp " +
                            "WHERE GarbageRegistrationId = @id";
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@name", reg.Name);  
                        command.Parameters.AddWithValue("@email", reg.Email);
                        command.Parameters.AddWithValue("@description", reg.Description);
                        command.Parameters.AddWithValue("@typeid", reg.GarbageTypeId);
                        command.Parameters.AddWithValue("@cityid", reg.CityId);
                        command.Parameters.AddWithValue("@street", reg.Street);
                        command.Parameters.AddWithValue("@weight", reg.Weight);
                        command.Parameters.AddWithValue("@lat", reg.Lat);
                        command.Parameters.AddWithValue("@long", reg.Long);
                        command.Parameters.AddWithValue("@timestamp", reg.Timestamp);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return new OkObjectResult(reg);

            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }


        [FunctionName("GetCities")]
        public static async Task<IActionResult> GetCities(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "garbage/cities")] HttpRequest req,
            ILogger log)
        {
            try
            {
                List<City> cities = new List<City>();

                string connectionString = Environment.GetEnvironmentVariable("DBConString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM City";

                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            City city = new City();
                            city.CityId = Guid.Parse(reader["CityId"].ToString());
                            city.Name = reader["Name"].ToString();
                            cities.Add(city);
                        }
                    }
                }

                return new OkObjectResult(cities);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }


        [FunctionName("GetTypes")]
        public static async Task<IActionResult> GetTypes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "garbage/types")] HttpRequest req,
            ILogger log)
        {
            try
            {
                List<GarbageType> types = new List<GarbageType>();

                string connectionString = Environment.GetEnvironmentVariable("DBConString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM GarbageType";

                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            GarbageType type = new GarbageType();
                            type.GarbageTypeId = Guid.Parse(reader["GarbageTypeId"].ToString());
                            type.Name = reader["Name"].ToString();
                            types.Add(type);
                        }
                    }
                }

                return new OkObjectResult(types);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult(500);
            }
        }
    }
}
