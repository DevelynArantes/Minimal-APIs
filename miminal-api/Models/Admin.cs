using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace miminal_api.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!; 
        public string Name { get; set; } = default!;
    }
}