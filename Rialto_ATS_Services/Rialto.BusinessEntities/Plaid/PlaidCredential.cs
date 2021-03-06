﻿namespace Rialto.BusinessEntities.Plaid
{
    public class PlaidCredential
    {
        #region Public Attributes
        
        public Shareholder Shareholder { get; set; }

        public User User { get; set; }

        public string UserIdentifier { get; set; }
        
        public string AccessToken { get; set; }
        
        public string PlaidItemId { get; set; }
        
        public string ClientId { get; set; }
        
        public string Secret { get; set; }


        #endregion
      
    }
}