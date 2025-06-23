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
using System.Text.RegularExpressions;

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
                return StatusCode(403, GetError("Логін або пароль не правильні"));
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
                return BadRequest(GetError(errorStatus));
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
            Console.WriteLine("validating");
            Console.WriteLine(item.Login);
            Console.WriteLine(string.IsNullOrEmpty(item.Login));
            if (string.IsNullOrEmpty(item.Login))
            {
                return "Логін пустий";
            }
            if (string.IsNullOrEmpty(item.Password))
            {
                return "Пароль пустий";
            }
            if (string.IsNullOrEmpty(item.Email))
            {
                return "Е-мейл пустий";
            }
            
            if (!Regex.IsMatch(item.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                return "Неправильний е-мейл";
            }

            if (!repository.validateUnique(item.Login))
            {
                return "Цей логін вже використовується";
            }
            
            if (!string.IsNullOrEmpty(item.Number) && !Regex.IsMatch(item.Number, @"^\+?\d{10,15}$"))
            {
                return "Неправильний номер";
            }

            if (!repository.validateUnique(item.Email))
            {
                return "Цей е-мейл вже використовується";
            }

            return null;
        }
    }
}
