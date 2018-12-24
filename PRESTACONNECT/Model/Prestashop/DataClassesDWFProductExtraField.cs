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
	
	
	[Table(Name= "ps_dwfproductextrafields")]
	public partial class PsDWFProductExtraField : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idDWFProductExtraFields;
		
		private string _fieldName;

		private string _type;

		//private string _config;

		private string _location;

		private uint _position;

		private uint _active;

		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIdDWFProductExtraFieldsChanged();
		
		partial void OnIdDWFProductExtraFieldsChanging(uint value);
		
		partial void OnFieldNameChanged();
		
		partial void OnFieldNameChanging(string value);

		partial void OnTypeChanged();

		partial void OnTypeChanging(string value);

		//partial void OnConfigChanged();

		//partial void OnConfigChanging(string value);

		partial void OnLocationChanged();

		partial void OnLocationChanging(string value);

		partial void OnPositionChanged();

		partial void OnPositionChanging(uint value);

		partial void OnActiveChanged();

		partial void OnActiveChanging(uint value);
		#endregion


		public PsDWFProductExtraField()
		{
			this.OnCreated();
		}
		
		[Column(Storage= "_idDWFProductExtraFields", Name= "id_dwfproductextrafields", DbType= "int unsigned", IsPrimaryKey = true, IsDbGenerated = true, AutoSync =AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IdDWFProductExtraFields
		{
			get
			{
				return this._idDWFProductExtraFields;
			}
			set
			{
				if (((_idDWFProductExtraFields == value) 
							== false))
				{
					this.OnIdDWFProductExtraFieldsChanging(value);
					this.SendPropertyChanging();
					this._idDWFProductExtraFields = value;
					this.SendPropertyChanged("IdDWFProductExtraFields");
					this.OnIdDWFProductExtraFieldsChanged();
				}
			}
		}
		
		[Column(Storage= "_fieldName", Name="fieldname", DbType="varchar(255)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string FieldName
		{
			get
			{
				return this._fieldName;
			}
			set
			{
				if (((_fieldName == value) 
							== false))
				{
					this.OnFieldNameChanging(value);
					this.SendPropertyChanging();
					this._fieldName = value;
					this.SendPropertyChanged("FieldName");
					this.OnFieldNameChanged();
				}
			}
		}

		[Column(Storage = "_type", Name = "type", DbType = "varchar(255)", AutoSync = AutoSync.Never, CanBeNull = false)]
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

		/*[Column(Storage = "_config", Name = "config", DbType = "text", AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public string Config
		{
			get
			{
				return this._config;
			}
			set
			{
				if (((_config == value)
							== false))
				{
					this.OnConfigChanging(value);
					this.SendPropertyChanging();
					this._config = value;
					this.SendPropertyChanged("Config");
					this.OnConfigChanged();
				}
			}
		}*/

		[Column(Storage = "_location", Name = "location", DbType = "varchar(255)", AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public string Location
		{
			get
			{
				return this._location;
			}
			set
			{
				if (((_location == value)
							== false))
				{
					this.OnLocationChanging(value);
					this.SendPropertyChanging();
					this._location = value;
					this.SendPropertyChanged("Location");
					this.OnLocationChanged();
				}
			}
		}

		[Column(Storage = "_position", Name = "position", DbType = "int unsigned", AutoSync = AutoSync.Never, CanBeNull = false)]
        [DebuggerNonUserCode()]
        public uint Position
		{
            get
            {
                return this._position;
            }
            set
            {
                if ((_position != value))
                {
                    this.OnPositionChanging(value);
                    this.SendPropertyChanging();
                    this._position = value;
                    this.SendPropertyChanged("Position");
                    this.OnPositionChanged();
                }
            }
        }

		[Column(Storage = "_active", Name = "active", DbType = "tinyint(1) unsigned", AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint Active
		{
			get
			{
				return this._active;
			}
			set
			{
				if ((_active != value))
				{
					this.OnActiveChanging(value);
					this.SendPropertyChanging();
					this._active = value;
					this.SendPropertyChanged("Active");
					this.OnActiveChanged();
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
	

	[Table(Name = "ps_dwfproductextrafields_lang")]
	public partial class PsDWFProductExtraFieldsLang : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{

		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");

		private uint _idDWFProductExtraFields;

		private uint _idLang;

		private string _label;

		#region Extensibility Method Declarations
		partial void OnCreated();

		partial void OnIdDWFProductExtraFieldsChanged();

		partial void OnIdDWFProductExtraFieldsChanging(uint value);

		partial void OnIdLangChanged();

		partial void OnIdLangChanging(uint value);

		partial void OnLabelChanged();

		partial void OnLabelChanging(string value);
		#endregion


		public PsDWFProductExtraFieldsLang()
		{
			this.OnCreated();
		}

		[Column(Storage = "_idDWFProductExtraFields", Name = "id_dwfproductextrafields", DbType = "int unsigned", IsPrimaryKey = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdDWFProductExtraFields
		{
			get
			{
				return this._idDWFProductExtraFields;
			}
			set
			{
				if (((_idDWFProductExtraFields == value)
							== false))
				{
					this.OnIdDWFProductExtraFieldsChanging(value);
					this.SendPropertyChanging();
					this._idDWFProductExtraFields = value;
					this.SendPropertyChanged("IdDWFProductExtraFields");
					this.OnIdDWFProductExtraFieldsChanged();
				}
			}
		}

		[Column(Storage = "_idLang", Name = "id_lang", DbType = "int unsigned", IsPrimaryKey = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdLang
		{
			get
			{
				return this._idLang;
			}
			set
			{
				if (((_idLang == value)
							== false))
				{
					this.OnIdLangChanging(value);
					this.SendPropertyChanging();
					this._idLang = value;
					this.SendPropertyChanged("IdLang");
					this.OnIdLangChanged();
				}
			}
		}

		[Column(Storage = "_label", Name = "label", DbType = "varchar(255)", AutoSync = AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Label
		{
			get
			{
				return this._label;
			}
			set
			{
				if (((_label == value)
							== false))
				{
					this.OnLabelChanging(value);
					this.SendPropertyChanging();
					this._label = value;
					this.SendPropertyChanged("Label");
					this.OnLabelChanged();
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


	[Table(Name = "ps_product_extra_field")]
	public partial class PsProductExtraField : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{

		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");

		private uint _idProductExtraField;

		private uint _idShopDefault;

		private uint _idProduct;

		private System.DateTime _dateAdd;

		private System.DateTime _dateUpd;

		#region Extensibility Method Declarations
		partial void OnCreated();

		partial void OnIdProductExtraFieldChanged();

		partial void OnIdProductExtraFieldChanging(uint value);

		partial void OnIdShopDefaultChanged();

		partial void OnIdShopDefaultChanging(uint value);

		partial void OnIdProductChanged();

		partial void OnIdProductChanging(uint value);

		partial void OnDateAddDateChanged();

		partial void OnDateAddChanging(System.DateTime value);

		partial void OnDateUpdChanged();

		partial void OnDateUpdChanging(System.DateTime value);
		#endregion


		public PsProductExtraField()
		{
			this.OnCreated();
		}

		[Column(Storage = "_idProductExtraField", Name = "id_product_extra_field", DbType = "int unsigned", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdProductExtraField
		{
			get
			{
				return this._idProductExtraField;
			}
			set
			{
				if (((_idProductExtraField == value)
							== false))
				{
					this.OnIdProductExtraFieldChanging(value);
					this.SendPropertyChanging();
					this._idProductExtraField = value;
					this.SendPropertyChanged("IdProductExtraField");
					this.OnIdProductExtraFieldChanged();
				}
			}
		}

		[Column(Storage = "_idShopDefault", Name = "id_shop_default", DbType = "int unsigned", AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdShopDefault
		{
			get
			{
				return this._idShopDefault;
			}
			set
			{
				if (((_idShopDefault == value)
							== false))
				{
					this.OnIdShopDefaultChanging(value);
					this.SendPropertyChanging();
					this._idShopDefault = value;
					this.SendPropertyChanged("IdShopDefault");
					this.OnIdShopDefaultChanged();
				}
			}
		}

		[Column(Storage = "_idProduct", Name = "id_product", DbType = "int unsigned", AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdProduct
		{
			get
			{
				return this._idProduct;
			}
			set
			{
				if (((_idProduct == value)
							== false))
				{
					this.OnIdProductChanging(value);
					this.SendPropertyChanging();
					this._idProduct = value;
					this.SendPropertyChanged("IdProduct");
					this.OnIdProductChanged();
				}
			}
		}

		[Column(Storage = "_dateAdd", Name = "date_add", DbType = "datetime", AutoSync = AutoSync.Never, CanBeNull = false)]
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
					this.OnDateAddDateChanged();
				}
			}
		}

		[Column(Storage = "_dateUpd", Name = "date_upd", DbType = "datetime", AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public System.DateTime DateUpd
		{
			get
			{
				return this._dateUpd;
			}
			set
			{
				if ((_dateUpd != value))
				{
					this.OnDateUpdChanging(value);
					this.SendPropertyChanging();
					this._dateUpd = value;
					this.SendPropertyChanged("DateUpd");
					this.OnDateUpdChanged();
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


	[Table(Name = "ps_product_extra_field_lang")]
	public partial class PsProductExtraFieldLang : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{

		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");

		private uint _idProductExtraField;

		private uint _idShop;

		private uint _idLang;

		#region Extensibility Method Declarations
		partial void OnCreated();

		partial void OnIdProductExtraFieldChanged();

		partial void OnIdProductExtraFieldChanging(uint value);

		partial void OnIdShopChanged();

		partial void OnIdShopChanging(uint value);

		partial void OnIdLangChanged();

		partial void OnIdLangChanging(uint value);
		#endregion


		public PsProductExtraFieldLang()
		{
			this.OnCreated();
		}

		[Column(Storage = "_idProductExtraField", Name = "id_product_extra_field", DbType = "int unsigned", IsPrimaryKey = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdProductExtraField
		{
			get
			{
				return this._idProductExtraField;
			}
			set
			{
				if (((_idProductExtraField == value)
							== false))
				{
					this.OnIdProductExtraFieldChanging(value);
					this.SendPropertyChanging();
					this._idProductExtraField = value;
					this.SendPropertyChanged("IdProductExtraField");
					this.OnIdProductExtraFieldChanged();
				}
			}
		}

		[Column(Storage = "_idShop", Name = "id_shop", DbType = "int unsigned", IsPrimaryKey = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdShop
		{
			get
			{
				return this._idShop;
			}
			set
			{
				if (((_idShop == value)
							== false))
				{
					this.OnIdShopChanging(value);
					this.SendPropertyChanging();
					this._idShop = value;
					this.SendPropertyChanged("IdShop");
					this.OnIdShopChanged();
				}
			}
		}

		[Column(Storage = "_idLang", Name = "id_lang", DbType = "int unsigned", IsPrimaryKey = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdLang
		{
			get
			{
				return this._idLang;
			}
			set
			{
				if (((_idLang == value)
							== false))
				{
					this.OnIdLangChanging(value);
					this.SendPropertyChanging();
					this._idLang = value;
					this.SendPropertyChanged("IdLang");
					this.OnIdLangChanged();
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


	[Table(Name = "ps_product_extra_field_shop")]
	public partial class PsProductExtraFieldShop : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{

		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");

		private uint _idProductExtraField;

		private uint _idShop;

		#region Extensibility Method Declarations
		partial void OnCreated();

		partial void OnIdProductExtraFieldChanged();

		partial void OnIdProductExtraFieldChanging(uint value);

		partial void OnIdShopChanged();

		partial void OnIdShopChanging(uint value);
		#endregion


		public PsProductExtraFieldShop()
		{
			this.OnCreated();
		}

		[Column(Storage = "_idProductExtraField", Name = "id_product_extra_field", DbType = "int unsigned", IsPrimaryKey = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdProductExtraField
		{
			get
			{
				return this._idProductExtraField;
			}
			set
			{
				if (((_idProductExtraField == value)
							== false))
				{
					this.OnIdProductExtraFieldChanging(value);
					this.SendPropertyChanging();
					this._idProductExtraField = value;
					this.SendPropertyChanged("IdProductExtraField");
					this.OnIdProductExtraFieldChanged();
				}
			}
		}

		[Column(Storage = "_idShop", Name = "id_shop", DbType = "int unsigned", IsPrimaryKey = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdShop
		{
			get
			{
				return this._idShop;
			}
			set
			{
				if (((_idShop == value)
							== false))
				{
					this.OnIdShopChanging(value);
					this.SendPropertyChanging();
					this._idShop = value;
					this.SendPropertyChanged("IdShop");
					this.OnIdShopChanged();
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
