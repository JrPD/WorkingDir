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
            categoryList = new List<Category>();
                                                                          
            categoryList.Add(new Category("Arts & Photography",           "http://www.amazon.com/Best-Sellers-Kindle-Store-Arts-Photography/zgbs/digital-text/154607011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Biographies & Memoirs",        "http://www.amazon.com/Best-Sellers-Kindle-Store-Biographies-Memoirs/zgbs/digital-text/154754011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Business & Money",             "http://www.amazon.com/Best-Sellers-Kindle-Store-Business-Investing/zgbs/digital-text/154821011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Children's eBooks",            "http://www.amazon.com/Best-Sellers-Kindle-Store-Childrens-eBooks/zgbs/digital-text/155009011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Comics & Graphic Novels",      "http://www.amazon.com/Best-Sellers-Kindle-Store-Comics-Graphic-Novels/zgbs/digital-text/156104011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
                                                                          
            categoryList.Add(new Category("Computers & Technology",       "http://www.amazon.com/Best-Sellers-Kindle-Store-Computers-Technology/zgbs/digital-text/156116011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Cookbooks, Food & Wine",       "http://www.amazon.com/Best-Sellers-Kindle-Store-Cookbooks-Food-Wine/zgbs/digital-text/156154011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Crafts, Hobbies & Home",       "http://www.amazon.com/Best-Sellers-Kindle-Store-Crafts-Hobbies-Home/zgbs/digital-text/156699011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Education & Teaching",         "http://www.amazon.com/Best-Sellers-Kindle-Store-Education-Teaching/zgbs/digital-text/158125011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Foreign Languages",            "http://www.amazon.com/Best-Sellers-Kindle-Store-Foreign-Language-eBooks/zgbs/digital-text/7735160011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));

            categoryList.Add(new Category("Health, Fitness & Dieting",    "http://www.amazon.com/Best-Sellers-Kindle-Store-Health-Fitness-Dieting/zgbs/digital-text/156430011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("History",                      "http://www.amazon.com/Best-Sellers-Kindle-Store-History/zgbs/digital-text/156576011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Humor & Entertainment",        "http://www.amazon.com/Best-Sellers-Kindle-Store-Humor-Entertainment/zgbs/digital-text/156279011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Lesbian, Gay, Bisexual &" +
                                          " Transgender eBooks",          "http://www.amazon.com/Best-Sellers-Kindle-Store-LGBT-eBooks/zgbs/digital-text/156424011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Literature & Fiction",         "http://www.amazon.com/Best-Sellers-Kindle-Store-Literature-Fiction/zgbs/digital-text/157028011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));

            categoryList.Add(new Category("Mystery, Thriller & Suspense", "http://www.amazon.com/Best-Sellers-Kindle-Store-Mystery-Thriller-Suspense/zgbs/digital-text/157305011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Nonfiction",                   "http://www.amazon.com/Best-Sellers-Kindle-Store-Nonfiction/zgbs/digital-text/157325011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Parenting & Relationships",    "http://www.amazon.com/Best-Sellers-Kindle-Store-Parenting-Relationships/zgbs/digital-text/157584011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Politics & Social Sciences",   "http://www.amazon.com/Best-Sellers-Kindle-Store-Politics-Social-Sciences/zgbs/digital-text/305951011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Professional & Technical",     "http://www.amazon.com/Best-Sellers-Kindle-Store-Professional-Technical/zgbs/digital-text/157626011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));

            categoryList.Add(new Category("Reference",                    "http://www.amazon.com/Best-Sellers-Kindle-Store-Reference-eBooks/zgbs/digital-text/9154158011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Religion & Spirituality",      "http://www.amazon.com/Best-Sellers-Kindle-Store-Religion-Spirituality/zgbs/digital-text/158280011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Romance",                      "http://www.amazon.com/Best-Sellers-Kindle-Store-Romance/zgbs/digital-text/158566011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Science & Math",               "http://www.amazon.com/Best-Sellers-Kindle-Store-Science-Math/zgbs/digital-text/158597011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Science Fiction & Fantasy",    "http://www.amazon.com/Best-Sellers-Kindle-Store-Science-Fiction-Fantasy/zgbs/digital-text/668010011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));

            categoryList.Add(new Category("Self-Help",                    "http://www.amazon.com/Best-Sellers-Kindle-Store-Self-Help/zgbs/digital-text/156563011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Sports & Outdoors",            "http://www.amazon.com/Best-Sellers-Kindle-Store-Sports-Outdoors/zgbs/digital-text/159818011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Teen & Young Adult",           "http://www.amazon.com/Best-Sellers-Kindle-Store-Teen-Young-Adult-eBooks/zgbs/digital-text/3511261011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));
            categoryList.Add(new Category("Travel",                       "http://www.amazon.com/Best-Sellers-Kindle-Store-Travel/zgbs/digital-text/159936011/ref=zg_bs_nav_kstore_2_154606011?_encoding=UTF8&tf=1"));

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
            InitializeList();
            // просто для демонстрації
            IEnumerable<Book> books = from b in dbContext.Books
                select b;

            // треба передавати 2 моделі. 2-га для Partial
            var tuple = new Tuple<List<Category>, IEnumerable<Book>>(categoryList, books);
            return View(tuple);
        }


        public ActionResult ParseBooks( IQueryable<SalesForAmazon.Models.Book> books)
        {
            return PartialView("BooksList", books);
        }
    }
}
