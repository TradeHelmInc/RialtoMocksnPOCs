﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.BusinessEntities
{
    public class Shareholder
    {
        #region Public Static Consts

        public static string _STATUS_ONBOARDING = "ONBOARDING";

        public static int _FIRM_TYPE_INDIV_RETAIL = 11;

        #endregion

        #region Public Methods

        public int Id { get; set; }

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

        public string IssuerFirmCheckbox { get; set; }

        public string LargeTraderId { get; set; }

        public string UniqueId { get; set; }

        public string Status { get; set; }

        public string LargeTraderFlag { get; set; }

        public int FirmType { get; set; }

        public bool Enabled { get; set; }

        public List<User> Users { get; set; }

        public ShareholderType ShareholderType { get; set; }

        public KoreConXShareholderId KoreConXShareholderId { get; set; }

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
            if (Users != null && Users.Where(x=>x.Email==email).Count()>0)
            {
                return Users.Where(x=>x.Email==email).FirstOrDefault();

            }
            else
                return null;
        }

        public string GetFullName()
        {
            string msg = Name + "(id=" + Id + ")";

            return msg;
        }

        public void MapKCXPersonResponse()
        { 
        
        
        
        
        
        }

        #endregion
    }
}
