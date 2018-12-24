using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Windows;
using System.Security.Cryptography;

namespace PRESTACONNECT.Core
{
    public static class Log
    {
        public const String LogLineSeparator = "[-]";
        public enum LogIdentifier
        {
            TransfertClient,
            ImportAutoCatalog, ImportAutoArticle, ImportAutoStatInfoLibreArticle,
            ImportAutoCompositionGammes, 
            ImportAutoImage, ImportAutoDocument, ImportAutoSageMedia,
            GestionStatutArticle,
            SynchroGroupCatTarif,
            SynchroGroupCodeRisque,
            SynchroPortfolioCustomerEmployee,
            SynchroEncoursClient,
            SynchroSuiviComptable,
            SynchroSuiviComptableDetaille,
            SynchroClient,
            ImportPrestashopCaracteristiqueArticle,
            TransfertPrestashopCaracteristique,

            ChronoSynchroStockPrice,
        };
        public enum LogStreamType
        {
            LogStream,
            LogChronoStream
        }
        private const String Key = "A9gJL3eMmYH4ruiW7t0b6bp";

        //<JG> 29/05/2012
        public static String LogDirectory = System.Windows.Forms.Application.StartupPath + "\\Logs";
        public static String ArchiveDirectory = System.Windows.Forms.Application.StartupPath + "\\Logs" + "\\Log_Archives";
        public static StreamWriter LogStream = null;
        public static StreamWriter LogChronoStream = null;
        public static String LogPath = string.Empty;

        private static void CreateLog(String LogIdentifier, LogStreamType SelectedLogStream)
        {
            LogIdentifier = CheckLogIdentifier(LogIdentifier);
            if (Directory.Exists(LogDirectory) == false)
            {
                Directory.CreateDirectory(LogDirectory);
            }
            else
            {
                #region Archivage
                try
                {
                    List<String> FileList = System.IO.Directory.GetFiles(LogDirectory, "*_Log.txt").ToList();

                    if (FileList != null && FileList.Count > 19)
                    {
                        //Archivage des anciens logs
                        if (Directory.Exists(ArchiveDirectory) == false)
                        {
                            Directory.CreateDirectory(ArchiveDirectory);
                        }

                        String DayDirectory = ArchiveDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
                        if (Directory.Exists(DayDirectory) == false)
                        {
                            Directory.CreateDirectory(DayDirectory);
                        }

                        foreach (String File in FileList)
                        {
                            try
                            {
                                System.IO.FileInfo Name = new System.IO.FileInfo(File);
                                Directory.Move(LogDirectory + "\\" + Name.Name, DayDirectory + "\\" + Name.Name);
                            }
                            catch {}
                        }
                    }
                    if (Directory.Exists(ArchiveDirectory))
                    {
                        List<String> DirList = System.IO.Directory.GetDirectories(ArchiveDirectory).ToList();
                        if (DirList != null && DirList.Count > 20)
                        {
                            DeleteFolder(DirList.OrderBy(d => d).First());
                        }
                    }
                }
                catch (Exception ex)
                {
                    // erreur d'archivage des logs
                    Core.Error.SendMailError(ex.ToString());
                }
                #endregion
            }
            LogPath = LogDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ((!String.IsNullOrWhiteSpace(LogIdentifier)) ? "_" : "") + LogIdentifier + "_Log.txt";
            switch (SelectedLogStream)
            {
                case LogStreamType.LogStream:
                default:
                    LogStream = new StreamWriter(LogPath, true, Encoding.UTF8);
                    LogStream.AutoFlush = true;
                    break;
                case LogStreamType.LogChronoStream:
                    int counter = 1;
                    while (System.IO.File.Exists(LogPath))
                    {
                        LogPath = LogDirectory + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ((!String.IsNullOrWhiteSpace(LogIdentifier)) ? "_" : "") + LogIdentifier + "_Log" + counter.ToString() + ".txt";
                        counter++;
                    }
                    LogChronoStream = new StreamWriter(LogPath, true, Encoding.UTF8);
                    LogChronoStream.AutoFlush = true;
                    break;
            }
        }

        public static void DeleteFolder(string directory)
        {
            try
            {
                List<String> DirList = System.IO.Directory.GetDirectories(directory).ToList();
                List<String> FileList = System.IO.Directory.GetFiles(directory).ToList();

                foreach (string dir in DirList)
                    DeleteFolder(dir);

                foreach (string file in FileList)
                    System.IO.File.Delete(file);

                System.IO.Directory.Delete(directory);
            }
            catch (Exception)
            {
                // Not implemented
            }
        }

        public static void CloseLog(LogStreamType SelectedLogStream)
        {
            switch (SelectedLogStream)
            {
                case LogStreamType.LogStream:
                default:

                    if (LogStream != null)
                    {
                        LogStream.WriteLine("");
                        LogStream.WriteLine("Fermeture du log courant");
                        LogStream.Flush();
                        LogStream.Close();
                        LogStream = null;
                    }
                    break;
                case LogStreamType.LogChronoStream:
                    if (LogChronoStream != null)
                    {
                        LogChronoStream.WriteLine("");
                        LogChronoStream.WriteLine("Fermeture du log courant");
                        LogChronoStream.Flush();
                        LogChronoStream.Close();
                        LogChronoStream = null;
                    }
                    break;
            }
        }
        public static void WriteLog(String Value, Boolean NewLogGroup = false)
        {
            if (LogStream == null)
            {
                CreateLog("", LogStreamType.LogStream); lock (Core.Log.LogStream)
                {
                    LogStream.WriteLine("");
                    LogStream.WriteLine("PrestaConnect version : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    LogStream.WriteLine("");
                    LogStream.WriteLine("Informations de connexion :");
                    LogStream.WriteLine("Serveur d'exécution : " + Environment.MachineName);
                    LogStream.WriteLine("Domaine réseau : " + Environment.UserDomainName);
                    LogStream.WriteLine("Session utilisateur de : " + Environment.UserName);
                    LogStream.WriteLine("Connexion base de données PrestaConnect : " + Properties.Settings.Default.PRESTACONNECTConnectionString);
                    LogStream.WriteLine("Connexion base de données Sage : " + Properties.Settings.Default.SAGEConnectionString);
                    LogStream.WriteLine("Connexion ODBC Sage : " + Properties.Settings.Default.SAGEDSN + " - " + Properties.Settings.Default.SAGEUSER.Replace("<", "&lt;").Replace(">", "&gt;"));
                    LogStream.WriteLine("");
                }
            }
            else if (NewLogGroup)
            {
                lock (Core.Log.LogStream)
                {
                    LogStream.WriteLine("--------------------------------------------------------");
                    LogStream.WriteLine("--------------------------------------------------------");
                }
            }
            lock (Core.Log.LogStream)
            {
                if (NewLogGroup)
                {
                    LogStream.WriteLine("");
                    LogStream.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH-mm"));
                    LogStream.WriteLine("");
                    LogStream.WriteLine("Utilisateur : " + Environment.UserName);
                    LogStream.WriteLine("");
                }
                LogStream.WriteLine(Value);
            }
        }

        public static void OpenDirectory(string dir)
        {
            //not open if in task
            if (Core.Global.UILaunch)
            {
                if (Directory.Exists(dir) == false)
                {
                    MessageBox.Show("Le dossier n'existe pas !", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    System.Diagnostics.Process.Start("explorer.exe", dir);
                }
            }
        }

        public static void WriteSpecificLog(List<String> log_in, LogIdentifier LogIdentifier)
        {
            try
            {
                if (log_in.Count > 0)
                {
                    if (LogStream != null)
                    {
                        CloseLog(LogStreamType.LogStream);
                    }
                    CreateLog(LogIdentifier.ToString(), LogStreamType.LogStream);
                    LogStream.WriteLine("");
                    LogStream.WriteLine("PrestaConnect version : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                    LogStream.WriteLine("");
                    LogStream.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH-mm"));
                    LogStream.WriteLine("");
                    LogStream.WriteLine("Informations de connexion :");
                    LogStream.WriteLine("Serveur d'exécution : " + Environment.MachineName);
                    LogStream.WriteLine("Domaine réseau : " + Environment.UserDomainName);
                    LogStream.WriteLine("Session utilisateur de : " + Environment.UserName);
                    LogStream.WriteLine("Connexion base de données PrestaConnect : " + Properties.Settings.Default.PRESTACONNECTConnectionString);
                    LogStream.WriteLine("Connexion base de données Sage : " + Properties.Settings.Default.SAGEConnectionString);
                    LogStream.WriteLine("Connexion ODBC Sage : " + Properties.Settings.Default.SAGEDSN + " - " + Properties.Settings.Default.SAGEUSER);
                    LogStream.WriteLine("");
                    LogStream.WriteLine("--------------------------------------------------------");
                    LogStream.WriteLine("");

                    foreach (String line in log_in)
                    {
                        if (line == LogLineSeparator)
                        {
                            LogStream.WriteLine("");
                            LogStream.WriteLine("--------------------------------------------------------");
                            LogStream.WriteLine("");
                        }
                        else
                            LogStream.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(ex.ToString(), true);
                Core.Log.WriteLog(LogIdentifier.ToString(), false);
                foreach (String line in log_in)
                    Core.Log.WriteLog(line, true);
            }
            CloseLog(LogStreamType.LogStream);
        }
        public static void WriteChronoLog(List<String> log_in, LogIdentifier LogIdentifier)
        {
            try
            {
                if (log_in.Count > 0)
                {
                    if (LogChronoStream == null)
                    {
                        CreateLog(LogIdentifier.ToString(), LogStreamType.LogChronoStream);
                        LogChronoStream.WriteLine("");
                        LogChronoStream.WriteLine("PrestaConnect version : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                        LogChronoStream.WriteLine("");
                        LogChronoStream.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH-mm"));
                        LogChronoStream.WriteLine("");
                        LogChronoStream.WriteLine("Informations de connexion :");
                        LogChronoStream.WriteLine("Serveur d'exécution : " + Environment.MachineName);
                        LogChronoStream.WriteLine("Domaine réseau : " + Environment.UserDomainName);
                        LogChronoStream.WriteLine("Session utilisateur de : " + Environment.UserName);
                        LogChronoStream.WriteLine("Connexion base de données PrestaConnect : " + Properties.Settings.Default.PRESTACONNECTConnectionString);
                        LogChronoStream.WriteLine("Connexion base de données Sage : " + Properties.Settings.Default.SAGEConnectionString);
                        LogChronoStream.WriteLine("Connexion ODBC Sage : " + Properties.Settings.Default.SAGEDSN + " - " + Properties.Settings.Default.SAGEUSER);
                        LogChronoStream.WriteLine("");
                        LogChronoStream.WriteLine("--------------------------------------------------------");
                        LogChronoStream.WriteLine("");
                    }

                    foreach (String line in log_in)
                    {
                        if (line == LogLineSeparator)
                        {
                            LogChronoStream.WriteLine("");
                            LogChronoStream.WriteLine("--------------------------------------------------------");
                            LogChronoStream.WriteLine("");
                        }
                        else
                            LogChronoStream.WriteLine(line);
                    }
                }
                //CloseLog(LogStreamType.LogChronoStream);
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(ex.ToString(), true);
            }
        }

        public static Boolean SendLogMail(List<String> log_in, String Destinataire, out String MessageNotSend, LogIdentifier LogType)
        {
            Boolean send = false;
            MessageNotSend = "Compte mail expéditeur invalide";
            try
            {
                if (Core.Global.GetConfig().ConfigMailActive)
                {
                    String User = Core.Global.GetConfig().ConfigMailUser;
                    String Password = Core.Global.GetConfig().ConfigMailPassword;
                    String SMTP = Core.Global.GetConfig().ConfigMailSMTP;
                    Int32 Port = Core.Global.GetConfig().ConfigMailPort;
                    Boolean isSSL = Core.Global.GetConfig().ConfigMailSSL;

                    if (!string.IsNullOrWhiteSpace(User)
                        //&& !string.IsNullOrWhiteSpace(Password)
                        && !string.IsNullOrWhiteSpace(SMTP))
                    {
                        MailMessage ObjMessage = new MailMessage();
                        MailAddress ObjAdrExp = new MailAddress(User);
                        MailAddress ObjAdrRec = new MailAddress(Destinataire);
                        SmtpClient ObjSmtpClient = new SmtpClient(SMTP, Port);

                        ObjMessage.From = ObjAdrExp;
                        ObjMessage.To.Add(ObjAdrRec);

                        String subject = "Log PrestaConnect", title = string.Empty;

                        switch (LogType)
                        {
                            case LogIdentifier.TransfertClient:
                                subject = "Log de transfert des comptes clients Sage vers Prestashop";
                                break;
                            case LogIdentifier.ImportAutoCatalog:
                                subject = "Import automatique des catalogues Sage";
                                break;
                            case LogIdentifier.ImportAutoArticle:
                                subject = "Import automatique des articles Sage";
                                break;
                            case LogIdentifier.ImportAutoImage:
                                subject = "Import automatique d'images";
                                break;
                            case LogIdentifier.ImportAutoDocument:
                                subject = "Import automatique de documents";
                                break;
                            case LogIdentifier.ImportAutoSageMedia:
                                subject = "Import automatique de médias Sage";
                                break;
                            case LogIdentifier.SynchroGroupCatTarif:
                                subject = "Synchronisation groupe/catégorie tarifaire";
                                break;
                            case LogIdentifier.GestionStatutArticle:
                                subject = "Gestion des statuts articles";
                                break;
                            case LogIdentifier.SynchroClient:
                                subject = "Synchronisation client";
                                break;
                            case LogIdentifier.ChronoSynchroStockPrice:
                                subject = "Temps de traitement synchronisation Stock et Prix";
                                break;
                            case LogIdentifier.ImportAutoStatInfoLibreArticle:
                                subject = "Import des valeurs de caractéristiques depuis Sage";
                                break;
                            case LogIdentifier.SynchroEncoursClient:
                                subject = "Synchronisation des encours client";
                                break;
                            case LogIdentifier.ImportPrestashopCaracteristiqueArticle:
                                subject = "Import des valeurs de caractéristiques depuis PrestaShop";
                                break;
                            case LogIdentifier.TransfertPrestashopCaracteristique:
                                subject = "Transfert des caractéristiques vers Prestashop";
                                break;
                        }

                        title = subject;
                        ObjMessage.Subject = subject;

                        StringBuilder Body = new StringBuilder();
                        Body.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
                        Body.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                        Body.Append("<head>");
                        Body.Append("<meta http-equiv=\"content-type\" content=\"text/html\"; charset=UTF-8\" />");
                        title += ". PRESTACONNECT version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                        Body.Append("<title>" + title + "</title>");
                        Body.Append("</head>");
                        Body.Append("<body>");
                        Body.Append(title);
                        Body.Append("<br />");
                        Body.Append("Informations de connexion :<br />");
                        Body.Append("<ul>");
                        Body.Append("<li>" + "Serveur d'exécution : " + Environment.MachineName + "</li>");
                        Body.Append("<li>" + "Domaine réseau : " + Environment.UserDomainName + "</li>");
                        Body.Append("<li>" + "Session utilisateur de : " + Environment.UserName + "</li>");
                        Body.Append("<li>" + "Connexion base de données PrestaConnect : " + Properties.Settings.Default.PRESTACONNECTConnectionString + "</li>");
                        Body.Append("<li>" + "Connexion base de données Sage : " + Properties.Settings.Default.SAGEConnectionString + "</li>");
                        Body.Append("<li>" + "Connexion ODBC Sage : " + Properties.Settings.Default.SAGEDSN + " - " + Properties.Settings.Default.SAGEUSER.Replace("<", "&lt;").Replace(">", "&gt;") + "</li>");
                        Body.Append("</ul>");
                        Body.Append("<br />");
                        foreach (String str in log_in)
                        {
                            Body.Append("<br />" + str);
                        }
                        Body.Append("</body>");
                        Body.Append("</html>");

                        ObjMessage.Body = Body.ToString();
                        ObjMessage.IsBodyHtml = true;
                        ObjSmtpClient.EnableSsl = isSSL;
                        ObjSmtpClient.Credentials = new NetworkCredential(User, Password);
                        ObjSmtpClient.Send(ObjMessage);
                        send = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageNotSend = ex.Message;
                Core.Error.SendMailError(ex.ToString());
            }
            return send;
        }

        public static String CheckLogIdentifier(String LogIdentifier)
        {
            if (string.IsNullOrEmpty(LogIdentifier))
                return string.Empty;
            else
            {
                string r = LogIdentifier;
                foreach (Char c in LogIdentifier)
                    if (!Char.IsLetter(c))
                        r.Replace(c, '_');

                return r;
            }
        }

        public static void SendLog(List<String> log, LogIdentifier LogType)
        {
            if (log.Count > 0)
            {
                if (Core.Global.GetConfig().TransfertSendAdminResultReport || LogType != LogIdentifier.TransfertClient)
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

                        if (Core.Global.UILaunch)
                            MessageBox.Show("Échec d'envoi du log par mail !\nLe log va être écrit dans un fichier !", "", MessageBoxButton.OK, MessageBoxImage.Error);

                        Core.Log.WriteSpecificLog(log, LogType);
                        Core.Log.OpenDirectory(LogDirectory);
                    }
                    else
                        Core.Log.WriteSpecificLog(log, LogType);
                }
                else
                {
                    Core.Log.WriteSpecificLog(log, LogType);
                    Core.Log.OpenDirectory(LogDirectory);
                }
            }
        }

        public static String EncryptString(String ValueToSecure, Boolean useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(ValueToSecure);

            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(Key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(Key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
    }
}
