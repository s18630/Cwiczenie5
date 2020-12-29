using Cwiczenie5.DTOs.Requests;
using Cwiczenie5.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cwiczenie5.Services
{
    public interface IStudentsDbService
    {

        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
        PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request);

    }
}
