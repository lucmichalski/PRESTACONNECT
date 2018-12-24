using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PRESTACONNECT.Core.Parametres
{
    //<YH> 21/08/2012
    internal sealed class AppFolders
    {
        #region Properties

        public string Root { get; private set; }
        public string RootArticle
        {
            get
            {
                string path = Path.Combine(Root, "Article");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }
        public string RootCatalog
        {
            get
            {
                string path = Path.Combine(Root, "Catalog");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }
        public string RootAttachment
        {
            get
            {
                string path = Path.Combine(Root, "Attachment");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path + "\\";
            }
        }
        public string RootReport
        {
            get
            {
                string path = Path.Combine(Root, "Report");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        public string SmallArticle
        {
            get
            {
                string path = Path.Combine(RootArticle, "small");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }
        public string SmallCatalog
        {
            get
            {
                string path = Path.Combine(RootCatalog, "small");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        public string TempArticle
        {
            get
            {
                string path = Path.Combine(RootArticle, "tmp");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }
        public string TempCatalog
        {
            get
            {
                string path = Path.Combine(RootCatalog, "tmp");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        public string Temp
        {
            get
            {
                string path = Path.Combine(Path.GetTempPath(), "CD5B9652A82D425399B264CADB982CD5");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        #endregion
        #region Constructors

        internal AppFolders(string root)
        {
            Root = root;
        }

        #endregion
    }
}
