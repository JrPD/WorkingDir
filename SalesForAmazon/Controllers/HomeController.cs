using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookParser;
using FluentSharp.CoreLib;
using NLog.Fluent;
using SalesForAmazon.Models;
using Category = SalesForAmazon.Models.Category;
using PagedList;

/********************************************************************/
// todo треба зробити так, щоб по виборі певної категорії робився запит - парсер і потім відображало у BooksList
// todo при виборі книги(щолк на книгу чи на посилання Details) відкривалося нове вікно з детальною інфою
// todo десь там повинен бути dropdownlist, що вказуватиме кількість книг на сторінці
// todo зробити pagedlist для гортання сторінок

/********************************************************************/



namespace SalesForAmazon.Controllers
{
    using System.Collections;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private List<Category> categoryList;
        private readonly NLog.Logger Log = MvcApplication.logger;

        public void InitializeList()
        {
            //categoryList = Category.DefaultUrls();
            // щоб визначати вибрану категорію по цьому значенню
            // ато по айдішці у строці не дуже вигдядає
            //foreach (var item in categoryList)
            //{
            //    item.UrlName = Func.ParseURL(item.Name);
            //}

            ViewBag.dropdownCount = new SelectList(new Dictionary<string, int>
            {
                {"2", 2},
                {"3", 3},
                {"4", 4},
                {"20", 20},
                {"40", 40},
                {"60", 60},
                {"80", 80},
                {"100", 100}
            }, "Key", "Value");
            ViewBag.counts = 4;
        }

        public ActionResult Index(string id, int? page, int? PageSize)
        {
            if (ViewBag.count==null)
            {
                ViewBag.count = 4;
            }
            ViewBag.counts = (PageSize.isNotNull()) ? ViewBag.count : 4;
            Log.Info("Creating new project from {0} at {1}", (int)ViewBag.counts, page);

            // todo установка категорій. треба потім занести все в базу
            InitializeList();                                       
            // просто для демонстрації
            //var books = from b in
            //                dbContext.Books.
            //                OrderBy(n => n.Name)
            //            select b;
            var pageNumber = (page ?? 1);
            Log.Info("Creating new project from {0} at {1}", (int)ViewBag.counts, page);

            var pageSize = (PageSize ?? (int)ViewBag.counts);
            //var booksPaged = books.ToPagedList(pageNumber, pageSize);
            //var worker = new MainWindow();
                //var urlList = categoryList.Where(y => y.UrlName == id).Select(x => x.Url).toList();
                //var books = worker.AllWorker(worker.LoadData (urlList));
            // треба передавати 2 моделі. 2-га для Partial
            var tuple = new Tuple<List<Category>, IPagedList<Book>>(categoryList, null);
            return View(tuple);
        }

        public ActionResult ParseBooks(IPagedList<Book> books)
        {
            return PartialView("BooksList", books);
        }
    }
}
