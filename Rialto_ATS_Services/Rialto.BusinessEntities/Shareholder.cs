using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.BusinessEntities
{
    public class Shareholder
    {

        #region Constructors

        public Shareholder()
        {
            Users = new List<User>();
        }

        #endregion

        #region Public Static Consts

        public static string _STATUS_ONBOARDING = "APPLICATION_IN_PROGRESS";
        
        public static string _STATUS_APP_APPROVED = "APPLICATION_APPROVED";

        public static int _FIRM_TYPE_INDIV_RETAIL = 11;

        #endregion

        #region Public Methods

        public long Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public double? FirmLimit { get; set; }

        public string FirmAccountNumber { get; set; }

        public string Principal { get; set; }

        public string FirmTaxId { get; set; }

        public string PepStatus { get; set; }

        public string InsiderStatus { get; set; }

        public string FeeMatrix { get; set; }

        public bool IssuerFirmCheckbox { get; set; }

        public string LargeTraderId { get; set; }

        public string UniqueId { get; set; }

        public bool Status { get; set; }

        public bool LargeTraderFlag { get; set; }

        public int FirmType { get; set; }

        public bool Enabled { get; set; }

        public List<User> Users { get; set; }

        public List<Account> Accounts { get; set; }

        public ShareholderType ShareholderType { get; set; }

        public KoreConXShareholderId KoreConXShareholderId { get; set; }
        
        public string OnboardingStatus { get; set; }

        #endregion

        #region Public Methods

        public User User
        {
            get
            {
                if (Users != null && Users.Count > 0)
                {
                    return Users.FirstOrDefault();
                        
                }
                else
                    return null;
            }
        }


        public User GetUser(string email)
        {
            if (Users != null && Users.Where(x => x.Email == email).Count() > 0)
            {
                return Users.Where(x => x.Email == email).FirstOrDefault();

            }
            else
                return null;
        }

        public Account GetAccount(string email)
        {
            if (Accounts != null && Accounts.Where(x => x.Email == email).Count() > 0)
            {
                return Accounts.Where(x => x.Email == email).FirstOrDefault();

            }
            else
                return null;
        }

        public string GetFullName()
        {
            string msg = Name + "( id=" + Id + ")";

            return msg;
        }

        public void MapKCXPersonResponse()
        { 
        
        
        
        
        
        }

        #endregion
    }
}
