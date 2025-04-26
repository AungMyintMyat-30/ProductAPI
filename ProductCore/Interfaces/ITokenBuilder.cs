using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCore.Interfaces
{
    public interface ITokenBuilder
    {
        public string GenerateAccessToken(string user);
    }
}
