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
	
	
	[Table(Name= "ps_dwfproductguiderates")]
	public partial class PsDWFProductGuiderates : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idDWFProductGuiderates;
		
		private string _fileName;

		private string _name;

		private uint _position;

		private uint _active;

		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIdDWFProductGuideratesChanged();
		
		partial void OnIdDWFProductGuideratesChanging(uint value);
		
		partial void OnFileNameChanged();
		
		partial void OnFileNameChanging(string value);

		partial void OnNameChanged();

		partial void OnNameChanging(string value);

		partial void OnPositionChanged();

		partial void OnPositionChanging(uint value);

		partial void OnActiveChanged();

		partial void OnActiveChanging(uint value);
		#endregion


		public PsDWFProductGuiderates()
		{
			this.OnCreated();
		}
		
		[Column(Storage= "_idDWFProductGuiderates", Name= "id_dwfproductguiderates", DbType= "int unsigned", IsPrimaryKey = true, IsDbGenerated = true, AutoSync =AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IdDWFProductGuiderates
		{
			get
			{
				return this._idDWFProductGuiderates;
			}
			set
			{
				if (((_idDWFProductGuiderates == value) 
							== false))
				{
					this.OnIdDWFProductGuideratesChanging(value);
					this.SendPropertyChanging();
					this._idDWFProductGuiderates = value;
					this.SendPropertyChanged("IdDWFProductGuiderates");
					this.OnIdDWFProductGuideratesChanged();
				}
			}
		}
		
		[Column(Storage="_fileName", Name="file_name", DbType="varchar(100)", AutoSync=AutoSync.Never, CanBeNull=false)]
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

		[Column(Storage = "_name", Name = "name", DbType = "varchar(100)", AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (((_name == value)
							== false))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
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
	

	[Table(Name = "ps_product_guide_rate")]
	public partial class PsProductGuideRate : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{

		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");

		private uint _idProductGuideRate;

		private uint _idShopDefault;

		private uint _idProduct;

		private uint _idDwfproductguiderates;

		private string _rate;

		private System.DateTime _dateAdd;

		private System.DateTime _dateUpd;

		#region Extensibility Method Declarations
		partial void OnCreated();

		partial void OnIdProductGuideRateChanged();

		partial void OnIdProductGuideRateChanging(uint value);

		partial void OnIdShopDefaultChanged();

		partial void OnIdShopDefaultChanging(uint value);

		partial void OnIdProductChanged();

		partial void OnIdProductChanging(uint value);

		partial void OnIdDwfproductguideratesChanged();

		partial void OnIdDwfproductguideratesChanging(uint value);

		partial void OnRateChanged();

		partial void OnRateChanging(string value);

		partial void OnDateAddDateChanged();

		partial void OnDateAddChanging(System.DateTime value);

		partial void OnDateUpdChanged();

		partial void OnDateUpdChanging(System.DateTime value);
		#endregion


		public PsProductGuideRate()
		{
			this.OnCreated();
		}

		[Column(Storage = "_idProductGuideRate", Name = "id_product_guide_rate", DbType = "int unsigned", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdProductGuideRate
		{
			get
			{
				return this._idProductGuideRate;
			}
			set
			{
				if (((_idProductGuideRate == value)
							== false))
				{
					this.OnIdProductGuideRateChanging(value);
					this.SendPropertyChanging();
					this._idProductGuideRate = value;
					this.SendPropertyChanged("IdProductGuideRate");
					this.OnIdProductGuideRateChanged();
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

		[Column(Storage = "_idDwfproductguiderates", Name = "id_dwfproductguiderates", DbType = "int unsigned", AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdDwfproductguiderates
		{
			get
			{
				return this._idDwfproductguiderates;
			}
			set
			{
				if (((_idDwfproductguiderates == value)
							== false))
				{
					this.OnIdDwfproductguideratesChanging(value);
					this.SendPropertyChanging();
					this._idDwfproductguiderates = value;
					this.SendPropertyChanged("IdDwfproductguiderates");
					this.OnIdDwfproductguideratesChanged();
				}
			}
		}

		[Column(Storage = "_rate", Name = "rate", DbType = "varchar(100)", AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public string Rate
		{
			get
			{
				return this._rate;
			}
			set
			{
				if (((_rate == value)
							== false))
				{
					this.OnRateChanging(value);
					this.SendPropertyChanging();
					this._rate = value;
					this.SendPropertyChanged("Rate");
					this.OnRateChanged();
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


	[Table(Name = "ps_product_guide_rate_lang")]
	public partial class PsProductGuideRateLang : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{

		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");

		private uint _idProductGuideRate;

		private uint _idShop;

		private uint _idLang;

		private string _comment;

		#region Extensibility Method Declarations
		partial void OnCreated();

		partial void OnIdProductGuideRateChanged();

		partial void OnIdProductGuideRateChanging(uint value);

		partial void OnIdShopChanged();

		partial void OnIdShopChanging(uint value);

		partial void OnIdLangChanged();

		partial void OnIdLangChanging(uint value);

		partial void OnCommentChanged();

		partial void OnCommentChanging(string value);
		#endregion


		public PsProductGuideRateLang()
		{
			this.OnCreated();
		}

		[Column(Storage = "_idProductGuideRate", Name = "id_product_guide_rate", DbType = "int unsigned", IsPrimaryKey = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdProductGuideRate
		{
			get
			{
				return this._idProductGuideRate;
			}
			set
			{
				if (((_idProductGuideRate == value)
							== false))
				{
					this.OnIdProductGuideRateChanging(value);
					this.SendPropertyChanging();
					this._idProductGuideRate = value;
					this.SendPropertyChanged("IdProductGuideRate");
					this.OnIdProductGuideRateChanged();
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

		[Column(Storage = "_comment", Name = "comment", DbType = "text", AutoSync = AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Comment
		{
			get
			{
				return this._comment;
			}
			set
			{
				if (((_comment == value)
							== false))
				{
					this.OnCommentChanging(value);
					this.SendPropertyChanging();
					this._comment = value;
					this.SendPropertyChanged("Comment");
					this.OnCommentChanged();
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


	[Table(Name = "ps_product_guide_rate_shop")]
	public partial class PsProductGuideRateShop : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{

		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");

		private uint _idProductGuideRate;

		private uint _idShop;

		#region Extensibility Method Declarations
		partial void OnCreated();

		partial void OnIdProductGuideRateChanged();

		partial void OnIdProductGuideRateChanging(uint value);

		partial void OnIdShopChanged();

		partial void OnIdShopChanging(uint value);
		#endregion


		public PsProductGuideRateShop()
		{
			this.OnCreated();
		}

		[Column(Storage = "_idProductGuideRate", Name = "id_product_guide_rate", DbType = "int unsigned", IsPrimaryKey = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IdProductGuideRate
		{
			get
			{
				return this._idProductGuideRate;
			}
			set
			{
				if (((_idProductGuideRate == value)
							== false))
				{
					this.OnIdProductGuideRateChanging(value);
					this.SendPropertyChanging();
					this._idProductGuideRate = value;
					this.SendPropertyChanged("IdProductGuideRate");
					this.OnIdProductGuideRateChanged();
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
