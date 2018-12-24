// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from dblinq on 2014-03-18 14:53:18Z.
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
	
	
	[Table(Name="ps_aec_stock")]
	public partial class PsAEcStock : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idsTockAvailable;
		
		private int _maximalQuantity;
		
		private int _minimalQuantity;
		
		private int _quantityFuture;
		
		private int _quantityReal;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDStockAvailableChanged();
		
		partial void OnIDStockAvailableChanging(uint value);
		
		partial void OnMaximalQuantityChanged();
		
		partial void OnMaximalQuantityChanging(int value);
		
		partial void OnMinimalQuantityChanged();
		
		partial void OnMinimalQuantityChanging(int value);
		
		partial void OnQuantityFutureChanged();
		
		partial void OnQuantityFutureChanging(int value);
		
		partial void OnQuantityRealChanged();
		
		partial void OnQuantityRealChanging(int value);
		#endregion
		
		
		public PsAEcStock()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_idsTockAvailable", Name="id_stock_available", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDStockAvailable
		{
			get
			{
				return this._idsTockAvailable;
			}
			set
			{
				if ((_idsTockAvailable != value))
				{
					this.OnIDStockAvailableChanging(value);
					this.SendPropertyChanging();
					this._idsTockAvailable = value;
					this.SendPropertyChanged("IDStockAvailable");
					this.OnIDStockAvailableChanged();
				}
			}
		}
		
		[Column(Storage="_maximalQuantity", Name="maximal_quantity", DbType="int(10)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int MaximalQuantity
		{
			get
			{
				return this._maximalQuantity;
			}
			set
			{
				if ((_maximalQuantity != value))
				{
					this.OnMaximalQuantityChanging(value);
					this.SendPropertyChanging();
					this._maximalQuantity = value;
					this.SendPropertyChanged("MaximalQuantity");
					this.OnMaximalQuantityChanged();
				}
			}
		}
		
		[Column(Storage="_minimalQuantity", Name="minimal_quantity", DbType="int(10)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int MinimalQuantity
		{
			get
			{
				return this._minimalQuantity;
			}
			set
			{
				if ((_minimalQuantity != value))
				{
					this.OnMinimalQuantityChanging(value);
					this.SendPropertyChanging();
					this._minimalQuantity = value;
					this.SendPropertyChanged("MinimalQuantity");
					this.OnMinimalQuantityChanged();
				}
			}
		}
		
		[Column(Storage="_quantityFuture", Name="quantity_future", DbType="int(10)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int QuantityFuture
		{
			get
			{
				return this._quantityFuture;
			}
			set
			{
				if ((_quantityFuture != value))
				{
					this.OnQuantityFutureChanging(value);
					this.SendPropertyChanging();
					this._quantityFuture = value;
					this.SendPropertyChanged("QuantityFuture");
					this.OnQuantityFutureChanged();
				}
			}
		}
		
		[Column(Storage="_quantityReal", Name="quantity_real", DbType="int(10)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int QuantityReal
		{
			get
			{
				return this._quantityReal;
			}
			set
			{
				if ((_quantityReal != value))
				{
					this.OnQuantityRealChanging(value);
					this.SendPropertyChanging();
					this._quantityReal = value;
					this.SendPropertyChanged("QuantityReal");
					this.OnQuantityRealChanged();
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
