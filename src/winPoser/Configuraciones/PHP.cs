using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace winPoser.Configuraciones
{
    public class PHP
    {
        public string nombre = "php.exe";
        public string version = "";
        public string arquitectura = "";
        public string path = "";
        public Dictionary<string,string> getVersionAndArc(string path) {
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            string comando = '"' + path.Replace("php-cgi.exe","php.exe") + '"'+" -i";

            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + comando);

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            //Indica que el proceso no despliegue una pantalla negra (El proceso se ejecuta en background)
            procStartInfo.CreateNoWindow = true;
            //Esconder la ventana
            procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //Inicializa el proceso
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            string standard_output = "";
            bool versionf = false;
            bool arcf = false;
            string version_local="";
            string arq_local = "";
            while ((standard_output = proc.StandardOutput.ReadLine()) != null)
            {
                if (standard_output.Contains("pause"))
                {
                    break;
                }
                else
                {
                    if (standard_output != "")
                    {
                        if(standard_output.IndexOf("PHP Version") > -1 && !versionf)
                        {
                            version_local = standard_output.Replace("PHP Version =>", "");
                            versionf = true;
                        }
                        if (standard_output.IndexOf("Architecture") > -1 && !arcf)
                        {
                            arq_local = standard_output.Replace("Architecture =>", "");
                            arcf = true;
                        }
                        
                    }

                }
            }
            tmp.Add("ARQUITECTURA", arq_local);
            tmp.Add("VERSION", version_local);
            return tmp;

        }
    }
}
