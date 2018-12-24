// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from dblinq on 2015-07-15 16:37:36Z.
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

	[Table(Name="ps_aec_representative")]
	public partial class PsAEcRepresentative : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private string _email;
		
		private string _fax;
		
		private string _firstName;
		
		private uint _idrEpresentative;
		
		private uint _idsAge;
		
		private string _lastName;
		
		private string _mobile;
		
		private string _phone;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnEmailChanged();
		
		partial void OnEmailChanging(string value);
		
		partial void OnFaxChanged();
		
		partial void OnFaxChanging(string value);
		
		partial void OnFirstNameChanged();
		
		partial void OnFirstNameChanging(string value);
		
		partial void OnIDRepresentativeChanged();
		
		partial void OnIDRepresentativeChanging(uint value);
		
		partial void OnIDSageChanged();
		
		partial void OnIDSageChanging(uint value);
		
		partial void OnLastNameChanged();
		
		partial void OnLastNameChanging(string value);
		
		partial void OnMobileChanged();
		
		partial void OnMobileChanging(string value);
		
		partial void OnPhoneChanged();
		
		partial void OnPhoneChanging(string value);
		#endregion
		
		
		public PsAEcRepresentative()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_email", Name="email", DbType="varchar(128)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Email
		{
			get
			{
				return this._email;
			}
			set
			{
				if (((_email == value) 
							== false))
				{
					this.OnEmailChanging(value);
					this.SendPropertyChanging();
					this._email = value;
					this.SendPropertyChanged("Email");
					this.OnEmailChanged();
				}
			}
		}
		
		[Column(Storage="_fax", Name="fax", DbType="varchar(21)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Fax
		{
			get
			{
				return this._fax;
			}
			set
			{
				if (((_fax == value) 
							== false))
				{
					this.OnFaxChanging(value);
					this.SendPropertyChanging();
					this._fax = value;
					this.SendPropertyChanged("Fax");
					this.OnFaxChanged();
				}
			}
		}
		
		[Column(Storage="_firstName", Name="firstname", DbType="varchar(35)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string FirstName
		{
			get
			{
				return this._firstName;
			}
			set
			{
				if (((_firstName == value) 
							== false))
				{
					this.OnFirstNameChanging(value);
					this.SendPropertyChanging();
					this._firstName = value;
					this.SendPropertyChanged("FirstName");
					this.OnFirstNameChanged();
				}
			}
		}
		
		[Column(Storage="_idrEpresentative", Name="id_representative", DbType="int unsigned", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDRepresentative
		{
			get
			{
				return this._idrEpresentative;
			}
			set
			{
				if ((_idrEpresentative != value))
				{
					this.OnIDRepresentativeChanging(value);
					this.SendPropertyChanging();
					this._idrEpresentative = value;
					this.SendPropertyChanged("IDRepresentative");
					this.OnIDRepresentativeChanged();
				}
			}
		}
		
		[Column(Storage="_idsAge", Name="id_sage", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDSage
		{
			get
			{
				return this._idsAge;
			}
			set
			{
				if ((_idsAge != value))
				{
					this.OnIDSageChanging(value);
					this.SendPropertyChanging();
					this._idsAge = value;
					this.SendPropertyChanged("IDSage");
					this.OnIDSageChanged();
				}
			}
		}
		
		[Column(Storage="_lastName", Name="lastname", DbType="varchar(35)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string LastName
		{
			get
			{
				return this._lastName;
			}
			set
			{
				if (((_lastName == value) 
							== false))
				{
					this.OnLastNameChanging(value);
					this.SendPropertyChanging();
					this._lastName = value;
					this.SendPropertyChanged("LastName");
					this.OnLastNameChanged();
				}
			}
		}
		
		[Column(Storage="_mobile", Name="mobile", DbType="varchar(21)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Mobile
		{
			get
			{
				return this._mobile;
			}
			set
			{
				if (((_mobile == value) 
							== false))
				{
					this.OnMobileChanging(value);
					this.SendPropertyChanging();
					this._mobile = value;
					this.SendPropertyChanged("Mobile");
					this.OnMobileChanged();
				}
			}
		}
		
		[Column(Storage="_phone", Name="phone", DbType="varchar(21)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Phone
		{
			get
			{
				return this._phone;
			}
			set
			{
				if (((_phone == value) 
							== false))
				{
					this.OnPhoneChanging(value);
					this.SendPropertyChanging();
					this._phone = value;
					this.SendPropertyChanged("Phone");
					this.OnPhoneChanged();
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
	
	[Table(Name="ps_aec_representative_config")]
	public partial class PsAEcRepresentativeConFig : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
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
		
		
		public PsAEcRepresentativeConFig()
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
	
	[Table(Name="ps_aec_representative_customer")]
	public partial class PsAEcRepresentativeCustomer : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idcUstomer;
		
		private uint _idrEpresentative;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDCustomerChanged();
		
		partial void OnIDCustomerChanging(uint value);
		
		partial void OnIDRepresentativeChanged();
		
		partial void OnIDRepresentativeChanging(uint value);
		#endregion
		
		
		public PsAEcRepresentativeCustomer()
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
		
		[Column(Storage="_idrEpresentative", Name="id_representative", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDRepresentative
		{
			get
			{
				return this._idrEpresentative;
			}
			set
			{
				if ((_idrEpresentative != value))
				{
					this.OnIDRepresentativeChanging(value);
					this.SendPropertyChanging();
					this._idrEpresentative = value;
					this.SendPropertyChanged("IDRepresentative");
					this.OnIDRepresentativeChanged();
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
