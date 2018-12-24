using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class MediaAssignmentRuleRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(MediaAssignmentRule text)
        {
            this.DBLocal.MediaAssignmentRule.InsertOnSubmit(text);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(MediaAssignmentRule text)
        {
            this.DBLocal.MediaAssignmentRule.DeleteOnSubmit(text);
            this.Save();
        }

        public Boolean ExistSuffix(string suffix)
        {
            return this.DBLocal.MediaAssignmentRule.Count(t => t.SuffixText == suffix) > 0;
        }

        public MediaAssignmentRule ReadSuffix(string suffix)
        {
            return this.DBLocal.MediaAssignmentRule.FirstOrDefault(t => t.SuffixText == suffix);
        }

        public List<MediaAssignmentRule> List()
        {
            return (from Table in this.DBLocal.MediaAssignmentRule
                    select Table).ToList();
        }
    }
}
