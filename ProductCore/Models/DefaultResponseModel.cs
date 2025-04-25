using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCore.Models
{
    public class DefaultResponseModel
    {
        public required bool Success { get; set; }
        public required int Code { get; set; }
        public dynamic? Meta { get; set; }
        public dynamic? Data { get; set; }
        public dynamic? Error { get; set; }
    }
}
