using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace GKCaptcha
{
    public class CaptchasLibrary
    {
        public static string Path = @"G:\GKData\samples\";

        public static List<Captcha> GetRecognizedCaptchas(int limit = -1)
        {
            var files = Directory.EnumerateFiles(Path);
            var res = new List<Captcha>();
            foreach (var filePath in files)
            {
                if (Regex.IsMatch(filePath, @"\.res$"))
                {
                    var name = filePath.Replace(".res", "").Replace(Path, "");
                    res.Add(Captcha.FromLibrary(name));
                }
                if (limit != -1 && res.Count >= limit)
                    break;
            }
            return res;
        }
    }

    public class Captcha
    {
        public string Source;
        public string Name;
        public string Answer;
        public Image Image;

        public string ImagePath
        {
            get { return CaptchasLibrary.Path + Name + ".png"; }
        }

        public string AnswerPath
        {
            get { return CaptchasLibrary.Path + Name + ".res"; }
        }

        public static Captcha FromLibrary(string name)
        {
            var ret = new Captcha
            {
                Source = "",
                Name = name,
            };
            ret.Image = Image.FromFile(ret.ImagePath);
            ret.Answer = File.ReadAllText(ret.AnswerPath);
            return ret;
        }

        public void SaveToLibrary()
        {
            Image?.Save(ImagePath, ImageFormat.Png);
            if (!string.IsNullOrEmpty(Answer))
                File.WriteAllText(AnswerPath, Answer);
        }

        public void Download()
        {
            using (var client = new WebClient())
            {
                var stream = client.OpenRead(Source);
                var bitmap = new Bitmap(stream);
                stream.Flush();
                stream.Close();
                Image = bitmap;
            }
        }
    }

    class GKRegistration
    {
        private static Captcha _getCaptchaSource()
        {
            var res = new Captcha();
            using (var client = new WebClient())
            {
                // Or you can get the file content without saving it:
                var htmlCode = client.DownloadString("http://govnokod.ru/user/register");

                var re = new Regex("http://govnokod.ru/captcha/image\\?rand=(\\w+)", RegexOptions.IgnoreCase);

                var m = re.Match(htmlCode);
                if (m.Success)
                {
                    res.Source = m.Captures[0].Value;
                    res.Name = m.Groups[1].Value;
                }
            }
            return res;
        }

        public static Captcha GetNextCaptcha()
        {
            var source = _getCaptchaSource();
            if (source == null)
                return null;

            source.Download();
            return source;
        }
    }
}
