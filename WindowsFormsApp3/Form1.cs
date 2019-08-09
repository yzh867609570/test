using SpeechLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using Baidu.Aip.Speech;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            comboBox6.Text = @"C:\Users\tom86\Desktop\";
            comboBox7.Text = @"C:\Users\tom86\Desktop\";

            comboBox4.Text = @"C:\Users\tom86\Desktop\";
        }
        //文字转语音
        private void btnSave_Click(object sender, EventArgs e)
        {
            VoiceToText(comboBox4.Text);
        }
        //文字转语音录频
        private void btnSpeak_Click(object sender, EventArgs e)
        {
            SpFileStream stream = new SpFileStream();
            stream.Open(@"C:\Users\tom86\Desktop\voice.wav", SpeechStreamFileMode.SSFMCreateForWrite, false);
            SpVoice voice = new SpVoice();
            voice.AudioOutputStream = stream;
            //voice.Rate = 1;//语速
            voice.Speak(comboBox5.Text);
            voice.WaitUntilDone(Timeout.Infinite);
            stream.Close();
            MessageBox.Show("转换音频文件成功");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                comboBox3.Text = GetResult(
                    GetHisogram(Resize(comboBox6.Text)),
                    GetHisogram(Resize(comboBox7.Text))
                    ).ToString();
            }catch (Exception ex){}

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
                    { "file", @"C:\Users\tom86\Desktop\1.jpg"}
                });

            //comboBox2.Text = PostData(url, "file:文件");

            //comboBox3.Text = Post1(url, "file=文件") + Post1(url, "file:文件");

        }

        #region cmd
        public void Cmd()
        {
            CMDHelper.RunCmd("", out string result);
            comboBox8.Text = result;
        }
        #endregion

        #region 语音/文本
        private SpeechRecognitionEngine SRE = new SpeechRecognitionEngine();
        /// <summary>
        //  语音转文本
        /// </summary>
        /// <param name="str"></param>
        private void VoiceToText(object str)
        {
            try
            {
                string filepath = str.ToString(); ;
                SRE.SetInputToWaveFile(filepath);         //<=======默认的语音输入设备，你可以设定为去识别一个WAV文件。
                GrammarBuilder GB = new GrammarBuilder();
                //需要判断的文本（相当于语音库）
                GB.Append(new Choices(new string[] { "时间", "电话", "短信", "定位", "天气", "帮助" }));
                Grammar G = new Grammar(GB);
                G.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(G_SpeechRecognized);
                SRE.LoadGrammar(G);
                SRE.RecognizeAsync(RecognizeMode.Multiple); //<=======异步调用识别引擎，允许多次识别（否则程序只响应你的一句话）
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
        }

        SpeechSynthesizer speech = new SpeechSynthesizer();
        /// <summary>
        /// 判断语音并转化为需要输出的文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void G_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string result = e.Result.Text;
            string RetSpeck = string.Empty;
            switch (result)
            {
                case "时间":
                    RetSpeck = "你输入了时间";
                    break;
                case "电话":
                    RetSpeck = "你输入了电话";
                    break;
                case "短信":
                    RetSpeck = "你输入了短信";
                    break;
                case "定位":
                    RetSpeck = "你输入了定位";
                    break;
                case "天气":
                    RetSpeck = "你输入了天气";
                    break;
                case "帮助":
                    RetSpeck = "你输入了帮助";
                    break;
            }
            speech.SpeakAsync(RetSpeck);

            //SpVoice voice = new SpVoice();
            //voice.Rate = -5; //语速,[-10,10]
            //voice.Volume = 100; //音量,[0,100]
            //voice.Voice = voice.GetVoices().Item(0); //语音库
            //voice.Speak(RetSpeck);
        }
        #endregion

        #region 图像相似度对比
        /// <summary>
        /// 将图像转化成相同大小，我们暂且转化成256 X 256
        /// </summary>
        /// <param name="imageFile">需要对比的图片地址</param>
        /// <param name="newImageFile">转化后的新图片地址</param>
        /// <returns></returns>
        public Bitmap Resize(string imageFile)
        {
            if (!File.Exists(imageFile))
            {
                MessageBox.Show("文件不存在！");
                throw new Exception("文件不存在！");
            }

            var dn = Path.GetDirectoryName(imageFile);//文件所在目录
            var fnwe = Path.GetFileNameWithoutExtension(imageFile);//文件名（不带后缀）
            var e = Path.GetExtension(imageFile);//后缀名
            var newImageFile = dn + "\\" + fnwe;
            var fullPath = newImageFile + e;

            Bitmap imgOutput = new Bitmap(Image.FromFile(imageFile), 256, 256);
            var i = 0;
            while (File.Exists(fullPath))
            {
                i++;
                fullPath = newImageFile + "(" + i + ")" + e;
            }
            imgOutput.Save(fullPath, ImageFormat.Jpeg);
            imgOutput.Dispose();

            return (Bitmap)Image.FromFile(fullPath);
        }

        //计算图像的直方图
        public int[] GetHisogram(Bitmap img)
        {
            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

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
            image1.Save(ms1, ImageFormat.Png);

            string img1 = Convert.ToBase64String(ms1.ToArray());

            Image image2 = Image.FromFile(filePath2);
            image2.Save(ms1, ImageFormat.Png);
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

        private void button3_Click(object sender, EventArgs e)
        {
            Cmd();
        }
        //语音识别
        private void button4_Click(object sender, EventArgs e)
        {
            //设置语音识别应用的账号信息(百度智能云管理中心) APP_ID/API_KEY/SECRET_KEY
            //demo里的账号
            //var APP_ID = "14433392";
            //var API_KEY = "C7WMYgLeWv3Wm2yogwv5gD08";
            //var SECRET_KEY = "xcvwiwikALBDBaIcGisNQ6aQImtj3qua";
            //请更改成自己的账号
            string APP_ID = "16982575";
            string API_KEY = "0kC4dDwWl3hqo2xz2c4113ZP";
            string SECRET_KEY = "8aneCyn9KVAWjGKAIfpcr3vMCFt19kIb";

            var client = new Asr(APP_ID, API_KEY, SECRET_KEY);

            var videoPath = @"C:\Users\tom86\Desktop\voice.wav";
            var type = Path.GetExtension(videoPath);
            var data = File.ReadAllBytes(videoPath);

            //dev_pid 可选参数
            //1536    普通话(支持简单的英文识别)  搜索模型 无标点 支持自定义词库 http://vop.baidu.com/server_api
            //1537    普通话(纯中文识别)  输入法模型 有标点 不支持自定义词库 http://vop.baidu.com/server_api
            //1737    英语 无标点 不支持自定义词库 http://vop.baidu.com/server_api
            //1637    粤语 有标点 不支持自定义词库 http://vop.baidu.com/server_api
            //1837    四川话 有标点 不支持自定义词库 http://vop.baidu.com/server_api
            //1936    普通话远场 远场模型    有标点 不支持自定义词库    http://vop.baidu.com/server_api
            //80001语音识别极速版(收费)
            //80001   普通话 极速版输入法模型    有标点 支持自定义词库 http://vop.baidu.com/pro_api
            var options = new Dictionary<string, object>
                 {
                    {"dev_pid", 1536}
                 };
            client.Timeout = 120000; // 若语音较长，建议设置更大的超时时间. ms
            var result = client.Recognize(data, type.TrimStart('.'), 16000, options);
            comboBox9.Text = Convert.ToString(result);
            result.TryGetValue("result", out JToken resultStr);
            if (Convert.ToString(resultStr) != "")
            {
                comboBox9.Text = Convert.ToString(resultStr);
            }
        }
    }
}
