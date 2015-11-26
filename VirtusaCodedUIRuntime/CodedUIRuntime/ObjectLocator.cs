using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodedUI.Virtusa.Runtime
{
    public class ObjectLocator
    {
        private string logicalName;
        private string identifier;
        private string actualLocator;

        public ObjectLocator(string logicalName, string identifier, string actualLocator) 
        {
            this.logicalName = logicalName;
            this.identifier = identifier;
            this.actualLocator = actualLocator;
        }

        public string LogicalName
        {
            get { return logicalName; }
            set { logicalName = value; }
        }

        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }

        public string ActualLocator
        {
            get { return actualLocator; }
            set { actualLocator = value; }
        }
    }
}
