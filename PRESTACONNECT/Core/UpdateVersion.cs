using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core
{
    public static class UpdateVersion
    {
        //TODO Versionning for deployment of updates
        public static uint CurrentVersion = 21260;
		#if (SAGE_VERSION_16)
        public static int CurrentSageVersion = 16;
		#elif (SAGE_VERSION_17)
        public static int CurrentSageVersion = 17;
		#elif (SAGE_VERSION_1770)
        public static int CurrentSageVersion = 177;
		#elif (SAGE_VERSION_18)
        public static int CurrentSageVersion = 18;
		#elif (SAGE_VERSION_19)
        public static int CurrentSageVersion = 19;
		#elif (SAGE_VERSION_20)
        public static int CurrentSageVersion = 20;
		#elif (SAGE_VERSION_21)
        public static int CurrentSageVersion = 21;
		#endif

		#if (PRESTASHOP_VERSION_15)
		public static int CurrentPrestaShopVersion = 15;
		#elif (PRESTASHOP_VERSION_160)
		public static int CurrentPrestaShopVersion = 160;
		#elif (PRESTASHOP_VERSION_161)
		public static int CurrentPrestaShopVersion = 161;
		#elif (PRESTASHOP_VERSION_172)
		public static int CurrentPrestaShopVersion = 172;
		#endif

		public enum LicenceActivation
        {
            disabled,
            enabled,
            expired,
            incorrect,
        }

        public static LicenceActivation LicenseIsActive = LicenceActivation.disabled;
        public static PRESTACONNECT.WSKEY.Key License;
        public static bool LaunchUpdate = false;

        // equal to Alternetis_Updater.Core.UpdateInfos 
        public enum ParamInfo
        {
            Product,
            ProductVersion,
            SageVersion,
            PrestaShopVersion,
            TargetVersion,
            ClientName,
            DatabaseServer,
            DatabaseName,
            SQLUser,
            SQLPass,
        }
        public const char ArgSplitter = '=';
        //end Alternetis_Updater.Core.UpdateInfos

        public static string UpdaterArguments(PRESTACONNECT.WSVERSION.Version Version)
        {
            string r = string.Empty;
            r += " " + ParamInfo.Product + ArgSplitter + System.IO.Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);
            r += " " + ParamInfo.ProductVersion + ArgSplitter + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            r += " " + ParamInfo.SageVersion + ArgSplitter + SageFolder(License.Sage);
            r += " " + ParamInfo.PrestaShopVersion + ArgSplitter + PrestaShopFolder(License.Prestashop);
            r += " " + ParamInfo.TargetVersion + ArgSplitter + Version.Target;
            r += " " + ParamInfo.ClientName + ArgSplitter + Properties.Settings.Default.CLIENT.Replace(" ", "_");

            
            r += " " + ParamInfo.DatabaseServer + ArgSplitter + Core.Global.GetConnectionInfos().PrestaconnectServer;
            r += " " + ParamInfo.DatabaseName + ArgSplitter + Core.Global.GetConnectionInfos().PrestaconnectDatabase;
            if (!string.IsNullOrEmpty(Core.Global.GetConnectionInfos().PrestaconnectSQLUser))
            {
                r += " " + ParamInfo.SQLUser + ArgSplitter + Core.Global.GetConnectionInfos().PrestaconnectSQLUser;
                r += " " + ParamInfo.SQLPass + ArgSplitter + Core.Global.GetConnectionInfos().PrestaconnectSQLPass;
            }

            r += " Relaunch-PRESTACONNECT.exe";
            return r;
        }

        public static string SageFolder(int SageVersion)
        {
            string r = string.Empty;
            switch (SageVersion)
            {
                case 16:
                    r = "V16";
                    break;
                case 17:
                    r = "I7";
                    break;
                case 177:
                    r = "I7.70";
                    break;
                case 18:
                    r = "V8.01";
                    break;
                case 19:
                    r = "100C";
                    break;
				case 20:
					r = "100CV2";
                    break;
                case 21:
                    r = "100CV3";
                    break;
				default:
                    break;
            }
            return r;
        }

        public static string PrestaShopFolder(int PrestaShopVersion)
        {
            string r = string.Empty;
            switch (PrestaShopVersion)
            {
                case 15:
                    r = "1.5";
                    break;
                case 160:
                    r = "1.6.0";
                    break;
                case 161:
                    r = "1.6.1";
                    break;
                case 172:
                    r = "1.7.2";
                    break;
                default:
                    break;
            }
            return r;
        }
    }
}
