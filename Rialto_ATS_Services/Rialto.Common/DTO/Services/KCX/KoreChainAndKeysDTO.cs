namespace Rialto.Common.DTO.Services
{
    public class Encryption
    {
        public string Algorithm { get; set; }
        
        public string Mode { get; set; }
        
        public string Key_Size_bits { get; set; }
        
        public string IV { get; set; }

        public string Secret_Key { get; set; }


    }
    
    public class KoreChainAndKeysDTO
    {
        public string KoreChainId { get; set; }
        
        public Encryption Encryption { get; set; }
        
        public string Nonce { get; set; }
    }
}