using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using PRESTACONNECT.Core;
using System.IO;

namespace PRESTACONNECT.Contexts
{
    internal sealed class ConfigurationAvanceeContext : Context
    {
        #region Properties

        private string appRootFolder;
        public string AppRootFolder
        {
            get { return appRootFolder; }
            set
            {
                appRootFolder = value;
                OnPropertyChanged("AppRootFolder");
            }
        }

        private BackgroundWorker applyWorker;
        private BackgroundWorker ApplyWorker
        {
            get { return applyWorker; }
            set
            {
                if (applyWorker != null)
                {
                    applyWorker.DoWork -= new DoWorkEventHandler(ApplyWorker_DoWork);
                    applyWorker.ProgressChanged -= new ProgressChangedEventHandler(ApplyWorker_ProgressChanged);
                    applyWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(ApplyWorker_RunWorkerCompleted);
                }

                applyWorker = value;

                if (applyWorker != null)
                {
                    applyWorker.DoWork += new DoWorkEventHandler(ApplyWorker_DoWork);
                    applyWorker.ProgressChanged += new ProgressChangedEventHandler(ApplyWorker_ProgressChanged);
                    applyWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ApplyWorker_RunWorkerCompleted);
                }
            }
        }

        #endregion
        #region Constructors

        public ConfigurationAvanceeContext()
            : base()
        {
            ApplyWorker = new BackgroundWorker();
            ApplyWorker.WorkerReportsProgress = true;
        }

        #endregion
        #region Overrriden methods

        protected override void OnLoaded()
        {
            base.OnLoaded();

            AppRootFolder = Global.GetConfig().Folders.Root;
        }

        #endregion
        #region Event methods

        public event EventHandler Saved;
        private void OnSaved()
        {
            if (Saved != null)
                Saved(this, EventArgs.Empty);
        }

        private void ApplyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = Cursors.Wait; }), null);

            IsBusy = true;
            LoadingStep = "Transfert en cours ...";

            string from = Global.GetConfig().Folders.Root;

            if (!from.Equals(AppRootFolder, StringComparison.CurrentCultureIgnoreCase))
            {
                Global.GetConfig().UpdateFolders(AppRootFolder);

                if (Directory.Exists(from))
                {
                    foreach (var folder in Directory.GetDirectories(from))
                        Directory.Move(folder, Path.Combine(AppRootFolder, Path.GetFileName(folder)));

                    foreach (var file in Directory.GetFiles(from))
                        File.Move(file, Path.Combine(AppRootFolder, Path.GetFileName(file)));

                    Directory.Delete(from);
                }

            }
        }
        private void ApplyWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }
        private void ApplyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                OnSaved();
            }
            finally
            {
                LoadingStep = string.Empty;
                IsBusy = false;
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = null; }), null);
            }
        }

        #endregion
        #region Methods

        public void ApplyChanges()
        {
            ApplyWorker.RunWorkerAsync();
        }

        #endregion
    }
}
