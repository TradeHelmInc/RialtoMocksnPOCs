namespace Rialto.Common.DTO.Services
{
    public class OnKCXOnboardingApproved4096DTO
    {
        public string[] Params { get; set; }

        public string GetCSV()
        {
            if (Params != null)
            {
                string csv = "";

                foreach (string param in Params)

                {
                    csv += (csv.Length != 0 ? "," : "") + param;
                }

                return csv;
            }
            else
            {
                return "";
            }

        }
    }
}