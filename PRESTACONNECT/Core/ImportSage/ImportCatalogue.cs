using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.ImportSage
{
    public class ImportCatalogue
    {
        List<String> log;

        public void Exec(Int32 F_CATALOGUESend, out List<String> log_out)
        {
            log = new List<string>();
            try
            {
                Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                if (CatalogRepository.ExistSag_Id(F_CATALOGUESend) == false)
                {
                    Model.Sage.F_CATALOGUERepository F_CATALOGUERepository = new Model.Sage.F_CATALOGUERepository();
                    Model.Sage.F_CATALOGUE F_CATALOGUE = F_CATALOGUERepository.ReadCatalogue(F_CATALOGUESend);

                    string dft = Core.Global.RemovePurge(F_CATALOGUE.CL_Intitule, 128);
                    Model.Local.Catalog Catalog = new Model.Local.Catalog()
                    {
                        Cat_Name = dft,
                        Cat_Description = F_CATALOGUE.CL_Intitule,
                        Cat_MetaTitle = dft,
                        Cat_MetaDescription = dft,
                        Cat_MetaKeyword = Core.Global.RemovePurgeMeta(dft, 255),
                        Cat_LinkRewrite = Core.Global.ReadLinkRewrite(dft),
                        Cat_Active = true,
                        Cat_Date = DateTime.Now,
                        Cat_Sync = true,
                        Cat_Level = 2,
                        Sag_Id = F_CATALOGUE.cbMarq
                    };

                    //if (F_CATALOGUERepository.ExistParent(F_CATALOGUE.CL_NoParent.Value))
                    //{
                    //    Model.Sage.F_CATALOGUE F_CATALOGUEParent = F_CATALOGUERepository.ReadParent(F_CATALOGUE.CL_NoParent.Value);
                    //    Catalog.Cat_Parent = F_CATALOGUEParent.cbMarq;
                    //}
                    //<YH> 19/11/2012 : L'usage du Cat_Parent a été modifié
                    //<JG> 29/01/2013 Correction récupération du Cat_Id par rapport au cbMarq du parent dans la base locale et non au CL_No
                    // <JG> 07/10/2016 correction calcul niveau catalogue si import des enfants sans les parents
                    if (F_CATALOGUERepository.ExistParent(F_CATALOGUE.CL_NoParent.Value) && CatalogRepository.ExistSag_Id(F_CATALOGUE.F_CATALOGUE1.cbMarq))
                    {
                        Model.Local.Catalog parent = CatalogRepository.ReadSag_Id(F_CATALOGUE.F_CATALOGUE1.cbMarq);
                        Catalog.Cat_Parent = parent.Cat_Id;
                        Catalog.Cat_Level = parent.Cat_Level + 1;
                    }

                    CatalogRepository.Add(Catalog);

                    log.Add("IC10- Import du catalogue Sage [ " + F_CATALOGUE.ComboText + " ]");
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("IC01- Une erreur est survenue : " + ex.ToString());
            }
            finally
            {
                log_out = log;
            }
        }
    }
}
