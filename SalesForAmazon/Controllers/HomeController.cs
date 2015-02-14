using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookParser;
using FluentSharp.CoreLib;
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
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private List<Category> categoryList;
        readonly BookInfoEntities dbContext = new BookInfoEntities();

        public void InitializeList()
        {
            categoryList = Category.DefaultUrls();
            // щоб визначати вибрану категорію по цьому значенню
            // ато по айдішці у строці не дуже вигдядає
            foreach (var item in categoryList)
            {
                item.UrlName = Func.ParseURL(item.Name);
            }
        }

        public ActionResult Index(string id, int? page, int? PageSize)
        {
            // todo установка категорій. треба потім занести все в базу
            //нафіга!?!?!? яякщо в базу то не треба парсити         \\ взагалі-то навіть не треба
            //або як зараз я зробив                                 \\ хотів зробити у базі таблицю categories
            //або перестати гамнокодити і зробити динамічно         \\ а потім занести у базу
            //хіба так хоче замовник                                \\ але категорії не змінюються, так що не треба
            InitializeList();                                       
            // просто для демонстрації
            var books = from b in dbContext.Books
                                      select b;
            books = books.OrderBy(n => n.Name);
            var pageNumber = (page ?? 1);
            var pageSize = (PageSize ?? 4);
            var booksPaged = books.ToPagedList(pageNumber, pageSize);

            // треба передавати 2 моделі. 2-га для Partial
            var tuple = new Tuple<List<Category>, IPagedList<Book>>(categoryList, booksPaged);
            return View(tuple);
        }


        public ActionResult ParseBooks(IPagedList<Book> books)
        {
            return PartialView("BooksList", books);
        }
    }
}
