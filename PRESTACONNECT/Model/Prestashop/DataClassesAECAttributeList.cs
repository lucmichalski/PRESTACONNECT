// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from dblinq on 2016-12-09 10:24:01Z.
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
	
	
	[Table(Name="ps_aec_attributelist")]
	public partial class PsAEcAttributeList : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private sbyte _attributeListActive;
		
		private uint _displayTypeValue;
		
		private uint _idaeCAttributeList;
		
		private uint _idpRoduct;
		
		private sbyte _imGDisplay;
		
		private uint _psAttributeDisplay;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnAttributeListActiveChanged();
		
		partial void OnAttributeListActiveChanging(sbyte value);
		
		partial void OnDisplayTypeValueChanged();
		
		partial void OnDisplayTypeValueChanging(uint value);
		
		partial void OnIDAEcAttributeListChanged();
		
		partial void OnIDAEcAttributeListChanging(uint value);
		
		partial void OnIDProductChanged();
		
		partial void OnIDProductChanging(uint value);
		
		partial void OnIMgDisplayChanged();
		
		partial void OnIMgDisplayChanging(sbyte value);
		
		partial void OnPsAttributeDisplayChanged();
		
		partial void OnPsAttributeDisplayChanging(uint value);
		#endregion
		
		
		public PsAEcAttributeList()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_attributeListActive", Name="attribute_list_active", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte AttributeListActive
		{
			get
			{
				return this._attributeListActive;
			}
			set
			{
				if ((_attributeListActive != value))
				{
					this.OnAttributeListActiveChanging(value);
					this.SendPropertyChanging();
					this._attributeListActive = value;
					this.SendPropertyChanged("AttributeListActive");
					this.OnAttributeListActiveChanged();
				}
			}
		}
		
		[Column(Storage="_displayTypeValue", Name="display_type_value", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint DisplayTypeValue
		{
			get
			{
				return this._displayTypeValue;
			}
			set
			{
				if ((_displayTypeValue != value))
				{
					this.OnDisplayTypeValueChanging(value);
					this.SendPropertyChanging();
					this._displayTypeValue = value;
					this.SendPropertyChanged("DisplayTypeValue");
					this.OnDisplayTypeValueChanged();
				}
			}
		}
		
		[Column(Storage="_idaeCAttributeList", Name="id_aec_attributelist", DbType="int unsigned", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDAEcAttributeList
		{
			get
			{
				return this._idaeCAttributeList;
			}
			set
			{
				if ((_idaeCAttributeList != value))
				{
					this.OnIDAEcAttributeListChanging(value);
					this.SendPropertyChanging();
					this._idaeCAttributeList = value;
					this.SendPropertyChanged("IDAEcAttributeList");
					this.OnIDAEcAttributeListChanged();
				}
			}
		}
		
		[Column(Storage="_idpRoduct", Name="id_product", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDProduct
		{
			get
			{
				return this._idpRoduct;
			}
			set
			{
				if ((_idpRoduct != value))
				{
					this.OnIDProductChanging(value);
					this.SendPropertyChanging();
					this._idpRoduct = value;
					this.SendPropertyChanged("IDProduct");
					this.OnIDProductChanged();
				}
			}
		}
		
		[Column(Storage="_imGDisplay", Name="img_display", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte IMgDisplay
		{
			get
			{
				return this._imGDisplay;
			}
			set
			{
				if ((_imGDisplay != value))
				{
					this.OnIMgDisplayChanging(value);
					this.SendPropertyChanging();
					this._imGDisplay = value;
					this.SendPropertyChanged("IMgDisplay");
					this.OnIMgDisplayChanged();
				}
			}
		}
		
		[Column(Storage="_psAttributeDisplay", Name="ps_attribute_display", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint PsAttributeDisplay
		{
			get
			{
				return this._psAttributeDisplay;
			}
			set
			{
				if ((_psAttributeDisplay != value))
				{
					this.OnPsAttributeDisplayChanging(value);
					this.SendPropertyChanging();
					this._psAttributeDisplay = value;
					this.SendPropertyChanged("PsAttributeDisplay");
					this.OnPsAttributeDisplayChanged();
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
	
	[Table(Name="ps_aec_attributelist_attribute")]
	public partial class PsAEcAttributeListAttribute : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private sbyte _active;
		
		private uint _idaeCAttributeListAttribute;
		
		private uint _idpRoduct;
		
		private uint _idpRoductAttribute;
		
		private uint _packing;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnActiveChanged();
		
		partial void OnActiveChanging(sbyte value);
		
		partial void OnIDAEcAttributeListAttributeChanged();
		
		partial void OnIDAEcAttributeListAttributeChanging(uint value);
		
		partial void OnIDProductChanged();
		
		partial void OnIDProductChanging(uint value);
		
		partial void OnIDProductAttributeChanged();
		
		partial void OnIDProductAttributeChanging(uint value);
		
		partial void OnPackingChanged();
		
		partial void OnPackingChanging(uint value);
		#endregion
		
		
		public PsAEcAttributeListAttribute()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_active", Name="active", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte Active
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
		
		[Column(Storage="_idaeCAttributeListAttribute", Name="id_aec_attributelist_attribute", DbType="int unsigned", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDAEcAttributeListAttribute
		{
			get
			{
				return this._idaeCAttributeListAttribute;
			}
			set
			{
				if ((_idaeCAttributeListAttribute != value))
				{
					this.OnIDAEcAttributeListAttributeChanging(value);
					this.SendPropertyChanging();
					this._idaeCAttributeListAttribute = value;
					this.SendPropertyChanged("IDAEcAttributeListAttribute");
					this.OnIDAEcAttributeListAttributeChanged();
				}
			}
		}
		
		[Column(Storage="_idpRoduct", Name="id_product", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDProduct
		{
			get
			{
				return this._idpRoduct;
			}
			set
			{
				if ((_idpRoduct != value))
				{
					this.OnIDProductChanging(value);
					this.SendPropertyChanging();
					this._idpRoduct = value;
					this.SendPropertyChanged("IDProduct");
					this.OnIDProductChanged();
				}
			}
		}
		
		[Column(Storage="_idpRoductAttribute", Name="id_product_attribute", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDProductAttribute
		{
			get
			{
				return this._idpRoductAttribute;
			}
			set
			{
				if ((_idpRoductAttribute != value))
				{
					this.OnIDProductAttributeChanging(value);
					this.SendPropertyChanging();
					this._idpRoductAttribute = value;
					this.SendPropertyChanged("IDProductAttribute");
					this.OnIDProductAttributeChanged();
				}
			}
		}
		
		[Column(Storage="_packing", Name="packing", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint Packing
		{
			get
			{
				return this._packing;
			}
			set
			{
				if ((_packing != value))
				{
					this.OnPackingChanging(value);
					this.SendPropertyChanging();
					this._packing = value;
					this.SendPropertyChanged("Packing");
					this.OnPackingChanged();
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
	
	[Table(Name="ps_aec_attributelist_lang")]
	public partial class PsAEcAttributeListLang : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idaeCAttributeListLang;
		
		private uint _idlAng;
		
		private uint _idpRoduct;
		
		private uint _idpRoductAttribute;
		
		private string _newName;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDAEcAttributeListLangChanged();
		
		partial void OnIDAEcAttributeListLangChanging(uint value);
		
		partial void OnIDLangChanged();
		
		partial void OnIDLangChanging(uint value);
		
		partial void OnIDProductChanged();
		
		partial void OnIDProductChanging(uint value);
		
		partial void OnIDProductAttributeChanged();
		
		partial void OnIDProductAttributeChanging(uint value);
		
		partial void OnNewNameChanged();
		
		partial void OnNewNameChanging(string value);
		#endregion
		
		
		public PsAEcAttributeListLang()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_idaeCAttributeListLang", Name="id_aec_attributelist_lang", DbType="int unsigned", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDAEcAttributeListLang
		{
			get
			{
				return this._idaeCAttributeListLang;
			}
			set
			{
				if ((_idaeCAttributeListLang != value))
				{
					this.OnIDAEcAttributeListLangChanging(value);
					this.SendPropertyChanging();
					this._idaeCAttributeListLang = value;
					this.SendPropertyChanged("IDAEcAttributeListLang");
					this.OnIDAEcAttributeListLangChanged();
				}
			}
		}
		
		[Column(Storage="_idlAng", Name="id_lang", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
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
		
		[Column(Storage="_idpRoduct", Name="id_product", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDProduct
		{
			get
			{
				return this._idpRoduct;
			}
			set
			{
				if ((_idpRoduct != value))
				{
					this.OnIDProductChanging(value);
					this.SendPropertyChanging();
					this._idpRoduct = value;
					this.SendPropertyChanged("IDProduct");
					this.OnIDProductChanged();
				}
			}
		}
		
		[Column(Storage="_idpRoductAttribute", Name="id_product_attribute", DbType="int unsigned", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDProductAttribute
		{
			get
			{
				return this._idpRoductAttribute;
			}
			set
			{
				if ((_idpRoductAttribute != value))
				{
					this.OnIDProductAttributeChanging(value);
					this.SendPropertyChanging();
					this._idpRoductAttribute = value;
					this.SendPropertyChanged("IDProductAttribute");
					this.OnIDProductAttributeChanged();
				}
			}
		}
		
		[Column(Storage="_newName", Name="new_name", DbType="varchar(256)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string NewName
		{
			get
			{
				return this._newName;
			}
			set
			{
				if (((_newName == value) 
							== false))
				{
					this.OnNewNameChanging(value);
					this.SendPropertyChanging();
					this._newName = value;
					this.SendPropertyChanged("NewName");
					this.OnNewNameChanged();
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
