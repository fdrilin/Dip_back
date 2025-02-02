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
        protected ErrorItem GetError(string message) 
        {
            return new ErrorItem(message);
        }
    }
}