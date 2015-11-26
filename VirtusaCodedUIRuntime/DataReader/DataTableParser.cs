using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CodedUI.Virtusa.DataReader
{
    class DataTableParser : IDataTableParser
    {

        private T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                string attributeXml = string.Empty;

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }

            return objectOut;
        }

        Table IDataTableParser.getDataTable(String tableName)
        {
            DataTables dataTables = DeSerializeObject<DataTables>(getRunningLocation());
            Table table = null;
            for (int i = 0; i < dataTables.getTableCount(); i++)
            {
                Table t = dataTables.getTable(i);
                String name = t.tableName;
                Console.WriteLine(name);
                if (name == tableName)
                {
                    table = t;
                }
            }
            return table;
        }

        private String getRunningLocation()
        {
            string dataFileName = null;

            dataFileName = @"DataTables\DataTables.xml";

            return dataFileName;
        }
    }
}
