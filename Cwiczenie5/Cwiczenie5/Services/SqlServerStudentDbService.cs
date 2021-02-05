using Cwiczenie5.DTOs.Requests;
using Cwiczenie5.DTOs.Responses;
using Cwiczenie5.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cwiczenie5.Services
{
    public class SqlServerStudentDbService : IStudentsDbService
    {

        private const string ConString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True";

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            var student = new Student();
            student.IndexNumber = request.IndexNumber;
            student.FirstName = request.FirstName;
            student.LastName = request.LastName;
            student.BirthDate = request.BirthDate;

            var response = new EnrollStudentResponse();

            
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
                            throw new Exception("Studia nie istnieją");
                        }

                        response.IdStudies = (int)dr["IdStudy"];
                        dr.Close();

    
                        com.CommandText = "Select IndexNumber from Student where IndexNumber =@IndexNumber";
                        com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                        com.Transaction = transacion;
                        dr = com.ExecuteReader();

                        if (dr.Read())
                        {
                            dr.Close();
                            transacion.Rollback();
                             throw new Exception("Student o tym ID już istnieje");
                        }
                        dr.Close();




                        com.CommandText = "Select IdEnrollment from Enrollment where Semester=1 and IdStudy= (select IdStudy from Studies where Name Like @name) order by StartDate DESC ";


                        dr = com.ExecuteReader();

                        if (dr.Read())
                        {
                            response.IdEnrollemnt = (int)dr["IdEnrollment"];
                            dr.Close();

                        }
                        else if (!dr.Read())
                        {
                            dr.Close();

                            com.CommandText = "select IdEnrollment from Enrollment where IdEnrollment =(select max(IdEnrollment) from Enrollment )";
                            dr = com.ExecuteReader();

                            if (dr.Read())
                            {
                                int IdEnroll = (int)dr["IdEnrollment"];
                                response.IdEnrollemnt = IdEnroll + 1;
                            }

                            dr.Close();


                            using (SqlCommand com1 = new SqlCommand())
                            {

                                com1.Connection = con;

                                com1.Transaction = transacion;
                                com1.CommandText = "INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES (@IdEnrollment , 1, @IdStudy,  convert(datetime, @StartDate));";

                                com1.Parameters.AddWithValue("IdStudy", response.IdStudies);
                                DateTime thisDay = DateTime.Today;
                                com1.Parameters.AddWithValue("StartDate", thisDay);
                                com1.Parameters.AddWithValue("IdEnrollment", response.IdEnrollemnt);


                                com1.ExecuteNonQuery();
 


                            }



                        }

                        com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES(@IndexNumber, @FirstName , @LastName, convert(datetime,@BirthDate), @IdEnrollment)";
                        com.Parameters.AddWithValue("FirstName", student.FirstName);
                        com.Parameters.AddWithValue("LastName", student.LastName);
                        com.Parameters.AddWithValue("BirthDate", student.BirthDate);
                        com.Parameters.AddWithValue("IdEnrollment", response.IdEnrollemnt);

                        com.ExecuteNonQuery();
                        transacion.Commit();


                        com.CommandText = "SELECT StartDate from Enrollment where IdEnrollment=@IdEnrollment";

                        dr = com.ExecuteReader();
                        if (dr.Read())
                        {
                            response.StartDate = dr["StartDate"].ToString();
                        }



                response.IndexNumber = student.IndexNumber;
                response.Semester = "1";
                response.Studies = request.Studies;
                response.setConString(ConString);


                return response;

            }
         
       
                

        }

































        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request)
        {
            PromoteStudentsResponse response = new PromoteStudentsResponse();


            using (SqlConnection con = new SqlConnection(ConString))
            {


                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;

                    com.CommandText = "select* from Enrollment " +
                        "inner join Studies on Studies.IdStudy = Enrollment.IdStudy" +
                        " where Enrollment.Semester =@semester and Studies.Name =@studies ";

                    con.Open();

                    com.Parameters.AddWithValue("studies", request.Studies);
                    com.Parameters.AddWithValue("semester", request.Semester);

                    var reader = com.ExecuteReader();

                    if (!reader.Read())
                    {
                        throw new Exception();
                    }
                    reader.Close();

                }



                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;

                    com.CommandText = "PromoteStudents";

                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@Studies", SqlDbType.NVarChar).Value = request.Studies;
                    com.Parameters.AddWithValue("@Semester", SqlDbType.Int).Value = request.Semester;

                    com.ExecuteNonQuery();
                }


            


            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;

                com.CommandText = " Select * from Enrollment where Semester =@NewSemester and IdStudy = (select IdStudy from Studies where Name =@Studies) " +
                 "and StartDate = (select max(StartDate) from Enrollment where Semester =@NewSemester and IdStudy = (select IdStudy from Studies where Name =@Studies))";

                com.Parameters.AddWithValue("NewSemester", request.Semester + 1);
                com.Parameters.AddWithValue("Studies", request.Studies);


                SqlDataReader dr = com.ExecuteReader();


                if (dr.Read())
                {
                    response.IdEnrollment = (int)dr["IdEnrollment"];
                    response.Semester = (int)dr["Semester"];
                    response.idStudy = (int)dr["IdStudy"];
                    response.StartDate = dr["StartDate"].ToString();


                    response.Studies = request.Studies;


                }




            } 
           

        } 
            
            response.setConString(ConString);
            return response;




             

           

            }












        /*


        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request)
        {
            PromoteStudentsResponse response = new PromoteStudentsResponse();


            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;


                com.CommandText = "select* from Enrollment " +
                    "inner join Studies on Studies.IdStudy = Enrollment.IdStudy" +
                    " where Enrollment.Semester =@semester and Studies.Name =@studies ";

                con.Open();

                com.Parameters.AddWithValue("studies", request.Studies);
                com.Parameters.AddWithValue("semester", request.Semester);

                var reader=com.ExecuteReader();

                if (!reader.Read())
                {
                    throw new Exception();
                }

            }


             







            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;


                con.Open();

                com.CommandText = "PromoteStudents";

                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Studies", SqlDbType.NVarChar).Value = request.Studies;
                com.Parameters.AddWithValue("@Semester", SqlDbType.Int).Value = request.Semester;

                com.ExecuteNonQuery();
                
            }


            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();

                com.CommandText = " Select * from Enrollment where Semester =@NewSemester and IdStudy = (select IdStudy from Studies where Name =@Studies) " +
                    "and StartDate = (select max(StartDate) from Enrollment where Semester =@NewSemester and IdStudy = (select IdStudy from Studies where Name =@Studies))";

                com.Parameters.AddWithValue("NewSemester", request.Semester + 1);
                com.Parameters.AddWithValue("Studies", request.Studies);
               


              
                SqlDataReader dr = com.ExecuteReader();
               

                if (dr.Read())
                {
                    response.IdEnrollment = (int)dr["IdEnrollment"];
                    response.Semester = (int)dr["Semester"];
                    response.idStudy = (int)dr["IdStudy"];
                    response.StartDate = dr["StartDate"].ToString();


                    response.Studies = request.Studies;


                }
                else
                {
                    dr.Close();
                //    throw new Exception();
                }

                dr.Close();

            }
            

                response.setConString(ConString);
                return response;
            }











        */



        }
    }

