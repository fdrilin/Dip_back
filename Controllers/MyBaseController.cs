using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Repositories;
using ZstdSharp.Unsafe;
using Mysqlx;

namespace TodoApi.Controllers
{
    public class MyBaseController : ControllerBase
    {
        protected UserItem? currentUser { get; set; } = null;

        public void BeforeAction()
        {
            string authHeader = Request.Headers["Authorization"].ToString() ?? "";

            if (authHeader == "")
            {
                return;
            }
            if (!authHeader.StartsWith("Bearer "))
            {
                return;
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();

            UserRepository repository = new();

            currentUser = repository.getUserItemByToken(token);

            if (currentUser == null)
            {
                Console.WriteLine("User token not valid");
                return;
            }
            Console.WriteLine(currentUser);
        }

        protected bool isAdmin()
        {
            return currentUser != null && currentUser.Admin == 1;
        }

        protected IActionResult? checkAdmin()
        {
            return isAdmin() ? null : StatusCode(403, "admin only");
        }

        protected ErrorItem GetError(string message)
        {
            return new ErrorItem(message);
        }
    }
}