using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodedUI.Virtusa.Report.Reporter;

namespace CodedUI.Virtusa.Runtime
{
    //[MbUnit.Framework.AssemblyFixture]
    [TestClass]
    public class MbUnitHelperSuper
    {
        private static IReporter reporter;
        private static ILog log;

        public static IReporter getReporterInstance()
        {
            if (reporter == null)
            {
                reporter = new Reporter();
            }            
            return reporter;
        }



        [AssemblyInitialize()]
        public static void BeforeRunAssembly(TestContext context)
        {
            log = LogManager.GetLogger(typeof(MbUnitHelperSuper));
            killDriverExecutablesProcesses();
            Settings.ProjectData.Default.CURRENT_TEST_SUITE = "";
            reporter = new Reporter();
            reporter.addNewTestExecution();
        }

        [AssemblyCleanup]
        public static void AfterRunAssembly()
        {
            killDriverExecutablesProcesses();
        }

        private static void killDriverExecutablesProcesses() 
        {
            killProcess("chromedriver");
            killProcess("IEDriverServer");
        }

        private static void killProcess(String processName)
        {
            foreach (Process process in System.Diagnostics.Process.GetProcessesByName(processName))
            {
                try
                {
                    process.Kill();
                }
                catch (InvalidOperationException e)
                {
                    log.Error(e);
                }
                catch (NotSupportedException e)
                {
                    log.Error(e);
                }
                catch (System.ComponentModel.Win32Exception e)
                {
                    log.Error(e);
                }
            }
        }
    }
}
