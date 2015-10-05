using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using App.Models;
using App.Data;

namespace App.Web.Controllers
{
    public class PostController : Controller
    {
        [FromServices]
        public LibraryDbContext _libraryContext { get; set; }
        
        public IActionResult Index()
        {
            var models = _libraryContext.Posts.ToList();
            return View(models);
        }
    }
}