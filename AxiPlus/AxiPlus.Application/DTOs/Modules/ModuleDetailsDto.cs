using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxiPlus.Application.DTOs.Modules
{
    public class ModuleDetailsDto
   {
        public int ModuleId{ get; set; }

        public string Title{ get; set; }
            = string.Empty;

        public string Description{ get; set; }
            = string.Empty;

        public decimal ProgressPercentage{ get; set; }

        public List<LessonProgressDto> Lessons
       { get; set; } = new();
    }
}
