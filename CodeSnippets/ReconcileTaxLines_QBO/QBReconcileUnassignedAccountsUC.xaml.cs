using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Xml.Serialization;

namespace Intuit.Ctg.Wte.Service.EasyStep.Import.QuickBooks
{
    /// <summary>
    /// Interaction logic for QBReconcileUnassignedAccountsUC.xaml
    /// </summary>
    public partial class QBReconcileUnassignedAccountsUC : UserControl
    {
        QBReconcileUnassignedAccountsViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="QBReconcileUnassignedAccountsUC"/> class.
        /// </summary>
        public QBReconcileUnassignedAccountsUC()
        {
            InitializeComponent();

            viewModel = new QBReconcileUnassignedAccountsViewModel();
            DataContext = viewModel;
        }
    }
} // end namespace
