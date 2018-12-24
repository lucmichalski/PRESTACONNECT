// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from cagc on 2013-10-16 18:16:10Z.
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
		
	[Table(Name="ps_oleapromo")]
	public partial class PsOleaPromo : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private System.Nullable<byte> _active;
		
		private System.Nullable<decimal> _attributionAmount;
		
		private System.Nullable<sbyte> _attributionAmountWithTaxes;
		
		private string _attributionCategories;
		
		private System.Nullable<sbyte> _attributionCategoriesOfCriteria;
		
		private System.Nullable<uint> _attributionIdcUrrency;
		
		private string _attributionManufacturers;
		
		private System.Nullable<sbyte> _attributionManufacturersOfCriteria;
		
		private System.Nullable<decimal> _attributionMaXaMount;
		
		private System.Nullable<int> _attributionMaXaMountIdcUrrency;
		
		private System.Nullable<sbyte> _attributionMaXaMountWithTaxes;
		
		private System.Nullable<uint> _attributionNbiMpactedProducts;
		
		private System.Nullable<decimal> _attributionPercent;
		
		private System.Nullable<sbyte> _attributionProductsOfCriteria;
		
		private System.Nullable<sbyte> _attributionProductWithQuantityPrice;
		
		private System.Nullable<sbyte> _attributionProductWithReductions;
		
		private string _attributionSuppliers;
		
		private System.Nullable<sbyte> _attributionSuppliersOfCriteria;
		
		private System.Nullable<uint> _attributionType;
		
		private string _attributionZonesFdP;
		
		private string _comments;
		
		private System.Nullable<sbyte> _criteriaAmountWithTaxes;
		
		private System.Nullable<sbyte> _criteriaAssociationType;
		
		private string _criteriaCategories;
		
		private System.Nullable<uint> _criteriaIdcUrrency;
		
		private string _criteriaManufacturers;
		
		private System.Nullable<decimal> _criteriaProductsAmount;
		
		private System.Nullable<uint> _criteriaProductsNumber;
		
		private System.Nullable<sbyte> _criteriaProductWithQuantityPrice;
		
		private System.Nullable<sbyte> _criteriaProductWithReductions;
		
		private System.Nullable<uint> _criteriaRepetitions;
		
		private string _criteriaSuppliers;
		
		private System.Nullable<uint> _criteriaType;
		
		private System.DateTime _dateAdd;
		
		private System.DateTime _dateUpd;
		
		private System.Nullable<sbyte> _globalAllowsFamily;
		
		private System.Nullable<sbyte> _globalAllowsOthers;
		
		private string _globalCartRuleExclusion;
		
		private System.Nullable<int> _globalCartRulePriority;
		
		private System.Nullable<sbyte> _globalCumUlAbleWithDiscounts;
		
		private System.DateTime _globalDateFrom;
		
		private System.DateTime _globalDateTo;
		
		private string _globalFamily;
		
		private string _globalGroups;
		
		private System.Nullable<int> _globalOrderType;
		
		private uint _idoLeApRomo;
		
		private string _mailCategories;
		
		private System.Nullable<sbyte> _mailCumUlAble;
		
		private System.Nullable<sbyte> _mailCumUlAbleReduction;
		
		private System.Nullable<System.DateTime> _mailDateFrom;
		
		private System.Nullable<sbyte> _mailDateFromOfOrder;
		
		private System.Nullable<System.DateTime> _mailDateTo;
		
		private System.Nullable<sbyte> _mailDiscountType;
		
		private System.Nullable<decimal> _mailDiscountValue;
		
		private System.Nullable<decimal> _mailMinimal;
		
		private System.Nullable<int> _mailValidityDays;
		
		private string _name;
		
		private System.Nullable<uint> _position;
		
		private System.Nullable<sbyte> _sendingMethod;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnActiveChanged();
		
		partial void OnActiveChanging(System.Nullable<byte> value);
		
		partial void OnAttributionAmountChanged();
		
		partial void OnAttributionAmountChanging(System.Nullable<decimal> value);
		
		partial void OnAttributionAmountWithTaxesChanged();
		
		partial void OnAttributionAmountWithTaxesChanging(System.Nullable<sbyte> value);
		
		partial void OnAttributionCategoriesChanged();
		
		partial void OnAttributionCategoriesChanging(string value);
		
		partial void OnAttributionCategoriesOfCriteriaChanged();
		
		partial void OnAttributionCategoriesOfCriteriaChanging(System.Nullable<sbyte> value);
		
		partial void OnAttributionIDCurrencyChanged();
		
		partial void OnAttributionIDCurrencyChanging(System.Nullable<uint> value);
		
		partial void OnAttributionManufacturersChanged();
		
		partial void OnAttributionManufacturersChanging(string value);
		
		partial void OnAttributionManufacturersOfCriteriaChanged();
		
		partial void OnAttributionManufacturersOfCriteriaChanging(System.Nullable<sbyte> value);
		
		partial void OnAttributionMaXAmountChanged();
		
		partial void OnAttributionMaXAmountChanging(System.Nullable<decimal> value);
		
		partial void OnAttributionMaXAmountIDCurrencyChanged();
		
		partial void OnAttributionMaXAmountIDCurrencyChanging(System.Nullable<int> value);
		
		partial void OnAttributionMaXAmountWithTaxesChanged();
		
		partial void OnAttributionMaXAmountWithTaxesChanging(System.Nullable<sbyte> value);
		
		partial void OnAttributionNBImpactedProductsChanged();
		
		partial void OnAttributionNBImpactedProductsChanging(System.Nullable<uint> value);
		
		partial void OnAttributionPercentChanged();
		
		partial void OnAttributionPercentChanging(System.Nullable<decimal> value);
		
		partial void OnAttributionProductsOfCriteriaChanged();
		
		partial void OnAttributionProductsOfCriteriaChanging(System.Nullable<sbyte> value);
		
		partial void OnAttributionProductWithQuantityPriceChanged();
		
		partial void OnAttributionProductWithQuantityPriceChanging(System.Nullable<sbyte> value);
		
		partial void OnAttributionProductWithReductionsChanged();
		
		partial void OnAttributionProductWithReductionsChanging(System.Nullable<sbyte> value);
		
		partial void OnAttributionSuppliersChanged();
		
		partial void OnAttributionSuppliersChanging(string value);
		
		partial void OnAttributionSuppliersOfCriteriaChanged();
		
		partial void OnAttributionSuppliersOfCriteriaChanging(System.Nullable<sbyte> value);
		
		partial void OnAttributionTypeChanged();
		
		partial void OnAttributionTypeChanging(System.Nullable<uint> value);
		
		partial void OnAttributionZonesFdPChanged();
		
		partial void OnAttributionZonesFdPChanging(string value);
		
		partial void OnCommentsChanged();
		
		partial void OnCommentsChanging(string value);
		
		partial void OnCriteriaAmountWithTaxesChanged();
		
		partial void OnCriteriaAmountWithTaxesChanging(System.Nullable<sbyte> value);
		
		partial void OnCriteriaAssociationTypeChanged();
		
		partial void OnCriteriaAssociationTypeChanging(System.Nullable<sbyte> value);
		
		partial void OnCriteriaCategoriesChanged();
		
		partial void OnCriteriaCategoriesChanging(string value);
		
		partial void OnCriteriaIDCurrencyChanged();
		
		partial void OnCriteriaIDCurrencyChanging(System.Nullable<uint> value);
		
		partial void OnCriteriaManufacturersChanged();
		
		partial void OnCriteriaManufacturersChanging(string value);
		
		partial void OnCriteriaProductsAmountChanged();
		
		partial void OnCriteriaProductsAmountChanging(System.Nullable<decimal> value);
		
		partial void OnCriteriaProductsNumberChanged();
		
		partial void OnCriteriaProductsNumberChanging(System.Nullable<uint> value);
		
		partial void OnCriteriaProductWithQuantityPriceChanged();
		
		partial void OnCriteriaProductWithQuantityPriceChanging(System.Nullable<sbyte> value);
		
		partial void OnCriteriaProductWithReductionsChanged();
		
		partial void OnCriteriaProductWithReductionsChanging(System.Nullable<sbyte> value);
		
		partial void OnCriteriaRepetitionsChanged();
		
		partial void OnCriteriaRepetitionsChanging(System.Nullable<uint> value);
		
		partial void OnCriteriaSuppliersChanged();
		
		partial void OnCriteriaSuppliersChanging(string value);
		
		partial void OnCriteriaTypeChanged();
		
		partial void OnCriteriaTypeChanging(System.Nullable<uint> value);
		
		partial void OnDateAddChanged();
		
		partial void OnDateAddChanging(System.DateTime value);
		
		partial void OnDateUpdChanged();
		
		partial void OnDateUpdChanging(System.DateTime value);
		
		partial void OnGlobalAllowsFamilyChanged();
		
		partial void OnGlobalAllowsFamilyChanging(System.Nullable<sbyte> value);
		
		partial void OnGlobalAllowsOthersChanged();
		
		partial void OnGlobalAllowsOthersChanging(System.Nullable<sbyte> value);
		
		partial void OnGlobalCartRuleExclusionChanged();
		
		partial void OnGlobalCartRuleExclusionChanging(string value);
		
		partial void OnGlobalCartRulePriorityChanged();
		
		partial void OnGlobalCartRulePriorityChanging(System.Nullable<int> value);
		
		partial void OnGlobalCumUlAbleWithDiscountsChanged();
		
		partial void OnGlobalCumUlAbleWithDiscountsChanging(System.Nullable<sbyte> value);
		
		partial void OnGlobalDateFromChanged();
		
		partial void OnGlobalDateFromChanging(System.DateTime value);
		
		partial void OnGlobalDateToChanged();
		
		partial void OnGlobalDateToChanging(System.DateTime value);
		
		partial void OnGlobalFamilyChanged();
		
		partial void OnGlobalFamilyChanging(string value);
		
		partial void OnGlobalGroupsChanged();
		
		partial void OnGlobalGroupsChanging(string value);
		
		partial void OnGlobalOrderTypeChanged();
		
		partial void OnGlobalOrderTypeChanging(System.Nullable<int> value);
		
		partial void OnIDOleAPromoChanged();
		
		partial void OnIDOleAPromoChanging(uint value);
		
		partial void OnMailCategoriesChanged();
		
		partial void OnMailCategoriesChanging(string value);
		
		partial void OnMailCumUlAbleChanged();
		
		partial void OnMailCumUlAbleChanging(System.Nullable<sbyte> value);
		
		partial void OnMailCumUlAbleReductionChanged();
		
		partial void OnMailCumUlAbleReductionChanging(System.Nullable<sbyte> value);
		
		partial void OnMailDateFromChanged();
		
		partial void OnMailDateFromChanging(System.Nullable<System.DateTime> value);
		
		partial void OnMailDateFromOfOrderChanged();
		
		partial void OnMailDateFromOfOrderChanging(System.Nullable<sbyte> value);
		
		partial void OnMailDateToChanged();
		
		partial void OnMailDateToChanging(System.Nullable<System.DateTime> value);
		
		partial void OnMailDiscountTypeChanged();
		
		partial void OnMailDiscountTypeChanging(System.Nullable<sbyte> value);
		
		partial void OnMailDiscountValueChanged();
		
		partial void OnMailDiscountValueChanging(System.Nullable<decimal> value);
		
		partial void OnMailMinimalChanged();
		
		partial void OnMailMinimalChanging(System.Nullable<decimal> value);
		
		partial void OnMailValidityDaysChanged();
		
		partial void OnMailValidityDaysChanging(System.Nullable<int> value);
		
		partial void OnNameChanged();
		
		partial void OnNameChanging(string value);
		
		partial void OnPositionChanged();
		
		partial void OnPositionChanging(System.Nullable<uint> value);
		
		partial void OnSendingMethodChanged();
		
		partial void OnSendingMethodChanging(System.Nullable<sbyte> value);
		#endregion
		
		
		public PsOleaPromo()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_active", Name="active", DbType="tinyint(1) unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<byte> Active
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
		
		[Column(Storage="_attributionAmount", Name="attribution_amount", DbType="decimal(10,2)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<decimal> AttributionAmount
		{
			get
			{
				return this._attributionAmount;
			}
			set
			{
				if ((_attributionAmount != value))
				{
					this.OnAttributionAmountChanging(value);
					this.SendPropertyChanging();
					this._attributionAmount = value;
					this.SendPropertyChanged("AttributionAmount");
					this.OnAttributionAmountChanged();
				}
			}
		}
		
		[Column(Storage="_attributionAmountWithTaxes", Name="attribution_amount_withtaxes", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> AttributionAmountWithTaxes
		{
			get
			{
				return this._attributionAmountWithTaxes;
			}
			set
			{
				if ((_attributionAmountWithTaxes != value))
				{
					this.OnAttributionAmountWithTaxesChanging(value);
					this.SendPropertyChanging();
					this._attributionAmountWithTaxes = value;
					this.SendPropertyChanged("AttributionAmountWithTaxes");
					this.OnAttributionAmountWithTaxesChanged();
				}
			}
		}
		
		[Column(Storage="_attributionCategories", Name="attribution_categories", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string AttributionCategories
		{
			get
			{
				return this._attributionCategories;
			}
			set
			{
				if (((_attributionCategories == value) 
							== false))
				{
					this.OnAttributionCategoriesChanging(value);
					this.SendPropertyChanging();
					this._attributionCategories = value;
					this.SendPropertyChanged("AttributionCategories");
					this.OnAttributionCategoriesChanged();
				}
			}
		}
		
		[Column(Storage="_attributionCategoriesOfCriteria", Name="attribution_categories_of_criteria", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> AttributionCategoriesOfCriteria
		{
			get
			{
				return this._attributionCategoriesOfCriteria;
			}
			set
			{
				if ((_attributionCategoriesOfCriteria != value))
				{
					this.OnAttributionCategoriesOfCriteriaChanging(value);
					this.SendPropertyChanging();
					this._attributionCategoriesOfCriteria = value;
					this.SendPropertyChanged("AttributionCategoriesOfCriteria");
					this.OnAttributionCategoriesOfCriteriaChanged();
				}
			}
		}
		
		[Column(Storage="_attributionIdcUrrency", Name="attribution_id_currency", DbType="int unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<uint> AttributionIDCurrency
		{
			get
			{
				return this._attributionIdcUrrency;
			}
			set
			{
				if ((_attributionIdcUrrency != value))
				{
					this.OnAttributionIDCurrencyChanging(value);
					this.SendPropertyChanging();
					this._attributionIdcUrrency = value;
					this.SendPropertyChanged("AttributionIDCurrency");
					this.OnAttributionIDCurrencyChanged();
				}
			}
		}
		
		[Column(Storage="_attributionManufacturers", Name="attribution_manufacturers", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string AttributionManufacturers
		{
			get
			{
				return this._attributionManufacturers;
			}
			set
			{
				if (((_attributionManufacturers == value) 
							== false))
				{
					this.OnAttributionManufacturersChanging(value);
					this.SendPropertyChanging();
					this._attributionManufacturers = value;
					this.SendPropertyChanged("AttributionManufacturers");
					this.OnAttributionManufacturersChanged();
				}
			}
		}
		
		[Column(Storage="_attributionManufacturersOfCriteria", Name="attribution_manufacturers_of_criteria", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> AttributionManufacturersOfCriteria
		{
			get
			{
				return this._attributionManufacturersOfCriteria;
			}
			set
			{
				if ((_attributionManufacturersOfCriteria != value))
				{
					this.OnAttributionManufacturersOfCriteriaChanging(value);
					this.SendPropertyChanging();
					this._attributionManufacturersOfCriteria = value;
					this.SendPropertyChanged("AttributionManufacturersOfCriteria");
					this.OnAttributionManufacturersOfCriteriaChanged();
				}
			}
		}
		
		[Column(Storage="_attributionMaXaMount", Name="attribution_maxamount", DbType="decimal(10,2)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<decimal> AttributionMaXAmount
		{
			get
			{
				return this._attributionMaXaMount;
			}
			set
			{
				if ((_attributionMaXaMount != value))
				{
					this.OnAttributionMaXAmountChanging(value);
					this.SendPropertyChanging();
					this._attributionMaXaMount = value;
					this.SendPropertyChanged("AttributionMaXAmount");
					this.OnAttributionMaXAmountChanged();
				}
			}
		}
		
		[Column(Storage="_attributionMaXaMountIdcUrrency", Name="attribution_maxamount_id_currency", DbType="int(10)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> AttributionMaXAmountIDCurrency
		{
			get
			{
				return this._attributionMaXaMountIdcUrrency;
			}
			set
			{
				if ((_attributionMaXaMountIdcUrrency != value))
				{
					this.OnAttributionMaXAmountIDCurrencyChanging(value);
					this.SendPropertyChanging();
					this._attributionMaXaMountIdcUrrency = value;
					this.SendPropertyChanged("AttributionMaXAmountIDCurrency");
					this.OnAttributionMaXAmountIDCurrencyChanged();
				}
			}
		}
		
		[Column(Storage="_attributionMaXaMountWithTaxes", Name="attribution_maxamount_withtaxes", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> AttributionMaXAmountWithTaxes
		{
			get
			{
				return this._attributionMaXaMountWithTaxes;
			}
			set
			{
				if ((_attributionMaXaMountWithTaxes != value))
				{
					this.OnAttributionMaXAmountWithTaxesChanging(value);
					this.SendPropertyChanging();
					this._attributionMaXaMountWithTaxes = value;
					this.SendPropertyChanged("AttributionMaXAmountWithTaxes");
					this.OnAttributionMaXAmountWithTaxesChanged();
				}
			}
		}
		
		[Column(Storage="_attributionNbiMpactedProducts", Name="attribution_nb_impacted_products", DbType="int unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<uint> AttributionNBImpactedProducts
		{
			get
			{
				return this._attributionNbiMpactedProducts;
			}
			set
			{
				if ((_attributionNbiMpactedProducts != value))
				{
					this.OnAttributionNBImpactedProductsChanging(value);
					this.SendPropertyChanging();
					this._attributionNbiMpactedProducts = value;
					this.SendPropertyChanged("AttributionNBImpactedProducts");
					this.OnAttributionNBImpactedProductsChanged();
				}
			}
		}
		
		[Column(Storage="_attributionPercent", Name="attribution_percent", DbType="decimal(10,3)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<decimal> AttributionPercent
		{
			get
			{
				return this._attributionPercent;
			}
			set
			{
				if ((_attributionPercent != value))
				{
					this.OnAttributionPercentChanging(value);
					this.SendPropertyChanging();
					this._attributionPercent = value;
					this.SendPropertyChanged("AttributionPercent");
					this.OnAttributionPercentChanged();
				}
			}
		}
		
		[Column(Storage="_attributionProductsOfCriteria", Name="attribution_products_of_criteria", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> AttributionProductsOfCriteria
		{
			get
			{
				return this._attributionProductsOfCriteria;
			}
			set
			{
				if ((_attributionProductsOfCriteria != value))
				{
					this.OnAttributionProductsOfCriteriaChanging(value);
					this.SendPropertyChanging();
					this._attributionProductsOfCriteria = value;
					this.SendPropertyChanged("AttributionProductsOfCriteria");
					this.OnAttributionProductsOfCriteriaChanged();
				}
			}
		}
		
		[Column(Storage="_attributionProductWithQuantityPrice", Name="attribution_product_with_quantity_price", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> AttributionProductWithQuantityPrice
		{
			get
			{
				return this._attributionProductWithQuantityPrice;
			}
			set
			{
				if ((_attributionProductWithQuantityPrice != value))
				{
					this.OnAttributionProductWithQuantityPriceChanging(value);
					this.SendPropertyChanging();
					this._attributionProductWithQuantityPrice = value;
					this.SendPropertyChanged("AttributionProductWithQuantityPrice");
					this.OnAttributionProductWithQuantityPriceChanged();
				}
			}
		}
		
		[Column(Storage="_attributionProductWithReductions", Name="attribution_product_with_reductions", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> AttributionProductWithReductions
		{
			get
			{
				return this._attributionProductWithReductions;
			}
			set
			{
				if ((_attributionProductWithReductions != value))
				{
					this.OnAttributionProductWithReductionsChanging(value);
					this.SendPropertyChanging();
					this._attributionProductWithReductions = value;
					this.SendPropertyChanged("AttributionProductWithReductions");
					this.OnAttributionProductWithReductionsChanged();
				}
			}
		}
		
		[Column(Storage="_attributionSuppliers", Name="attribution_suppliers", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string AttributionSuppliers
		{
			get
			{
				return this._attributionSuppliers;
			}
			set
			{
				if (((_attributionSuppliers == value) 
							== false))
				{
					this.OnAttributionSuppliersChanging(value);
					this.SendPropertyChanging();
					this._attributionSuppliers = value;
					this.SendPropertyChanged("AttributionSuppliers");
					this.OnAttributionSuppliersChanged();
				}
			}
		}
		
		[Column(Storage="_attributionSuppliersOfCriteria", Name="attribution_suppliers_of_criteria", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> AttributionSuppliersOfCriteria
		{
			get
			{
				return this._attributionSuppliersOfCriteria;
			}
			set
			{
				if ((_attributionSuppliersOfCriteria != value))
				{
					this.OnAttributionSuppliersOfCriteriaChanging(value);
					this.SendPropertyChanging();
					this._attributionSuppliersOfCriteria = value;
					this.SendPropertyChanged("AttributionSuppliersOfCriteria");
					this.OnAttributionSuppliersOfCriteriaChanged();
				}
			}
		}
		
		[Column(Storage="_attributionType", Name="attribution_type", DbType="int unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<uint> AttributionType
		{
			get
			{
				return this._attributionType;
			}
			set
			{
				if ((_attributionType != value))
				{
					this.OnAttributionTypeChanging(value);
					this.SendPropertyChanging();
					this._attributionType = value;
					this.SendPropertyChanged("AttributionType");
					this.OnAttributionTypeChanged();
				}
			}
		}
		
		[Column(Storage="_attributionZonesFdP", Name="attribution_zones_fdp", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string AttributionZonesFdP
		{
			get
			{
				return this._attributionZonesFdP;
			}
			set
			{
				if (((_attributionZonesFdP == value) 
							== false))
				{
					this.OnAttributionZonesFdPChanging(value);
					this.SendPropertyChanging();
					this._attributionZonesFdP = value;
					this.SendPropertyChanged("AttributionZonesFdP");
					this.OnAttributionZonesFdPChanged();
				}
			}
		}
		
		[Column(Storage="_comments", Name="comments", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string Comments
		{
			get
			{
				return this._comments;
			}
			set
			{
				if (((_comments == value) 
							== false))
				{
					this.OnCommentsChanging(value);
					this.SendPropertyChanging();
					this._comments = value;
					this.SendPropertyChanged("Comments");
					this.OnCommentsChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaAmountWithTaxes", Name="criteria_amount_withtaxes", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> CriteriaAmountWithTaxes
		{
			get
			{
				return this._criteriaAmountWithTaxes;
			}
			set
			{
				if ((_criteriaAmountWithTaxes != value))
				{
					this.OnCriteriaAmountWithTaxesChanging(value);
					this.SendPropertyChanging();
					this._criteriaAmountWithTaxes = value;
					this.SendPropertyChanged("CriteriaAmountWithTaxes");
					this.OnCriteriaAmountWithTaxesChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaAssociationType", Name="criteria_association_type", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> CriteriaAssociationType
		{
			get
			{
				return this._criteriaAssociationType;
			}
			set
			{
				if ((_criteriaAssociationType != value))
				{
					this.OnCriteriaAssociationTypeChanging(value);
					this.SendPropertyChanging();
					this._criteriaAssociationType = value;
					this.SendPropertyChanged("CriteriaAssociationType");
					this.OnCriteriaAssociationTypeChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaCategories", Name="criteria_categories", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string CriteriaCategories
		{
			get
			{
				return this._criteriaCategories;
			}
			set
			{
				if (((_criteriaCategories == value) 
							== false))
				{
					this.OnCriteriaCategoriesChanging(value);
					this.SendPropertyChanging();
					this._criteriaCategories = value;
					this.SendPropertyChanged("CriteriaCategories");
					this.OnCriteriaCategoriesChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaIdcUrrency", Name="criteria_id_currency", DbType="int unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<uint> CriteriaIDCurrency
		{
			get
			{
				return this._criteriaIdcUrrency;
			}
			set
			{
				if ((_criteriaIdcUrrency != value))
				{
					this.OnCriteriaIDCurrencyChanging(value);
					this.SendPropertyChanging();
					this._criteriaIdcUrrency = value;
					this.SendPropertyChanged("CriteriaIDCurrency");
					this.OnCriteriaIDCurrencyChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaManufacturers", Name="criteria_manufacturers", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string CriteriaManufacturers
		{
			get
			{
				return this._criteriaManufacturers;
			}
			set
			{
				if (((_criteriaManufacturers == value) 
							== false))
				{
					this.OnCriteriaManufacturersChanging(value);
					this.SendPropertyChanging();
					this._criteriaManufacturers = value;
					this.SendPropertyChanged("CriteriaManufacturers");
					this.OnCriteriaManufacturersChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaProductsAmount", Name="criteria_products_amount", DbType="decimal(10,2)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<decimal> CriteriaProductsAmount
		{
			get
			{
				return this._criteriaProductsAmount;
			}
			set
			{
				if ((_criteriaProductsAmount != value))
				{
					this.OnCriteriaProductsAmountChanging(value);
					this.SendPropertyChanging();
					this._criteriaProductsAmount = value;
					this.SendPropertyChanged("CriteriaProductsAmount");
					this.OnCriteriaProductsAmountChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaProductsNumber", Name="criteria_products_number", DbType="int unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<uint> CriteriaProductsNumber
		{
			get
			{
				return this._criteriaProductsNumber;
			}
			set
			{
				if ((_criteriaProductsNumber != value))
				{
					this.OnCriteriaProductsNumberChanging(value);
					this.SendPropertyChanging();
					this._criteriaProductsNumber = value;
					this.SendPropertyChanged("CriteriaProductsNumber");
					this.OnCriteriaProductsNumberChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaProductWithQuantityPrice", Name="criteria_product_with_quantity_price", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> CriteriaProductWithQuantityPrice
		{
			get
			{
				return this._criteriaProductWithQuantityPrice;
			}
			set
			{
				if ((_criteriaProductWithQuantityPrice != value))
				{
					this.OnCriteriaProductWithQuantityPriceChanging(value);
					this.SendPropertyChanging();
					this._criteriaProductWithQuantityPrice = value;
					this.SendPropertyChanged("CriteriaProductWithQuantityPrice");
					this.OnCriteriaProductWithQuantityPriceChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaProductWithReductions", Name="criteria_product_with_reductions", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> CriteriaProductWithReductions
		{
			get
			{
				return this._criteriaProductWithReductions;
			}
			set
			{
				if ((_criteriaProductWithReductions != value))
				{
					this.OnCriteriaProductWithReductionsChanging(value);
					this.SendPropertyChanging();
					this._criteriaProductWithReductions = value;
					this.SendPropertyChanged("CriteriaProductWithReductions");
					this.OnCriteriaProductWithReductionsChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaRepetitions", Name="criteria_repetitions", DbType="int unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<uint> CriteriaRepetitions
		{
			get
			{
				return this._criteriaRepetitions;
			}
			set
			{
				if ((_criteriaRepetitions != value))
				{
					this.OnCriteriaRepetitionsChanging(value);
					this.SendPropertyChanging();
					this._criteriaRepetitions = value;
					this.SendPropertyChanged("CriteriaRepetitions");
					this.OnCriteriaRepetitionsChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaSuppliers", Name="criteria_suppliers", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string CriteriaSuppliers
		{
			get
			{
				return this._criteriaSuppliers;
			}
			set
			{
				if (((_criteriaSuppliers == value) 
							== false))
				{
					this.OnCriteriaSuppliersChanging(value);
					this.SendPropertyChanging();
					this._criteriaSuppliers = value;
					this.SendPropertyChanged("CriteriaSuppliers");
					this.OnCriteriaSuppliersChanged();
				}
			}
		}
		
		[Column(Storage="_criteriaType", Name="criteria_type", DbType="int unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<uint> CriteriaType
		{
			get
			{
				return this._criteriaType;
			}
			set
			{
				if ((_criteriaType != value))
				{
					this.OnCriteriaTypeChanging(value);
					this.SendPropertyChanging();
					this._criteriaType = value;
					this.SendPropertyChanged("CriteriaType");
					this.OnCriteriaTypeChanged();
				}
			}
		}
		
		[Column(Storage="_dateAdd", Name="date_add", DbType="datetime", AutoSync=AutoSync.Never, CanBeNull=false)]
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
					this.OnDateAddChanged();
				}
			}
		}
		
		[Column(Storage="_dateUpd", Name="date_upd", DbType="datetime", AutoSync=AutoSync.Never, CanBeNull=false)]
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
		
		[Column(Storage="_globalAllowsFamily", Name="global_allows_family", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> GlobalAllowsFamily
		{
			get
			{
				return this._globalAllowsFamily;
			}
			set
			{
				if ((_globalAllowsFamily != value))
				{
					this.OnGlobalAllowsFamilyChanging(value);
					this.SendPropertyChanging();
					this._globalAllowsFamily = value;
					this.SendPropertyChanged("GlobalAllowsFamily");
					this.OnGlobalAllowsFamilyChanged();
				}
			}
		}
		
		[Column(Storage="_globalAllowsOthers", Name="global_allows_others", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> GlobalAllowsOthers
		{
			get
			{
				return this._globalAllowsOthers;
			}
			set
			{
				if ((_globalAllowsOthers != value))
				{
					this.OnGlobalAllowsOthersChanging(value);
					this.SendPropertyChanging();
					this._globalAllowsOthers = value;
					this.SendPropertyChanged("GlobalAllowsOthers");
					this.OnGlobalAllowsOthersChanged();
				}
			}
		}
		
		[Column(Storage="_globalCartRuleExclusion", Name="global_cart_rule_exclusion", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string GlobalCartRuleExclusion
		{
			get
			{
				return this._globalCartRuleExclusion;
			}
			set
			{
				if (((_globalCartRuleExclusion == value) 
							== false))
				{
					this.OnGlobalCartRuleExclusionChanging(value);
					this.SendPropertyChanging();
					this._globalCartRuleExclusion = value;
					this.SendPropertyChanged("GlobalCartRuleExclusion");
					this.OnGlobalCartRuleExclusionChanged();
				}
			}
		}
		
		[Column(Storage="_globalCartRulePriority", Name="global_cart_rule_priority", DbType="int(6)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> GlobalCartRulePriority
		{
			get
			{
				return this._globalCartRulePriority;
			}
			set
			{
				if ((_globalCartRulePriority != value))
				{
					this.OnGlobalCartRulePriorityChanging(value);
					this.SendPropertyChanging();
					this._globalCartRulePriority = value;
					this.SendPropertyChanged("GlobalCartRulePriority");
					this.OnGlobalCartRulePriorityChanged();
				}
			}
		}
		
		[Column(Storage="_globalCumUlAbleWithDiscounts", Name="global_cumulable_with_discounts", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> GlobalCumUlAbleWithDiscounts
		{
			get
			{
				return this._globalCumUlAbleWithDiscounts;
			}
			set
			{
				if ((_globalCumUlAbleWithDiscounts != value))
				{
					this.OnGlobalCumUlAbleWithDiscountsChanging(value);
					this.SendPropertyChanging();
					this._globalCumUlAbleWithDiscounts = value;
					this.SendPropertyChanged("GlobalCumUlAbleWithDiscounts");
					this.OnGlobalCumUlAbleWithDiscountsChanged();
				}
			}
		}
		
		[Column(Storage="_globalDateFrom", Name="global_date_from", DbType="date", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public System.DateTime GlobalDateFrom
		{
			get
			{
				return this._globalDateFrom;
			}
			set
			{
				if ((_globalDateFrom != value))
				{
					this.OnGlobalDateFromChanging(value);
					this.SendPropertyChanging();
					this._globalDateFrom = value;
					this.SendPropertyChanged("GlobalDateFrom");
					this.OnGlobalDateFromChanged();
				}
			}
		}
		
		[Column(Storage="_globalDateTo", Name="global_date_to", DbType="date", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public System.DateTime GlobalDateTo
		{
			get
			{
				return this._globalDateTo;
			}
			set
			{
				if ((_globalDateTo != value))
				{
					this.OnGlobalDateToChanging(value);
					this.SendPropertyChanging();
					this._globalDateTo = value;
					this.SendPropertyChanged("GlobalDateTo");
					this.OnGlobalDateToChanged();
				}
			}
		}
		
		[Column(Storage="_globalFamily", Name="global_family", DbType="varchar(32)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string GlobalFamily
		{
			get
			{
				return this._globalFamily;
			}
			set
			{
				if (((_globalFamily == value) 
							== false))
				{
					this.OnGlobalFamilyChanging(value);
					this.SendPropertyChanging();
					this._globalFamily = value;
					this.SendPropertyChanged("GlobalFamily");
					this.OnGlobalFamilyChanged();
				}
			}
		}
		
		[Column(Storage="_globalGroups", Name="global_groups", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string GlobalGroups
		{
			get
			{
				return this._globalGroups;
			}
			set
			{
				if (((_globalGroups == value) 
							== false))
				{
					this.OnGlobalGroupsChanging(value);
					this.SendPropertyChanging();
					this._globalGroups = value;
					this.SendPropertyChanged("GlobalGroups");
					this.OnGlobalGroupsChanged();
				}
			}
		}
		
		[Column(Storage="_globalOrderType", Name="global_order_type", DbType="int(10)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> GlobalOrderType
		{
			get
			{
				return this._globalOrderType;
			}
			set
			{
				if ((_globalOrderType != value))
				{
					this.OnGlobalOrderTypeChanging(value);
					this.SendPropertyChanging();
					this._globalOrderType = value;
					this.SendPropertyChanged("GlobalOrderType");
					this.OnGlobalOrderTypeChanged();
				}
			}
		}
		
		[Column(Storage="_idoLeApRomo", Name="id_oleapromo", DbType="int unsigned", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDOleAPromo
		{
			get
			{
				return this._idoLeApRomo;
			}
			set
			{
				if ((_idoLeApRomo != value))
				{
					this.OnIDOleAPromoChanging(value);
					this.SendPropertyChanging();
					this._idoLeApRomo = value;
					this.SendPropertyChanged("IDOleAPromo");
					this.OnIDOleAPromoChanged();
				}
			}
		}
		
		[Column(Storage="_mailCategories", Name="mail_categories", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string MailCategories
		{
			get
			{
				return this._mailCategories;
			}
			set
			{
				if (((_mailCategories == value) 
							== false))
				{
					this.OnMailCategoriesChanging(value);
					this.SendPropertyChanging();
					this._mailCategories = value;
					this.SendPropertyChanged("MailCategories");
					this.OnMailCategoriesChanged();
				}
			}
		}
		
		[Column(Storage="_mailCumUlAble", Name="mail_cumulable", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> MailCumUlAble
		{
			get
			{
				return this._mailCumUlAble;
			}
			set
			{
				if ((_mailCumUlAble != value))
				{
					this.OnMailCumUlAbleChanging(value);
					this.SendPropertyChanging();
					this._mailCumUlAble = value;
					this.SendPropertyChanged("MailCumUlAble");
					this.OnMailCumUlAbleChanged();
				}
			}
		}
		
		[Column(Storage="_mailCumUlAbleReduction", Name="mail_cumulable_reduction", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> MailCumUlAbleReduction
		{
			get
			{
				return this._mailCumUlAbleReduction;
			}
			set
			{
				if ((_mailCumUlAbleReduction != value))
				{
					this.OnMailCumUlAbleReductionChanging(value);
					this.SendPropertyChanging();
					this._mailCumUlAbleReduction = value;
					this.SendPropertyChanged("MailCumUlAbleReduction");
					this.OnMailCumUlAbleReductionChanged();
				}
			}
		}
		
		[Column(Storage="_mailDateFrom", Name="mail_date_from", DbType="date", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<System.DateTime> MailDateFrom
		{
			get
			{
				return this._mailDateFrom;
			}
			set
			{
				if ((_mailDateFrom != value))
				{
					this.OnMailDateFromChanging(value);
					this.SendPropertyChanging();
					this._mailDateFrom = value;
					this.SendPropertyChanged("MailDateFrom");
					this.OnMailDateFromChanged();
				}
			}
		}
		
		[Column(Storage="_mailDateFromOfOrder", Name="mail_date_from_of_order", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> MailDateFromOfOrder
		{
			get
			{
				return this._mailDateFromOfOrder;
			}
			set
			{
				if ((_mailDateFromOfOrder != value))
				{
					this.OnMailDateFromOfOrderChanging(value);
					this.SendPropertyChanging();
					this._mailDateFromOfOrder = value;
					this.SendPropertyChanged("MailDateFromOfOrder");
					this.OnMailDateFromOfOrderChanged();
				}
			}
		}
		
		[Column(Storage="_mailDateTo", Name="mail_date_to", DbType="date", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<System.DateTime> MailDateTo
		{
			get
			{
				return this._mailDateTo;
			}
			set
			{
				if ((_mailDateTo != value))
				{
					this.OnMailDateToChanging(value);
					this.SendPropertyChanging();
					this._mailDateTo = value;
					this.SendPropertyChanged("MailDateTo");
					this.OnMailDateToChanged();
				}
			}
		}
		
		[Column(Storage="_mailDiscountType", Name="mail_discount_type", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> MailDiscountType
		{
			get
			{
				return this._mailDiscountType;
			}
			set
			{
				if ((_mailDiscountType != value))
				{
					this.OnMailDiscountTypeChanging(value);
					this.SendPropertyChanging();
					this._mailDiscountType = value;
					this.SendPropertyChanged("MailDiscountType");
					this.OnMailDiscountTypeChanged();
				}
			}
		}
		
		[Column(Storage="_mailDiscountValue", Name="mail_discount_value", DbType="decimal(10,2)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<decimal> MailDiscountValue
		{
			get
			{
				return this._mailDiscountValue;
			}
			set
			{
				if ((_mailDiscountValue != value))
				{
					this.OnMailDiscountValueChanging(value);
					this.SendPropertyChanging();
					this._mailDiscountValue = value;
					this.SendPropertyChanged("MailDiscountValue");
					this.OnMailDiscountValueChanged();
				}
			}
		}
		
		[Column(Storage="_mailMinimal", Name="mail_minimal", DbType="decimal(10,2)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<decimal> MailMinimal
		{
			get
			{
				return this._mailMinimal;
			}
			set
			{
				if ((_mailMinimal != value))
				{
					this.OnMailMinimalChanging(value);
					this.SendPropertyChanging();
					this._mailMinimal = value;
					this.SendPropertyChanged("MailMinimal");
					this.OnMailMinimalChanged();
				}
			}
		}
		
		[Column(Storage="_mailValidityDays", Name="mail_validity_days", DbType="int(6)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> MailValidityDays
		{
			get
			{
				return this._mailValidityDays;
			}
			set
			{
				if ((_mailValidityDays != value))
				{
					this.OnMailValidityDaysChanging(value);
					this.SendPropertyChanging();
					this._mailValidityDays = value;
					this.SendPropertyChanged("MailValidityDays");
					this.OnMailValidityDaysChanged();
				}
			}
		}
		
		[Column(Storage="_name", Name="name", DbType="varchar(64)", AutoSync=AutoSync.Never)]
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
		
		[Column(Storage="_position", Name="position", DbType="int unsigned", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<uint> Position
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
		
		[Column(Storage="_sendingMethod", Name="sending_method", DbType="tinyint(1)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<sbyte> SendingMethod
		{
			get
			{
				return this._sendingMethod;
			}
			set
			{
				if ((_sendingMethod != value))
				{
					this.OnSendingMethodChanging(value);
					this.SendPropertyChanging();
					this._sendingMethod = value;
					this.SendPropertyChanged("SendingMethod");
					this.OnSendingMethodChanged();
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
	
	[Table(Name="ps_oleapromo_lang")]
	public partial class PsOleaPromoLang : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private string _communicationExtraRight;
		
		private string _communicationProductFooter;
		
		private string _discountObjdEscription;
		
		private uint _idlAng;
		
		private uint _idoLeApRomo;
		
		private string _mailDescription;
		
		private string _mailMessage;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnCommunicationExtraRightChanged();
		
		partial void OnCommunicationExtraRightChanging(string value);
		
		partial void OnCommunicationProductFooterChanged();
		
		partial void OnCommunicationProductFooterChanging(string value);
		
		partial void OnDiscountOBJDescriptionChanged();
		
		partial void OnDiscountOBJDescriptionChanging(string value);
		
		partial void OnIDLangChanged();
		
		partial void OnIDLangChanging(uint value);
		
		partial void OnIDOleAPromoChanged();
		
		partial void OnIDOleAPromoChanging(uint value);
		
		partial void OnMailDescriptionChanged();
		
		partial void OnMailDescriptionChanging(string value);
		
		partial void OnMailMessageChanged();
		
		partial void OnMailMessageChanging(string value);
		#endregion
		
		
		public PsOleaPromoLang()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_communicationExtraRight", Name="communication_extra_right", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string CommunicationExtraRight
		{
			get
			{
				return this._communicationExtraRight;
			}
			set
			{
				if (((_communicationExtraRight == value) 
							== false))
				{
					this.OnCommunicationExtraRightChanging(value);
					this.SendPropertyChanging();
					this._communicationExtraRight = value;
					this.SendPropertyChanged("CommunicationExtraRight");
					this.OnCommunicationExtraRightChanged();
				}
			}
		}
		
		[Column(Storage="_communicationProductFooter", Name="communication_product_footer", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string CommunicationProductFooter
		{
			get
			{
				return this._communicationProductFooter;
			}
			set
			{
				if (((_communicationProductFooter == value) 
							== false))
				{
					this.OnCommunicationProductFooterChanging(value);
					this.SendPropertyChanging();
					this._communicationProductFooter = value;
					this.SendPropertyChanged("CommunicationProductFooter");
					this.OnCommunicationProductFooterChanged();
				}
			}
		}
		
		[Column(Storage="_discountObjdEscription", Name="discountobj_description", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string DiscountOBJDescription
		{
			get
			{
				return this._discountObjdEscription;
			}
			set
			{
				if (((_discountObjdEscription == value) 
							== false))
				{
					this.OnDiscountOBJDescriptionChanging(value);
					this.SendPropertyChanging();
					this._discountObjdEscription = value;
					this.SendPropertyChanged("DiscountOBJDescription");
					this.OnDiscountOBJDescriptionChanged();
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
		
		[Column(Storage="_idoLeApRomo", Name="id_oleapromo", DbType="int unsigned", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public uint IDOleAPromo
		{
			get
			{
				return this._idoLeApRomo;
			}
			set
			{
				if ((_idoLeApRomo != value))
				{
					this.OnIDOleAPromoChanging(value);
					this.SendPropertyChanging();
					this._idoLeApRomo = value;
					this.SendPropertyChanged("IDOleAPromo");
					this.OnIDOleAPromoChanged();
				}
			}
		}
		
		[Column(Storage="_mailDescription", Name="mail_description", DbType="text", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string MailDescription
		{
			get
			{
				return this._mailDescription;
			}
			set
			{
				if (((_mailDescription == value) 
							== false))
				{
					this.OnMailDescriptionChanging(value);
					this.SendPropertyChanging();
					this._mailDescription = value;
					this.SendPropertyChanged("MailDescription");
					this.OnMailDescriptionChanged();
				}
			}
		}
		
		[Column(Storage="_mailMessage", Name="mail_message", DbType="varchar(256)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string MailMessage
		{
			get
			{
				return this._mailMessage;
			}
			set
			{
				if (((_mailMessage == value) 
							== false))
				{
					this.OnMailMessageChanging(value);
					this.SendPropertyChanging();
					this._mailMessage = value;
					this.SendPropertyChanged("MailMessage");
					this.OnMailMessageChanged();
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
	
	[Table(Name="ps_oleapromo_shop")]
	public partial class PsOleaPromoShop : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _idoLeApRomo;
		
		private int _idsHop;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnIDOleAPromoChanged();
		
		partial void OnIDOleAPromoChanging(int value);
		
		partial void OnIDShopChanged();
		
		partial void OnIDShopChanging(int value);
		#endregion
		
		
		public PsOleaPromoShop()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_idoLeApRomo", Name="id_oleapromo", DbType="int(10)", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int IDOleAPromo
		{
			get
			{
				return this._idoLeApRomo;
			}
			set
			{
				if ((_idoLeApRomo != value))
				{
					this.OnIDOleAPromoChanging(value);
					this.SendPropertyChanging();
					this._idoLeApRomo = value;
					this.SendPropertyChanged("IDOleAPromo");
					this.OnIDOleAPromoChanged();
				}
			}
		}
		
		[Column(Storage="_idsHop", Name="id_shop", DbType="int", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int IDShop
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
}
