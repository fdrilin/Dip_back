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
                return StatusCode(403, GetError("login or password incorrect"));
            }
            dbUser.Token = GenerateSimpleToken(dbUser.Login + dbUser.Password);
            repository.updateUserItem(dbUser, dbUser.Id);

            return Ok(dbUser);
        }

        [HttpPost("register")]
        public IActionResult Register(UserItem userItem)
        {
            UserRepository repository = new();

            string errorStatus = ValidateItem(repository, userItem);
            if (errorStatus != null)
            {
                BadRequest(GetError(errorStatus));
            }

            UserItem newUser = repository.addUserItem(userItem);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(newUser));

            return Login(newUser);
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
        
        private string? ValidateItem(UserRepository repository, UserItem item)
        {
            if (string.IsNullOrEmpty(item.Login))
            {
                return "Login empty";
            }
            if (string.IsNullOrEmpty(item.Password))
            {
                return "Password empty";
            }
            if (string.IsNullOrEmpty(item.Name))
            {
                return "Name empty";
            }
            if (string.IsNullOrEmpty(item.Email))
            {
                return "Email empty";
            }

            if (!repository.validateUnique(item.Login, item.Id))
            {
                return "This login is already used";
            }

            if (!repository.validateUnique(item.Email, item.Id))
            {
                return "This email is already used";
            }

            return null;
        }
    }
}
