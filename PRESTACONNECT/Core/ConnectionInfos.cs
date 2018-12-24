using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;

namespace PRESTACONNECT.Core
{
    internal sealed class ConnectionInfos
    {
        public string SageServer { get; private set; }
        public string SageDatabase { get; private set; }
        public string SageSQLUser { get; private set; }
        public string SageSQLPass { get; private set; }
        public bool SageIntegratedSecurity { get; private set; }
        public string PrestaconnectServer { get; private set; }
        public string PrestaconnectDatabase { get; private set; }
        public string PrestaconnectSQLUser { get; private set; }
        public string PrestaconnectSQLPass { get; private set; }
        public bool PrestaconnectIntegratedSecurity { get; private set; }
        public string PrestashopDatabase { get; private set; }
        public string PrestashopServer { get; private set; }
        public string PrestashopSQLUser { get; private set; }

        public ConnectionInfos()
        {
            string[] PsConnection = Properties.Settings.Default.PRESTASHOPConnectionString.Split(';');
            foreach (String str in PsConnection)
            {
                if (str.ToLower().StartsWith("server="))
                    PrestashopServer = str.Split(Core.UpdateVersion.ArgSplitter)[1];
                else if (str.ToLower().StartsWith("database="))
                    PrestashopDatabase = str.Split(Core.UpdateVersion.ArgSplitter)[1];
                if (!string.IsNullOrEmpty(PrestashopServer) && !string.IsNullOrEmpty(PrestashopDatabase))
                    break;
            }

            string[] PcConnection = Properties.Settings.Default.PRESTACONNECTConnectionString.Split(';');
            foreach (string info in PcConnection)
            {
                if (info.StartsWith("Data Source="))
                {
                    PrestaconnectServer = info.Split(Core.UpdateVersion.ArgSplitter)[1];
                }
                else if (info.StartsWith("Initial Catalog="))
                {
                    PrestaconnectDatabase = info.Split(Core.UpdateVersion.ArgSplitter)[1];
                }
                else if (info.StartsWith("User ID="))
                {
                    PrestaconnectSQLUser = info.Split(Core.UpdateVersion.ArgSplitter)[1];
                }
                else if (info.StartsWith("Password="))
                {
                    PrestaconnectSQLPass = info.Split(Core.UpdateVersion.ArgSplitter)[1];
                }
                else if (info.StartsWith("Integrated Security="))
                {
                    bool integratedsecurity = false;
                    bool.TryParse(info.Split(Core.UpdateVersion.ArgSplitter)[1], out integratedsecurity);
                    PrestaconnectIntegratedSecurity = integratedsecurity;
                }
            }

            string[] SageConnection = Properties.Settings.Default.SAGEConnectionString.Split(';');
            foreach (string info in SageConnection)
            {
                if (info.StartsWith("Data Source="))
                {
                    SageServer = info.Split(Core.UpdateVersion.ArgSplitter)[1];
                }
                else if (info.StartsWith("Initial Catalog="))
                {
                    SageDatabase = info.Split(Core.UpdateVersion.ArgSplitter)[1];
                }
                else if (info.StartsWith("User ID="))
                {
                    SageSQLUser = info.Split(Core.UpdateVersion.ArgSplitter)[1];
                }
                else if (info.StartsWith("Password="))
                {
                    SageSQLPass = info.Split(Core.UpdateVersion.ArgSplitter)[1];
                }
                else if (info.StartsWith("Integrated Security="))
                {
                    bool integratedsecurity = false;
                    bool.TryParse(info.Split(Core.UpdateVersion.ArgSplitter)[1], out integratedsecurity);
                    SageIntegratedSecurity = integratedsecurity;
                }
            }
        }

		public MySqlConnection MySQLConnexionOpen()
		{
			MySqlConnection connection = null;
			string connectionString = Properties.Settings.Default.PRESTASHOPConnectionString;
			connection = new MySqlConnection(connectionString);
			connection.Open();

			return connection;
		}

		public DataTable MySQLConnexionRequest(string request)
		{
			MySqlConnection connection = null;
			DataTable Result = new DataTable();
			try
			{
				connection = MySQLConnexionOpen();
				MySqlCommand cmd = connection.CreateCommand();
				cmd.CommandText = request;
				MySqlDataReader reader = cmd.ExecuteReader();
				if (reader.HasRows)
				{
					DataTable schemaTable = reader.GetSchemaTable();
					foreach (DataRow row in schemaTable.Rows)
					{
						Result.Columns.Add(row[0].ToString());
					}

					int j = 0;
					while (reader.Read())
					{
						DataRow rowData = Result.NewRow();
						for (int i = 0; i < Result.Columns.Count; i++)
						{
							rowData[i] = reader.GetValue(i).ToString();
						}
						Result.Rows.Add(rowData);
						j++;
					}
				}
			}
			catch (Exception ex)
			{
				Core.Log.WriteLog("Connexion MySQL : " + ex.Message);
			}
			finally
			{
				MySQLConnexionClose(connection);
			}
			return Result;
		}

		public void MySQLConnexionClose(MySqlConnection connection)
		{
			if (connection != null && connection.State != ConnectionState.Closed)
				connection.Close();
		}

		public void SAGEODBCConnexionF_DOCENTETE(string NomColonne, string Valeur, ABSTRACTION_SAGE.F_DOCENTETE.Obj F_DOCENTETE)
		{
			try
			{
				ABSTRACTION_SAGE.ALTERNETIS.Connexion SageODBC = new ABSTRACTION_SAGE.ALTERNETIS.Connexion(Properties.Settings.Default.SAGEUSER,
					Properties.Settings.Default.SAGEPASSWORD, Properties.Settings.Default.SAGEDSN);
				SageODBC.Command.CommandTimeout = 3600;
				SageODBC.Request = "UPDATE F_DOCENTETE SET " + NomColonne + " = '" + Valeur + "' WHERE DO_Domaine = " + ((short)F_DOCENTETE.DO_Domaine).ToString()
					+ " AND DO_Type = " + ((short)F_DOCENTETE.DO_Type).ToString() + " AND DO_Piece  = '" + F_DOCENTETE.DO_Piece + "'";
				SageODBC.Exec_Request();
			}
			catch (Exception ex)
			{
				Core.Log.WriteLog("Connexion SAGE ODBC : " + ex.Message);
			}
		}
    }
}
