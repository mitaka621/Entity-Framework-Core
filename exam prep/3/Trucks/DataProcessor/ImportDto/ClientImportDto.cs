using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trucks.DataProcessor.ImportDto
{
    public class ClientImportDto
    {
        [MinLength(3)]
        [MaxLength(40)]
        public string Name { get; set; }

        [MinLength(2)]
        [MaxLength(40)]
        public string Nationality { get; set; }
   
        public string Type { get; set; }

        public List<int> Trucks { get; set; }
    }
}
