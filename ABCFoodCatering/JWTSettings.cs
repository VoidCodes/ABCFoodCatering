using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABCFoodCatering
{
    public class JWTSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int TokenExpirationInMinutes { get; set; }
    }
}
