// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from dblinq on 2015-10-22 09:35:31Z.
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
	
		
	[Table(Name="ps_portfolio_customer_employee")]
	public partial class PsPortfolioCustomerEmployee : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private System.Nullable<uint> _idcUstomer;
		
		private System.Nullable<uint> _ideMployee;
		
		private uint _idpOrtfolio;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDCustomerChanged();
		
		partial void OnIDCustomerChanging(System.Nullable<uint> value);
		
		partial void OnIDEmployeeChanged();
		
		partial void OnIDEmployeeChanging(System.Nullable<uint> value);
		
		partial void OnIDPortfolioChanged();
		
		partial void OnIDPortfolioChanging(uint value);
		#endregion
		
		
		public PsPortfolioCustomerEmployee()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_idcUstomer", Name="id_customer", DbType="int unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<uint> IDCustomer
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
		
		[Column(Storage="_ideMployee", Name="id_employee", DbType="int unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<uint> IDEmployee
		{
			get
			{
				return this._ideMployee;
			}
			set
			{
				if ((_ideMployee != value))
				{
					this.OnIDEmployeeChanging(value);
					this.SendPropertyChanging();
					this._ideMployee = value;
					this.SendPropertyChanged("IDEmployee");
					this.OnIDEmployeeChanged();
				}
			}
		}
		
		[Column(Storage="_idpOrtfolio", Name="id_portfolio", DbType="int unsigned", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDPortfolio
		{
			get
			{
				return this._idpOrtfolio;
			}
			set
			{
				if ((_idpOrtfolio != value))
				{
					this.OnIDPortfolioChanging(value);
					this.SendPropertyChanging();
					this._idpOrtfolio = value;
					this.SendPropertyChanged("IDPortfolio");
					this.OnIDPortfolioChanged();
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
