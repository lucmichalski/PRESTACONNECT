using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public partial class Attachment
    {
        public string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            try
            {
                string ext = System.IO.Path.GetExtension(fileName).ToLower();
                Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    mimeType = regKey.GetValue("Content Type").ToString();
            }
            catch (Exception ex) { Core.Error.SendMailError("System MIME Error : " + ex.ToString()); }
            return mimeType;
        }

        #region Methods

        public override string ToString()
        {
            return Att_Name;
        }

        #endregion

        public void EraseFile()
        {
            try
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootAttachment, this.Att_File)))
                    System.IO.File.Delete(System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootAttachment, this.Att_File));
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
