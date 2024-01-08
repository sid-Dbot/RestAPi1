using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiConroller
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]

        public async Task<ActionResult<AppUser>> Register(string username, string password)
        {

            using var hmac = new HMACSHA512();

            var user = new AppUser { UserName = username, PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)), PasswordSalt = hmac.Key };

            _context.Add(user);

            return user;
        }




    }
}