using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core
{
    public class Error
    {
        #region Method
        public static void SendMailError(String Msg)
        {
            try
            {
                DisplayErrorMessage(Msg);

                try { Core.Log.WriteLog(Msg, true); }
                catch (Exception) { };

                StringBuilder Body = new StringBuilder();
                Body.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
                Body.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                Body.Append("<head>");
                Body.Append("<meta http-equiv=\"content-type\" content=\"text/html\"; charset=UTF-8\" />");
                Body.Append("<title>Une erreur est survenue pour le module PRESTACONNECT version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " du client " + Properties.Settings.Default.CLIENT + "</title>");
                Body.Append("</head>");
                Body.Append("<body>");
                Body.Append("Une erreur est survenue pour le module PRESTACONNECT du client " + Properties.Settings.Default.CLIENT + "<br /><br />");
                Body.Append(Msg);
                Body.Append("<br /><br />");
                Body.Append("Informations de connexion :<br />");
                Body.Append("<ul>");
                Body.Append("<li>" + "Serveur d'exécution : " + Environment.MachineName + "</li>");
                Body.Append("<li>" + "Domaine réseau : " + Environment.UserDomainName + "</li>");
                Body.Append("<li>" + "Session utilisateur de : " + Environment.UserName + "</li>");
                Body.Append("<li>" + "Connexion base de données PrestaConnect : " + Properties.Settings.Default.PRESTACONNECTConnectionString + "</li>");
                Body.Append("<li>" + "Connexion base de données Sage : " + Properties.Settings.Default.SAGEConnectionString + "</li>");
                Body.Append("<li>" + "Connexion ODBC Sage : " + Properties.Settings.Default.SAGEDSN + " - " + Properties.Settings.Default.SAGEUSER.Replace("<", "&lt;").Replace(">", "&gt;") + "</li>");
                Body.Append("</ul>");
                Body.Append("</body>");
                Body.Append("</html>");
                SendMailGoogle("Erreur sur le module PRESTACONNECT version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " du client " + Properties.Settings.Default.CLIENT, Body.ToString(), "prestaconnect@gmail.com");
            }
            catch (Exception exmail) { Core.Log.WriteLog(exmail.ToString(), true); };
        }

        private static void DisplayErrorMessage(String Msg)
        {
            if (Environment.GetCommandLineArgs().Length <= 1)
            {
                ConnectionInfos ci = new ConnectionInfos();
                PRESTACONNECT.View.PrestaMessage prestamessage;
                if (Msg.Contains("Unknown database"))
                {
                    prestamessage = new View.PrestaMessage("La base de données Prestashop est introuvable, veuillez vérifier vos paramètres !",
                        "Base de données introuvable", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error, 480, Msg);
                    prestamessage.ShowDialog();
                }
                else if (Msg.Contains("System.Data.SqlClient.SqlException: Impossible d'ouvrir la base de données"))
                {
                    if (Msg.Contains(" " + ci.SageDatabase + " "))
                    {
                        prestamessage = new View.PrestaMessage("La base de données Sage nommée " + ci.SageDatabase
                            + " est introuvable sur le serveur SQL " + ci.SageServer
                            + ". Vous pouvez commencer par vérifier la présence de la base de données et vos droits d'accès à celle-ci.",
                            "Base de donnés Sage introuvable", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error, 480, Msg);
                        prestamessage.ShowDialog();
                    }
                    else if (Msg.Contains(ci.PrestaconnectDatabase))
                    {
                        prestamessage = new View.PrestaMessage("La base de données PrestaConnect nommée " + ci.PrestaconnectDatabase
                            + " est introuvable sur le serveur SQL " + ci.PrestaconnectServer
                            + ". Vous pouvez commencer par vérifier la présence de la base de données et vos droits d'accès à celle-ci.",
                            "Base de donnés introuvable", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error, 480, Msg);
                        prestamessage.ShowDialog();
                    }
                }
                else if (Msg.Contains("(0x80004005): Unable to connect to any of the specified MySQL hosts")
                    || (Msg.Contains("MySql.Data.MySqlClient.MySqlException") && Msg.Contains("Unable to connect to any of the specified MySQL hosts."))
                    || (Msg.Contains("System.Net.Sockets.SocketException") && Msg.Contains("Aucune connexion n’a pu être établie car l’ordinateur cible l’a expressément refusée"))
                    || (Msg.Contains("MySql.Data.MySqlClient.MySqlException") && Msg.Contains("Access denied for user")))
                {
                    prestamessage = new View.PrestaMessage("La connexion au serveur MySql " + ci.PrestashopServer + " ne peut être établie, veuillez vérifier votre connectivité côté client et côté serveur !",
                        "Erreur de connexion MySql", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error, 480, Msg);
                    prestamessage.ShowDialog();
                }
                else if (Msg.Contains("System.Data.SqlClient.SqlException (0x80131904)") || Msg.Contains("Une erreur liée au réseau ou spécifique à l'instance s'est produite lors de l'établissement d'une connexion à SQL Server. Le serveur est introuvable ou n'est pas accessible."))
                {
                    if (Msg.Contains("PRESTACONNECT.Model.Sage."))
                    {
                        prestamessage = new View.PrestaMessage("Le serveur SQL de la base Sage " + ci.SageServer + " est inaccessible, ou la version de la base de données est incorrecte !",
                            "Serveur SQL Sage", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error, 480, Msg);
                        prestamessage.ShowDialog();
                    }
                    else if (Msg.Contains("PRESTACONNECT.Model.Local."))
                    {
                        prestamessage = new View.PrestaMessage("Le serveur SQL de la base PrestaConnect " + ci.PrestaconnectServer + " est inaccessible, ou la version de la base de données est incorrecte  !",
                            "Serveur SQL PrestaConnect", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error, 480, Msg);
                        prestamessage.ShowDialog();
                    }
                }
            }
        }

        public static void SendMailGoogle(String Subject, String Body, String Destinataire)
        {
            try
            {
                System.Net.Mail.MailMessage ObjMessage = new System.Net.Mail.MailMessage();
                System.Net.Mail.MailAddress ObjAdrExp = new System.Net.Mail.MailAddress("prestaconnect@gmail.com");
                System.Net.Mail.MailAddress ObjAdrRec = new System.Net.Mail.MailAddress(Destinataire);
                System.Net.Mail.SmtpClient ObjSmtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);

                ObjMessage.From = ObjAdrExp;
                ObjMessage.To.Add(ObjAdrRec);
                ObjMessage.Subject = Subject;
                ObjMessage.Body = Body;
                ObjMessage.IsBodyHtml = true;
                ObjSmtpClient.EnableSsl = true;
                ObjSmtpClient.Credentials = new System.Net.NetworkCredential("prestaconnect@gmail.com", "lm29041968;86");

                ObjSmtpClient.Send(ObjMessage);
            }
            catch (Exception exmail) { Core.Log.WriteLog(exmail.ToString(), true); };
        }

        public static void SendMailWS(Exception ex)
        {
            try
            {
                System.Text.StringBuilder Body = new System.Text.StringBuilder();
                Body.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
                Body.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                Body.Append("<head>");
                Body.Append("<meta http-equiv=\"content-type\" content=\"text/html\"; charset=UTF-8\" />");
                Body.Append("<title>Erreur de connexion WS PRESTACONNECT version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " du client " + Properties.Settings.Default.CLIENT + "</title>");
                Body.Append("</head>");
                Body.Append("<body>");
                Body.Append("<br />");
                Body.Append("Erreur de connexion WS PRESTACONNECT du client " + Properties.Settings.Default.CLIENT + "<br /><br />");
                Body.Append(ex.ToString());
                Body.Append("<br /><br />");
                Body.Append("Informations de connexion :<br />");
                Body.Append("<ul>");
                Body.Append("<li>" + "Serveur d'exécution : " + Environment.MachineName + "</li>");
                Body.Append("<li>" + "Domaine réseau : " + Environment.UserDomainName + "</li>");
                Body.Append("<li>" + "Session utilisateur de : " + Environment.UserName + "</li>");
                Body.Append("<li>" + "Connexion base de données PrestaConnect : " + Properties.Settings.Default.PRESTACONNECTConnectionString + "</li>");
                Body.Append("<li>" + "Connexion base de données Sage : " + Properties.Settings.Default.SAGEConnectionString + "</li>");
                Body.Append("<li>" + "Connexion ODBC Sage : " + Properties.Settings.Default.SAGEDSN + " - " + Properties.Settings.Default.SAGEUSER.Replace("<", "&lt;").Replace(">", "&gt;") + "</li>");
                Body.Append("</ul>");
                Body.Append("</body>");
                Body.Append("</html>");
                Core.Error.SendMailGoogle("Erreur de connexion WS", Body.ToString(), "prestaconnect@alternetis.fr");
                Core.Error.SendMailGoogle("Erreur de connexion WS", Body.ToString(), "s-semence@alternetis.fr");
            }
            catch (Exception exmail) { Core.Log.WriteLog(exmail.ToString(), true); };
        }
        #endregion
    }
}
