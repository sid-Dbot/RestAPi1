using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        public TokenService()
        {
        }

        public string CreateToken(AppUser user)
        {
            throw new NotImplementedException();
        }
    }
}