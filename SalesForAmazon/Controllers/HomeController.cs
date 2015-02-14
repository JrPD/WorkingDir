using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookParser;
using FluentSharp.CoreLib;
using SalesForAmazon.Models;
using Category = SalesForAmazon.Models.Category;

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
            // ато по айдішні у строці не дуже вигдядає
            foreach (var item in categoryList)
            {
                item.UrlName = Func.ParseURL(item.Name);
            }
        }

        public ActionResult Index(string id)
        {
            // todo установка категорій. треба потім занести все в базу
            //нафіга!?!?!? яякщо в базу то не треба парсити
            //або як зараз я зробив
            //або перестати гамнокодити і зробити динамічно
            //хіба так хоче замовник 
            InitializeList();
            // просто для демонстрації
            IEnumerable<Book> books = from b in dbContext.Books
                                      select b;

            // треба передавати 2 моделі. 2-га для Partial
            var tuple = new Tuple<List<Category>, IEnumerable<Book>>(categoryList, books);
            return View(tuple);
        }


        public ActionResult ParseBooks(IQueryable<SalesForAmazon.Models.Book> books)
        {
            return PartialView("BooksList", books);
        }
    }
}
