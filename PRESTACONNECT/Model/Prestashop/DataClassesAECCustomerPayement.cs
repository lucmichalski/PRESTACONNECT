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


    [Table(Name = "ps_aec_customer_payement")]
	public partial class PsAEcCustomerPayement : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idCustomer;
		
		private uint _idSage;
		
		private string _payement;
		
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDCustomerChanged();
		
		partial void OnIDCustomerChanging(uint value);
		
		partial void OnIDSageChanged();
		
		partial void OnIDSageChanging(uint value);
		
		partial void OnPayementChanged();
		
		partial void OnPayementChanging(string value);
		
		#endregion


        public PsAEcCustomerPayement()
		{
			this.OnCreated();
		}

        [Column(Storage = "_idCustomer", Name = "id_customer", DbType = "int unsigned", IsPrimaryKey = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IDCustomer
		{
			get
			{
                return this._idCustomer;
			}
			set
			{
                if ((_idCustomer != value))
				{
					this.OnIDCustomerChanging(value);
					this.SendPropertyChanging();
                    this._idCustomer = value;
					this.SendPropertyChanged("IDCustomer");
					this.OnIDCustomerChanged();
				}
			}
		}

        [Column(Storage = "_idsage", Name = "Sag_Id", DbType = "int unsigned", IsPrimaryKey = true, AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public uint IDSage
		{
			get
			{
                return this._idSage;
			}
			set
			{
                if ((_idSage != value))
				{
					this.OnIDSageChanging(value);
					this.SendPropertyChanging();
                    this._idSage = value;
                    this.SendPropertyChanged("IDSage");
					this.OnIDSageChanged();
				}
			}
		}

        [Column(Storage = "_payement", Name = "Payement", DbType = "varchar(100)", AutoSync = AutoSync.Never, CanBeNull = false)]
		[DebuggerNonUserCode()]
		public string Payement
		{
			get
			{
                return this._payement;
			}
			set
			{
                if ((_payement != value))
				{
                    this.OnPayementChanging(value);
					this.SendPropertyChanging();
                    this._payement = value;
                    this.SendPropertyChanged("Payement");
                    this.OnPayementChanged();
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
