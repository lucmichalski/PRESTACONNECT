// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from dblinq on 2015-07-15 16:38:42Z.
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

	[Table(Name="ps_aec_balance_accounting")]
	public partial class PsAEcBalanceAccounting : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private decimal _creditAmount;
		
		private System.DateTime _dateAdd;
		
		private System.DateTime _dateTerm;
		
		private decimal _debitAmount;
		
		private string _description;
		
		private uint _idcUstomer;
		
		private uint _idcUstomerAccounting;
		
		private string _invoiceNumber;
		
		private string _lettering;
		
		private string _reference;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnCreditAmountChanged();
		
		partial void OnCreditAmountChanging(decimal value);
		
		partial void OnDateAddChanged();
		
		partial void OnDateAddChanging(System.DateTime value);
		
		partial void OnDateTermChanged();
		
		partial void OnDateTermChanging(System.DateTime value);
		
		partial void OnDebitAmountChanged();
		
		partial void OnDebitAmountChanging(decimal value);
		
		partial void OnDescriptionChanged();
		
		partial void OnDescriptionChanging(string value);
		
		partial void OnIDCustomerChanged();
		
		partial void OnIDCustomerChanging(uint value);
		
		partial void OnIDCustomerAccountingChanged();
		
		partial void OnIDCustomerAccountingChanging(uint value);
		
		partial void OnInvoiceNumberChanged();
		
		partial void OnInvoiceNumberChanging(string value);
		
		partial void OnLetteringChanged();
		
		partial void OnLetteringChanging(string value);
		
		partial void OnReferenceChanged();
		
		partial void OnReferenceChanging(string value);
		#endregion
		
		
		public PsAEcBalanceAccounting()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_creditAmount", Name="credit_amount", DbType="decimal(20,6)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public decimal CreditAmount
		{
			get
			{
				return this._creditAmount;
			}
			set
			{
				if ((_creditAmount != value))
				{
					this.OnCreditAmountChanging(value);
					this.SendPropertyChanging();
					this._creditAmount = value;
					this.SendPropertyChanged("CreditAmount");
					this.OnCreditAmountChanged();
				}
			}
		}
		
		[Column(Storage="_dateAdd", Name="date_add", DbType="datetime", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public System.DateTime DateAdd
		{
			get
			{
				return this._dateAdd;
			}
			set
			{
				if ((_dateAdd != value))
				{
					this.OnDateAddChanging(value);
					this.SendPropertyChanging();
					this._dateAdd = value;
					this.SendPropertyChanged("DateAdd");
					this.OnDateAddChanged();
				}
			}
		}
		
		[Column(Storage="_dateTerm", Name="date_term", DbType="datetime", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public System.DateTime DateTerm
		{
			get
			{
				return this._dateTerm;
			}
			set
			{
				if ((_dateTerm != value))
				{
					this.OnDateTermChanging(value);
					this.SendPropertyChanging();
					this._dateTerm = value;
					this.SendPropertyChanged("DateTerm");
					this.OnDateTermChanged();
				}
			}
		}
		
		[Column(Storage="_debitAmount", Name="debit_amount", DbType="decimal(20,6)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public decimal DebitAmount
		{
			get
			{
				return this._debitAmount;
			}
			set
			{
				if ((_debitAmount != value))
				{
					this.OnDebitAmountChanging(value);
					this.SendPropertyChanging();
					this._debitAmount = value;
					this.SendPropertyChanged("DebitAmount");
					this.OnDebitAmountChanged();
				}
			}
		}
		
		[Column(Storage="_description", Name="description", DbType="varchar(69)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (((_description == value) 
							== false))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
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
		
		[Column(Storage="_idcUstomerAccounting", Name="id_customer_accounting", DbType="int unsigned", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCustomerAccounting
		{
			get
			{
				return this._idcUstomerAccounting;
			}
			set
			{
				if ((_idcUstomerAccounting != value))
				{
					this.OnIDCustomerAccountingChanging(value);
					this.SendPropertyChanging();
					this._idcUstomerAccounting = value;
					this.SendPropertyChanged("IDCustomerAccounting");
					this.OnIDCustomerAccountingChanged();
				}
			}
		}
		
		[Column(Storage="_invoiceNumber", Name="invoice_number", DbType="varchar(13)", AutoSync=AutoSync.Never)]
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
		
		[Column(Storage="_lettering", Name="lettering", DbType="varchar(11)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Lettering
		{
			get
			{
				return this._lettering;
			}
			set
			{
				if (((_lettering == value) 
							== false))
				{
					this.OnLetteringChanging(value);
					this.SendPropertyChanging();
					this._lettering = value;
					this.SendPropertyChanged("Lettering");
					this.OnLetteringChanged();
				}
			}
		}
		
		[Column(Storage="_reference", Name="reference", DbType="varchar(13)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Reference
		{
			get
			{
				return this._reference;
			}
			set
			{
				if (((_reference == value) 
							== false))
				{
					this.OnReferenceChanging(value);
					this.SendPropertyChanging();
					this._reference = value;
					this.SendPropertyChanged("Reference");
					this.OnReferenceChanged();
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
	
	[Table(Name="ps_aec_balance_config")]
	public partial class PsAEcBalanceConFig : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idcUstomer;
		
		private uint _showLettering;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDCustomerChanged();
		
		partial void OnIDCustomerChanging(uint value);
		
		partial void OnShowLetteringChanged();
		
		partial void OnShowLetteringChanging(uint value);
		#endregion
		
		
		public PsAEcBalanceConFig()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_idcUstomer", Name="id_customer", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
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
		
		[Column(Storage="_showLettering", Name="show_lettering", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint ShowLettering
		{
			get
			{
				return this._showLettering;
			}
			set
			{
				if ((_showLettering != value))
				{
					this.OnShowLetteringChanging(value);
					this.SendPropertyChanging();
					this._showLettering = value;
					this.SendPropertyChanged("ShowLettering");
					this.OnShowLetteringChanged();
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
	
	[Table(Name="ps_aec_balance_lang")]
	public partial class PsAEcBalanceLang : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private string _buttonName;
		
		private uint _idlAng;
		
		private string _title;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnButtonNameChanged();
		
		partial void OnButtonNameChanging(string value);
		
		partial void OnIDLangChanged();
		
		partial void OnIDLangChanging(uint value);
		
		partial void OnTitleChanged();
		
		partial void OnTitleChanging(string value);
		#endregion
		
		
		public PsAEcBalanceLang()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_buttonName", Name="button_name", DbType="varchar(255)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string ButtonName
		{
			get
			{
				return this._buttonName;
			}
			set
			{
				if (((_buttonName == value) 
							== false))
				{
					this.OnButtonNameChanging(value);
					this.SendPropertyChanging();
					this._buttonName = value;
					this.SendPropertyChanged("ButtonName");
					this.OnButtonNameChanged();
				}
			}
		}
		
		[Column(Storage="_idlAng", Name="id_lang", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDLang
		{
			get
			{
				return this._idlAng;
			}
			set
			{
				if ((_idlAng != value))
				{
					this.OnIDLangChanging(value);
					this.SendPropertyChanging();
					this._idlAng = value;
					this.SendPropertyChanged("IDLang");
					this.OnIDLangChanged();
				}
			}
		}
		
		[Column(Storage="_title", Name="title", DbType="varchar(255)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (((_title == value) 
							== false))
				{
					this.OnTitleChanging(value);
					this.SendPropertyChanging();
					this._title = value;
					this.SendPropertyChanged("Title");
					this.OnTitleChanged();
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
	
	[Table(Name="ps_aec_balance_outstanding")]
	public partial class PsAEcBalanceOutstanding : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private sbyte _accountLocked;
		
		private uint _idcUstomer;
		
		private decimal _outstandingAllowAmount;
		
		private sbyte _oversee;
		
		private decimal _sageOutstanding;
		
		private decimal _sageWallet;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnAccountLockedChanged();
		
		partial void OnAccountLockedChanging(sbyte value);
		
		partial void OnIDCustomerChanged();
		
		partial void OnIDCustomerChanging(uint value);
		
		partial void OnOutstandingAllowAmountChanged();
		
		partial void OnOutstandingAllowAmountChanging(decimal value);
		
		partial void OnOverseeChanged();
		
		partial void OnOverseeChanging(sbyte value);
		
		partial void OnSageOutstandingChanged();
		
		partial void OnSageOutstandingChanging(decimal value);
		
		partial void OnSageWalletChanged();
		
		partial void OnSageWalletChanging(decimal value);
		#endregion
		
		
		public PsAEcBalanceOutstanding()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_accountLocked", Name="account_locked", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte AccountLocked
		{
			get
			{
				return this._accountLocked;
			}
			set
			{
				if ((_accountLocked != value))
				{
					this.OnAccountLockedChanging(value);
					this.SendPropertyChanging();
					this._accountLocked = value;
					this.SendPropertyChanged("AccountLocked");
					this.OnAccountLockedChanged();
				}
			}
		}
		
		[Column(Storage="_idcUstomer", Name="id_customer", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
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
		
		[Column(Storage="_outstandingAllowAmount", Name="outstanding_allow_amount", DbType="decimal(20,6)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public decimal OutstandingAllowAmount
		{
			get
			{
				return this._outstandingAllowAmount;
			}
			set
			{
				if ((_outstandingAllowAmount != value))
				{
					this.OnOutstandingAllowAmountChanging(value);
					this.SendPropertyChanging();
					this._outstandingAllowAmount = value;
					this.SendPropertyChanged("OutstandingAllowAmount");
					this.OnOutstandingAllowAmountChanged();
				}
			}
		}
		
		[Column(Storage="_oversee", Name="oversee", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte Oversee
		{
			get
			{
				return this._oversee;
			}
			set
			{
				if ((_oversee != value))
				{
					this.OnOverseeChanging(value);
					this.SendPropertyChanging();
					this._oversee = value;
					this.SendPropertyChanged("Oversee");
					this.OnOverseeChanged();
				}
			}
		}
		
		[Column(Storage="_sageOutstanding", Name="sage_outstanding", DbType="decimal(20,6)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public decimal SageOutstanding
		{
			get
			{
				return this._sageOutstanding;
			}
			set
			{
				if ((_sageOutstanding != value))
				{
					this.OnSageOutstandingChanging(value);
					this.SendPropertyChanging();
					this._sageOutstanding = value;
					this.SendPropertyChanged("SageOutstanding");
					this.OnSageOutstandingChanged();
				}
			}
		}
		
		[Column(Storage="_sageWallet", Name="sage_wallet", DbType="decimal(20,6)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public decimal SageWallet
		{
			get
			{
				return this._sageWallet;
			}
			set
			{
				if ((_sageWallet != value))
				{
					this.OnSageWalletChanging(value);
					this.SendPropertyChanging();
					this._sageWallet = value;
					this.SendPropertyChanged("SageWallet");
					this.OnSageWalletChanged();
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
