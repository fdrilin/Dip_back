using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Repositories;
using ZstdSharp.Unsafe;
using System.Security.Cryptography;
using System.Text;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : MyBaseController
    {

        public AuthController() { }

        // POST: api/ResourceItems
        [HttpPost]
        public IActionResult Login(UserItem userItem)
        {
            UserRepository repository = new();

            UserItem dbUser = repository.getUserItemByLogin(userItem.Login);

            if (dbUser == null || dbUser.Password != userItem.Password)
            {
                return StatusCode(403, "login or password incorrect");
            }
            dbUser.Token = GenerateSimpleToken(dbUser.Login + dbUser.Password);
            repository.updateUserItem(dbUser, dbUser.Id);

            return Ok(dbUser);
        }
        
        private string GenerateSimpleToken(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                string input = value + DateTime.UtcNow.ToString("o") + Guid.NewGuid().ToString();
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
