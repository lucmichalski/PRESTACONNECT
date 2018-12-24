using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.Model.Sage
{
    public class cbSysLibreRepository
    {
        public enum CB_File
        {
            F_ARTICLE,
            F_DOCENTETE,
            F_COMPTET,
            F_DOCLIGNE,
            F_ECRITUREC
        }
        public enum CB_Type
        {
            Deleted = 0,
            SageSmallDate = 3,
            SageValeur = 7,
            SageText = 9,
            SageDate = 14,
            SageMontant = 20,
            SageTable = 22
        }

        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();


        public ObservableCollection<cbSysLibre> List()
        {
            return new ObservableCollection<cbSysLibre>(this.DBSage.cbSysLibre.ToList());
        }

        public ObservableCollection<cbSysLibre> ListFileOrderByPosition(CB_File File)
        {
            IQueryable<cbSysLibre> Return = from Table in this.DBSage.cbSysLibre
                                            where Table.CB_File.ToUpper() == File.ToString().ToUpper()
                                            && (Table.CB_Type == (short)CB_Type.SageText
                                                || Table.CB_Type == (short)CB_Type.SageTable
                                                || Table.CB_Type == (short)CB_Type.SageDate
                                                || Table.CB_Type == (short)CB_Type.SageMontant
                                                || Table.CB_Type == (short)CB_Type.SageSmallDate
                                                || Table.CB_Type == (short)CB_Type.SageValeur)
                                            orderby Table.CB_Pos
                                            select Table;
            return new ObservableCollection<cbSysLibre>(Return.ToList());
        }
        public ObservableCollection<cbSysLibre> ListFileOrderByName(CB_File File)
        {
            IQueryable<cbSysLibre> Return = from Table in this.DBSage.cbSysLibre
                                            where Table.CB_File.ToUpper() == File.ToString().ToUpper()
                                            && (Table.CB_Type == (short)CB_Type.SageText
                                                || Table.CB_Type == (short)CB_Type.SageTable
                                                || Table.CB_Type == (short)CB_Type.SageDate
                                                || Table.CB_Type == (short)CB_Type.SageMontant
                                                || Table.CB_Type == (short)CB_Type.SageSmallDate
                                                || Table.CB_Type == (short)CB_Type.SageValeur)
                                            orderby Table.CB_Name
                                            select Table;
            return new ObservableCollection<cbSysLibre>(Return.ToList());
        }

        public Boolean ExistInformationLibre(String Name, CB_File File)
        {
            return this.DBSage.cbSysLibre.Count(i => i.CB_Name == Name && i.CB_File.ToUpper() == File.ToString().ToUpper()) == 1;
        }

        public cbSysLibre ReadInformationLibre(String Name, CB_File File)
        {
            return this.DBSage.cbSysLibre.FirstOrDefault(i => i.CB_Name == Name && i.CB_File.ToUpper() == File.ToString().ToUpper());
        }

        public CB_Type ReadTypeInformationLibre(String Name, CB_File File)
        {
            CB_Type Type = CB_Type.Deleted;

            if (ExistInformationLibre(Name, File))
                Type = (CB_Type)this.DBSage.cbSysLibre.First(i => i.CB_Name == Name && i.CB_File.ToUpper() == File.ToString().ToUpper()).CB_Type;

            return Type;
        }
    }
}
