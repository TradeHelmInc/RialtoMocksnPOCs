﻿using System;
using System.Security.Permissions;
using Rialto.DataAccessLayer;

namespace Rialto.LogicLayer
{
    public class AuditLogic
    {
        
    #region Public Static Attributes

    public string NEW_ONBOARDING_LAUNCHED = "NEW_ONBORADING_LAUNCHED";
    public string NEW_ONBOARDING_DECR_KCX_PUBLIC_KEY = "NEW_ONBORADING_DECR_KCX_PUBLIC_K";
    public string NEW_ONBOARDING_DECRYPTING_RIALTO_PRIVATE_KEY = "NEW_ONBORADING_DECRYPTING_RIALTO_PRIVATE_KEY";
    public string NEW_ONBOARDING_DECRYPTED_RIALTO_PRIVATE_KEY = "NEW_ONBORADING_DECRYPTED_RIALTO_PRIVATE_KEY";
    public string NEW_ONBOARDING_FETCHING_SHAREHOLDER_FROM_KCX = "NEW_ONBOARDING_FETCHING_SHAREHOLDER_FROM_KCX";
    public string NEW_ONBOARDING_RETRIEVING_SHAREHOLDER_IN_ATS = "NEW_ONBOARDING_RETRIEVING_SHAREHOLDER_IN_ATS";
    public string NEW_ONBOARDING_FOUND_SHAREHOLDER_FOR_KORE_CHAIN = "NEW_ONBOARDING_FOUND_SHAREHOLDER_FOR_KORE_CHAIN";
    public string NEW_ONBOARDING_DECRYPTING_PD = "NEW_ONBOARDING_DECRYPTING_PD";
    public string NEW_ONBOARDING_DECRYPTED_PD = "NEW_ONBOARDING_DECRYPTED_PD";
    public string NEW_ONBOARDING_BUILDING_SHAREHOLDER_FROM_KCX_PD = "NEW_ONBOARDING_BUILDING_SHAREHOLDER_FROM_KCX_PD";
    public string NEW_ONBOARDING_BUILDED_SHAREHOLDER_FROM_KCX_PD = "NEW_ONBOARDING_BUILDED_SHAREHOLDER_FROM_KCX_PD";
    public string NEW_ONBOARDING_FETCH_SHAREHOLDER_BY_TAXID = "NEW_ONBOARDING_FETCH_SHAREHOLDER_BY_TAXID";
    public string NEW_ONBOARDING_FOUND_SHAREHOLDER_BY_TAXID = "NEW_ONBOARDING_FETCH_SHAREHOLDER_BY_TAXID";
    public string NEW_ONBOARDING_UPDATING_ONBOARDING_STATUS = "NEW_ONBOARDING_UPDATING_ONBOARDING_STATUS";
    public string NEW_ONBOARDING_UPDATED_ONBOARDING_STATUS = "NEW_ONBOARDING_UPDATED_ONBOARDING_STATUS";
    public string NEW_ONBOARDING_SENDING_TO_SOLIDUS = "NEW_ONBOARDING_SENDING_TO_SOLIDUS";
    public string NEW_ONBOARDING_SENT_TO_SOLIDUS = "NEW_ONBOARDING_SENT_TO_SOLIDUS";
    public string NEW_ONBOARDING_NOT_FOUND_SHAREHOLDER_BY_TAXID = "NEW_ONBOARDING_NOT_FETCH_SHAREHOLDER_BY_TAXID";
    public string NEW_ONBOARDING_NOT_FOUND_SHAREHOLDER_BY_KORECHAINID = "NEW_ONBOARDING_NOT_FOUND_SHAREHOLDER_BY_KORECHAINID";
    public string NEW_ONBOARDING_NOT_FOUND_BY_KORECHAIN_AND_TAXID = "NEW_ONBOARDING_NOT_FOUND_BY_KORECHAIN_AND_TAXID";
    public string NEW_ONBOARDING_BUILD_SHAREHOLDER_FROM_KCX = "NEW_ONBOARDING_BUILD_SHAREHOLDER_FROM_KCX";
    public string NEW_ONBOARDING_BUILT_SHAREHOLDER_FROM_KCX = "NEW_ONBOARDING_BUILD_SHAREHOLDER_FROM_KCX";

    public string ID_NAME_KORE_SHAREHOLDER_ID = "KoreShareholderId";

    public string ONBOARDING_FAILED = "ONBOARDING_FAILED";
    
    #endregion
        
      #region Protected Attributes

      protected AuditEventManager AuditEventManager { get; set; }

      protected string Principal { get; set; }
      
      #endregion
      
      #region Constructors

      public AuditLogic(string pPrincipal,string pConnectionString)
      {

          AuditEventManager = new AuditEventManager(pConnectionString);

          Principal = pPrincipal;
      }

      #endregion
      
      #region Public Methods

      public void AuditException(String eventType,Exception exception,string message)
      {
          AuditEventManager.InsertAudit(Principal, DateTime.Now, eventType, exception.GetType().ToString(), message);
      }
      
      public void AuditMessage(String eventType,string message,string idName,string idValue)
      {
          AuditEventManager.InsertAudit(Principal, DateTime.Now, eventType, null, message,idName,idValue);
      }
      
      

      #endregion
    }
}