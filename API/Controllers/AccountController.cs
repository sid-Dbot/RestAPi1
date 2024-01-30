using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)

        {
            if (await CheckUsername(registerDto.Username.ToLower())) return BadRequest("Username Exists");


            using var hmac = new HMACSHA512();

            var user = new AppUser { UserName = registerDto.Username.ToLower(), PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)), PasswordSalt = hmac.Key };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> CheckUsername(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);


        }




    }
}