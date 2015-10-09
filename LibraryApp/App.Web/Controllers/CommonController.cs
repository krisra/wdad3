using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using App.Models;
using App.Data;

namespace App.Web.Controllers
{
    public class CommonController : Controller
    {
        [FromServices]
        public ILibraryRepo _libraryRepo { get; set; }
    }
}
