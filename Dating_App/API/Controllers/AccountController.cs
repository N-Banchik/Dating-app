using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x=>x.UserName == username.ToLower());
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            using  var hmac = new HMACSHA512();
            if(await UserExists(registerDto.Username)) return BadRequest("Username Already exists");
            
            AppUser newuser = new AppUser{
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
                
            };
            await _context.AddAsync(newuser);
            await _context.SaveChangesAsync();

            return newuser;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login (LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x=> x.UserName == loginDto.Username.ToLower());
            if(user ==null) return Unauthorized("Invalid username");

            using  var hmac = new HMACSHA512(user.PasswordSalt);

            var comutedhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i <comutedhash.Length; i++)
            {
                if (comutedhash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("invalid password");
                }
            }
            return user;
        }
    }
}