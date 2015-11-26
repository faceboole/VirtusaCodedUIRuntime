using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodedUI.Virtusa.DataReader
{
    public interface IDataTableParser
    {
        Table getDataTable(string tableName);
    }
}
