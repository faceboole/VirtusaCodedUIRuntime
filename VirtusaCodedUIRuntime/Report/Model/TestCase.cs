using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodedUI.Virtusa.Report.Model
{
    class TestCase
    {
        public String ModuleName
        {
            get;
            set;
        }
        public String ModuleType
        {
            get;
            set;
        }

        public String Result
        {
            get;
            set;
        }
        public String Duration
        {
            get;
            set;
        }
        public String ModelType
        {
            get;
            set;
        }
        public String Rid
        {
            get;
            set;
        }
        public int TestCaseId
        {
            get;
            set;
        }

        public List<TestStep> testSteps;

        /**
         * @param modulename
         * @param moduletype
         * @param result
         * @param duration
         * @param type
         * @param rid
         */
        public TestCase(String modulename, String moduletype,
                String duration, String type, String rid, int testCaseid)
        {
            this.ModuleName = modulename;
            this.ModuleType = moduletype;
            this.Result = "Success";
            this.Duration = duration;
            this.ModelType = type;
            this.Rid = rid;
            this.TestCaseId = testCaseid;

            testSteps = new List<TestStep>();
        }

    }

}
