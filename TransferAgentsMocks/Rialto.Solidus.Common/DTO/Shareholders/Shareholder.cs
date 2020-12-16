using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Solidus.Common.DTO.Shareholders
{
    public class Shareholder
    {

        #region Public Static Consts

        #region GENDER
        public static string _GENDER_MALE = "MALE";
        public static string _GENDER_FEMALE = "FEMALE";
        public static string _GENDER_NON_BINARY = "NON_BINARY";
        public static string _GENDER_PREFER_NOT_TO_IDENTIFY = "PREFER_NOT_TO_IDENTIFY";
        #endregion

        #region NATIONALITY

        public static string _NATIONALITY_AMERICAN = "AMERICAN";

        #endregion

        #region Country

        public static string _COUNTRY_US = "US";

        #endregion

        #region SALARY RANGE

        public static string _SALARY_RANGE_DEFAULT = "0";

        #endregion

        #region INVESTMENT EXPIRIENCE

        public static string _INVESTMENT_EXPIRIENCE_EXPERT = "EXPERT";
        public static string _INVESTMENT_EXPIRIENCE_EXPIRIENCED = "EXPIRIENCED";
        public static string _INVESTMENT_EXPIRIENCE_BEGINNER = "BEGINNER";

        #endregion

        #region COUNTRY_CODE


        public static long COUNTRY_CODE_US = 1; 

        #endregion

        #endregion

        #region Public Attributes

        public string emailAddress { get; set; }

        public string title { get; set; }

        public string firstName { get; set; }

        public string middleName { get; set; }

        public string lastName { get; set; }

        public string suffix { get; set; }

        public string gender { get; set; }

        public string dateOfBirth { get; set; }

        public string nationality { get; set; }

        public string taxIdOrSSNNumber { get; set; }

        public string residenceStreetAddress { get; set; }

        public string residenceCountry { get; set; }

        public string residencePostalCode { get; set; }

        public string residenceCity { get; set; }

        public string residenceState { get; set; }

        public long phoneCountryCode { get; set; }

        public long phoneNumber { get; set; }

        public string employer { get; set; }

        public string jobTitle { get; set; }

        public string salaryRange { get; set; }

        public double? netWorth { get; set; }

        public string investmentExperience { get; set; }

        public bool? accreditedInvestor { get; set; }

        #endregion

        #region Public Methods

        public string GetFullName()
        {
            string name = "";

            if (firstName != null)
                name += firstName + " " ;

            if (middleName != null)
                name += middleName + " ";

            if (lastName != null)
                name += lastName + " ";

            return name;
            
        }

        public string GetAddress()
        {
            string address = "";

            if (residenceStreetAddress != null)
                address += residenceStreetAddress + " ";

            if (residenceCity != null)
                address += residenceCity + " ";

            if (residenceState != null)
                address += residenceState + " ";

            if (residencePostalCode != null)
                address += " ZIP " + residencePostalCode + " ";


            return address.Substring(0, 200);
        
        
        }

        public string GetPhone()
        {
            string phone = phoneCountryCode + " " + phoneNumber;

            return phone.Substring(0, 200);
        }


        #endregion
    }
}
