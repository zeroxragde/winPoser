using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace winPoser
{
    static class getComnposerFiles
    {
        public static string  composer = "https://getcomposer.org/download/";
        public static List<string> get()
        {
            List<string> lista = new List<string>();

            var client = new System.Net.Http.HttpClient();
            var content = client.GetStringAsync(getComnposerFiles.composer).Result;
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(content);
            var archivos = document.DocumentNode.SelectNodes("//td//a");
            foreach (HtmlAgilityPack.HtmlNode node in archivos)
            {
                if (node.InnerHtml != "changelog")
                {
                    foreach (HtmlAgilityPack.HtmlAttribute atributo in node.Attributes)
                    {
                        if (atributo.Name == "href")
                        {
                            if (atributo.Value.IndexOf("/") > -1 && atributo.Value.IndexOf(".sha256sum") <= -1 && atributo.Value.IndexOf(".asc") <= -1)
                            {
                                lista.Add(atributo.Value.Replace("/download/",""));
                            }
                        }

                    }

                }

            }
            return lista;
        }
        public static bool downloadVersion(string path, string v)
        {
            List<string> files = getComnposerFiles.get();
            foreach (string file in files)
            {
                string version = file.Replace("/", "").Replace("composer.phar", "");
                if(version != v)
                {
                    continue;
                }
                string folder = path + "composer_" + version + ".phar";

                if (File.Exists(folder))
                {
                    continue;
                }
                else
                {
                    using (var client = new WebClient())
                    {

                        client.DownloadFile(getComnposerFiles.composer + file, folder);
                    }
                }
            }
            return true;
        }
        public static bool downloadAll(string path)
        {
            List<string> files=getComnposerFiles.get();
            foreach(string file in files)
            {
                string version = file.Replace("/", "").Replace("composer.phar", "");

                string folder = path + "composer_" + version + ".phar";
     
                if (File.Exists(folder))
                {
                    continue;
                }
                else
                {
                    using (var client = new WebClient())
                    {

                        client.DownloadFile(getComnposerFiles.composer + file, folder);
                    }
                }
            }
            return true;
        }
    }
}
