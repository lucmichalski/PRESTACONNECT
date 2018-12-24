// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from test on 2013-12-02 16:54:01Z.
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
	
	[Table(Name="ps_cart_preorder")]
	public partial class PsCartPreOrder : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idcArt;
		
		private uint _idpReOrder;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDCartChanged();
		
		partial void OnIDCartChanging(uint value);
		
		partial void OnIDPreOrderChanged();
		
		partial void OnIDPreOrderChanging(uint value);
		#endregion
		
		
		public PsCartPreOrder()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_idcArt", Name="id_cart", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDCart
		{
			get
			{
				return this._idcArt;
			}
			set
			{
				if ((_idcArt != value))
				{
					this.OnIDCartChanging(value);
					this.SendPropertyChanging();
					this._idcArt = value;
					this.SendPropertyChanged("IDCart");
					this.OnIDCartChanged();
				}
			}
		}
		
		[Column(Storage="_idpReOrder", Name="id_preorder", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDPreOrder
		{
			get
			{
				return this._idpReOrder;
			}
			set
			{
				if ((_idpReOrder != value))
				{
					this.OnIDPreOrderChanging(value);
					this.SendPropertyChanging();
					this._idpReOrder = value;
					this.SendPropertyChanged("IDPreOrder");
					this.OnIDPreOrderChanged();
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
