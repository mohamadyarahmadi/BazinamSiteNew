using Bazinam.ServiceLayer.Contracts;
using Bazinam.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bazinam.web.Areas.Admin.Controllers
{
    public partial class NewsController : Controller
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }
        // GET: Admin/News
        [AllowAnonymous]
        public virtual ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public virtual async Task<JsonResult> News([Bind(Prefix = "page")]int page, [Bind(Prefix = "start")]int start, [Bind(Prefix = "limit")]int limit)
        {
            var NewsVM = await _newsService.GetNewsWithPagging(10, 1);
            
            string title = "salam";
            for(int i = 0; i < 100; i++) {
                var News = new NewsMV() {
                    NewsID = i,
                    Title = NewsVM[0].Title +"-"+ i,
                    Content = NewsVM[0].Content,
                    ReleaseDate = NewsVM[0].ReleaseDate
                };                
                NewsVM.Add(News);
            }
            return Json(new
            {
                total = 100,
                data = NewsVM.Skip(start).Take(limit)

            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPut]
        public virtual async Task<ActionResult> News()
        {
            return Json(new
            {
                data= "success"

            }, JsonRequestBehavior.AllowGet);
        }
        public virtual async Task<JsonResult> GetNewsPicWithPagging([Bind(Prefix = "pageSize")]int pageSize, [Bind(Prefix = "page")]int page)
        {
            /*using (SystemDbContext context = new SystemDbContext())
            {
                var total1 = await context.Pictures.CountAsync();
                var PictureVM = await context.Pictures
                  .Select(x => new ViewModel.Picture()
                  {
                      PictureID = x.PictureID,
                      PicName = x.PicName,
                      PicUrl = x.PicUrl
                  }).OrderBy(row => row.PictureID).Skip((page - 1) * 10).Take(pageSize).ToListAsync();
                return Json(new { total = total1, data = PictureVM }, JsonRequestBehavior.AllowGet);
            }*/
            var total1 = await _newsService.TotalOfPicture();
            var PictureVM1 = await _newsService.GetNewsPicWithPagging(pageSize, page);
            return Json(new { total = total1, data = PictureVM1 }, JsonRequestBehavior.AllowGet);
        }
        public virtual async Task<ActionResult> GetImage(int? id)
        {
            if (id == null) return null;
            /*using (SystemDbContext context = new SystemDbContext())
            {
                Models.Picture pic = new Models.Picture();
                pic = await context.Pictures.FindAsync(id);
                return base.File(pic.PicSourceBytes, "image/jpeg");
            }*/
            byte[] buffer = (await _newsService.GetImage((int)id)).PicSourceBytes;
            return base.File(buffer, "image/jpeg");
        }
        public virtual async Task<JsonResult> GetNewsWithPagging([Bind(Prefix = "pageSize")]int pageSize, [Bind(Prefix = "page")]int page)
        {
            /*using (SystemDbContext context = new SystemDbContext())
            {

                PersianCalendar pc = new PersianCalendar();
                var total1 = await context.News.CountAsync();
                var PictureVM = await context.News
                  .Select(x => new ViewModel.News()
                  {
                      NewsID = x.NewsID,
                      Title = x.Title,
                      Content = x.Content.Substring(0, x.Content.Length > 50 ? 50 : x.Content.Length),
                      ReleaseDate = x.ReleaseDate.ToString()
                  }).OrderBy(row => row.NewsID).Skip((page - 1) * 10).Take(pageSize).ToListAsync();
                return Json(new { total = total1, data = PictureVM }, JsonRequestBehavior.AllowGet);
            }*/
            var total1 = await _newsService.TotalOfNews();
            var NewsVM = await _newsService.GetNewsWithPagging(pageSize, page);
            return Json(new { total = total1, data = NewsVM }, JsonRequestBehavior.AllowGet);
        }
        // GET: Admin/News/Details/5
        public virtual async Task<ActionResult> Details(int? id)
        {
            if (id == null) return View();
            /*using (SystemDbContext context = new SystemDbContext())
            {
                var result = await context.News.Where(i => i.NewsID == id)
                    .Select(x => new ViewModel.News()
                    {
                        NewsID = x.NewsID,
                        Title = x.Title,
                        Content = x.Content,
                        ReleaseDate = x.ReleaseDate.ToString()
                    }).FirstAsync();
                return View(result);
            }*/
            var result = await _newsService.GetNews((int)id);
            return View(result);
        }

        // GET: Admin/News/Create
        public virtual ActionResult Create()
        {
            return View();
        }

        // POST: Admin/News/Create
        [HttpPost]
        public virtual async Task<ActionResult> Create(NewsMV _new)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    /* Models.News news = new Models.News()
                     {
                         Title = _new.Title,
                         Content = _new.Content,
                         ReleaseDate = DateTime.Parse(_new.ReleaseDate)
                     };
                     if (Session["NewsFile"] != null)
                     {
                         var files = (List<string>)Session["NewsFile"];
                         foreach (var fileName in files)
                         {
                             byte[] result;
                             using (FileStream SourceStream = System.IO.File.Open(Server.MapPath("~/Areas/Admin/TempPic/") + fileName, FileMode.Open))
                             {
                                 result = new byte[SourceStream.Length];
                                 await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
                             }
                             Models.Picture pic = new Models.Picture()
                             {
                                 PicName = fileName,
                                 PicSourceBytes = result,
                                 PicUrl = "",
                                 IsRefrence = false
                             };
                             news.PicturescCollection.Add(pic);
                         }

                     }
                     using (SystemDbContext context = new SystemDbContext())
                     {
                         context.News.Add(news);
                         await context.SaveChangesAsync();
                     }*/
                    var files = (List<string>)Session["NewsFile"] ?? new List<string>();
                    await _newsService.CreateNews(_new, files, Server.MapPath("~/Areas/Admin/TempPic/"));
                    Session["NewsFile"] = null;
                }
                return RedirectToAction("/Index");
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public virtual ActionResult UploadNewsPic(IEnumerable<HttpPostedFileBase> files)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/Areas/Admin/TempPic"), fileName);

                    // The files are not actually saved in this demo
                    file.SaveAs(physicalPath);
                    if (Session["NewsFile"] == null)
                    {
                        var picList = new List<string>();
                        Session["NewsFile"] = picList;
                    }
                    ((List<string>)Session["NewsFile"]).Add(fileName);
                }
            }

            // Return an empty string to signify success
            return Content("{data:'success'}");
        }
        public virtual async Task<ActionResult> RemovePic(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/Areas/Admin/TempPic"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        System.IO.File.Delete(physicalPath);
                        if (Session["NewsFile"] != null)
                        {
                            ((List<string>)Session["NewsFile"]).Remove(fileName);
                        }

                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }
        // GET: Admin/News/Edit/5
        public virtual async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return View();
            /* using (SystemDbContext context = new SystemDbContext())
             {
                 var result = await context.News.Where(i => i.NewsID == id)
                     .Select(x => new ViewModel.News()
                     {
                         NewsID = x.NewsID,
                         Title = x.Title,
                         Content = x.Content,
                         ReleaseDate = x.ReleaseDate.ToString()
                     }).FirstAsync();
                 return View(result);
             }*/
            var result = await _newsService.GetNews((int)id);
            return View(result);
        }

        // POST: Admin/News/Edit/5
        [HttpPost]
        public virtual async Task<ActionResult> Edit(NewsMV _new)
        {
            try
            {
                // TODO: Add update logic here
                /*var editedNews = new Models.News()
                {
                    NewsID = id,
                    Title = _new.Title,
                    Content = _new.Content,
                    ReleaseDate = DateTime.Parse(_new.ReleaseDate)
                };
                using (SystemDbContext context = new SystemDbContext())
                {
                    context.Entry(editedNews).State = System.Data.Entity.EntityState.Modified;
                    await context.SaveChangesAsync();
                }*/
                await _newsService.EditNews(_new);
                return RedirectToAction("/Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/News/Delete/5
        public virtual async Task<ActionResult> Delete(int id)
        {
            /* using (SystemDbContext context = new SystemDbContext())
             {
                 var result = await context.News.Where(i => i.NewsID == id)
                     .Select(x => new ViewModel.News()
                     {
                         NewsID = x.NewsID,
                         Title = x.Title,
                         Content = x.Content,
                         ReleaseDate = x.ReleaseDate.ToString()
                     }).FirstAsync();
                 return View(result);
             }*/
            var result = await _newsService.GetNews((int)id);
            return View(result);
        }

        // POST: Admin/News/Delete/5
        [HttpPost]
        public virtual async Task<ActionResult> Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                /* using (SystemDbContext context = new SystemDbContext())
                 {
                     var deletedNews = await context.News.FindAsync(id);
                     context.News.Remove(deletedNews);
                     await context.SaveChangesAsync();
                 }*/
                await _newsService.Delete(id);
                return RedirectToAction("/Index");
                }
            catch
            {
                return View();
            }
        }
    }
}
