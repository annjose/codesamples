using System;
using System.Diagnostics;
using System.Linq;

using Intuit.Ctg.Wte.Service.Interview.Interview.ViewModel;
using Intuit.Ctg.Map.Factory;
using Intuit.Ctg.Map.UI;
using Intuit.Ctg.Wte.Kernel;
using Intuit.Ctg.Wte.Service.Import;
using Intuit.Ctg.Wte.Service.Import.QuickBooks;
using Intuit.Ctg.Wte.Service.Outline;
using System.Collections.Generic;
using Intuit.Ctg.Wte.Service.Interview;
using System.Windows.Input;
using Intuit.Ctg.Map.Mediator;
using Intuit.Ctg.Wte.Service.Help;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Intuit.Ctg.Wte.Service.EasyStep.Import.QuickBooks
{
    /// <summary>
    /// 
    /// </summary>
    public class ReconciledAccount
    {
        /// <summary>
        /// 
        /// </summary>
        public Account QBAccount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<TaxLine> AssignableTaxLines { get; set; }

        /// <summary>
        /// Selected Tax Line
        /// </summary>
        public TaxLine SelectedTaxLine {get; set;}
    }

    /// <summary>
    /// ViewModel for the WPF screen QBReconcileUnassignedAccountsUC
    /// </summary>
    public class QBReconcileUnassignedAccountsViewModel : CommandViewModelBase
    {
        /// <summary>
        /// Constructor for the QBReconcileUnassignedAccountsViewModel class - find/show the list of accounts to reconcile
        /// </summary>
        public QBReconcileUnassignedAccountsViewModel()
        {
            qbConnector = Ioc.TryResolve<IImportService>().QBConnector;
            Debug.Assert(qbConnector != null);

            LoadReconciledAccounts();
        }

        private ObservableCollection<ReconciledAccount> myReconciledAccountCollection = new ObservableCollection<ReconciledAccount>();
        /// <summary>
        /// Property for the collection used in listview
        /// </summary>
        public ObservableCollection<ReconciledAccount> ReconciledAccountCollection
        {
            get { return myReconciledAccountCollection; }
            set { myReconciledAccountCollection = value; }
        }

        private ReconciledAccount myselectedReconciledAccount;
        /// <summary>
        /// Notification enabled property for SelectedReconciledAccount
        /// </summary>
        public ReconciledAccount SelectedReconciledAccount
        {
            get { return myselectedReconciledAccount; }
            set { myselectedReconciledAccount = value; OnPropertyChanged("SelectedReconciledAccount"); }
        }

        private TaxLine EmptyTaxLine = new TaxLine() { ID = 0, Name = "Select a Tax Line" };
        private IQBConnector qbConnector = null;

        private void LoadReconciledAccounts()
        {
            ReconciledAccountCollection.Clear();

            List<Account> qbAccounts = qbConnector.GetAccounts(false);
            if (qbAccounts != null && qbAccounts.Count > 0)
            {
                foreach (Account qbAccount in qbAccounts)
                {
                    if (qbAccount.TaxLine == null)
                    {
                        ReconciledAccount reconAccount = new ReconciledAccount();
                        reconAccount.QBAccount = qbAccount;
                        reconAccount.AssignableTaxLines = GetAssignableTaxLines(qbAccount);
                        if (reconAccount.AssignableTaxLines.Count > 1)
                            reconAccount.SelectedTaxLine = reconAccount.AssignableTaxLines.First();
                        
                        ReconciledAccountCollection.Add(reconAccount);
                    }
                }
            }
        }

        private ObservableCollection<TaxLine> GetAssignableTaxLines(Account qbAccount)
        {
            ObservableCollection<TaxLine> assignableTaxLines = new ObservableCollection<TaxLine>();
            
            assignableTaxLines.Add(EmptyTaxLine);
            if(qbAccount.Name.Contains("Rental"))
            {
                assignableTaxLines.Add(new TaxLine() { ID = 260, Name = "Income:Gross rents" });
                assignableTaxLines.Add(new TaxLine() { ID = 286, Name = "Income:Returns and allowances" });
                assignableTaxLines.Add(new TaxLine() { ID = 520, Name = "Income:Other income" });

            }
            else if (qbAccount.Name.Contains("Sales"))
            {
                assignableTaxLines.Add(new TaxLine() { ID = 1617, Name = "Income Gross Receipts or Sales" });
                assignableTaxLines.Add(new TaxLine() { ID = 1809, Name = "Income: Interest Income" });
                assignableTaxLines.Add(new TaxLine() { ID = 1629, Name = "Income: Other Income" });
            }
            else if (qbAccount.Name.Contains("Savings"))
            {
                assignableTaxLines.Add(new TaxLine() { ID = 1617, Name = "Income Gross Receipts or Sales" });
                assignableTaxLines.Add(new TaxLine() { ID = 1809, Name = "Income: Interest Income" });
                assignableTaxLines.Add(new TaxLine() { ID = 1629, Name = "Income: Other Income" });
            }
            else if (qbAccount.Name.Contains("Receivables"))
            {
                assignableTaxLines.Add(new TaxLine() { ID = 260, Name = "Deductions:Charitable contributions" });
                assignableTaxLines.Add(new TaxLine() { ID = 286, Name = "Deductions:Licenses" });
                assignableTaxLines.Add(new TaxLine() { ID = 520, Name = "Deductions:State taxes" });
            }
            return assignableTaxLines;
        }

        #region Commands

        #region ReconcileAccountsCommand

        private ICommand myReconcileAccountsCommand;

        /// <summary>
        /// Reconcile the accounts
        /// </summary>
        /// <value>The reconcile accounts command.</value>
        public ICommand ReconcileAccountsCommand
        {
            get
            {
                if (myReconcileAccountsCommand == null)
                    myReconcileAccountsCommand = new RelayCommand(() => ReconcileAccounts());

                return myReconcileAccountsCommand;
            }
        }

        /// <summary>
        /// Executes the back.
        /// </summary>
        internal void ReconcileAccounts()
        {
            bool allAccountsAreAssigned = true;
            foreach (ReconciledAccount reconAccount in ReconciledAccountCollection)
            {
                if ((reconAccount.SelectedTaxLine == null) || (reconAccount.SelectedTaxLine == EmptyTaxLine))
                {
                    // Do not allow the user to proceed.
                    allAccountsAreAssigned = false;
                    ShowUnassignedAccountsWarning();
                    break;
                }
            }
            if (allAccountsAreAssigned)
            { 
                List<Account> qbAccounts = qbConnector.GetAccounts(false);
                foreach (Account qbAccount in qbAccounts)
                {
                    if (qbAccount.TaxLine == null)
                    {
                        ReconciledAccount reconAccount = (from account in ReconciledAccountCollection
                                                          where account.QBAccount.Number == qbAccount.Number
                                                          select account).Single();
                        qbAccount.TaxLine = new TaxLine() { ID = reconAccount.SelectedTaxLine.ID, Name = reconAccount.SelectedTaxLine.Name };
                    }
                }

                // Proceed to the function to parse the QB data
                Ioc.Resolve<IInterview>().SetUserControlResultCode((int)EasyStepResultCodeType.Button1);
            }
        }

        private void ShowUnassignedAccountsWarning()
        {
            string warningMsg = "There are some more accounts to be reconciled. Please assign tax lines for all accounts to start import";
            AlertEventArgs alertArgs = new AlertEventArgs(warningMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Intuit.Ctg.Map.UI.AlertEventArgs result = ServiceMediator.CommandMediator.Publish(ServiceCommands.AlertPrompt, this, alertArgs) as Intuit.Ctg.Map.UI.AlertEventArgs;
        }

        #endregion

        #region BackCommand

        private ICommand myBackCommand;

        /// <summary>
        /// Gets the back command.
        /// </summary>
        /// <value>The back command.</value>
        public ICommand BackCommand
        {
            get
            {
                if (myBackCommand == null)
                    myBackCommand = new RelayCommand(() => ExecuteBack());

                return myBackCommand;
            }
        }

        /// <summary>
        /// Executes the back.
        /// </summary>
        internal void ExecuteBack()
        {
            bool fromInterview = Ioc.Resolve<IImportService>().ImportContext.FromInterview;

            EasyStepResultCodeType result = fromInterview ? EasyStepResultCodeType.Button9 : EasyStepResultCodeType.Back;
            Ioc.Resolve<IInterview>().SetUserControlResultCode((int)result);
        }
        #endregion BackCommand

        #region HelpCommand
        RelayCommand<string> myHelpCommand;

        /// <summary>
        /// Navigate to the HomeBase view - Amend
        /// </summary>
        public ICommand HelpCommand
        {
            get
            {
                if (myHelpCommand == null)
                    myHelpCommand = new RelayCommand<string>((o) => this.ExecuteProgramHelp(o));

                return myHelpCommand;
            }
        }

        void ExecuteProgramHelp(string param)
        {
            // Display program help in browser
            HelpService.Instance.DisplayHelp(Intuit.Ctg.Wte.Service.Help.HelpType.ProgramHelpType, param, false);
        }
        #endregion HelpCommand

        #endregion Commands
    }
}

