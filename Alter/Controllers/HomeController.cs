using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Alter.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Error()
        {
            return this.View();
        }
    }
}
