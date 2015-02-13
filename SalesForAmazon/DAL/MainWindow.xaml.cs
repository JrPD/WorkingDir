using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;
using Microsoft.Win32;

//#########################################################################
//#########################################################################
//#########################################################################
//#########################################################################

// ну я видалив все непотрібне
// твоя задача - при натисненні на посилання, екшн в контроллері приймав 
// значення категорії
// чи по id чи по назві
// спрощуй все до неможливості
//

//#########################################################################
//#########################################################################
//#########################################################################
//#########################################################################
using SalesForAmazon.Models;


namespace BookParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public  class MainWindow 
    {
        private readonly int MaxPageCount = 100;//change for max count
        private readonly BookInfoEntities _dbContext;
        private readonly BackgroundWorker worker; 
        private double _currectItem = 1;
        private double _maxItem = 1;
        private bool CurrentStepCompleted;
        List<string> contentList;
        private IEnumerable<string> urlList;


        public MainWindow()
        {
            _dbContext = new BookInfoEntities();
            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;           
            CurrentStepCompleted = false;
        }

        private void LoadListBtn_Click()
        {  
            //StatusLabel.Content = "Loading list wiht URLs...";
            // Make a list of web addresses.
            //urlList = SetUpList();
        }

        private async void ParseBtn_Click()
        {

            try
            {
                if (urlList != null)
                {
                    //StatusLabel.Content = "Downloading content for pages";
                    contentList = await LoadDataAsync(urlList);
                    if (contentList.Count!=0 & contentList!=null)
                    {
                        AllWorker(contentList);
                    }
                }
                else
                {
                    //StatusLabel.Content = "List not loaded";
                }
            }
            catch (NullReferenceException)
            {
                //MessageBox.Show("Неможливо загрузити список посилань!");
            }
        }

        private void SaveBtn_Click()
        {
            try
            {
                _dbContext.SaveChanges();
                if (_dbContext.Books.Any())
                {
                    //StatusLabel.Content = "Changes has been saved to database";
                }
                else
                {
                    //StatusLabel.Content = "Error saving changes";
                }
            
            }
            catch (Exception)
            {
                //StatusLabel.Content = "Error saving changes";
            }

        }

       
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //while (!CurrentStepCompleted)
            //{
            //    Dispatcher.Invoke(new Action(() =>
            //    {
            //        progressBar.Value =
            //            (_currectItem / _maxItem *
            //             100.00);
            //    }), DispatcherPriority.ContextIdle);
            //    Thread.Sleep(500);
            //}
        }

        public async void AllWorker(List<string> contentList)
        {
            
            CurrentStepCompleted = false;
            //progressBar.Value = 0.00;

            var pageCount = 20;
            // List of parsed books
            var books = new List<Book>();
            // list with content - not parsed
            // load content async
           
            // adding books to list
            List<string> bookUrls = new List<string>();
            _currectItem = 0;
            worker.RunWorkerAsync();

            //StatusLabel.Content = "Getting links...";
            foreach (var content in contentList)
            {
                // parse content async
                // add to parsed books list
                //books.Add(await Parse(content));
                bookUrls.AddRange(SelecrUrl(content));
                if (_currectItem < _maxItem)
                    _currectItem++;
            }
            bookUrls = bookUrls.Take(pageCount).ToList();
            contentList.Clear();

           // StatusLabel.Content = "Downloading books content...";
            foreach (var bookUrl in bookUrls)
            {
                string urlContents = await GetURLContentsAsync(bookUrl);
                contentList.Add(urlContents);
                if (_currectItem < _maxItem)
                    _currectItem++;
            }

            //StatusLabel.Content = "Parse books pages...";
            foreach (var content in contentList)
            {
                // parse content async
                // add to parsed books list
                books.Add(await Parse(content));
                if (_currectItem < _maxItem)
                    _currectItem++;
            }

            // Save to DB
            Save(books);
            CurrentStepCompleted = true;


        }

        //todo напевно не буде використовуватися
        // open file in disc
        // create urls list


        //public IEnumerable<string> SetUpList()
        //{
        //    // bad code
        //    Stream myStream = null;
        //    List<string> URLlist = null;
        //    OpenFileDialog openFileDialog = new OpenFileDialog();
        //    openFileDialog.InitialDirectory = "C:\\";
        //    openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
        //    openFileDialog.FilterIndex = 2;
        //    openFileDialog.RestoreDirectory = true;
        //    if (openFileDialog.ShowDialog() ==true)
        //    {
        //        try
        //        {
        //            if ((myStream = openFileDialog.OpenFile()) != null)
        //            {
        //                using (myStream)
        //                {
        //                    string file = openFileDialog.FileName;
        //                    PathTb.Text = file;
        //                    string text = File.ReadAllText(file);
        //                    URLlist = new List<string>();
        //                    string[] list = Regex.Split(text, "\r\n");

        //                    foreach (var item in list)
        //                    {
        //                        URLlist.Add(item);
        //                        URLList.Items.Add(item);
        //                    }
        //                    StatusLabel.Content = " URLs list loaded";
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            StatusLabel.Content = " Could not read file from disk";
        //        }
        //    }
        //    return URLlist;
        //}

        // Async content Download

        private async Task<List<string>> LoadDataAsync(IEnumerable<string> urlList)
        {
            //todo задаватиметься іншим чином
            var pageCount = 20;

            List<string> contentList = new List<string>();
            //todo перевірити скільки ссилок на книги нам треба якщо їх менше 20, тобто одна сторінка 
            // то все добре, якщо ж більше достати наступні ссилки на сторінки яких бракує у цьому місці
            //і потім усі потрібні ссилки вернути з цьої ф-ції
            _maxItem = urlList.Count() * pageCount * 2;
            try
            {
            foreach (var url in urlList)
            {
                var urlContents = await GetURLContentsAsync(url);
                contentList.Add(urlContents);
                urlContents = string.Empty;

                if (pageCount > 20)
                {
                    urlContents = await GetURLContentsAsync(url + "#2");
                    contentList.Add(urlContents);
                    urlContents = string.Empty;
                }

                if (pageCount > 40)
                {
                    urlContents = await GetURLContentsAsync(url + "#3");
                    contentList.Add(urlContents);
                    urlContents = string.Empty;
                }

                if (pageCount > 60)
                {
                    urlContents = await GetURLContentsAsync(url + "#4");
                    contentList.Add(urlContents);
                    urlContents = string.Empty;
                }

                if (pageCount > 80)
                {
                    urlContents = await GetURLContentsAsync(url + "#5");
                    contentList.Add(urlContents);
                    urlContents = string.Empty;
                }
                return contentList;
            }
            }
            catch (WebException)
            {
                //StatusLabel.Content = "Cannot access to the Internet";
            }
            return contentList;
        }

        // load content async
        private async Task<string> GetURLContentsAsync(string url)
        {
            // сам точно не знаю шо тут до чого. copy-paste
            var webReq = (HttpWebRequest)WebRequest.Create(url);

            using (WebResponse response = await webReq.GetResponseAsync())
            {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    // getting string
                    var responseJson = sr.ReadToEnd();
                    return responseJson;
                }
            }
        }

        public async Task<Book> Parse(string content)
        {
            lock(this)
            { 
            try
            {
                // load content to HTML doc
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.OptionFixNestedTags = true;
                htmlDoc.LoadHtml(content);

                if (htmlDoc.DocumentNode != null)
                {
                    // new book instance
                    Book book;
                    // select body node
                    HtmlNode mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='singlecolumnminwidth']");

                    if (mainNode != null)
                    {
                        // initialize variables
                        string _image = "";
                        string _Name = "";
                        string _Author = "";
                        int _Comments = new int();
                        double _Price  = new double();
                        int _BestSellersRank = new int();
                        string _Categories = "";
                        DateTime _PublicationDate = new DateTime();
                        HtmlNode workNode = null;

                        //Image
                        workNode = mainNode.SelectSingleNode("//img[@id='main-image']");
                        if (workNode != null)
                            _image = workNode.Attributes["src"].Value;

                        // Name
                        workNode = mainNode.SelectSingleNode("//span[@id='btAsinTitle']");
                        if (workNode != null)
                            _Name = workNode.ChildNodes[0].InnerText;

                        // Author
                        workNode = mainNode.SelectSingleNode("//div[@class='buying']/span");
                        if (workNode != null)
                            _Author = workNode.InnerHtml.ParseAuthor();

                        // Comments

                        workNode = mainNode.SelectSingleNode("//div[@class='fl gl5 mt3 txtnormal acrCount']/a");
                        if (workNode != null)
                            _Comments = workNode.ChildNodes[0].InnerText.ParseCount();

                        // Price
                        workNode = mainNode.SelectSingleNode("//b[@class='priceLarge']");
                        if (workNode != null)
                            _Price = workNode.InnerText.ParsePrice();

                        // Amazon Best Sellers Rank
                        workNode = mainNode.SelectSingleNode("//li[@id='SalesRank']");
                        if (workNode != null)
                            _BestSellersRank = workNode.InnerText.ParseRank();

                        // Categories
                        // select ul with categories
                        workNode = mainNode.SelectSingleNode("//ul[@class='zg_hrsr']");
                        // for each in li span a
                        // select category name
                        IEnumerable<Category> cont = null;
                        if (workNode != null)
                            cont = from li in workNode.Descendants("li")
                                   from span in li.Descendants("span")
                                   from a in span.Descendants("a")
                                   select new Category
                                   {
                                       Name = a.InnerText
                                   };
                        // list for cont (context)
                        List<string> categories = new List<string>();

                        // adding items from cont to list
                        if (cont != null)
                            foreach (Category item in cont)
                            {
                                if (!categories.Contains(item.Name))
                                {
                                    categories.Add(item.Name);
                                }
                            }

                        // make string
                        StringBuilder sb = new StringBuilder();
                        foreach (var category in categories)
                        {
                            sb.Append(category + '\r');
                        }
                        // return string
                        _Categories = sb.ToString();

                        // Publication Data
                        workNode = mainNode.SelectSingleNode("//input[@id='pubdate']");
                        if (workNode != null)
                            _PublicationDate = workNode.OuterHtml.ParseDate();

                        //new book
                        book = new Book()
                        {
                            Image = _image,
                            Name = _Name,
                            Author = _Author,
                            BestSellersRank = _BestSellersRank,
                            Categories = _Categories,
                            Comments = _Comments,
                            Price = _Price,
                            PublicationDate = _PublicationDate
                        };
                        // return book async
                        return book;
                    }
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show("Network  or parse Problem!");
            }
            return null;
            }
        }

        public List<string> SelecrUrl(string content)
        {
            // load content to HTML doc
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(content);

            if (htmlDoc.DocumentNode != null)
            {
                // select body node
                HtmlNode mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='zg_centerListWrapper']");

                if (mainNode != null)
                {
                    // select ul with categories
                    var nodes = mainNode.SelectNodes("//div[@class='zg_title']");
                    var cont = from a in nodes.Descendants("a")
                        select a.Attributes["href"].Value.Replace("\n", "");

                    // list for cont (context)
                    List<string> bookURLs = new List<string>();

                    // adding items from cont to list
                    foreach (var item in cont)
                    {
                        if (!bookURLs.Contains(item))
                        {
                            bookURLs.Add(item);
                        }
                    }
                    return bookURLs;
                }
            }
            return new List<string>();
        }
      
        private  void Save(List<Book> books )
        {
            // ID adding
            int maxBookId = 0;
            try
            {
                maxBookId = _dbContext.Books.Select(r => r.Id).Max();
            }
            catch (Exception)
            {
                maxBookId = 1;
            }
            try
            {
                if (books.Count!=0 & books!=null)
                {
                    foreach (var book in books)
                    {
                        //book.Id = maxBookId + 1;
                        maxBookId++;
                        _dbContext.Books.Add(book);
                    }
                    //StatusLabel.Content = "All done. You can save content to database.";

                }
                else
                {
                    //StatusLabel.Content = "List of parsed books is empty";
                    
                }
               
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
    }
}
