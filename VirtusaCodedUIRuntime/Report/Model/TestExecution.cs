using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodedUI.Virtusa.Report.Model
{
    class TestExecution
    {
        public String User
        {
            get;
            set;
        }
        public String Host
        {
            get;
            set;
        }
        public String OsVersion
        {
            get;
            set;
        }
        public String Language
        {
            get;
            set;
        }
        public String ScreenResolution
        {
            get;
            set;
        }
        public String TimeStamp
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
        public String Type
        {
            get;
            set;
        }
        public int TotalErrorCount
        {
            get;
            set;
        }
        public int TotalWarningCount
        {
            get;
            set;
        }
        public int TotalSuccessCount
        {
            get;
            set;
        }
        public int TotalFailedCount
        {
            get;
            set;
        }
        public int TotalBlockedCount
        {
            get;
            set;
        }

        public List<TestSuite> TestSuites
        {
            get;
            set;
        }

        /**
         * Constructs the TestExecution Object
         */
        public TestExecution(String user, String host, String osversion,
                String language, String screenresolution, String timestamp,
                String duration, String type,
                int totalerrorcount, int totalwarningcount,
                int totalsuccesscount, int totalfailedcount,
                int totalblockedcount)
        {
            //super();
            this.User = user;
            this.Host = host;
            this.OsVersion = osversion;
            this.Language = language;
            this.ScreenResolution = screenresolution;
            this.TimeStamp = timestamp;
            this.Result = "Success";
            this.Duration = duration;
            this.Type = type;
            this.TotalErrorCount = totalerrorcount;
            this.TotalWarningCount = totalwarningcount;
            this.TotalSuccessCount = totalsuccesscount;
            this.TotalFailedCount = totalfailedcount;
            this.TotalBlockedCount = totalblockedcount;

            this.TestSuites = new List<TestSuite>();
        }


        public void setTotalerrorcount(int totalerrorcount)
        {

            this.TotalErrorCount += totalerrorcount;
        }


        public void setTotalwarningcount(int totalwarningcount)
        {
            this.TotalWarningCount += totalwarningcount;
        }


        public void setTotalsuccesscount(int totalsuccesscount)
        {
            this.TotalSuccessCount += totalsuccesscount;
        }


        public void setTotalfailedcount(int totalfailedcount)
        {
            this.TotalFailedCount += totalfailedcount;
        }


        public void setTotalblockedcount(int totalblockedcount)
        {
            this.TotalBlockedCount += totalblockedcount;
        }
    }


}
