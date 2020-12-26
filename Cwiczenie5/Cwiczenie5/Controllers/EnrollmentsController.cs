using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cwiczenie5.DTOs.Requests;
using Cwiczenie5.DTOs.Responses;
using Cwiczenie5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenie5.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True";


        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {

            var student = new Student();
            student.IndexNumber = request.IndexNumber;
            student.FirstName = request.FirstName;
            student.LastName = request.LastName;
            student.BirthDate = request.BirthDate;





            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;


                com.CommandText = " select IdStudy from Studies where Name Like @name ";

                com.Parameters.AddWithValue("name", request.Studies);
                con.Open();
                var transacion = con.BeginTransaction();
                com.Transaction = transacion;
                using (SqlDataReader dr = com.ExecuteReader()) { 



                    if (!dr.Read())
                    {
                        dr.Close();
                        transacion.Rollback();
                        return BadRequest("nie znaleziono studiów");


                    }
               var response = new EnrollStudentResponse();
            response.IndexNumber = student.IndexNumber;
            //  response.Semester = ;
            //  response.StartDate =/


            return Ok(response);}
             }
            
        }

























    }
}
