using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PRESTACONNECT.Model.Internal;
using System.Threading.Tasks;

namespace PRESTACONNECT.Contexts
{
    internal sealed class LienConditionnementContext : Context
    {
        #region Properties

        private ObservableCollection<Model.Local.ConditioningArticle> listConditioningArticle;
        public ObservableCollection<Model.Local.ConditioningArticle> ListConditioningArticle
        {
            get { return listConditioningArticle; }
            set
            {
                listConditioningArticle = value;
                OnPropertyChanged("ListConditioningArticle");
            }
        }
        private Model.Local.ConditioningArticle selectedConditioningArticle;
        public Model.Local.ConditioningArticle SelectedConditioningArticle
        {
            get { return selectedConditioningArticle; }
            set
            {
                selectedConditioningArticle = value;
                OnPropertyChanged("SelectedConditioningArticle");
                OnPropertyChanged("TextButtonCheckByRef");
            }
        }

        public String TextButtonCheckByRef
        {
            get
            {
                return (SelectedConditioningArticle != null) ? "Cocher/décocher pour la référence : " + SelectedConditioningArticle.Article.Art_Ref : "Pas de référence sélectionnée";
            }
        }

        private Model.Local.ConditioningArticleRepository ConditioningArticleRepository = new Model.Local.ConditioningArticleRepository();

        #endregion

        #region Constructors

        public LienConditionnementContext()
            : base()
        {
            Core.Temp.ListF_CONDITION = new Model.Sage.F_CONDITIONRepository().List();
            Core.Temp.ListPsAttributeLang = new Model.Prestashop.PsAttributeLangRepository().ListLang(Core.Global.Lang).AsParallel().ToList();
            Core.Temp.ListP_CONDITIONNEMENT = new Model.Sage.P_CONDITIONNEMENTRepository().ListIntituleNotNull();

            this.ConditioningArticleRepository = new Model.Local.ConditioningArticleRepository();
            ListConditioningArticle = new ObservableCollection<Model.Local.ConditioningArticle>(this.ConditioningArticleRepository.List());
        }

        #endregion

        #region Methods

        private void DeletePsProductAttribute(int idproductattribute)
        {
            try
            {
                Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
                if (PsProductAttributeRepository.ExistProductAttribute((uint)idproductattribute))
                {
                    PsProductAttributeRepository.Delete(PsProductAttributeRepository.ReadProductAttribute((uint)idproductattribute));
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        internal void CheckByRef()
        {
            int importCount = ListConditioningArticle.AsParallel().Count(r => r.Art_Id == SelectedConditioningArticle.Art_Id && r.CanReplace && r.Replace);
            bool toCheck = ((ListConditioningArticle.AsParallel().Count(r => r.Art_Id == SelectedConditioningArticle.Art_Id && r.CanReplace && !r.Replace) - importCount) >= importCount);

            Parallel.ForEach(ListConditioningArticle.AsParallel().Where(r => r.Art_Id == SelectedConditioningArticle.Art_Id && r.CanReplace), a => a.Replace = toCheck);
        }

        internal void CheckAll()
        {
            int importCount = ListConditioningArticle.AsParallel().Count(r => r.CanReplace && r.Replace);
            bool toCheck = ((ListConditioningArticle.AsParallel().Count(r => r.CanReplace && !r.Replace) - importCount) >= importCount);

            Parallel.ForEach(ListConditioningArticle.AsParallel().Where(r => r.CanReplace), a => a.Replace = toCheck);
        }

        internal void SaveReplace()
        {
            foreach (Model.Local.ConditioningArticle item in ListConditioningArticle)
            {
                if(item.Replace && item.EnumereF_CONDITIONSageNew.cbMarq > 0)
                {
                    item.Sag_Id = item.EnumereF_CONDITIONSageNew.cbMarq;
                }
            }
            this.ConditioningArticleRepository.Save();

            // reload
            this.ConditioningArticleRepository = new Model.Local.ConditioningArticleRepository();
            ListConditioningArticle = new ObservableCollection<Model.Local.ConditioningArticle>(this.ConditioningArticleRepository.List());
        }
    }
}
