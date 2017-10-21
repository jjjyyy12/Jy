using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Models
{
    public class Paged<T> where T:class
    {
        public int rowCount { get; set; }
        public decimal pageCount { get; set; }

        public List<T> rows { get; set; }
    }

}
