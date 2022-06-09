using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using winPoser.Configuraciones;

namespace winPoser
{

    static class Program
    {


        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Configuracion my_config = new Configuracion();

            string defaultComposer = "1.10.26";
            string phpv = "";
            if (File.Exists(Application.StartupPath+"\\configuracion.json"))
            {
                 my_config = JsonConvert.DeserializeObject<Configuracion>(File.ReadAllText(Application.StartupPath+"\\configuracion.json"));
                 phpv = my_config.PHP_INSTALLED[0].path;
            }



            if (args.Length > 0) {
               
                if (args[0] == "--help" || args[0] == "-h")
                {
                    Console.WriteLine("  ");
                    Console.WriteLine("Aplicacion para el manejo rapido de multiples versiones de composer");

                    Console.WriteLine("--help -h                                                             Muestra las instrucciones de ayuda");
                    Console.WriteLine("--allComposers                                                        Descarga todas las versiones disponibles de composer");
                    Console.WriteLine("--reloadPHP                                                           Registra todas las versiones de PHP en tu sistema");
                    Console.WriteLine("--composerD [version]                                                 Descarga una version especifica de composer");
                    Console.WriteLine("--pack [NOMBRE] [PHP VERSION] [AQUITECTURA] [COMPOSER VERSION]        Configura acceso rapido para ejecutar composer");
                    Console.WriteLine("--removepack [NOMBRE]                                                 Borra el Pack seleccionado");

                    Console.WriteLine("-php [-v Especificar version a ejecutar, -arc Especificar aqrquitectura]   Ejecutar version de php por default");

                    
                    Console.WriteLine("  ");

                    Console.WriteLine("-phpls Ver la lista de PHP encontrado en el sistema");
                    Console.WriteLine("-composerls Ver las versiones de composer descargadas");
                    Console.WriteLine("-packls Ver los pack configurados");

                    Console.WriteLine("  ");
                    Console.WriteLine("-p PHP VERSION");
                    Console.WriteLine("-a PHP ARQUITECTURA");
                    Console.WriteLine("-c COMPOSER VERSION");
                    Console.WriteLine("Ejemplo:");
                    Console.WriteLine("winPoser -p 5.6.9 -a x64 -c 1.10.26 install jquery");
                    Console.WriteLine("  ");
                    Console.WriteLine("-d SELECCIONA UN PACK YA CONFIGURADO");
                    Console.WriteLine("Ejemplo:");
                    Console.WriteLine("winPoser -d DESARROLLO install jquery");
                    Environment.Exit(0);
                }

                if(args[0]== "--reloadPHP")
                {

                    string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
                    string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
                    my_config.PHP_INSTALLED.Clear();
                    Dictionary<string,string> rutas = new Dictionary<string, string>() {
                        {"64",programFiles},
                        {"32",programFilesX86}
                    };
             
                    foreach (KeyValuePair<string,string> ruta in rutas)
                    {
                        DirectoryInfo di = new DirectoryInfo(ruta.Value + "/PHP");

                        if (di != null)
                        {
                            DirectoryInfo[] dirs = di.GetDirectories();
                            if (dirs.Length > 0)
                            {
                               
                                foreach (DirectoryInfo dir in dirs)
                                {
                                    PHP p = new PHP();
                                    FileInfo[] subFiles = dir.GetFiles("*.exe");
                                    foreach (FileInfo subFile in subFiles)
                                    {
                                        if ("php-cgi.exe" == subFile.Name)
                                        {
                                            p.nombre = subFile.Name;
                                            p.path = subFile.FullName;
                                            Dictionary<string, string> datos = p.getVersionAndArc(p.path);
                                            p.arquitectura = datos["ARQUITECTURA"].Trim();
                                            p.version = datos["VERSION"].Trim();
                                        }
                                        
                                    }
                                    my_config.PHP_INSTALLED.Add(p);
                                }
                            }
                        }
                    }
 

                    string config_string  =  JsonConvert.SerializeObject(my_config, Formatting.Indented);

                    // serialize JSON to a string and then write string to a file
                    File.WriteAllText(Application.StartupPath+"\\configuracion.json", config_string);
                    Console.WriteLine("Versiones de PHP actualizadas");
                 //   Console.WriteLine(config_string);
                    Environment.Exit(0);
                }

                if (!File.Exists(Application.StartupPath+"\\configuracion.json")) {
                    Console.WriteLine("Executa --reloadPHP para que el sistema reconosca todas las versiones de PHP");
                    Environment.Exit(0);
                }

                if (args[0] == "-php") {
                    int skip = 1;

                    string phpversion = "7.3.25";
                    string arcquit = "x64";

                    if (args.Length >= 2) {

                        if (args[1] == "-v")
                        {
                            phpversion = args[2];
                            skip++;
                            skip++;
                        }
                        try {
                            if (args[3] == "-arc")
                            {
                                arcquit = args[4];
                                skip++;
                                skip++;
                            }
                        } catch {

                        }


                    }
                   

                    PHP p = my_config.getPHP(phpversion, arcquit);
                    if (p == null)
                    {
                        Console.WriteLine("No se encontro la version especificada de PHP: " + phpversion);
                        Environment.Exit(0);
                    }

                    args = args.Skip(skip).ToArray();
                    phpv = p.path;

                    string _comando = '"' + phpv + '"' + " ";
                    string _argumentos = "";
                    foreach (string argumento in args)
                    {
                        _argumentos += " " + argumento;
                    }

                    try
                    {

                        ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c "+ _comando+" "+_argumentos);

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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Environment.Exit(0);
                    }

                    Environment.Exit(0);

                }

                if (args[0] == "--composerD")
                {
                    if (args[1]!="")
                    {
                       if(getComnposerFiles.downloadVersion(Application.StartupPath+"/composer/", args[1]))
                        {
                            Console.WriteLine("Descarga completa...");
                        }
                    }
                    Environment.Exit(0);
                }

                if (args[0] == "--allComposers")
                {
                    if (args[1] != "")
                    {
                        if (getComnposerFiles.downloadAll(Application.StartupPath + "/composer/"))
                        {
                            Console.WriteLine("Descarga completa...");
                        }
                    }
                    Environment.Exit(0);
                }

                if (args[0] == "--pack")
                {
                    Packs pack = new Packs();
                    pack.nombre = args[1];
                    PHP php = my_config.getPHP(args[2],args[3]);
                    pack.php = php;
                    pack.composer = args[4];
                    my_config.PACKS.Add(pack);
                    string config_string = JsonConvert.SerializeObject(my_config, Formatting.Indented);
                    // serialize JSON to a string and then write string to a file
                    try
                    {
                        File.WriteAllText(Application.StartupPath + "\\configuracion.json", config_string);
                        Console.WriteLine("Pack agregado y listo para usar");
                    }
                    catch {
                        Console.WriteLine("Pack no se logro guardar");
                    }

      
                    Environment.Exit(0);
                }
                   
                if(args[0]== "--removepack")
                {
                    if (my_config.removePack(args[1]))
                    {
                        string config_string = JsonConvert.SerializeObject(my_config, Formatting.Indented);
                        try
                        {
                            File.WriteAllText(Application.StartupPath + "\\configuracion.json", config_string);
                            Console.WriteLine("PACK Removido correctamnete");
                        }
                        catch
                        {
                            Console.WriteLine("Pack no se logro eliminar");
                        }
                        
                    }
                    Environment.Exit(0);
                }

                if (args[0] == "-phpls")
                {
                    foreach(PHP p in my_config.PHP_INSTALLED)
                    {
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine("Nombre: "+p.nombre);
                        Console.WriteLine("Arquitectura: " + p.arquitectura);
                        Console.WriteLine("Version: " + p.version);
                        Console.WriteLine("Ruta: " + p.path);
                        Console.WriteLine("--------------------------------------");
                    }
                    Environment.Exit(0);
                }

                if (args[0] == "-packls")
                {
                   
                    foreach (Packs p in my_config.PACKS)
                    {
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine("PACK: " + p.nombre);
                        Console.WriteLine("Arquitectura: " + p.php.arquitectura);
                        Console.WriteLine("Version PHP: " + p.php.version);
                        Console.WriteLine("Version Composer: " + p.composer);
                        Console.WriteLine("Ruta: " + p.php.path);
                        Console.WriteLine("--------------------------------------");
                    }
                    Environment.Exit(0);
                }

                if (args[0] == "-composerls")
                {
                
                    DirectoryInfo di = new DirectoryInfo(Application.StartupPath + "\\composer");
                    if (di != null)
                    {
                        FileInfo[] subFiles = di.GetFiles("*.phar");
                        foreach (FileInfo subFile in subFiles)
                        {
                            Console.WriteLine(subFile.Name.Replace("composer_", "").Replace(".phar", ""));
                        }
                    }
                }

                if (args[0] == "-d")
                {
                    Packs p = my_config.getPack(args[1]);
                    phpv = p.php.path;
                    defaultComposer = p.composer;
                    args = args.Skip(2).ToArray();
                }

                if (args[0] == "-p")
                {
                    string phpversion = args[1];
                    string arcquit = "x64";
                    if (args[2] == "-a")
                    {
                         arcquit = args[3];
                    }
                    else {
                        Console.WriteLine("Falta el parametro de arquitectura (-a)");
                        Environment.Exit(0);
                    }

                    PHP p = my_config.getPHP(phpversion, arcquit);
                    if (p == null)
                    {
                        Console.WriteLine("No se encontro la version especificada de PHP: "+phpversion);
                        Environment.Exit(0);
                    }
                    phpv = p.path;

                    if (args[4] == "-c")
                    {
                         defaultComposer = args[5];
                    }
                    else
                    {
                        Console.WriteLine("Falta el parametro de version de composer (-c)");
                        Environment.Exit(0);
                    }
                    args = args.Skip(6).ToArray();
                }

                string comando ='"' + phpv + '"'+" ";
                string argumentos = "";
                foreach(string argumento in args)
                {
                    argumentos += " " + argumento;
                }
                string phar = "composer_" + defaultComposer + ".phar";
                string pharpath='"'+ Application.StartupPath + "\\composer\\" + phar + '"';
                try {

                    string rcommand =  pharpath + " " + argumentos;
                   // Console.WriteLine(comando);
                   // Console.WriteLine(rcommand);
                    Consola.runCommand(comando, rcommand);

                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    Environment.Exit(0);
                }
            } else {
                Environment.Exit(0);
            }
        }

    }
}
