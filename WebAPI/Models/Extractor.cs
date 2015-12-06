using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebAPI.Models
{
    public class Extractor
    {
        

        public static IQueryable<DataCapsule> GetAllNews()
        {
            string[] seperator = { "\n", "\t", "\r" };

            List<DataCapsule> rs = new List<DataCapsule>();

            string url = "http://www.fit.hcmus.edu.vn/vn/";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            HtmlNode div =   doc.DocumentNode.SelectSingleNode("//*[@id='dnn_ctr989_ModuleContent']");

            
            var tables = div.SelectNodes(".//table").ToList();
            foreach (var table in tables)
            {
                DataCapsule dc = new DataCapsule();
                string Day =  table.SelectSingleNode(".//td[@class='day_month']").InnerHtml.Split(seperator, StringSplitOptions.RemoveEmptyEntries)[0];
                string Month = table.SelectSingleNode(".//tr[2]/td[@class='day_month']").InnerHtml.Split(seperator, StringSplitOptions.RemoveEmptyEntries)[0];
                string Year = table.SelectSingleNode(".//td[@class='post_year']").InnerHtml.Split(seperator, StringSplitOptions.RemoveEmptyEntries)[0];

                dc.Date = new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day));

                dc.Title = table.SelectSingleNode(".//td[@class='post_title']/a").InnerHtml.Split(seperator, StringSplitOptions.RemoveEmptyEntries)[0];

                rs.Add(dc);
            }


            return rs.AsQueryable();
        }

        
    }
}
