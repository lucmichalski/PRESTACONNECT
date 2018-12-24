using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PRESTACONNECT.View
{
    /// <summary>
    /// Logique d'interaction pour CrystalViewer.xaml
    /// </summary>
    public partial class CrystalViewer : Window
    {
        CrystalDecisions.CrystalReports.Engine.ReportDocument Etat;

        public CrystalViewer()
        {
            InitializeComponent();

            if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                this.WindowState = Core.Temp.Current;

            LoadDatas();
            LoadReport();
        }

        public void LoadDatas()
        {
            Model.Sage.F_DOCENTETERepository F_DOCENTETERepository = new Model.Sage.F_DOCENTETERepository();
            List<Model.Sage.F_DOCENTETE_Light> List = F_DOCENTETERepository.ListLight(0, (short)ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Facture_Comptabilisee_Vente, this.NucCountDoc.Value, true, this.textBoxFiltreNumeroClient.Text);
            ListBoxDocument.ItemsSource = List;
        }

        public void LoadReport()
        {
            string file = System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootReport, "AEC_Invoice.rpt");

            CRViewer.Owner = this;
            //System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(this);
            //helper.Owner = this.Handle;

            if (System.IO.File.Exists(file))
            {
                Etat = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                Etat.Load(file);
                // fonctions avec application sur les sous-états
                Core.PrintCrystal.Logon(Etat, Core.Global.GetConnectionInfos().SageServer, Core.Global.GetConnectionInfos().SageIntegratedSecurity, Core.Global.GetConnectionInfos().SageDatabase, Core.Global.GetConnectionInfos().SageSQLUser, Core.Global.GetConnectionInfos().SageSQLPass);
                
                // zoomfactor of 1 = pagewidth
                // zoomfactor of 2 = wholepage
                // zoomfactor of 25-400 = the % magnification
                CRViewer.ViewerCore.ZoomFactor = 2;
                CRViewer.ViewerCore.ZoomFactor = 75;
                
            }
        }

        private void buttonReloadListDocument_Click(object sender, RoutedEventArgs e)
        {
            LoadDatas();
        }

        private void ListBoxDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadViewer();
        }

        public void LoadViewer()
        {
            if (Etat != null && ListBoxDocument.SelectedItem != null)
            {
                Model.Sage.F_DOCENTETE_Light selected = (Model.Sage.F_DOCENTETE_Light)ListBoxDocument.SelectedItem;

                if (Etat.ParameterFields.Count == 0)
                {
                    Etat.RecordSelectionFormula = "{Commande.DO_Piece} = '" + selected.DO_Piece + "'";
                    Etat.Refresh();
                }
                else if (Etat.ParameterFields.Count > 0)
                {
                    Etat.SetParameterValue("piece", selected.DO_Piece);
                    //Etat.SetParameterValue("piecetaxes", selected.DO_Piece);
                    //Etat.SetParameterValue("piecetaxes", selected.DO_Piece, "Tableau_Taxes");
                }

                CRViewer.ViewerCore.ReportSource = Etat;

                /* pour fonction future 

                if (Etat.ParameterFields.Count == 0 && parameters != null && parameters.Count(p => p.ParameterType == Model.Internal.CrystalReportParameters._Type.SelectionFormula) > 0)
                    Etat.RecordSelectionFormula = parameters.FirstOrDefault(p => p.ParameterType == Model.Internal.CrystalReportParameters._Type.SelectionFormula).ParameterValue;

                // fonctions avec application sur les sous-états
                Core.PrintCrystal.Logon(Etat, Core.Global.GetConnectionInfos().SageServer, Core.Global.GetConnectionInfos().SageIntegratedSecurity, Core.Global.GetConnectionInfos().SageDatabase, Core.Global.GetConnectionInfos().SageSQLUser, Core.Global.GetConnectionInfos().SageSQLPass);

                Etat.Refresh();

                if (Etat.ParameterFields.Count > 0 && parameters != null && parameters.Count(p => p.ParameterType == Model.Internal.CrystalReportParameters._Type.ParameterField) > 0)
                {
                    foreach (Model.Internal.CrystalReportParameters crparam in parameters.Where(p => p.ParameterType == Model.Internal.CrystalReportParameters._Type.ParameterField))
                    {
                        Etat.SetParameterValue(crparam.ParameterName, crparam.ParameterValue);
                    }
                }
                */
            }
        }

        private void buttonExportModule_Click(object sender, RoutedEventArgs e)
        {
            if (Etat != null && ListBoxDocument.SelectedItem != null)
            {
                string report = System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootReport, "AEC_Invoice.rpt");
                Model.Sage.F_DOCENTETE_Light selected = (Model.Sage.F_DOCENTETE_Light)ListBoxDocument.SelectedItem;
                string temp_file = System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootReport, selected.DO_Piece + ".pdf");
                Core.PrintCrystal.ExportPDF(report
                    , new Model.Sage.Piece() { TotalAmountTaxExcl= 0, TotalTaxAmount= 0, DO_Date = selected.DO_Date, DO_Piece = selected.DO_Piece, cbMarq = 0 }
                    , 7
                    , selected.DO_Tiers
                    , temp_file);
            }
        }
    }
}
