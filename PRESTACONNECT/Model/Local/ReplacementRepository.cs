using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class ReplacementRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Replacement text)
        {
            this.DBLocal.Replacement.InsertOnSubmit(text);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Replacement text)
        {
            this.DBLocal.Replacement.DeleteOnSubmit(text);
            this.Save();
        }

        public Boolean ExistOrigin(string text)
        {
            if (this.DBLocal.Replacement.Count(t => t.OriginText == text) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Replacement ReadOrigin(string text)
        {
            return this.DBLocal.Replacement.FirstOrDefault(t => t.OriginText == text);
        }

        public List<Replacement> List()
        {
            return (from Table in this.DBLocal.Replacement
                    select Table).ToList();
        }
    }
}
