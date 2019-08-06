using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox3.Text = GetResult(
                GetHisogram(Resize(@"C:\Users\tom86\Desktop\85491.png", @"C:\Users\tom86\Desktop\85491-AAA.png")),
                GetHisogram(Resize(@"C:\Users\tom86\Desktop\85490.png", @"C:\Users\tom86\Desktop\85490-BBB.png"))
                ).ToString();

            //comboBox3.Text = CheckImg(@"C:\Users\tom86\Desktop\85491.png", @"C:\Users\tom86\Desktop\85491.png").ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var url = "http://120.24.220.115:8080/Lafite_war/a/automation/testAPI/interface/insertSituation";

            var body = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "file", "文件"}
                });
            comboBox1.Text = Post(url, body);

            //Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("file", "文件");
            //comboBox1.Text = CreatePostHttpResponse(url, parameters).ContentLength.ToString();

            //post请求并调用
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //dic.Add("file", "文件");
            //comboBox1.Text = GetResponseString(CreatePostHttpResponse(url, dic));

            //get请求并调用
            //comboBox1.Text = GetResponseString(CreateGetHttpResponse(url));

            //HttpDldFile df = new HttpDldFile();
            //comboBox2.Text = df.Download(url, @"D:\Temp\1").ToString();

            //comboBox2.Text = HttpDownloadFile(url, @"D:\Temp\1");

            //comboBox2.Text = PostDataGetHtml(url, "file:文件");

            comboBox2.Text = Post2(url, new Dictionary<string, string>
                {
                    { "file", "文件"}
                });

            //comboBox2.Text = PostData(url, "file:文件");

            //comboBox3.Text = Post1(url, "file=文件") + Post1(url, "file:文件");
        }

        #region 图像相似度对比
        /// <summary>
        /// 将图像转化成相同大小，我们暂且转化成256 X 256
        /// </summary>
        /// <param name="imageFile">需要对比的图片地址</param>
        /// <param name="newImageFile">转化后的新图片地址</param>
        /// <returns></returns>
        public Bitmap Resize(string imageFile, string newImageFile)
        {
            Bitmap imgOutput = new Bitmap(Image.FromFile(imageFile), 256, 256);
            imgOutput.Save(newImageFile, ImageFormat.Jpeg);
            imgOutput.Dispose();

            return (Bitmap)Image.FromFile(newImageFile);
        }

        //计算图像的直方图
        public int[] GetHisogram(Bitmap img)
        {
            BitmapData data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int[] histogram = new int[256];

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;

                int remain = data.Stride - data.Width * 3;

                for (int i = 0; i < histogram.Length; i++)
                    histogram[i] = 0;
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        int mean = ptr[0] + ptr[1] + ptr[2];

                        mean /= 3;

                        histogram[mean]++;

                        ptr += 3;

                    }
                    ptr += remain;
                }
            }
            img.UnlockBits(data);
            return histogram;
        }

        //计算相减后的绝对值
        private float GetAbs(int firstNum, int secondNum)
        {
            float abs = Math.Abs((float)firstNum - (float)secondNum);
            float result = Math.Max(firstNum, secondNum);

            if (result == 0)
                result = 1;

            return abs / result;
        }

        //最终计算结果
        public float GetResult(int[] firstNum, int[] scondNum)
        {
            if (firstNum.Length != scondNum.Length)
                return 0;
            else
            {
                float result = 0;
                int j = firstNum.Length;

                for (int i = 0; i < j; i++)
                {
                    result += 1 - GetAbs(firstNum[i], scondNum[i]);

                    Console.WriteLine(i + "----" + result);
                }

                return result / j;
            }
        }

        #endregion

        public bool CheckImg(string filePath1, string filePath2)
        {
            MemoryStream ms1 = new MemoryStream();
            Image image1 = Image.FromFile(filePath1);
            image1.Save(ms1, System.Drawing.Imaging.ImageFormat.Png);

            string img1 = Convert.ToBase64String(ms1.ToArray());

            Image image2 = Image.FromFile(filePath2);
            image2.Save(ms1, System.Drawing.Imaging.ImageFormat.Png);
            string img2 = Convert.ToBase64String(ms1.ToArray());

            if (img1.Equals(img2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// POST传值
        /// </summary>
        /// <param name="posturl">传递的地址</param>
        /// <param name="postData">传递的内容（如：a=1&b=2&c=3）</param>
        /// <returns>返回结果</returns>
        public string PostData(string posturl, string postData)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            byte[] data = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(posturl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return string.Empty;
            }
        }

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        private string Post2(string url, Dictionary<string, string> dic)
        {
            string result = string.Empty;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// HttpWebRequest 通过Post
        /// </summary>
        /// <param name="url">URI</param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static string PostDataGetHtml(string url, string postData)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(postData);

                Uri uri = new Uri(url);
                HttpWebRequest req = WebRequest.Create(uri) as HttpWebRequest;
                if (req == null)
                {
                    return "Network error:" + new ArgumentNullException("httpWebRequest").Message;
                }
                req.Method = "POST";
                req.KeepAlive = true;
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = data.Length;
                req.AllowAutoRedirect = true;

                Stream outStream = req.GetRequestStream();
                outStream.Write(data, 0, data.Length);
                outStream.Close();

                var res = req.GetResponse() as HttpWebResponse;
                if (res == null)
                {
                    return "Network error:" + new ArgumentNullException("HttpWebResponse").Message;
                }
                Stream inStream = res.GetResponseStream();
                var sr = new StreamReader(inStream, Encoding.UTF8);
                string htmlResult = sr.ReadToEnd();

                return htmlResult;
            }
            catch (Exception ex)
            {
                return "网络错误(Network error)：" + ex.Message;
            }
        }

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <param name="content">Post提交数据内容(utf-8编码的)</param>
        /// <returns></returns>
        public static string Post1(string url, string content)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            #region 添加Post 参数
            byte[] data = Encoding.UTF8.GetBytes(content);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        
        public string Post(string url, FormUrlEncodedContent body)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");//设置请求头
                body.Headers.ContentType.CharSet = "utf-8";
                // response
                var response = httpClient.PostAsync(url, body).Result;
                var data = response.Content.ReadAsStringAsync().Result;
                return data;//接口调用成功数据
            }
        }

        /// <summary>
        /// 发送http post请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="parameters">查询参数集合</param>
        /// <returns></returns>
        public HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;//创建请求对象
            request.Method = "POST";//请求方式
            request.ContentType = "application/x-www-form-urlencoded";//链接类型
                                                                      //构造查询字符串
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                bool first = true;
                foreach (string key in parameters.Keys)
                {

                    if (!first)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        first = false;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                //写入请求流
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 发送http Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpWebResponse CreateGetHttpResponse(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";//链接类型
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 从HttpWebResponse对象中提取响应的数据转换为字符串
        /// </summary>
        /// <param name="webresponse"></param>
        /// <returns></returns>
        public string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

    }
}
