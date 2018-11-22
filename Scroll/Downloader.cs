using System.IO;
using System.Net;
using System.Net.Mime;

namespace Scroll
{
    public class Downloader
    {
        public static byte[] DownloadTile(int iTileX, int iTileY, int iTileZ, int scaler = 1)
        {
            string baseURL = "http://online3.map.bdimg.com/tile/";
            byte[] imageData;
            using (WebClient client = new WebClient())
            {
                client.QueryString.Add("qt", "vtile");
                client.QueryString.Add("x", iTileX.ToString());
                client.QueryString.Add("y", iTileY.ToString());
                client.QueryString.Add("z", iTileZ.ToString());
                client.QueryString.Add("styles", "pl");
                client.QueryString.Add("scaler", scaler.ToString());
                client.QueryString.Add("udt", System.DateTime.Today.ToString("yyyyMMdd"));
                imageData = client.DownloadData(baseURL);
            }
            return imageData;
        }
    }
}