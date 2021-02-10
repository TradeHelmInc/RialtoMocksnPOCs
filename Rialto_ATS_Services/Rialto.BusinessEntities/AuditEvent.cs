using System;

namespace Rialto.BusinessEntities
{
    public class AuditEvent
    {
        #region Public Attributes
        
        public string Principal { get; set; }
        
        public DateTime EventDate { get; set; }
        
        public string EventType { get; set; }
        
        public string ExceptionType { get; set; }

        public string Message { get; set; }

        #endregion
    }
}