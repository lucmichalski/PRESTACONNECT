
namespace PRESTACONNECT.Core.Sync
{
    internal sealed class Centrale
    {
        #region Properties

        public Model.Sage.F_COMPTET_Light Tiers { get; set; }
        public Model.Sage.F_COMPTET_Light CentraleAchat { get; set; }
        public Model.Local.Customer Customer { get; set; }
        public Model.Prestashop.idcustomer PsCustomer { get; set; }

        #endregion
        #region Constructors

        public Centrale(Model.Sage.F_COMPTET_Light tiers, Model.Local.Customer customer,
            Model.Prestashop.idcustomer psCustomer, Model.Sage.F_COMPTET_Light centraleAchat)
        {
            Tiers = tiers;
            Customer = customer;
            PsCustomer = psCustomer;
            CentraleAchat = centraleAchat;
        }
        public Centrale(Model.Sage.F_COMPTET_Light tiers, Model.Local.Customer customer,
            Model.Prestashop.idcustomer psCustomer)
            : this(tiers, customer, psCustomer, null)
        {
        }

        #endregion

        public int cat_tarif
        {
            get
            {
                return (CentraleAchat != null) ? CentraleAchat.N_CatTarif.Value : Tiers.N_CatTarif.Value;
            }
        }
    }
}
