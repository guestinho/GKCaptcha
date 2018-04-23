using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GKCaptcha
{
    // https://gist.github.com/joeandaverde/3994603
    // async server
    class RecognizerServer
    {
        public RecognizerServer()
        {
            new Thread(() =>
            {
                var listener = new HttpListener();

                listener.Prefixes.Add("http://localhost:8081/");
                listener.Prefixes.Add("http://127.0.0.1:8081/");
                listener.Start();

                while (true)
                {
                    try
                    {
                        var context = listener.GetContext();
                        ThreadPool.QueueUserWorkItem(o => _handleRequest(context));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Error");
                    }
                }
            }).Start();
        }

        private void _handleRequest(object state)
        {
            var context = (HttpListenerContext)state;

            context.Response.StatusCode = 200;
            context.Response.SendChunked = true;

            var bytes = Encoding.UTF8.GetBytes(_process(context));
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.OutputStream.Close();
        }

        private string _process(HttpListenerContext context)
        {
            var rand = context.Request.QueryString["rand"];
            var url = context.Request.QueryString["url"];
            var errorStr = "{\"status\": \"error\"}";
            if (url == null)
            {
                return errorStr;
            }
            var captcha = new Captcha
            {
                Name = rand,
                Source = url,
            };
            try
            {
                captcha.Download();
            }
            catch (Exception)
            {
                return errorStr;
            }

            var tries = 5;
            var results = new List<CaptchaRecognizer.RecognizeResult>();
            for (var tryNo = 0; tryNo < tries; tryNo++)
            {
                var rec = new CaptchaRecognizer(captcha.Image);
                try
                {
                    var res = rec.Recognize();
                    results.Add(res);
                    if (res.Probability > 0.9)
                        break;
                }
                catch (CaptchaRecognizer.CantSplitDigitsException)
                {
                    results.Add(new CaptchaRecognizer.RecognizeResult
                    {
                        Probabilities = new double[] {0,0,0,0},
                        Result = "1111"// just random string
                    });
                }
            }
            var result = results.OrderByDescending(x => x.Probability).First();
            return "{\"status\": \"OK\", \"result\": \"" + result.Result + "\", \"probability\": \"" + result.Probability + "\"}";
        }
    }
}
