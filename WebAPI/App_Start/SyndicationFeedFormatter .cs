using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Syndication;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using WebAPI.Models;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace WebAPI.App_Start
{
    public class SyndicationFeedFormatter : MediaTypeFormatter
    {
        private readonly string atom = "application/atom+xml";
        private readonly string rss = "application/rss+xml";

        public SyndicationFeedFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(atom));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(rss));
        }

        Func<Type, bool> SupportedType = (type) =>
        {
            if (type == typeof(DataCapsule) || type == typeof(IEnumerable<DataCapsule>))
                return true;
            else
                return false;
        };

        public override bool CanReadType(Type type)
        {
            return SupportedType(type);
        }

        public override bool CanWriteType(Type type)
        {
            return SupportedType(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                if (type == typeof(DataCapsule) || type == typeof(IEnumerable<DataCapsule>))
                    BuildSyndicationFeed(value, writeStream, content.Headers.ContentType.MediaType);
            });
        }

        private void BuildSyndicationFeed(object models, Stream stream, string contenttype)
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            var feed = new SyndicationFeed()
            {
                Title = new TextSyndicationContent("My Feed")
            };

            if (models is IEnumerable<DataCapsule>)
            {
                var enumerator = ((IEnumerable<DataCapsule>)models).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    items.Add(BuildSyndicationItem(enumerator.Current));
                }
            }
            else
            {
                items.Add(BuildSyndicationItem((DataCapsule)models));
            }

            feed.Items = items;

            using (XmlWriter writer = XmlWriter.Create(stream))
            {
                if (string.Equals(contenttype, rss))
                {
                    Rss20FeedFormatter rssformatter = new Rss20FeedFormatter(feed);
                    rssformatter.WriteTo(writer);
                   
                }
                else
                {
                    //Atom10FeedFormatter atomformatter = new Atom10FeedFormatter(feed);
                    //atomformatter.WriteTo(writer);

                    Rss20FeedFormatter rssformatter = new Rss20FeedFormatter(feed);
                    rssformatter.WriteTo(writer);
                }
            }
        }

        private SyndicationItem BuildSyndicationItem(DataCapsule u)
        {
            var item = new SyndicationItem()
            {
             
                Title = new TextSyndicationContent(u.Title),
                BaseUri = new Uri("fit.hcmus.edu.vn"),
                LastUpdatedTime = u.Date,
                Content = new TextSyndicationContent("Khoa Công nghệ Thông tin (CNTT) của Trường Đại học Khoa học Tự nhiên Tp. HCM  được thành lập  theo quyết định số 3818/GD-ĐT ngày 13/12/1994 của Bộ Trưởng Bộ GD&ĐT, dựa trên Bộ môn tin học của Khoa Toán Trường Đại học Tổng hợp Tp.HCM. Trải qua gần 16 năm hoạt động, Khoa đã phát triển vững chắc và được chính phủ bảo trợ để trở thành một trong những khoa CNTT đầu ngành trong hệ thống giáo dục đại học của Việt Nam.")
            };
            item.Authors.Add(new SyndicationPerson() { Name = "HTP"});
            return item;
        }
    }
}