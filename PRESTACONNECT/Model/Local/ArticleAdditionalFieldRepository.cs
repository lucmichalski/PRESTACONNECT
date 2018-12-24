using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class ArticleAdditionalFieldRepository
	{
		private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(ArticleAdditionalField Obj)
        {
            this.DBLocal.ArticleAdditionalField.InsertOnSubmit(Obj);
			this.DBLocal.SubmitChanges();
		}

        public void Save(ArticleAdditionalField Obj)
        {
			ArticleAdditionalField field = ReadFieldName(Obj.Art_id, Obj.FieldName);
			field.FieldValue = Obj.FieldValue;
			field.FieldValue2 = Obj.FieldValue2;
			this.DBLocal.SubmitChanges();
        }

        public void Delete(ArticleAdditionalField Obj)
        {
            this.DBLocal.ArticleAdditionalField.DeleteOnSubmit(Obj);
			this.DBLocal.SubmitChanges();
		}

		public List<Model.Local.ArticleAdditionalField> ListArticle(int Art_Id)
		{
			return DBLocal.ArticleAdditionalField.ToList().Where(obj => obj.Art_id == Art_Id).ToList();
		}

		public bool ExistFieldName(int Art_Id, string fieldName)
		{
			return DBLocal.ArticleAdditionalField.ToList().Where(obj => obj.Art_id == Art_Id && obj.FieldName == fieldName).Count()>0;
		}

		public Model.Local.ArticleAdditionalField ReadFieldName(int Art_Id, string fieldName)
		{
			return DBLocal.ArticleAdditionalField.ToList().Where(obj => obj.Art_id == Art_Id && obj.FieldName == fieldName).FirstOrDefault();
		}
	}
}
