namespace Rialto.Common.DTO.Services
{
    public class OnPlaidCredentialsLoadDTO
    {
        public string UserIdentifier { get; set; }
        
        public string PlaidAccessToken { get; set; }

        public string PlaidItemId { get; set; }
    }
}