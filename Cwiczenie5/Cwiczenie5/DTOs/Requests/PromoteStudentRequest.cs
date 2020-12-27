using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cwiczenie5.DTOs.Requests
{
    public class PromoteStudentRequest
    {
        [Required]
        public string Studies { get; set; }
        [Required]
        public string Semester { get; set; }
    }
}
