using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBiliTracer.Model
{
    public class BiliData<T>
    {
        public int code { get; set; }
        public string message { get; set; }
        public int ttl { get; set; }
        public T data { get; set; }
    }
}
