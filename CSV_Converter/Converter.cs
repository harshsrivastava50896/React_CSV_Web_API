using ChoETL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Converter
{
    public class Converter
    {
        public object convert()
         {           
            string fileName = @"D:\React_CSV_Tool\sample.csv";
            string content;
            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {   
                content = reader.ReadToEnd();
            }            
            StringBuilder sb = new StringBuilder();
            using (var p = ChoCSVReader.LoadText(content).WithFirstLineHeader())
            {
                using (var w = new ChoJSONWriter(sb))
                w.Write(p);
            }
            var json = JsonConvert.DeserializeObject(sb.ToString());
            return json;
        }
    }
}
