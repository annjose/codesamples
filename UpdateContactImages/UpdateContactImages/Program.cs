using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Office.Interop.Outlook;
using Exception = System.Exception;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OutlookContacts
{
    class Program
    {
        private static int count = 0;
        static void Main()
        {
            Console.WriteLine("This program will update your Outlook contacts with profile images from Joinin...");
            FixupContactsWithJoininPictures();
            Console.WriteLine(string.Format("Updated {0} contacts\n Finished.", count));
            Console.ReadKey();
        }

        private static void FixupContactsWithJoininPictures()
        {
            count = 0;
            WebClient webClient = new WebClient();
            string rootPath = GetRootPath();

            var outloook = new Application();
            var outlookSession = outloook.Session;


            if (outlookSession.AddressLists == null || outlookSession.AddressLists.Count <= 0)
                return;

            Outlook.AddressLists addressLists = outlookSession.AddressLists as Outlook.AddressLists;
            foreach (Outlook.AddressList addressList in addressLists)
            {

                if (addressList.AddressListType == Outlook.OlAddressListType.olOutlookAddressList)
                {

                    foreach (Outlook.AddressEntry entry in addressList.AddressEntries)
                    {
                        try
                        {
                            ContactItem contact = entry.GetContact();
                            if (contact != null && !string.IsNullOrWhiteSpace(contact.Account))
                            {
                                Console.Write(string.Format("\nUpdating: {0}", contact.FullName));

                                string imgPath = Path.Combine(rootPath, contact.Account + ".jpg");

                                if (File.Exists(imgPath))
                                {
                                    Console.Write("\t Skipped....");
                                    continue;
                                }

                                string content = webClient.DownloadString(
                                    string.Format(
                                        @"http://joinin.intuit.com/profiles/atom/profile.do?uid={0}&lang=en",
                                        contact.Account));

                                XDocument xDocument = XDocument.Parse(content);
                                XNamespace xns = "http://www.w3.org/2005/Atom";

                                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
                                namespaceManager.AddNamespace("p", "http://www.w3.org/2005/Atom");

                                IEnumerable<XElement> entryNodes = xDocument.Descendants(xns + "entry");
                                foreach (XElement entryNode in entryNodes)
                                {
                                    XElement imgURLNode = entryNode.XPathSelectElement("p:link[@type='image']",
                                                                                       namespaceManager);
                                    if (imgURLNode == null)
                                        continue;

                                    string imgUrl = imgURLNode.Attribute("href").Value;
                                    webClient.DownloadFile(imgUrl, imgPath);
                                    contact.AddPicture(imgPath);
                                    contact.Body = string.Empty;
                                    contact.Save();
                                    Console.Write("\t Done.");
                                    count++;
                                    break;
                                }

                            }
                        }
                        catch (WebException webException)
                        {
                            Console.WriteLine(string.Format("Netwrk Error: {0}", webException.Message));
                            Console.WriteLine("Check if you are connected to Intuit LAN");
                            continue;

                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(string.Format("Error: {0}", exception.Message));
                            continue;
                        }
                    }
                }
            }
        }

        private static string GetRootPath()
        {
            string MyPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.Combine(MyPictures, "ContactsPictures"));
            return directoryInfo.FullName;
        }
    }
}
