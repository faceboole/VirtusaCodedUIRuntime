using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodedUI.Virtusa.Report.Model
{
    class TestSuite
    {
        public String TestSuiteName
        {
            get;
            set;
        }
        public String IterationCount
        {
            get;
            set;
        }
        public String MaxChildren
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

        public List<TestCase> TestCases
        {
            get;
            set;
        }

        /**
         * @param testcasename
         * @param iterationcount
         * @param maxchildren
         * @param result
         * @param duration
         * @param type
         * @param rid
         */
        public TestSuite(String testcasename, String iterationcount,
                String maxchildren, String duration, String type,
                String rid)
        {
            this.TestSuiteName = testcasename;
            this.IterationCount = iterationcount;
            this.MaxChildren = maxchildren;
            this.Result = "Success";
            this.Duration = duration;
            this.ModelType = type;
            this.Rid = rid;

            TestCases = new List<TestCase>();
        }

    }
}
