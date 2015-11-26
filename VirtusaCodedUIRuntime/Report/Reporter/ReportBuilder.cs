using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodedUI.Virtusa.Report.Model;

namespace CodedUI.Virtusa.Report.Reporter
{
    class ReportBuilder
    {
        public TestCase testCase;
        public TestSuite testSuite;
        public TestStep testStep;
        public TestExecution testExecution;
        public String reportFolderLocation;

        public int rid;
        public int uniqueTestCaseId;
        public List<int> reportedTestCases;

        public ReportBuilder(String reportFolderLocation)
        {
            this.reportFolderLocation = reportFolderLocation;
            rid = 0;
            uniqueTestCaseId = 0;
            reportedTestCases = new List<int>();
        }

        public String getReportFolderLocation()
        {
            return this.reportFolderLocation;
        }

        public void addNewTestCase(String modulename, String duration)
        {

            String moduletype = "UserCode";
            String type = "test module";
            String rid = getRid();

            testCase = new TestCase(modulename, moduletype, duration, type, rid, getUniqueTestCaseId());
            testSuite.TestCases.Add(testCase);
        }


        public void addNewTestSuite(String testSuiteName, String duration)
        {
            String iterationcount = "1";
            String maxchildren = "0";
            String type = "folder";
            String rid = getRid();

            testSuite = new TestSuite(testSuiteName, iterationcount, maxchildren,
                    duration, type, rid);

            testExecution.TestSuites.Add(testSuite);

        }


        public void addNewTestExecution()
        {

            int totalerrorcount = 0;
            int totalwarningcount = 0;
            int totalsuccesscount = 0;
            int totalfailedcount = 0;
            int totalblockedcount = 0;
            String host = "UNKNOWN";
            String user = "UNKNOWN";
            String osversion = "UNKNOWN";
            String language = "EN-US";
            String screenresolution = "UNKNOWN";
            String timestamp = "UNKNOWN";
            String duration = "UNKNOWN";
            String type = "root";
            try
            {
                user = Environment.UserName;
                host = Environment.MachineName;
                osversion = Environment.OSVersion.VersionString;
                screenresolution = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width.ToString() + "X" + System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height.ToString();

                DateTime date = DateTime.Now;
                timestamp = date.ToString("MM/dd/yyyy h:mm:ss aa");


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            testExecution = new TestExecution(user, host, osversion, language,
                    screenresolution, timestamp, duration, type, totalerrorcount,
                    totalwarningcount, totalsuccesscount, totalfailedcount,
                    totalblockedcount);

        }

        public void addNewTestStep(bool isPassed, String category,
                String message, String loglvl)
        {

            DateTime date = DateTime.Now;
            String time = date.ToString("h:mm:ss");
            String codefile = "UNKNOWN";
            String codeline = "UNKNOWN";

            testStep = new TestStep(isPassed, time, category, message, codefile,
                    codeline, loglvl);

            testCase.testSteps.Add(testStep);

        }

        public void addNewTestStep(bool isPassed, String category,
                String errimg, String errthumb, String message, String stacktrace,
                String loglvl)
        {

            DateTime date = DateTime.Now;
            String time = date.ToString("h:mm:ss");
            String codefile = "UNKNOWN";
            String codeline = "UNKNOWN";

            testStep = new TestStep(isPassed, time, category, errimg, errthumb,
                    message, stacktrace, codefile, codeline, loglvl);

            testCase.testSteps.Add(testStep);

            if (!isPassed)
            {
                testCase.Result = "Failed";
                testSuite.Result = "Failed";
            }
        }

        public void setExecutionSummary()
        {

            //foreach (TestSuite ts in testExecution.testSuites) {
            //    foreach (TestCase tc in ts.testCases) {
            //        if (tc.getResult().Equals("Failed")) {
            //            testExecution.setTotalfailedcount(1);
            //            testExecution.setTotalerrorcount(1);
            //        } else if (tc.getResult().Equals("Success")) {
            //            testExecution.setTotalsuccesscount(1);
            //        }
            //    }
            //}


            foreach (TestSuite ts in testExecution.TestSuites)
            {
                foreach (TestCase tc in ts.TestCases)
                {
                    int tcId = tc.TestCaseId;
                    if (!reportedTestCases.Contains(tcId))
                    {
                        reportedTestCases.Add(tcId);
                        if ("Failed".Equals(tc.Result))
                        {
                            testExecution.setTotalfailedcount(1);
                            testExecution.setTotalerrorcount(1);
                        }
                        else if ("Success".Equals(tc.Result))
                        {
                            testExecution.setTotalsuccesscount(1);
                        }
                    }
                }
            }

            Console.Out.WriteLine("Report created successfully to the folder " + getReportFolderLocation());
        }

        public TestExecution getTestExecution()
        {
            return this.testExecution;
        }

        public int getUniqueTestCaseId()
        {
            return uniqueTestCaseId++;
        }

        public String getRid()
        {
            rid++;
            return rid.ToString();
        }


    }
}
