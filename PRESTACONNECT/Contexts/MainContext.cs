using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using PRESTACONNECT.Core;

namespace PRESTACONNECT.Contexts
{
    internal sealed class MainContext : Context
    {
        #region Properties

        private BackgroundWorker loadShopsWorker;
        private BackgroundWorker LoadShopsWorker
        {
            get { return loadShopsWorker; }
            set
            {
                if (loadShopsWorker != null)
                {
                    loadShopsWorker.DoWork -= new DoWorkEventHandler(LoadShopsWorker_DoWork);
                    loadShopsWorker.ProgressChanged -= new ProgressChangedEventHandler(LoadShopsWorker_ProgressChanged);
                    loadShopsWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(LoadShopsWorker_RunWorkerCompleted);
                }

                loadShopsWorker = value;

                if (loadShopsWorker != null)
                {
                    loadShopsWorker.DoWork += new DoWorkEventHandler(LoadShopsWorker_DoWork);
                    loadShopsWorker.ProgressChanged += new ProgressChangedEventHandler(LoadShopsWorker_ProgressChanged);
                    loadShopsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadShopsWorker_RunWorkerCompleted);
                }
            }
        }

        private ObservableCollection<Model.Prestashop.PsShop> shops;
        public ObservableCollection<Model.Prestashop.PsShop> Shops
        {
            get { return shops; }
            set
            {
                shops = value;
                OnPropertyChanged("Shops");
            }
        }

        private Model.Prestashop.PsShop selectedShop;
        public Model.Prestashop.PsShop SelectedShop
        {
            get { return selectedShop; }
            set
            {
                selectedShop = value;
                Core.Global.Selected_Shop = (value != null) ? value.Name : "";
                OnPropertyChanged("SelectedShop");
            }
        }

        #endregion
        #region Constructors

        public MainContext()
            : base()
        {
            LoadShopsWorker = new BackgroundWorker();
            LoadShopsWorker.WorkerReportsProgress = true;

            Shops = new ObservableCollection<Model.Prestashop.PsShop>();
        }

        #endregion
        #region Overrriden methods

        protected override void OnLoaded()
        {
            base.OnLoaded();
        }

        #endregion
        #region Event methods

        public event EventHandler ShopsLoaded;
        private void OnShopsLoaded()
        {
            if (ShopsLoaded != null)
                ShopsLoaded(this, EventArgs.Empty);
        }

        private void LoadShopsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = Cursors.Wait; }), null);
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Shops.Clear(); }), null);

            IsBusy = true;
            LoadingStep = "Chargement des boutiques ...";

            Model.Prestashop.PsShopRepository PsShopRepository = new Model.Prestashop.PsShopRepository();

            foreach (var shop in PsShopRepository.List())
                if (shop.IDShop == Global.CurrentShop.IDShop)
                    LoadShopsWorker.ReportProgress(0, shop);
        }
        private void LoadShopsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(
                delegate { Shops.Add(e.UserState as Model.Prestashop.PsShop); }), null);
        }
        private void LoadShopsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadingStep = string.Empty;
            IsBusy = false;

            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = null; }), null);

            OnShopsLoaded();
        }

        #endregion
        #region Methods

        public void LoadShops()
        {
            if (!LoadShopsWorker.IsBusy)
                LoadShopsWorker.RunWorkerAsync();
        }

        #endregion
    }
}
