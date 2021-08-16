using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Finger_ATM
{
   public static  class Helper
    {
        public static string Beautify(string jsonstr)
        {
            JToken parseJson = JToken.Parse(jsonstr);
            return parseJson.ToString(Formatting.Indented);
        }
    }
    public class SearchResult
    {
        public string title { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string content { get; set; }
       
    }
}
