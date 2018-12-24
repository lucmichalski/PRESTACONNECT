// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from dblinq on 2015-01-15 09:12:37Z.
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
	
	
	[Table(Name="ps_aec_customer_outstanding")]
	public partial class PsAEcCustomerOutstanding : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");

        private System.Nullable<decimal> _encoursActuelSage;
		
		private uint _idcUstomer;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnEnCOursAcTueLSageChanged();
		
		partial void OnEnCOursAcTueLSageChanging(System.Nullable<decimal> value);
		
		partial void OnIDCustomerChanged();
		
		partial void OnIDCustomerChanging(uint value);
		#endregion
		
		
		public PsAEcCustomerOutstanding()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_encoursActuelSage", Name="encours_actuel_sage", DbType="decimal(20,6)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<decimal> EncoursActuelSage
		{
			get
			{
				return this._encoursActuelSage;
			}
			set
			{
				if ((_encoursActuelSage != value))
				{
					this.OnEnCOursAcTueLSageChanging(value);
					this.SendPropertyChanging();
					this._encoursActuelSage = value;
                    this.SendPropertyChanged("EncoursActuelSage");
					this.OnEnCOursAcTueLSageChanged();
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
