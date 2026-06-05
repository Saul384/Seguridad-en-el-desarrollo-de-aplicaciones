using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace VulnerableApp.Controllers
{
    public class CommentController : Controller
    {
        // Lista estática para almacenar los comentarios en memoria
        private static List<string> _comments = new();

        public IActionResult Index()
        {
            return View(_comments);
        }

        [HttpPost]
        public IActionResult AddComment(string comment)
        {
            if (!string.IsNullOrEmpty(comment))
            {
                _comments.Add(comment);
            }
            return RedirectToAction("Index");
        }
    }
}