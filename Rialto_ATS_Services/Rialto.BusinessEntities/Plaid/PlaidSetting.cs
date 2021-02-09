namespace Rialto.BusinessEntities.Plaid
{
    public class PlaidSetting
    {
        #region Public Attributes

        public string EnvName { get; set; }
        
        public string URL { get; set; }
        
        public string ClientId { get; set; }
        
        public string Secret { get; set; }
        
        public bool Enabled { get; set; }
        
        #endregion
        
          
        #region Public Methods

        public bool SecretAndClientIdLoaded()
        {
            return !string.IsNullOrEmpty(ClientId) && !string.IsNullOrEmpty(Secret);

        }

        #endregion
    }
}