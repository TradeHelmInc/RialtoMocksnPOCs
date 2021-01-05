using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.KoreConX.Common.DTO.Shareholders
{
    public class PersonDrivingLicense
    {
        public string number { get; set; }

        public string issued_date { get; set; }

        public string expiry_date { get; set; }

        public string country { get; set; }

        public string state { get; set; }
    }

    public class PersonPassport
    {
        public string number { get; set; }

        public string issued_date { get; set; }

        public string expiry_date { get; set; }

        public string country { get; set; }

        public string state { get; set; }

    }

    public class PersonAddress
    {
        public string address1 { get; set; }

        public string address2 { get; set; }

    }

    public class PersonAmlVerification
    {
        public string tid { get; set; }
        public string providor_id { get; set; }

        public string requestor_id { get; set; }

        public string requestor_source { get; set; }

        public string certificate_id { get; set; }

        public int status { get; set; }

        public string date { get; set; }

        public string expiry_date { get; set; }


    }

    public class PersonBadActorVerification
    {
        public string tid { get; set; }
        public string providor_id { get; set; }

        public string requestor_id { get; set; }

        public string requestor_source { get; set; }

        public string certificate_id { get; set; }

        public int status { get; set; }

        public string date { get; set; }

        public string expiry_date { get; set; }


    }

    public class PersonSuitabilityVerification
    {
        public string tid { get; set; }
        public string providor_id { get; set; }

        public string requestor_id { get; set; }

        public string requestor_source { get; set; }

        public string certificate_id { get; set; }

        public int status { get; set; }

        public string date { get; set; }

        public string expiry_date { get; set; }


    }

    public class PersonAccreditedInvestorVerification
    {
        public string tid { get; set; }

        public string providor_id { get; set; }

        public string requestor_id { get; set; }

        public string requestor_source { get; set; }

        public string certificate_id { get; set; }

        public int status { get; set; }

        public string date { get; set; }

        public string expiry_date { get; set; }


    }

    public class PersonKYCVerificationRules
    {
        public string rule_group { get; set; }

        public string rule_number { get; set; }

        public string rule_version { get; set; }

        public string status { get; set; }

    }

    public class PersonKYCVerification
    {

        public string provider_id { get; set; }

        public string prifile_id { get; set; }

        public string date { get; set; }

        public string id { get; set; }

        public string tier { get; set; }

        public PersonKYCVerificationRules[] rules { get; set; }

        public string overal_status { get; set; }
    }

    public class PersonVerification
    {
        public PersonAmlVerification aml_verification { get; set; }

        public PersonBadActorVerification bad_actor_verification { get; set; }

        public PersonSuitabilityVerification suitability_verification { get; set; }

        public PersonAccreditedInvestorVerification accredited_investor_verification { get; set; }

        public PersonKYCVerification kyc_verification { get; set; }

        public string created_at { get; set; }

        public string updated_at { get; set; }

        public string doc_type { get; set; }
    }

    public class PersonMainInfo
    {

        #region Public Static Consts

        public static string _COUNTRY_USA = "USA";

        public static string _COUNTRY_CODE_US = "US";

        #endregion

        #region Public Attributes

        public string first_name { get; set; }

        public string middle_name { get; set; }

        public string last_name { get; set; }

        public string address { get; set; }

        public PersonAddress addresses { get; set; }

        public string country { get; set; }

        public string email { get; set; }

        public string country_code { get; set; }

        public string phone { get; set; }

        public string date_of_birth { get; set; }

        public string[] national_id { get; set; }

        public PersonDrivingLicense driving_license { get; set; }

        public PersonPassport passport { get; set; }

        public string pd { get; set; }

        public PersonVerification verifications { get; set; }

        #endregion

    }

    public class Person
    {
        public string key { get; set; }

        public PersonMainInfo doc { get; set; }


    }
}
