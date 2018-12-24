using System;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Threading;

namespace PRESTACONNECT.Core
{
    public class Global
    {
        public static bool UILaunch = false;

        public static String URL_Prestashop = string.Empty;
        public static String URL_Prestashop_Lang = string.Empty;
        public static String Selected_Shop = string.Empty;

        public static String SQLPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "SQL");

        public static String SplitValue(String value)
        {
            String[] SplitValue = value.Split('-');
            return SplitValue[0].Replace(" ", string.Empty);
        }

        public static String[] SplitValueArray(String value)
        {
            String[] SplitValue = value.Split('-');
            return SplitValue;
        }

        public static Boolean IsNumeric(String value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[-+]?\d*[.,]?\d*$");
                return regex.IsMatch(value);
            }
            else
            {
                return false;
            }
        }

        public static Boolean IsNumericSimple(string Value)
        {
            bool flag = false;
            try
            {
                Decimal numvalue;
                if (String.IsNullOrWhiteSpace(Value) == false)
                    flag = Decimal.TryParse(Value, out numvalue);
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            return flag;
        }

        public static Boolean IsNumeric(string Value, out bool ReplaceDot)
        {
            bool flag = false;
            ReplaceDot = false;
            try
            {
                if (String.IsNullOrWhiteSpace(Value) == false)
                {
                    if (Value.Contains('.') && IsNumericSimple("1.2") == false)
                    {
                        ReplaceDot = true;
                        Value = Value.Replace('.', ',');
                    }
                    Decimal numvalue;
                    flag = Decimal.TryParse(Value, out numvalue);
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            return flag;
        }

        public static Boolean IsInteger(String Value)
        {
            Boolean verif = false;
            try
            {
                Int64 numvalue;
                verif = Int64.TryParse(Value, out numvalue);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return verif;
        }
        public static Boolean IsIntegerUnsigned(String Value)
        {
            Boolean verif = false;
            try
            {
                uint numvalue;
                verif = uint.TryParse(Value, out numvalue);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return verif;
        }

        public static Boolean IsDate(String Value)
        {
            Boolean verif = false;
            try
            {
                DateTime datevalue;
                verif = DateTime.TryParse(Value, out datevalue);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return verif;
        }

        public static string GetRandomHexNumber(int digits)
        {
            Random random = new Random();
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }

        public static int SearchReference(string filename)
        {
            string reference;
            int position, AttributeArticle;
            return SearchReference(filename, out reference, out position, out AttributeArticle);
        }
        public static int SearchReference(string filename, out int AttributeArticle)
        {
            string reference;
            int position;
            return SearchReference(filename, out reference, out position, out AttributeArticle);
        }
        public static int SearchReference(string filename, out string reference, out int AttributeArticle)
        {
            int position;
            return SearchReference(filename, out reference, out position, out AttributeArticle);
        }
        public static int SearchReference(string filename, out int position, out int AttributeArticle)
        {
            string reference;
            return SearchReference(filename, out reference, out position, out AttributeArticle);
        }
        public static int SearchReference(string filename, out string ar_ref, out int position, out int Declination)
        {
            Declination = 0;
            position = 1;
            ar_ref = string.Empty;
            Model.Local.Article Article = new Model.Local.Article();
            String reference = filename;
            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();

            if (!F_ARTICLERepository.ExistReference(reference) && reference.Contains("#"))
                reference = reference.Replace('#', '/');

            if (F_ARTICLERepository.ExistReference(reference))
            {
                ar_ref = reference;
            }
            else if (reference.Contains("_")) // else = !F_ARTICLERepository.ExistReference(AR_Ref) && 
            {
                // méthode de récupération de la référence lorsque celle-ci contient des "/" remplacés par des "_"
                if (F_ARTICLERepository.ExistReference(reference.Replace("_", "/")))
                {
                    ar_ref = reference.Replace("_", "/");
                }
                else
                {
                    string temp = filename.Substring(0, filename.LastIndexOf('_'));
                    int count = filename.Count(c => c == '_');
                    if (/*count == 1 &&*/ F_ARTICLERepository.ExistReference(temp))		// Dans le cas où la référence de l'article contient des '_'.
                    {
                        ar_ref = temp;
                        string pos = filename.Substring(filename.LastIndexOf('_') + 1);
                        if (IsInteger(pos))
                            position = int.Parse(pos);
                    }
                    else if (F_ARTICLERepository.ExistReference(temp.Replace("_", "/")))
                    {
                        ar_ref = temp.Replace("_", "/");
                        string pos = filename.Substring(filename.LastIndexOf('_') + 1);
                        if (IsInteger(pos))
                            position = int.Parse(pos);
                    }
                }
            }
            Article = ReadArticleByRefSage(ar_ref, out Declination);


            // si article non identifié recherche dans les références gammes
            if (Article.Art_Id == 0)
            {
                Article = ReadArticleByRefGamme(reference, out Declination, out ar_ref);
                if (Article.Art_Id == 0 && reference.Contains("_")) // else = !F_ARTICLERepository.ExistReference(AR_Ref) && 
                {
                    Model.Sage.F_ARTENUMREFRepository F_ARTENUMREFRepository = new Model.Sage.F_ARTENUMREFRepository();
                    // méthode de récupération de la référence lorsque celle-ci contient des "/" remplacés par des "_"
                    if (F_ARTENUMREFRepository.ExistReference(reference.Replace("_", "/")))
                    {
                        Article = ReadArticleByRefGamme(reference.Replace("_", "/"), out Declination, out ar_ref);
                    }
                    else
                    {
                        string temp = filename.Substring(0, filename.LastIndexOf('_'));
                        int count = filename.Count(c => c == '_');
                        if (/*count == 1 &&*/ F_ARTENUMREFRepository.ExistReference(temp))		// Dans le cas où la référence de l'article contient des '_'.
						{
                            Article = ReadArticleByRefGamme(temp, out Declination, out ar_ref);
                            string pos = filename.Substring(filename.LastIndexOf('_') + 1);
                            if (IsInteger(pos))
                                position = int.Parse(pos);
                        }
                        else if (F_ARTENUMREFRepository.ExistReference(temp.Replace("_", "/")))
                        {
                            Article = ReadArticleByRefGamme(temp.Replace("_", "/"), out Declination, out ar_ref);
                            string pos = filename.Substring(filename.LastIndexOf('_') + 1);
                            if (IsInteger(pos))
                                position = int.Parse(pos);
                        }
                    }
                }
            }
            // si article non identifié recherche dans les références fournisseurs
            if (Article.Art_Id == 0)
            {
                Article = ReadArticleByRefFournisseur(reference, out Declination, out ar_ref);
                if (Article.Art_Id == 0 && reference.Contains("_")) // else = !F_ARTICLERepository.ExistReference(AR_Ref) && 
                {
                    Model.Sage.F_ARTFOURNISSRepository F_ARTFOURNISSRepository = new Model.Sage.F_ARTFOURNISSRepository();
                    // méthode de récupération de la référence lorsque celle-ci contient des "/" remplacés par des "_"
                    if (F_ARTFOURNISSRepository.ExistReference(reference.Replace("_", "/")))
                    {
                        Article = ReadArticleByRefFournisseur(reference.Replace("_", "/"), out Declination, out ar_ref);
                    }
                    else
                    {
                        string temp = filename.Substring(0, filename.LastIndexOf('_'));
                        int count = filename.Count(c => c == '_');
                        if (/*count == 1 &&*/ F_ARTFOURNISSRepository.ExistReference(temp))     // Dans le cas où la référence de l'article contient des '_'.
						{
                            Article = ReadArticleByRefFournisseur(temp, out Declination, out ar_ref);
                            string pos = filename.Substring(filename.LastIndexOf('_') + 1);
                            if (IsInteger(pos))
                                position = int.Parse(pos);
                        }
                        else if (F_ARTFOURNISSRepository.ExistReference(temp.Replace("_", "/")))
                        {
                            Article = ReadArticleByRefFournisseur(temp.Replace("_", "/"), out Declination, out ar_ref);
                            string pos = filename.Substring(filename.LastIndexOf('_') + 1);
                            if (IsInteger(pos))
                                position = int.Parse(pos);
                        }
                    }
                }
            }
            // si article non identifié recherche dans les références fournisseurs/gammes
            if (Article.Art_Id == 0)
            {
                Article = ReadArticleByRefGammeFournisseur(reference, out Declination, out ar_ref);
                if (Article.Art_Id == 0 && reference.Contains("_")) // else = !F_ARTICLERepository.ExistReference(AR_Ref) && 
                {
                    Model.Sage.F_TARIFGAMRepository F_TARIFGAMRepository = new Model.Sage.F_TARIFGAMRepository();
                    // méthode de récupération de la référence lorsque celle-ci contient des "/" remplacés par des "_"
                    if (F_TARIFGAMRepository.ExistReference(reference.Replace("_", "/")))
                    {
                        Article = ReadArticleByRefGammeFournisseur(reference.Replace("_", "/"), out Declination, out ar_ref);
                    }
                    else
                    {
                        string temp = filename.Substring(0, filename.LastIndexOf('_'));
                        int count = filename.Count(c => c == '_');
                        if (/*count == 1 &&*/ F_TARIFGAMRepository.ExistReference(temp))        // Dans le cas où la référence de l'article contient des '_'.
						{
                            Article = ReadArticleByRefGammeFournisseur(temp, out Declination, out ar_ref);
                            string pos = filename.Substring(filename.LastIndexOf('_') + 1);
                            if (IsInteger(pos))
                                position = int.Parse(pos);
                        }
                        else if (F_TARIFGAMRepository.ExistReference(temp.Replace("_", "/")))
                        {
                            Article = ReadArticleByRefGammeFournisseur(temp.Replace("_", "/"), out Declination, out ar_ref);
                            string pos = filename.Substring(filename.LastIndexOf('_') + 1);
                            if (IsInteger(pos))
                                position = int.Parse(pos);
                        }
                    }
                }
            }
            return Article.Art_Id;
        }
        private static Model.Local.Article ReadArticleByRefSage(string ar_ref, out int Declination)
        {
            Declination = 0;
            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
            Model.Local.Article Article = new Model.Local.Article();
            if (!string.IsNullOrWhiteSpace(ar_ref))
            {
                Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadReference(ar_ref);
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                if (ArticleRepository.ExistSag_Id(F_ARTICLE.cbMarq))
                {
                    Article = ArticleRepository.ReadSag_Id(F_ARTICLE.cbMarq);
                }
                else if (CompositionArticleRepository.ExistSage(F_ARTICLE.cbMarq))
                {
                    Model.Local.CompositionArticle CompositionArticle = CompositionArticleRepository.ReadSage(F_ARTICLE.cbMarq);
                    Declination = CompositionArticle.ComArt_Id;
                    Article = ArticleRepository.ReadArticle(CompositionArticle.ComArt_ArtId);
                }
            }
            return Article;
        }
        private static Model.Local.Article ReadArticleByRefGamme(string reference, out int Declination, out string sage_ar_ref)
        {
            sage_ar_ref = string.Empty;
            Declination = 0;
            Model.Local.Article Article = new Model.Local.Article();
            Model.Sage.F_ARTENUMREFRepository F_ARTENUMREFRepository = new Model.Sage.F_ARTENUMREFRepository();
            if (F_ARTENUMREFRepository.ExistReference(reference))
            {
                Model.Sage.F_ARTENUMREF F_ARTENUMREF = F_ARTENUMREFRepository.ReadReference(reference);
                Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                if (AttributeArticleRepository.ExistSage(F_ARTENUMREF.cbMarq))
                {
                    Article = ReadArticleByRefSage(F_ARTENUMREF.AR_Ref, out Declination);
                    Declination = AttributeArticleRepository.ReadSage(F_ARTENUMREF.cbMarq).AttArt_Id;
                    sage_ar_ref = F_ARTENUMREF.AR_Ref;
                }
                else if (CompositionArticleRepository.ExistSageGamme(F_ARTENUMREF.cbMarq))
                {
                    Model.Local.CompositionArticle CompositionArticle = CompositionArticleRepository.ReadSageGamme(F_ARTENUMREF.cbMarq);
                    Article = CompositionArticle.Article;
                    Declination = CompositionArticle.ComArt_Id;
                    sage_ar_ref = F_ARTENUMREF.AR_Ref;
                }
            }
            return Article;
        }
        private static Model.Local.Article ReadArticleByRefFournisseur(string reference, out int Declination, out string sage_ar_ref)
        {
            Declination = 0;
            sage_ar_ref = string.Empty;
            Model.Local.Article Article = new Model.Local.Article();
            Model.Sage.F_ARTFOURNISSRepository F_ARTFOURNISSRepository = new Model.Sage.F_ARTFOURNISSRepository();
            if (F_ARTFOURNISSRepository.ExistReference(reference))
            {
                Model.Sage.F_ARTFOURNISS F_ARTFOURNISS = F_ARTFOURNISSRepository.ReadReference(reference);
                Article = ReadArticleByRefSage(F_ARTFOURNISS.AR_Ref, out Declination);
                sage_ar_ref = F_ARTFOURNISS.AR_Ref;
            }
            return Article;
        }
        private static Model.Local.Article ReadArticleByRefGammeFournisseur(string reference, out int Declination, out string sage_ar_ref)
        {
            Declination = 0;
            sage_ar_ref = string.Empty;
            Declination = 0;
            Model.Local.Article Article = new Model.Local.Article();
            Model.Sage.F_TARIFGAMRepository F_TARIFGAMRepository = new Model.Sage.F_TARIFGAMRepository();
            if (F_TARIFGAMRepository.ExistReference(reference))
            {
                Model.Sage.F_TARIFGAM F_TARIFGAM = F_TARIFGAMRepository.ReadReference(reference);
                Model.Sage.F_ARTENUMREFRepository F_ARTENUMREFRepository = new Model.Sage.F_ARTENUMREFRepository();
                if (F_ARTENUMREFRepository.ExistReferenceGamme1Gamme2(F_TARIFGAM.AR_Ref, (F_TARIFGAM.AG_No1.HasValue ? F_TARIFGAM.AG_No1.Value : 0), (F_TARIFGAM.AG_No2.HasValue ? F_TARIFGAM.AG_No2.Value : 0)))
                {
                    Model.Sage.F_ARTENUMREF F_ARTENUMREF = F_ARTENUMREFRepository.ReadReferenceGamme1Gamme2(F_TARIFGAM.AR_Ref, (F_TARIFGAM.AG_No1.HasValue ? F_TARIFGAM.AG_No1.Value : 0), (F_TARIFGAM.AG_No2.HasValue ? F_TARIFGAM.AG_No2.Value : 0));
                    Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                    Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                    if (AttributeArticleRepository.ExistSage(F_ARTENUMREF.cbMarq))
                    {
                        Article = ReadArticleByRefSage(F_TARIFGAM.AR_Ref, out Declination);
                        Declination = AttributeArticleRepository.ReadSage(F_TARIFGAM.cbMarq).AttArt_Id;
                        sage_ar_ref = F_TARIFGAM.AR_Ref;
                    }
                    else if (CompositionArticleRepository.ExistSageGamme(F_ARTENUMREF.cbMarq))
                    {
                        Model.Local.CompositionArticle CompositionArticle = CompositionArticleRepository.ReadSageGamme(F_ARTENUMREF.cbMarq);
                        Article = CompositionArticle.Article;
                        Declination = CompositionArticle.ComArt_Id;
                        sage_ar_ref = F_ARTENUMREF.AR_Ref;
                    }
                }
            }
            return Article;
        }

        public static List<int> SearchListReference(string filename)
        {
            List<int> ListID = new List<int>();
            try
            {
                String reference = filename;
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();

                if (!F_ARTICLERepository.ExistReference(reference) && reference.Contains("#"))
                    reference = reference.Replace('#', '/');

                string ar_ref_modele = string.Empty;
                if (F_ARTICLERepository.ExistReference(reference))
                {
                    ar_ref_modele = reference;
                }
                else if (reference.Contains("_"))
                {
                    // méthode de récupération de la référence lorsque celle-ci contient des "/" remplacés par des "_"
                    if (F_ARTICLERepository.ExistReference(reference.Replace("_", "/")))
                    {
                        ar_ref_modele = reference.Replace("_", "/");
                    }
                    else
                    {
                        string temp = filename.Substring(0, filename.LastIndexOf('_'));
                        int count = filename.Count(c => c == '_');
                        if (count == 1 && F_ARTICLERepository.ExistReference(temp))
                        {
                            ar_ref_modele = temp;
                        }
                        else if (F_ARTICLERepository.ExistReference(temp.Replace("_", "/")))
                        {
                            ar_ref_modele = temp.Replace("_", "/");
                        }
                    }
                }

                Model.Sage.F_ARTCLIENTRepository F_ARTCLIENTRepository = new Model.Sage.F_ARTCLIENTRepository();

                if (F_ARTCLIENTRepository.ExistReferenceClient(ar_ref_modele))
                {
                    // chargement liste des clients mappés
                    if (Core.Temp.ListF_COMPTET_Light.Count == 0)
                    {
                        Core.Temp.LoadListesClients();
                    }

                    List<string> ct_num = Core.Temp.ListF_COMPTET_Light.Select(c => c.CT_Num).ToList();
                    ct_num.AddRange(Core.Temp.ListF_COMPTET_Centrales.Select(c => c.CT_Num));

                    // récupération des articles concernés et filtrages parmis les clients présents sur le site
                    List<Model.Sage.F_ARTCLIENT> list_fartclient = F_ARTCLIENTRepository.ListClien(ar_ref_modele).Where(ca => ct_num.Contains(ca.CT_Num)).ToList();

                    foreach (Model.Sage.F_ARTCLIENT art_client in list_fartclient)
                    {
                        // lecture de l'article enfant rattaché à l'article parent via la référence client
                        Model.Sage.F_ARTICLE f_article = F_ARTICLERepository.ReadReference(art_client.AR_Ref);
                        // si l'article enfant existe dans PrestaConnect
                        if (ArticleRepository.ExistSag_Id(f_article.cbMarq))
                        {
                            // on ajoute son identifiant à la liste des articles sur lequel il faut importer l'image
                            ListID.Add(ArticleRepository.ReadSag_Id(f_article.cbMarq).Art_Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return ListID;
        }

        public static Boolean ExtractNumeroClient(String MaxClient, out String Lettrage, out Int64 Numero)
        {
            Boolean result = false;
            String Temp = string.Empty;
            Lettrage = string.Empty;
            Numero = 0;
            Int32 compteur = 0;
            try
            {
                while (string.IsNullOrEmpty(Temp))
                {
                    Temp = MaxClient.Substring(compteur);
                    if (IsInteger(Temp) && Temp.Substring(0, 1) != "0")
                    {
                        Numero = Int64.Parse(Temp);
                        Numero += 1;
                        result = true;

                        if (Numero.ToString().Length > Temp.Length && Lettrage.Substring(Lettrage.Length - 2) == "0")
                        {
                            Lettrage = Lettrage.Substring(0, Lettrage.Length - 1);
                        }
                        break;
                    }
                    else
                    {
                        Temp = string.Empty;
                        compteur += 1;
                        Lettrage = MaxClient.Substring(0, compteur);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[SYNCRO CLIENT] Erreur lors de la recherche du numéro de client suivant<br />" + ex.ToString());
            }
            return result;
        }

        public static String RemoveDiacritics(String input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            else
            {
                string formD = input.Normalize(NormalizationForm.FormD);
                StringBuilder sbNoDiacritics = new StringBuilder();
                foreach (char c in formD)
                {
                    if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                        sbNoDiacritics.Append(c);
                }
                string noDiacritics = sbNoDiacritics.ToString().Normalize(NormalizationForm.FormC);
                return noDiacritics;
            }
        }

        public static String RemoveDiacriticsAdvanced(String input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            else
            {
                string formD = input.Normalize(NormalizationForm.FormD);
                StringBuilder sbNoDiacritics = new StringBuilder();

                foreach (char c in formD)
                {
                    if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                        sbNoDiacritics.Append(c);
                    else if (autorised_accent.Contains(c))
                    {
                        sbNoDiacritics.Append(c);
                    }
                }
                string noDiacritics = sbNoDiacritics.ToString().Normalize(NormalizationForm.FormC);

                return noDiacritics;
                /*
                 * 
                 if (str == null) return null;
                    var chars =
                        from c in str.Normalize(NormalizationForm.FormD).ToCharArray()
                        let uc = CharUnicodeInfo.GetUnicodeCategory(c)
                        where uc != UnicodeCategory.NonSpacingMark
                        select c;

                    var cleanStr = new string(chars.ToArray()).Normalize(NormalizationForm.FormC);

                    return cleanStr;
                 * 
                 */
            }
        }
        private static List<char> _autorised_accent = null;
        public static List<char> autorised_accent
        {
            get
            {
                if (_autorised_accent == null)
                {
                    _autorised_accent = new List<char>();
                    System.Globalization.UnicodeCategory temp_unicode_category;
                    foreach (char a in "áàâäãåçéèêëíìîïñóòôöõúùûüýÿæœÁÀÂÄÃÅÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸÆŒ".Normalize(NormalizationForm.FormD))
                    {
                        temp_unicode_category = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(a);
                        if (temp_unicode_category != System.Globalization.UnicodeCategory.UppercaseLetter
                            && temp_unicode_category != System.Globalization.UnicodeCategory.LowercaseLetter
                            && !_autorised_accent.Contains(a))
                        {
                            _autorised_accent.Add(a);
                        }
                    }
                }
                return _autorised_accent;
            }
        }
        public static string ConvertUnicodeToISO_8859_1(string input)
        {
            Encoding target = Encoding.GetEncoding("ISO-8859-1");
            Encoding unicode = Encoding.UTF8;
            byte[] utfBytes = unicode.GetBytes(input);
            byte[] isoBytes = Encoding.Convert(unicode, target, utfBytes);
            string msg = target.GetString(isoBytes);
            return msg;
        }

        public static int MaxIndexOfPonctuation(string line)
        {
            return MaxIndexOfPonctuation(line, new Char[] { ' ', ',', '.', '?', '!', ':', ')', '}', ']' });
        }
        public static int MaxIndexOfPonctuation(string line, Char[] ponctuation)
        {
            List<int> list = new List<int>();
            list.Add(0);
            foreach (Char p in ponctuation)
            {
                list.Add(line.LastIndexOf(p));
            }

            // limited version
            //char space = ' ', comma = ',', dot = '.', interrogation = '?', exclamation = '!';
            //list.Add(line.LastIndexOf(space));
            //list.Add(line.LastIndexOf(comma));
            //list.Add(line.LastIndexOf(dot));
            //list.Add(line.LastIndexOf(interrogation));
            //list.Add(line.LastIndexOf(exclamation));

            return list.Max() + 1;
        }

        public static String ReadLinkRewrite(string input)
        {
            input = RemoveDiacritics(input);
            input = RemovePurge(input, input.Length + 1);

            StringBuilder builder = new StringBuilder(input);

            builder = builder.Replace(", ", "-");
            builder = builder.Replace("\\", string.Empty);
            builder = builder.Replace("/", string.Empty);
            builder = builder.Replace("(", string.Empty);
            builder = builder.Replace(")", string.Empty);
            // ASCII character 32
            builder = builder.Replace(" ", "-");
            // ASCII character 160
            builder = builder.Replace(" ", "-");
            builder = builder.Replace(",", "-");
            builder = builder.Replace("+", string.Empty);
            builder = builder.Replace("'", string.Empty);
            builder = builder.Replace("&", string.Empty);
            builder = builder.Replace("?", string.Empty);
            builder = builder.Replace("!", string.Empty);
            builder = builder.Replace("^", string.Empty);
            builder = builder.Replace("$", string.Empty);
            builder = builder.Replace("£", string.Empty);
            builder = builder.Replace("*", string.Empty);
            builder = builder.Replace(":", string.Empty);
            builder = builder.Replace("!", string.Empty);
            builder = builder.Replace("?", string.Empty);
            builder = builder.Replace("@", "a");
            builder = builder.Replace("\"", string.Empty);
            builder = builder.Replace(".", string.Empty);
            builder = builder.Replace("§", string.Empty);
            builder = builder.Replace("°", string.Empty);
            builder = builder.Replace("¨", string.Empty);
            builder = builder.Replace("%", string.Empty);
            builder = builder.Replace("µ", string.Empty);
            builder = builder.Replace("€", string.Empty);
            builder = builder.Replace("²", "2");
            builder = builder.Replace("Ø", "d");
            builder = builder.Replace("©", string.Empty);
            builder = builder.Replace("®", string.Empty);
            builder = builder.Replace("~", string.Empty);
            builder = builder.Replace("¼", string.Empty);
            builder = builder.Replace("½", string.Empty);
            builder = builder.Replace("¾", string.Empty);
            builder = builder.Replace("¦", string.Empty);
            builder = builder.Replace("|", string.Empty);

            //builder = builder.Replace("", string.Empty);

            while (builder.ToString().IndexOf("--") != -1)
                builder = builder.Replace("--", "-");

            while (builder.ToString().EndsWith("-"))
                builder = builder.Remove(builder.ToString().LastIndexOf("-"), 1);

            string output = builder.ToString().ToLower();
            if (output.Length > 128)
                output = output.Substring(0, 128);

            return output;
        }

        public static String RemovePurge(String input, int length_limit)
        {
            StringBuilder builder = new StringBuilder(input);

            builder = builder.Replace("<", string.Empty);
            builder = builder.Replace(">", string.Empty);
            builder = builder.Replace(";", string.Empty);
            builder = builder.Replace("=", string.Empty);
            builder = builder.Replace("#", string.Empty);
            builder = builder.Replace("{", string.Empty);
            builder = builder.Replace("}", string.Empty);

            if (builder.Length > length_limit)
            {
                builder.Remove(length_limit - 1, builder.Length - length_limit);
            }

            return builder.ToString();
        }

        public static String RemovePurgeMeta(String input, int length_limit)
        {
            StringBuilder builder = new StringBuilder(input);

            builder = builder.Replace("<", string.Empty);
            builder = builder.Replace(">", string.Empty);
            builder = builder.Replace(";", string.Empty);
            builder = builder.Replace("=", string.Empty);
            builder = builder.Replace("#", string.Empty);
            builder = builder.Replace("{", string.Empty);
            builder = builder.Replace("}", string.Empty);

            builder = builder.Replace("!", string.Empty);
            builder = builder.Replace("?", string.Empty);
            builder = builder.Replace("+", string.Empty);
            builder = builder.Replace("\"", string.Empty);
            builder = builder.Replace("°", string.Empty);
            builder = builder.Replace("_", string.Empty);
            builder = builder.Replace("$", string.Empty);
            builder = builder.Replace("%", string.Empty);

            if (builder.Length > length_limit)
            {
                builder.Remove(length_limit - 1, builder.Length - length_limit);
            }

            return builder.ToString();
        }

        public static String RemovePurgeEAN(String input)
        {
            string ean = input;
            if (!string.IsNullOrWhiteSpace(ean))
            {
                ean = Regex.Replace(ean, @"[^\d]", String.Empty, RegexOptions.IgnoreCase);

                //StringBuilder builder = new StringBuilder(ean);
                //builder = builder.Replace(" ", string.Empty);
                //builder = builder.Replace(".", string.Empty);
                //builder = builder.Replace("/", string.Empty);
                //builder = builder.Replace("+", string.Empty);
                //builder = builder.Replace("-", string.Empty);
                //builder = builder.Replace("_", string.Empty);
                //builder = builder.Replace("$", string.Empty);
                //builder = builder.Replace("%", string.Empty);
                //ean = builder.ToString();

                if (ean.Length > 13)
                    ean = ean.Substring(0, 13);
            }
            else
            {
                ean = string.Empty;
            }
            return ean;
        }

        public static String EscapeSqlString(String value)
        {
            return value.Replace("'", "''");
        }

        public static String EscapeArgumentSyntax(String value)
        {
            return value.Replace(" ", "_");
        }

        public static String ConfigTheme = "[CONFIGTHEME]";

        public const String PrestashopInstallVersion = "PS_INSTALL_VERSION";
        public const String PrestaShopLogo = "PS_LOGO";
        public const String PrestashopTypeAddressBaseTaxKey = "PS_TAX_ADDRESS_TYPE";
        public const String PrestaShopCustomerPortFolioProfilKey = "CUSTOMER_PORTFOLIO_PROFIL";
        public enum PrestashopTypeAddressBaseTax { id_address_invoice, id_address_delivery };
        public const String PrestashopTaxRulesGroupForEcotaxKey = "PS_ECOTAX_TAX_RULES_GROUP_ID";

        #region Lang

        public static String ConfigLang = "[LANG]";

        //private static UInt32 lang;
        //public static UInt32 Lang
        //{
        //    get
        //    {
        //        if (lang == null)
        //        {
        //            lang = 1;
        //            Boolean isLang = false;
        //            Model.Prestashop.PsLangRepository LangRepository = new Model.Prestashop.PsLangRepository();
        //            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
        //            if (ConfigRepository.ExistName(ConfigLang))
        //            {
        //                Model.Local.Config Config = ConfigRepository.ReadName(ConfigLang);
        //                if (IsNumeric(Config.Con_Value))
        //                {
        //                    if (LangRepository.ExistId(Convert.ToUInt32(Config.Con_Value)))
        //                    {
        //                        isLang = true;
        //                        lang = Convert.ToUInt32(Config.Con_Value);
        //                        URL_Prestashop_Lang = LangRepository.ReadId(lang).IsoCode;
        //                    }
        //                }
        //            }
        //            if (isLang == false)
        //            {
        //                if (LangRepository.ExistIso("fr"))
        //                {
        //                    Model.Prestashop.PsLang Lang = LangRepository.ReadIso("fr");
        //                    lang = Lang.IDLang;
        //                    URL_Prestashop_Lang = Lang.IsoCode;
        //                }
        //            }
        //        }
        //        return lang;
        //    }
        //    set
        //    {
        //        lang = value;
        //    }
        //}

        public static UInt32 Lang = ReadLang();

        private static UInt32 ReadLang()
        {
            UInt32 Return = 1;
            Boolean isLang = false;
            Model.Prestashop.PsLangRepository LangRepository = new Model.Prestashop.PsLangRepository();
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(ConfigLang))
            {
                Model.Local.Config Config = ConfigRepository.ReadName(ConfigLang);
                if (IsNumeric(Config.Con_Value))
                {
                    if (LangRepository.ExistId(Convert.ToUInt32(Config.Con_Value)))
                    {
                        isLang = true;
                        Return = Convert.ToUInt32(Config.Con_Value);
                        URL_Prestashop_Lang = LangRepository.ReadId(Return).IsoCode;
                    }
                }
            }
            if (isLang == false)
            {
                if (LangRepository.ExistIso("fr"))
                {
                    Model.Prestashop.PsLang Lang = LangRepository.ReadIso("fr");
                    Return = Lang.IDLang;
                    URL_Prestashop_Lang = Lang.IsoCode;
                }
            }
            return Return;
        }

        #endregion

        #region Article

        public static String ConfigArticleStock = "[ARTICLESTOCK]";
        public static String ConfigArticleContremarque = "[ARTICLECONTREMARQUE]";
        public static String ConfigArticleRupture = "[ARTICLERUPTURE]";

        public static String ConfigArticlePoidsType = "[ARTICLETYPE]";
        public static String ConfigArticlePoidsUnite = "[ARTICLEUNITE]";

        public static String ConfigArticleCatTarif = "[ARTICLECATTARIF]";

        public static String ConfigArticleTaxe = "[ARTICLETAXE]";

        public const String GestionStockAucun = "0 - Aucun";
        public const String GestionStockAterme = "1 - Stock à terme";
        public const String GestionStockReel = "2 - Stock réel";
        public const String GestionStockDisponible = "3 - Stock disponible";
        public const String GestionStockDisponibleAvance = "4 - Stock disponible avancé";

        #endregion

        #region Client

        // <JG> 08/11/2012
        public static String ConfigClientTypeLien = "[CLIENTTYPELIEN]";
        public enum ConfigClientTypeLienEnum { ComptesIndividuels, CompteCentralisateur };

        public static String ConfigClientCompteCentralisateur = "[CLIENTCENTRALISATEUR]";
        //END

        public static String ConfigClientCompteGeneral = "[CLIENTCOMPTEGENERAL]";
        public static String ConfigClientCompteComptable = "[CLIENTCOMPTECOMPTABLE]";
        public static String ConfigClientCategorieTarifaire = "[CLIENTCATEGORIETARIFAIRE]";

        public static String ConfigClientPeriodicite = "[CLIENTPERIODICITE]";
        public static String ConfigClientCodeRisque = "[CLIENTCODERISQUE]";
        public static String ConfigClientNombreLigne = "[CLIENTNOMBRELIGNE]";
        public static String ConfigClientSaut = "[CLIENTSAUT]";
        public static String ConfigClientLettrage = "[CLIENTLETTRAGE]";
        public static String ConfigClientPrioriteLivraison = "[CLIENTPRIORITELIVRAISON]";
        public static String ConfigClientCollaborateur = "[CLIENTCOLLABORATEUR]";
        public static String ConfigClientDepot = "[CLIENTDEPOT]";
        //public static String ConfigClientAnalytique = "[CLIENTANALYTIQUE]";
        public static String ConfigClientCodeAffaire = "[CLIENTCODEAFFAIRE]";
        public static String ConfigClientDevise = "[CLIENTDEVISE]";
        public static String ConfigClientLangue = "[CLIENTLANGUE]";

        public static String ConfigClientBLFacture = "[CLIENTBLFACTURE]";
        public static String ConfigClientLivraisonPartielle = "[CLIENTLIVRAISONPARTIELLE]";
        public static String ConfigClientValidationAutomatique = "[CLIENTVALIDATIONAUTOMATIQUE]";
        public static String ConfigClientRappel = "[CLIENTRAPPEL]";
        public static String ConfigClientPenalite = "[CLIENTPENALITE]";
        public static String ConfigClientSurveillance = "[CLIENTSURVEILLANCE]";


        public static String ConfigClientConditionLivraison = "[CLIENTCONDITIONLIVRAISON]";
        public static String ConfigClientModeExpedition = "[CLIENTMODEEXPEDITION]";
        // <JG> 07/09/2012
        public static String ConfigClientIntituleAdresse = "[CLIENTINTITULEADRESSE]";
        public enum ConfigClientIntituleAdresseEnum { CodePrestashop, NomPrenomPrestashop };

        #endregion

        #region Commande

        public static String ConfigCommandeDepot = "[COMMANDEDEPOT]";
        public static String ConfigCommandeSouche = "[COMMANDESOUCHE]";
        public static String ConfigCommandeSageBCStatut = "[COMMANDESTATUT]";
        public static String ConfigCommandeSageDevisStatut = "[COMMANDESAGEDEVISSTATUT]";

        public static String ConfigCommandeStatutCreateBC = "[CONFIGCOMMANDESYNCHRO]";
        public static String ConfigCommandeStatutCreateDevis = "[CONFIGCOMMANDESTATUTCREATEDEVIS]";
        public static String ConfigCommandePayment = "[CONFIGCOMMANDESPAYMENT]";
        public static String ConfigCommandeRelance = "[CONFIGCOMMANDESRELANCE]";
        public static String ConfigCommandeAnnulation = "[CONFIGCOMMANDESANNULATION]";

        #endregion

        #region Mail

        #region Paramètres contenu

        public const String MailOrderId = "[ORDERID]";
        public const String MailOrderFirstName = "[ORDERFIRSTNAME]";
        public const String MailOrderLastName = "[ORDERLASTNAME]";
        public const String MailOrderAddress = "[ORDERADDRESS]";
        public const String MailOrderPostCode = "[ORDERPOSTCODE]";
        public const String MailOrderCity = "[ORDERCITY]";

        public const String MailOrderCountry = "[ORDERCOUNTRY]";
        public const String MailOrderAddress1 = "[ORDERADDRESS1]";
        public const String MailOrderAddress2 = "[ORDERADDRESS2]";
        public const String MailOrderPhone = "[ORDERPHONE]";
        public const String MailOrderMobile = "[ORDERMOBILE]";

        public const String MailOrderTotalPaidTTC = "[ORDERTOTALPAIDTTC]";
        public const String MailOrderTotalPaidHT = "[ORDERTOTALPAIDHT]";
        public const String MailOrderDate = "[ORDERDATE]";

        public const String MailOrderCartLink = "[CART_LINK]";

        public const String MailOrderTrackingLink = "[TRACKING_LINK]";

        public const String MailAccountLastName = "[CUSTOMERLASTNAME]";
        public const String MailAccountFirstName = "[CUSTOMERFIRSTNAME]";
        public const String MailAccountPassword = "[CUSTOMERPASSWORD]";
        public const String MailAccountCompany = "[CUSTOMERCOMPANY]";

        public const String MailInvoiceNumbers = "[INVOICENUMBERS]";

        #endregion

        #endregion

        #region Boutique

        public static String PrestashopShopDefault = "PS_SHOP_DEFAULT";

        #endregion

        //<YH> 23/08/2012
        private static AppConfig config { get; set; }
        internal static AppConfig GetConfig()
        {
            if (config == null)
                config = new AppConfig();

            return config;
        }

        private static ConnectionInfos connectionInfos;
        internal static ConnectionInfos GetConnectionInfos()
        {
            if (connectionInfos == null)
                connectionInfos = new ConnectionInfos();

            return connectionInfos;
        }

        public static Boolean PrestashopConnectionIsValid()
        {
            Boolean flag = false;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString))
                {
                    try
                    {
                        connection.Open();
                        flag = true;
                    }
                    finally { connection.Close(); }
                }
            }
            catch (Exception exception)
            {
                Core.Error.SendMailError(exception.ToString());
            }
            return flag;
        }

        //<YH> 12/09/2012
        private static Model.Prestashop.PsShop currentShop;
        public static Model.Prestashop.PsShop CurrentShop
        {
            get
            {
                if (currentShop == null)
                    foreach (var shop in new Model.Prestashop.PsShopRepository().List())
                        if (shop.IDShop == 1)
                        {
                            currentShop = shop;
                            break;
                        }

                return currentShop;
            }
        }

        public static void GoShop()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Core.Global.URL_Prestashop));
        }

        public static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        public static bool IsMailAddress(string Value)
        {
            return IsMailAddress(Value, Core.Global.GetConfig().RegexMailLevel);
        }

        public static bool IsMailAddress(string Value, Core.Parametres.RegexMail RegexMail)
        {
            bool flag = false;

            if (!string.IsNullOrWhiteSpace(Value))
            {
                try
                {
                    flag = Regex.IsMatch(Value, new Model.Internal.RegexMail(RegexMail).RegexExpression);
                }
                catch {};
            }

            //if (String.IsNullOrWhiteSpace(Value) == false && Value.Contains("@"))
            //{
            //    string[] tablemail = Value.Split('@');

            //    if (tablemail.Count() == 2)
            //    {
            //        string contact = tablemail[0];

            //        System.Text.RegularExpressions.Regex regex_contact = new System.Text.RegularExpressions.Regex("^([a-z-A-Z0-9])[a-z-A-Z0-9_.-]*");
            //        if (regex_contact.IsMatch(contact))
            //        {
            //            string system = tablemail[1];

            //            if (system.Contains('.'))
            //            {
            //                string hebergeur = "";
            //                string domaine = "";

            //                System.Text.RegularExpressions.Regex regex_hebergeur = new System.Text.RegularExpressions.Regex("^([a-z-A-Z0-9])[a-z-A-Z0-9_-]*");
            //                System.Text.RegularExpressions.Regex regex_domaine = new System.Text.RegularExpressions.Regex("\\.[a-z-A-Z]{2,}$");

            //                string[] tablesysteme = system.Split('.');
            //                int count = tablesysteme.Count();
            //                if (count > 2)
            //                {
            //                    bool heb = true;
            //                    hebergeur = tablesysteme[0];
            //                    if (regex_hebergeur.IsMatch(hebergeur))
            //                    {
            //                        for (int i = 1; i < count - 1; i++)
            //                        {
            //                            hebergeur += "." + tablesysteme[i];
            //                            heb = regex_hebergeur.IsMatch(tablesysteme[i]);
            //                        }
            //                        domaine = "." + tablesysteme[count - 1];
            //                        if (heb && regex_domaine.IsMatch(domaine))
            //                        {
            //                            flag = true;
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    hebergeur = tablesysteme[0];
            //                    domaine = "." + tablesysteme[1];
            //                    if (regex_hebergeur.IsMatch(hebergeur) && regex_domaine.IsMatch(domaine))
            //                    {
            //                        flag = true;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            return flag;
        }

        public static Boolean IsValidateSiret(string value)
        {
            bool valid = false;
            // suppression des espaces en début, fin et milieu de value
            value = value.Replace(" ", string.Empty);
            if (value.Length == 14)
            {
                char[] siret = value.ToCharArray();
                int sum = 0;
                for (int i = 0; i != 14; i++)
                {
                    int carac;
                    if (int.TryParse(siret[i].ToString(), out carac))
                    {
                        int tmp = ((((i + 1) % 2) + 1) * carac);
                        if (tmp >= 10)
                            tmp -= 9;
                        sum += tmp;
                    }
                }
                valid = (sum % 10 == 0);
            }
            return valid;
        }

        public static string CleanAPE(string value)
        {
            value = RemoveDiacritics(value).ToUpper();
            return Regex.Replace(value, @"[^A-Z0-9]", String.Empty, RegexOptions.IgnoreCase);
        }
        public static Boolean IsValidateAPE(string value)
        {
            value = CleanAPE(value);
            System.Text.RegularExpressions.Regex regex_contact = new System.Text.RegularExpressions.Regex("^([0-9]{4}[A-Z]{1})");
            return regex_contact.IsMatch(value);
        }

        public static string CleanCustomerName(string value)
        {
            string r = string.Empty;
            if (!string.IsNullOrEmpty(value))
                r = (Core.Global.GetConfig().TransfertNameIncludeNumbers)
                ? Regex.Replace(value, "[!<>,;?=+()@#°{}_$%:\"]", String.Empty, RegexOptions.IgnoreCase)
                : Regex.Replace(value, "[0-9!<>,;?=+()@#°{}_$%:\"]", String.Empty, RegexOptions.IgnoreCase);
            return r;
        }
        public static string CleanGenericName(string value)
        {
            string r = string.Empty;
            if (!string.IsNullOrEmpty(value))
                r = Regex.Replace(value, "[<>={}]", String.Empty, RegexOptions.IgnoreCase);
            return r;
        }
        public static string CleanPhoneNumber(string value)
        {
            string r = string.Empty;
            if (!string.IsNullOrEmpty(value))
                r = Regex.Replace(value, "[^+0-9. ()-]", String.Empty, RegexOptions.IgnoreCase);
            if (r.Length > 32)
                r = r.Substring(0, 32);
            return r;
        }
        public static string CleanPostCode(string value)
        {
            string r = string.Empty;
            if (!string.IsNullOrEmpty(value))
                r = Regex.Replace(value, "[^a-zA-Z 0-9-]", String.Empty, RegexOptions.IgnoreCase);
            if (r.Length > 12)
                r = r.Substring(0, 12);
            return r;
        }
        public static string CleanCityName(string value)
        {
            string r = string.Empty;
            if (!string.IsNullOrEmpty(value))
                r = Regex.Replace(value, "[!<>;?=+@#°{}_$%\"]", String.Empty, RegexOptions.IgnoreCase);
            if (r.Length > 64)
                r = r.Substring(0, 64);
            return r;
        }
        public static string CleanAddress(string value)
        {
            string r = string.Empty;
            if (!string.IsNullOrEmpty(value))
                r = Regex.Replace(value, "[!<>?=+@{}_$%]", String.Empty, RegexOptions.IgnoreCase);
            if (r.Length > 128)
                r = r.Substring(0, 128);
            return r;
        }

        // <JG> 28/03/2014
        public static Boolean ExtractNumeroPiece(String CurrentPiece, out String Lettrage, out Int64 Numero)
        {
            Boolean result = false;
            String Temp = string.Empty;
            Lettrage = string.Empty;
            Numero = 0;
            Int32 compteur = 0;
            try
            {
                while (Temp == string.Empty)
                {
                    Temp = CurrentPiece.Substring(compteur);
                    if (IsNumericSimple(Temp) && Temp.Substring(0, 1) != "0")
                    {
                        Numero = Int64.Parse(Temp);
                        Numero += 1;
                        result = true;
                        break;
                    }
                    else
                    {
                        Temp = string.Empty;
                        compteur += 1;
                        Lettrage = CurrentPiece.Substring(0, compteur);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("Erreur lors de la recherche du numéro de pièce courant<br />" + ex.ToString());
            }
            return result;
        }

        public static Boolean VerifyODBCRegistry(out string message)
        {
            Boolean flag = false;
            message = string.Empty;
            try
            {
                string DSN = Properties.Settings.Default.SAGEDSN;
                RegistryKey clelist = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\ODBC\ODBC.INI\ODBC Data Sources\");
                //if (clelist == null)
                //    clelist = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\ODBC\ODBC.INI\ODBC Data Sources\");
                string[] table = clelist.GetValueNames();
                int count = table.Count();
                if (count > 0 && table.Contains(DSN))
                {
                    RegistryKey clesource = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\ODBC\ODBC.INI\" + DSN);
                    if (clelist.GetValue(DSN).ToString() == "SAGE Gestion commerciale 100"
                        || clesource.GetValue("Driver").ToString() == "C:\\windows\\SysWOW64\\CBODBC32.DLL")
                    {
                        string cial_file = clesource.GetValue("DBQ").ToString();
                        string cpta_file = clesource.GetValue("FILE2").ToString();

                        if (string.IsNullOrWhiteSpace(cial_file) || Path.GetExtension(cial_file).ToLower() != ".gcm")
                        {
                            message = "ODBC Sage : Le fichier commercial pour la source de données système \"" + DSN + "\" est invalide !";
                            Log.WriteLog(message, true);
                        }
                        else if (string.IsNullOrWhiteSpace(cpta_file) || Path.GetExtension(cpta_file).ToLower() != ".mae")
                        {
                            message = "ODBC Sage : Le fichier comptable pour la source de données système \"" + DSN + "\" est invalide !";
                            Log.WriteLog(message, true);
                        }
                        else
                        {
                            string[] ValuesCialFile = Path.GetFileNameWithoutExtension(cial_file).Split('\\');
                            string cial_db = ValuesCialFile[ValuesCialFile.Length - 1];

                            string[] ValuesCptaFile = Path.GetFileNameWithoutExtension(cpta_file).Split('\\');
                            string cpta_db = ValuesCptaFile[ValuesCptaFile.Length - 1];

                            if (cial_db.ToLower() == cpta_db.ToLower())
                            {
                                if (cial_db.ToLower() == Core.Global.GetConnectionInfos().SageDatabase.ToLower())
                                {
                                    flag = true;
                                }
                                else
                                {
                                    message = "ODBC Sage : La source de données système \"" + DSN + "\" n'utilise pas la même base de données Sage que PrestaConnect !";
                                    Log.WriteLog(message, true);
                                }
                            }
                            else
                            {
                                message = "ODBC Sage : Le fichier commercial et le fichier comptable pour la source de données système \"" + DSN + "\" n'utilise pas la même base de données Sage !";
                                Log.WriteLog(message, true);
                            }
                        }
                    }
                    else
                    {
                        message = "ODBC Sage : La source de données système \"" + DSN + "\" n'est pas de type \"SAGE Gestion commerciale 100\" ou n'utilise pas le driver ODBC Sage !";
                        Log.WriteLog(message, true);
                    }
                }
                else
                {
                    message = "ODBC Sage : La source de données système \"" + DSN + "\" n'existe pas !";
                    Log.WriteLog(message, true);
                }
            }
            catch (Exception ex)
            {
                message = "DSN ODBC introuvable dans le registre : " + ex.Message;
                Core.Error.SendMailError("DSN ODBC introuvable dans le registre : " + ex.ToString());
            }
            return flag;
        }
        public static ABSTRACTION_SAGE.ALTERNETIS.Connexion GetODBC()
        {
            ABSTRACTION_SAGE.ALTERNETIS.Connexion c = null;
            try
            {
                string message;
                if (VerifyODBCRegistry(out message))
                {
                    c = new ABSTRACTION_SAGE.ALTERNETIS.Connexion(Properties.Settings.Default.SAGEUSER, Properties.Settings.Default.SAGEPASSWORD, Properties.Settings.Default.SAGEDSN);
                }
                if (!string.IsNullOrEmpty(message) && UILaunch)
                {
                    System.Windows.MessageBox.Show(message + "\n\nAucune écriture de données ne sera réalisée dans Sage !", "Connexion ODBC Sage invalide", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid Password") && UILaunch)
                {
                    System.Windows.MessageBox.Show("Impossible d'ouvrir une connexion ODBC Sage : " + "Mot(s) de passe utilisateur Sage invalide(s) !" + "\n\nAucune écriture de données ne sera réalisée dans Sage !", "Connexion ODBC Sage invalide", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
                else if (ex.Message.Contains("Mauvaise version de fichier") && UILaunch)
                {
                    System.Windows.MessageBox.Show("Impossible d'ouvrir une connexion ODBC Sage : " + "Mauvaise version de fichier !" + "\n\nAucune écriture de données ne sera réalisée dans Sage !", "Connexion ODBC Sage invalide", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
                Core.Error.SendMailError(ex.ToString());
            }
            return c;
        }

        // <JG> 04/08/2017
        /// <summary>
        /// GetSempahore
        /// </summary>
        /// <returns>return new Semaphore Object with processor limit defined in configuration</returns>
        public static Semaphore GetSemaphore(out int MaximumThreadCount)
        {
            // si déverrouillage temporaire via l'interface 
            if (Core.Temp.UnlockProcessorCore)
            {
                MaximumThreadCount = GetSemaphoreSystemLimit();
            }
            // limitation définie par l'utilisateur
            else if (Core.Global.GetConfig().ConfigUnlockProcessorCore)
            {
                if (System.Environment.ProcessorCount >= Core.Global.GetConfig().ConfigAllocatedProcessorCore)
                    MaximumThreadCount = Core.Global.GetConfig().ConfigAllocatedProcessorCore;
                else
                    MaximumThreadCount = GetSemaphoreSystemLimit();
            }
            // limitation par le nombre de coeurs logiques du système
            else if (System.Environment.ProcessorCount <= 4)
            {
                MaximumThreadCount = System.Environment.ProcessorCount;
            }
            // limitation par défaut à 4 coeurs logiques (lié à la configuration de MySQL) 
            else
            {
                MaximumThreadCount = 4;
            }
            return new Semaphore(MaximumThreadCount, MaximumThreadCount);
        }
        private static int GetSemaphoreSystemLimit()
        {
            int MaximumThreadCount;
            // utilisation des X coeurs -1 pour éviter la saturation du serveur/poste
            if (System.Environment.ProcessorCount > 2)
                MaximumThreadCount = System.Environment.ProcessorCount - 1;
            // utilisation mono ou bi-coeurs
            else
                MaximumThreadCount = System.Environment.ProcessorCount;
            return MaximumThreadCount;
        }

        public static void LaunchAlternetis_ClearSmartyCache()
        {
            LaunchCron(Core.Global.URL_Prestashop + "/AEC_ClearCache.php");
        }
        public static void LaunchAlternetis_RegenerateCategoryTree()
        {
            LaunchCron(Core.Global.URL_Prestashop + "/AEC_CategoryTree.php");
        }
        public static void LaunchCron(string CronURL)
        {
            LaunchCron(CronURL, 60000);
        }
        public static void LaunchCron(string CronURL, int Timeout)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(CronURL))
                {
                    try
                    {
                        string callurl = System.Windows.Forms.Application.StartupPath + "\\CallURL.exe";
                        if (!System.IO.File.Exists(callurl))
                        {
                            Core.Log.WriteLog("Le programme CallURL.exe n'est pas présent dans le dossier : " + System.Windows.Forms.Application.StartupPath);
                            Core.Log.WriteLog("URL cible : " + CronURL);
                        }
                        else
                        {
                            string arguments = " " + CronURL + " " + Timeout + " " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                            System.Diagnostics.Process.Start(callurl, arguments);
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Log.WriteLog("CallURL:" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public static string SageValueReplacement(string SageValue)
        {
            string r = SageValue;
            if (!string.IsNullOrWhiteSpace(SageValue))
            {
                try
                {
                    Model.Local.ReplacementRepository ReplacementRepository = new Model.Local.ReplacementRepository();
                    if (ReplacementRepository.ExistOrigin(SageValue))
                    {
                        r = ReplacementRepository.ReadOrigin(SageValue).ReplacementText;
                    }
                }
                catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            }
            return r;
        }

        public static bool ExistCustomerFeatureModule()
        {
            string[] tables = { "ps_customer_feature", "ps_customer_feature_customer", "ps_customer_feature_lang", "ps_customer_feature_shop", "ps_customer_feature_value", "ps_customer_feature_value_lang" };
            return ExistModuleTables(tables);
        }
        public static bool ExistOleaPromoModule()
        {
            string[] tables = { "ps_oleapromo" };
            return ExistModuleTables(tables);
        }
        public static bool ExistPreorderModule()
        {
            string[] tables = { "ps_cart_preorder" };
            return ExistModuleTables(tables);
        }
        public static bool ExistAECInvoiceHistoryModule()
        {
            string[] tables = { "ps_aec_invoice_history" };
            return ExistModuleTables(tables);
        }
        public static bool ExistAECStockModule()
        {
            string[] tables = { "ps_aec_stock" };
            return ExistModuleTables(tables);
        }
        public static bool ExistAECCollaborateurModule()
        {
            string[] tables = { "ps_aec_customer_collaborateur" };
            return ExistModuleTables(tables);
        }
        public static bool ExistAECPaiementModule()
        {
            string[] tables = { "ps_aec_customer_payement" };
            return ExistModuleTables(tables);
        }
        public static bool ExistAECCustomerOutstandingModule()
        {
            string[] tables = { "ps_aec_customer_outstanding" };
            return ExistModuleTables(tables);
        }
        public static bool ExistCustomerInfoModule()
        {
            string[] tables = { "ps_customer_info" };
            return ExistModuleTables(tables);
        }
        public static bool ExistAECRepresentativeModule()
        {
            string[] tables = { "ps_aec_representative", "ps_aec_representative_customer", "ps_aec_representative_config" };
            return ExistModuleTables(tables);
        }
        public static bool ExistAECBalanceModule()
        {
            string[] tables = { "ps_aec_balance_accounting", "ps_aec_balance_outstanding", "ps_aec_balance_config" };
            return ExistModuleTables(tables);
        }
        public static bool ExistPortfolioCustomerEmployeeModule()
        {
            string[] tables = { "ps_portfolio_customer_employee" };
            return ExistModuleTables(tables);
        }
        public static bool ExistCartRuleTax()
        {
            string[] columns = { "id_tax" };
            return ExistModuleColumns("ps_order_cart_rule", columns);
        }
        public static bool ExistAECAttributeStatut()
        {
            string[] tables = { "ps_aec_attributelist_attribute" };
            return ExistModuleTables(tables);
        }
        public static bool ExistAECAttributeList()
        {
            string[] tables = { "ps_aec_attributelist", "ps_aec_attributelist_lang", "ps_aec_attributelist_attribute" };
            return ExistModuleTables(tables);
        }
        public static bool ExistSoColissimoDeliveryModule()
        {
            string[] tables = { "ps_so_delivery" };
            return ExistModuleTables(tables);
		}
		public static bool ExistCanActiveDWFProductGuideratesModule()
		{
			string[] tables = { "ps_dwfproductguiderates", "ps_product_guide_rate", "ps_product_guide_rate_lang", "ps_product_guide_rate_shop" };
			return ExistModuleTables(tables);
		}
		public static bool ExistCanActiveDWFProductExtraFieldsModule()
		{
			string[] tables = { "ps_dwfproductextrafields", "ps_dwfproductextrafields_lang", "ps_product_extra_field", "ps_product_extra_field_lang", "ps_product_extra_field_shop" };
			return ExistModuleTables(tables);
		}

		private static bool ExistModuleTables(String[] Tables)
        {
            bool r = false;
            try
            {
                string mysql_request = "show TABLES LIKE '{0}'";

                using (MySql.Data.MySqlClient.MySqlConnection Connection = new MySql.Data.MySqlClient.MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString))
                {
                    try
                    {
                        Connection.Open();
                        using (MySql.Data.MySqlClient.MySqlCommand Command = Connection.CreateCommand())
                        {
                            foreach (string table in Tables)
                            {
                                Command.CommandText = string.Format(mysql_request, table);
                                MySql.Data.MySqlClient.MySqlDataReader reader = Command.ExecuteReader();
                                if (reader.HasRows)
                                {
                                    r = true;
                                }
                                else
                                {
                                    r = false;
                                    break;
                                }
                                reader.Dispose();
                                Command.Dispose();
                            }
                        }
                    }
                    finally
                    {
                        Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return r;
        }
        public static bool ExistModuleColumns(String Table, String[] Columns)
        {
            bool r = false;
            try
            {
                string mysql_request = "show COLUMNS from " + Table + " where field = '{0}'";

                using (MySql.Data.MySqlClient.MySqlConnection Connection = new MySql.Data.MySqlClient.MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString))
                {
                    try
                    {
                        Connection.Open();
                        using (MySql.Data.MySqlClient.MySqlCommand Command = Connection.CreateCommand())
                        {
                            foreach (string champ in Columns)
                            {
                                Command.CommandText = string.Format(mysql_request, champ);
                                MySql.Data.MySqlClient.MySqlDataReader reader = Command.ExecuteReader();
                                if (reader.HasRows)
                                {
                                    r = true;
                                }
                                else
                                {
                                    r = false;
                                    break;
                                }
                                reader.Dispose();
                                Command.Dispose();
                            }
                        }
                    }
                    finally
                    {
                        Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return r;
        }
    }
}
