using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelAgent.Domain.DTO
{
    public class ChangeStateRequestDto
    {
        public int Id { get; set; }
        public bool IsOn { get; set; }
    }
}
