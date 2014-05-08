using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Outlook = Microsoft.Office.Interop.Outlook;
using System.Diagnostics;

namespace ContactsSync
{
    class Program
    {
        private static Outlook.NameSpace session;
        private static List<Outlook.ContactItem> contactList = new List<Outlook.ContactItem>();
        private static bool foundDL = false;

        static void Main(string[] args)
        {
            Console.WriteLine("\tContactsSync - a tool to synchronize the Distribution Lists in MS Exchange with your local Contacts list");
            Console.WriteLine("\tUsage: Select a Distribution List from the popup dialog or type command 'OutlookPlugin.exe <DL_Name>'");
            try
            {
                Outlook.Application app = new Outlook.Application();
                session = app.Session;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured while starting Outlook session. Make sure that Outlook is running. Exception Details: " + ex.Message);
                return;
            }
            if (session != null)
            {
                string dlName = ( (args != null) && (args.Length > 0) ? args[0] : string.Empty);
                if (string.IsNullOrEmpty(dlName))
                {
                    Outlook.SelectNamesDialog snDialog = session.GetSelectNamesDialog();
                    snDialog.Display();
                    if (snDialog.Recipients.Count > 0)
                    {
                        for (int i = 1; i <= snDialog.Recipients.Count; i++ )
                        {
                            Outlook.AddressEntry addrEntry = snDialog.Recipients[i].AddressEntry;
                            if (addrEntry != null)
                            {
                                Console.WriteLine("\nSynchronizing Distribution List " + addrEntry.Name + "..........");
                                // PopulateDistList("Bangalore All");
                                bool success = PopulateDistList(addrEntry);
                            }
                        }
                    }
                    else 
                    {
                        Console.WriteLine("Please enter the name of a distibution list or choose from the popup dialog");                    
                    }
                }
            }
        }

        private static bool PopulateDistList(Outlook.AddressEntry dlAddrEntry)
        {
            bool success = false;
            try
            {
                Console.WriteLine("Retrieving members..");
                GetDLMembers(dlAddrEntry);
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException occured while populating DLs. Exception Details: " + ex.Message);
            }

            Console.WriteLine();
            if (success)
                Console.WriteLine("Done! Check Outlook Contact list for changes");
            else
                Console.WriteLine("Error! Could not complete sync");
            return success;
        }

        private static void GetDLMembers(Outlook.AddressEntry addressEntry)
        {

            if (addressEntry.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeDistributionListAddressEntry)
            {
                // Outlook.ContactItem contact = entry.GetContact();
                Outlook.ExchangeDistributionList exchDL = addressEntry.GetExchangeDistributionList();

                Outlook.AddressEntries exchDLMembers = exchDL.GetExchangeDistributionListMembers();
                if (exchDLMembers != null)
                {
                    foreach (Outlook.AddressEntry exchDLMember in exchDLMembers)
                    {
                        GetDLMembers(exchDLMember);
                    }
                }
            }
            else if (addressEntry.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry)
            {
                Console.Write("Synchronizing user '" + addressEntry.Name + "':\t");
                Outlook.ExchangeUser exchUser = addressEntry.GetExchangeUser();
                if (exchUser != null)
                {
                    Outlook.ContactItem localContact = ContactUtil.FindContact(exchUser, session);
                    if (localContact != null)
                    {
                        Console.Write("Modifying...\t");
                        ContactUtil.CopyContact(exchUser, localContact);
                        localContact.Save();
                    }
                    else
                    {
                        Console.Write("Creating new...\t");
                        Outlook.ContactItem newContact = ContactUtil.CreateNewContactFromExchangeUser(exchUser, session);
                        newContact.Save();
                    }
                }
                Console.Write("Done");
                Console.WriteLine();
            }
        }

        private static bool PopulateDistListOld(string dlName)
        {
            bool success = false;
            try
            {
                Outlook.AddressLists addressLists = session.AddressLists as Outlook.AddressLists;
                foreach (Outlook.AddressList addressList in addressLists)
                {
                    //if (addressList.AddressListType == Outlook.OlAddressListType.olExchangeGlobalAddressList)
                    {
                        Console.WriteLine("Searching for Distribution list '" + dlName + "' in address list '" + addressList.Name + "'...");
                        int listIndex = 0;
                        foreach (Outlook.AddressEntry addressEntryFirstLevel in addressList.AddressEntries)
                        {
                            listIndex++;
                            if (listIndex % 100 == 0)
                            {
                                Console.Write("..");
                                if (listIndex % 4000 == 0)
                                    Console.WriteLine();
                            }
                            if ((addressEntryFirstLevel != null) &&
                                (addressEntryFirstLevel.AddressEntryUserType == Outlook.OlAddressEntryUserType.olExchangeDistributionListAddressEntry) &&
                                  addressEntryFirstLevel.Name.StartsWith(dlName))
                            {
                                foundDL = true;
                                Console.WriteLine("\nFound Distribution List " + dlName + ". Retrieving members..");
                                // GetDLMembers(addressEntryFirstLevel, dlName);
                                success = true;
                                break;
                            }
                        }
                    }
                    if (foundDL)
                        break;
                    else
                        Console.WriteLine("\n\tCould not find DL in Address List'" + addressList + "'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nException occured while populating DLs. Exception Details: " + ex.Message);
            }

            Console.WriteLine();
            if (success)
                Console.WriteLine("Done! Check Outlook Contact list for changes");
            else
                Console.WriteLine("Error! Could not complete sync");
            return success;
        }
    }
}
