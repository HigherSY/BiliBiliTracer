using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BiliBiliTracer.Model
{
    public class Followers
    {
        public List<JsonElement> list { get; set; }
        public int re_version { get; set; }
        public int total { get; set; }
    }
}
