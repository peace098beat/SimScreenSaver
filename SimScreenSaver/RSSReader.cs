using System.ServiceModel.Syndication;
using System.Xml;

namespace SimScreenSaver
{
    static public class RSSReader
    {

        static void _()
        {
            string url = @"https://www.google.co.jp/alerts/feeds/09047520966360389555/18173224082898862477";

            using (XmlReader rdr = XmlReader.Create(url))
            {
                SyndicationFeed feed = SyndicationFeed.Load(rdr);

                foreach (SyndicationItem item in feed.Items)
                {
                    TextSyndicationContent c = (TextSyndicationContent)item.Content;

                    //Console.WriteLine("item Title:" + item.Title.Text);
                    //Console.WriteLine("item Title:" + item.Content.Type);
                    //Console.WriteLine("item Title:" + c.Text);

                    //Console.WriteLine("link:" + (item.Links.Count > 0
                    //                ? item.Links[0].Uri.AbsolutePath : ""));
                }
            }
        }
    }
}
