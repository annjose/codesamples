using System;
using System.IO;
using System.Net;
using IPPUtils;
using Microsoft.Practices.Unity;

namespace IAMDataService.OneIntuitIdentity
{
    public interface IOneIntuitIdentityService
    {
        bool VerifyTicket(string ticket, string id, string agentId, string remoteIp);
        GetRealmIdFromTicketResponse GetRealmIdFromTicket(string ticket, string id, string agentId, string remoteIp);
        bool Authenticate(string username, string password, string hostAddress, out AccountSessionMsg accountSessionMsg);
    }

    /// <summary>
    /// Mock for IOneIntuitIdentityService to be used during AirplaneMode.
    /// </summary>
    public class MockOneIntuitIdentityService : IOneIntuitIdentityService
    {
        public bool VerifyTicket(string ticket, string id, string agentId, string remoteIp)
        {
            return true;
        }

        public GetRealmIdFromTicketResponse GetRealmIdFromTicket(string ticket, string id, string agentId, string remoteIp)
        {
            throw new NotImplementedException();
        }

        public bool Authenticate(string username, string password, string hostAddress, out AccountSessionMsg accountSessionMsg)
        {
            throw new NotImplementedException();
        }

        public string SendTicket(string ticket, string id, string agentId, string remoteIp)
        {
            throw new NotImplementedException();
        }
    }

    public class OneIntuitIdentityService : IOneIntuitIdentityService
    {
        private const string Host = "https://accounts-e2e.platform.intuit.com/";
        private const string TicketEndpointWithQueryParam = "v1/iamtickets/{0}?{1}={2}";
        private const string TicketEndpoint = "v1/iamtickets";

        protected static ILogger log = ContainerLocator.Container.Resolve<ILogger>(new ParameterOverride("callerMethod", System.Reflection.MethodBase.GetCurrentMethod()));

        /// <summary>
        /// Successful response 
        /// {"realmId":"50000003","authenticationLevel":"20","agentId":"250237121","access":"null","ticket":"V1-193-Q392pf2m06ouoedyhqgul4","namespaceId":"50000003","userId":"250237121"}
        /// Error response
        /// {"responseCode":"INVALID_IAM_TICKET","responseMessage":"Ticket verification failed - 000 Success DENIED (not found)"}
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="id"></param>
        /// <param name="agentId"></param>
        /// <param name="remoteIp"></param>
        /// <returns></returns>
        public bool VerifyTicket(string ticket, string id, string agentId, string remoteIp)
        {
            IAMTicketModel ticketModel = new IAMTicketModel();

            HttpWebRequest request = CreateIamServiceRequest(Host + string.Format(TicketEndpointWithQueryParam, ticket, "user_id", id));
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        ticketModel = JsonUtil.Deserialize<IAMTicketModel>(content);

                    }

                }
            }

            catch (WebException we)
            {
                return HandleError(GetErrorObject(we).responseCode);
            }

            return (ticketModel != null) && (ticketModel.responseCode == null);
        }


        public GetRealmIdFromTicketResponse GetRealmIdFromTicket(string ticket, string id, string agentId, string remoteIp)
        {
            IAMTicketModel ticketModel = new IAMTicketModel();

            HttpWebRequest request = CreateIamServiceRequest(Host + string.Format(TicketEndpointWithQueryParam, ticket, "user_id", id));
            ApplicationType app = new ApplicationType();
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        ticketModel = JsonUtil.Deserialize<IAMTicketModel>(content);

                    }

                }
            }

            catch (WebException we)
            {
                HandleError(GetErrorObject(we).responseCode);
            }

            if ((ticketModel != null) && (ticketModel.responseCode == null))
            {
                return new GetRealmIdFromTicketResponse(true, ticketModel.realmId);
            }

            return new GetRealmIdFromTicketResponse(false, null);
        }


        public bool Authenticate(string username, string password, string hostAddress, out AccountSessionMsg accountSessionMsg)
        {
            IAMTicketModel ticketModel = new IAMTicketModel();

            HttpWebRequest request = CreateIamServiceRequest(Host + TicketEndpoint);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("intuit_originatingip",hostAddress);

            //construct the request payload
            SignInModel signin = new SignInModel { username = username, password = password, namespaceId = Config.IAMNamespaceID };
            try
            {

                using (Stream postStream = request.GetRequestStream())
                {
                    var data = JsonUtil.Serialize(signin);

                    using (StreamWriter requestWriter = new StreamWriter(postStream, System.Text.Encoding.ASCII))
                    {
                        requestWriter.Write(data);
                    }
                }


                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        ticketModel = JsonUtil.Deserialize<IAMTicketModel>(content);

                    }

                }

                accountSessionMsg = new AccountSessionMsg { authId = ticketModel.userId, agentId = ticketModel.agentId, errorCode = ticketModel.responseCode, errorMessage = ticketModel.responseMessage, loginName = username, ticket = ticketModel.ticket };
            }

            catch (WebException we)
            {
                ticketModel = GetErrorObject(we);
                accountSessionMsg = new AccountSessionMsg { authId = ticketModel.userId, agentId = ticketModel.agentId, errorCode = ticketModel.responseCode, errorMessage = ticketModel.responseMessage, loginName = username, ticket = ticketModel.ticket };
                return IamClientUtil.GetIAMErrorCodeEnum(ticketModel.responseCode) == IAMError.IAMErrorCodes.OK;
            }

            return (ticketModel.responseCode == null);
        }

        private IAMTicketModel GetErrorObject(WebException we)
        {
            HttpWebResponse response = (HttpWebResponse) we.Response;
            string statusCode = response.StatusCode.ToString();
            log.Info("AuthorizeRequestToken-->Got Status Code: " + statusCode, we);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string errorPayload = LogPayload(reader);
                // Need to seek to the beginning or else the deserialization will fail since
                // we've already read to the end of the stream in LogPayload
                if (reader.BaseStream.CanSeek)
                {
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                }
                IAMTicketModel ticketModel = JsonUtil.Deserialize<IAMTicketModel>(reader.ReadToEnd());

                return ticketModel;
            }

        }

        private static bool HandleError(string errorCode)
        {        
            string errorMsg = "An error has occured while authorizing this application.";

                switch (errorCode)
                {
                    case "INVALID_IAM_TICKET":
                        errorMsg = "Ticket verification failed - 000 Success DENIED (not found)";
                        return false;
                        break;
                    case "INVALID_CREDENTIALS":
                        errorMsg = "Invalid Credentials.";
                        return false;
                        break;
                    default:
                        //Use default message 
                        break;
                }
            
            throw new Exception(errorMsg);           
        }

        /// <summary>
        /// Logs the contents of the StreamReader if the log level is set to Debug.
        /// </summary>
        /// <param name="reader">The StreamReader to Dump.</param>
        private string LogPayload(StreamReader reader)
        {
            string x = reader.ReadToEnd();
            log.Info(x);
            return x;
        }

        /// <summary>
        /// Creates a WebRequest with the standard headers already attached.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateIamServiceRequest(string uri)
        {
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            request.Headers.Add("Authorization", "Intuit_IAM_Authentication intuit_appid=Intuit.cto.iam.ius.tests,intuit_app_secret=F3MVISrnOmHsz7Y1Fzwvb7");
            request.Accept = "application/json";
            return request;
        }

    }
}
