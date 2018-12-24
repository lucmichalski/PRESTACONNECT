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


    [Table(Name = "ps_aec_customer_collaborateur")]
	public partial class PsAEcCustomerCollaborateur : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private uint _idCustomer;
		
		private string _nomCollaborateur;
		
		private string _prenomCollaborateur;
		
		private string _telephoneCollaborateur;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDCustomerChanged();
		
		partial void OnIDCustomerChanging(uint value);
		
		partial void OnNomCollaborateurChanged();
		
		partial void OnNomCollaborateurChanging(string value);
		
		partial void OnPrenomCollaborateurChanged();
		
		partial void OnPrenomCollaborateurChanging(string value);
		
		partial void OnTelephoneCollaborateurChanged();
		
		partial void OnTelephoneCollaborateurChanging(string value);
		#endregion
		
		
		public PsAEcCustomerCollaborateur()
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

        [Column(Storage = "_nomCollaborateur", Name = "nom_collaborateur", DbType = "varchar(35)", AutoSync = AutoSync.Never, CanBeNull = true)]
		[DebuggerNonUserCode()]
		public string NomCollaborateur
		{
			get
			{
                return this._nomCollaborateur;
			}
			set
			{
                if ((_nomCollaborateur != value))
				{
					this.OnNomCollaborateurChanging(value);
					this.SendPropertyChanging();
                    this._nomCollaborateur = value;
					this.SendPropertyChanged("NomCollaborateur");
					this.OnNomCollaborateurChanged();
				}
			}
		}

        [Column(Storage = "_prenomCollaborateur", Name = "prenom_collaborateur", DbType = "varchar(35)", AutoSync = AutoSync.Never, CanBeNull = true)]
		[DebuggerNonUserCode()]
		public string PrenomCollaborateur
		{
			get
			{
                return this._prenomCollaborateur;
			}
			set
			{
                if ((_prenomCollaborateur != value))
				{
					this.OnPrenomCollaborateurChanging(value);
					this.SendPropertyChanging();
                    this._prenomCollaborateur = value;
                    this.SendPropertyChanged("PrenomCollaborateur");
					this.OnPrenomCollaborateurChanged();
				}
			}
		}

        [Column(Storage = "_telephoneCollaborateur", Name = "telephone_collaborateur", DbType = "varchar(21)", AutoSync = AutoSync.Never, CanBeNull = true)]
		[DebuggerNonUserCode()]
		public string TelephoneCollaborateur
		{
			get
			{
                return this._telephoneCollaborateur;
			}
			set
			{
                if ((_telephoneCollaborateur != value))
				{
					this.OnTelephoneCollaborateurChanging(value);
					this.SendPropertyChanging();
                    this._telephoneCollaborateur = value;
                    this.SendPropertyChanged("TelephoneCollaborateur");
					this.OnTelephoneCollaborateurChanged();
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
