using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Resources.Scripts.Models
{
    public class PMTemplate
    {
        public string Name { get; set; }
        public Status Status { get; set; }
        public PMTemplate(string name, Status status)
        {
            Name = name;
            Status = status;
        }
    }
}
