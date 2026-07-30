using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Laboratory
{
    public class TestDto
    {
       public int Id { get; set; }
        public string TestName { get; set; } = null!;
        public string Description { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
