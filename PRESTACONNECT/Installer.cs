using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;


namespace PRESTACONNECT
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        private String Prestashop = "";

        //string conStr = "packet size=4096;integrated security=SSPI;" +
        //    "data source=\"(local)\";persist security info=False;" +
        //    "initial catalog=master";

        public Installer()
        {

        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            InitializeComponent();

            Assembly ASM = Assembly.GetExecutingAssembly();
            String PathConfig = ASM.Location + ".config";

            #region création base SQL
            //conStr = GetLogin(
            //    this.Context.Parameters["LOCALSRV"].Replace("\\\\", "\\"),
            //    "master");
            //SqlConnection sqlCon = new SqlConnection(conStr);
            //sqlCon.Open();
            //ExecuteSql(sqlCon);
            //if (sqlCon.State != ConnectionState.Closed) sqlCon.Close();
            #endregion

            System.IO.FileInfo FileInfo = new System.IO.FileInfo(PathConfig);
            if (FileInfo.Exists == false)
            {
                throw new InstallException("Impossible de trouver le fichier de configuration " + PathConfig);
            }
            XmlDocument XmlDocument = new XmlDocument();
            XmlDocument.Load(FileInfo.FullName);
            Prestashop = "Server=" + this.Context.Parameters["PRESTASHOPSRV"].Replace("\\\\", "\\") + ";Database=" + this.Context.Parameters["PRESTASHOPBDD"] + ";Uid=" + this.Context.Parameters["PRESTASHOPUSER"] + ";Pwd=" + this.Context.Parameters["PRESTASHOPPWD"] + ";ConvertZeroDateTime=True;";

            foreach (XmlNode Node in XmlDocument["configuration"]["connectionStrings"])
            {
                if (Node.Attributes["name"].Value == "PRESTACONNECT.Properties.Settings.SAGEConnectionString")
                {
                    Node.Attributes["connectionString"].Value = "Data Source=" + this.Context.Parameters["SAGESRV"].Replace("\\\\", "\\") + ";Initial Catalog=" + this.Context.Parameters["SAGEBDD"] + ";Integrated Security=True";
                }
                else if (Node.Attributes["name"].Value == "PRESTACONNECT.Properties.Settings.PRESTACONNECTConnectionString")
                {
                    Node.Attributes["connectionString"].Value = "Data Source=" + this.Context.Parameters["LOCALSRV"].Replace("\\\\", "\\") + ";Initial Catalog=PRESTACONNECT;Integrated Security=True";
                }
            }

            foreach (XmlNode Node in XmlDocument["configuration"]["applicationSettings"]["PRESTACONNECT.Properties.Settings"])
            {
                if (Node.Name == "setting")
                {
                    switch (Node.Attributes["name"].Value)
                    {
                        case "SAGEDSN":
                            Node["value"].InnerText = this.Context.Parameters["SAGEDSN"];
                            break;
                        case "SAGEUSER":
                            Node["value"].InnerText = this.Context.Parameters["SAGEUSER"];
                            break;
                        case "SAGEPASSWORD":
                            Node["value"].InnerText = this.Context.Parameters["SAGEPWD"];
                            break;
                        case "PRESTASHOPConnectionString":
                            Node["value"].InnerText = Prestashop;
                            break;
                        case "CLIENT":
                            Node["value"].InnerText = this.Context.Parameters["CLIENTNAME"].Replace(" ", "_");
                            break;
                        case "LICENCEKEY" :
                            Node["value"].InnerText = this.Context.Parameters["CLIENTLICENCE"];
                            break;
                    }
                }
            }
            XmlDocument.Save(FileInfo.FullName);
        }

        private static string GetScript()
        {
            Assembly ASM = Assembly.GetExecutingAssembly();
            String PathScript = ASM.Location.Replace(".exe", ".sql");
            StreamReader reader = new StreamReader(PathScript);
            return reader.ReadToEnd();
        }

        private static string GetLogin(string databaseServer, string database)
        {
            return "server=" + databaseServer + ";database=" + database + ";Integrated Security=True";
        }

        private static void ExecuteSql(SqlConnection sqlCon)
        {
            string[] SqlLine;
            Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            string txtSQL = GetScript();

            txtSQL = txtSQL.Replace("D:\\Program Files\\Microsoft SQL Server\\MSSQL10.MSSQLSERVER\\MSSQL\\DATA\\PRESTACONNECT.mdf", Assembly.GetExecutingAssembly().Location.Replace("PRESTACONNECT.exe", "").Replace("Prestaconnect.exe", "") + "PRESTACONNECT.mdf");
            txtSQL = txtSQL.Replace("D:\\Program Files\\Microsoft SQL Server\\MSSQL10.MSSQLSERVER\\MSSQL\\DATA\\PRESTACONNECT_log.ldf", Assembly.GetExecutingAssembly().Location.Replace("PRESTACONNECT.exe", "").Replace("Prestaconnect.exe", "") + "PRESTACONNECT_log.ldf");
            txtSQL = txtSQL.Replace("C:\\Program Files\\Microsoft SQL Server\\MSSQL10.MSSQLSERVER\\MSSQL\\DATA\\PRESTACONNECT.mdf", Assembly.GetExecutingAssembly().Location.Replace("PRESTACONNECT.exe", "").Replace("Prestaconnect.exe", "") + "PRESTACONNECT.mdf");
            txtSQL = txtSQL.Replace("C:\\Program Files\\Microsoft SQL Server\\MSSQL10.MSSQLSERVER\\MSSQL\\DATA\\PRESTACONNECT_log.ldf", Assembly.GetExecutingAssembly().Location.Replace("PRESTACONNECT.exe", "").Replace("Prestaconnect.exe", "") + "PRESTACONNECT_log.ldf");
            
            SqlLine = regex.Split(txtSQL);

            SqlCommand cmd = sqlCon.CreateCommand();
            cmd.Connection = sqlCon;

            foreach (string line in SqlLine)
            {
                if (line.Length > 0)
                {
                    cmd.CommandText = line;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
