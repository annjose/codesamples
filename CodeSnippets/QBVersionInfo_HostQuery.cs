using System;
using System.Collections.Generic;
using Interop.QBXMLRP2;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using Intuit.Ctg.Map;
using QBInstanceFinder;

namespace Intuit.Ctg.Wte.Service.Import.QuickBooks
{
    /// <summary>
    /// Class that wraps calls via QB SDK
    /// </summary>
    public class QBSDKConnector : IQBConnector
    {
        // Constants
        private const string TurboTaxApplicationID = "TurboTax";
        private static QBVersion MaxSupportedQBVersion = new QBVersion(22, 0);  // Version: '22.0' - QB 2012 (Blaze)
        private static QBVersion MinSupportedQBVersion = new QBVersion(16, 0);  // Version: '16.0' - QB 2006 

        // Minimum supported version of QB XML (for HostQuery API)
        private static QBVersion MinQBXmlVersion = new QBVersion(2, 0);  // Version: '2.0' 

        // Private data that cannot be set from outside
        /// <summary>
        /// Get accessor for QB App Info
        /// </summary>
        public QBAppInfo CurrentAppInfo { get; private set; }

        /// <summary>
        /// version of the QB installed on the machine
        /// </summary>
        public QBVersion CurrentQBVerison;

        private string currentSessionTicket;
        RequestProcessor2 reqProcessor;

        /// <summary>
        /// Constructor
        /// </summary>
        public QBSDKConnector()
        {
            QBQueryParams = new QueryParams();
        }

        #region IQBConnector implementation

        /// <summary>
        /// Type of connector - in this case, QB SDK Wrapper
        /// </summary>
        public ConnectorType ConnectorType
        {
            get { return ConnectorType.QBSDKConnector; }
        }

        /// <summary>
        /// Tells if the connection is open
        /// </summary>
        /// <returns></returns>
        public bool IsConnectionOpen { get; private set; }

        /// <summary>
        /// Company File info (only get accessor is exposed)
        /// </summary>
        public CompanyInfo CurrentCompanyInfo { get; private set; }

        /// <summary>
        /// Parameters that will be used for QB Import
        /// </summary>
        public QueryParams QBQueryParams { get; set; }

        /// <summary>
        /// Accounts that can be imported to TurboTax (set accessor can be called by clients who want to store them)
        /// </summary>
        /// <returns></returns>
        public List<Account> Accounts { get; set; }

        /// <summary>
        /// Checks if application can connect to QB
        /// </summary>
        /// <returns>Status</returns>
        public QBConnectStatus CanConnect()
        {
            QBConnectStatus canConnectStatus = QBConnectStatus.ErrorQBNotInstalled;

            try
            {
                // Get verison of QB installed on the user machine using InstanceFinder
                QBVersion currentQBVersion = GetQuickBooksVersion();
                if (currentQBVersion != null)
                {
                    if (currentQBVersion >= MinSupportedQBVersion)
                    {
                        if (currentQBVersion <= MaxSupportedQBVersion)
                        {
                            canConnectStatus = QBConnectStatus.Success;
                        }
                        else
                        {
                            canConnectStatus = QBConnectStatus.ErrorCurrentVersionHigherThanMaxSupported;
                        }
                    }
                    else
                    {
                        canConnectStatus = QBConnectStatus.ErrorCurrentVersionLowerThanMinSupported;
                    }
                }
                CurrentQBVerison = currentQBVersion;

            }
            catch (Exception ex)
            {
                LogError("QBSDKConnector.CanConnect()", "Exception occurred while checking QB versions Possible reason is that QB is not installed.", ex);
            }


            return canConnectStatus;
        }

        private QBVersion GetQuickBooksVersion()
        {
            QBVersion qbVersion = null;
            InstalledInstanceFinder qbInstanceFinder = null;
            try
            {
                qbInstanceFinder = new InstalledInstanceFinder();

                IInstalledInstanceInfo qbInstanceInfo =
                    qbInstanceFinder.GetInstallation(
                        "", FLAVORMATCHTYPE.ANY_FLAVOR, 1, VERSIONMATCHTYPE.ANY_VERSION, VERSIONTYPE.MATCH_MAJOR_VERSION);
                if (qbInstanceInfo != null)
                {
                    qbVersion = new QBVersion(qbInstanceInfo.majorVersion, 0);
                    Logger.Instance().Info("QB Exe path" + qbInstanceInfo.exePath + "Major version: " + qbInstanceInfo.majorVersion);
                }
            }
            catch (Exception ex)
            {
                // An exception occurred. That means QBInstanceFinder is no registered, i.e. QB is not installed on this machine.
                LogError("QBSDKConnector.GetQuickBooksVersion()", "Exception occurred while checking QB versions Possible reason is that QB is not installed.", ex);
            }

            return qbVersion;
        }
        /// <summary>
        /// Gives a list of companies that a connector can connect to
        /// </summary>
        /// <returns></returns>
        public List<CompanyInfo> GetCompanies()
        {
            List<CompanyInfo> companies = new List<CompanyInfo>();

            // Read the registry key to get the MRU list of QB company files
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Intuit\QuickBooksCommon\QBFinder");
            if ((regKey != null) && (regKey.ValueCount > 0))
            {
                string[] valueNames = regKey.GetValueNames();
                foreach (string valueName in valueNames)
                {
                    string value = regKey.GetValue(valueName) as string;

                    // registry value is in format d:\datafiles\qb\2011\qb_1065.qbw|21|pro   Split it and extract file name with path
                    string fileNameWithPath = value.Split(new char[] { '|' }).GetValue(0) as string;
                    FileInfo fileInfo = new FileInfo(fileNameWithPath);
                    if (fileInfo != null && fileInfo.Exists)
                    {
                        CompanyInfo companyInfo = new CompanyInfo();

                        // QB stores File MRU list in the registry in lower-case; it doesn't look good on the UI. Hence we need to get the file name in the originial case (as is stored on disk)
                        //  the only way to get that is to locate the file on disk and read attributes
                        string[] filesInFolder = Directory.GetFiles(fileInfo.DirectoryName);
                        foreach (string FileNameWithPathAsOnDisk in filesInFolder)
                        {
                            if (string.Compare(FileNameWithPathAsOnDisk, fileNameWithPath, true) == 0)
                            {
                                FileInfo fileInfoAsOnDisk = new FileInfo(FileNameWithPathAsOnDisk);
                                if (fileInfoAsOnDisk != null)
                                {
                                    companyInfo.FileName = fileInfoAsOnDisk.Name;
                                    companyInfo.FileLocation = fileInfoAsOnDisk.DirectoryName;
                                    companyInfo.FilePath = fileInfoAsOnDisk.FullName;
                                    companyInfo.LastModifiedDate = fileInfoAsOnDisk.LastWriteTime;

                                    companies.Add(companyInfo);
                                }
                            }
                        }
                    }
                }
            }

            return companies;
        }

        /// <summary>
        /// Launch QuickBooks
        /// </summary>
        public void LaunchQuickBooks() { }

        /// <summary>
        /// Connect to a company file
        /// </summary>
        /// <param name="companyFile"></param>
        /// <returns></returns>
        public QBConnectStatus Connect(string companyFile)
        {
            QBConnectStatus connectionSuccess = QBConnectStatus.ErrorUnknown;

            // Create a request processor
            //  CAUTION: You must use the same instance of the RequestProcessor to call all SDK APIs
            reqProcessor = new RequestProcessor2();

            try
            {
                LogInfo("QBSDKConnector.Connect()", "Connecting to QuickBooks company file '" + companyFile + "'");

                reqProcessor.OpenConnection2(TurboTaxApplicationID, TurboTaxApplicationID, QBXMLRPConnectionType.localQBD);
                string ticket = reqProcessor.BeginSession(companyFile, QBFileMode.qbFileOpenDoNotCare);
                if (!string.IsNullOrEmpty(ticket))
                {
                    LogInfo("QBSDKConnector.Connect()", "Successfully connected to company file '" + companyFile + "'");
                    connectionSuccess = QBConnectStatus.Success;

                    IsConnectionOpen = true;
                    currentSessionTicket = ticket;

                    CurrentAppInfo = GetQBAppInfo();
                    CurrentCompanyInfo = GetCompanyInfo(companyFile);
                }
            }
            catch (COMException comEx)
            {
                // An error occurred. Do the clean up and exception handling
                LogError("QBSDKConnector.Connect()", "QBSDK API BeginSession threw COMException. Exception Message: '" + comEx.Message + " '", comEx);
                connectionSuccess = GetConnectStatus(comEx.Message);

                CloseConnection();

                CurrentCompanyInfo = null;
                CurrentAppInfo = null;
            }
            return connectionSuccess;
        }

        /// <summary>
        /// Disconnect from QB company
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (reqProcessor != null && !string.IsNullOrEmpty(currentSessionTicket))
                {
                    reqProcessor.EndSession(currentSessionTicket);
                    CloseConnection();

                    string companyFile = CurrentCompanyInfo != null ? CurrentCompanyInfo.FilePath : string.Empty;
                    LogError("QBSDKConnector.Disconnect()", "Closed connection to QuickBooks Company File: " + companyFile);
                }
            }
            catch (COMException comEx)
            {
                string companyFile = CurrentCompanyInfo != null ? CurrentCompanyInfo.FilePath : string.Empty;
                LogError("QBSDKConnector.Disconnect()", "QBSDK API EndSession threw COMException. Exception Message: '" + comEx.Message + " '; Company File: " + companyFile, comEx);
            }
        }


        // Close the QB connection via the request processor
        private void CloseConnection()
        {
            if (reqProcessor != null)
            {
                reqProcessor.CloseConnection();
                reqProcessor = null;
            }

            IsConnectionOpen = false;
            currentSessionTicket = string.Empty;
        }

        private QBConnectStatus GetConnectStatus(string message)
        {
            QBConnectStatus connectStatus = QBConnectStatus.ErrorUnknown;

            if (message.Contains("A QuickBooks company data file is already open and it is different from the one requested."))
            {
                connectStatus = QBConnectStatus.ErrorAnotherCompanyFileOpen;
            }
            else if (message.Contains("A modal dialog box is showing in the QuickBooks user interface"))
            {
                connectStatus = QBConnectStatus.ErrorModalDialogBoxOpen;
            }
            else if (message.Contains("Could not start QuickBooks"))
            {
                connectStatus = QBConnectStatus.ErrorCannotConnect;
            }
            else if (string.IsNullOrEmpty(message))
            {
                connectStatus = QBConnectStatus.Success;
            }
            return connectStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Account> GetChartOfAccounts()
        {
            List<Account> chartOfAccounts = new List<Account>();

            StringBuilder qbRequestXML = new StringBuilder();
            qbRequestXML.AppendFormat("<?xml version=\"1.0\"?><?qbxml version=\"{0}\"?><QBXML>", CurrentAppInfo.MaxSupportedQbXmlVersion);
            qbRequestXML.Append("<QBXMLMsgsRq onError = \"stopOnError\"><AccountQueryRq requestID = \"1\" ></AccountQueryRq></QBXMLMsgsRq>");
            qbRequestXML.Append("</QBXML>");

            string responseXml = reqProcessor.ProcessRequest(currentSessionTicket, qbRequestXML.ToString());

            XElement responseXmlRoot = XElement.Parse(responseXml);
            if (responseXmlRoot != null)
            {
                XElement accountRespElement = responseXmlRoot.Element("QBXMLMsgsRs").Element("AccountQueryRs");
                if (accountRespElement != null)
                {
                    QBResponseStatus responseStatus = ParseStatus(accountRespElement);
                    if (responseStatus.StatusCode == 0)
                    {
                        foreach (XElement accountRetElement in accountRespElement.Elements("AccountRet"))
                        {
                            Account account = new Account();

                            account.ListID = accountRetElement.Element("ListID") != null ? accountRetElement.Element("ListID").Value : string.Empty;
                            account.Name = accountRetElement.Element("Name") != null ? accountRetElement.Element("Name").Value : string.Empty;
                            account.FullName = accountRetElement.Element("FullName") != null ? accountRetElement.Element("FullName").Value : string.Empty;
                            account.Description = accountRetElement.Element("Desc") != null ? accountRetElement.Element("Desc").Value : string.Empty;

                            string acctType = accountRetElement.Element("AccountType") != null ? accountRetElement.Element("AccountType").Value : string.Empty;
                            QBAccountType qbAcctType = QBAccountType.Unknown;
                            account.Type = Enum.TryParse<QBAccountType>(acctType, true, out qbAcctType) ? qbAcctType : QBAccountType.Unknown;

                            string acctNumberString = accountRetElement.Element("AccountNumber") != null ? accountRetElement.Element("AccountNumber").Value : string.Empty;
                            int acctNumber = 0;
                            account.Number = Int32.TryParse(acctNumberString, out acctNumber) ? acctNumber : 0;

                            XElement isActiveElement = accountRetElement.Element("IsActive");
                            if (isActiveElement != null)
                            {
                                account.IsActive = isActiveElement.Value == "true" ? true : false;
                            }

                            XElement taxInfoRetElement = accountRetElement.Element("TaxLineInfoRet");
                            if (taxInfoRetElement != null)
                            {
                                TaxLine taxLine = new TaxLine();
                                string taxLineIdStr = taxInfoRetElement.Element("TaxLineID").Value; ;
                                int taxLineId = 0;
                                taxLine.ID = Int32.TryParse(taxLineIdStr, out taxLineId) ? taxLineId : 0;
                                taxLine.Name = taxInfoRetElement.Element("TaxLineName").Value;

                                account.TaxLine = taxLine;
                            }

                            chartOfAccounts.Add(account);
                        }
                    }
                }
            }

            return chartOfAccounts;
        }

        /// <summary>
        /// Gets accounts from Profit and Loss report
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="accountingMethod"></param>
        /// <returns></returns>
        public List<Account> GetPAndLReportAccounts(DateTime startDate, DateTime endDate, AccountingMethod accountingMethod)
        {
            string startDateStr = startDate.ToString("yyyy-MM-dd");
            string endDateStr = endDate.ToString("yyyy-MM-dd");

            List<Account> pAndLAccounts = new List<Account>();

            StringBuilder qbRequestXML = new StringBuilder();

            qbRequestXML.AppendFormat("<?xml version=\"1.0\"?><?qbxml version=\"{0}\"?>", CurrentAppInfo.MaxSupportedQbXmlVersion);
            qbRequestXML.Append("<QBXML>");
            qbRequestXML.Append("<QBXMLMsgsRq onError = \"stopOnError\">");
            qbRequestXML.Append("<GeneralSummaryReportQueryRq requestID = \"1\" >");
            qbRequestXML.Append("<GeneralSummaryReportType>ProfitAndLossStandard</GeneralSummaryReportType>");
            qbRequestXML.Append("<ReportPeriod>");
            qbRequestXML.Append("<FromReportDate>").Append(startDateStr).Append("</FromReportDate>");
            qbRequestXML.Append("<ToReportDate>").Append(endDateStr).Append("</ToReportDate>");
            qbRequestXML.Append("</ReportPeriod>");
            qbRequestXML.Append("<SummarizeColumnsBy>TotalOnly</SummarizeColumnsBy>");
            qbRequestXML.Append("<ReportBasis>").Append(accountingMethod).Append("</ReportBasis>");
            qbRequestXML.Append("</GeneralSummaryReportQueryRq>");
            qbRequestXML.Append("</QBXMLMsgsRq>");
            qbRequestXML.Append("</QBXML>");

            string responseXml = reqProcessor.ProcessRequest(currentSessionTicket, qbRequestXML.ToString());

            pAndLAccounts = ParseReportResponse(responseXml);

            return pAndLAccounts;
        }

        /// <summary>
        /// Parse the QB response XML that contains report data and extracts the accounts from it
        /// </summary>
        /// <param name="responseXml"></param>
        /// <returns></returns>
        private List<Account> ParseReportResponse(string responseXml)
        {
            List<Account> accountsInReport = new List<Account>();

            XElement responseXmlRoot = XElement.Parse(responseXml);
            if (responseXmlRoot != null)
            {
                XElement reportRespElement = responseXmlRoot.Element("QBXMLMsgsRs").Element("GeneralSummaryReportQueryRs");

                QBResponseStatus responseStatus = ParseStatus(reportRespElement);

                XElement reportDataElement = null;
                if (responseStatus.StatusCode == 0)
                {
                    reportDataElement = reportRespElement.Element("ReportRet").Element("ReportData");

                    IEnumerable<XElement> dataRowElements = reportDataElement.Elements("DataRow");
                    foreach (XElement dataRowElement in dataRowElements)
                    {
                        if ((dataRowElement != null) && (dataRowElement.Element("RowData") != null))
                        {
                            Account accountInReport = new Account();

                            // Get the parent account name from row data (this is of format "<ParentAccount>: <Account>")
                            XElement rowDataElement = dataRowElement.Element("RowData");
                            accountInReport.FullName = rowDataElement.Attribute("value").Value;

                            // Get the account name and amount from the column data
                            List<XElement> colDataElements = dataRowElement.Elements("ColData").ToList();
                            if (colDataElements != null && colDataElements.Count >= 2)
                            {
                                foreach (var colDataElement in colDataElements)
                                {

                                    if (colDataElement.Attribute("colID").Value == "1")
                                    {
                                        // This is the account name
                                        accountInReport.Name = colDataElement.Attribute("value").Value.Trim();
                                    }
                                    else if (colDataElement.Attribute("colID").Value == "2")
                                    {
                                        // This is the amount
                                        string amountStr = colDataElement.Attribute("value").Value.Trim();
                                        double amount = 0;
                                        accountInReport.Amount = Double.TryParse(amountStr, out amount) ? Math.Round(amount, 2, MidpointRounding.AwayFromZero) : 0;
                                    }
                                }
                            }
                            accountsInReport.Add(accountInReport);
                        }
                    }
                }
            }

            return accountsInReport;
        }

        /// <summary>
        /// Gets accounts from Balance Sheet report
        /// </summary>
        /// <param name="accountingMethod"></param>
        /// <returns></returns>
        public List<Account> GetBalanceSheetReportAccounts(AccountingMethod accountingMethod)
        {
            List<Account> balanceSheetAccounts = new List<Account>();

            StringBuilder qbRequestXML = new StringBuilder();

            qbRequestXML.AppendFormat("<?xml version=\"1.0\"?><?qbxml version=\"{0}\"?>", CurrentAppInfo.MaxSupportedQbXmlVersion);
            qbRequestXML.Append("<QBXML>");
            qbRequestXML.Append("<QBXMLMsgsRq onError = \"stopOnError\">");
            // replace with balance sheet query
            // qbRequestXML.Append("<GeneralSummaryReportQueryRq requestID = \"1\" >");
            // qbRequestXML.Append("<GeneralSummaryReportType>ProfitAndLossStandard</GeneralSummaryReportType>");
            qbRequestXML.Append("<ReportPeriod>");
            qbRequestXML.Append("</ReportPeriod>");
            qbRequestXML.Append("<SummarizeColumnsBy>TotalOnly</SummarizeColumnsBy>");
            qbRequestXML.Append("<ReportBasis>").Append(accountingMethod).Append("</ReportBasis>");
            qbRequestXML.Append("</GeneralSummaryReportQueryRq>");
            qbRequestXML.Append("</QBXMLMsgsRq>");
            qbRequestXML.Append("</QBXML>");

            string responseXml = reqProcessor.ProcessRequest(currentSessionTicket, qbRequestXML.ToString());

            balanceSheetAccounts = ParseReportResponse(responseXml);

            return balanceSheetAccounts;
        }

        #endregion

        #region Util functions

        /// <summary>
        /// Gets the info about the specified company file
        /// </summary>
        /// <param name="companyFile"></param>
        /// <returns></returns>
        private CompanyInfo GetCompanyInfo(string companyFile)
        {
            CompanyInfo companyInfo = null;

            StringBuilder qbRequestXML = new StringBuilder();
            qbRequestXML.AppendFormat("<?xml version=\"1.0\"?><?qbxml version=\"{0}\"?><QBXML>", CurrentAppInfo.MaxSupportedQbXmlVersion);
            qbRequestXML.Append("<QBXMLMsgsRq onError = \"stopOnError\"><CompanyQueryRq requestID = \"1\" ></CompanyQueryRq></QBXMLMsgsRq>");
            qbRequestXML.Append("</QBXML>");

            string responseXml = reqProcessor.ProcessRequest(currentSessionTicket, qbRequestXML.ToString());

            XElement responseXmlRoot = XElement.Parse(responseXml);
            if (responseXmlRoot != null)
            {
                XElement messageResponseElement = responseXmlRoot.Element("QBXMLMsgsRs");
                if (messageResponseElement != null)
                {
                    XElement companyRespElement = messageResponseElement.Element("CompanyQueryRs");
                    if (companyRespElement != null)
                    {
                        QBResponseStatus responseStatus = ParseStatus(companyRespElement);
                        if (responseStatus.StatusCode == 0)
                        {
                            XElement companyRetElement = companyRespElement.Element("CompanyRet");
                            if (companyRetElement != null)
                            {
                                string companyName = companyRetElement.Element("CompanyName") != null ? companyRetElement.Element("CompanyName").Value : string.Empty;

                                XElement taxMonthElement = companyRetElement.Element("FirstMonthIncomeTaxYear");
                                string taxMonth = taxMonthElement != null ? taxMonthElement.Value : string.Empty;
                                int taxMonthNum = GetMonthNumber(taxMonth);

                                string taxForm = companyRetElement.Element("TaxForm") != null ? companyRetElement.Element("TaxForm").Value : string.Empty;
                                taxForm = taxForm.Trim().Replace("Form", string.Empty);

                                // Populate the company info object
                                companyInfo = new CompanyInfo();

                                companyInfo.CompanyName = companyName;
                                companyInfo.FileName = Path.GetFileName(companyFile);
                                companyInfo.FileLocation = Path.GetDirectoryName(companyFile);
                                companyInfo.FilePath = companyFile;
                                companyInfo.DisplayName = Path.GetFileNameWithoutExtension(companyFile);
                                companyInfo.FirstMonthIncomeTaxYear = taxMonthNum;
                                companyInfo.TaxForm = taxForm;
                            }
                        }
                    }
                }
            }
            return companyInfo;
        }

        /// <summary>
        /// Get the info about the QB installed on the user's machine
        /// </summary>
        /// <returns></returns>
        private QBAppInfo GetQBAppInfo()
        {
            QBAppInfo qbAppInfo = null;

            // Build the message request to get the supported versions on QB. (Use minimum QBXML version to ensure that this will work any version of QB)
            StringBuilder qbRequestXML = new StringBuilder();
            qbRequestXML.AppendFormat("<?xml version=\"1.0\"?><?qbxml version=\"{0}\"?><QBXML>", MinQBXmlVersion.ToString());
            qbRequestXML.Append("<QBXMLMsgsRq onError = \"stopOnError\"><HostQueryRq requestID = \"1\"/></QBXMLMsgsRq>");
            qbRequestXML.Append("</QBXML>");

            string responseXml = reqProcessor.ProcessRequest(currentSessionTicket, qbRequestXML.ToString());

            XElement responseXmlRoot = XElement.Parse(responseXml);
            if (responseXmlRoot != null)
            {
                XElement messageResponseElement = responseXmlRoot.Element("QBXMLMsgsRs");
                if (messageResponseElement != null)
                {
                    XElement hostQueryRespElement = messageResponseElement.Element("HostQueryRs");
                    if (hostQueryRespElement != null)
                    {
                        QBResponseStatus responseStatus = ParseStatus(hostQueryRespElement);
                        if (responseStatus.StatusCode == 0)
                        {
                            XElement hostRetElement = hostQueryRespElement.Element("HostRet");
                            if (hostRetElement != null)
                            {
                                qbAppInfo = new QBAppInfo();

                                // get the major & minor versions of QB application
                                int majorVersion = 0;
                                int minorVersion = 0;
                                XElement majorVersionElement = hostRetElement.Element("MajorVersion");
                                if (majorVersionElement != null)
                                {
                                    majorVersion = Int32.TryParse(majorVersionElement.Value, out majorVersion) ? majorVersion : 0;
                                }
                                XElement minorVersionElement = hostRetElement.Element("MinorVersion");
                                if (minorVersionElement != null)
                                {
                                    minorVersion = Int32.TryParse(minorVersionElement.Value, out minorVersion) ? minorVersion : 0;

                                }
                                string minorVersionStr = hostRetElement.Element("MinorVersion").Value;
                                qbAppInfo.CurrentVersion = new QBVersion(majorVersion, minorVersion);

                                // Find the latest version of qbxml supported by QB
                                double latestSupportedVersion = double.Parse(MinQBXmlVersion.ToString());
                                IEnumerable<XElement> supportedVersionElements = hostRetElement.Elements("SupportedQBXMLVersion");
                                foreach (XElement supportedVersionStr in supportedVersionElements)
                                {
                                    double supportedVersion = Double.Parse(supportedVersionStr.Value);
                                    if (supportedVersion > latestSupportedVersion)
                                        latestSupportedVersion = supportedVersion;
                                }
                                qbAppInfo.MaxSupportedQbXmlVersion = QBVersion.Parse(latestSupportedVersion); ;
                            }
                        }
                    }
                }
            }
            return qbAppInfo;
        }
        #endregion
    }
    /// <summary>
    /// Info about the QB Application
    /// </summary>
    public class QBAppInfo
    {
        /// <summary>
        /// Version of QB on the target machine
        /// </summary>
        public QBVersion CurrentVersion { get; set; }

        /// <summary>
        /// maximum supported version of QB XML
        /// </summary>
        public QBVersion MaxSupportedQbXmlVersion { get; set; }

    }
}
