using ExcelDataReader;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using Newtonsoft.Json.Linq;
using javax.script;
using Newtonsoft.Json;

namespace CheckZalo
{
    public partial class Form1 : Form
    {
        List<string> SdtList = new List<string>();
        static volatile bool paused = false;
        static volatile bool finished = false;
        private static readonly HttpClient client = new HttpClient();
        string profile_path;
        int success;
        public Form1()
        {
            InitializeComponent();
        }
        int dem1 = 0;
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void Update_ListView(int stt, string SDT,string status, string name, string email)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);  // this line was missing
            row.Cells[0].Value = stt;
            row.Cells[1].Value = SDT;
            row.Cells[2].Value = status;
            row.Cells[3].Value = email;
            row.Cells[4].Value = name;


            Invoke(new System.Action(() =>
            {
                dataGridView1.Rows.Add(row);
            }));
        }
        public void Update_Total(int total)
        {
            Invoke(new System.Action(() =>
            {
                label3.Text = "Tổng số: "+total.ToString();
            }));
        }
        
        public void Update_Success()
        {
            Invoke(new System.Action(() =>
            {
                label4.Text = "Thành công: " + success;
            }));
        }
        public async Task<string> CheckSdt(string stt, string sdt)
        {
            var handler = new HttpClientHandler{};

            handler.AutomaticDecompression = ~DecompressionMethods.All;

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://m.facebook.com/share_preview/?surl=https%3A%2F%2Fzalo.me%2F" + sdt))
                {
                    request.Headers.TryAddWithoutValidation("authority", "zalo.me");
                    request.Headers.TryAddWithoutValidation("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    request.Headers.TryAddWithoutValidation("accept-language", "vi,vi-VN;q=0.9,fr-FR;q=0.8,fr;q=0.7,en-US;q=0.6,en;q=0.5");
                    request.Headers.TryAddWithoutValidation("cache-control", "max-age=0");
                    request.Headers.TryAddWithoutValidation("cookie", "_ga=GA1.2.1047268475.1606667818; fpsend=150189; _hjid=5ee000d5-eee6-4bdd-b21e-119fd136c0a3; __zi=3000.SSZzejyD3jSkdkMgrmaCmp30_x662rZTVixdyjePKfCopE-ta1XSdZFCfREH6nQPCyIZiPuH7vG.1; __zi-legacy=3000.SSZzejyD3jSkdkMgrmaCmp30_x662rZTVixdyjePKfCopE-ta1XSdZFCfREH6nQPCyIZiPuH7vG.1; zpsid=TG-M.288636573.16.FQOdzwlUzfGsuCA5fzxsbDwjXAQJvi2ddUl1fv5bRGW89d2pgbIV6ydUzfG");
                    request.Headers.TryAddWithoutValidation("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"102\", \"Google Chrome\";v=\"102\"");
                    request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                    request.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "document");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "navigate");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "same-origin");
                    request.Headers.TryAddWithoutValidation("sec-fetch-user", "?1");
                    request.Headers.TryAddWithoutValidation("upgrade-insecure-requests", "1");
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.63 Safari/537.36");
                    request.Headers.TryAddWithoutValidation("cookie", "sb=KZ-XYkdo2GLQ-Upvv-j-RWMt; wd=2560x1297; datr=KZ-XYo2DO7Mn4XIE-CmEHpSB; c_user=100022013826417; xs=45%3AhHGPnFb3eqMpLA%3A2%3A1654103855%3A-1%3A13343; fr=0UvS5V7s3RQzSgNE6.AWXCYZgscYT6YhMBve17RJmnTq4.Bil58p.Fe.AAA.0.0.Bil58x.AWUIZEs-QIg; presence=C%7B%22t3%22%3A%5B%5D%2C%22utc3%22%3A1654103861594%2C%22v%22%3A1%7D");

                    var response = await httpClient.SendAsync(request);
                    var result = await response.Content.ReadAsStringAsync();
                    result = result.ToString();
                    MessageBox.Show(result);
                    bool flag = result.Contains("attachment[params][title]");
                    success++;
                    Update_Success();
                    if (flag)
                    {
                        bool flag2 = result.Contains("Zalo App");
                        bool flag3 = result.Contains("Zalo -");
                        if (flag2)
                        {
                            Update_ListView(Int32.Parse(stt), sdt, "Không có tài khoản", "", "");
                        }
                        else
                        {   
                            string pat = "(mfss fcb)(.+)(sharerAttachmentCaption)";
                            string pat2 = @"(=60&h=60&url=)(.+)(&ext=jpg&utld=zadn.vn)";

                            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                            Regex r2 = new Regex(pat2, RegexOptions.IgnoreCase);

                            MatchCollection name = r.Matches(result);
                            MatchCollection link_avt = r2.Matches(result);

                            //var img_url = link_avt[0].Value.Replace("safe_image.php?w=60&h=60&url=", "").Replace("&ext=jpg&utld=zadn.vn", "");
                            var name_zalo = name[0].Value.Replace("mfss fcb\\\">Zalo","").Replace("\\u003C\\/span>\\u003Cdiv class=\\\"sharerAttachmentCaption","");

                            Update_ListView(Int32.Parse(stt), sdt, "OK", "", name_zalo);

                            return ("Có SDT");
                        }
                        

                    }
                    else
                    {   
                        Update_ListView(Int32.Parse(stt), sdt, "Block", "", "");
                        return "2";
                    }
                    
                    return "2";
                }
            }
        }
        public void read_Excel()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var stream = File.Open(profile_path, FileMode.Open, FileAccess.Read);
            var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet();
            var tables = result.Tables;
            int count = tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                SdtList.Add(tables[0].Rows[i][0].ToString());
            }
            Update_Total(count);
        }
        public void run()
        {
            if (checkBox1.Checked)
            {
                for (int i = 0; i < textBox2.Lines.Length; i++)
                {
                    SdtList.Add(textBox2.Lines[i]);
                }
            }
            else
            {
                read_Excel();
            }
            for (int y = 0; y < SdtList.Count(); y++)
            {
                while (paused) { };
                CheckSdt(y.ToString(), SdtList[y]);

                Thread.Sleep(500);
                finished = true;


            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Clear();
            SdtList.Clear();
            ThreadStart ts = new ThreadStart(run);
            Thread thrd = new Thread(ts);
            thrd.IsBackground = false;
            thrd.Start();
            
            //var proxy = new TinProxyHelper();
            //var data = proxy.ChangeProxy("TLX1JQBeaPOumLaMo2gNJ6gedfm6NHqqft9nm6");
            //MessageBox.Show(data.ToString());
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                profile_path = openFileDialog1.FileName;
            }
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Creating DataTable
            System.Data.DataTable dt = new System.Data.DataTable();

            //Adding the Columns
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {   
                
                dt.Columns.Add(column.HeaderText);
            }

            //Adding the Rows
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                dt.Rows.Add();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    dt.Rows[dt.Rows.Count - 1][cell.ColumnIndex] = cell.Value.ToString();
                }
            }

            //Exporting to Excel
            string folderPath = "C:\\Excel\\";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                var temp = DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss");
                wb.Worksheets.Add(dt, "Customers");
                wb.SaveAs(folderPath + "OutPutCheckZalo-"+ temp + ".xlsx");
                MessageBox.Show(@"Copied Đường dẫn C:\Excel\" + temp + ".xlsx", "Thành công" );
                Clipboard.SetText(@"C:\Excel\" + temp + ".xlsx");

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            paused = true;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            paused = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
