using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT.Contexts
{
	internal sealed class ConfigurationMultiBonReductionContext : Context
	{
		#region Properties

		private Model.Local.OrderCartRuleRepository OrderCartRuleRepository = new Model.Local.OrderCartRuleRepository();

		private List<Model.Local.OrderCartRule> listOrderCartRule;
		public List<Model.Local.OrderCartRule> ListOrderCartRule
		{
			get { return listOrderCartRule; }
			set
			{
				listOrderCartRule = value;
				OnPropertyChanged("ListOrderCartRule");
			}
		}

		private Model.Local.OrderCartRule selectedOrderCartRule;
		public Model.Local.OrderCartRule SelectedOrderCartRule
		{
			get { return selectedOrderCartRule; }
			set
			{
				selectedOrderCartRule = value;
				OnPropertyChanged("SelectedOrderCartRule");
			}
		}

		private ObservableCollection<Model.Sage.F_ARTICLE_Light> listArticles;
		public ObservableCollection<Model.Sage.F_ARTICLE_Light> ListArticles
		{
			get { return listArticles; }
			set
			{
				listArticles = value;
				OnPropertyChanged("ListArticles");
			}
		}

		private Model.Sage.F_ARTICLE_Light selectedSageArticle;
		public Model.Sage.F_ARTICLE_Light SelectedSageArticle
		{
			get { return selectedSageArticle; }
			set
			{
				selectedSageArticle = value;
				OnPropertyChanged("SelectedSageArticle");
			}
		}

		private ObservableCollection<Model.Prestashop.PsCartRule> listReductionPs;

		#endregion

		#region Constructors

		public ConfigurationMultiBonReductionContext()
			: base()
		{
			ListArticles = new ObservableCollection<Model.Sage.F_ARTICLE_Light>(new Model.Sage.F_ARTICLERepository().ListLightHorsStock());

			listReductionPs = new ObservableCollection<Model.Prestashop.PsCartRule>(new Model.Prestashop.PsCartRuleRepository().List());

			ListOrderCartRule = new List<Model.Local.OrderCartRule>();
			foreach (Model.Prestashop.PsCartRule cart in listReductionPs)
			{
				if (OrderCartRuleRepository.Exist((int)cart.IDCartRule))
				{
					ListOrderCartRule.Add(OrderCartRuleRepository.Read((int)cart.IDCartRule));
				}
				else
				{
					Model.Local.OrderCartRule cartRule = new Model.Local.OrderCartRule();
					cartRule.Pre_id = (int)cart.IDCartRule;
					ListOrderCartRule.Add(cartRule);
				}
			}
		}

		#endregion

		#region Methods

		public void SaveCatCompta()
		{
			OrderCartRuleRepository.Save();
			MessageBox.Show("Paramètres enregistrés", "Prestaconnect", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		public void UnselectArticle()
		{
			if (SelectedOrderCartRule != null && SelectedOrderCartRule.Sag_id != null)
			{
				SelectedOrderCartRule.Sag_id = null;
			}
		}

		public void SageArticleChange(Model.Sage.F_ARTICLE_Light article)
		{
			if (article != null)
			{
				if (SelectedOrderCartRule.Sag_id == null)
				{
					SelectedOrderCartRule.Sag_id = article.cbMarq;
					OrderCartRuleRepository.Add(SelectedOrderCartRule);
				}
				else
				{
					SelectedOrderCartRule.Sag_id = article.cbMarq;
					OrderCartRuleRepository.Save();
				}

				ListOrderCartRule = new List<Model.Local.OrderCartRule>();
				List<Model.Local.OrderCartRule> listOrderCartRuleTemp = new List<Model.Local.OrderCartRule>();
				OrderCartRuleRepository = new Model.Local.OrderCartRuleRepository();
				foreach (Model.Prestashop.PsCartRule cart in listReductionPs)
				{
					if (OrderCartRuleRepository.Exist((int)cart.IDCartRule))
					{
						listOrderCartRuleTemp.Add(OrderCartRuleRepository.Read((int)cart.IDCartRule));
					}
					else
					{
						Model.Local.OrderCartRule cartRule = new Model.Local.OrderCartRule();
						cartRule.Pre_id = (int)cart.IDCartRule;
						listOrderCartRuleTemp.Add(cartRule);
					}
				}
				ListOrderCartRule = listOrderCartRuleTemp;
			}
		}
		#endregion
	}
}
