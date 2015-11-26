using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodedUITemplate5.CodedUIRuntime
{
    class XmlManager
    {

        public void XmlWrite(string key, string type, string value)
        {
            
                //-----writing to XML-----
                XNamespace testNM = "urn:lst-emp:emp";
                XDocument xDoc;
                string path = "project_data.xml";
                if (!File.Exists(path))
                {
                    xDoc = new XDocument(
                               new XDeclaration("1.0", "UTF-16", null),
                               new XElement(testNM + "Test")
                               );
                }
                else
                {
                    xDoc = XDocument.Load(path);
                }

                var element = new XElement("key",
                        new XAttribute("name", key),
                        new XElement("Type", type),
                        new XElement("Value", value));

                xDoc.Element(testNM + "Test").Add(element);

                // Save to Disk
                xDoc.Save(path);
        }

        public string XmlRead(string key, string type)
        {
            XDocument doc = XDocument.Load("project_data.xml");

            foreach (var currentElement in doc.Root.Elements("key"))
            {
                string vkey = currentElement.Attribute("name").Value;
                string vtype = currentElement.Element("Type").Value;
                string vvalue = currentElement.Element("Value").Value;

                if (vkey.Equals(key, StringComparison.OrdinalIgnoreCase) && vtype.Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    return vvalue;
                    /*  StreamWriter file2 = new StreamWriter("C:\\tempFolder\\results.xml", true);
                      file2.WriteLine("MATCH::" + vkey+"::"+vtype+"::"+vvalue);
                      file2.Close();*/
                }
            }
            return null;
        }

        public void XmlReport(Object message)
        {

            //-----writing to XML-----
            XNamespace testNM = "urn:lst-emp:emp";
            XDocument xDoc;
            string path = "report.xml";
            if (!File.Exists(path))
            {
                xDoc = new XDocument(
                           new XDeclaration("1.0", "UTF-16", null),
                           new XElement(testNM + "Message")
                           );
            }
            else
            {
                xDoc = XDocument.Load(path);
            }

            var element = new XElement("message",
                    new XElement("Type", message));

            xDoc.Element(testNM + "Message").Add(element);

            // Save to Disk
            xDoc.Save(path);
        }


    }
}
