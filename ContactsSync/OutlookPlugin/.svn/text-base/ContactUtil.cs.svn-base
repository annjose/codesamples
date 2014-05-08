using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace ContactsSync
{
    internal class ContactUtil
    {
        internal static Outlook.ContactItem FindContact(Outlook.ExchangeUser exchUserToFind, Outlook.NameSpace session)
        {
            Outlook.ContactItem contactFound = null;
            // Outlook.ContactItem newContact = new Outlook.ContactItem();
            Outlook.MAPIFolder contactsFolder = (Outlook.MAPIFolder) session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts);
            foreach (var item in contactsFolder.Items)
            {
                Outlook.ContactItem contact = item as Outlook.ContactItem;
                if (contact != null)
                {
                    if ( (string.Compare(contact.Account, exchUserToFind.Alias, true) == 0) ||
                         (string.Compare(contact.Email1Address, exchUserToFind.PrimarySmtpAddress, true) == 0) ||
                         (string.Compare(contact.Email1Address, exchUserToFind.PrimarySmtpAddress, true) == 0) )
                    {
                        contactFound = contact;
                        break;
                    }
                }
            }
            return contactFound;
        }

        internal static Outlook.ContactItem CreateNewContactFromExchangeUser(Outlook.ExchangeUser exchUser, Outlook.NameSpace session)
        {
            // Outlook.ContactItem newContact = new Outlook.ContactItem();
            Outlook.MAPIFolder contactsFolder = (Outlook.MAPIFolder)session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts);
            Outlook.ContactItem newContact = (Outlook.ContactItem)contactsFolder.Items.Add(Outlook.OlItemType.olContactItem);

            CopyContact(exchUser, newContact);

            return newContact;
        }

        internal static void CopyContact(Outlook.ExchangeUser srcContact, Outlook.ContactItem trgContact)
        {
            trgContact.FirstName = srcContact.FirstName;
            trgContact.LastName = srcContact.LastName;
            trgContact.CompanyName = srcContact.CompanyName;
            trgContact.MobileTelephoneNumber = srcContact.MobileTelephoneNumber;
            trgContact.JobTitle = srcContact.JobTitle;
            trgContact.Email1Address = srcContact.PrimarySmtpAddress;
            trgContact.BusinessAddress = string.Format("{0}, {1}, {2}, {3}",
                                            string.IsNullOrEmpty(srcContact.StreetAddress) ? "" : srcContact.StreetAddress,
                                            string.IsNullOrEmpty(srcContact.City) ? "" : srcContact.City,
                                            string.IsNullOrEmpty(srcContact.StateOrProvince) ? "" : srcContact.StateOrProvince,
                                            string.IsNullOrEmpty(srcContact.PostalCode) ? "" : srcContact.PostalCode);
            trgContact.Department = srcContact.Department;
            trgContact.Account = srcContact.Alias;
        }
    }
}
