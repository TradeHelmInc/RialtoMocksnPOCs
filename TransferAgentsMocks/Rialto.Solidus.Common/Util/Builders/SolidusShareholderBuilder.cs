﻿using Rialto.KoreConX.Common.DTO.Shareholders;
using Rialto.Solidus.Common.DTO.Shareholders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Solidus.Common.Util.Builders
{
    public class SolidusShareholderBuilder
    {
        #region Private StaticConsts

        private static string _KCX_DATE_OF_BIRTH_FORMAT = "yyyy-MM-dd";
        private static string _SOLIDUS_DATE_OF_BIRTH_FORMAT = "yyyy-MM-dd";

        #endregion

        #region Private Static Methods

        #region KoreConX

        private static string GetPersonBasicIdsFromKCX(PersonMainInfo person)
        {
            if (person.email!=null ||person.first_name != null || person.last_name == null || (person.national_id != null && person.national_id.Length > 0))
            {
                string shareholderDesc = "";

                if (person.national_id != null && person.national_id.Length > 0)
                    shareholderDesc += string.Format(" NationalId = {0} ", person.national_id[0]);

                if (person.first_name != null)
                    shareholderDesc += string.Format(" First Name={0} ", person.first_name);

                if (person.last_name != null)
                    shareholderDesc += string.Format(" LastName={0} ", person.last_name);

                if (person.email != null)
                    shareholderDesc += string.Format(" email={0} ", person.email);

                return shareholderDesc;
            }
            else
                return "unknown shareholder";
        }

        private static void ValidateKCXPersonMainInfo(PersonMainInfo person)
        {
            if (person == null)
                throw new Exception("Cannot send a null person to Solidus");

            if (person.email == null)
                throw new Exception(string.Format("The following shareholder DOES NOT have an email wich is a mandatory field: {0}", GetPersonBasicIdsFromKCX(person)));

        }

        private static string ConvertKCXDateOfBirth(string dateOfBirth)
        {

            if (dateOfBirth == null)
            {
                try
                {


                    DateTime birth = DateTime.ParseExact(dateOfBirth, _KCX_DATE_OF_BIRTH_FORMAT, CultureInfo.InvariantCulture);

                    return birth.ToString(_SOLIDUS_DATE_OF_BIRTH_FORMAT);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Date of birth '{0}' must match KCX format '{1}'", dateOfBirth, _KCX_DATE_OF_BIRTH_FORMAT));
                }
            }
            else
                return null;
        
        }

        private static string BuildNationalityFromKCX(PersonPassport passport)
        {
            if (passport != null)//Both entities are supposed to use the same ISO-3166 2-digit country codes
                return passport.country;
            else
                return null;
        }

        private static string BuildNationalIdFromKCX(string[] nationalId)
        {

            if (nationalId != null && nationalId.Length > 0)
                return nationalId[0];
            else
                return null;
        }

        private static string BuildCountryFromKCX(string country)
        {
            return country;//Both entities are supposed to use the same ISO-3166 2-digit country codes
        }

        private static long BuildCountryCodeFromKCX(string countryCode)
        {
            if (countryCode == null)
            {
                if (countryCode == PersonMainInfo._COUNTRY_CODE_US)
                    return Shareholder.COUNTRY_CODE_US = 1;
                else
                    throw new Exception(string.Format("Country Code '{0}' not supported. Contact administrator",countryCode));
            }
            else
                return 0;
        
        }

        private static long BuildPhoneFromKCX(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                try
                {
                    return Convert.ToInt64(phone.Trim());

                }
                catch(Exception ex)
                {
                    throw new Exception(string.Format("Phone '{0}' is not a valid format. Only numbers accepted. Contact administrator", phone));
                }
            }
            else
                return 0;
        
        }

        private static string BuildSalaryRangeFromKCX(string salaryRange)
        {
            if (string.IsNullOrEmpty(salaryRange))
            {
                return Shareholder._SALARY_RANGE_DEFAULT;
            }
            else
                return null;
        
        }

        private static string BuildInvestmentExpirienceFromKCX(string invExp)
        {
            return null;
        }

        #endregion

        #endregion

        #region Public Static Methods

        public static Shareholder BuildSolidusShareholderFromKCX(PersonMainInfo person)
        {
            Shareholder solidusShareholder = new Shareholder();

            solidusShareholder.emailAddress = person.email;
            solidusShareholder.title = "";
            solidusShareholder.firstName = person.first_name;
            solidusShareholder.middleName = person.middle_name;
            solidusShareholder.lastName = person.last_name;
            solidusShareholder.suffix = "";
            solidusShareholder.gender = Shareholder._GENDER_PREFER_NOT_TO_IDENTIFY;
            solidusShareholder.dateOfBirth = ConvertKCXDateOfBirth(person.date_of_birth);
            solidusShareholder.nationality = BuildNationalityFromKCX(person.passport);
            solidusShareholder.taxIdOrSSNNumber = BuildNationalIdFromKCX(person.national_id);
            solidusShareholder.residenceStreetAddress = person.address;
            solidusShareholder.residenceCountry = BuildCountryFromKCX(person.country);
            solidusShareholder.phoneCountryCode = BuildCountryCodeFromKCX(person.country_code);
            solidusShareholder.phoneNumber = BuildPhoneFromKCX(person.phone);
            solidusShareholder.employer = "";
            solidusShareholder.jobTitle = "";
            solidusShareholder.salaryRange = BuildSalaryRangeFromKCX(null);
            solidusShareholder.netWorth = null;
            solidusShareholder.investmentExperience = BuildInvestmentExpirienceFromKCX(null);
            solidusShareholder.accreditedInvestor = null;

            return solidusShareholder;
        
        }

        #endregion
    }
}
