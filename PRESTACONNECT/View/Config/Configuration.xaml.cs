using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using PRESTACONNECT.Contexts;
using PRESTACONNECT.Core;
using PRESTACONNECT.View;
using PRESTACONNECT.Model.Internal;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Data;

namespace PRESTACONNECT
{
    public partial class Configuration : Window
    {
        #region Properties

        internal new ConfigurationContext DataContext
        {
            get { return (ConfigurationContext)base.DataContext; }
            private set
            {
                if (value != null)
                {
                }

                base.DataContext = value;

                if (value != null)
                {
                }
            }
        }

        Boolean IsLoad = false;

        #endregion
        #region Event methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext.Load();
        }
        private void Advanced_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationAvancee dialog = new ConfigurationAvancee();
            dialog.Owner = this;

            dialog.ShowDialog();
        }

        #endregion

        public Configuration()
        {
            try
            {
                DataContext = new ConfigurationContext();
                this.InitializeComponent();
                this.TabItemMail.IsSelected = true;
                if (Core.Global.GetConfig().UIDisabledWYSIWYG)
                {
                    this.TabItemWYSIWYG.Visibility = System.Windows.Visibility.Collapsed;
                    this.TabItemHTMLEdit.IsSelected = true;
                    this.buttonInsertHTML.Visibility = System.Windows.Visibility.Hidden;
                    this.buttonLoadHTML.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    this.TinyMceMailTemplateContent.CreateEditor();
                }
                this.LoadMode();
                this.LoadComboLang();
                this.LoadFTP();
                this.LoadArticle();
                this.LoadClient();
                this.LoadTaxe();
                this.LoadCommande();
                this.LoadReglementEnabled();
                this.LoadReglement();
                this.LoadCarrier();
                this.LoadOrderMail();
				this.LoadOrderMarcketplace();
				DataContext.LoadGroups();
                DataContext.LoadConfig();
				LoadRequestSQLBase();

				if (Core.UpdateVersion.License.ExtranetOnly)
                {
                    this.TabItemFTP.IsSelected = true;
                    this.TabItemClientTransfert.IsSelected = true;
                    this.TabItemCommandeModules.IsSelected = true;
                }

                if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                    this.WindowState = Core.Temp.Current;

                this.IsLoad = true;
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            // Insérez le code requis pour la création d’objet sous ce point.
        }

        #region Button

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            this.WriteMode();
            this.WriteComboLang();
            this.WriteFTP();
            this.WriteArticle();
            this.WriteClient();
            this.WriteReglement();
            this.WriteCommande();
            this.WriteOrderMail();
            DataContext.SaveConfig();
            MessageBox.Show("Mise à jour effectuée avec succès", "Configuration");
        }

        private void ButtonFTPTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string Dir = System.IO.Directory.GetCurrentDirectory();
                //Dir += "\\Img\\";
                //Dir += "favicon.png";

                string Dir = System.IO.Directory.GetCurrentDirectory() + "\\test.txt";
                if (System.IO.File.Exists(Dir) == false)
                {
                    StreamWriter fichier = new StreamWriter(Dir, true, Encoding.UTF8);
                    fichier.WriteLine("Ce fichier correspond à un test d'écriture via une connexion FTP depuis le logiciel PRESTACONNECT.");
                    fichier.Close();
                }

                string ftpfullpath = this.TextBoxFTPIP.Text + "/prestaconnecttest.txt";
                System.Net.FtpWebRequest ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpfullpath);
                ftp.Credentials = new System.Net.NetworkCredential(this.TextBoxFTPUser.Text, this.PasswordBox.Password);
                //userid and password for the ftp server to given  

                ftp.KeepAlive = true;
                ftp.UseBinary = true;
                ftp.EnableSsl = this.CheckBoxFTPSSL.IsChecked.Value;
                ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                System.IO.FileStream fs = System.IO.File.OpenRead(Dir);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                System.IO.Stream ftpstream = ftp.GetRequestStream();
                ftpstream.Write(buffer, 0, buffer.Length);
                ftpstream.Close();
                ftp.Abort();
                MessageBox.Show("Connexion réussie", "Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Net.WebException ex2)
            {
                String status = (ex2.Response != null) ? ((System.Net.FtpWebResponse)ex2.Response).StatusDescription : ex2.Status.ToString();
                MessageBox.Show("Erreur FTP : " + ex2.Message + "\n" + status, "Configuration", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible de se connecter à votre ftp. Erreur : " + ex.Message, "Configuration", MessageBoxButton.OK, MessageBoxImage.Error);
            }
		}

		public DataGrid resultSQL;
		private void RequestExecButton_Click(object sender, RoutedEventArgs e)
		{
			string request = RequestSQLText.Text.Trim();
			if (!string.IsNullOrEmpty(request))
			{
				ConnectionInfos connexions = Core.Global.GetConnectionInfos();
				// Exécution de la requête
				switch (RequestSQLDatabase.SelectedIndex)
				{
					case 0:
						if (RequestSQLDatabase.SelectedValue.Equals(connexions.SageDatabase))
						{
						}
						break;
					case 1:
						if (RequestSQLDatabase.SelectedValue.Equals(connexions.PrestaconnectDatabase))
						{

						}
						break;
					case 2:
						if (RequestSQLDatabase.SelectedValue.Equals(connexions.PrestashopDatabase))
						{
							MySqlConnection connection = null;
							try
							{
								string connectionString = Properties.Settings.Default.PRESTASHOPConnectionString;
								connection = new MySqlConnection(connectionString);
								connection.Open();

								MySqlCommand cmd = connection.CreateCommand();
								cmd.CommandText = request;
								MySqlDataReader reader = cmd.ExecuteReader();

								if (reader.HasRows)
								{
									DataTable schemaTable = reader.GetSchemaTable();

									DataTable Result = new DataTable();
									foreach (DataRow row in schemaTable.Rows)
									{
										Result.Columns.Add(row[0].ToString());
									}

									int j = 0;
									while (reader.Read())
									{
										DataRow rowData = Result.NewRow();
										for( int i = 0; i < Result.Columns.Count; i++)
										{
											rowData[i] = reader.GetValue(i).ToString();
										}
										Result.Rows.Add(rowData);
										j++;
									}
								}
							}
							catch (Exception ex)
							{
								MessageBox.Show("Message d'erreur : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
							}
							finally
							{
								if (connection != null && connection.State != ConnectionState.Closed)
									connection.Close();
							}
						}
						break;
					default:
						MessageBox.Show("La base de données de travail n'a pas été sélectionnée !", "Erreur : Requête SQL", MessageBoxButton.OK, MessageBoxImage.Error);
						break;
				}
			}
		}

		private void RequestExecButtonCSV_Click(object sender, RoutedEventArgs e)
		{
			RequestExecButton_Click(sender, e);
		}

		#endregion

		#region Global

		private void LoadMode()
        {
            this.RadioButtonBToC.IsChecked = Core.Global.GetConfig().ConfigBToC;
            this.RadioButtonBToB.IsChecked = Core.Global.GetConfig().ConfigBToB;
        }

        private void WriteMode()
        {
            Core.Global.GetConfig().UpdateConfigBtoCBtoB(this.RadioButtonBToC.IsChecked.Value, this.RadioButtonBToB.IsChecked.Value);
        }

        private void LoadComboLang()
        {
            this.ComboBoxLang.Items.Clear();
            Model.Prestashop.PsLangRepository LangRepository = new Model.Prestashop.PsLangRepository();
            List<Model.Prestashop.PsLang> ListLang = LangRepository.ListActive(1, Global.CurrentShop.IDShop);
            String CurrentLang = "";
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigLang))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigLang);
                CurrentLang = Config.Con_Value;
            }
            foreach (Model.Prestashop.PsLang Lang in ListLang)
            {
                this.ComboBoxLang.Items.Add(Lang.IDLang + " - " + Lang.IsoCode);
                if (CurrentLang == Lang.IDLang.ToString())
                {
                    this.ComboBoxLang.SelectedItem = Lang.IDLang + " - " + Lang.IsoCode;
                }
            }
        }

        private void WriteComboLang()
        {
            if (this.ComboBoxLang.SelectedItem != null && this.ComboBoxLang.SelectedItem.ToString() != "")
            {
                Boolean isLang = false;

                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                Model.Local.Config Config = new Model.Local.Config();
                if (ConfigRepository.ExistName(Core.Global.ConfigLang))
                {
                    Config = ConfigRepository.ReadName(Core.Global.ConfigLang);
                    isLang = true;
                }
                Config.Con_Value = Core.Global.SplitValue(this.ComboBoxLang.SelectedItem.ToString());
                if (isLang == true)
                {
                    ConfigRepository.Save();
                }
                else
                {
                    Config.Con_Name = Core.Global.ConfigLang;
                    ConfigRepository.Add(Config);
                }

                Global.Lang = Convert.ToUInt32(Config.Con_Value);
            }
        }

        private void LoadFTP()
        {
            this.CheckBoxImage.IsChecked = Core.Global.GetConfig().ConfigFTPActive;
            this.TextBoxFTPIP.Text = Core.Global.GetConfig().ConfigFTPIP;
            this.TextBoxFTPUser.Text = Core.Global.GetConfig().ConfigFTPUser;
            this.PasswordBox.Password = Core.Global.GetConfig().ConfigFTPPassword;
            this.CheckBoxFTPSSL.IsChecked = Core.Global.GetConfig().ConfigFTPSSL;
        }

        private void WriteFTP()
        {
            Core.Global.GetConfig().UpdateFTPActive(this.CheckBoxImage.IsChecked.Value);
            Core.Global.GetConfig().UpdateFTPIP(this.TextBoxFTPIP.Text);
            Core.Global.GetConfig().UpdateFTPUser(this.TextBoxFTPUser.Text);
            Core.Global.GetConfig().UpdateFTPPass(this.PasswordBox.Password);
            Core.Global.GetConfig().UpdateFTPSSL(this.CheckBoxFTPSSL.IsChecked.Value);
        }

        private void ListBoxImageStorageMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoad)
            {
                Model.Prestashop.PsConfigurationRepository PsConfigurationRepository = new Model.Prestashop.PsConfigurationRepository();
                if (PsConfigurationRepository.ExistName("PS_LEGACY_IMAGES"))
                {
                    Model.Prestashop.PsConfiguration image_storage_mode = PsConfigurationRepository.ReadName("PS_LEGACY_IMAGES");
                    int value;
                    if (int.TryParse(image_storage_mode.Value, out value))
                    {
                        if (value != (short)this.DataContext.SelectedImageStorageMode._ImageStorageMode)
                        {
                            MessageBox.Show("Attention le système de gestion sélectionné est différent de celui de votre Prestashop !",
                                "Mode de gestion des images", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
        }

        private void ButtonChangeLocalStorageMode_Click(object sender, RoutedEventArgs e)
        {
            DataContext.ChangeLocalStorageMode();
        }

		private void LoadRequestSQLBase()
		{
			RequestSQLDatabase.Items.Clear();
			ConnectionInfos connexions = Core.Global.GetConnectionInfos();
			RequestSQLDatabase.Items.Add(connexions.SageDatabase);
			RequestSQLDatabase.Items.Add(connexions.PrestaconnectDatabase);
			RequestSQLDatabase.Items.Add(connexions.PrestashopDatabase);
		}

        #endregion

        #region Article

        private void LoadArticle()
        {
            this.LoadCheckBoxSupply();
            this.LoadComboBoxStock();
            this.LoadContremarque();
            this.LoadComboBoxRupture();
            this.LoadComboBoxType();
            this.LoadComboBoxUnite();
            this.LoadComboBoxCategorieTarifaire();
            this.LoadComboBoxCategorieComptable();
            this.LoadCheckBoxPublieWeb();
            this.LoadCheckBoxSommeil();
        }

        private void LoadCheckBoxSupply()
        {
            Model.Local.SupplyRepository SupplyRepository = new Model.Local.SupplyRepository();
            this.ListBoxSupply.ItemsSource = SupplyRepository.ListOrderByName();
        }

        private void CheckBoxSupply_Click(object sender, RoutedEventArgs e)
        {
            CheckBox Result = e.Source as CheckBox;
            Model.Local.SupplyRepository SupplyRepository = new Model.Local.SupplyRepository();
            Model.Local.Supply Supply = SupplyRepository.ReadName(Result.Content.ToString());
            Supply.Sup_Active = Result.IsChecked.Value;
            SupplyRepository.Save();
        }

        private void LoadComboBoxStock()
        {
            this.ComboBoxArticleStock.Items.Clear();
            this.ComboBoxArticleStock.Items.Add(Core.Global.GestionStockAucun);
            this.ComboBoxArticleStock.Items.Add(Core.Global.GestionStockAterme);
            this.ComboBoxArticleStock.Items.Add(Core.Global.GestionStockReel);
            this.ComboBoxArticleStock.Items.Add(Core.Global.GestionStockDisponible);
            this.ComboBoxArticleStock.Items.Add(Core.Global.GestionStockDisponibleAvance);

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigArticleStock))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleStock);
                switch (Config.Con_Value)
                {
                    case "0":
                        this.ComboBoxArticleStock.SelectedItem = Core.Global.GestionStockAucun;
                        break;
                    case "1":
                        this.ComboBoxArticleStock.SelectedItem = Core.Global.GestionStockAterme;
                        break;
                    case "2":
                        this.ComboBoxArticleStock.SelectedItem = Core.Global.GestionStockReel;
                        break;
                    case "3":
                        this.ComboBoxArticleStock.SelectedItem = Core.Global.GestionStockDisponible;
                        break;
                    case "4":
                        this.ComboBoxArticleStock.SelectedItem = Core.Global.GestionStockDisponibleAvance;
                        break;
                    default:
                        this.ComboBoxArticleStock.SelectedItem = Core.Global.GestionStockAterme;
                        break;
                }
            }
        }

        private void ComboBoxArticleStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.LabelInfoGestionStock.Text = "";
            if (this.ComboBoxArticleStock.SelectedItem != null)
            {
                string item = Core.Global.SplitValue(this.ComboBoxArticleStock.SelectedItem.ToString());
                if (Core.Global.IsInteger(item))
                {
                    Int32 stock = Int32.Parse(item);
                    switch (stock)
                    {
                        case 0:
                            this.LabelInfoGestionStock.Text = "Aucun : ce paramètre permet de désactiver l'actualisation des stocks.";
                            break;
                        case 2:
                            this.LabelInfoGestionStock.Text = "Stock réel : ce paramètre ne tient compte ni des achats ni des ventes en cours.";
                            break;
                        case 3:
                            this.LabelInfoGestionStock.Text = "Stock disponible : ce paramètre prends en compte les quantités saisies en préparation de livraison vente ainsi que les quantités à contrôler.";
                            break;
                        case 4:
                            this.LabelInfoGestionStock.Text = "Stock disponible avancé : ce paramètre prends en compte le stock disponible moins les quantités réservées (commandes vente).";
                            break;
                        case 1:
                        default:
                            this.LabelInfoGestionStock.Text = "Stock à terme : ce paramètre prends en comptes les bons de commandes achats ainsi que les quantités en commande et préparation de livraison vente.";
                            break;
                    }
                }
            }
        }

        private void LoadContremarque()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigArticleContremarque))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleContremarque);
                this.TextBoxArticleContremarque.Text = Config.Con_Value;
            }
        }

        private void LoadComboBoxRupture()
        {
            this.ComboBoxArticleRupture.Items.Clear();
            this.ComboBoxArticleRupture.Items.Add("0 - Refusé");
            this.ComboBoxArticleRupture.Items.Add("1 - Accepté");
            this.ComboBoxArticleRupture.Items.Add("2 - Par défaut");
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigArticleRupture))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleRupture);
                switch (Config.Con_Value)
                {
                    case "0":
                        this.ComboBoxArticleRupture.SelectedItem = "0 - Refusé";
                        break;
                    case "1":
                        this.ComboBoxArticleRupture.SelectedItem = "1 - Accepté";
                        break;
                    case "2":
                        this.ComboBoxArticleRupture.SelectedItem = "2 - Par défaut";
                        break;
                    default:
                        this.ComboBoxArticleRupture.SelectedItem = "2 - Par défaut";
                        break;
                }
            }
        }

        private void LoadComboBoxType()
        {
            this.ComboBoxArticleType.Items.Clear();
            this.ComboBoxArticleType.Items.Add("1 - Poids Brut");
            this.ComboBoxArticleType.Items.Add("2 - Poids Net");
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigArticlePoidsType))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticlePoidsType);
                switch (Config.Con_Value)
                {
                    case "1":
                        this.ComboBoxArticleType.SelectedItem = "1 - Poids Brut";
                        break;
                    case "2":
                        this.ComboBoxArticleType.SelectedItem = "2 - Poids Net";
                        break;
                }
            }
        }

        private void LoadComboBoxUnite()
        {
            this.ComboBoxArticleUnite.Items.Clear();
            this.ComboBoxArticleUnite.Items.Add("0 - Tonne");
            this.ComboBoxArticleUnite.Items.Add("1 - Quintal");
            this.ComboBoxArticleUnite.Items.Add("2 - Kilogramme");
            this.ComboBoxArticleUnite.Items.Add("3 - Gramme");
            this.ComboBoxArticleUnite.Items.Add("4 - Milligramme");
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigArticlePoidsUnite))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticlePoidsUnite);
                switch (Config.Con_Value)
                {
                    case "0":
                        this.ComboBoxArticleUnite.SelectedItem = "0 - Tonne";
                        break;
                    case "1":
                        this.ComboBoxArticleUnite.SelectedItem = "1 - Quintal";
                        break;
                    case "2":
                        this.ComboBoxArticleUnite.SelectedItem = "2 - Kilogramme";
                        break;
                    case "3":
                        this.ComboBoxArticleUnite.SelectedItem = "3 - Gramme";
                        break;
                    case "4":
                        this.ComboBoxArticleUnite.SelectedItem = "4 - Milligramme";
                        break;
                }
            }
        }

        private void LoadComboBoxCategorieTarifaire()
        {
            this.ComboBoxArticleCategorieTarifaire.Items.Clear();
            Model.Sage.P_CATTARIFRepository P_CATTARIFRepository = new Model.Sage.P_CATTARIFRepository();
            List<Model.Sage.P_CATTARIF> ListCatTarif = P_CATTARIFRepository.ListIntituleNotNull();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigArticleCatTarif))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigArticleCatTarif);
            }

            foreach (Model.Sage.P_CATTARIF P_Cattarif in ListCatTarif)
            {
                this.ComboBoxArticleCategorieTarifaire.Items.Add(P_Cattarif.cbMarq + " - " + P_Cattarif.CT_Intitule);
                if (P_Cattarif.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxArticleCategorieTarifaire.SelectedItem = P_Cattarif.cbMarq + " - " + P_Cattarif.CT_Intitule;
                }
            }

        }

        private void LoadComboBoxCategorieComptable()
        {
            this.ComboBoxArticleCategorieComptable.Items.Clear();
            Model.Sage.P_CATCOMPTARepository P_CATCOMPTARepository = new Model.Sage.P_CATCOMPTARepository();
            List<String> String = P_CATCOMPTARepository.ListStringVente();
            String StringNumber = "";
            foreach (String Item in String)
            {
                StringNumber = Core.Global.SplitValue(Item);
                if (Core.Global.IsNumeric(StringNumber))
                {
                    Int32 Number = Convert.ToInt32(StringNumber);
                    if (Number < 10)
                    {
                        StringNumber = StringNumber.Replace("0", "");
                    }
                }
                this.ComboBoxArticleCategorieComptable.Items.Add(StringNumber + " - " + Item.Split('-')[1].ToString().Trim());
                if (StringNumber == Core.Global.GetConfig().ConfigArticleCatComptable.ToString())
                {
                    this.ComboBoxArticleCategorieComptable.SelectedItem = StringNumber + " - " + Item.Split('-')[1].ToString().Trim();
                }
            }
        }

        private void LoadCheckBoxPublieWeb()
        {
            CheckBoxArticleNonPublieWeb.IsChecked = Global.GetConfig().ArticleNonPublieSurLeWeb;
        }

        private void LoadCheckBoxSommeil()
        {
            CheckBoxArticleSommeil.IsChecked = Global.GetConfig().ArticleEnSommeil;
        }

        private void WriteArticle()
        {
            this.WriteComboBoxStock();
            this.WriteContremarque();
            this.WriteComboBoxRupture();
            this.WriteComboBoxType();
            this.WriteComboBoxUnite();
            this.WriteComboBoxCategorieTarifaire();
            this.WriteComboBoxCategorieComptable();
            this.WriteCheckBoxPublieWeb();
            this.WriteCheckBoxSommeil();
        }

        private void WriteComboBoxStock()
        {
            if (this.ComboBoxArticleStock.SelectedItem != null)
            {
                if (this.ComboBoxArticleStock.SelectedItem.ToString() != "")
                {
                    Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                    Model.Local.Config Config = new Model.Local.Config();
                    if (ConfigRepository.ExistName(Core.Global.ConfigArticleStock))
                    {
                        Config = ConfigRepository.ReadName(Core.Global.ConfigArticleStock);
                        Config.Con_Value = Core.Global.SplitValue(this.ComboBoxArticleStock.SelectedItem.ToString());
                        ConfigRepository.Save();
                    }
                    else
                    {
                        Config.Con_Name = Core.Global.ConfigArticleStock;
                        Config.Con_Value = Core.Global.SplitValue(this.ComboBoxArticleStock.SelectedItem.ToString());
                        ConfigRepository.Add(Config);
                    }
                    ConfigRepository = null;
                    Config = null;
                }
            }
        }

        private void WriteContremarque()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigArticleContremarque))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigArticleContremarque);
                Config.Con_Value = this.TextBoxArticleContremarque.Text;
                ConfigRepository.Save();
            }
            else
            {
                Config.Con_Name = Core.Global.ConfigArticleContremarque;
                Config.Con_Value = this.TextBoxArticleContremarque.Text;
                ConfigRepository.Add(Config);
            }
            ConfigRepository = null;
            Config = null;
        }

        private void WriteComboBoxRupture()
        {
            if (this.ComboBoxArticleRupture.SelectedItem != null)
            {
                if (this.ComboBoxArticleRupture.SelectedItem.ToString() != "")
                {
                    Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                    Model.Local.Config Config = new Model.Local.Config();
                    if (ConfigRepository.ExistName(Core.Global.ConfigArticleRupture))
                    {
                        Config = ConfigRepository.ReadName(Core.Global.ConfigArticleRupture);
                        Config.Con_Value = Core.Global.SplitValue(this.ComboBoxArticleRupture.SelectedItem.ToString());
                        ConfigRepository.Save();
                    }
                    else
                    {
                        Config.Con_Name = Core.Global.ConfigArticleRupture;
                        Config.Con_Value = Core.Global.SplitValue(this.ComboBoxArticleRupture.SelectedItem.ToString());
                        ConfigRepository.Add(Config);
                    }
                    ConfigRepository = null;
                    Config = null;
                }
            }
        }

        private void WriteComboBoxType()
        {
            if (this.ComboBoxArticleType.SelectedItem != null && this.ComboBoxArticleType.SelectedItem.ToString() != "")
            {
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                Model.Local.Config Config = new Model.Local.Config();
                if (ConfigRepository.ExistName(Core.Global.ConfigArticlePoidsType))
                {
                    Config = ConfigRepository.ReadName(Core.Global.ConfigArticlePoidsType);
                    Config.Con_Value = Core.Global.SplitValue(this.ComboBoxArticleType.SelectedItem.ToString());
                    ConfigRepository.Save();
                }
                else
                {
                    Config.Con_Name = Core.Global.ConfigArticlePoidsType;
                    Config.Con_Value = Core.Global.SplitValue(this.ComboBoxArticleType.SelectedItem.ToString());
                    ConfigRepository.Add(Config);
                }
                ConfigRepository = null;
                Config = null;
            }
        }

        private void WriteComboBoxUnite()
        {
            if (this.ComboBoxArticleUnite.SelectedItem != null && this.ComboBoxArticleUnite.SelectedItem.ToString() != "")
            {
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                Model.Local.Config Config = new Model.Local.Config();
                if (ConfigRepository.ExistName(Core.Global.ConfigArticlePoidsUnite))
                {
                    Config = ConfigRepository.ReadName(Core.Global.ConfigArticlePoidsUnite);
                    Config.Con_Value = Core.Global.SplitValue(this.ComboBoxArticleUnite.SelectedItem.ToString());
                    ConfigRepository.Save();
                }
                else
                {
                    Config.Con_Name = Core.Global.ConfigArticlePoidsUnite;
                    Config.Con_Value = Core.Global.SplitValue(this.ComboBoxArticleUnite.SelectedItem.ToString());
                    ConfigRepository.Add(Config);
                }
                ConfigRepository = null;
                Config = null;
            }
        }

        private void WriteComboBoxCategorieTarifaire()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();

            if (ConfigRepository.ExistName(Core.Global.ConfigArticleCatTarif))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigArticleCatTarif);
                Config.Con_Value = (this.ComboBoxArticleCategorieTarifaire.SelectedItem != null && this.ComboBoxArticleCategorieTarifaire.SelectedItem.ToString() != "") ? Core.Global.SplitValue(this.ComboBoxArticleCategorieTarifaire.SelectedItem.ToString()) : "0";
                ConfigRepository.Save();
            }
            else
            {
                Config.Con_Name = Core.Global.ConfigArticleCatTarif;
                Config.Con_Value = (this.ComboBoxArticleCategorieTarifaire.SelectedItem != null && this.ComboBoxArticleCategorieTarifaire.SelectedItem.ToString() != "") ? Core.Global.SplitValue(this.ComboBoxArticleCategorieTarifaire.SelectedItem.ToString()) : "0";
                ConfigRepository.Add(Config);
            }
            ConfigRepository = null;
            Config = null;
        }

        private void WriteComboBoxCategorieComptable()
        {
            if (this.ComboBoxArticleCategorieComptable.SelectedItem != null && this.ComboBoxArticleCategorieComptable.SelectedItem.ToString() != "")
            {
                Core.Global.GetConfig().UpdateConfigArticleCatComptable(int.Parse(Core.Global.SplitValue(this.ComboBoxArticleCategorieComptable.SelectedItem.ToString())));
            }
            else
            {
                Core.Global.GetConfig().UpdateConfigArticleCatComptable(0);
            }
        }

        private void WriteCheckBoxPublieWeb()
        {
            Global.GetConfig().UpdateArticleNonPublieSurLeWeb(CheckBoxArticleNonPublieWeb.IsChecked.Value);
        }

        private void WriteCheckBoxSommeil()
        {
            Global.GetConfig().UpdateArticleEnSommeil(CheckBoxArticleSommeil.IsChecked.Value);
        }

        private void ButtonCreateFeatureFromSageInformationLibre_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext.CreateFeatureFromSageInformationLibre();
        }

        private void ButtonCreateFeatureFromSageStatistiqueArticle_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext.CreateFeatureFromSageStatistiqueArticle();
        }

        private void ButtonCreateFeatureFromSageInformationArticle_Click(object sender, RoutedEventArgs e)
        {
            DataContext.CreateFeatureFromSageInformationArticle();
        }

        private void ButtonCreateCustomerFeatureFromSageStatistiqueClient_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext.CreateCustomerFeatureFromSageStatistiqueClient();
        }

        private void ButtonCreateCustomerFeatureFromSageInformationLibreClient_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext.CreateCustomerFeatureFromSageInformationLibreClient();
        }

        private void ComboBoxArticleCategorieComptable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboBoxArticleCategorieComptable.SelectedItem != null && DataContext.SelectedLigneRemiseMode != null)
                this.ComboBoxTaxSageArticleRemise.IsEnabled = DataContext.SelectedLigneRemiseMode._LigneRemiseMode == Core.Parametres.LigneRemiseMode.LigneRemise;
            else
                this.ComboBoxTaxSageArticleRemise.IsEnabled = false;
        }

        private void ButtonDeleteReplacement_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext.DeleteReplacement();
        }

        private void ButtonAddReplacement_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext.AddReplacement();
        }

        private void Bt_Effacer_ArticleCategorieTarifaire_Click(object sender, RoutedEventArgs e)
        {
            this.ComboBoxArticleCategorieTarifaire.SelectedItem = null;
        }

        private void Bt_Effacer_ArticleCategorieComptable_Click(object sender, RoutedEventArgs e)
        {
            this.ComboBoxArticleCategorieComptable.SelectedItem = null;
        }

        #endregion

        #region Client

        private void LoadClient()
        {
            this.LoadRadioClientTypeLien();
            this.LoadComboBoxClientCompteGeneral();
            this.LoadComboBoxClientCompteComptable();
            this.LoadComboBoxClientCategorieTarifaire();
            this.LoadComboBoxClientConditionLivraison();
            this.LoadComboBoxClientModeExpedition();
            this.LoadComboBoxClientPeriodicite();
            this.LoadComboBoxClientRisque();
            this.LoadComboBoxClientSautLigne();
            this.LoadTextBoxClientNombreLigne();
            this.LoadComboBoxClientCollaborateur();
            this.LoadComboBoxClientDepot();
            this.LoadComboBoxClientAffaire();
            this.LoadComboBoxClientDevise();
            this.LoadTextBoxClientPrioriteLivraison();
            this.LoadCheckBoxClientLivraisonPartielle();
            this.LoadCheckBoxClientBLFacture();
            this.LoadCheckBoxClientLettrageAutomatique();
            this.LoadCheckBoxClientValidationAutomatique();
            this.LoadCheckBoxClientRappel();
            this.LoadCheckBoxClientPenalite();
            this.LoadCheckBoxClientSurveillance();

            // <JG> 07/09/2012
            this.LoadRadioClientIntituleAdresse();
            // <JG> 10/09/2012
            this.LoadPasswordBoxClientPrestashopCookieKey();

            // <JG> 14/02/2013
            this.LoadConfigTransfert();
        }

        private void LoadRadioClientTypeLien()
        {
            Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
            this.ComboClientCentralisateur.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<Model.Sage.F_COMPTET>(F_COMPTETRepository.ListTypeSommeil((short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_Type.Client, 0));
            this.ComboClientCentralisateur.DisplayMemberPath = Model.Sage.F_COMPTET._Fields.ComboText.ToString();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientTypeLien))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientTypeLien);
                if (Config.Con_Value == Core.Global.ConfigClientTypeLienEnum.CompteCentralisateur.ToString())
                {
                    this.RadioClientCompteCentralisateur.IsChecked = true;
                }
                else
                {
                    this.RadioClientComptesIndividuels.IsChecked = true;
                }
            }
            else
            {
                this.RadioClientComptesIndividuels.IsChecked = true;
            }
            this.LoadComboBoxClientCentralisateur(F_COMPTETRepository);
        }

        private void RadioCompteTypeLien_Checked(object sender, RoutedEventArgs e)
        {
            if (this.GroupBoxOptionsComptesIndividuels != null && this.ComboClientCentralisateur != null)
                if (this.RadioClientCompteCentralisateur.IsChecked == true)
                {
                    this.GroupBoxOptionsComptesIndividuels.IsEnabled = false;
                    this.ComboClientCentralisateur.IsEnabled = true;
                    this.RadioClientAdresseIntituleNomPrenomPrestashop.IsChecked = true;
                    this.RadioClientAdresseIntituleCodePrestashop.IsEnabled = false;
                }
                else
                {
                    this.GroupBoxOptionsComptesIndividuels.IsEnabled = true;
                    this.ComboClientCentralisateur.IsEnabled = false;
                    this.RadioClientAdresseIntituleCodePrestashop.IsEnabled = true;
                    this.LoadRadioClientIntituleAdresse();
                }
        }

        private void LoadComboBoxClientCentralisateur(Model.Sage.F_COMPTETRepository F_COMPTETRepository)
        {
            this.ComboClientCentralisateur.SelectedItem = null;
            if (this.RadioClientCompteCentralisateur != null && this.RadioClientCompteCentralisateur.IsChecked == true)
            {
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                if (ConfigRepository.ExistName(Core.Global.ConfigClientCompteCentralisateur))
                {
                    Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientCompteCentralisateur);
                    if (Core.Global.IsInteger(Config.Con_Value))
                    {
                        Int32 cbMarqCentralisateur = Int32.Parse(Config.Con_Value);
                        if (F_COMPTETRepository.ExistId(cbMarqCentralisateur))
                        {
                            this.ComboClientCentralisateur.SelectedItem = F_COMPTETRepository.Read(cbMarqCentralisateur);
                        }
                    }
                }
            }
        }

        private void LoadComboBoxClientCompteGeneral()
        {
            this.ComboBoxClientCompteGeneral.Items.Clear();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientCompteGeneral))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientCompteGeneral);
            }

            Model.Sage.F_COMPTEGRepository F_COMPTEGRepository = new Model.Sage.F_COMPTEGRepository();
            // <JG> 10/05/2012 mise en place du filtre compte général pour la nature Client
            //List<Model.Sage.F_COMPTEG> ListF_COMPTEG = F_COMPTEGRepository.ListSommeilStartWithCompte(0, "41");
            List<Model.Sage.F_COMPTEG> ListF_COMPTEG = F_COMPTEGRepository.ListSommeilNature(0, ABSTRACTION_SAGE.F_COMPTEG.Obj._Enum_N_Nature.Client);
            foreach (Model.Sage.F_COMPTEG F_COMPTEG in ListF_COMPTEG)
            {
                this.ComboBoxClientCompteGeneral.Items.Add(F_COMPTEG.cbMarq + " - " + F_COMPTEG.CG_Num + " " + F_COMPTEG.CG_Intitule);
                if (F_COMPTEG.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxClientCompteGeneral.SelectedItem = F_COMPTEG.cbMarq + " - " + F_COMPTEG.CG_Num + " " + F_COMPTEG.CG_Intitule;
                }
            }
        }

        private void LoadComboBoxClientCompteComptable()
        {
            this.ComboBoxClientCompteComptable.Items.Clear();
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientCompteComptable))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientCompteComptable);
            }
            Model.Sage.P_CATCOMPTARepository P_CATCOMPTARepository = new Model.Sage.P_CATCOMPTARepository();
            List<String> String = P_CATCOMPTARepository.ListStringVente();
            String StringNumber = "";
            foreach (String Item in String)
            {
                StringNumber = Core.Global.SplitValue(Item);
                if (Core.Global.IsNumeric(StringNumber))
                {
                    Int32 Number = Convert.ToInt32(StringNumber);
                    if (Number < 10)
                    {
                        StringNumber = StringNumber.Replace("0", "");
                    }
                }
                this.ComboBoxClientCompteComptable.Items.Add(StringNumber + " - " + Item.Split('-')[1].ToString().Trim());
                if (StringNumber == Config.Con_Value)
                {
                    this.ComboBoxClientCompteComptable.SelectedItem = StringNumber + " - " + Item.Split('-')[1].ToString().Trim();
                }
            }
        }

        private void LoadComboBoxClientCategorieTarifaire()
        {
            this.ComboBoxClientCategorieTarifaire.Items.Clear();
            Model.Sage.P_CATTARIFRepository P_CATTARIFRepository = new Model.Sage.P_CATTARIFRepository();
            List<Model.Sage.P_CATTARIF> ListCatTarif = P_CATTARIFRepository.ListIntituleNotNull();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientCategorieTarifaire))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientCategorieTarifaire);

            }

            foreach (Model.Sage.P_CATTARIF P_Cattarif in ListCatTarif)
            {
                this.ComboBoxClientCategorieTarifaire.Items.Add(P_Cattarif.cbMarq + " - " + P_Cattarif.CT_Intitule);
                if (P_Cattarif.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxClientCategorieTarifaire.SelectedItem = P_Cattarif.cbMarq + " - " + P_Cattarif.CT_Intitule;
                }
            }
        }

        private void LoadComboBoxClientConditionLivraison()
        {
            this.ComboBoxClientConditionLivraison.Items.Clear();
            Model.Sage.P_CONDLIVRRepository P_CONDLIVRRepository = new Model.Sage.P_CONDLIVRRepository();
            List<Model.Sage.P_CONDLIVR> ListP_CONDLIVR = P_CONDLIVRRepository.ListIntituleNotNull();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientConditionLivraison))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientConditionLivraison);

            }

            foreach (Model.Sage.P_CONDLIVR P_CONDLIVR in ListP_CONDLIVR)
            {
                this.ComboBoxClientConditionLivraison.Items.Add(P_CONDLIVR.cbMarq + " - " + P_CONDLIVR.C_Intitule);
                if (P_CONDLIVR.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxClientConditionLivraison.SelectedItem = P_CONDLIVR.cbMarq + " - " + P_CONDLIVR.C_Intitule;
                }
            }
        }

        private void LoadComboBoxClientModeExpedition()
        {
            this.ComboBoxClientModeExpedition.Items.Clear();
            Model.Sage.P_EXPEDITIONRepository P_EXPEDITIONRepository = new Model.Sage.P_EXPEDITIONRepository();
            List<Model.Sage.P_EXPEDITION> ListP_EXPEDITION = P_EXPEDITIONRepository.ListIntituleNotNull();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientModeExpedition))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientModeExpedition);

            }

            foreach (Model.Sage.P_EXPEDITION P_EXPEDITION in ListP_EXPEDITION)
            {
                this.ComboBoxClientModeExpedition.Items.Add(P_EXPEDITION.cbMarq + " - " + P_EXPEDITION.E_Intitule);
                if (P_EXPEDITION.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxClientModeExpedition.SelectedItem = P_EXPEDITION.cbMarq + " - " + P_EXPEDITION.E_Intitule;
                }
            }
        }

        private void LoadComboBoxClientPeriodicite()
        {
            this.ComboBoxClientPeriodicite.Items.Clear();
            Model.Sage.P_PERIODRepository P_PERIODRepository = new Model.Sage.P_PERIODRepository();
            List<Model.Sage.P_PERIOD> ListP_PERIOD = P_PERIODRepository.ListIntituleNotNull();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientPeriodicite))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientPeriodicite);

            }

            foreach (Model.Sage.P_PERIOD P_PERIOD in ListP_PERIOD)
            {
                this.ComboBoxClientPeriodicite.Items.Add(P_PERIOD.cbMarq + " - " + P_PERIOD.P_Period1);
                if (P_PERIOD.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxClientPeriodicite.SelectedItem = P_PERIOD.cbMarq + " - " + P_PERIOD.P_Period1;
                }
            }
        }

        private void LoadComboBoxClientRisque()
        {
            this.ComboBoxClientCodeRisque.Items.Clear();
            Model.Sage.P_CRISQUERepository P_CRISQUERepository = new Model.Sage.P_CRISQUERepository();
            List<Model.Sage.P_CRISQUE> ListP_CRISQUE = P_CRISQUERepository.ListIntituleNotNull();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientCodeRisque))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientCodeRisque);

            }

            foreach (Model.Sage.P_CRISQUE P_CRISQUE in ListP_CRISQUE)
            {
                this.ComboBoxClientCodeRisque.Items.Add(P_CRISQUE.cbMarq + " - " + P_CRISQUE.R_Intitule);
                if (P_CRISQUE.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxClientCodeRisque.SelectedItem = P_CRISQUE.cbMarq + " - " + P_CRISQUE.R_Intitule;
                }
            }
        }

        private void LoadComboBoxClientSautLigne()
        {
            this.ComboBoxClientSautLigne.Items.Clear();
            this.ComboBoxClientSautLigne.Items.Add("1 - Saut de page");
            this.ComboBoxClientSautLigne.Items.Add("2 - Saut de ligne");
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientSaut))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientSaut);
                switch (Config.Con_Value)
                {
                    case "1":
                        this.ComboBoxClientSautLigne.SelectedItem = "1 - Saut de page";
                        break;
                    case "2":
                        this.ComboBoxClientSautLigne.SelectedItem = "2 - Saut de ligne";
                        break;
                }

            }
        }

        private void ComboBoxClientSautLigne_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboBoxClientSautLigne.SelectedItem != null)
            {
                if (this.ComboBoxClientSautLigne.SelectedItem.ToString() == "1 - Saut de page")
                {
                    this.TextBoxClientNombreLigne.Text = "0";
                }
            }
        }

        private void TextBoxClientNombreLigne_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.TextBoxClientNombreLigne.Text != null)
            {
                if (Core.Global.IsNumeric(this.TextBoxClientNombreLigne.Text) == false)
                {
                    this.TextBoxClientNombreLigne.Text = "0";
                }
                else if (Convert.ToInt32(this.TextBoxClientNombreLigne.Text) > 0)
                {
                    this.ComboBoxClientSautLigne.SelectedItem = "2 - Saut de ligne";
                }
                else
                {
                    this.ComboBoxClientSautLigne.SelectedItem = "1 - Saut de page";
                }
            }
        }

        private void LoadTextBoxClientNombreLigne()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientNombreLigne))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientNombreLigne);
                this.TextBoxClientNombreLigne.Text = Config.Con_Value;
            }
        }

        private void LoadComboBoxClientCollaborateur()
        {
            this.ComboBoxClientCollaborateur.Items.Clear();
            Model.Sage.F_COLLABORATEURRepository F_COLLABORATEURRepository = new Model.Sage.F_COLLABORATEURRepository();
            List<Model.Sage.F_COLLABORATEUR> ListF_COLLABORATEUR = F_COLLABORATEURRepository.List();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientCollaborateur))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientCollaborateur);
            }

            this.ComboBoxClientCollaborateur.Items.Add("0 - Aucun");
            if (Config.Con_Value == "0")
            {
                this.ComboBoxClientCollaborateur.SelectedItem = "0 - Aucun";
            }
            foreach (Model.Sage.F_COLLABORATEUR F_COLLABORATEUR in ListF_COLLABORATEUR)
            {
                if (F_COLLABORATEUR.CO_Vendeur != null && F_COLLABORATEUR.CO_Vendeur == (short)ABSTRACTION_SAGE.F_COLLABORATEUR.Obj._Enum_Boolean.Oui)
                {
                    this.ComboBoxClientCollaborateur.Items.Add(F_COLLABORATEUR.cbMarq + " - " + F_COLLABORATEUR.CO_Nom + " - " + F_COLLABORATEUR.CO_Prenom);
                    if (F_COLLABORATEUR.cbMarq.ToString() == Config.Con_Value)
                    {
                        this.ComboBoxClientCollaborateur.SelectedItem = F_COLLABORATEUR.cbMarq + " - " + F_COLLABORATEUR.CO_Nom + " - " + F_COLLABORATEUR.CO_Prenom;
                    }
                }
            }
        }

        private void LoadComboBoxClientDepot()
        {
            this.ComboBoxClientDepot.Items.Clear();
            Model.Sage.F_DEPOTRepository F_DEPOTRepository = new Model.Sage.F_DEPOTRepository();
            List<Model.Sage.F_DEPOT> ListF_DEPOT = F_DEPOTRepository.List();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientDepot))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientDepot);

            }

            foreach (Model.Sage.F_DEPOT F_DEPOT in ListF_DEPOT)
            {
                this.ComboBoxClientDepot.Items.Add(F_DEPOT.cbMarq + " - " + F_DEPOT.DE_Intitule);
                if (F_DEPOT.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxClientDepot.SelectedItem = F_DEPOT.cbMarq + " - " + F_DEPOT.DE_Intitule;
                }
            }
        }

        private void LoadComboBoxClientAffaire()
        {
            this.ComboBoxClientCodeAffaire.Items.Clear();
            Model.Sage.F_COMPTEARepository F_COMPTEARepository = new Model.Sage.F_COMPTEARepository();
            List<Model.Sage.F_COMPTEA> ListF_COMPTEA = F_COMPTEARepository.ListSommeil(0);

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientCodeAffaire))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientCodeAffaire);

            }

            this.ComboBoxClientCodeAffaire.Items.Add("0 - Aucun");
            if (Config.Con_Value == "0")
            {
                this.ComboBoxClientCodeAffaire.SelectedItem = "0 - Aucun";
            }
            foreach (Model.Sage.F_COMPTEA F_COMPTEA in ListF_COMPTEA)
            {
                this.ComboBoxClientCodeAffaire.Items.Add(F_COMPTEA.cbMarq + " - " + F_COMPTEA.CA_Num);
                if (F_COMPTEA.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxClientCodeAffaire.SelectedItem = F_COMPTEA.cbMarq + " - " + F_COMPTEA.CA_Num;
                }
            }
        }

        private void LoadComboBoxClientDevise()
        {
            this.ComboBoxClientDevise.Items.Clear();
            Model.Sage.P_DEVISERepository P_DEVISERepository = new Model.Sage.P_DEVISERepository();
            List<Model.Sage.P_DEVISE> ListP_DEVISE = P_DEVISERepository.ListIntituleNotNull();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientDevise))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientDevise);
            }

            this.ComboBoxClientDevise.Items.Add("0 - Aucun");
            if (Config.Con_Value == "0")
            {
                this.ComboBoxClientDevise.SelectedItem = "0 - Aucun";
            }
            foreach (Model.Sage.P_DEVISE P_DEVISE in ListP_DEVISE)
            {
                this.ComboBoxClientDevise.Items.Add(P_DEVISE.cbMarq + " - " + P_DEVISE.D_Intitule);
                if (P_DEVISE.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxClientDevise.SelectedItem = P_DEVISE.cbMarq + " - " + P_DEVISE.D_Intitule;
                }
            }
        }

        private void TextBoxClientPrioriteLivraison_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Core.Global.IsNumeric(this.TextBoxClientPrioriteLivraison.Text) == false)
            {
                this.TextBoxClientPrioriteLivraison.Text = "0";
            }
        }

        private void LoadTextBoxClientPrioriteLivraison()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientPrioriteLivraison))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientPrioriteLivraison);
                this.TextBoxClientPrioriteLivraison.Text = Config.Con_Value;
            }
        }

        private void LoadCheckBoxClientLivraisonPartielle()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientLivraisonPartielle))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientLivraisonPartielle);
                if (Config.Con_Value == "True")
                {
                    this.CheckBoxClientLivraisonPartielle.IsChecked = true;
                }
            }
        }

        private void LoadCheckBoxClientBLFacture()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientBLFacture))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientBLFacture);
                if (Config.Con_Value == "True")
                {
                    this.CheckBoxClientBLFacture.IsChecked = true;
                }
            }
        }

        private void LoadCheckBoxClientLettrageAutomatique()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientLettrage))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientLettrage);
                if (Config.Con_Value == "True")
                {
                    this.CheckBoxClientLettrageAutomatique.IsChecked = true;
                }
            }
        }

        private void LoadCheckBoxClientValidationAutomatique()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientValidationAutomatique))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientValidationAutomatique);
                if (Config.Con_Value == "True")
                {
                    this.CheckBoxClientValidationReglement.IsChecked = true;
                }
            }
        }

        private void LoadCheckBoxClientRappel()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientRappel))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientRappel);
                if (Config.Con_Value == "True")
                {
                    this.CheckBoxClientRappel.IsChecked = true;
                }
            }
        }

        private void LoadCheckBoxClientPenalite()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientPenalite))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientPenalite);
                if (Config.Con_Value == "True")
                {
                    this.CheckBoxClientPenalite.IsChecked = true;
                }
            }
        }

        private void LoadCheckBoxClientSurveillance()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientSurveillance))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientSurveillance);
                if (Config.Con_Value == "True")
                {
                    this.CheckBoxClientSurveillance.IsChecked = true;
                }
            }
        }

        // <JG> 07/09/2012
        private void LoadRadioClientIntituleAdresse()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientIntituleAdresse))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientIntituleAdresse);
                if (Config.Con_Value == Core.Global.ConfigClientIntituleAdresseEnum.NomPrenomPrestashop.ToString())
                {
                    this.RadioClientAdresseIntituleNomPrenomPrestashop.IsChecked = true;
                }
                else
                {
                    this.RadioClientAdresseIntituleCodePrestashop.IsChecked = true;
                }
            }
        }

        // <JG> 07/09/2012
        private void LoadPasswordBoxClientPrestashopCookieKey()
        {
            //Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            //if (ConfigRepository.ExistName(Core.Global.ConfigClientPrestashopCookieKey))
            //{
            //    Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigClientPrestashopCookieKey);
            this.PasswordBoxPrestashopCookieKey.Password = Core.Global.GetConfig().TransfertPrestashopCookieKey;
            //}
        }

        // <JG> 14/02/2013
        private void LoadConfigTransfert()
        {
            Core.AppConfig config = Core.Global.GetConfig();

            this.TextBoxMailAdminSite.Text = config.AdminMailAddress;

            this.checkBox_MailAccountAlternative.IsChecked = config.TransfertMailAccountAlternative;
            this.checkBox_RandomPasswordIncludeSpecialCharacters.IsChecked = config.TransfertRandomPasswordIncludeSpecialCharacters;

            this.checkBox_MailCopyAdmin.IsChecked = config.TransfertNotifyAccountAdminCopy;
            this.radioButton_MailPerAddress.IsChecked = config.TransfertNotifyAccountDeliveryMethod == Core.Parametres.DeliveryMethod.Independent;

            this.textBox_PhoneFictionValue.Text = config.TransfertLockPhoneNumberReplaceEntryValue;

            this.checkBox_AccountActivation.IsChecked = config.TransfertAccountActivation;

            this.checkBox_NewsLetter.IsChecked = config.TransfertNewsLetterSuscribe;
            this.checkBox_OptIn.IsChecked = config.TransfertOptInSuscribe;

            this.checkBox_GenerateAccountFile.IsChecked = config.TransfertGenerateAccountFile;

            this.checkBox_IntegrationSynchroClient.IsChecked = config.TransfertIntegrateCustomerSynchronizationProcess;
            this.checkBox_RapportTransfert.IsChecked = config.TransfertSendAdminResultReport;
        }

        private void WriteClient()
        {
            this.WriteClientTypeLien();

            this.WriteClientCompteGeneral();
            this.WriteClientCompteComptable();
            this.WriteClientCategorieTarifaire();
            this.WriteClientConditionLivraison();
            this.WriteClientModeExpedition();
            this.WriteClientPeriodicite();
            this.WriteClientCodeRisque();
            this.WriteClientSautLigne();
            this.WriteClientNombreLigne();
            this.WriteClientCollaborateur();
            this.WriteClientDepot();
            this.WriteClientAffaire();
            this.WriteClientDevise();
            this.WriteClientPrioriteLivraison();
            this.WriteClientLivraisonPartielle();
            this.WriteClientBLFacture();
            this.WriteClientLettrageAutomatique();
            this.WriteClientValidationAutomatique();
            this.WriteClientRappel();
            this.WriteClientPenalite();
            this.WriteClientSurveillance();

            // <JG> 07/09/2012
            this.WriteClientIntituleAdresse();
            // <JG> 10/09/2012
            this.WriteClientPrestashopCookieKey();

            // <JG> 14/02/2013
            this.WriteConfigTransfert();
        }

        // <JG> 08/11/2012
        private void WriteClientTypeLien()
        {
            String value = "";
            Boolean IsValid = true, IsCentralisateur = false;
            if (this.RadioClientCompteCentralisateur.IsChecked == true)
            {
                value = Core.Global.ConfigClientTypeLienEnum.CompteCentralisateur.ToString();
            }
            else
            {
                value = Core.Global.ConfigClientTypeLienEnum.ComptesIndividuels.ToString();
            }
            if (value == Core.Global.ConfigClientTypeLienEnum.CompteCentralisateur.ToString())
            {
                IsValid = this.ComboClientCentralisateur.SelectedItem != null && (this.RadioButtonBToB == null || this.RadioButtonBToB.IsChecked != true);
                IsCentralisateur = IsValid;
            }
            if (IsValid)
            {
                Boolean isConfig = false;
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                Model.Local.Config Config = new Model.Local.Config();
                if (ConfigRepository.ExistName(Core.Global.ConfigClientTypeLien))
                {
                    Config = ConfigRepository.ReadName(Core.Global.ConfigClientTypeLien);
                    isConfig = true;
                }
                Config.Con_Value = value;
                if (isConfig == true)
                {
                    ConfigRepository.Save();
                }
                else
                {
                    Config.Con_Name = Core.Global.ConfigClientTypeLien;
                    ConfigRepository.Add(Config);
                }
                // enregistrement du cbMarq du compte centralisateur
                if (IsCentralisateur)
                {
                    this.WriteClientCompteCentralisateur();
                }
            }
            else
            {
                if (this.RadioButtonBToB != null && this.RadioButtonBToB.IsChecked == true)
                {
                    MessageBox.Show("L'option de centralisation des commandes vers un seul compte client n'est pas compatible avec le mode de gestion BtoB", "BtoB incompatible", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.RadioClientComptesIndividuels.IsChecked = true;
                }
                else
                {
                    MessageBox.Show("Pour activer la centralisation des commandes vous devez sélectionner un compte client centralisateur", "Sélection non renseignée", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        }

        private void WriteClientCompteCentralisateur()
        {
            Model.Sage.F_COMPTET F_COMPTET = (Model.Sage.F_COMPTET)this.ComboClientCentralisateur.SelectedItem;
            Boolean isConfig = false;
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientCompteCentralisateur))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientCompteCentralisateur);
                isConfig = true;
            }
            Config.Con_Value = F_COMPTET.cbMarq.ToString();
            if (isConfig == true)
            {
                ConfigRepository.Save();
            }
            else
            {
                Config.Con_Name = Core.Global.ConfigClientCompteCentralisateur;
                ConfigRepository.Add(Config);
            }
        }

        private void WriteClientTextBox(TextBox TextBox, String Name)
        {
            Boolean isConfig = false;
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Name))
            {
                Config = ConfigRepository.ReadName(Name);
                isConfig = true;
            }
            Config.Con_Value = TextBox.Text;
            if (isConfig == true)
            {
                ConfigRepository.Save();
            }
            else
            {
                Config.Con_Name = Name;
                ConfigRepository.Add(Config);
            }
        }

        private void WriteClientComboBox(ComboBox ComboBox, String Name)
        {
            if (ComboBox.SelectedItem != null)
            {
                if (ComboBox.SelectedItem.ToString() != "")
                {
                    Boolean isConfig = false;
                    Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                    Model.Local.Config Config = new Model.Local.Config();
                    if (ConfigRepository.ExistName(Name))
                    {
                        Config = ConfigRepository.ReadName(Name);
                        isConfig = true;
                    }
                    Config.Con_Value = Core.Global.SplitValue(ComboBox.SelectedItem.ToString());
                    if (isConfig == true)
                    {
                        ConfigRepository.Save();
                    }
                    else
                    {
                        Config.Con_Name = Name;
                        ConfigRepository.Add(Config);
                    }
                }
            }
        }

        private void WriteClientCheckBox(CheckBox CheckBox, String Name)
        {
            Boolean isConfig = false;
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Name))
            {
                Config = ConfigRepository.ReadName(Name);
                isConfig = true;
            }
            Config.Con_Value = CheckBox.IsChecked.ToString();
            if (isConfig == true)
            {
                ConfigRepository.Save();
            }
            else
            {
                Config.Con_Name = Name;
                ConfigRepository.Add(Config);
            }
        }

        private void WriteClientCompteGeneral()
        {
            this.WriteClientComboBox(this.ComboBoxClientCompteGeneral, Core.Global.ConfigClientCompteGeneral);
        }

        private void WriteClientCompteComptable()
        {
            this.WriteClientComboBox(this.ComboBoxClientCompteComptable, Core.Global.ConfigClientCompteComptable);
        }

        private void WriteClientCategorieTarifaire()
        {
            this.WriteClientComboBox(this.ComboBoxClientCategorieTarifaire, Core.Global.ConfigClientCategorieTarifaire);
        }

        private void WriteClientConditionLivraison()
        {
            this.WriteClientComboBox(this.ComboBoxClientConditionLivraison, Core.Global.ConfigClientConditionLivraison);
        }

        private void WriteClientModeExpedition()
        {
            this.WriteClientComboBox(this.ComboBoxClientModeExpedition, Core.Global.ConfigClientModeExpedition);
        }

        private void WriteClientPeriodicite()
        {
            this.WriteClientComboBox(this.ComboBoxClientPeriodicite, Core.Global.ConfigClientPeriodicite);
        }

        private void WriteClientCodeRisque()
        {
            this.WriteClientComboBox(this.ComboBoxClientCodeRisque, Core.Global.ConfigClientCodeRisque);
        }

        private void WriteClientSautLigne()
        {
            this.WriteClientComboBox(this.ComboBoxClientSautLigne, Core.Global.ConfigClientSaut);
        }

        private void WriteClientCollaborateur()
        {
            this.WriteClientComboBox(this.ComboBoxClientCollaborateur, Core.Global.ConfigClientCollaborateur);
        }

        private void WriteClientDepot()
        {
            this.WriteClientComboBox(this.ComboBoxClientDepot, Core.Global.ConfigClientDepot);
        }

        private void WriteClientAffaire()
        {
            this.WriteClientComboBox(this.ComboBoxClientCodeAffaire, Core.Global.ConfigClientCodeAffaire);
        }

        private void WriteClientDevise()
        {
            this.WriteClientComboBox(this.ComboBoxClientDevise, Core.Global.ConfigClientDevise);
        }

        private void WriteClientPrioriteLivraison()
        {
            this.WriteClientTextBox(this.TextBoxClientPrioriteLivraison, Core.Global.ConfigClientPrioriteLivraison);
        }

        private void WriteClientNombreLigne()
        {
            this.WriteClientTextBox(this.TextBoxClientNombreLigne, Core.Global.ConfigClientNombreLigne);
        }

        private void WriteClientLivraisonPartielle()
        {
            this.WriteClientCheckBox(this.CheckBoxClientLivraisonPartielle, Core.Global.ConfigClientLivraisonPartielle);
        }

        private void WriteClientBLFacture()
        {
            this.WriteClientCheckBox(this.CheckBoxClientBLFacture, Core.Global.ConfigClientBLFacture);
        }

        private void WriteClientLettrageAutomatique()
        {
            this.WriteClientCheckBox(this.CheckBoxClientLettrageAutomatique, Core.Global.ConfigClientLettrage);
        }

        private void WriteClientValidationAutomatique()
        {
            this.WriteClientCheckBox(this.CheckBoxClientValidationReglement, Core.Global.ConfigClientValidationAutomatique);
        }

        private void WriteClientRappel()
        {
            this.WriteClientCheckBox(this.CheckBoxClientRappel, Core.Global.ConfigClientRappel);
        }

        private void WriteClientPenalite()
        {
            this.WriteClientCheckBox(this.CheckBoxClientPenalite, Core.Global.ConfigClientPenalite);
        }

        private void WriteClientSurveillance()
        {
            this.WriteClientCheckBox(this.CheckBoxClientSurveillance, Core.Global.ConfigClientSurveillance);
        }

        // <JG> 07/09/2012
        private void WriteClientIntituleAdresse()
        {
            String value = (this.RadioClientAdresseIntituleNomPrenomPrestashop.IsChecked == true)
                ? Core.Global.ConfigClientIntituleAdresseEnum.NomPrenomPrestashop.ToString()
                : Core.Global.ConfigClientIntituleAdresseEnum.CodePrestashop.ToString();
            Boolean isConfig = false;
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigClientIntituleAdresse))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigClientIntituleAdresse);
                isConfig = true;
            }
            Config.Con_Value = value;
            if (isConfig == true)
            {
                ConfigRepository.Save();
            }
            else
            {
                Config.Con_Name = Core.Global.ConfigClientIntituleAdresse;
                ConfigRepository.Add(Config);
            }
        }

        // <JG> 07/09/2012
        private void WriteClientPrestashopCookieKey()
        {
            //Boolean isConfig = false;
            //Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            //Model.Local.Config Config = new Model.Local.Config();
            //if (ConfigRepository.ExistName(Core.Global.ConfigClientPrestashopCookieKey))
            //{
            //    Config = ConfigRepository.ReadName(Core.Global.ConfigClientPrestashopCookieKey);
            //    isConfig = true;
            //}
            //Config.Con_Value = this.PasswordBoxPrestashopCookieKey.Password;
            //if (isConfig == true)
            //{
            //    ConfigRepository.Save();
            //}
            //else
            //{
            //    Config.Con_Name = Core.Global.ConfigClientPrestashopCookieKey;
            //    ConfigRepository.Add(Config);
            //}
            Core.Global.GetConfig().UpdateTransfertPrestashopCookieKey(this.PasswordBoxPrestashopCookieKey.Password.Trim());
        }

        // <JG> 14/02/2013
        private void WriteConfigTransfert()
        {
            Core.AppConfig config = Core.Global.GetConfig();

            config.UpdateAdminMailAddress(this.TextBoxMailAdminSite.Text.Trim());

            config.UpdateTransfertMailAccountAlternative(this.checkBox_MailAccountAlternative.IsChecked == true);
            config.UpdateTransfertRandomPasswordIncludeSpecialCharacters(this.checkBox_RandomPasswordIncludeSpecialCharacters.IsChecked == true);

            config.UpdateTransfertNotifyAccountAdminCopy(this.checkBox_MailCopyAdmin.IsChecked == true);
            config.UpdateTransfertNotifyAccountDeliveryMethod((this.radioButton_MailPerAddress.IsChecked == true) ? Core.Parametres.DeliveryMethod.Independent : Core.Parametres.DeliveryMethod.Copy);

            config.UpdateTransfertLockPhoneNumberReplaceEntryValue(this.textBox_PhoneFictionValue.Text);

            config.UpdateTransfertAccountActivation(this.checkBox_AccountActivation.IsChecked == true);
            config.UpdateTransfertNewsLetterSuscribe(this.checkBox_NewsLetter.IsChecked == true);
            config.UpdateTransfertOptInSuscribe(this.checkBox_OptIn.IsChecked == true);
            config.UpdateTransfertGenerateAccountFile(this.checkBox_GenerateAccountFile.IsChecked == true);

            config.UpdateTransfertIntegrateCustomerSynchronizationProcess(this.checkBox_IntegrationSynchroClient.IsChecked == true);
            config.UpdateTransfertSendAdminResultReport(this.checkBox_RapportTransfert.IsChecked == true);
        }

        private void ButtonConfigurationCatCompta_Click(object sender, RoutedEventArgs e)
        {
            View.Config.ConfigurationCatCompta ConfigCatCompta = new View.Config.ConfigurationCatCompta();
            ConfigCatCompta.ShowDialog();
        }

        #endregion

        #region Taxe

        private void LoadTaxe()
        {
            this.ComboBoxTaxSageArticleRemise.ItemsSource = new List<Model.Sage.F_ARTICLE>();
            this.ComboBoxTaxSageArticleRemplacement.ItemsSource = new List<Model.Sage.F_ARTICLE>();
            this.ComboBoxTaxPrestashop.Items.Clear();
            Model.Prestashop.PsLangRepository PsLangRepository = new Model.Prestashop.PsLangRepository();
            Model.Prestashop.PsLang PsLang = PsLangRepository.ReadId(Core.Global.Lang);
            Model.Prestashop.PsCountryRepository PsCountryRepository = new Model.Prestashop.PsCountryRepository();

            if (PsCountryRepository.ExistIsoCode(PsLang.IsoCode))
            {
                Model.Local.TaxRepository LocalTaxRepository = new Model.Local.TaxRepository();
                List<Model.Local.Tax> taxes = LocalTaxRepository.ListOrderBySagName();

                this.DataGridTaxe.ItemsSource = taxes;

                Model.Prestashop.PsCountry PsCountry = PsCountryRepository.ReadIsoCode(PsLang.IsoCode);
                Model.Prestashop.PsTaxRulesGroupRepository PsTaxRulesGroupRepository = new Model.Prestashop.PsTaxRulesGroupRepository();
                List<Model.Prestashop.PsTaxRulesGroup> ListTaxRulesGroup = PsTaxRulesGroupRepository.ListActive(1);
                Model.Prestashop.PsTaxRuleRepository PsTaxRuleRepository = new Model.Prestashop.PsTaxRuleRepository();
                Model.Prestashop.PsTaxRule PsTaxRule;

                foreach (Model.Prestashop.PsTaxRulesGroup TaxRulesGroup in ListTaxRulesGroup)
                    if (taxes.Count(result => result.Pre_Id == TaxRulesGroup.IDTaxRulesGroup) == 0)
                    {
                        if (PsTaxRuleRepository.ExistTaxeRulesGroupCountry((Int32)TaxRulesGroup.IDTaxRulesGroup, (Int32)PsCountry.IDCountry))
                        {
                            PsTaxRule = PsTaxRuleRepository.ReadTaxesRulesGroupCountry((Int32)TaxRulesGroup.IDTaxRulesGroup, (Int32)PsCountry.IDCountry);
                            this.ComboBoxTaxPrestashop.Items.Add(TaxRulesGroup.IDTaxRulesGroup + " - " + TaxRulesGroup.Name);
                        }
                    }

                this.ComboBoxTaxSage.Items.Clear();

                Model.Sage.F_TAXERepository F_TAXERepository = new Model.Sage.F_TAXERepository();
                List<Model.Sage.F_TAXE> ListTaxe = F_TAXERepository.ListTTauxSens(ABSTRACTION_SAGE.F_TAXE.Obj._Enum_TA_TTaux.Taux, 1);

                foreach (Model.Sage.F_TAXE Taxe in ListTaxe)
                    if (taxes.Count(result => result.Sag_Id == Taxe.cbMarq) == 0)
                        this.ComboBoxTaxSage.Items.Add(Taxe.cbMarq + " - " + Taxe.TA_Intitule);

                ComboBoxTaxPrestashop.IsEnabled = (ComboBoxTaxPrestashop.Items.Count > 0);
                ComboBoxTaxSage.IsEnabled = (ComboBoxTaxSage.Items.Count > 0);
            }
        }

        private void ComboBoxTaxSage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonAssocierTaxes.IsEnabled = (ComboBoxTaxPrestashop.SelectedItem != null && ComboBoxTaxSage.SelectedItem != null);

            if (ComboBoxTaxSage.SelectedItem != null && this.ComboBoxTaxSageArticleRemise.IsEnabled && ComboBoxArticleCategorieComptable.SelectedItem != null)
            {
                this.ComboBoxTaxSageArticleRemise.ItemsSource = new Model.Sage.F_ARTICLERepository()
                    .ListCatCompta(Convert.ToInt32(Core.Global.SplitValue(this.ComboBoxArticleCategorieComptable.SelectedItem.ToString())),
                    0,
                    new Model.Sage.F_TAXERepository().Read(Convert.ToInt32(Core.Global.SplitValueArray(this.ComboBoxTaxSage.SelectedItem.ToString())[0].Replace(" ", ""))).TA_Code);
            }
            if (ComboBoxTaxSage.SelectedItem != null && ComboBoxArticleCategorieComptable.SelectedItem != null)
            {
                this.ComboBoxTaxSageArticleRemplacement.ItemsSource = new Model.Sage.F_ARTICLERepository()
                    .ListCatCompta(Convert.ToInt32(Core.Global.SplitValue(this.ComboBoxArticleCategorieComptable.SelectedItem.ToString())),
                    0,
                    new Model.Sage.F_TAXERepository().Read(Convert.ToInt32(Core.Global.SplitValueArray(this.ComboBoxTaxSage.SelectedItem.ToString())[0].Replace(" ", ""))).TA_Code);
            }
        }

        private void ComboBoxTaxPrestashop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonAssocierTaxes.IsEnabled = (ComboBoxTaxPrestashop.SelectedItem != null && ComboBoxTaxSage.SelectedItem != null);
        }

        private void AssocierTaxes_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxTaxPrestashop.SelectedItem != null && ComboBoxTaxSage.SelectedItem != null)
            {
                String[] StringSage = Core.Global.SplitValueArray(this.ComboBoxTaxSage.SelectedItem.ToString());
                String[] StringPrestashop = Core.Global.SplitValueArray(this.ComboBoxTaxPrestashop.SelectedItem.ToString());

                if (Core.Global.IsNumeric(StringPrestashop[0].Replace(" ", ""))
                    && Core.Global.IsNumeric(StringSage[0].Replace(" ", "")))
                {
                    Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
                    Model.Local.Tax Tax = new Model.Local.Tax();
                    if (TaxRepository.ExistSage(Convert.ToInt32(StringSage[0].Replace(" ", ""))))
                    {
                        Tax = TaxRepository.ReadSage(Convert.ToInt32(StringSage[0].Replace(" ", "")));
                        TaxRepository.Delete(Tax);
                        Tax = new Model.Local.Tax();
                    }
                    Tax.Pre_Id = Convert.ToInt32(StringPrestashop[0].Replace(" ", ""));
                    Tax.Sag_Id = Convert.ToInt32(StringSage[0].Replace(" ", ""));
                    Tax.Pre_Name = StringPrestashop[1].Trim();
                    Tax.Sag_Name = StringSage[1].Trim();

                    if (this.ComboBoxTaxSageArticleRemise.SelectedItem != null
                            && (new Model.Sage.F_ARTICLERepository().ExistReference(this.ComboBoxTaxSageArticleRemise.Text)))
                        Tax.Sag_ArticleRemise = this.ComboBoxTaxSageArticleRemise.Text;

                    if (this.ComboBoxTaxSageArticleRemplacement.SelectedItem != null
                            && (new Model.Sage.F_ARTICLERepository().ExistReference(this.ComboBoxTaxSageArticleRemplacement.Text)))
                        Tax.Sag_ArticleRemplacement = this.ComboBoxTaxSageArticleRemplacement.Text;

                    TaxRepository.Add(Tax);
                }
                this.LoadTaxe();
            }

            ButtonAssocierTaxes.IsEnabled = (ComboBoxTaxPrestashop.SelectedItem != null && ComboBoxTaxSage.SelectedItem != null);
        }
        private void DataGridButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Model.Local.Tax Tax = this.DataGridTaxe.SelectedItem as Model.Local.Tax;
            Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
            if (TaxRepository.ExistSagePrestashop(Tax.Sag_Id, Tax.Pre_Id))
            {
                Model.Local.Tax TaxDelete = TaxRepository.ReadSagePrestashop(Tax.Sag_Id, Tax.Pre_Id);
                TaxRepository.Delete(TaxDelete);
                this.LoadTaxe();
            }
        }

        private void listBoxLigneRemiseMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboBoxArticleCategorieComptable.SelectedItem != null && DataContext.SelectedLigneRemiseMode != null)
                this.ComboBoxTaxSageArticleRemise.IsEnabled = DataContext.SelectedLigneRemiseMode._LigneRemiseMode == Core.Parametres.LigneRemiseMode.LigneRemise;
            else
                this.ComboBoxTaxSageArticleRemise.IsEnabled = false;
        }

        #endregion

        #region Commande

        private void LoadCommande()
        {
            this.LoadComboBoxCommandeDepot();
            this.LoadComboBoxCommandeSouche();
            this.LoadComboBoxSageBCStatut();
            this.LoadComboBoxSageDevisStatut();
            this.LoadComboBoxCommandeStatut();
            this.LoadDataGridCommandeStatut();
            LoadPeriodiciteRelancesAndAnnulation();
        }

        private void LoadComboBoxCommandeDepot()
        {
            this.ComboBoxCommandeDepot.Items.Clear();
            Model.Sage.F_DEPOTRepository F_DEPOTRepository = new Model.Sage.F_DEPOTRepository();
            List<Model.Sage.F_DEPOT> ListF_DEPOT = F_DEPOTRepository.List();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigCommandeDepot))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeDepot);
            }

            foreach (Model.Sage.F_DEPOT F_DEPOT in ListF_DEPOT)
            {
                this.ComboBoxCommandeDepot.Items.Add(F_DEPOT.cbMarq + " - " + F_DEPOT.DE_Intitule);
                if (F_DEPOT.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxCommandeDepot.SelectedItem = F_DEPOT.cbMarq + " - " + F_DEPOT.DE_Intitule;
                }
            }
        }

        private void LoadComboBoxCommandeSouche()
        {
            this.ComboBoxCommandeSouche.Items.Clear();
            Model.Sage.P_SOUCHEVENTERepository P_SOUCHEVENTERepository = new Model.Sage.P_SOUCHEVENTERepository();
            List<Model.Sage.P_SOUCHEVENTE> ListP_SOUCHEVENTE = P_SOUCHEVENTERepository.ListIntituleNotNull();

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            if (ConfigRepository.ExistName(Core.Global.ConfigCommandeSouche))
            {
                Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeSouche);
            }

            foreach (Model.Sage.P_SOUCHEVENTE P_SOUCHEVENTE in ListP_SOUCHEVENTE)
            {
                this.ComboBoxCommandeSouche.Items.Add(P_SOUCHEVENTE.cbMarq + " - " + P_SOUCHEVENTE.S_Intitule);
                if (P_SOUCHEVENTE.cbMarq.ToString() == Config.Con_Value)
                {
                    this.ComboBoxCommandeSouche.SelectedItem = P_SOUCHEVENTE.cbMarq + " - " + P_SOUCHEVENTE.S_Intitule;
                }
            }
        }

        private void LoadComboBoxSageBCStatut()
        {
            this.ComboBoxSageBCStatut.Items.Clear();

            Model.Sage.P_ORGAVENRepository P_ORGAVENRepository = new Model.Sage.P_ORGAVENRepository();
            if (P_ORGAVENRepository.Exist(Model.Sage.P_ORGAVENRepository.Doc_Type.BonCommande))
            {
                Model.Sage.P_ORGAVEN P_ORGAVEN = P_ORGAVENRepository.Read(Model.Sage.P_ORGAVENRepository.Doc_Type.BonCommande);
                if (P_ORGAVEN.D_Saisie != null && P_ORGAVEN.D_Saisie == 1)
                    this.ComboBoxSageBCStatut.Items.Add("0 - Saisi");
                if (P_ORGAVEN.D_Confirme != null && P_ORGAVEN.D_Confirme == 1)
                    this.ComboBoxSageBCStatut.Items.Add("1 - Confirmé");
            }
            this.ComboBoxSageBCStatut.Items.Add("2 - À Préparer");
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigCommandeSageBCStatut))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeSageBCStatut);
                switch (Config.Con_Value)
                {
                    case "0":
                        this.ComboBoxSageBCStatut.SelectedItem = "0 - Saisi";
                        break;
                    case "1":
                        this.ComboBoxSageBCStatut.SelectedItem = "1 - Confirmé";
                        break;
                    case "2":
                        this.ComboBoxSageBCStatut.SelectedItem = "2 - À Préparer";
                        break;
                }
            }
        }

        private void LoadComboBoxSageDevisStatut()
        {
            this.ComboBoxSageDevisStatut.Items.Clear();
            Model.Sage.P_ORGAVENRepository P_ORGAVENRepository = new Model.Sage.P_ORGAVENRepository();
            if (P_ORGAVENRepository.Exist(Model.Sage.P_ORGAVENRepository.Doc_Type.Devis))
            {
                Model.Sage.P_ORGAVEN P_ORGAVEN = P_ORGAVENRepository.Read(Model.Sage.P_ORGAVENRepository.Doc_Type.Devis);
                if (P_ORGAVEN.D_Saisie != null && P_ORGAVEN.D_Saisie == 1)
                    this.ComboBoxSageDevisStatut.Items.Add("0 - Saisi");
                if (P_ORGAVEN.D_Confirme != null && P_ORGAVEN.D_Confirme == 1)
                    this.ComboBoxSageDevisStatut.Items.Add("1 - Confirmé");
            }
            this.ComboBoxSageDevisStatut.Items.Add("2 - Accepté");
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.Global.ConfigCommandeSageDevisStatut))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeSageDevisStatut);
                switch (Config.Con_Value)
                {
                    case "0":
                        this.ComboBoxSageDevisStatut.SelectedItem = "0 - Saisi";
                        break;
                    case "1":
                        this.ComboBoxSageDevisStatut.SelectedItem = "1 - Confirmé";
                        break;
                    case "2":
                        this.ComboBoxSageDevisStatut.SelectedItem = "2 - Accepté";
                        break;
                }
            }
        }

        private void LoadComboBoxCommandeStatut()
        {
            //ComboBox.Items.Clear();
            Model.Prestashop.PsOrderStateLangRepository PsOrderStateLangRepository = new Model.Prestashop.PsOrderStateLangRepository();
            List<Model.Prestashop.PsOrderStateLang> ListPsOrderStateLang = PsOrderStateLangRepository.ListLang(Core.Global.Lang);

            ComboBoxCommandeStatutDE.ItemsSource = ListPsOrderStateLang;
            if (ListPsOrderStateLang.Count(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandeDE) == 1)
                ComboBoxCommandeStatutDE.SelectedItem = ListPsOrderStateLang.FirstOrDefault(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandeDE);

            ComboBoxCommandeStatutBC.ItemsSource = ListPsOrderStateLang;
            if (ListPsOrderStateLang.Count(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandeBC) == 1)
                ComboBoxCommandeStatutBC.SelectedItem = ListPsOrderStateLang.FirstOrDefault(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandeBC);

            ComboBoxCommandeStatutPL.ItemsSource = ListPsOrderStateLang;
            if (ListPsOrderStateLang.Count(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandePL) == 1)
                ComboBoxCommandeStatutPL.SelectedItem = ListPsOrderStateLang.FirstOrDefault(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandePL);

            ComboBoxCommandeStatutBL.ItemsSource = ListPsOrderStateLang;
            if (ListPsOrderStateLang.Count(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandeBL) == 1)
                ComboBoxCommandeStatutBL.SelectedItem = ListPsOrderStateLang.FirstOrDefault(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandeBL);

            ComboBoxCommandeStatutFA.ItemsSource = ListPsOrderStateLang;
            if (ListPsOrderStateLang.Count(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandeFA) == 1)
                ComboBoxCommandeStatutFA.SelectedItem = ListPsOrderStateLang.FirstOrDefault(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandeFA);

            ComboBoxCommandeStatutFC.ItemsSource = ListPsOrderStateLang;
            if (ListPsOrderStateLang.Count(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandeFC) == 1)
                ComboBoxCommandeStatutFC.SelectedItem = ListPsOrderStateLang.FirstOrDefault(os => os.IDOrderState == Core.Global.GetConfig().ConfigCommandeFC);
        }

        private void LoadDataGridCommandeStatut()
        {
            Model.Prestashop.PsOrderStateLangRepository PsOrderStateLangRepository = new Model.Prestashop.PsOrderStateLangRepository();
            List<Model.Prestashop.PsOrderStateLang> ListPsOrderStateLang = PsOrderStateLangRepository.ListLang(Core.Global.Lang);
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            List<CommandeStatut> ListCommandeStatut = new List<CommandeStatut>();

            String[] ArrayCreateBCConfig = null;
            String[] ArrayCreateDevisConfig = null;
            String[] ArrayPaymentConfig = null;
            String[] ArrayRelanceConfig = null;
            String[] ArrayAnnulationConfig = null;

            Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeStatutCreateBC);
            if (Config != null)
                ArrayCreateBCConfig = Config.Con_Value.Split('#');

            Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeStatutCreateDevis);
            if (Config != null)
                ArrayCreateDevisConfig = Config.Con_Value.Split('#');

            Config = ConfigRepository.ReadName(Core.Global.ConfigCommandePayment);
            if (Config != null)
                ArrayPaymentConfig = Config.Con_Value.Split('#');

            Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeRelance);
            if (Config != null)
                ArrayRelanceConfig = Config.Con_Value.Split('#');

            Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeAnnulation);
            if (Config != null)
                ArrayAnnulationConfig = Config.Con_Value.Split('#');

            foreach (Model.Prestashop.PsOrderStateLang PsOrderStateLang in ListPsOrderStateLang)
            {
                //Boolean isArraySyncConfig = false;
                //Boolean isArrayPaymentConfig = false;
                //Boolean isArrayRelanceConfig = false;
                //Boolean isArrayAnnulationConfig = false;
                CommandeStatut CommandeStatut = new CommandeStatut();
                CommandeStatut.Id = PsOrderStateLang.IDOrderState;
                CommandeStatut.Statut = PsOrderStateLang.Name;
                CommandeStatut.CreateBCSage = (ArrayCreateBCConfig != null && ArrayCreateBCConfig.Contains(PsOrderStateLang.IDOrderState.ToString()));
                CommandeStatut.CreateDevisSage = (ArrayCreateDevisConfig != null && ArrayCreateDevisConfig.Contains(PsOrderStateLang.IDOrderState.ToString()));
                CommandeStatut.Payment = (ArrayPaymentConfig != null && ArrayPaymentConfig.Contains(PsOrderStateLang.IDOrderState.ToString()));
                CommandeStatut.Relance = (ArrayRelanceConfig != null && ArrayRelanceConfig.Contains(PsOrderStateLang.IDOrderState.ToString()));
                CommandeStatut.Annulation = (ArrayAnnulationConfig != null && ArrayAnnulationConfig.Contains(PsOrderStateLang.IDOrderState.ToString()));

                //if (ArrayPaymentConfig != null)
                //    foreach (String ArrayConfigItem in ArraySyncConfig)
                //    {
                //        if (ArrayConfigItem == PsOrderStateLang.IDOrderState.ToString())
                //        {
                //            isArraySyncConfig = true;
                //        }
                //    }
                //if (ArrayPaymentConfig != null)
                //    foreach (String ArrayConfigItem in ArrayPaymentConfig)
                //    {
                //        if (ArrayConfigItem == PsOrderStateLang.IDOrderState.ToString())
                //        {
                //            isArrayPaymentConfig = true;
                //        }
                //    }
                //if (ArrayRelanceConfig != null)
                //    foreach (String ArrayConfigItem in ArrayRelanceConfig)
                //    {
                //        if (ArrayConfigItem == PsOrderStateLang.IDOrderState.ToString())
                //        {
                //            isArrayRelanceConfig = true;
                //        }
                //    }
                //if (ArrayAnnulationConfig != null)
                //    foreach (String ArrayConfigItem in ArrayAnnulationConfig)
                //    {
                //        if (ArrayConfigItem == PsOrderStateLang.IDOrderState.ToString())
                //        {
                //            isArrayAnnulationConfig = true;
                //        }
                //    }
                //CommandeStatut.Sync = isArraySyncConfig;
                //CommandeStatut.Payment = isArrayPaymentConfig;
                //CommandeStatut.Relance = isArrayRelanceConfig;
                //CommandeStatut.Annulation = isArrayAnnulationConfig;
                ListCommandeStatut.Add(CommandeStatut);
            }

            this.DataGridCommande.ItemsSource = ListCommandeStatut;
        }

        private void LoadPeriodiciteRelancesAndAnnulation()
        {
            AppConfig config = Global.GetConfig();

            List<int> relancePeriodicites = new List<int>();

            for (int i = 0; i <= 15; i++)
                relancePeriodicites.Add(i);

            Relance1.ItemsSource = relancePeriodicites;
            Relance2.ItemsSource = relancePeriodicites;
            Relance3.ItemsSource = relancePeriodicites;
            Relance1.SelectedItem = config.DureeJourAvantPremiereRelance;
            Relance2.SelectedItem = config.DureeJourApresPremiereRelance;
            Relance3.SelectedItem = config.DureeJourApresDeuxiemeRelance;

            List<int> annulationPeriodicites = new List<int>();

            for (int i = 0; i <= 60; i++)
                annulationPeriodicites.Add(i);

            AnnulationCommande.ItemsSource = annulationPeriodicites;
            AnnulationCommande.SelectedItem = config.DureeJourAnnulationApresDerniereRelance;
        }

        private void WriteCommande()
        {
            this.WriteComboBoxCommandeDepot();
            this.WriteComboBoxCommandeSouche();
            this.WriteComboBoxSageBCStatut();
            this.WriteComboBoxSageDevisStatut();
            this.WriteComboBoxCommandeStatut();
            this.WriteDataGridCommandeStatut();
            this.WritePeriodiciteRelancesAndAnnulation();
        }

        private void WriteCommandeComboBox(ComboBox ComboBox, String Name)
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = new Model.Local.Config();
            Boolean isConfig = false;
            if (ConfigRepository.ExistName(Name))
            {
                isConfig = true;
                Config = ConfigRepository.ReadName(Name);
            }
            Config.Con_Value = (ComboBox.SelectedItem != null && ComboBox.SelectedItem.ToString() != string.Empty)
                ? Core.Global.SplitValue(ComboBox.SelectedItem.ToString())
                : string.Empty;
            if (isConfig == false)
            {
                Config.Con_Name = Name;
                ConfigRepository.Add(Config);
            }
            else
            {
                ConfigRepository.Save();
            }
        }

        private void WriteComboBoxCommandeDepot()
        {
            this.WriteCommandeComboBox(this.ComboBoxCommandeDepot, Core.Global.ConfigCommandeDepot);
        }

        private void WriteComboBoxCommandeSouche()
        {
            this.WriteCommandeComboBox(this.ComboBoxCommandeSouche, Core.Global.ConfigCommandeSouche);
        }

        private void WriteComboBoxSageBCStatut()
        {
            this.WriteCommandeComboBox(this.ComboBoxSageBCStatut, Core.Global.ConfigCommandeSageBCStatut);
        }
        private void WriteComboBoxSageDevisStatut()
        {
            this.WriteCommandeComboBox(this.ComboBoxSageDevisStatut, Core.Global.ConfigCommandeSageDevisStatut);
        }

        private void WriteComboBoxCommandeStatut()
        {
            Core.Global.GetConfig().UpdateConfigCommandeStatuts(
                ReadComboBoxStatut(ComboBoxCommandeStatutDE),
                ReadComboBoxStatut(ComboBoxCommandeStatutBC),
                ReadComboBoxStatut(ComboBoxCommandeStatutPL),
                ReadComboBoxStatut(ComboBoxCommandeStatutBL),
                ReadComboBoxStatut(ComboBoxCommandeStatutFA),
                ReadComboBoxStatut(ComboBoxCommandeStatutFC));
        }
        private int ReadComboBoxStatut(ComboBox item)
        {
            return (item.SelectedItem != null) ? (int)((Model.Prestashop.PsOrderStateLang)item.SelectedItem).IDOrderState : 0;
        }

        private void WriteDataGridCommandeStatut()
        {
            List<CommandeStatut> ListCommandeStatut = this.DataGridCommande.ItemsSource as List<CommandeStatut>;
            String CreateBCValue = string.Empty;
            String CreateDevisValue = string.Empty;
            String PaymentValue = string.Empty;
            String RelanceValue = string.Empty;
            String AnnulationValue = string.Empty;
            foreach (CommandeStatut CommandeStatut in ListCommandeStatut)
            {
                if (CommandeStatut.CreateBCSage)
                    CreateBCValue += CommandeStatut.Id + "#";

                if (CommandeStatut.CreateDevisSage)
                    CreateDevisValue += CommandeStatut.Id + "#";

                if (CommandeStatut.Payment)
                    PaymentValue += CommandeStatut.Id + "#";

                if (CommandeStatut.Relance)
                    RelanceValue += CommandeStatut.Id + "#";

                if (CommandeStatut.Annulation)
                    AnnulationValue += CommandeStatut.Id + "#";
            }

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config CreateBCConfig = new Model.Local.Config();
            Model.Local.Config CreateDevisConfig = new Model.Local.Config();
            Model.Local.Config PaymentConfig = new Model.Local.Config();
            Model.Local.Config RelanceConfig = new Model.Local.Config();
            Model.Local.Config AnnulationConfig = new Model.Local.Config();
            Boolean isCreateBCConfig = false;
            Boolean isCreateDevisConfig = false;
            Boolean isPaymentConfig = false;
            Boolean isRelanceConfig = false;
            Boolean isAnnulationConfig = false;

            // Create BC
            if (ConfigRepository.ExistName(Core.Global.ConfigCommandeStatutCreateBC))
            {
                isCreateBCConfig = true;
                CreateBCConfig = ConfigRepository.ReadName(Core.Global.ConfigCommandeStatutCreateBC);
            }
            CreateBCConfig.Con_Value = CreateBCValue;
            if (isCreateBCConfig == false)
            {
                CreateBCConfig.Con_Name = Core.Global.ConfigCommandeStatutCreateBC;
                ConfigRepository.Add(CreateBCConfig);
            }
            else
            {
                ConfigRepository.Save();
            }
            // Create Devis
            if (ConfigRepository.ExistName(Core.Global.ConfigCommandeStatutCreateDevis))
            {
                isCreateDevisConfig = true;
                CreateDevisConfig = ConfigRepository.ReadName(Core.Global.ConfigCommandeStatutCreateDevis);
            }
            CreateDevisConfig.Con_Value = CreateDevisValue;
            if (isCreateDevisConfig == false)
            {
                CreateDevisConfig.Con_Name = Core.Global.ConfigCommandeStatutCreateDevis;
                ConfigRepository.Add(CreateDevisConfig);
            }
            else
            {
                ConfigRepository.Save();
            }
            // Paiement accepté
            if (ConfigRepository.ExistName(Core.Global.ConfigCommandePayment))
            {
                isPaymentConfig = true;
                PaymentConfig = ConfigRepository.ReadName(Core.Global.ConfigCommandePayment);
            }
            PaymentConfig.Con_Value = PaymentValue;

            if (isPaymentConfig == false)
            {
                PaymentConfig.Con_Name = Core.Global.ConfigCommandePayment;
                ConfigRepository.Add(PaymentConfig);
            }
            else
            {
                ConfigRepository.Save();
            }
            // Relances
            if (ConfigRepository.ExistName(Core.Global.ConfigCommandeRelance))
            {
                isRelanceConfig = true;
                RelanceConfig = ConfigRepository.ReadName(Core.Global.ConfigCommandeRelance);
            }
            RelanceConfig.Con_Value = RelanceValue;

            if (isRelanceConfig == false)
            {
                RelanceConfig.Con_Name = Core.Global.ConfigCommandeRelance;
                ConfigRepository.Add(RelanceConfig);
            }
            else
            {
                ConfigRepository.Save();
            }
            // annulation auto
            if (ConfigRepository.ExistName(Core.Global.ConfigCommandeAnnulation))
            {
                isAnnulationConfig = true;
                AnnulationConfig = ConfigRepository.ReadName(Core.Global.ConfigCommandeAnnulation);
            }
            AnnulationConfig.Con_Value = AnnulationValue;

            if (isAnnulationConfig == false)
            {
                AnnulationConfig.Con_Name = Core.Global.ConfigCommandeAnnulation;
                ConfigRepository.Add(AnnulationConfig);
            }
            else
            {
                ConfigRepository.Save();
            }
        }

        private void WritePeriodiciteRelancesAndAnnulation()
        {
            Global.GetConfig().UpdatePeriodiciteRelances(
                Convert.ToInt32(Relance1.SelectedValue),
                Convert.ToInt32(Relance2.SelectedValue),
                Convert.ToInt32(Relance3.SelectedValue));
            Global.GetConfig().UpdateAnnulation(Convert.ToInt32(AnnulationCommande.SelectedValue));
        }
        private class CommandeStatut : INotifyPropertyChanged
        {
            private uint _Id;
            private String _Statut;
            private Boolean _CreateBCSage = false;
            private Boolean _CreateDevisSage = false;
            private Boolean _Payment = false;
            private Boolean _Relance = false;
            private Boolean _Annulation = false;

            public uint Id
            {
                get { return this._Id; }
                set
                {
                    this._Id = value;
                    OnPropertyChanged("Id");
                }
            }

            public String Statut
            {
                get { return this._Statut; }
                set
                {
                    this._Statut = value;
                    OnPropertyChanged("Statut");
                }
            }

            public Boolean CreateBCSage
            {
                get { return this._CreateBCSage; }
                set
                {
                    this._CreateBCSage = value;
                    OnPropertyChanged("CreateBCSage");
                    if (value)
                    {
                        this._CreateDevisSage = !value;
                        OnPropertyChanged("CreateDevisSage");
                    }
                }
            }
            public Boolean CreateDevisSage
            {
                get { return this._CreateDevisSage; }
                set
                {
                    this._CreateDevisSage = value;
                    OnPropertyChanged("CreateDevisSage");
                    if (value)
                    {
                        this._CreateBCSage = !value;
                        OnPropertyChanged("CreateBCSage");
                    }
                }
            }

            public Boolean Payment
            {
                get { return this._Payment; }
                set
                {
                    this._Payment = value;
                    OnPropertyChanged("Payment");
                }
            }

            public Boolean Relance
            {
                get { return this._Relance; }
                set
                {
                    this._Relance = value;
                    OnPropertyChanged("Relance");
                }
            }

            public Boolean Annulation
            {
                get { return this._Annulation; }
                set
                {
                    this._Annulation = value;
                    OnPropertyChanged("Annulation");
                }
            }

            #region Events

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            #endregion
        }

        #endregion

        #region Reglement
        private void LoadReglementEnabled()
        {
            this.CheckBoxReglement.IsChecked = Core.Global.GetConfig().SyncReglementActif;
        }
        private void LoadReglement()
        {
            //this.ComboBoxReglementModeReglement.IsEnabled = false;
            this.ComboBoxReglementModeReglement.Items.Clear();
            this.ComboBoxReglementCodeJournal.Items.Clear();
            this.ComboBoxReglementPrestashop.Items.Clear();

            String[] PaymentMethods = { "Virement bancaire", "Chèque", "PayPal", "Atos" };
            foreach (string p in PaymentMethods)
                this.ComboBoxReglementPrestashop.Items.Add(p);


            // <JG> 09/04/2013 Modification pour gestion version 1.5
            //Model.Prestashop.PsHookRepository PsHookRepository = new Model.Prestashop.PsHookRepository();
            //if (PsHookRepository.ExistHookPayment())
            //{
            //    List<Model.Prestashop.PsHook> ListPsHook = PsHookRepository.ListHookPayment();
            //    foreach (Model.Prestashop.PsHook PsHook in ListPsHook)
            //    {
            //        Model.Prestashop.PsHookModuleRepository PsHookModuleRepository = new Model.Prestashop.PsHookModuleRepository();
            //        List<Model.Prestashop.PsHookModule> ListPsHookModule = PsHookModuleRepository.ListHook(PsHook.IDHook, Global.CurrentShop.IDShop);
            //        Model.Prestashop.PsModuleRepository PsModuleRepository = new Model.Prestashop.PsModuleRepository();
            //        Model.Prestashop.PsModule PsModule;

            //        foreach (Model.Prestashop.PsHookModule PsHookModule in ListPsHookModule)
            //        {
            //            if (PsModuleRepository.ExistModule(PsHookModule.IDModule, Global.CurrentShop.IDShop))
            //            {
            //                PsModule = PsModuleRepository.ReadModule(PsHookModule.IDModule, Global.CurrentShop.IDShop);
            //                if (PsModule.Active == 1 && !this.ComboBoxReglementPrestashop.Items.Contains(PsModule.Name))
            //                {
            //                    this.ComboBoxReglementPrestashop.Items.Add(PsModule.Name);
            //                }
            //            }
            //        }
            //    }
            //}

            Model.Sage.P_REGLEMENTRepository P_REGLEMENTRepository = new Model.Sage.P_REGLEMENTRepository();
            List<Model.Sage.P_REGLEMENT> ListP_REGLEMENT = P_REGLEMENTRepository.List();
            foreach (Model.Sage.P_REGLEMENT P_REGLEMENT in ListP_REGLEMENT)
                this.ComboBoxReglementModeReglement.Items.Add(P_REGLEMENT.ComboText);

            Model.Sage.F_JOURNAUXRepository F_JOURNAUXRepository = new Model.Sage.F_JOURNAUXRepository();
            List<Model.Sage.F_JOURNAUX> ListF_JOURNAUX = F_JOURNAUXRepository.ListTypeSommeil(2, 0);
            foreach (Model.Sage.F_JOURNAUX F_JOURNAUX in ListF_JOURNAUX)
                this.ComboBoxReglementCodeJournal.Items.Add(F_JOURNAUX.JO_Num);


            Model.Local.SettlementRepository SettlementRepository = new Model.Local.SettlementRepository();
            this.DataGridReglement.ItemsSource = SettlementRepository.List();
        }

        private void WriteReglement()
        {
            if (this.CheckBoxReglement.IsChecked.HasValue)
                Core.Global.GetConfig().UpdateSyncReglementActif(this.CheckBoxReglement.IsChecked.Value);
        }

        private void ComboBoxReglementPrestashop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (this.ComboBoxReglementPrestashop.SelectedItem != null && this.ComboBoxReglementPrestashop.SelectedItem.ToString() != "")
            //{
            //    this.ComboBoxReglementModeReglement.IsEnabled = true;
            //}
        }

        private void ComboBoxReglementModeReglement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ButtonAssocierReglements.IsEnabled = (this.ComboBoxReglementModeReglement.SelectedItem != null
                                                        && !String.IsNullOrWhiteSpace(this.ComboBoxReglementPrestashop.Text)
                                                        && this.ComboBoxReglementCodeJournal.SelectedItem != null);
            //if (this.ComboBoxReglementModeReglement.SelectedItem != null && this.ComboBoxReglementModeReglement.SelectedItem.ToString() != "")
            //{
            //    this.ComboBoxReglementCodeJournal.IsEnabled = true;
            //}
        }

        private void ComboBoxReglementPrestashop_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ButtonAssocierReglements.IsEnabled = (this.ComboBoxReglementModeReglement.SelectedItem != null
                                                        && !String.IsNullOrWhiteSpace(this.ComboBoxReglementPrestashop.Text)
                                                        && this.ComboBoxReglementCodeJournal.SelectedItem != null);
        }

        private void ComboBoxReglementCodeJournal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ButtonAssocierReglements.IsEnabled = (this.ComboBoxReglementModeReglement.SelectedItem != null
                                                        && !String.IsNullOrWhiteSpace(this.ComboBoxReglementPrestashop.Text)
                                                        && this.ComboBoxReglementCodeJournal.SelectedItem != null);
            // move to AssocierReglements_Click 
        }

        private void DataGridReglementButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Model.Local.Settlement Settlement = this.DataGridReglement.SelectedItem as Model.Local.Settlement;
            Model.Local.SettlementRepository SettlementRepository = new Model.Local.SettlementRepository();
            if (SettlementRepository.ExistId(Settlement.Set_Id))
            {
                Model.Local.Settlement SettlementDelete = SettlementRepository.ReadId(Settlement.Set_Id);
                SettlementRepository.Delete(SettlementDelete);
                this.LoadReglement();
            }
        }

        private void AssocierReglements_Click(object sender, RoutedEventArgs e)
        {
            if (this.ComboBoxReglementPrestashop.Text != ""
                && this.ComboBoxReglementModeReglement.SelectedItem != null && this.ComboBoxReglementModeReglement.SelectedItem.ToString() != ""
                && this.ComboBoxReglementCodeJournal.SelectedItem != null && this.ComboBoxReglementCodeJournal.SelectedItem.ToString() != "")
            {
                String[] StringSage = Core.Global.SplitValueArray(this.ComboBoxReglementModeReglement.SelectedItem.ToString());
                if (Core.Global.IsNumeric(StringSage[0].Replace(" ", "")))
                {
                    Model.Local.SettlementRepository SettlementRepository = new Model.Local.SettlementRepository();
                    Model.Local.Settlement Settlement = new Model.Local.Settlement();
                    if (SettlementRepository.ExistPayment(this.ComboBoxReglementPrestashop.Text))
                    {
                        Settlement = SettlementRepository.ReadPayment(this.ComboBoxReglementPrestashop.Text);
                        Settlement.Set_PaymentMethod = this.ComboBoxReglementPrestashop.Text;
                        Settlement.Set_Journal = this.ComboBoxReglementCodeJournal.SelectedItem.ToString();
                        Settlement.Set_Intitule = StringSage[1];
                        Settlement.Sag_Id = Convert.ToInt32(StringSage[0]);
                        SettlementRepository.Save();
                    }
                    else
                    {
                        Settlement.Set_PaymentMethod = this.ComboBoxReglementPrestashop.Text;
                        Settlement.Set_Journal = this.ComboBoxReglementCodeJournal.SelectedItem.ToString();
                        Settlement.Set_Intitule = StringSage[1];
                        Settlement.Sag_Id = Convert.ToInt32(StringSage[0]);
                        SettlementRepository.Add(Settlement);
                    }
                }
                this.LoadReglement();
            }
        }

        #endregion

        #region Carrier

        private void LoadCarrier()
        {
            this.ComboBoxCarrierPrestashop.Items.Clear();

            Model.Local.CarrierRepository LocalCarrierRepository = new Model.Local.CarrierRepository();
            List<Model.Local.Carrier> carriers = LocalCarrierRepository.ListOrderBySagName();

            this.DataGridCarrier.ItemsSource = carriers;

            Model.Prestashop.PsCarrierRepository PsCarrierRepository = new Model.Prestashop.PsCarrierRepository();
            List<Model.Prestashop.PsCarrier> ListCarrier = PsCarrierRepository.ListActive(1, Global.CurrentShop.IDShop);

            foreach (Model.Prestashop.PsCarrier PsCarrier in ListCarrier)
                if (carriers.Count(result => result.Pre_Id == PsCarrier.IDCarrier) == 0)
                    if (PsCarrier.Name != "0" && Convert.ToBoolean(PsCarrier.Active))
                        this.ComboBoxCarrierPrestashop.Items.Add(PsCarrier.IDCarrier + " - " + PsCarrier.Name);

            this.ComboBoxCarrierSage.Items.Clear();
            Model.Sage.P_EXPEDITIONRepository P_EXPEDITIONRepository = new Model.Sage.P_EXPEDITIONRepository();
            List<Model.Sage.P_EXPEDITION> ListP_EXPEDITION = P_EXPEDITIONRepository.ListIntituleNotNull();

            foreach (Model.Sage.P_EXPEDITION P_EXPEDITION in ListP_EXPEDITION)
                if (carriers.Count(result => result.Sag_Id == P_EXPEDITION.cbMarq) == 0)
                    this.ComboBoxCarrierSage.Items.Add(P_EXPEDITION.cbMarq + " - " + P_EXPEDITION.E_Intitule);

            ComboBoxCarrierSage.IsEnabled = (ComboBoxCarrierSage.Items.Count > 0);
            ComboBoxCarrierPrestashop.IsEnabled = (ComboBoxCarrierPrestashop.Items.Count > 0);
        }

        private void ComboBoxCarrierSage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonAssocierCarriers.IsEnabled = (ComboBoxCarrierSage.SelectedItem != null && ComboBoxCarrierPrestashop.SelectedItem != null);
        }

        private void ComboBoxCarrierPrestashop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonAssocierCarriers.IsEnabled = (ComboBoxCarrierSage.SelectedItem != null && ComboBoxCarrierPrestashop.SelectedItem != null);
        }

        private void AssocierCarriers_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxCarrierSage.SelectedItem != null && ComboBoxCarrierPrestashop.SelectedItem != null)
            {
                String[] StringSage = Core.Global.SplitValueArray(this.ComboBoxCarrierSage.SelectedItem.ToString());
                String[] StringPrestashop = Core.Global.SplitValueArray(this.ComboBoxCarrierPrestashop.SelectedItem.ToString());
                if (Core.Global.IsNumeric(StringPrestashop[0].Replace(" ", "")) && Core.Global.IsNumeric(StringSage[0].Replace(" ", "")))
                {
                    Model.Local.CarrierRepository CarrierRepository = new Model.Local.CarrierRepository();
                    Model.Local.Carrier Carrier = new Model.Local.Carrier();
                    if (CarrierRepository.ExistSage(Convert.ToInt32(StringSage[0].Replace(" ", ""))))
                    {
                        Carrier = CarrierRepository.ReadSage(Convert.ToInt32(StringSage[0].Replace(" ", "")));
                        CarrierRepository.Delete(Carrier);
                        Carrier = new Model.Local.Carrier();
                    }
                    Carrier.Pre_Id = Convert.ToInt32(StringPrestashop[0].Replace(" ", ""));
                    Carrier.Sag_Id = Convert.ToInt32(StringSage[0].Replace(" ", ""));
                    Carrier.Pre_Name = StringPrestashop[1].Trim();
                    Carrier.Sag_Name = StringSage[1].Trim();
                    CarrierRepository.Add(Carrier);
                }
                this.LoadCarrier();
            }

            ButtonAssocierCarriers.IsEnabled = (ComboBoxCarrierSage.SelectedItem != null && ComboBoxCarrierPrestashop.SelectedItem != null);
        }
        private void DataGridCarrierButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Model.Local.Carrier Carrier = this.DataGridCarrier.SelectedItem as Model.Local.Carrier;
            Model.Local.CarrierRepository CarrierRepository = new Model.Local.CarrierRepository();
            if (CarrierRepository.ExistSagePrestashop(Carrier.Sag_Id, Carrier.Pre_Id))
            {
                Model.Local.Carrier CarrierDelete = CarrierRepository.ReadSagePrestashop(Carrier.Sag_Id, Carrier.Pre_Id);
                CarrierRepository.Delete(CarrierDelete);
                this.LoadCarrier();
            }
        }

        #endregion

        #region OrderMail

        private void LoadOrderMail()
        {
            this.CheckBoxOrderMailActive.IsChecked = Core.Global.GetConfig().ConfigMailActive;
            this.TextBoxOrderMailUser.Text = Core.Global.GetConfig().ConfigMailUser;
            this.PasswordBoxOrderMailPassword.Password = Core.Global.GetConfig().ConfigMailPassword;
            this.TextBoxOrderMailPort.Text = Core.Global.GetConfig().ConfigMailPort.ToString();
            this.TextBoxOrderMailSMTP.Text = Core.Global.GetConfig().ConfigMailSMTP;
            this.CheckBoxOrderMailSSL.IsChecked = Core.Global.GetConfig().ConfigMailSSL;
        }

        private void TextBoxOrderMailPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.TextBoxOrderMailPort.Text != null)
            {
                if (Core.Global.IsInteger(this.TextBoxOrderMailPort.Text) == false)
                {
                    this.TextBoxOrderMailPort.Text = "25";
                }
            }
            else
            {
                this.TextBoxOrderMailPort.Text = "25";
            }
        }

        private void ButtonMailTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MailMessage ObjMessage = new MailMessage();
                MailAddress ObjAdrExp = new MailAddress(this.TextBoxOrderMailUser.Text);
                MailAddress ObjAdrRec = new MailAddress(this.TextBoxOrderMailUserTest.Text);
                SmtpClient ObjSmtpClient = new SmtpClient(this.TextBoxOrderMailSMTP.Text, Convert.ToInt32(this.TextBoxOrderMailPort.Text));

                ObjMessage.From = ObjAdrExp;
                ObjMessage.To.Add(ObjAdrRec);
                if (Core.UpdateVersion.License.ExtranetOnly)
                {
                    ObjMessage.Subject = "Module Extranet pour Sage : Test d'envoi de mail";
                    ObjMessage.Body = "Mail de test envoyé à partir de Module Extranet pour Sage. Votre configuration est valide !";
                }
                else
                {
                    ObjMessage.Subject = "PrestaConnect : Test d'envoi de mail";
                    ObjMessage.Body = "Mail de test envoyé à partir de PrestaConnect. Votre configuration est valide !";
                }
                ObjMessage.IsBodyHtml = true;
                if (this.CheckBoxOrderMailSSL.IsChecked == true)
                {
                    ObjSmtpClient.EnableSsl = true;
                }
                else
                {
                    ObjSmtpClient.EnableSsl = false;
                }
                ObjSmtpClient.Credentials = new System.Net.NetworkCredential(this.TextBoxOrderMailUser.Text, this.PasswordBoxOrderMailPassword.Password);
                ObjSmtpClient.Send(ObjMessage);
                MessageBox.Show("E-mail envoyé avec succès");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Votre configuration est invalide. \n L'erreur retournée est la suivante :\n " + ex.Message, "Configuration", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ButtonMailParametre_Click(object sender, RoutedEventArgs e)
        {
            if (this.ComboBoxMailTemplaceStatut.SelectedItem != null)
            {
                String Statut = this.ComboBoxMailTemplaceStatut.SelectedItem.ToString();
                Statut = Statut.Replace("System.Windows.Controls.ComboBoxItem: ", "");
                String Type = Core.Global.SplitValue(Statut);
                if (Core.Global.IsNumeric(Type))
                {
                    switch (Type)
                    {
                        case "40":
                            new ConfigurationMailExtranet().Show();
                            break;
                        case "31":
                            new ConfigurationMailTransfert().Show();
                            break;
                        default:
                            new ConfigurationMail().Show();
                            break;
                    }
                }
            }
        }

        private void ComboBoxMailTemplaceStatut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // <JG> 08/11/2012 Correction initialisation des champs du template de Mail
            this.TextBoxMailTemplaceHeader.Text = string.Empty;
            this.CheckBoxMailTemplaceActive.IsChecked = false;
            this.TinyMceMailTemplateContent.HtmlContent = string.Empty;
            this.TextBoxInsertHTML.Text = string.Empty;
            if (this.ComboBoxMailTemplaceStatut.SelectedItem != null && this.ComboBoxMailTemplaceStatut.SelectedItem.ToString() != string.Empty)
            {
                String Statut = this.ComboBoxMailTemplaceStatut.SelectedItem.ToString();
                Statut = Statut.Replace("System.Windows.Controls.ComboBoxItem: ", string.Empty);
                String Type = Core.Global.SplitValue(Statut);
                Model.Local.OrderMailRepository OrderMailRepository = new Model.Local.OrderMailRepository();
                if (Core.Global.IsNumeric(Type))
                {
                    if (OrderMailRepository.ExistType(Convert.ToInt32(Type)))
                    {
                        Model.Local.OrderMail OrderMail = OrderMailRepository.ReadType(Convert.ToInt32(Type));
                        this.TextBoxMailTemplaceHeader.Text = OrderMail.OrdMai_Header;
                        if (OrderMail.OrdMai_Active == true)
                        {
                            this.CheckBoxMailTemplaceActive.IsChecked = true;
                        }
                        else
                        {
                            this.CheckBoxMailTemplaceActive.IsChecked = false;
                        }
                        this.TinyMceMailTemplateContent.HtmlContent = OrderMail.OrdMai_Content;
                        this.TextBoxInsertHTML.Text = OrderMail.OrdMai_Content;
                    }
                }
            }
        }

        private void WriteOrderMail()
        {
            Core.Global.GetConfig().UpdateConfigMailActive(this.CheckBoxOrderMailActive.IsChecked == true);
            Core.Global.GetConfig().UpdateConfigMailUser(this.TextBoxOrderMailUser.Text);
            Core.Global.GetConfig().UpdateConfigMailPassword(this.PasswordBoxOrderMailPassword.Password);
            Core.Global.GetConfig().UpdateConfigMailSMTP(this.TextBoxOrderMailSMTP.Text);
            Core.Global.GetConfig().UpdateConfigMailSSL(this.CheckBoxOrderMailSSL.IsChecked == true);
            Core.Global.GetConfig().UpdateConfigMailPort((Core.Global.IsInteger(this.TextBoxOrderMailPort.Text)) ? int.Parse(this.TextBoxOrderMailPort.Text) : 25);
            this.WriteContentMail();
        }

        private void WriteContentMail()
        {
            if (this.ComboBoxMailTemplaceStatut.SelectedItem != null && this.ComboBoxMailTemplaceStatut.SelectedItem.ToString() != "")
            {
                String Statut = this.ComboBoxMailTemplaceStatut.SelectedItem.ToString();
                Statut = Statut.Replace("System.Windows.Controls.ComboBoxItem: ", "");
                String Type = Core.Global.SplitValue(Statut);
                Model.Local.OrderMailRepository OrderMailRepository = new Model.Local.OrderMailRepository();
                Model.Local.OrderMail OrderMail = new Model.Local.OrderMail();
                if (Core.Global.IsNumeric(Type))
                {
                    Boolean isOrderMail = false;
                    if (OrderMailRepository.ExistType(Convert.ToInt32(Type)))
                    {
                        isOrderMail = true;
                        OrderMail = OrderMailRepository.ReadType(Convert.ToInt32(Type));
                    }
                    OrderMail.OrdMai_Header = this.TextBoxMailTemplaceHeader.Text;
                    if (this.CheckBoxMailTemplaceActive.IsChecked == true)
                    {
                        OrderMail.OrdMai_Active = true;
                    }
                    else
                    {
                        OrderMail.OrdMai_Active = false;
                    }

                    if (Core.Global.GetConfig().UIDisabledWYSIWYG)
                    {
                        OrderMail.OrdMai_Content = this.TextBoxInsertHTML.Text;
                    }
                    else
                    {
                        OrderMail.OrdMai_Content = this.TinyMceMailTemplateContent.HtmlContent;
                    }

                    if (isOrderMail == true)
                    {
                        OrderMailRepository.Save();
                    }
                    else
                    {
                        OrderMail.OrdMai_Type = Convert.ToInt32(Type);
                        OrderMailRepository.Add(OrderMail);
                    }
                }
            }
        }

        #endregion

		private void LoadOrderMarcketplace()
		{

			//this.ComboBoxReglementModeReglement.IsEnabled = false;
			this.TextBoxCommandeMarcketPlaceColumnName.Text= "";
			this.TextBoxCommandeMarcketPlaceSQLRequest.Text = "";
			this.TextBoxCommandeMarcketPlaceTextRemplace.Text = "";

			Model.Local.OrderMacketplaceRepository OrderMacketplaceRepository = new Model.Local.OrderMacketplaceRepository();
			this.DataGridCommandeMarcketPlace.ItemsSource = OrderMacketplaceRepository.List();
		}


		private void Bt_EffacerInformationLibrePackaging_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SelectedInfolibrePackaging = null;
        }

        private void Bt_EffacerArticlePackaging_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SelectedArticlePackaging = null;
        }

        private void Bt_EffacerArticleBonReduction_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SelectedArticleReduction = null;
        }

        #region InfoLibre Catalogue
        private void DataGridImportInfoLibreCataButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Model.Local.InformationLibreArticle InformationLibreArticle = this.DataGridInformationLibreArticle.SelectedItem as Model.Local.InformationLibreArticle;
            Model.Local.InformationLibreArticleRepository InformationLibreArticleRepository = new Model.Local.InformationLibreArticleRepository();
            if (InformationLibreArticleRepository.ExistInfoLibre(InformationLibreArticle.Sag_InfoLibreArticle, InformationLibreArticle.Inf_IsStat))
            {
                Model.Local.InformationLibreArticle InformationLibreArticleDelete = InformationLibreArticleRepository.ReadInfoLibre(InformationLibreArticle.Sag_InfoLibreArticle, InformationLibreArticle.Inf_Catalogue);
                InformationLibreArticleRepository.Delete(InformationLibreArticleDelete);
            }

            if (this.Cb_NiveauCatalogue.SelectedItem != null)
            {
                if ((int)this.Cb_NiveauCatalogue.SelectedItem == 3)
                {
                    this.CB_ParentCatalogue.ItemsSource = null;
                    this.CB_ParentCatalogue.Items.Clear();
                    InformationLibreArticleRepository = new Model.Local.InformationLibreArticleRepository();
                    this.CB_ParentCatalogue.ItemsSource = InformationLibreArticleRepository.List();
                    this.CB_ParentCatalogue.DisplayMemberPath = "Sag_InfoLibreArticle";
                }
            }
            this.LoadInfoLibre_Catalogue();
        }

        private void LoadInfoLibre_Catalogue()
        {
            DataContext.ListInformationLibreArticle = new ObservableCollection<Model.Local.InformationLibreArticle>();
            foreach (Model.Local.InformationLibreArticle InformationLibreArticle in new Model.Local.InformationLibreArticleRepository().List())
                DataContext.ListInformationLibreArticle.Add(InformationLibreArticle);
        }

        private void CB_InfoLibreCatalogue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Cb_NiveauCatalogue.Items.Clear();
            this.Cb_NiveauCatalogue.Items.Add(2);
            this.Cb_NiveauCatalogue.Items.Add(3);
            this.Cb_NiveauCatalogue.SelectedItem = null;

            this.CB_ParentCatalogue.ItemsSource = null;
            this.CB_ParentCatalogue.Items.Clear();

            this.Btn_AddInfoLibreCataogue.IsEnabled = false;
        }

        private void Cb_NiveauCatalogue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.CB_ParentCatalogue.ItemsSource = null;
            this.CB_ParentCatalogue.Items.Clear();
            if (this.Cb_NiveauCatalogue.SelectedItem != null)
            {
                if ((int)this.Cb_NiveauCatalogue.SelectedItem == 2)
                {
                    Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                    this.CB_ParentCatalogue.ItemsSource = CatalogRepository.ListParent(0);
                    this.CB_ParentCatalogue.DisplayMemberPath = "Cat_Name";
                }
                else
                {
                    Model.Local.InformationLibreArticleRepository InformationLibreArticleRepository = new Model.Local.InformationLibreArticleRepository();
                    this.CB_ParentCatalogue.ItemsSource = InformationLibreArticleRepository.List();
                    this.CB_ParentCatalogue.DisplayMemberPath = "Sag_InfoLibreArticle";
                }
            }

            this.Btn_AddInfoLibreCataogue.IsEnabled = false;
        }

        private void CB_ParentCatalogue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Btn_AddInfoLibreCataogue.IsEnabled = true;
        }

        private void Btn_AddInfoLibreCataogue_Click(object sender, RoutedEventArgs e)
        {
            //if (this.CB)
            //{

            //}
            //else
            //{
            Model.Local.InformationLibreArticleRepository InformationLibreArticleRepository = new Model.Local.InformationLibreArticleRepository();

            if (this.RadIL.IsChecked == true && InformationLibreArticleRepository.ExistInfoLibre(this.CB_InfoLibreCatalogue.SelectedValue.ToString(), false))
            {
                MessageBox.Show("Cette information libre est déjà utilisée", "Import via les informations libres", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (this.RadStat.IsChecked == true && InformationLibreArticleRepository.ExistInfoLibre(this.CB_StatCatalogue.Text.ToString(), true))
            {
                MessageBox.Show("Cette statistique est déjà utilisée", "Import via les informations libres", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Model.Local.InformationLibreArticle InformationLibreArticle = new Model.Local.InformationLibreArticle();
                Model.Sage.P_INTSTATART P_INTSTATART = new Model.Sage.P_INTSTATART();
                if (this.RadIL.IsChecked == true)
                {
                    InformationLibreArticle.Sag_InfoLibreArticle = this.CB_InfoLibreCatalogue.SelectedValue.ToString();
                    InformationLibreArticle.Inf_IsStat = false;
                }
                else
                {
                    P_INTSTATART = (Model.Sage.P_INTSTATART)this.CB_StatCatalogue.SelectedItem;
                    InformationLibreArticle.Sag_InfoLibreArticle = P_INTSTATART.P_IntStatArt1;
                    InformationLibreArticle.Inf_Stat = P_INTSTATART.cbMarq;
                    InformationLibreArticle.Inf_IsStat = true;
                }

                InformationLibreArticle.Inf_Catalogue = (int)this.Cb_NiveauCatalogue.SelectedValue;
                InformationLibreArticle.Inf_Parent = this.CB_ParentCatalogue.Text;

                InformationLibreArticleRepository.Add(InformationLibreArticle);

                if (this.Cb_NiveauCatalogue.SelectedItem != null)
                {
                    if ((int)this.Cb_NiveauCatalogue.SelectedItem == 3)
                    {
                        this.CB_ParentCatalogue.ItemsSource = null;
                        this.CB_ParentCatalogue.Items.Clear();
                        InformationLibreArticleRepository = new Model.Local.InformationLibreArticleRepository();
                        this.CB_ParentCatalogue.ItemsSource = InformationLibreArticleRepository.List();
                        this.CB_ParentCatalogue.DisplayMemberPath = "Sag_InfoLibreArticle";
                    }
                }
                this.LoadInfoLibre_Catalogue();
            }
            // }


        }

        private void CB_StatCatalogue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Cb_NiveauCatalogue.Items.Clear();
            this.Cb_NiveauCatalogue.Items.Add(2);
            this.Cb_NiveauCatalogue.Items.Add(3);
            this.Cb_NiveauCatalogue.SelectedItem = null;

            this.CB_ParentCatalogue.ItemsSource = null;
            this.CB_ParentCatalogue.Items.Clear();

            this.Btn_AddInfoLibreCataogue.IsEnabled = false;
        }
        #endregion

        private void buttonCheckAllCountryReplaceCodeISO_Click(object sender, RoutedEventArgs e)
        {
            DataContext.CheckAllCountryReplaceCodeISO(true);
        }

        private void buttonUncheckAllCountryReplaceCodeISO_Click(object sender, RoutedEventArgs e)
        {
            DataContext.CheckAllCountryReplaceCodeISO(false);
        }

        private void Bt_EffacerStatutDE_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxCommandeStatutDE.SelectedItem = null;
        }

        private void Bt_EffacerStatutBC_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxCommandeStatutBC.SelectedItem = null;
        }

        private void Bt_EffacerStatutPL_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxCommandeStatutPL.SelectedItem = null;
        }

        private void Bt_EffacerStatutBL_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxCommandeStatutBL.SelectedItem = null;
        }

        private void Bt_EffacerStatutFA_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxCommandeStatutFA.SelectedItem = null;
        }

        private void Bt_EffacerStatutFC_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxCommandeStatutFC.SelectedItem = null;
        }

        private void ButtonUnselectCatTarif_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SelectedGroup.CategorieTarifaire = null;
            DataContext.SelectedGroup.Source.Grp_CatTarifId = null;
        }

        private void ButtonUnselectInformationLibreFeature_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.SelectedInformationLibre.Feature != null)
            {
                //if (MessageBox.Show("Supprimer le lien entre l'information libre \"" + DataContext.SelectedInformationLibre.SageInfoLibre.CB_Name + "\""
                //    + " et la caractéristique \"" + DataContext.SelectedInformationLibre.Feature.Name + "\" ?", "Gestion des caractéristiques",
                //    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DataContext.SelectedInformationLibre.InfoLibre.Cha_Id = 0;
                    DataContext.SelectedInformationLibre.Feature = null;
                    DataContext.SelectedInformationLibre.InfoValeursMode = DataContext.ListInformationLibreValeursMode.FirstOrDefault(m => m._InformationLibreValeursMode == Core.Parametres.InformationLibreValeursMode.NonTransferees);
                }
            }
        }

        private void ButtonUnselectStatistiqueFeature_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.SelectedStatistiqueArticle.Feature != null)
            {
                //if (MessageBox.Show("Supprimer le lien entre la statistique \"" + DataContext.SelectedStatistiqueArticle.SageStatistique.P_IntStatArt1 + "\""
                //    + " et la caractéristique \"" + DataContext.SelectedStatistiqueArticle.Feature.Name + "\" ?", "Gestion des caractéristiques",
                //    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DataContext.SelectedStatistiqueArticle.StatArt.Cha_Id = 0;
                    DataContext.SelectedStatistiqueArticle.Feature = null;
                    DataContext.SelectedStatistiqueArticle.InfoValeursMode = DataContext.ListInformationLibreValeursMode.FirstOrDefault(m => m._InformationLibreValeursMode == Core.Parametres.InformationLibreValeursMode.NonTransferees);
                }
            }
        }

        private void ButtonUnselectInformationArticleFeature_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.SelectedSageInfoArticle.Feature != null)
            {
                //if (MessageBox.Show("Supprimer le lien entre l'information \"" + DataContext.SelectedSageInfoArticle.SageInfoArticle.Intitule + "\""
                //    + " et la caractéristique \"" + DataContext.SelectedSageInfoArticle.Feature.Name + "\" ?", "Gestion des caractéristiques",
                //    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DataContext.SelectedSageInfoArticle.InfoArticle.Cha_Id = 0;
                    DataContext.SelectedSageInfoArticle.Feature = null;
                    DataContext.SelectedSageInfoArticle.InfoValeursMode = DataContext.ListInformationLibreValeursMode.FirstOrDefault(m => m._InformationLibreValeursMode == Core.Parametres.InformationLibreValeursMode.NonTransferees);
                }
            }
        }

        private void ButtonUnselectCodeRisqueGroup_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.SelectedCodeRisque.SelectedPsGroup != null)
            {
                DataContext.SelectedCodeRisque.SelectedPsGroup = null;
            }
        }

        private void TextBoxInsertHTML_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.TextBoxInsertHTML.Text))
            {
                this.ViewWebBrowser.NavigateToString(this.TextBoxInsertHTML.Text);
            }
            else
            {
                this.ViewWebBrowser.NavigateToString("<html></html>");
            }
        }

        private void buttonInsertHTML_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxMailTemplaceStatut.SelectedItem != null)
                if (this.TextBoxInsertHTML.Text != string.Empty)
                {
                    this.TinyMceMailTemplateContent.HtmlContent = this.TextBoxInsertHTML.Text;
                }

        }

        private void buttonLoadHTML_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxMailTemplaceStatut.SelectedItem != null)
            {
                this.TextBoxInsertHTML.Text = this.TinyMceMailTemplateContent.HtmlContent;
            }
        }

        private void buttonLoadDbHTML_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxMailTemplaceStatut.SelectedItem != null)
            {
                try
                {
                    if (this.ComboBoxMailTemplaceStatut.SelectedItem != null && this.ComboBoxMailTemplaceStatut.SelectedItem.ToString() != "")
                    {
                        String Statut = this.ComboBoxMailTemplaceStatut.SelectedItem.ToString();
                        Statut = Statut.Replace("System.Windows.Controls.ComboBoxItem: ", "");
                        String Type = Core.Global.SplitValue(Statut);
                        Model.Local.OrderMailRepository OrderMailRepository = new Model.Local.OrderMailRepository();
                        if (Core.Global.IsNumeric(Type))
                        {
                            if (OrderMailRepository.ExistType(Convert.ToInt32(Type)))
                            {
                                Model.Local.OrderMail OrderMail = OrderMailRepository.ReadType(Convert.ToInt32(Type));
                                this.TextBoxInsertHTML.Text = OrderMail.OrdMai_Content;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur chargement", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void buttonInsertDbHTML_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxMailTemplaceStatut.SelectedItem != null)
            {
                try
                {
                    String Statut = this.ComboBoxMailTemplaceStatut.SelectedItem.ToString();
                    Statut = Statut.Replace("System.Windows.Controls.ComboBoxItem: ", "");
                    String Type = Core.Global.SplitValue(Statut);
                    Model.Local.OrderMailRepository OrderMailRepository = new Model.Local.OrderMailRepository();
                    if (Core.Global.IsNumeric(Type))
                    {
                        if (OrderMailRepository.ExistType(Convert.ToInt32(Type)))
                        {
                            Model.Local.OrderMail OrderMail = OrderMailRepository.ReadType(Convert.ToInt32(Type));
                            OrderMail.OrdMai_Content = this.TextBoxInsertHTML.Text;
                            OrderMailRepository.Save();
                            MessageBox.Show("Insertion html réussie !", "Sauvegarde", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur sauvegarde", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ButtonUnselectCodeRisqueGroupDefault_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.SelectedCodeRisque.SelectedPsGroupDefault != null)
            {
                DataContext.SelectedCodeRisque.SelectedPsGroupDefault = null;
            }
        }

        private void ButtonUnselectCollaborateurEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.SelectedCollaborateur.SelectedPsEmployee != null)
            {
                DataContext.SelectedCollaborateur.SelectedPsEmployee = null;
            }
        }

        private void ButtonTestMailModel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ComboBoxMailTemplaceStatut.SelectedItem != null)
                {
                    if (string.IsNullOrWhiteSpace(this.TextBoxOrderMailUserTest.Text))
                    {
                        MessageBox.Show("Veuillez entrer une adresse mail de test !", "Test mail", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (!Core.Global.IsMailAddress(this.TextBoxOrderMailUserTest.Text, Core.Parametres.RegexMail.lvl16_Q))
                    {
                        MessageBox.Show("Format d'adresse mail de test invalide !", "Test mail", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (string.IsNullOrWhiteSpace(this.textBoxMailTestModelOrderID.Text))
                    {
                        MessageBox.Show("Veuillez entrer un numéro de commande PrestaShop !", "Test mail", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (!Core.Global.IsIntegerUnsigned(this.textBoxMailTestModelOrderID.Text))
                    {
                        MessageBox.Show("Numéro de commande PrestaShop incorrect !", "Test mail", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (!new Model.Prestashop.PsOrdersRepository().ExistOrder(int.Parse(this.textBoxMailTestModelOrderID.Text)))
                    {
                        MessageBox.Show("La commande " + this.textBoxMailTestModelOrderID.Text + " n'existe pas dans PrestaShop !", "Test mail", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        int idorder = int.Parse(this.textBoxMailTestModelOrderID.Text);
                        Model.Prestashop.PsOrders PsOrders = new Model.Prestashop.PsOrdersRepository().ReadOrder(idorder);
                        String Statut = this.ComboBoxMailTemplaceStatut.SelectedItem.ToString();
                        Statut = Statut.Replace("System.Windows.Controls.ComboBoxItem: ", string.Empty);
                        String Type = Core.Global.SplitValue(Statut);
                        if (Core.Global.IsInteger(Type))
                        {
                            int type = int.Parse(Type);
                            String User = Core.Global.GetConfig().ConfigMailUser;
                            String Password = Core.Global.GetConfig().ConfigMailPassword;
                            String SMTP = Core.Global.GetConfig().ConfigMailSMTP;
                            Int32 Port = Core.Global.GetConfig().ConfigMailPort;
                            Boolean isSSL = Core.Global.GetConfig().ConfigMailSSL;

                            Model.Local.OrderMailRepository OrderMailRepository = new Model.Local.OrderMailRepository();
                            if (OrderMailRepository.ExistType(type))
                            {
                                Model.Local.OrderMail OrderMail = OrderMailRepository.ReadType(type);
                                // <JG> 05/12/2014 correction test type de mail actif
                                //if (OrderMail.OrdMai_Active)
                                {
                                    OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderId, PsOrders.IDOrder.ToString());
                                    OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderTotalPaidHT, PsOrders.TotalPaidTaxExCl.ToString());
                                    OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderTotalPaidTTC, PsOrders.TotalPaidTaxInCl.ToString());
                                    OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderDate, PsOrders.DateAdd.ToString());
                                    if (!string.IsNullOrEmpty(PsOrders.Cart_URL))
                                        OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderCartLink, PsOrders.Cart_URL);

                                    Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                                    if (PsCustomerRepository.ExistCustomer(PsOrders.IDCustomer))
                                    {
                                        Model.Prestashop.PsCustomer PsCustomer = PsCustomerRepository.ReadCustomer(PsOrders.IDCustomer);
                                        OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderLastName, PsCustomer.LastName);
                                        OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderFirstName, PsCustomer.FirstName);

                                        OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailAccountFirstName, PsCustomer.FirstName);
                                        OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailAccountLastName, PsCustomer.LastName);
                                        OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailAccountCompany, PsCustomer.Company);

                                        Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                                        if (PsAddressRepository.ExistAddress(PsOrders.IDAddressDelivery))
                                        {
                                            Model.Prestashop.PsAddress PsAddress = PsAddressRepository.ReadAddress(PsOrders.IDAddressDelivery);
                                            OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderAddress, PsAddress.Address1 + " " + PsAddress.Address2);
                                            OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderAddress1, PsAddress.Address1);
                                            OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderAddress2, PsAddress.Address2);
                                            OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderPostCode, PsAddress.PostCode);
                                            OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderCity, PsAddress.City);

                                            Model.Prestashop.PsCountryLangRepository PsCountryLangRepository = new Model.Prestashop.PsCountryLangRepository();
                                            if (PsCountryLangRepository.ExistCountryLang(PsAddress.IDCountry, Core.Global.Lang))
                                            {
                                                OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderCountry, PsCountryLangRepository.ReadCountryLang(PsAddress.IDCountry, Core.Global.Lang).Name);
                                            }
                                            OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderPhone, PsAddress.Phone);
                                            OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderMobile, PsAddress.PhoneMobile);
                                        }
                                        MailMessage ObjMessage = new MailMessage();
                                        MailAddress ObjAdrExp = new MailAddress(User);
                                        MailAddress ObjAdrRec = new MailAddress(this.TextBoxOrderMailUserTest.Text);
                                        SmtpClient ObjSmtpClient = new SmtpClient(SMTP, Port);

                                        ObjMessage.From = ObjAdrExp;
                                        ObjMessage.To.Add(ObjAdrRec);
                                        ObjMessage.Subject = OrderMail.OrdMai_Header;
                                        ObjMessage.Body = OrderMail.OrdMai_Content;
                                        ObjMessage.IsBodyHtml = true;
                                        ObjSmtpClient.EnableSsl = isSSL;
                                        ObjSmtpClient.Credentials = new System.Net.NetworkCredential(User, Password);
                                        ObjSmtpClient.Send(ObjMessage);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ButtonArchivePDFFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Tb_ArchivePDFFolder.Text = FBD.SelectedPath.ToString();
            }
        }

		private void DataGridCommandeMarcketPlaceButtonDelete_Click(object sender, RoutedEventArgs e)
		{
			Model.Local.OrderMacketplace OrderMacketplace = this.DataGridCommandeMarcketPlace.SelectedItem as Model.Local.OrderMacketplace;
			Model.Local.OrderMacketplaceRepository OrderMacketplaceRepository = new Model.Local.OrderMacketplaceRepository();
			if (OrderMacketplaceRepository.Exist(OrderMacketplace))
			{
				Model.Local.OrderMacketplace OrderMacketplaceDelete = OrderMacketplaceRepository.Read(OrderMacketplace);
				OrderMacketplaceRepository.Delete(OrderMacketplaceDelete);
				this.LoadOrderMarcketplace();
			}
		}

		private void ButtonCommandeMarcketPlaceAjout_Click(object sender, RoutedEventArgs e)
		{
			Model.Local.OrderMacketplace OrderMacketplace = new Model.Local.OrderMacketplace();
			OrderMacketplace.Ord_ColoumName = TextBoxCommandeMarcketPlaceColumnName.Text;
			OrderMacketplace.Ord_MySQLRequest = TextBoxCommandeMarcketPlaceSQLRequest.Text;
			OrderMacketplace.Ord_ReplaceText = TextBoxCommandeMarcketPlaceTextRemplace.Text;
			Model.Local.OrderMacketplaceRepository OrderMacketplaceRepository = new Model.Local.OrderMacketplaceRepository();
			OrderMacketplaceRepository.Add(OrderMacketplace);
			this.LoadOrderMarcketplace();
		}

		private void ButtonGestionMultiBonReduction_Click(object sender, RoutedEventArgs e)
		{
			View.Config.ConfigurationMultiBonReduction configurationMultiBonReduction = new View.Config.ConfigurationMultiBonReduction();
			configurationMultiBonReduction.ShowDialog();
		}
	}
}