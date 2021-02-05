﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cwiczenie5.DTOs.Requests;
using Cwiczenie5.DTOs.Responses;
using Cwiczenie5.Models;
using Cwiczenie5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenie5.Controllers
{
    // [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        //   private const string ConString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True";
        private IStudentsDbService _service;

        public EnrollmentsController(IStudentsDbService service)
        {
            _service = service;
        }





        [HttpPost]
        [Route("api/enrollments")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {


            try
            {

                EnrollStudentResponse response = _service.EnrollStudent(request);
                string ConString = response.getConnectionString();
                return Created(ConString, response);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }

        }







        [HttpPost("api/enrollments/promotions")]

        public IActionResult PromoteStudents(PromoteStudentsRequest request)
        {

          try
           {
               PromoteStudentsResponse response= _service.PromoteStudents(request);
                string ConString = response.getConnectionString();
                return Created(ConString, response);

           }
            catch(Exception ex)
           {
               return NotFound();
           }


        }

    }



    }

        

























    

