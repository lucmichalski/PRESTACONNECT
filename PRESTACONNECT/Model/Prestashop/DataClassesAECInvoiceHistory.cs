// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from dblinq on 2014-05-06 17:56:04Z.
// Please visit http://code.google.com/p/dblinq2007/ for more information.
//
namespace PRESTACONNECT.Model.Prestashop
{
	using System;
	using System.ComponentModel;
	using System.Data;
#if MONO_STRICT
	using System.Data.Linq;
#else   // MONO_STRICT
	using DbLinq.Data.Linq;
	using DbLinq.Vendor;
#endif  // MONO_STRICT
	using System.Data.Linq.Mapping;
	using System.Diagnostics;
	
	
	[Table(Name="ps_aec_invoice_history")]
	public partial class PsAEcInvoiceHistory : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private string _file;
		
		private string _fileName;
		
		private uint _idcUstomer;
		
		private uint _idiNvoiceHistory;
		
		private System.DateTime _invoiceDate;
		
		private string _invoiceNumber;
		
		private decimal _totalAmountTaxExCl;
		
		private decimal _totalAmountTaxInCl;

        private uint _typeDocument;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnFileChanged();
		
		partial void OnFileChanging(string value);
		
		partial void OnFileNameChanged();
		
		partial void OnFileNameChanging(string value);
		
		partial void OnIDCustomerChanged();
		
		partial void OnIDCustomerChanging(uint value);
		
		partial void OnIDInvoiceHistoryChanged();
		
		partial void OnIDInvoiceHistoryChanging(uint value);
		
		partial void OnInvoiceDateChanged();
		
		partial void OnInvoiceDateChanging(System.DateTime value);
		
		partial void OnInvoiceNumberChanged();
		
		partial void OnInvoiceNumberChanging(string value);
		
		partial void OnTotalAmountTaxExClChanged();
		
		partial void OnTotalAmountTaxExClChanging(decimal value);
		
		partial void OnTotalAmountTaxInClChanged();

        partial void OnTotalAmountTaxInClChanging(decimal value);

        partial void OnTypeDocumentChanged();

        partial void OnTypeDocumentChanging(decimal value);
		#endregion
		
		
		public PsAEcInvoiceHistory()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_file", Name="file", DbType="varchar(255)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string File
		{
			get
			{
				return this._file;
			}
			set
			{
				if (((_file == value) 
							== false))
				{
					this.OnFileChanging(value);
					this.SendPropertyChanging();
					this._file = value;
					this.SendPropertyChanged("File");
					this.OnFileChanged();
				}
			}
		}
		
		[Column(Storage="_fileName", Name="file_name", DbType="varchar(128)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string FileName
		{
			get
			{
				return this._fileName;
			}
			set
			{
				if (((_fileName == value) 
							== false))
				{
					this.OnFileNameChanging(value);
					this.SendPropertyChanging();
					this._fileName = value;
					this.SendPropertyChanged("FileName");
					this.OnFileNameChanged();
				}
			}
		}
		
		[Column(Storage="_idcUstomer", Name="id_customer", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCustomer
		{
			get
			{
				return this._idcUstomer;
			}
			set
			{
				if ((_idcUstomer != value))
				{
					this.OnIDCustomerChanging(value);
					this.SendPropertyChanging();
					this._idcUstomer = value;
					this.SendPropertyChanged("IDCustomer");
					this.OnIDCustomerChanged();
				}
			}
		}
		
		[Column(Storage="_idiNvoiceHistory", Name="id_invoice_history", DbType="int unsigned", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDInvoiceHistory
		{
			get
			{
				return this._idiNvoiceHistory;
			}
			set
			{
				if ((_idiNvoiceHistory != value))
				{
					this.OnIDInvoiceHistoryChanging(value);
					this.SendPropertyChanging();
					this._idiNvoiceHistory = value;
					this.SendPropertyChanged("IDInvoiceHistory");
					this.OnIDInvoiceHistoryChanged();
				}
			}
		}
		
		[Column(Storage="_invoiceDate", Name="invoice_date", DbType="datetime", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public System.DateTime InvoiceDate
		{
			get
			{
				return this._invoiceDate;
			}
			set
			{
				if ((_invoiceDate != value))
				{
					this.OnInvoiceDateChanging(value);
					this.SendPropertyChanging();
					this._invoiceDate = value;
					this.SendPropertyChanged("InvoiceDate");
					this.OnInvoiceDateChanged();
				}
			}
		}
		
		[Column(Storage="_invoiceNumber", Name="invoice_number", DbType="varchar(10)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string InvoiceNumber
		{
			get
			{
				return this._invoiceNumber;
			}
			set
			{
				if (((_invoiceNumber == value) 
							== false))
				{
					this.OnInvoiceNumberChanging(value);
					this.SendPropertyChanging();
					this._invoiceNumber = value;
					this.SendPropertyChanged("InvoiceNumber");
					this.OnInvoiceNumberChanged();
				}
			}
		}
		
		[Column(Storage="_totalAmountTaxExCl", Name="total_amount_tax_excl", DbType="decimal(17,2)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public decimal TotalAmountTaxExCl
		{
			get
			{
				return this._totalAmountTaxExCl;
			}
			set
			{
				if ((_totalAmountTaxExCl != value))
				{
					this.OnTotalAmountTaxExClChanging(value);
					this.SendPropertyChanging();
					this._totalAmountTaxExCl = value;
					this.SendPropertyChanged("TotalAmountTaxExCl");
					this.OnTotalAmountTaxExClChanged();
				}
			}
		}
		
		[Column(Storage="_totalAmountTaxInCl", Name="total_amount_tax_incl", DbType="decimal(17,2)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public decimal TotalAmountTaxInCl
		{
			get
			{
				return this._totalAmountTaxInCl;
			}
			set
			{
				if ((_totalAmountTaxInCl != value))
				{
					this.OnTotalAmountTaxInClChanging(value);
					this.SendPropertyChanging();
					this._totalAmountTaxInCl = value;
					this.SendPropertyChanged("TotalAmountTaxInCl");
					this.OnTotalAmountTaxInClChanged();
				}
			}
		}

        [Column(Storage = "_typeDocument", Name = "type_document", DbType = "int unsigned", AutoSync = AutoSync.Never, CanBeNull = false)]
        [DebuggerNonUserCode()]
        public uint TypeDocument
        {
            get
            {
                return this._typeDocument;
            }
            set
            {
                if ((_typeDocument != value))
                {
                    this.OnTypeDocumentChanging(value);
                    this.SendPropertyChanging();
                    this._typeDocument = value;
                    this.SendPropertyChanged("TypeDocument");
                    this.OnTypeDocumentChanged();
                }
            }
        }
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
