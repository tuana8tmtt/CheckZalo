using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Windows.Forms;

namespace CheckZalo
{
    public class TinProxyHelper
    {
        private static readonly HttpClient client = new HttpClient();
        public string GetProxy(string KeyTinProxy)
        {
            var responseString = client.GetStringAsync("https://proxy.tinsoftsv.com/api/getProxy.php?key=" + KeyTinProxy);
            var result = responseString.Result.ToString();
            JObject result_json = JObject.Parse(result);
            bool flag = bool.Parse(result_json["success"].ToString()) == true;
            if (flag)
            {
                var IpProxy = result_json["proxy"].ToString();
                return IpProxy;
            }
            else
            {
                return "2";
            }
            return "2";
        }
        public string ChangeProxy(string KeyTinProxy)
        {
            var responseString = client.GetStringAsync("https://proxy.tinsoftsv.com/api/changeProxy.php?key=" + KeyTinProxy);
            var result = responseString.Result.ToString();
            JObject result_json = JObject.Parse(result);
            bool flag = bool.Parse(result_json["success"].ToString()) == true;
            string result_check = "FAIL";
            if (flag)
            {
                result_check = "OK";
            }
            else
            {
                result_check = result_json["next_change"].ToString();
            }
            return result_check;
        }
    }
}