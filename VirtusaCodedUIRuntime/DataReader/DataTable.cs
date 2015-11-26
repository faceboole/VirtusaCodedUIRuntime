using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CodedUI.Virtusa.DataReader
{
    public class DataTables
    {
        [XmlElement("Table")]
        public List<Table> table { get; set; }

        public int getTableCount()
        {
            return table.Count;
        }

        public Table getTable(int index)
        {
            return table.ElementAt<Table>(index);
        }

    }

    public class Table
    {
        [XmlAttribute("name")]
        public String tableName;


        [XmlElement("Header")]
        public List<Header> header { get; set; }

        //[XmlElement("Column")]
        //public List<Column> columns { get; set; }

        [XmlElement("Row")]
        public List<Row> rows { get; set; }

        public int getColumnCount()
        {
            return header[0].columns.Count;
        }

        public int getRowCount()
        {
            return rows.Count;
        }

        public List<Column> getColumns()
        {
            return header[0].columns;
        }

        internal void addColumn(Column colNew)
        {
            header[0].columns.Add(colNew);
        }

        internal void addRow(Row rowNew)
        {
            rows.Add(rowNew);
        }

        internal String getValueAt(String columnName, int row)
        {
            String value = "";
            for (int i = 0; i < header[0].columns.Count; i++)
            {
                if (header[0].columns[i].name.Equals(columnName))
                {
                    value = rows[row].value[i];
                    break;
                }
            }
            return value;
        }

        internal void setValueAt(String columnName, int row, String value)
        {
            for (int i = 0; i < header[0].columns.Count; i++)
            {
                if (header[0].columns[i].name.Equals(columnName))
                {
                    rows[row].value[i] = value;
                    break;
                }
            }
        }

    }

    public class Header
    {
        [XmlElement("Column")]
        public List<Column> columns { get; set; }
    }

    public class Column
    {
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("type")]
        public string type { get; set; }
    }

    public class Row
    {
        [XmlElement("Value")]
        public List<String> value { get; set; }
    }

    //public class Value
    //{
    //    [XmlText]
    //    public string value { get; set; }
    //}
}
