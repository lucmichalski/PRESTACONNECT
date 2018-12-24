using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class P_CATCOMPTARepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public List<P_CATCOMPTA> List()
        {
            return this.DBSage.P_CATCOMPTA.ToList();
        }

        public List<String> ListStringVente()
        {
            List<String> Return = new List<String>();
            System.Linq.IQueryable<P_CATCOMPTA> List = this.DBSage.P_CATCOMPTA;
            System.Reflection.PropertyInfo[] Properties = List.ToList()[0].GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo Property in Properties)
            {
                if (Property.GetValue(List.ToList()[0], null).ToString() != "" && Property.Name != "cbMarq")
                {
                    if (Property.Name.Contains("Ven"))
                    {
                        Return.Add(Property.Name.Split(new String[] { "Ven" }, StringSplitOptions.None)[1] + " - " + Property.GetValue(List.ToList()[0], null).ToString());
                    }           
                }
            }
            return Return;
        }

        public List<Model.Internal.CategorieComptable> ListCatComptaVente()
        {
            List<Model.Internal.CategorieComptable> Return = new List<Model.Internal.CategorieComptable>();
            System.Linq.IQueryable<P_CATCOMPTA> List = this.DBSage.P_CATCOMPTA;
            System.Reflection.PropertyInfo[] Properties = List.ToList()[0].GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo Property in Properties)
            {
                if (Property.GetValue(List.ToList()[0], null).ToString() != "" 
                    && Property.Name != "cbMarq" 
                    && Property.Name.Contains("Ven"))
                {
                    string Marq = Property.Name.Split(new String[] { "Ven" }, StringSplitOptions.None)[1];
                    if (Core.Global.IsInteger(Marq))
                    {
                        Return.Add(new Model.Internal.CategorieComptable()
                        {
                            SageMarq = int.Parse(Marq),
                            Intitule = Property.GetValue(List.ToList()[0], null).ToString(),
                        });
                    }
                }
            }
            return Return;
        }
    }
}
