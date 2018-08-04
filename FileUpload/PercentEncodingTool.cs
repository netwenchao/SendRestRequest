using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace FileUpload
{
    public partial class PercentEncodingTool : Form
    {
        public PercentEncodingTool()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            tbBaseString.Text =PercentEncoding.GetBaseString(tbUrl.Text, cbMethord.SelectedItem.ToString(),tbRequestBody.Text, tbAppSecret.Text);
            tbSignature.Text = PercentEncoding.GetSign(tbBaseString.Text);
        }

        public sealed class PercentEncoding
        {
            public static string GetBaseString(string url, string method, string requestBody, string appSecret, StringBuilder log = null)
            {
                List<string> combined = new List<string>();

                // request method
                combined.Add(method.ToUpper());

                Uri uri = new Uri(url);
                // scheme
                combined.Add(uri.Scheme.ToLower());
                // host
                combined.Add(uri.Host.ToLower());
                // port
                combined.Add(uri.Port.ToString());
                // path
                string path = uri.AbsolutePath.ToLower();
                path = path.Replace("\\", "/");
                if (path.EndsWith("/"))
                    path = path.Substring(0, path.Length - 1);
                combined.Add(PercentEncoding.Encode(path));

                // query string
                string q = (uri.Query ?? "").Trim();
                if (q.Length > 0)
                {
                    if (q.StartsWith("?"))
                        q = q.Substring(1);
                    string[] itemStrs = q.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
                    foreach (string itemStr in itemStrs)
                    {
                        if (itemStr.Trim().Length == 0) continue;
                        string key = "", value = "";

                        int index = itemStr.IndexOf("=");
                        if (index <= 0) // = is missing or key is missing, ignore
                        {
                            continue;
                        }
                        else
                        {
                            key = HttpUtility.UrlDecode(itemStr.Substring(0, index)).Trim().ToLower();
                            value = HttpUtility.UrlDecode(itemStr.Substring(index + 1)).Trim();
                            items.Add(new KeyValuePair<string, string>(key, value));
                        }
                    }

                    // query
                    combined.Add(String.Join("&",
                        items.OrderBy(t => t.Key).Select(t => String.Format("{0}={1}", PercentEncoding.Encode(t.Key), PercentEncoding.Encode(t.Value))).ToArray()));
                }
                else
                    combined.Add("");

                // body
                combined.Add(PercentEncoding.Encode(requestBody ?? ""));
                // salt
                combined.Add(appSecret);

                string baseString = String.Join("|", combined.ToArray());
                if (log != null)
                    log.AppendLine("Base String: " + baseString);
                return baseString;
            }

            public static string GetSign(string baseString)
            {
                System.Security.Cryptography.SHA256Managed s256 = new System.Security.Cryptography.SHA256Managed();
                byte[] buff;
                buff = s256.ComputeHash(Encoding.UTF8.GetBytes(baseString));
                s256.Clear();
                return Convert.ToBase64String(buff);
            }

            public static string Encode(string s)
            {
                string t = HttpUtility.UrlEncode(s);
                t = t.Replace("+", "%20");
                t = t.Replace("!", "%21");
                t = t.Replace("(", "%28");
                t = t.Replace(")", "%29");
                t = t.Replace("*", "%2a");
                return t;
            }
            public static string Decode(string s)
            {
                return HttpUtility.UrlDecode(s);
            }
        }
    }
}
