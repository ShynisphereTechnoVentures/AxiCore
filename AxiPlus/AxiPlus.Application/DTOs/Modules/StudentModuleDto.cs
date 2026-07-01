using AxiPlus.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxiPlus.Application.DTOs.Modules
{
    public class StudentModuleDto
   {
        public int ModuleId{ get; set; }

        public string Title{ get; set; }
            = string.Empty;

        public string Description{ get; set; }
            = string.Empty;

        public int Order{ get; set; }

        public int LessonCount{ get; set; }

        public bool IsUnlocked{ get; set; }

        public bool IsCompleted{ get; set; }

        public decimal ProgressPercentage{ get; set; }

        public ModuleStatus Status{ get; set; }
    }
}
