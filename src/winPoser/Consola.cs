using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace winPoser
{
    class Consola
    {
        public static void runSyncCommand(string comando,string rcommand) {
            ProcessStartInfo procStartInfo = new ProcessStartInfo(comando, rcommand);

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            //Indica que el proceso no despliegue una pantalla negra (El proceso se ejecuta en background)
            procStartInfo.CreateNoWindow = true;
            //Esconder la ventana
            procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            procStartInfo.RedirectStandardError = true;
            //Inicializa el proceso

            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();



            string standard_output = "";
            Console.WriteLine("Ejecutando comando, espere por favor...");
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
                        Console.WriteLine(standard_output);
                    }

                }
            }
        }
        public static void runCommand(string app,string args)
        {
            //* Create your Process
            Process process = new Process();
            process.StartInfo.FileName = app;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            //* Set your output and error (asynchronous) handlers
            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            //* Start process and handlers
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }

        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine(outLine.Data);
        }
    }
}
