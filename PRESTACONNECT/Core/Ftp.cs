using System;
using System.Net;
using System.IO;

namespace PRESTACONNECT.Core
{
    public static class Ftp
    {
        public static bool ExistFile(string _path, string _user, string _pass)
        {
            bool res = false;
            try
            {
                System.Net.FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(_path);
                ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                ftpRequest.Credentials = new NetworkCredential(_user, _pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                ftpRequest.EnableSsl = Core.Global.GetConfig().ConfigFTPSSL;
                System.Net.FtpWebResponse response;
                try
                {
                    response = (FtpWebResponse)ftpRequest.GetResponse();
                    res = true;
                    if (response != null)
                        response.Close();
                }
                catch (Exception)
                {
                    res = false;
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return res;
        }

        public static void CreateFolder(string _path, string _user, string _pass)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(_path);

                request.Credentials = new NetworkCredential(_user, _pass);
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;
                request.EnableSsl = Core.Global.GetConfig().ConfigFTPSSL;

                request.Method = WebRequestMethods.Ftp.MakeDirectory;

                FtpWebResponse makeDirectoryResponse = (FtpWebResponse)request.GetResponse();

            }
            catch //(Exception ex)
            {
                // not implemented
            }
        }

        public static bool UploadFile(string _sourcefile, string _targetfile, string _user, string _pass)
        {
            bool up = false;
            try
            {
                System.Net.FtpWebRequest ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(_targetfile);
                ftp.Credentials = new System.Net.NetworkCredential(_user, _pass);
                //userid and password for the ftp server to given  

                ftp.UseBinary = true;
                ftp.UsePassive = true;
                ftp.EnableSsl = Core.Global.GetConfig().ConfigFTPSSL;
                ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                System.IO.FileStream fs = System.IO.File.OpenRead(_sourcefile);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                System.IO.Stream ftpstream = ftp.GetRequestStream();
                ftpstream.Write(buffer, 0, buffer.Length);
                ftpstream.Close();
                ftp.Abort();
                up = true;
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(ex.ToString(), true);
            }
            return up;
        }

        public static void Uploadhtaccess(string _htaccesscontent, string _path, string _user, string _pass)
        {
            try
            {
                if (!ExistFile(_path + "/.htaccess", _user, _pass))
                {
                    string file = Core.Global.GetConfig().Folders.RootReport + "\\htaccess.txt";
                    if (System.IO.File.Exists(file))
                        System.IO.File.Delete(file);

                    StreamWriter fichier = new StreamWriter(file, true, System.Text.Encoding.UTF8);
                    fichier.WriteLine(_htaccesscontent);
                    fichier.Close();

                    UploadFile(file, _path + "/.htaccess", _user, _pass);
                    System.IO.File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(ex.ToString(), true);
            }
        }

        public static void DeleteFile(string _path, string _user, string _pass)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(_path);
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpRequest.EnableSsl = Core.Global.GetConfig().ConfigFTPSSL;
                ftpRequest.Credentials = new NetworkCredential(_user, _pass);

                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                Core.Log.WriteLog(ex.ToString(), true);
            }
        }
    }
}
