using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxiPlus.Application.DTOs.Modules
{
    using AxiPlus.Domain.Enums;

    public class LessonProgressDto
   {
        public Guid LessonId{ get; set; }

        public string Title{ get; set; }
            = string.Empty;

        public string Description{ get; set; }
            = string.Empty;

        public int Order{ get; set; }

        public LessonStatus Status{ get; set; }

        public bool IsCompleted{ get; set; }
    }
}
