using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp
{
    class Program
    {
        /// <summary>
        /// Account type as given by QB source
        /// </summary>
        public enum QBAccountType
        {
            /// <summary>
            /// Income Account
            /// </summary>
            Income,
            /// <summary>
            /// Expense account
            /// </summary>
            Expense,
            /// <summary>
            /// COGS account
            /// </summary>
            CostOfGoodsSold,
            /// <summary>
            /// Asset account
            /// </summary>
            Asset,
            /// <summary>
            /// Liability account
            /// </summary>
            Liability
        }

        static void Main(string[] args)
        {
            Console.WriteLine("COGS (plain value) = " + QBAccountType.Asset);
            Console.WriteLine("COGS (ToString)= " + QBAccountType.Asset.ToString());
            Console.WriteLine("COGS (Enum.GetName) = " + Enum.GetName(typeof(QBAccountType), QBAccountType.Asset));

            try
            {
                QBAccountType acctType = (QBAccountType)Enum.Parse(typeof(QBAccountType), "Income", true);
                Console.WriteLine("type=" + acctType);

                acctType = (QBAccountType)Enum.Parse(typeof(QBAccountType), "3", true);
                Console.WriteLine("type=" + acctType);

                acctType = (QBAccountType)Enum.Parse(typeof(QBAccountType), "300", true);
                if(!Enum.IsDefined(typeof(QBAccountType), acctType))
                    Console.WriteLine("Not defined " + acctType);
                else
                    Console.WriteLine("type=" + acctType);

                acctType = (QBAccountType)Enum.Parse(typeof(QBAccountType), "Ann", true);
                Console.WriteLine("type=" + acctType);

            }
            catch (ArgumentException argEx)
            {
                Console.WriteLine("Exception!");
            }
        }
    }
}
