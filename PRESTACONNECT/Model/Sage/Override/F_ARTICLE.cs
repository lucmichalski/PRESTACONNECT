using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Sage
{
    public partial class F_ARTICLE : INotifyPropertyChanged
    {
        public bool Exist
        {
            get
            {
                return Core.Temp.ListArticleLocal.Contains(this.cbMarq);
            }
        }

        public string UniteVenteString
        {
            get
            {
                string r = string.Empty;
                Model.Sage.P_UNITERepository P_UNITERepository = new P_UNITERepository();
                if (AR_UniteVen != null && P_UNITERepository.ExistUnite(AR_UniteVen.Value))
                    r = P_UNITERepository.ReadUnite(AR_UniteVen.Value).U_Intitule;
                return r;
            }
        }

        #region Events

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Information libre
        //private ObservableCollection<InformationLibre> m_AR_ObservableInformationLibre = new ObservableCollection<InformationLibre>();

        //public ObservableCollection<InformationLibre> AR_ObservableInformationLibre
        //{
        //    get
        //    {
        //        return this.m_AR_ObservableInformationLibre;
        //    }
        //    set
        //    {
        //        this.m_AR_ObservableInformationLibre = value;
        //        OnPropertyChanged("AR_ObservableInformationLibre");
        //    }
        //}

        //public void LoadInformationsLibres(Model.Sage.F_ARTICLERepository F_ARTICLERepository)
        //{
        //    Model.Sage.cbSysLibreRepository cbSysLibreRepository = new cbSysLibreRepository();
        //    ObservableCollection<Model.Sage.cbSysLibre> ObservablecbSysLibre = cbSysLibreRepository.ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE);
        //    if (ObservablecbSysLibre.Count > 0)
        //        foreach (Model.Sage.cbSysLibre cbSysLibre in ObservablecbSysLibre)
        //            if (F_ARTICLERepository.ExistInformationLibreRef(cbSysLibre.CB_Name, this.AR_Ref))
        //                this.AR_ObservableInformationLibre.Add(new InformationLibre()
        //                {
        //                    Name = cbSysLibre.CB_Name,
        //                    Value = F_ARTICLERepository.ReadInformationLibreRef(cbSysLibre.CB_Name, this.AR_Ref),
        //                    Pos = cbSysLibre.CB_Pos
        //                });
        //}
        #endregion

        #region Methods

        public override string ToString()
        {
            return AR_Design;
        }

        #endregion
    }

    public class F_ARTICLE_Light : INotifyPropertyChanged
    {
        private string ar_Ref;
        public string AR_Ref
        {
            get { return ar_Ref; }
            set { ar_Ref = value; OnPropertyChanged("AR_Ref"); }
        }
        public string ar_Design = string.Empty;
        public string AR_Design
        {
            get { return ar_Design; }
            set { ar_Design = value; OnPropertyChanged("AR_Design"); }
        }
        public string FA_CodeFamille = string.Empty;
        public short? AR_Gamme1 = 0;
        public short? AR_Gamme2 = 0;
        public short? AR_Condition = 0;
        public short? AR_SuiviStock = 0;
        public short? AR_Sommeil = 0;
        public short? AR_Publie = 0;
        public int _cbMarq = 0;
        public int cbMarq
        {
            get { return _cbMarq; }
            set { _cbMarq = value; OnPropertyChanged("cbMarq"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return AR_Ref + " " + AR_Design;
        }
    }

    public class F_ARTICLE_Import : INotifyPropertyChanged
    {
        private string ar_Ref = string.Empty;
        public string AR_Ref
        {
            get { return ar_Ref; }
            set { ar_Ref = value; OnPropertyChanged("AR_Ref"); }
        }
        public string ar_Design = string.Empty;
        public string AR_Design
        {
            get { return ar_Design; }
            set { ar_Design = value; OnPropertyChanged("AR_Design"); }
        }
        public string FA_CodeFamille = string.Empty;
        public short? AR_Gamme1 = 0;
        public short? AR_Gamme2 = 0;
        public short? AR_Condition = 0;
        public short? AR_SuiviStock = 0;
        public short? AR_Sommeil = 0;
        public short? AR_Publie = 0;
        public string AR_CodeBarre = string.Empty;
        public int? CL_No1 = 0;
        public int? CL_No2 = 0;
        public int? CL_No3 = 0;
        public int? CL_No4 = 0;
        public int cbMarq;

        private string af_Ref = string.Empty;
        public string AF_Ref
        {
            get { return af_Ref; }
            set { af_Ref = (value != null) ? value : string.Empty; OnPropertyChanged("AF_Ref"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Exist
        {
            get
            {
                return Core.Temp.ListArticleLocal.Contains(this.cbMarq);
            }
            set { }
        }

        private Boolean m_AR_IsCheckedImport = false;
        public Boolean AR_IsCheckedImport
        {
            get { return this.m_AR_IsCheckedImport; }
            set
            {
                this.m_AR_IsCheckedImport = value;
                OnPropertyChanged("AR_IsCheckedImport");
            }
        }

        public override string ToString()
        {
            return AR_Ref + " " + AR_Design;
        }
    }

    public class F_ARTICLE_Composition : INotifyPropertyChanged
    {
        private string ar_Ref;
        public string AR_Ref
        {
            get { return (ar_Ref != null) ? ar_Ref : string.Empty; }
            set { ar_Ref = value; OnPropertyChanged("AR_Ref"); }
        }
        private string ar_Design;
        public string AR_Design
        {
            get { return (ar_Design != null) ? ar_Design : string.Empty; }
            set { ar_Design = value; OnPropertyChanged("AR_Design"); }
        }

        private string fa_Intitule;
        public string FA_Intitule
        {
            get { return (fa_Intitule != null) ? fa_Intitule : string.Empty; }
            set { fa_Intitule = value; OnPropertyChanged("FA_Intitule"); }
        }

        private string gamme1;
        public string Gamme1
        {
            get { return (gamme1 != null) ? gamme1 : string.Empty; }
            set { gamme1 = value; OnPropertyChanged("Gamme1"); }
        }
        private string gamme2;
        public string Gamme2
        {
            get { return (gamme2 != null) ? gamme2 : string.Empty; }
            set { gamme2 = value; OnPropertyChanged("Gamme2"); }
        }
        private string enumereGamme1;
        public string EnumereGamme1
        {
            get { return (enumereGamme1 != null) ? enumereGamme1 : string.Empty; }
            set { enumereGamme1 = value; OnPropertyChanged("EnumereGamme1"); }
        }
        private string enumereGamme2;
        public string EnumereGamme2
        {
            get { return (enumereGamme2 != null) ? enumereGamme2 : string.Empty; }
            set { enumereGamme2 = value; OnPropertyChanged("EnumereGamme2"); }
        }
        private string ae_Ref;
        public string AE_Ref
        {
            get { return (ae_Ref != null) ? ae_Ref : string.Empty; }
            set { ae_Ref = value; OnPropertyChanged("AE_Ref"); }
        }

        private string typeconditionnement;
        public string TypeConditionnement
        {
            get { return (typeconditionnement != null) ? typeconditionnement : string.Empty; }
            set { typeconditionnement = value; OnPropertyChanged("TypeConditionnement"); }
        }
        private string ec_enumere;
        public string EC_Enumere
        {
            get { return (ec_enumere != null) ? ec_enumere : string.Empty; }
            set { ec_enumere = value; OnPropertyChanged("EC_Enumere"); }
        }
        private string co_Ref;
        public string CO_Ref
        {
            get { return (co_Ref != null) ? co_Ref : string.Empty; }
            set { co_Ref = value; OnPropertyChanged("CO_Ref"); }
        }
        private string ta_Code1;
        public string TA_Code1
        {
            get { return (ta_Code1 != null) ? ta_Code1 : string.Empty; }
            set { ta_Code1 = value; OnPropertyChanged("TA_Code1"); }
        }
        private string ta_Code2;
        public string TA_Code2
        {
            get { return (ta_Code2 != null) ? ta_Code2 : string.Empty; }
            set { ta_Code2 = value; OnPropertyChanged("TA_Code2"); }
        }
        private string ta_Code3;
        public string TA_Code3
        {
            get { return (ta_Code3 != null) ? ta_Code3 : string.Empty; }
            set { ta_Code3 = value; OnPropertyChanged("TA_Code3"); }
        }
        private string ta_CodeFamille1;
        public string TA_CodeFamille1
        {
            get { return (ta_CodeFamille1 != null) ? ta_CodeFamille1 : string.Empty; }
            set { ta_CodeFamille1 = value; OnPropertyChanged("TA_CodeFamille1"); }
        }
        private string ta_CodeFamille2;
        public string TA_CodeFamille2
        {
            get { return (ta_CodeFamille2 != null) ? ta_CodeFamille2 : string.Empty; }
            set { ta_CodeFamille2 = value; OnPropertyChanged("TA_CodeFamille2"); }
        }
        private string ta_CodeFamille3;
        public string TA_CodeFamille3
        {
            get { return (ta_CodeFamille3 != null) ? ta_CodeFamille3 : string.Empty; }
            set { ta_CodeFamille3 = value; OnPropertyChanged("TA_CodeFamille3"); }
        }

        public string FA_CodeFamille = string.Empty;
        public short? AR_Gamme1 = 0;
        public short? AR_Gamme2 = 0;
        public short? AR_Condition = 0;
        public short? AR_SuiviStock = 0;
        public short? AR_Sommeil = 0;
        public short? AR_Publie = 0;
        public decimal? EC_Quantite = 0;
        public short? AE_Sommeil = 0;

        public int? CL_No1 = 0;
        private string catalogue1;
        public string Catalogue1
        {
            get { return (catalogue1 != null) ? catalogue1 : string.Empty; }
            set { catalogue1 = value; OnPropertyChanged("Catalogue1"); }
        }
        public int? Catalogue1_SagId;

        public int? CL_No2 = 0;
        private string catalogue2;
        public string Catalogue2
        {
            get { return (catalogue2 != null) ? catalogue2 : string.Empty; }
            set { catalogue2 = value; OnPropertyChanged("Catalogue2"); }
        }
        public int? Catalogue2_SagId;

        public int? CL_No3 = 0;
        private string catalogue3;
        public string Catalogue3
        {
            get { return (catalogue3 != null) ? catalogue3 : string.Empty; }
            set { catalogue3 = value; OnPropertyChanged("Catalogue3"); }
        }
        public int? Catalogue3_SagId;

        public int? CL_No4 = 0;
        private string catalogue4;
        public string Catalogue4
        {
            get { return (catalogue4 != null) ? catalogue4 : string.Empty; }
            set { catalogue4 = value; OnPropertyChanged("Catalogue4"); }
        }
        public int? Catalogue4_SagId;

        public string Catalogue
        {
            get
            {
                string t = Catalogue1;
                if(!string.IsNullOrEmpty(Catalogue2))
                    t += " > " + Catalogue2;
                if(!string.IsNullOrEmpty(Catalogue3))
                    t += " > " + Catalogue3;
                if(!string.IsNullOrEmpty(Catalogue4))
                    t += " > " + Catalogue4;
                return t;
            }
            set { }
        }

        public int cbMarq;
        public int? F_ARTENUMREF_SagId;
        public int? F_CONDITION_SagId;

        public string Gamme
        {
            get
            {
                string r = string.Empty;
                if (!string.IsNullOrWhiteSpace(Gamme1))
                    r += Gamme1 + " : " + EnumereGamme1;
                if (!string.IsNullOrWhiteSpace(Gamme2))
                    r += " / " + Gamme2 + " : " + EnumereGamme2;
                return r;
            }
            set { }
        }
        public string Conditionnement
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(TypeConditionnement)) ? TypeConditionnement + " : " + EC_Enumere : string.Empty;
            }
            set { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private Boolean ar_IsCheckedToAdd = false;
        public Boolean AR_IsCheckedToAdd
        {
            get { return this.ar_IsCheckedToAdd; }
            set
            {
                this.ar_IsCheckedToAdd = value;
                OnPropertyChanged("AR_IsCheckedToAdd");
            }
        }

        public override string ToString()
        {
            return AR_Ref + " " + AR_Design;
        }
    }

    public class F_ARTICLE_Photo : INotifyPropertyChanged
    {
        public int _cbMarq = 0;
        public int cbMarq
        {
            get { return _cbMarq; }
            set { _cbMarq = value; OnPropertyChanged("cbMarq"); }
        }
        private string ar_Ref;
        public string AR_Ref
        {
            get { return ar_Ref; }
            set { ar_Ref = value; OnPropertyChanged("AR_Ref"); }
        }
        public string ar_Photo = string.Empty;
        public string AR_Photo
        {
            get { return ar_Photo; }
            set { ar_Photo = value; OnPropertyChanged("AR_Photo"); }
        }
        
        private string ac_RefClient;
        public string AC_RefClient
        {
            get { return ac_RefClient; }
            set { ac_RefClient = value; OnPropertyChanged("AC_RefClient"); }
        }
        public string ac_Photo = string.Empty;
        public string AC_Photo
        {
            get { return ac_Photo; }
            set { ac_Photo = value; OnPropertyChanged("AC_Photo"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return AR_Ref;
        }
    }
}
