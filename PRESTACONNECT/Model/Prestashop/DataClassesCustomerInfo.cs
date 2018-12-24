// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from dblinq on 2015-04-13 14:40:08Z.
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
	
		
	[Table(Name="ps_customer_info")]
	public partial class PsCustomerInfo : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private byte _braille;
		
		private string _customerNumber;
		
		private uint _idcUstomer;
		
		private uint _idcUstomerInfo;
		
		private System.Nullable<uint> _idsToreCustomer;
		
		private byte _maLvoyaNt;
		
		private byte _nonVoyaNt;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnBrailleChanged();
		
		partial void OnBrailleChanging(byte value);
		
		partial void OnCustomerNumberChanged();
		
		partial void OnCustomerNumberChanging(string value);
		
		partial void OnIDCustomerChanged();
		
		partial void OnIDCustomerChanging(uint value);
		
		partial void OnIDCustomerInfoChanged();
		
		partial void OnIDCustomerInfoChanging(uint value);
		
		partial void OnIDStoreCustomerChanged();
		
		partial void OnIDStoreCustomerChanging(System.Nullable<uint> value);
		
		partial void OnMaLVOYAntChanged();
		
		partial void OnMaLVOYAntChanging(byte value);
		
		partial void OnNonVOYAntChanged();
		
		partial void OnNonVOYAntChanging(byte value);
		#endregion
		
		
		public PsCustomerInfo()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_braille", Name="braille", DbType="tinyint(1) unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public byte Braille
		{
			get
			{
				return this._braille;
			}
			set
			{
				if ((_braille != value))
				{
					this.OnBrailleChanging(value);
					this.SendPropertyChanging();
					this._braille = value;
					this.SendPropertyChanged("Braille");
					this.OnBrailleChanged();
				}
			}
		}
		
		[Column(Storage="_customerNumber", Name="customer_number", DbType="varchar(19)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string CustomerNumber
		{
			get
			{
				return this._customerNumber;
			}
			set
			{
				if (((_customerNumber == value) 
							== false))
				{
					this.OnCustomerNumberChanging(value);
					this.SendPropertyChanging();
					this._customerNumber = value;
					this.SendPropertyChanged("CustomerNumber");
					this.OnCustomerNumberChanged();
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
		
		[Column(Storage="_idcUstomerInfo", Name="id_customer_info", DbType="int unsigned", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCustomerInfo
		{
			get
			{
				return this._idcUstomerInfo;
			}
			set
			{
				if ((_idcUstomerInfo != value))
				{
					this.OnIDCustomerInfoChanging(value);
					this.SendPropertyChanging();
					this._idcUstomerInfo = value;
					this.SendPropertyChanged("IDCustomerInfo");
					this.OnIDCustomerInfoChanged();
				}
			}
		}
		
		[Column(Storage="_idsToreCustomer", Name="id_store_customer", DbType="int unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<uint> IDStoreCustomer
		{
			get
			{
				return this._idsToreCustomer;
			}
			set
			{
				if ((_idsToreCustomer != value))
				{
					this.OnIDStoreCustomerChanging(value);
					this.SendPropertyChanging();
					this._idsToreCustomer = value;
					this.SendPropertyChanged("IDStoreCustomer");
					this.OnIDStoreCustomerChanged();
				}
			}
		}
		
		[Column(Storage="_maLvoyaNt", Name="mal_voyant", DbType="tinyint(1) unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public byte MaLVOYAnt
		{
			get
			{
				return this._maLvoyaNt;
			}
			set
			{
				if ((_maLvoyaNt != value))
				{
					this.OnMaLVOYAntChanging(value);
					this.SendPropertyChanging();
					this._maLvoyaNt = value;
					this.SendPropertyChanged("MaLVOYAnt");
					this.OnMaLVOYAntChanged();
				}
			}
		}
		
		[Column(Storage="_nonVoyaNt", Name="non_voyant", DbType="tinyint(1) unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public byte NonVOYAnt
		{
			get
			{
				return this._nonVoyaNt;
			}
			set
			{
				if ((_nonVoyaNt != value))
				{
					this.OnNonVOYAntChanging(value);
					this.SendPropertyChanging();
					this._nonVoyaNt = value;
					this.SendPropertyChanged("NonVOYAnt");
					this.OnNonVOYAntChanged();
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
