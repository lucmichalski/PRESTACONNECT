using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace PRESTACONNECT.Core
{
    // Changement de connexion Crystal Reports
    // https://apps.support.sap.com/sap/support/knowledge/public/en/1214777

    internal static class PrintCrystal
    {
        static bool ApplyLogon(ReportDocument cr, ConnectionInfo ci)
        {
            TableLogOnInfo li;

            // for each table apply connection info
            foreach (Table tbl in cr.Database.Tables)
            {
                li = tbl.LogOnInfo;
                li.ConnectionInfo = ci;
                tbl.ApplyLogOnInfo(li);

                // check if logon was successful
                // if TestConnectivity returns false, check
                // logon credentials
                if (tbl.TestConnectivity())
                {
                    // drop fully qualified table location
                    if (tbl.Location.IndexOf(".") > 0)
                    {
                        tbl.Location = tbl.Location.Substring(tbl.Location.LastIndexOf(".") + 1);
                    }
                    else tbl.Location = tbl.Location;
                }
                else
                    return (false);
            }
            return (true);
        }

        // The Logon method iterates through all tables
        static internal bool Logon(ReportDocument cr, string server, bool integratedsecurity, string db, string id, string pass)
        {
            ConnectionInfo ci = new ConnectionInfo();
            SubreportObject subObj;

            ci.ServerName = server;
            ci.DatabaseName = db;
            ci.IntegratedSecurity = integratedsecurity;

            if (integratedsecurity)
            {
                ci.UserID = string.Empty;
                ci.Password = string.Empty;
            }
            else
            {
                ci.UserID = id;
                ci.Password = pass;
            }

            if (!ApplyLogon(cr, ci))
                return (false);

            if(Core.Global.GetConfig().CrystalForceConnectionInfoOnSubReports)
            {
                foreach (ReportObject obj in cr.ReportDefinition.ReportObjects)
                {
                    if (obj.Kind == ReportObjectKind.SubreportObject)
                    {
                        subObj = (SubreportObject)obj;
                        if (!ApplyLogon(cr.OpenSubreport(subObj.SubreportName), ci))
                            return (false);
                    }
                }
            }
            return (true);

        }

        // <JG> 03/08/2017 externalisation méthode export PDF
        static internal void ExportPDF(string report_path, Model.Sage.Piece Piece, uint TypeDoc, String CT_Num, string temp_file)
        {
            #region PDF
            CrystalDecisions.CrystalReports.Engine.ReportDocument Etat = new CrystalDecisions.CrystalReports.Engine.ReportDocument();

            Etat.Load(report_path);

            if (Etat.ParameterFields.Count == 0)
                Etat.RecordSelectionFormula = "{Commande.DO_Piece} = '" + Piece.DO_Piece + "'";

            // fonctions avec application sur les sous-états
            Core.PrintCrystal.Logon(Etat, Core.Global.GetConnectionInfos().SageServer, Core.Global.GetConnectionInfos().SageIntegratedSecurity, Core.Global.GetConnectionInfos().SageDatabase, Core.Global.GetConnectionInfos().SageSQLUser, Core.Global.GetConnectionInfos().SageSQLPass);

            Etat.Refresh();

            if (Etat.ParameterFields.Count > 0)
                Etat.SetParameterValue("piece", Piece.DO_Piece);

            Etat.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, temp_file);
            Etat.Close();

            if (Core.Global.GetConfig().ModuleAECInvoiceHistoryArchivePDFActive
                && System.IO.Directory.Exists(Core.Global.GetConfig().ModuleAECInvoiceHistoryArchivePDFFolder))
            {
                string path = null;
                if (!string.IsNullOrWhiteSpace(CT_Num))
                {
                    path = System.IO.Path.Combine(Core.Global.GetConfig().ModuleAECInvoiceHistoryArchivePDFFolder, CT_Num);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
                    switch (TypeDoc)
                    {
                        case 1:
                            path = System.IO.Path.Combine(path, "Order");
                            break;
                        case 7:
                            path = System.IO.Path.Combine(path, "Invoice");
                            break;
                        default:
                            break;
                    }
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    path = System.IO.Path.Combine(path, Piece.DO_Piece + ".pdf");
                }
                if (path != null)
                {
                    System.IO.File.Copy(temp_file, path, true);
                }
            }

            #endregion
        }
    }
}
