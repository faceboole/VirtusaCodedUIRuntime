using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using log4net;
using CodedUI.Virtusa.Report.Reporter;
using System.IO;
using System.Diagnostics;
using OpenQA.Selenium.Safari;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestHacks;
using Microsoft.VisualStudio.TestTools.UITesting;


namespace CodedUI.Virtusa.Runtime
{
    //[MbUnit.Framework.TestFixture]
    [TestClass]
    public class MbUnitHelper : TestBase
    {
        private IWebDriver driver;
        private int retry;
        private int retryInterval;
        private string browser;
        private static ILog log;
        private IReporter reporter;
        private String iedriverPath64bit = @"Libraries" + Path.DirectorySeparatorChar + "ExecutableFiles" + Path.DirectorySeparatorChar + "InternetExplorer" + Path.DirectorySeparatorChar + "IEDriver64";
        private String iedriverPath32bit = @"Libraries" + Path.DirectorySeparatorChar + "ExecutableFiles" + Path.DirectorySeparatorChar + "InternetExplorer" + Path.DirectorySeparatorChar + "IEDriver86";
        private String chromedriverLinux32 = @"Libraries" + Path.DirectorySeparatorChar + "ExecutableFiles" + Path.DirectorySeparatorChar + "Chrome" + Path.DirectorySeparatorChar + "Linux32";
        private String chromedriverLinux64 = @"Libraries" + Path.DirectorySeparatorChar + "ExecutableFiles" + Path.DirectorySeparatorChar + "Chrome" + Path.DirectorySeparatorChar + "Linux64";
        private String chromedriverWin = @"Libraries" + Path.DirectorySeparatorChar + "ExecutableFiles" + Path.DirectorySeparatorChar + "Chrome" + Path.DirectorySeparatorChar + "Windows";
        private String chromedriverMac = @"Libraries" + Path.DirectorySeparatorChar + "ExecutableFiles" + Path.DirectorySeparatorChar + "Chrome" + Path.DirectorySeparatorChar + "Mac";
        private StringBuilder verificationErrors;

        public Dictionary<string, IWebDriver> WebdriverInstances
        {
            get;
            set;
        }

        public enum LogLevel
        {
            Success,
            Error,
            Warn,
            VerificationError
        };


        //[MbUnit.Framework.FixtureInitializer]
        public MbUnitHelper()
        {
            retry = Settings.Execution.Default.RETRY;
            retryInterval = Settings.Execution.Default.RETRY_INTERVAL;
            browser = Settings.Execution.Default.BROWSER;
            WebdriverInstances = new Dictionary<string, IWebDriver>();
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(MbUnitHelper));

        }



        public void configureTestReport(String currentTestSuiteName, String currentTestCaseName)
        {
            reporter = MbUnitHelperSuper.getReporterInstance();
            String previousTestSuiteName = Settings.ProjectData.Default.CURRENT_TEST_SUITE;
            if (previousTestSuiteName != currentTestSuiteName)
            {
                reporter.addNewTestSuite(currentTestSuiteName);
                Settings.ProjectData.Default.CURRENT_TEST_SUITE = currentTestSuiteName;
            }

            reporter.addNewTestCase(currentTestCaseName);
        }

        //[MbUnit.Framework.SetUp]
        [TestInitialize]
        public void beforeMethodConfiguration()
        {
            Playback.Initialize(); 
            cleanBrowserInstances(browser);
            verificationErrors = new StringBuilder();
            
        }

        private void cleanBrowserInstances(String execBrowser)
        {
            if ("*firefox".Equals(execBrowser))
            {
                killProcess("firefox");
            }
            else if ("*iexplore".Equals(execBrowser))
            {
                killProcess("iexplore");
            }
            else if ("*chrome".Equals(execBrowser))
            {
                killProcess("chrome");
            }
            else if ("*safari".Equals(execBrowser))
            {
                killProcess("safari");
            }

        }


        private void killProcess(String processName)
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


        public void manageBrowser(IWebDriver driver)
        {
            try
            {
                driver.Manage().Window.Maximize();
                driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromMinutes(10));
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        public IWebDriver configWebDriverInstance(string browser)
        {
            IWebDriver webDriver = null;
            if (browser.ToLower().Contains("firefox"))
            {
                webDriver = new FirefoxDriver();
            }
            else if (browser.ToLower().Contains("iexplore"))
            {
                webDriver = new InternetExplorerDriver(setExecutableFiles(browser));
            }
            else if (browser.ToLower().Contains("chrome"))
            {
                webDriver = new ChromeDriver(setExecutableFiles("chrome"));
            }
            else if (browser.ToLower().Contains("safari"))
            {
                webDriver = new SafariDriver();
            }
            else
            {
                throw new InvalidOperationException("Invalid browser name " + browser + " passed.");
            }

            return webDriver;
        }

        public string setExecutableFiles(string browserName)
        {
            String driverPath = "";
            if (browserName.Contains("iexplore"))
            {

                if (isx64bit())
                {
                    driverPath = Path.GetFullPath(iedriverPath64bit);

                }
                else
                {
                    driverPath = Path.GetFullPath(iedriverPath32bit);

                }


            }
            else if (browserName.ToLower().Contains("chrome"))
            {
                String path = getChromeDriverServerExecutables();
                driverPath = Path.GetFullPath(path);

            }
            return driverPath;
        }
        protected bool isx64bit()
        {

            bool architecture = System.Environment.Is64BitProcess;
            return true;

        }
        private string getChromeDriverServerExecutables()
        {
            String filePath = "";
            String osInfo = Environment.OSVersion.VersionString;
            if (osInfo.Contains("Windows"))
            {

                filePath = chromedriverWin;
            }
            else if (osInfo.Contains("Mac"))
            {

                filePath = chromedriverMac;
                System.Diagnostics.Process.Start("/Applications/Utilities/Terminal.app", "chmod 777 " + Path.GetFullPath(filePath) + Path.DirectorySeparatorChar + "chromedriver");
            }
            else if (osInfo.Contains("Linux"))
            {
                if (isx64bit())
                {
                    filePath = chromedriverLinux64;
                }
                else
                {
                    filePath = chromedriverLinux32;
                }
            }

            String x = Path.GetFullPath(filePath);

            return filePath;

        }

        public void reportResults(String category, String message, LogLevel logLevel)
        {
            reporter = MbUnitHelperSuper.getReporterInstance();
            reporter.reportStepResults(true, category, message, logLevel.ToString(), "");
        }

        public void reportResults(bool stopOnFailure, string category, string message, LogLevel logLevel, string stackTrace, object customErrorMessage = null)
        {
            reporter = MbUnitHelperSuper.getReporterInstance();

            if (customErrorMessage!=null)
            {
                message = "Custom error message : " + customErrorMessage + ".\n " + message;
            }

            reporter.reportStepResults(false, category, message, logLevel.ToString(), stackTrace);
            if (stopOnFailure)
            {
                failure(message);
            }
            else
            {
                verificationErrors.Append(message + "\n");
            }
        }

        //[MbUnit.Framework.TearDown]
        [TestCleanup]
        public void afterMethodConfiguration()
        {
            reporter.endTestReporting();
            cleanBrowserInstances(browser);
            if (verificationErrors.Length > 0)
            {
                failure(verificationErrors.ToString());
            }
        }

        public void failure(String message)
        {
            //Assert.Terminate(TestOutcome.Failed, message);
            Assert.Fail(message);
        }

        public IWebDriver Driver
        {
            get { return driver; }
            set { driver = value; }
        }

    }
}
