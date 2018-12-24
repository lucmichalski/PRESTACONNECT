// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from ps154 on 2013-10-02 14:51:15Z.
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
	
		
	[Table(Name="ps_customer_feature")]
	public partial class PsCustomerFeature : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idcUstomerFeature;
		
		private uint _position;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDCustomerFeatureChanged();
		
		partial void OnIDCustomerFeatureChanging(uint value);
		
		partial void OnPositionChanged();
		
		partial void OnPositionChanging(uint value);
		#endregion
		
		
		public PsCustomerFeature()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_idcUstomerFeature", Name="id_customer_feature", DbType="int unsigned", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCustomerFeature
		{
			get
			{
				return this._idcUstomerFeature;
			}
			set
			{
				if ((_idcUstomerFeature != value))
				{
					this.OnIDCustomerFeatureChanging(value);
					this.SendPropertyChanging();
					this._idcUstomerFeature = value;
					this.SendPropertyChanged("IDCustomerFeature");
					this.OnIDCustomerFeatureChanged();
				}
			}
		}
		
		[Column(Storage="_position", Name="position", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
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
	
	[Table(Name="ps_customer_feature_customer")]
	public partial class PsCustomerFeatureCustomer : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idcUstomer;
		
		private uint _idcUstomerFeature;
		
		private uint _idcUstomerFeatureValue;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDCustomerChanged();
		
		partial void OnIDCustomerChanging(uint value);
		
		partial void OnIDCustomerFeatureChanged();
		
		partial void OnIDCustomerFeatureChanging(uint value);
		
		partial void OnIDCustomerFeatureValueChanged();
		
		partial void OnIDCustomerFeatureValueChanging(uint value);
		#endregion
		
		
		public PsCustomerFeatureCustomer()
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
		
		[Column(Storage="_idcUstomerFeature", Name="id_customer_feature", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCustomerFeature
		{
			get
			{
				return this._idcUstomerFeature;
			}
			set
			{
				if ((_idcUstomerFeature != value))
				{
					this.OnIDCustomerFeatureChanging(value);
					this.SendPropertyChanging();
					this._idcUstomerFeature = value;
					this.SendPropertyChanged("IDCustomerFeature");
					this.OnIDCustomerFeatureChanged();
				}
			}
		}
		
		[Column(Storage="_idcUstomerFeatureValue", Name="id_customer_feature_value", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCustomerFeatureValue
		{
			get
			{
				return this._idcUstomerFeatureValue;
			}
			set
			{
				if ((_idcUstomerFeatureValue != value))
				{
					this.OnIDCustomerFeatureValueChanging(value);
					this.SendPropertyChanging();
					this._idcUstomerFeatureValue = value;
					this.SendPropertyChanged("IDCustomerFeatureValue");
					this.OnIDCustomerFeatureValueChanged();
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
	
	[Table(Name="ps_customer_feature_lang")]
	public partial class PsCustomerFeatureLang : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idcUstomerFeature;
		
		private uint _idlAng;
		
		private string _name;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDCustomerFeatureChanged();
		
		partial void OnIDCustomerFeatureChanging(uint value);
		
		partial void OnIDLangChanged();
		
		partial void OnIDLangChanging(uint value);
		
		partial void OnNameChanged();
		
		partial void OnNameChanging(string value);
		#endregion
		
		
		public PsCustomerFeatureLang()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_idcUstomerFeature", Name="id_customer_feature", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCustomerFeature
		{
			get
			{
				return this._idcUstomerFeature;
			}
			set
			{
				if ((_idcUstomerFeature != value))
				{
					this.OnIDCustomerFeatureChanging(value);
					this.SendPropertyChanging();
					this._idcUstomerFeature = value;
					this.SendPropertyChanged("IDCustomerFeature");
					this.OnIDCustomerFeatureChanged();
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
		
		[Column(Storage="_name", Name="name", DbType="varchar(128)", AutoSync=AutoSync.Never)]
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
	
	[Table(Name="ps_customer_feature_shop")]
	public partial class PsCustomerFeatureShop : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idcUstomerFeature;
		
		private uint _idsHop;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDCustomerFeatureChanged();
		
		partial void OnIDCustomerFeatureChanging(uint value);
		
		partial void OnIDShopChanged();
		
		partial void OnIDShopChanging(uint value);
		#endregion
		
		
		public PsCustomerFeatureShop()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_idcUstomerFeature", Name="id_customer_feature", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCustomerFeature
		{
			get
			{
				return this._idcUstomerFeature;
			}
			set
			{
				if ((_idcUstomerFeature != value))
				{
					this.OnIDCustomerFeatureChanging(value);
					this.SendPropertyChanging();
					this._idcUstomerFeature = value;
					this.SendPropertyChanged("IDCustomerFeature");
					this.OnIDCustomerFeatureChanged();
				}
			}
		}
		
		[Column(Storage="_idsHop", Name="id_shop", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDShop
		{
			get
			{
				return this._idsHop;
			}
			set
			{
				if ((_idsHop != value))
				{
					this.OnIDShopChanging(value);
					this.SendPropertyChanging();
					this._idsHop = value;
					this.SendPropertyChanged("IDShop");
					this.OnIDShopChanged();
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
	
	[Table(Name="ps_customer_feature_value")]
	public partial class PsCustomerFeatureValue : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private System.Nullable<byte> _custom;
		
		private uint _idcUstomerFeature;
		
		private uint _idcUstomerFeatureValue;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnCustomChanged();
		
		partial void OnCustomChanging(System.Nullable<byte> value);
		
		partial void OnIDCustomerFeatureChanged();
		
		partial void OnIDCustomerFeatureChanging(uint value);
		
		partial void OnIDCustomerFeatureValueChanged();
		
		partial void OnIDCustomerFeatureValueChanging(uint value);
		#endregion
		
		
		public PsCustomerFeatureValue()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_custom", Name="custom", DbType="tinyint(3) unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<byte> Custom
		{
			get
			{
				return this._custom;
			}
			set
			{
				if ((_custom != value))
				{
					this.OnCustomChanging(value);
					this.SendPropertyChanging();
					this._custom = value;
					this.SendPropertyChanged("Custom");
					this.OnCustomChanged();
				}
			}
		}
		
		[Column(Storage="_idcUstomerFeature", Name="id_customer_feature", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCustomerFeature
		{
			get
			{
				return this._idcUstomerFeature;
			}
			set
			{
				if ((_idcUstomerFeature != value))
				{
					this.OnIDCustomerFeatureChanging(value);
					this.SendPropertyChanging();
					this._idcUstomerFeature = value;
					this.SendPropertyChanged("IDCustomerFeature");
					this.OnIDCustomerFeatureChanged();
				}
			}
		}
		
		[Column(Storage="_idcUstomerFeatureValue", Name="id_customer_feature_value", DbType="int unsigned", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCustomerFeatureValue
		{
			get
			{
				return this._idcUstomerFeatureValue;
			}
			set
			{
				if ((_idcUstomerFeatureValue != value))
				{
					this.OnIDCustomerFeatureValueChanging(value);
					this.SendPropertyChanging();
					this._idcUstomerFeatureValue = value;
					this.SendPropertyChanged("IDCustomerFeatureValue");
					this.OnIDCustomerFeatureValueChanged();
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
	
	[Table(Name="ps_customer_feature_value_lang")]
	public partial class PsCustomerFeatureValueLang : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idcUstomerFeatureValue;
		
		private uint _idlAng;
		
		private string _value;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDCustomerFeatureValueChanged();
		
		partial void OnIDCustomerFeatureValueChanging(uint value);
		
		partial void OnIDLangChanged();
		
		partial void OnIDLangChanging(uint value);
		
		partial void OnValueChanged();
		
		partial void OnValueChanging(string value);
		#endregion
		
		
		public PsCustomerFeatureValueLang()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_idcUstomerFeatureValue", Name="id_customer_feature_value", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCustomerFeatureValue
		{
			get
			{
				return this._idcUstomerFeatureValue;
			}
			set
			{
				if ((_idcUstomerFeatureValue != value))
				{
					this.OnIDCustomerFeatureValueChanging(value);
					this.SendPropertyChanging();
					this._idcUstomerFeatureValue = value;
					this.SendPropertyChanged("IDCustomerFeatureValue");
					this.OnIDCustomerFeatureValueChanged();
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
		
		[Column(Storage="_value", Name="value", DbType="varchar(255)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (((_value == value) 
							== false))
				{
					this.OnValueChanging(value);
					this.SendPropertyChanging();
					this._value = value;
					this.SendPropertyChanged("Value");
					this.OnValueChanged();
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
