using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace miminal_api.DTOs
{
    public class VeiculoDTO
    {
        public string Make { get; set; } = string.Empty;       
        public string Model { get; set; } = string.Empty;      
        public int Year { get; set; }                          
        public string LicensePlate { get; set; } = string.Empty;
    }
}
