using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Net.Mail;
using System.Net;

namespace PRESTACONNECT.Core
{
    public static class Csv
    {
        public static StreamWriter LogStream = null;
        public static String LogPath = "";

        private static void CreateLog(String LogIdentifier)
        {
            LogIdentifier = Core.Log.CheckLogIdentifier(LogIdentifier);
            if (Directory.Exists(Core.Log.LogDirectory) == false)
            {
                Directory.CreateDirectory(Core.Log.LogDirectory);
            }
            else
            {
                try
                {
                    List<String> FileList = System.IO.Directory.GetFiles(Core.Log.LogDirectory, "*_Log.csv").ToList();

                    if (FileList != null && FileList.Count > 14)
                    {
                        //Archivage des anciens logs
                        if (Directory.Exists(Core.Log.ArchiveDirectory) == false)
                        {
                            Directory.CreateDirectory(Core.Log.ArchiveDirectory);
                        }

                        String DayDirectory = Core.Log.ArchiveDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
                        if (Directory.Exists(DayDirectory) == false)
                        {
                            Directory.CreateDirectory(DayDirectory);
                        }

                        foreach (String File in FileList)
                        {
                            System.IO.FileInfo Name = new System.IO.FileInfo(File);
                            Directory.Move(Core.Log.LogDirectory + "\\" + Name.Name, DayDirectory + "\\" + Name.Name);
                        }
                    }
                    if (Directory.Exists(Core.Log.ArchiveDirectory))
                    {
                        List<String> DirList = System.IO.Directory.GetDirectories(Core.Log.ArchiveDirectory).ToList();
                        if (DirList != null && DirList.Count > 15)
                        {
                            Core.Log.DeleteFolder(DirList.OrderBy(d => d).First());
                        }
                    }
                }
                catch (Exception ex)
                {
                    // erreur d'archivage des logs
                    Core.Error.SendMailError(ex.ToString());
                }
            }
            LogPath = Core.Log.LogDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ((!String.IsNullOrWhiteSpace(LogIdentifier)) ? "_" : "") + LogIdentifier + "_Log.csv";
            LogStream = new StreamWriter(LogPath, true, Encoding.UTF8);
            LogStream.AutoFlush = true;
        }

        public static void CloseLog()
        {
            if (LogStream != null)
            {
                LogStream.WriteLine("");
                LogStream.WriteLine("Fermeture du log courant");
                LogStream.Flush();
                LogStream.Close();
                LogStream = null;
            }
        }

        public static void WriteLog(String Value, Boolean NewLogGroup, Core.Log.LogIdentifier LogType)
        {
            if (LogStream == null)
            {
                CreateLog(LogType.ToString());
                LogStream.WriteLine("");
                LogStream.WriteLine("\"PrestaConnect version : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\"");
                LogStream.WriteLine("");
                LogStream.WriteLine("\"Informations de connexion :\"");
                LogStream.WriteLine("\"Serveur d'exécution : " + Environment.MachineName + "\"");
                LogStream.WriteLine("\"Domaine réseau : " + Environment.UserDomainName + "\"");
                LogStream.WriteLine("\"Session utilisateur de : " + Environment.UserName + "\"");
                LogStream.WriteLine("\"Connexion base de données PrestaConnect : " + Properties.Settings.Default.PRESTACONNECTConnectionString + "\"");
                LogStream.WriteLine("\"Connexion base de données Sage : " + Properties.Settings.Default.SAGEConnectionString + "\"");
                LogStream.WriteLine("\"Connexion ODBC Sage : " + Properties.Settings.Default.SAGEDSN + " - " + Properties.Settings.Default.SAGEUSER.Replace("<", "&lt;").Replace(">", "&gt;") + "\"");
                LogStream.WriteLine("");
            }
            else if (NewLogGroup)
            {
                LogStream.WriteLine("\"--------------------------------------------------------\"");
                LogStream.WriteLine("\"--------------------------------------------------------\"");
            }
            if (NewLogGroup)
            {
                LogStream.WriteLine("");
                LogStream.WriteLine("\"" + DateTime.Now.ToString("yyyy/MM/dd HH-mm") + "\"");
                LogStream.WriteLine("");
                LogStream.WriteLine("\"Utilisateur : " + Environment.UserName + "\"");
                LogStream.WriteLine("");
            }
            LogStream.WriteLine(Value);
        }

        public static void WriteSpecificLog(List<String> log_in, String LogIdentifier)
        {
            if (log_in.Count > 0)
            {
                if (LogStream != null)
                {
                    CloseLog();
                }
                CreateLog(LogIdentifier);
                LogStream.WriteLine("");
                LogStream.WriteLine("\"PrestaConnect version : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\"");
                LogStream.WriteLine("");
                LogStream.WriteLine("\"" + DateTime.Now.ToString("yyyy/MM/dd HH-mm") + "\"");
                LogStream.WriteLine("");
                LogStream.WriteLine("\"Informations de connexion :\"");
                LogStream.WriteLine("\"Serveur d'exécution : " + Environment.MachineName + "\"");
                LogStream.WriteLine("\"Domaine réseau : " + Environment.UserDomainName + "\"");
                LogStream.WriteLine("\"Session utilisateur de : " + Environment.UserName + "\"");
                LogStream.WriteLine("\"Connexion base de données PrestaConnect : " + Properties.Settings.Default.PRESTACONNECTConnectionString + "\"");
                LogStream.WriteLine("\"Connexion base de données Sage : " + Properties.Settings.Default.SAGEConnectionString + "\"");
                LogStream.WriteLine("\"Connexion ODBC Sage : " + Properties.Settings.Default.SAGEDSN + " - " + Properties.Settings.Default.SAGEUSER.Replace("<", "&lt;").Replace(">", "&gt;") + "\"");
                LogStream.WriteLine("");
                LogStream.WriteLine("--------------------------------------------------------");
                LogStream.WriteLine("");

                foreach (String line in log_in)
                {
                    if (line == Core.Log.LogLineSeparator)
                    {
                        LogStream.WriteLine("");
                        LogStream.WriteLine("\"--------------------------------------------------------\"");
                        LogStream.WriteLine("");
                    }
                    else
                        LogStream.WriteLine(line);
                }
            }
        }

        public static void SendLog(List<String> log, Boolean UI, Core.Log.LogIdentifier LogType)
        {
            if (log.Count > 0)
            {
                if (Core.Global.GetConfig().TransfertSendAdminResultReport || LogType != Core.Log.LogIdentifier.TransfertClient)
                {
                    bool send = false;
                    string msg_not_send = "Compte mail administrateur invalide";
                    if (!String.IsNullOrWhiteSpace(Core.Global.GetConfig().AdminMailAddress) && Core.Global.IsMailAddress(Core.Global.GetConfig().AdminMailAddress, Parametres.RegexMail.lvl08_lUdS))
                    {
                        send = Core.Log.SendLogMail(log, Core.Global.GetConfig().AdminMailAddress, out msg_not_send, LogType);
                    }
                    if (!send)
                    {
                        log.Add(Core.Log.LogLineSeparator);
                        log.Add("PC99- Échec d'envoi du log par mail : " + msg_not_send);

                        if (UI)
                        {
                            // TODO si UI affichage log dans une fenetre
                            MessageBox.Show("Échec d'envoi du log par mail !\nLe log va être écrit dans un fichier !", "", MessageBoxButton.OK, MessageBoxImage.Error);
                            Core.Log.WriteSpecificLog(log, LogType);
                            Core.Log.OpenDirectory(Core.Log.LogDirectory);
                        }
                        else
                        {
                            Core.Log.WriteSpecificLog(log, LogType);
                            Core.Log.OpenDirectory(Core.Log.LogDirectory);
                        }
                    }
                }
                else
                    Core.Log.WriteSpecificLog(log, LogType);
            }
        }
    }
}
