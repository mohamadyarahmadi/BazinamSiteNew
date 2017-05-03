using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bazinam.ServiceLayer.Contracts;


namespace Bazinam.web.Areas.Admin.Controllers
{
    public partial class CommentController : Controller
    {
        private readonly INewsService _newsService;
        public CommentController(INewsService newsService)
        {
            _newsService = newsService;
        }
        // GET: Admin/Comment
        public virtual ActionResult Index()
        {
            return View();
        }
        public virtual async Task<JsonResult> GetComments([Bind(Prefix = "pageSize")]int pageSize, [Bind(Prefix = "page")]int page)
        {
            var total1 = await _newsService.TotalOfComment();
            var commentVM = await _newsService.GetComments(pageSize, page);
            return Json(new { total = total1, data = commentVM }, JsonRequestBehavior.AllowGet);
        }
        // GET: Admin/Comment/Details/5
        public virtual async Task<ActionResult> Details(int id)
        {
            var result = await _newsService.GetComment((int)id);
            return View(result);
        }

        // GET: Admin/Comment/Create
        public virtual ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Comment/Create
        [HttpPost]
        public virtual ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        // GET: Admin/Comment/Answer
        public virtual async Task<ActionResult> Answer(int id)
        {
            var result = await _newsService.GetComment((int)id);
            return View(result);
        }

        // POST: Admin/Comment/Create
        [HttpPost]
        public virtual ActionResult Answer(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        // GET: Admin/Comment/Edit/5
        public virtual async Task<ActionResult> Edit(int id)
        {
            var result = await _newsService.GetComment((int)id);
            return View(result);
        }

        // POST: Admin/Comment/Edit/5
        [HttpPost]
        public virtual ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public virtual async Task<ActionResult> EditAnswer(int id)
        {
            var result = await _newsService.GetCommentAnswer((int)id);
            return View(result);
        }

        // POST: Admin/Comment/Edit/5
        [HttpPost]
        public virtual ActionResult EditAnswer(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Comment/Delete/5
        public virtual async Task<ActionResult> Delete(int id)
        {
            var result = await _newsService.GetComment((int)id);
            return View(result);
        }

        // POST: Admin/Comment/Delete/5
        [HttpPost]
        public virtual async Task<ActionResult> Delete(int id, FormCollection collection)
        {
            try
            {
                var commentVM = await _newsService.DeleteComment(id);
                return RedirectToAction("/Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
