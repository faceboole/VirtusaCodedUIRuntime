using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodedUI.Virtusa.Report.Reporter
{
    public interface IReporter
    {
        void reportStepResults(bool isPassed, String category, String message, String loglvl, String stacktrace);

        void endTestReporting();

        void addNewTestSuite(string currentTestSuiteName);

        void addNewTestCase(string currentTestCaseName);

        void addNewTestExecution();
    }
}
