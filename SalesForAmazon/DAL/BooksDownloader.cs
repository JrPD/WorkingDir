using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SalesForAmazon.Models;
using System.Timers;

using BookParser;

using HtmlAgilityPack;

namespace SalesForAmazon.DAL
{
    using System.IO;
    using System.Net;

    using FluentSharp.CoreLib;


    public class BooksDownloader
    {
        public static BookInfoEntities DbContext;

        private static Timer timer;

        static BooksDownloader()
        {
            DbContext = new BookInfoEntities();
            OnTimedEvent(null,null);
            //timer = new Timer
            //            {
            //                Interval = 86400000,
            //                Enabled += new ElapsedEventHandler(OnTimedEvent)
            //            };
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            DbContext.Database.Delete();
            GetCategories(GetUrlContentsAsync(
                "http://www.amazon.com/Best-Sellers-Kindle-Store-eBooks/zgbs/digital-text/154606011/ref=zg_bs_unv_kstore_2_157632011_3"));
            foreach (var mainCategory in DbContext.Categories.Where( category => category.LevelName == 1))
            {
                
            }

        }

        private static void GetCategories(string mainUrl)
        {
            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true };
            htmlDoc.LoadHtml(mainUrl);
            if (htmlDoc.DocumentNode != null)
            {
                var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//ul[@id='zg_browseRoot']/ul/ul/ul");
                if (mainNode != null)
                {
                    var keys = mainNode.ChildNodes.Where(node => node.HasChildNodes)
                            .Select(node => node.FirstChild)
                            .Select(item => item.InnerText).toArray();
                    var values = mainNode.ChildNodes.Where(node => node.HasChildNodes)
                            .Select(node => node.FirstChild)
                            .Select(item => item.OuterHtml).toArray();
                    if (keys.notNull() && values.notNull())
                        for (var i = 0; i < values.Count(); i++)
                        {
                            var tmp = values[i].Remove(0, 9);
                            tmp = tmp.Remove(tmp.IndexOf('\''));
                            DbContext.Categories.Add(new Models.Category()
                                                         {
                                                             Name = keys[i], 
                                                             Url = tmp, 
                                                             LevelName = 1
                                                         });
                        }
                    DbContext.SaveChanges();
                }
            }
        }

        private static void GetSubCategory()
        {
            
        }

        private static string GetUrlContentsAsync(string url)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            using (var response = webReq.GetResponseAsync().Result)
                if (response != null) 
                    using (var sr = new StreamReader(response.GetResponseStream())) 
                        return sr.ReadToEnd();
            return string.Empty;
        }



    }
}