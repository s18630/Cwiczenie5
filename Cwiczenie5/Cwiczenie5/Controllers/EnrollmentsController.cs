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

                SqlDataReader dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    dr.Close();
                    transacion.Rollback();
                    return BadRequest("Studia nie istnieją");
                }

                int IdStudy = (int)dr["IdStudy"];
                dr.Close();



                com.CommandText = "Select IdEnrollment from Enrollment where Semester=1 and IdStudy= (select IdStudy from Studies where Name Like @name) order by StartDate DESC ";
                com.Transaction = transacion;

                dr = com.ExecuteReader();
                int IdEnrollment = 1;

                if (dr.Read())
                {
                    IdEnrollment = (int)dr["IdEnrollment"];
                    dr.Close();

                }else if (!dr.Read())
                {
                    dr.Close();
                    com.CommandText = "select IdEnrollment from Enrollment where IdEnrollment =(select max(IdEnrollment) from Enrollment )";
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {int IdEnroll = (int)dr["IdEnrollment"];
                        IdEnrollment = IdEnroll + 1;


                    }
                        
                        dr.Close();

                    com.CommandText = "INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES (@IdEnrollment , 1, @IdStudy,  convert(datetime, @StartDate));";

                    com.Parameters.AddWithValue("IdStudy", IdStudy);
                    DateTime thisDay = DateTime.Today;
                    com.Parameters.AddWithValue("StartDate", thisDay);
                  com.Parameters.AddWithValue("IdEnrollment", IdEnrollment);

                    
                    com.ExecuteNonQuery();
                    transacion.Commit();
                    
                }



            }


        







                //zapisujemy teraz studenta 








                // jeśli studia istnieją najnowszy wpis w tabeli enrollments zgodny ze studiami i wartościa semestru=1
                //zapisujemy studenta na semestr pierwszy
                // jesli wpisu nie ma to dodajemy go do bazy i ustawiamy start na aktualną dat e 






                //sprawdzamy index studenta czy unikalny
                //jesli nei zgłaszamy błąd 

                //jeśli tak dodajemy studenta

           

                var response = new EnrollStudentResponse();
                    response.IndexNumber = student.IndexNumber;
                    //  response.Semester = ;
                    //  response.StartDate =/


                    return Ok(response);
                }
             }
            
        

























    }

