using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace File.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Phone]
        public string Phone { get; set; }
    }
}
