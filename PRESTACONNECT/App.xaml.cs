using System;
using System.IO;
using System.Windows;
using PRESTACONNECT.Core;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ResourceDictionary obj;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            foreach (ResourceDictionary item in this.Resources.MergedDictionaries)
                if (item.Source != null)
                    obj = item;

            try
            {
                if (!Core.Global.GetConfig().UIIE11EmulationModeDisabled)
                    Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER" + @"\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", System.AppDomain.CurrentDomain.FriendlyName, 11000);
                else
                {
                    if (Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER" + @"\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", System.AppDomain.CurrentDomain.FriendlyName, null) != null)
                    {
                        Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER" + @"\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", System.AppDomain.CurrentDomain.FriendlyName, 0);
                        try
                        {
                            var variable = Microsoft.Win32.Registry.CurrentUser;
                            variable = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
                            if (variable != null)
                            {
                                variable.DeleteValue(System.AppDomain.CurrentDomain.FriendlyName);
                                variable.Close();
                            }
                        }
                        catch {}
                    }
                }

                // test de connexion PrestaShop
                if (Core.Global.PrestashopConnectionIsValid())
                {
                    try
                    {
                        if (System.IO.Directory.Exists(Global.GetConfig().Folders.Temp))
                            Directory.Delete(Global.GetConfig().Folders.Temp, true);
                    }
                    catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }

                    //if (e.Args.Length == 0 && System.IO.File.Exists(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Img/PrestaConnect_band.png")))
                    //    new SplashScreen("Img/PrestaConnect_band.png").Show(true, true);

                    MainWindow = new MainWindow();
                    if (Core.UpdateVersion.LicenseIsActive == Core.UpdateVersion.LicenceActivation.enabled && e.Args.Length == 0 && Core.UpdateVersion.LaunchUpdate == false)
                        MainWindow.Show();
                    else
                    {
                        if (Core.UpdateVersion.LicenseIsActive != Core.UpdateVersion.LicenceActivation.enabled && e.Args.Length == 0)
                            new License().ShowDialog();

                        Shutdown();
                    }
                }
                else
                {
                    Shutdown();
                }
            }
            catch (Exception exception)
            {
                Core.Error.SendMailError(exception.ToString());
                Shutdown();
            }
        }

        public void ChangeTheme(Uri dictionnaryUri)
        {
            if (String.IsNullOrEmpty(dictionnaryUri.OriginalString) == false)
            {
                ResourceDictionary objNewLanguageDictionary = (ResourceDictionary)(LoadComponent(dictionnaryUri));

                if (objNewLanguageDictionary != null)
                {
                    this.Resources.MergedDictionaries.Remove(obj);
                    this.Resources.MergedDictionaries.Add(objNewLanguageDictionary);
                }
            }
        }
    }
}