using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiConroller
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]

        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)

        {
            if (await CheckUsername(registerDto.Username.ToLower())) return BadRequest("Username Exists");


            using var hmac = new HMACSHA512();

            var user = new AppUser { UserName = registerDto.Username.ToLower(), PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)), PasswordSalt = hmac.Key };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserName = user.UserName,
                token = _tokenService.CreateToken(user),
            };
        }

        [HttpPost("login")] //api/account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computerHash.Length; i++)
            {
                if (computerHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");

            }
            return new UserDto
            {
                UserName = user.UserName,
                token = _tokenService.CreateToken(user),
            };
        }

        public async Task<bool> CheckUsername(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);


        }




    }
}