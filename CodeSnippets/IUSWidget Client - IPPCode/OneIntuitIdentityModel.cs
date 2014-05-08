namespace IAMDataService.OneIntuitIdentity
{
    public class IAMTicketModel : ErrorModel
    {
        public string ticket;

        public string userId;

        public string agentId;

        public string realmId;

        public IAMRole role;

        public string access;
    }

    public class IAMRole
    {
        public string name;
        public string roleType;
    }

    public class ErrorModel
    {
        public string responseCode;
        public string responseMessage;
    }

    public class GetRealmIdFromTicketResponse
    {
        public GetRealmIdFromTicketResponse(bool valid, string realmId)
        {
            this.Valid = valid;
            this.RealmId = realmId;
        }
        public bool Valid { get; set; }
        public string RealmId { get; set; }
    }

    public class SignInModel
    {
        public string username;
        public string password;
        public string namespaceId;

    }

    public class AccountSessionMsg
    {
        private string loginNameField;

        private string authIdField;

        private string agentIdField;

        private string namespaceIdField;

        private string ticketField;

        private string errorCodeField;

        private string errorMessageField;

        public string loginName
        {
            get
            {
                return this.loginNameField;
            }
            set
            {
                this.loginNameField = value;
            }
        }
      
        public string authId
        {
            get
            {
                return this.authIdField;
            }
            set
            {
                this.authIdField = value;
            }
        }

       
        public string agentId
        {
            get
            {
                return this.agentIdField;
            }
            set
            {
                this.agentIdField = value;               
            }
        }
      
        public string namespaceId
        {
            get
            {
                return this.namespaceIdField;
            }
            set
            {
                this.namespaceIdField = value;
               
            }
        }

        public string ticket
        {
            get
            {
                return this.ticketField;
            }
            set
            {
                this.ticketField = value;              
            }
        }
      
        public string errorCode
        {
            get
            {
                return this.errorCodeField;
            }
            set
            {
                this.errorCodeField = value;               
            }
        }

        public string errorMessage
        {
            get
            {
                return this.errorMessageField;
            }
            set
            {
                this.errorMessageField = value;
            }
        }

    }

}
