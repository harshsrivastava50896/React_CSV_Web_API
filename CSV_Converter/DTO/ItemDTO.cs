using CSV_Converter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSV_Converter.DTO
{
    public class ItemDTO
    {
        public Item  Product { get; set; }
        public bool Updated { get; set; }
    }
}
