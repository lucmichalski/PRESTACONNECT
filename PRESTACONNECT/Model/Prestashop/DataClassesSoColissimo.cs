// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from dblinq on 2017-02-17 09:41:07Z.
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
	

	[Table(Name="ps_so_delivery")]
	public partial class PsSoDelivery : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private string _adRessE1;
		
		private string _adRessE2;
		
		private System.Nullable<int> _cartID;
		
		private string _codePostal;
		
		private string _commune;
		
		private string _company;
		
		private int _customerID;
		
		private string _email;
		
		private string _firstName;
		
		private int _id;
		
		private string _inDice;
		
		private string _inFormations;
		
		private string _lastName;
		
		private string _liBelle;
		
		private string _lieuDiT;
		
		private System.Nullable<int> _orderID;
		
		private string _pays;
		
		private int _pointID;
		
		private string _telephone;
		
		private string _type;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnADressE1Changed();
		
		partial void OnADressE1Changing(string value);
		
		partial void OnADressE2Changed();
		
		partial void OnADressE2Changing(string value);
		
		partial void OnCartIDChanged();
		
		partial void OnCartIDChanging(System.Nullable<int> value);
		
		partial void OnCodePostalChanged();
		
		partial void OnCodePostalChanging(string value);
		
		partial void OnCommuneChanged();
		
		partial void OnCommuneChanging(string value);
		
		partial void OnCompanyChanged();
		
		partial void OnCompanyChanging(string value);
		
		partial void OnCustomerIDChanged();
		
		partial void OnCustomerIDChanging(int value);
		
		partial void OnEmailChanged();
		
		partial void OnEmailChanging(string value);
		
		partial void OnFirstNameChanged();
		
		partial void OnFirstNameChanging(string value);
		
		partial void OnIDChanged();
		
		partial void OnIDChanging(int value);
		
		partial void OnInDiceChanged();
		
		partial void OnInDiceChanging(string value);
		
		partial void OnInFormationsChanged();
		
		partial void OnInFormationsChanging(string value);
		
		partial void OnLastNameChanged();
		
		partial void OnLastNameChanging(string value);
		
		partial void OnLiBelleChanged();
		
		partial void OnLiBelleChanging(string value);
		
		partial void OnLieuDiTChanged();
		
		partial void OnLieuDiTChanging(string value);
		
		partial void OnOrderIDChanged();
		
		partial void OnOrderIDChanging(System.Nullable<int> value);
		
		partial void OnPaysChanged();
		
		partial void OnPaysChanging(string value);
		
		partial void OnPointIDChanged();
		
		partial void OnPointIDChanging(int value);
		
		partial void OnTelephoneChanged();
		
		partial void OnTelephoneChanging(string value);
		
		partial void OnTypeChanged();
		
		partial void OnTypeChanging(string value);
		#endregion
		
		
		public PsSoDelivery()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_adRessE1", Name="adresse1", DbType="varchar(38)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string ADressE1
		{
			get
			{
				return this._adRessE1;
			}
			set
			{
				if (((_adRessE1 == value) 
							== false))
				{
					this.OnADressE1Changing(value);
					this.SendPropertyChanging();
					this._adRessE1 = value;
					this.SendPropertyChanged("ADressE1");
					this.OnADressE1Changed();
				}
			}
		}
		
		[Column(Storage="_adRessE2", Name="adresse2", DbType="varchar(38)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string ADressE2
		{
			get
			{
				return this._adRessE2;
			}
			set
			{
				if (((_adRessE2 == value) 
							== false))
				{
					this.OnADressE2Changing(value);
					this.SendPropertyChanging();
					this._adRessE2 = value;
					this.SendPropertyChanged("ADressE2");
					this.OnADressE2Changed();
				}
			}
		}
		
		[Column(Storage="_cartID", Name="cart_id", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> CartID
		{
			get
			{
				return this._cartID;
			}
			set
			{
				if ((_cartID != value))
				{
					this.OnCartIDChanging(value);
					this.SendPropertyChanging();
					this._cartID = value;
					this.SendPropertyChanged("CartID");
					this.OnCartIDChanged();
				}
			}
		}
		
		[Column(Storage="_codePostal", Name="code_postal", DbType="varchar(5)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string CodePostal
		{
			get
			{
				return this._codePostal;
			}
			set
			{
				if (((_codePostal == value) 
							== false))
				{
					this.OnCodePostalChanging(value);
					this.SendPropertyChanging();
					this._codePostal = value;
					this.SendPropertyChanged("CodePostal");
					this.OnCodePostalChanged();
				}
			}
		}
		
		[Column(Storage="_commune", Name="commune", DbType="varchar(32)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Commune
		{
			get
			{
				return this._commune;
			}
			set
			{
				if (((_commune == value) 
							== false))
				{
					this.OnCommuneChanging(value);
					this.SendPropertyChanging();
					this._commune = value;
					this.SendPropertyChanged("Commune");
					this.OnCommuneChanged();
				}
			}
		}
		
		[Column(Storage="_company", Name="company", DbType="varchar(38)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Company
		{
			get
			{
				return this._company;
			}
			set
			{
				if (((_company == value) 
							== false))
				{
					this.OnCompanyChanging(value);
					this.SendPropertyChanging();
					this._company = value;
					this.SendPropertyChanged("Company");
					this.OnCompanyChanged();
				}
			}
		}
		
		[Column(Storage="_customerID", Name="customer_id", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int CustomerID
		{
			get
			{
				return this._customerID;
			}
			set
			{
				if ((_customerID != value))
				{
					this.OnCustomerIDChanging(value);
					this.SendPropertyChanging();
					this._customerID = value;
					this.SendPropertyChanged("CustomerID");
					this.OnCustomerIDChanged();
				}
			}
		}
		
		[Column(Storage="_email", Name="email", DbType="varchar(64)", AutoSync=AutoSync.Never)]
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
		
		[Column(Storage="_firstName", Name="firstname", DbType="varchar(38)", AutoSync=AutoSync.Never)]
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
		
		[Column(Storage="_id", Name="id", DbType="int", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int ID
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((_id != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[Column(Storage="_inDice", Name="indice", DbType="varchar(70)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string InDice
		{
			get
			{
				return this._inDice;
			}
			set
			{
				if (((_inDice == value) 
							== false))
				{
					this.OnInDiceChanging(value);
					this.SendPropertyChanging();
					this._inDice = value;
					this.SendPropertyChanged("InDice");
					this.OnInDiceChanged();
				}
			}
		}
		
		[Column(Storage="_inFormations", Name="informations", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string InFormations
		{
			get
			{
				return this._inFormations;
			}
			set
			{
				if (((_inFormations == value) 
							== false))
				{
					this.OnInFormationsChanging(value);
					this.SendPropertyChanging();
					this._inFormations = value;
					this.SendPropertyChanged("InFormations");
					this.OnInFormationsChanged();
				}
			}
		}
		
		[Column(Storage="_lastName", Name="lastname", DbType="varchar(38)", AutoSync=AutoSync.Never)]
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
		
		[Column(Storage="_liBelle", Name="libelle", DbType="varchar(50)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string LiBelle
		{
			get
			{
				return this._liBelle;
			}
			set
			{
				if (((_liBelle == value) 
							== false))
				{
					this.OnLiBelleChanging(value);
					this.SendPropertyChanging();
					this._liBelle = value;
					this.SendPropertyChanged("LiBelle");
					this.OnLiBelleChanged();
				}
			}
		}
		
		[Column(Storage="_lieuDiT", Name="lieudit", DbType="varchar(38)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string LieuDiT
		{
			get
			{
				return this._lieuDiT;
			}
			set
			{
				if (((_lieuDiT == value) 
							== false))
				{
					this.OnLieuDiTChanging(value);
					this.SendPropertyChanging();
					this._lieuDiT = value;
					this.SendPropertyChanged("LieuDiT");
					this.OnLieuDiTChanged();
				}
			}
		}
		
		[Column(Storage="_orderID", Name="order_id", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> OrderID
		{
			get
			{
				return this._orderID;
			}
			set
			{
				if ((_orderID != value))
				{
					this.OnOrderIDChanging(value);
					this.SendPropertyChanging();
					this._orderID = value;
					this.SendPropertyChanged("OrderID");
					this.OnOrderIDChanged();
				}
			}
		}
		
		[Column(Storage="_pays", Name="pays", DbType="varchar(32)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string Pays
		{
			get
			{
				return this._pays;
			}
			set
			{
				if (((_pays == value) 
							== false))
				{
					this.OnPaysChanging(value);
					this.SendPropertyChanging();
					this._pays = value;
					this.SendPropertyChanged("Pays");
					this.OnPaysChanged();
				}
			}
		}
		
		[Column(Storage="_pointID", Name="point_id", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int PointID
		{
			get
			{
				return this._pointID;
			}
			set
			{
				if ((_pointID != value))
				{
					this.OnPointIDChanging(value);
					this.SendPropertyChanging();
					this._pointID = value;
					this.SendPropertyChanged("PointID");
					this.OnPointIDChanged();
				}
			}
		}
		
		[Column(Storage="_telephone", Name="telephone", DbType="varchar(10)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Telephone
		{
			get
			{
				return this._telephone;
			}
			set
			{
				if (((_telephone == value) 
							== false))
				{
					this.OnTelephoneChanging(value);
					this.SendPropertyChanging();
					this._telephone = value;
					this.SendPropertyChanged("Telephone");
					this.OnTelephoneChanged();
				}
			}
		}
		
		[Column(Storage="_type", Name="type", DbType="varchar(3)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (((_type == value) 
							== false))
				{
					this.OnTypeChanging(value);
					this.SendPropertyChanging();
					this._type = value;
					this.SendPropertyChanged("Type");
					this.OnTypeChanged();
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
