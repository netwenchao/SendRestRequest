using Microsoft.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using FileUpload.Common;

namespace FileUpload
{
    public partial class FormTestRest : Form, IFormDataProvider
    {
        public FormTestRest()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (fileBrowser.ShowDialog() == DialogResult.OK)
            {
                tbFilePath.Text = fileBrowser.FileName;
            }
        }
        

        public void UploadFile()
        {
            using (var client = new HttpClient())
            {
                var fileName =tbFilePath.Text;
                var strUrl =tbUrl.Text;
                client.DefaultHeaders.Add(tbIdKey.Text,tbAppId.Text);
                var sign = PercentEncoding.GenerateSignature(tbUrl.Text,"post",GetFileBase64(), tbAppSecurity.Text);
                client.DefaultHeaders.Add(tbAppSecretyKey.Text, sign);

                var content = HttpContent.Create(File.OpenRead(fileName));
                try
                {
                    var response = client.Post(strUrl, content);
                    response.EnsureStatusIsSuccessful();
                    var readResult = response.Content.ReadAsString();
                    tbResult.Text = readResult;
                }
                catch(Exception exp)
                {
                    tbResult.Text = exp.Message;
                }
                finally
                {
                    content.Dispose();
                }
            }
        }

        public void DoRequest()
        {
            var methord = cbMethord.SelectedItem.ToString();
            if (methord.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                var rslt=HttpHelper.Instance.PostDataTo(tbUrl.Text, 
                    tbRequestBody.Text,
                    new Dictionary<string, string>()
                    {
                        {tbIdKey.Text, tbAppId.Text},
                        {tbAppSecretyKey.Text, PercentEncoding.GenerateSignature(tbUrl.Text,"post",tbRequestBody.Text,tbAppSecurity.Text)}
                    }
                );
                tbResult.Text = rslt;
            }
            else
            {
                var rslt= HttpHelper.Instance.GetResponseFrom(tbUrl.Text,
                    new Dictionary<string, string>()
                    {
                        {tbIdKey.Text, tbAppId.Text},
                        {tbAppSecretyKey.Text, PercentEncoding.GenerateSignature(tbUrl.Text,"get","",tbAppSecurity.Text)}
                    }
                );
                tbResult.Text = rslt;
            }
        }

        private string GetFileBase64()
        {
            using (var fileStream = File.OpenRead(tbFilePath.Text))
            {
                var bytes= new byte[fileStream.Length];
                fileStream.Read(bytes, 0, (int)fileStream.Length);
                return Convert.ToBase64String(bytes);
            }
        }
        
        private void btnRequest_Click(object sender, EventArgs e)
        {
            tbResult.Text = "";
            this.Text = string.Format("{0}-{1}",cbMethord.SelectedItem.ToString(),tbUrl.Text);
            if (cbUploadFile.Checked)
            {
                if (string.IsNullOrWhiteSpace(tbFilePath.Text))
                {
                    MessageBox.Show("请选择文件...");
                    return;
                }
                UploadFile();
            }
            else
            {
                try
                {
                    DoRequest();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        private void cbUploadFile_CheckedChanged(object sender, EventArgs e)
        {
            btnSelectFile.Enabled=cbUploadFile.Checked;
        }

        private void FormTestRest_Load(object sender, EventArgs e)
        {
            cbMethord.SelectedIndex = 0;
        }

        private void tbRequestBody_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if(e.KeyCode == Keys.A)
                { 
                    tbRequestBody.SelectAll();
                    return;
                }
            }

        }

        public FormDto GetFormData()
        {
            var formDto= new FormDto()
            {
                FormTitle = this.Text,
                FilePath = tbFilePath.Text,
                Url = tbUrl.Text,
                HttpMethord = cbMethord.SelectedItem.ToString(),
                IsSendFile = cbUploadFile.Checked,
                Headers = new List<HeaderDto>(),
                RequestBody = tbRequestBody.Text
            };
            formDto.Headers.Add(new HeaderDto(){Key = tbIdKey.Text,Value = tbAppId.Text});
            formDto.Headers.Add(new HeaderDto() { Key = tbAppSecretyKey.Text, Value = tbAppSecurity.Text });
            return formDto;
        }

        public void ApplyFormData(FormDto dto)
        {
            this.Text = dto.FormTitle;
            tbFilePath.Text = dto.FilePath;
            tbUrl.Text = dto.Url;
            cbMethord.SelectedItem = dto.HttpMethord;
            cbUploadFile.Checked = dto.IsSendFile;
            tbRequestBody.Text = dto.RequestBody;

            if(dto.Headers!=null && dto.Headers.Count > 0)
            { 
                tbIdKey.Text = dto.Headers[0].Key;
                tbAppId.Text = dto.Headers[0].Value;
            }

            if (dto.Headers != null && dto.Headers.Count > 0)
            {
                tbAppSecretyKey.Text = dto.Headers[1].Key;
                ;
                tbAppSecurity.Text = dto.Headers[1].Value;
            }
        }
    }
    public sealed class PercentEncoding
    {
        public static string GenerateSignature(string url, string method, string requestBody, string appSecret, StringBuilder log = null)
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
