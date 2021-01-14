﻿namespace Rialto.BusinessEntities.Plaid
{
    public class PlaidCredential
    {
        #region Public Attributes

        public User User { get; set; }

        public string UserIdentifier { get; set; }
        
        public string AccessToken { get; set; }
        
        public string PlaidItemId { get; set; }


        #endregion
    }
}