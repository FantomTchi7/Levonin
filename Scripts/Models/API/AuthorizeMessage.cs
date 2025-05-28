using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levonin.Scripts.Models.API
{
	public class ApiMessage
	{
		public bool Success { get; set; }
		public object? Response { get; set; }
		public string? ErrorMessage { get; set; }
	}
}
