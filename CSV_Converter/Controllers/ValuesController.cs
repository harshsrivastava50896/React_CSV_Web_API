using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChoETL;
using CSV_Converter.DTO;
using CSV_Converter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSV_Converter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private ApplicationDbContext _context;


        public ValuesController(ApplicationDbContext context)
        {
            _context = context;
            if(!_context.Porducts.Any())
            {
                _context.Porducts.AddRange(new Item(){
                   Id = 1, Name = "Disc", Description = "A 4.7 GB DVD" },
                new Item() { Id = 2, Name = "Marker", Description = "Black Marker" },
                new Item() { Id = 3, Name = "Stapler", Description = "A 10mm 50 pin stapler" },
                new Item() { Id = 4, Name = "Cello Tape", Description = "Transparent roll 1 meter" },
                new Item() { Id = 5, Name = "Clay", Description = "10 shades of soft clay" });
                _context.SaveChanges();
            }

        }

        // GET api/values
        [EnableCors("MyPolicy")]
        [HttpGet]
        public List<Item> Get()
        {          
            return _context.Porducts.ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [EnableCors("MyPolicy")]
        [HttpPost]
        public IEnumerable<ItemDTO> Post([FromBody] string value)
        {
            StringBuilder sb = new StringBuilder();
         
            using (var p = ChoCSVReader.LoadText(value).WithFirstLineHeader())
            {
                using (var w = new ChoJSONWriter(sb))
                {
                    w.Write(p);
                }

            }
            var items = _context.Porducts.ToList();
            var content = JsonConvert.DeserializeObject<IEnumerable<Item>>(sb.ToString());
            List<ItemDTO> newItems = new List<ItemDTO>();
            foreach(Item item in content)
            {
                if(!items.Any(y => y.Id == item.Id))
                {
                    var newItem = new ItemDTO() { Product = item, Updated = false };
                    newItems.Add(newItem);
                }
                else
                {
                    var existingItem = new ItemDTO() { Product = item, Updated = true };
                    newItems.Add(existingItem);
                }              
            }
            return newItems;
        }

        // PUT api/values/5
        [HttpPut]
        public IEnumerable<Item> Put( [FromBody] IEnumerable<ItemDTO>  items)
        {
            var products =  _context.Porducts.AsNoTracking().ToList();
            foreach (ItemDTO item in items)
            {
                if(products.Any(x => x.Id == item.Product.Id))
                {
                    var product = products.Where(x => x.Id == item.Product.Id).SingleOrDefault(); 
                    _context.Porducts.Remove(product);
                }
                _context.Porducts.Add(item.Product);
            }
            _context.SaveChanges();
            return _context.Porducts.ToList();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
