using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using CodedUI.Virtusa.ObjectMap;
using log4net;
using CodedUI.Virtusa.DataReader;
using OpenQA.Selenium.Support.UI;
using System.Threading;

using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.ObjectModel;

using CodedUI.Virtusa.Utils;
using WindowsInput;
using System.Windows.Forms;
using System.Drawing;
using CodedUITemplate5.CodedUIRuntime;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting.WinControls;



namespace CodedUI.Virtusa.Runtime
{
    [TestClass]
    public class CodedUITestBase : MbUnitHelper
    {
        private int retry;
        private int retryInterval;
        private ObjectMapParser objectMapParser;
        private static ILog log;
        private static CodedUITestBase instance;
        private static Dictionary<string, Object> dict;
        private static BrowserWindow qWin;
        private static ApplicationUnderTest application;

        public String[] OnErrorRecoveryScenarios
        {
            get;
            set;
        }
        public String[] BusinessRecoveryScenarios
        {
            get;
            set;
        }

        public static CodedUITestBase Instance
        {
            get { return instance; }
        }

        public enum ValidationType
        {
            /** The alloptions. */
            ALLOPTIONS,
            /** The selectedoption. */
            SELECTEDOPTION,
            /** The missingoption. */
            MISSINGOPTION,
            /** The elementpresent. */
            ELEMENTPRESENT,
            /** The propertypresent. */
            PROPERTYPRESENT
        };

        public enum WindowValidationType
        {
            /** The window present. */
            WINDOWPRESENT
        };

        public CodedUITestBase()
        {
            instance = this;
            retry = Settings.Execution.Default.RETRY;
            retryInterval = Settings.Execution.Default.RETRY_INTERVAL;
            objectMapParser = new ObjectMapParser();
            log = LogManager.GetLogger(typeof(CodedUITestBase));
        }

        public void Open(string url, string waitTime)
        {
            Open(url, "", waitTime);
        }

        public void Open(string url, string identifier, string waitTime)
        {
            String actualLocator = objectMapParser.getResolvedIdentifier(url, identifier);
            ObjectLocator locator = new ObjectLocator(url, identifier, actualLocator);
            DoOpen(locator, waitTime);
        }

        private void DoOpen(ObjectLocator locator, string waitTime)
        {
           string url = locator.ActualLocator;

            try
            {
                try
                {
                    // Resetting static values
                    qWin = null;
                    application = null;
                    if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith("https://"))
                    {
                        qWin = BrowserWindow.Launch(new System.Uri(url));
                        //qWin.WaitForControlReady(Int32.Parse(waitTime));
                    }
                    else
                    {
                        application = ApplicationUnderTest.Launch(url);
                    }
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
                reportResults("Open", "Open URL : " + url, LogLevel.Success);
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.Error(e);
                reportResults(true, 
                         "Open", 
                         "Open URL : " + url + " Failed. Actual error : " + e.Message, 
                         LogLevel.Error, 
                         e.StackTrace);
            }
        }

        public void Click(string objectName)
        {
            Click(objectName, "");
        }

        public void Click(string objectName, string identifier)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoClick(locator);
        }

        private void DoClick(ObjectLocator locator)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;
            try
            {
                UITestControl element = FindCodedUIElement(objectId);

                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        Mouse.Click(element);
                        reportResults("Click", "Click command passed. Object " + locator.LogicalName, LogLevel.Success);
                        break;
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {
                            log.Error(e);
                            reportResults(true, 
                                     "Click", 
                                     "Click command failed. Object " + locator.LogicalName + ". Actual Error :  " + e.Message, 
                                     LogLevel.Error, 
                                     e.StackTrace);
                        }
                    }
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.Error(e);
                reportResults(true, 
                    "Click", 
                    "Click command failed. Object " + locator.LogicalName + " cannot be found in the webpage. Actual Error :  " + e.Message, 
                    LogLevel.Error, 
                    e.StackTrace);
            }
        }

        private string CheckNullValue(Object theObject, string callingMethod, bool stopOnFailure)
        {
            string finalVar = "";
            if (theObject == null)
            {
                //fail and report result
                reportResults(
                                stopOnFailure, 
                                callingMethod, 
                                callingMethod + " failed. Empty variable passed.", 
                                LogLevel.Error, 
                                "", 
                                null
                              );
            }
            else
            {
                finalVar = theObject.ToString();
            }

            return finalVar;
        }


        public void Type(string objectName, Object value)
        {
            Type(objectName, "", value);
        }

        public void Type(string objectName, string identifier, Object value)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoType(locator, value);
        }

        private void DoType(ObjectLocator locator, Object valueObj)
        {
            string value = CheckNullValue(valueObj, "Type", true);
            
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;
            try
            {
                UITestControl element = FindCodedUIElement(objectId);

                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        element.SetProperty("Text","");// clear the text
                        Keyboard.SendKeys(element, value);
                        reportResults("Type", "Type command passed. Object " + locator.LogicalName + ". Input value : " + value, LogLevel.Success);
                        break;
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {
                            log.Error(e);
                            reportResults(true, 
                                          "Type", "Type command failed. Object " + locator.LogicalName 
                                          + ". Input value : " + value + 
                                          ". Actual Error :  " + e.Message, 
                                          LogLevel.Error, 
                                          e.StackTrace);
                        }
                    }
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.Error(e);
                reportResults(true, 
                         "Type", 
                         "Type command failed. Object " + locator.LogicalName + " cannot be found in the webpage. Actual Error :  " + e.Message, 
                         LogLevel.Error, 
                         e.StackTrace);
            }
        }


        //public void CheckElementPresent(string objectName, bool stopOnFailure, Object customErrorMessage = null)
        //{
        //    CheckElementPresent(objectName, "", stopOnFailure, customErrorMessage);
        //}

        //public void CheckElementPresent(string objectName, string identifier, bool stopOnFailure, Object customErrorMessage = null)
        //{
        //    string actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
        //    ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
        //    DoCheckElementPresent(locator, stopOnFailure, customErrorMessage);
        //}

        //private void DoCheckElementPresent(ObjectLocator locator, bool stopOnFailure, Object customErrorMessage = null)
        //{
        //    //IWebDriver driver = Driver;
        //    //string objectId = locator.ActualLocator;
        //    //int counter = retry;
        //    try
        //    {
        //        //UITestControl element = FindCodedUIElement(objectId);
        //        if (DoCheckElementPresent(locator))
        //        {
        //            reportResults("Check Element Present",
        //                       "Check Element Present command passed. Object "
        //                       + locator.LogicalName, LogLevel.Success);
        //        }
        //        else
        //        {
        //            throw new Exception(" Element not found"); 
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e);
        //        reportResults(stopOnFailure, 
        //            "Check Element Present", 
        //            "Check Element Present command failed. Object " + locator.LogicalName 
        //            + " cannot be found in the webpage. Actual Error :  " + e.Message, LogLevel.VerificationError, 
        //            e.StackTrace, 
        //            customErrorMessage);
        //    }
        //}

        public bool CheckElementPresent(string objectName, string identifier, bool stopOnFailure, Object customErrorMessage = null)
        {
            string actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            return DoCheckElementPresent(locator, stopOnFailure, customErrorMessage);
        }

        public bool CheckElementPresent(string objectName, bool stopOnFailure, Object customErrorMessage = null)
        {
            return CheckElementPresent(objectName, "", stopOnFailure, customErrorMessage);
        }
        
        private bool DoCheckElementPresent(ObjectLocator locator, bool stopOnFailure, Object customErrorMessage = null)
        {
            string objectId = locator.ActualLocator;
            bool elementFound = false;
            try
            {
                UITestControl element = FindCodedUIElement(objectId);
                elementFound = element.WaitForControlExist(1000); // wait for 1 sec
                if (elementFound)
                {
                     reportResults("Check Element Present",
                               "Check Element Present command passed. Object "
                               + locator.LogicalName, LogLevel.Success);
                }
                else
                {
                    throw new Exception(" Element not found"); 
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                reportResults(stopOnFailure,
                   "Check Element Present",
                   "Check Element Present command failed. Object " + locator.LogicalName
                   + " cannot be found in the webpage. Actual Error :  " + e.Message, LogLevel.VerificationError,
                   e.StackTrace,
                   customErrorMessage);
            }
            return elementFound;
        }
        


        public void CheckObjectProperty(string objectName, string propertyName, Object expectedValue, bool stopOnFailure, Object customErrorMessage = null)
        {
            CheckObjectProperty(objectName, "", propertyName, expectedValue, stopOnFailure, customErrorMessage);
        }

        public void CheckObjectProperty(string objectName, string identifier, string propertyName, Object expectedValueObj, bool stopOnFailure, Object customErrorMessage = null)
        {
            string expectedValue = CheckNullValue(expectedValueObj, "Check Object Property", stopOnFailure);

            string actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);


            if (propertyName.Equals(ValidationType.ALLOPTIONS.ToString()))
            {
                CheckAllSelections(locator, propertyName, expectedValue, stopOnFailure, customErrorMessage);
            }
            else if (propertyName.Equals(ValidationType.ELEMENTPRESENT.ToString()))
            {
                IsElementPresent(locator, expectedValue, stopOnFailure, customErrorMessage);
            }
            else if (propertyName.Equals(ValidationType.MISSINGOPTION.ToString()))
            {
                CheckMissingOption(locator, propertyName, expectedValue, stopOnFailure, customErrorMessage);
            }
            else if (propertyName.Equals(ValidationType.PROPERTYPRESENT.ToString()))
            {
                CheckPropertyPresent(locator, propertyName, expectedValue, stopOnFailure, customErrorMessage);
            }
            else if (propertyName.Equals(ValidationType.SELECTEDOPTION.ToString()))
            {
                CheckSelectedOption(locator, propertyName, expectedValue, stopOnFailure, customErrorMessage);
            }
            else
            {
                DoCheckObjectProperty(locator, propertyName, expectedValue, stopOnFailure, customErrorMessage);
            }

        }

        private void DoCheckObjectProperty(ObjectLocator locator, string propertyName, string expectedValue, bool stopOnFailure, Object customErrorMessage = null)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;
            try
            {
                UITestControl element = FindCodedUIElement(objectId);
                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        string attributeValue = ValidateObjectProperty(element,
                            propertyName);

                        if (attributeValue.Trim().Equals(expectedValue.Trim()))
                        {
                            reportResults(
                                    "Check Object Property", 
                                    "Check Object Property command passed" + locator.LogicalName + ". Input Value " + propertyName + expectedValue, LogLevel.Success);
                            break;
                        }
                        else
                        {
                            reportResults(
                                    stopOnFailure, 
                                    "Check Object Property",
                                    "Check Object Property command failed" + locator.LogicalName + "."  
                                    + propertyName + " Object property value match expected. Expected value : "
                                    + expectedValue + " is not equal to the Actual value : " + attributeValue,
                                    LogLevel.VerificationError, 
                                    "", 
                                    customErrorMessage);

                            break;
                        }

                    }
                    catch (AssertFailedException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {
                            log.Error(e);
                            // reportResults(false, "Type", "Type command faied. Object " + locator.LogicalName + ". Input value : " + value + ". Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
                        }
                    }
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.Error(e);
                reportResults(stopOnFailure, 
                            "Check Object Property", 
                            "Check Object Property command failed. Object " + locator.LogicalName 
                            + " cannot be found in the webpage. Actual Error :  " + e.Message, 
                            LogLevel.Error, 
                            e.StackTrace,
                            customErrorMessage);
            }
        }

        private string ValidateObjectProperty(UITestControl element, string propertyName)
        {
           string attributeValue = "";
            
           try
           {
               attributeValue = element.GetProperty(propertyName).ToString(); ;

               if (attributeValue == null)
               {
                  throw new Exception("Attribute " + propertyName);
               }
            }
            catch (Exception e1)
            {

                throw new Exception("Attribute " + propertyName, e1);
            }
            return attributeValue;
        }

        private void sleep(int retryInterval)
        {
            try
            {
                Thread.Sleep(retryInterval);
            }
            catch (Exception e)
            {
                log.Error(e);
                throw e;
            }
        }

        private UITestControl FindCodedUIElement(string searchPath)
        {
            String[] stringSeparators = new String[] { "=>" };
            String[] objectHierarchy = searchPath.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

            UITestControl uiControl = buildUIControl(null, objectHierarchy[0]);

            for (int i = 1; i < objectHierarchy.Length; i++ )
            {
                String locatorInstance = objectHierarchy[i];
                uiControl = buildUIControl(uiControl, locatorInstance);
            }

            //Playback.PlaybackSettings.SearchTimeout = (retry * retryInterval);
            //Playback.PlaybackSettings.WaitForReadyLevel = 0;
            //uiControl.Find();

            return uiControl;
        }

        private UITestControl buildUIControl(UITestControl parent, String locatorInstance) 
        {
            UITestControl uiControl = null;
            String[] propHierarchy = locatorInstance.Split('|');

            Dictionary<string,string> searchProps = getSearchProps(propHierarchy);

            String objectType = searchProps["Object"];
            searchProps.Remove("Object");

            Type type = GetType("Microsoft.VisualStudio.TestTools.UITesting", objectType);

            if (parent == null)
            {
                uiControl = (UITestControl)Activator.CreateInstance(type);
            }
            else 
            {
                uiControl = (UITestControl)Activator.CreateInstance(type, parent);
            }

            //if (objectType.Equals("BrowserWindow"))
            //{
            //    uiControl = new BrowserWindow();
            //}
            //else 
            //{
            //    if (parent != null)
            //    {
            //        uiControl = new UITestControl(parent);
            //    }
            //    else 
            //    {
            //        uiControl = new UITestControl();
            //    }
            //}            

            foreach (KeyValuePair<string, string> entry in searchProps)
            {
                if (entry.Key.Equals("TechnologyName"))
                {
                    uiControl.TechnologyName = entry.Value;
                }
                else
                {
                    uiControl.SearchProperties.Add(entry.Key, entry.Value);
                }               
                
            }

            return uiControl;
        }

        private Type GetType(String namespaceStr, string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                String assemblyName = assembly.GetName().Name;
                if (assemblyName.Equals(namespaceStr))
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type t in types)
                    {
                        if (t.Name.Equals(typeName))
                        {
                            return t;
                        }
                    }

                }

            }
            return null;
        }

        private Dictionary<string, string> getSearchProps(string[] propHierarchy)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();

            foreach (String searchProperty in propHierarchy)
            {
                String[] stringSeparators = new String[] { ":=" };
                String[] propKV = searchProperty.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                props.Add(propKV[0], propKV[1]);
            }

            return props;
        }





        //private IWebElement FindWebElement(string searchPath)
        //{
        //    IWebDriver driver = Driver;
        //    By locator = GetLocatorType(searchPath);
        //    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds((retry * retryInterval)));
        //    wait.PollingInterval = TimeSpan.FromMilliseconds(retryInterval);
        //    wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        //    IWebElement element = wait.Until<IWebElement>((webDriver) =>
        //    {
        //        return webDriver.FindElement(locator);
        //    });

        //    if (element != null)
        //    {
        //        try
        //        {
        //            IJavaScriptExecutor jsExecutor = driver as IJavaScriptExecutor;
        //            jsExecutor.ExecuteScript("arguments[0].scrollIntoView(false);", element);
        //        }
        //        catch (Exception e)
        //        {
        //            log.Error(e);
        //        }
        //    }

        //    return element;
        //}


        private By GetLocatorType(String searchPath)
        {
            string typeString = "";
            string reference = "";
            string searchPathInLowerCase = searchPath.ToLower();
            bool isObjectTypeisXpath = searchPathInLowerCase.StartsWith("/");
            if (!isObjectTypeisXpath)
            {
                try
                {
                    typeString = searchPath
                            .Substring(0, searchPath.IndexOf('='));
                    reference = searchPath.Replace(typeString + "=", "");
                }
                catch (Exception)
                {
                    throw new Exception("Invalid Locator Passed " + searchPath);
                }
            }
            if (isObjectTypeisXpath)
            {

                return By.XPath(searchPath);
            }
            else if (typeString.Contains("xpath"))
            {

                return By.XPath(reference);
            }
            else if (typeString.Contains("css"))
            {

                return By.CssSelector(reference);
            }
            else if (typeString.Contains("id"))
            {

                return By.Id(reference);
            }
            else if (typeString.Contains("link"))
            {

                return By.LinkText(reference);
            }
            else if (typeString.Contains("tagname"))
            {

                return By.TagName(reference);
            }
            else if (typeString.Contains("name"))
            {

                return By.Name(reference);
            }
            else if (typeString.Contains("classname"))
            {

                return By.ClassName(reference);
            }

            throw new Exception("Invalid Locator Type Passed " + reference);
        }



        public System.Collections.Generic.IEnumerable<Dictionary<string, object>> GetVirtualDataTable(params String[] tableNames)
        {
            IDataTableParser parser = new DataTableParser();

            List<List<String>> columns = new List<List<String>>();
            List<String> headers = new List<String>();
            bool isMultipleDataTable = false;
            int colCount = 0;
            int rowCount = 0;
            int currentTableColCount = 0;

            for (int index = 0; index < tableNames.Length; index++)
            {

                Table table = parser.getDataTable(tableNames[index]);

                if (table.rows.Count > rowCount)
                {
                    rowCount += table.rows.Count;
                }



                colCount = currentTableColCount;

                currentTableColCount += table.getColumnCount();

                foreach (Column column in table.header[0].columns)
                {
                    headers.Add(table.tableName + "_" + column.name);
                    List<string> col = new List<string>();
                    columns.Add(col);

                    if (index > 0)
                    {
                        isMultipleDataTable = true;

                    }
                }

                foreach (Row row in table.rows)
                {
                    for (int i = 0; i < row.value.Count; i++)
                    {
                        if (!isMultipleDataTable)
                        {
                            columns[i].Add(row.value[i]);
                        }
                        else
                        {
                            columns[(i + colCount)].Add(row.value[i]);
                        }

                    }
                }
            }

            for (int i = 0; i < rowCount; i++)
            {
                Dictionary<string, object> objd = new Dictionary<string, object>();
                object[] obj = new object[columns.Count];
                for (int j = 0; j < columns.Count; j++)
                {
                    try
                    {
                        objd.Add(headers[j], columns[j][i]);
                        obj[j] = columns[j][i];
                    }
                    catch (Exception)
                    {
                        obj[j] = null;
                    }
                }
                yield return objd;
            }
        }


        private void CheckAllSelections(ObjectLocator locator, string propertyName, string expectedValue, bool stopOnFailure, Object customErrorMessage = null)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            string verificationErrors = "";
            int counter = retry;
            string[] actualSelectOptions = null;
            Object selectElement = null;
            bool isWebParadigm = false;

            try
            {
                //IWebElement element = FindWebElement(objectId);
                UITestControl element = FindCodedUIElement(objectId);
                string[] expectedSelectOptions = expectedValue.Split(',');

                string paradigm = element.TechnologyName;
                if (paradigm.Equals("Web", StringComparison.OrdinalIgnoreCase))
                {
                    selectElement = (HtmlComboBox)element;
                    isWebParadigm = true;
                }
                else
                {
                    selectElement = (WinComboBox)element;
                    isWebParadigm = false;
                }

                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        //SelectElement selectElement = new SelectElement(element);
                        //IList<IWebElement> elementOptions = selectElement.Options;
                        //actualSelectOptions = new string[elementOptions.Count];

                        UITestControlCollection elementOptions = null;

                        if (selectElement is HtmlComboBox)
                        {
                            HtmlComboBox tempElement = selectElement as HtmlComboBox;
                            actualSelectOptions = new string[tempElement.ItemCount];
                            elementOptions = tempElement.Items;
                        }
                        else if (selectElement is WinComboBox)
                        {
                            WinComboBox tempElement = selectElement as WinComboBox;
                            actualSelectOptions = new string[tempElement.Items.Count];
                            elementOptions = tempElement.Items;
                        }

                        for (int i = 0; i < elementOptions.Count; i++)
                        {
                            if (isWebParadigm)
                            {
                                HtmlListItem control = (HtmlListItem)elementOptions[i];
                                actualSelectOptions[i] = control.DisplayText;
                                System.Diagnostics.Debug.WriteLine("Available options in Web : " + control.DisplayText);
                            }
                            else
                            {
                                WinListItem control = (WinListItem)elementOptions[i];
                                actualSelectOptions[i] = control.DisplayText;
                                System.Diagnostics.Debug.WriteLine("Available options in Win : " + control.DisplayText);
                            }
                            //actualSelectOptions[i] = elementOptions[i].Text;
                        }

                        if (actualSelectOptions.Length != expectedSelectOptions.Length)
                        {
                            reportResults(
                                stopOnFailure,
                                "Check All Selected Options",
                                "Check All Selected Options command failed. Object " + locator.LogicalName + "." + propertyName
                                        + " : Expected options count : "
                                        + expectedSelectOptions.Length
                                        + " is different from the Actual options count : "
                                        + actualSelectOptions.Length
                                        + " : Expected : " + string.Join(",", expectedSelectOptions)
                                        + " Actual : " + string.Join(",", actualSelectOptions),
                                 LogLevel.VerificationError,
                                 "",
                                 customErrorMessage
                            );


                            break;
                        }

                        StringBuilder verificationErrorBuilder = CompareActualOptionValuesWithExpected(actualSelectOptions, expectedSelectOptions);
                        verificationErrors = verificationErrorBuilder.ToString();

                        // If there is a mismatch
                        if (verificationErrors.Length > 0)
                        {

                            reportResults(
                                     stopOnFailure,
                                     "Check All Selected Options",
                                     "Check All Selected Options command failed. Object " + locator.LogicalName + "." + propertyName
                                     + " : Expected : " + string.Join(",", expectedSelectOptions)
                                     + " Actual : " + string.Join(",", actualSelectOptions),
                                     LogLevel.VerificationError,
                                     "",
                                     customErrorMessage
                                );

                            break;

                        }
                        else
                        {

                            reportResults(
                                  "Check All Selected Options",
                                  "Check All Selected Options command passed. Object " + locator.LogicalName + "." + propertyName
                                   + " : Expected : " + string.Join(",", expectedSelectOptions)
                                        + " Actual : " + string.Join(",", actualSelectOptions),
                                   LogLevel.Success
                              );
                            break;
                        }

                        // If the length of the input does not match with the actual option count

                    }
                    catch (AssertFailedException e)
                    {
                        throw e;
                    }
                    catch (StaleElementReferenceException staleElementException)
                    {

                        element = FindCodedUIElement(objectId);
                        log.Warn(staleElementException);
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);

                        if (!(counter > 0))
                        {
                            log.Error(e);
                            reportResults(
                                    stopOnFailure,
                                    "Check All Selected Options",
                                    "Check All Selected Options command failed. Object " + locator.LogicalName + "." + propertyName + " : Actual Error : " + e.Message
                                     + " : Expected : " + string.Join(",", expectedSelectOptions)
                                        + " Actual : " + string.Join(",", actualSelectOptions),
                                     LogLevel.VerificationError,
                                     e.StackTrace,
                                     customErrorMessage
                                );
                        }
                    }
                }

            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {

                log.Error(e);
                reportResults(
                             stopOnFailure,
                             "Check All Selected Options",
                             "Check All Selected Options command failed. Object " + locator.LogicalName + "." + propertyName + " cannot be found in the webpage. Actual Error : "
                             + e.Message,
                             LogLevel.VerificationError,
                             e.StackTrace,
                             customErrorMessage
                        );
            }
        }


        //------------------------start---------------------------------------------------------------

        private StringBuilder CompareActualOptionValuesWithExpected(string[] actualSelectOptions, string[] expectedSelectOptions)
        {

            StringBuilder verificationErrorBuilder = new StringBuilder();
            for (int optionIndex = 0; optionIndex < actualSelectOptions.Length; optionIndex++)
            {

                IList<string> listActualSelOptions = actualSelectOptions.ToList();
                if (!listActualSelOptions.Contains(expectedSelectOptions[optionIndex]))
                {

                    verificationErrorBuilder.Append("\n Option :"
                                    + optionIndex
                                    + " : "
                                    + expectedSelectOptions[optionIndex]
                                    + " Option is not available in the actual element. Actual ["
                                    + string.Join(",", actualSelectOptions) + "]");

                }
            }
            return verificationErrorBuilder;
        }


        private void CheckSelectedOption(ObjectLocator locator, string propertyname, string expectedValue, bool stopOnFailure, Object customErrorMessage = null)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;

            try
            {

                //IWebElement element = FindWebElement(objectId);
                UITestControl element = FindCodedUIElement(objectId);
                Object selectElement = null;
                string paradigm = element.TechnologyName;
                if (paradigm.Equals("Web", StringComparison.OrdinalIgnoreCase))
                {
                   selectElement = (HtmlComboBox)element;
                }
                else
                {
                    selectElement = (WinComboBox)element;
                }
                //SelectElement selectElement = new SelectElement(element);

                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        string selectedOptionLabel = null;

                        if (selectElement is HtmlComboBox)
                        {
                            HtmlComboBox tempElement = selectElement as HtmlComboBox;
                            selectedOptionLabel = tempElement.SelectedItem;
                        }
                        else if (selectElement is WinComboBox)
                        {
                            WinComboBox tempElement = selectElement as WinComboBox;
                            selectedOptionLabel = tempElement.SelectedItem;
                        }
                        //string selectedOptionLabel = selectElement.SelectedOption.Text;

                        if (selectedOptionLabel.Equals(expectedValue))
                        {

                            reportResults(
                                "Check Selected Options",
                                "Check Selected Options : " + locator.LogicalName + "." + propertyname + " passed. Input Value " + selectedOptionLabel,
                                LogLevel.Success
                                );
                            break;
                        }
                        else
                        {
                            //if (stopOnFailure) -> change LogLevel
                            reportResults(stopOnFailure,
                              "Check Selected Options",
                              "Check Selected Options :" + locator.LogicalName + "." + propertyname + " failed. Expected : " + expectedValue + ", Input Value : " + selectedOptionLabel,
                              LogLevel.Error,
                              "",
                              customErrorMessage);
                            break;
                        }

                    }
                    catch (AssertFailedException e)
                    {
                        throw e;
                    }
                    catch (StaleElementReferenceException staleElementException)
                    {
                        log.Warn(staleElementException);
                        element = element = FindCodedUIElement(objectId);
                        //selectElement = new SelectElement(element);

                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        log.Error(e);
                        if (!(counter > 0))
                        {
                            reportResults(stopOnFailure,
                                 "Check Selected Options",
                                 "Check Selected Options : " + locator.LogicalName + "." + propertyname + " failed. Cannot access element. Actual Error : " + e.Message,
                                 LogLevel.Error,
                                 e.StackTrace,
                                 customErrorMessage);
                        }
                    }
                }

            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                reportResults(stopOnFailure,
                "Check Selected Options",
                "Check Selected Options : " + locator.LogicalName + "." + propertyname + " failed. Element not present. Actual Error : " + e.Message,
                LogLevel.Error,
                e.StackTrace,
                customErrorMessage);
            }
        }


           private void CheckMissingOption(ObjectLocator locator, string propertyName, string expectedValue, bool stopOnFailure, Object customErrorMessage = null)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;
            bool failFlag = false;
            Object selectElement = null;
            bool isWebParadigm = false;

            try
            {


                //IWebElement element = FindWebElement(objectId);
                //SelectElement selectElement = new SelectElement(element);

                UITestControl element = FindCodedUIElement(objectId);
                string paradigm = element.TechnologyName;
                if (paradigm.Equals("Web", StringComparison.OrdinalIgnoreCase))
                {
                    selectElement = (HtmlComboBox)element;
                    isWebParadigm = true;
                }
                else
                {
                    selectElement = (WinComboBox)element;
                    isWebParadigm = false;
                }

                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        string[] selectOptions = null;
                        UITestControlCollection elementOptions = null;

                        if (selectElement is HtmlComboBox)
                        {
                            HtmlComboBox tempElement = selectElement as HtmlComboBox;
                            selectOptions = new string[tempElement.ItemCount];
                            elementOptions = tempElement.Items;
                        }
                        else if (selectElement is WinComboBox)
                        {
                            WinComboBox tempElement = selectElement as WinComboBox;
                            selectOptions = new string[tempElement.Items.Count];
                            elementOptions = tempElement.Items;
                        }

                        if (elementOptions != null)
                        {
                            for (int i = 0; i < elementOptions.Count; i++)
                            {
                                if (isWebParadigm)
                                {
                                    HtmlListItem control = (HtmlListItem)elementOptions[i];
                                    selectOptions[i] = control.DisplayText;
                                    System.Diagnostics.Debug.WriteLine("Available options in Web : " + control.DisplayText);
                                }
                                else
                                {
                                    WinListItem control = (WinListItem)elementOptions[i];
                                    selectOptions[i] = control.DisplayText;
                                    System.Diagnostics.Debug.WriteLine("Available options in Win : " + control.DisplayText);
                                }
                            }
                        }


                        foreach (string option in selectOptions)
                        {
                            if (option.Equals(expectedValue))
                            {
                                //if (stopOnFailure) -> change LogLevel
                                reportResults(stopOnFailure,
                                      "Check Missing Option",
                                      "Check Missing Option : " + locator.LogicalName + "." + propertyName + " failed. Option : " + expectedValue
                                      + " : of object [" + locator.LogicalName + "] is present.",
                                      LogLevel.Error,
                                      "",
                                      customErrorMessage);
                                failFlag = true;
                                break;
                            }
                        }
                        if (failFlag == false)
                        {
                            reportResults(
                                "Check Missing Option",
                                "Check Missing Option : " + locator.LogicalName + "." + propertyName + " passed. Input Value = " + expectedValue,
                                LogLevel.Success
                                );
                        }
                    }
                    catch (AssertFailedException e)
                    {
                        throw e;
                    }
                    catch (StaleElementReferenceException staleElementException)
                    {
                        log.Warn(staleElementException);
                        element = element = FindCodedUIElement(objectId);
                        //selectElement = new SelectElement(element);
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {
                            reportResults(stopOnFailure,
                                      "Check Missing Option",
                                      "Check Missing Option : " + locator.LogicalName + "." + propertyName + " failed. Element ["
                                      + locator.LogicalName + "] is not accessible. Actual Error : " + e.Message,
                                      LogLevel.Error,
                                      e.StackTrace,
                                      customErrorMessage);
                        }
                    }
                }

            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {

                reportResults(stopOnFailure,
                          "Check Missing Option",
                          "Check Missing Option : " + locator.LogicalName + "." + propertyName + " failed. Element ["
                          + locator.LogicalName + "] is not present. Actual Error : " + e.Message,
                          LogLevel.Error,
                          e.StackTrace,
                          customErrorMessage);
            }
        }
        //-------------------------NEW-------------------------------------

        private void CheckPropertyPresent(ObjectLocator locator, string property, string expectedValue, bool stopOnFailure, Object customErrorMessage = null)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;

            string propertyName = "";
            string condition = "";
            string @preSplit = expectedValue;
            try
            {

                //IWebElement element = FindWebElement(objectId);
                UITestControl element = FindCodedUIElement(objectId);

                try
                {
                    //String[] stringSeparators = new String[] { "\\|" };
                    string[] commandSet = preSplit.Split('|');
                    //String[] commandSet = expectedValue.Split(stringSeparators, StringSplitOptions.None);
                    propertyName = commandSet[0];
                    condition = commandSet[1];

                }
                catch (Exception e)
                {

                    reportResults(stopOnFailure,
                         "Check Object Property",
                         "Check Object Property : " + locator.LogicalName + "." + propertyName + " failed. User inputs ["
                         + expectedValue + "] are not in the correct format. Correct format: attributeName|condition. Actual Error : " + e.Message,
                         LogLevel.Error,
                         e.StackTrace,
                         customErrorMessage);
                    return;
                }


                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        bool isAttributePresent = CheckAttributePresent(element, propertyName);

                        if (string.Equals(isAttributePresent.ToString(), condition.Trim(), StringComparison.OrdinalIgnoreCase))
                        {

                            reportResults(
                                "Check Object Property Present",
                                "Check Object Property Present : " + locator.LogicalName + "." + propertyName + " passed.  Expected : " + expectedValue + " Actual : " + isAttributePresent.ToString(),
                                LogLevel.Success
                                );
                            break;

                        }
                        else
                        {

                            reportResults(stopOnFailure,
                               "Check Object Property Present",
                               "Check Object Property Present : " + locator.LogicalName + "." + propertyName + " failed. Expected : " + expectedValue + " Actual : " + isAttributePresent.ToString(),
                               LogLevel.Error,
                               "",
                               customErrorMessage);

                            break;
                        }

                    }
                    catch (AssertFailedException e)
                    {
                        throw e;
                    }
                    catch (StaleElementReferenceException staleElementException)
                    {
                        log.Warn(staleElementException);
                        //element = FindWebElement(objectId);
                        element = FindCodedUIElement(objectId);

                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {

                            reportResults(stopOnFailure,
                            "Check Object Property Present",
                            "Check Object Property Present : " + locator.LogicalName + "." + propertyName + " failed. Element is not accessible. Actual Error : " + e.Message,
                            LogLevel.Error,
                            e.StackTrace,
                            customErrorMessage);
                        }
                    }
                }

            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {

                reportResults(stopOnFailure,
                "Check Object Property Present",
                "Check Object Property Present : " + locator.LogicalName + "." + propertyName + " failed. Element is not present. Actual Error : " + e.Message,
                LogLevel.Error,
                e.StackTrace,
                customErrorMessage);
            }
        }

        //-------------------------
        private bool CheckAttributePresent(UITestControl element, string propertyName)
        {

            bool isAttributePresent;

            if (String.Equals("textContent", propertyName, StringComparison.OrdinalIgnoreCase))
            {
                String textValue = element.GetProperty("Text").ToString();
                if ("".Equals(textValue) || textValue == null)
                {
                    isAttributePresent = false;
                }
                else
                {
                    isAttributePresent = true;
                }
            }
            else
            {
                if (element.GetProperty(propertyName.ToUpper()) != null)
                {
                    isAttributePresent = true;
                }
                else
                {
                    isAttributePresent = false;
                }
            }
            return isAttributePresent;
        }


        //-----------------------------------------------------

        private void IsElementPresent(ObjectLocator locator, string expectedValue, bool stopOnFailure, Object customErrorMessage = null)
        {
            string objectId = locator.ActualLocator;
            int counter = retry;
 
            while (counter > 0)
            {
                try
                {
                    counter--;

                    bool isObjectFound = DoCheckElementPresent(locator, stopOnFailure);

                    if (String.Equals(isObjectFound.ToString(), expectedValue, StringComparison.OrdinalIgnoreCase))
                    {
                        reportResults(
                            "Check Element Present",
                            "Check Element Present passed. Element : " + locator.LogicalName + ". Actual same as expected : " + expectedValue,
                            LogLevel.Success);
                        break;
                    }
                    else
                    {
                        if (counter < 1)
                        {
                            reportResults(stopOnFailure,
                            "Check Element Present",
                            "Check Element Present failed. Element : " + locator.LogicalName + ". Expected : " + expectedValue
                            + ", Actual : " + isObjectFound,
                            LogLevel.Error,
                            "",
                            customErrorMessage);
                            break;
                        }
                    }

                }
                catch (AssertFailedException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    sleep(retryInterval);

                    reportResults(stopOnFailure,
                    "Check Element Present",
                    "Check Element Present failed. Element : " + locator.LogicalName + " is not accessible. Actual Error : " + e.Message,
                    LogLevel.Error,
                    e.StackTrace,
                    customErrorMessage);
                    break;
                }
            }

        }

        //-----------------------------------------


        private void CheckObjectOtherProperty(ObjectLocator locator, string propertyName, string expectedValue, bool stopOnFailure, Object customErrorMessage = null)
        {

            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;

            try
            {
                UITestControl element = FindCodedUIElement(objectId);

                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        string attributeValue = ValidateObjectProperty(element,
                                propertyName);
                        if (attributeValue.Trim().Equals(expectedValue.Trim()))
                        {

                            reportResults(
                                "Check Object Property",
                                "Check Object Property passed. Object : " + locator.LogicalName + ", Input : " + expectedValue,
                                LogLevel.Success
                            );
                            break;
                        }
                        else
                        {

                            reportResults(stopOnFailure,
                                "Check Object Property",
                                "Check Object Property failed. Object property value match expected. " +
                                "Object : " + locator.LogicalName + ", Expected : " + expectedValue +
                                ", Actual value : " + attributeValue.Trim(),
                                LogLevel.Error, 
                                "", 
                                customErrorMessage
                                );
                            break;
                        }

                    }
                    catch (AssertFailedException e)
                    {
                        throw e;
                    }
                    catch (StaleElementReferenceException staleElementException)
                    {
                        element = FindCodedUIElement(objectId);
                        log.Warn(staleElementException);
                    }
                    catch (Exception e)
                    {

                        sleep(retryInterval);

                        if (!(counter > 0))
                        {
                            if (e.Message.StartsWith("Attribute"))
                            {

                                reportResults(stopOnFailure,
                                    "Check Object Property",
                                    "Check Object Property failed. Attribute : " + propertyName + " not present. Actual Error : " + e.Message,
                                    LogLevel.Error,
                                    e.StackTrace,
                                    customErrorMessage);

                            }
                            else if (e.Message.StartsWith("Element"))
                            {

                                reportResults(stopOnFailure,
                                    "Check Object Property",
                                    "Check Object Property failed. Element : " + locator.LogicalName + " not present. Actual Error : " + e.Message,
                                    LogLevel.Error,
                                    e.StackTrace,
                                    customErrorMessage);
                            }
                        }
                    }
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                /*
                 * after the retry amout, if still the object is not found, report
                 * the failure error will be based on the exception message, if e
                 * contains attribute report attribute failure else if e contains
                 * element, report object not found
                 */


                reportResults(stopOnFailure,
                    "Check Object Property",
                    "Check Object Property failed. Element : " + locator.LogicalName + " not present. Actual Error : " + e.Message,
                    LogLevel.Error,
                    e.StackTrace,
                    customErrorMessage);
            }
        }

        //-------------------------HERE HERE HERE HERE HERE-------------------------------------  

        private string CheckNullObject(Object obj, string command, Object customErrorMessage = null)
        {
            string value = null;
            try
            {
                //LOGIC CORRECT?
                value = obj.ToString();
            }
            catch (NullReferenceException e)
            {
                reportResults(true,
                  "Check Null Object",
                  "Check Null Object failed. Invalid input. Cannot use null as input. Actual Error : " + e.Message,
                  LogLevel.Error,
                  e.StackTrace,
                  customErrorMessage);
            }
            return value;
        }


        //move up the document ^
        public enum TableValidationType
        {

            /** The colcount. */
            COLCOUNT,
            /** The rowcount. */
            ROWCOUNT,
            /** The tabledata. */
            TABLEDATA,
            /** The relative. */
            RELATIVE,
            /** The tablecell. */
            TABLECELL
        };


        public void CheckTable(string objectName, string validationTypes, Object expectedValue, bool stopOnFaliure, Object customErrorMessage = null)
        {
            CheckTable(objectName, "", validationTypes, expectedValue, stopOnFaliure, customErrorMessage);
        }

        public void CheckTable(string objectName, string identifier, string validationTypes, Object expectedValue, bool stopOnFaliure, Object customErrorMessage = null)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoCheckTable(locator, validationTypes, expectedValue, stopOnFaliure, customErrorMessage);
        }

        private void DoCheckTable(ObjectLocator locator, string validationTypes, Object checkExpValueObj, bool stopOnFaliure, Object customErrorMessage = null)
        {
            string checkExpValue = CheckNullValue(checkExpValueObj, "Check Table", stopOnFaliure);
            TableValidationType validationType = (TableValidationType)Enum.Parse(typeof(TableValidationType), validationTypes, true);

            string expectedValue = CheckNullObject(checkExpValue, "CHECK TABLE");
            string objectId = locator.ActualLocator;

            //string actualLocator = objectMapParser.getResolvedObjectSearchPath(locator.LogicalName, locator.ActualLocator);

            try
            {
                //IWebElement element = FindWebElement(locator.ActualLocator);
                UITestControl element = FindCodedUIElement(objectId);

                // Call the relavant internal method based on the
                // TableValidationType provided by the user
                try
                {
                    if (validationType == TableValidationType.ROWCOUNT)
                    {
                        ValidateTableRowCount(element, locator.ActualLocator, expectedValue, stopOnFaliure);
                    }
                    else if (validationType == TableValidationType.COLCOUNT)
                    {
                        ValidateTableColCount(element, locator.ActualLocator, expectedValue, stopOnFaliure);
                    }
                    else if (validationType == TableValidationType.TABLEDATA)
                    {

                        CompareTableData(element, locator.ActualLocator, expectedValue, stopOnFaliure);
                    }
                    else if (validationType == TableValidationType.RELATIVE)
                    {

                        ValidateTableOffset(element, locator.ActualLocator, expectedValue, stopOnFaliure);
                    }
                    else if (validationType == TableValidationType.TABLECELL)
                    {

                        ValidateCellValue(element, locator.ActualLocator, expectedValue, stopOnFaliure);
                    }
                }
                catch (AssertFailedException e)
                {
                    throw e;
                }
                //catch (Exception e)
                //{
                //    // waiting for the maximum amount of waiting time before
                //    // failing the test case
                //    // Several checks were introduced to narrow down to the
                //    // failure to the exact cause.

                //    reportResults(
                //        false,
                //        "Check Table " + validationType,
                //        "Check Table command failed. Element : " + locator.LogicalName + " [" + locator.ActualLocator + "] is not accessible.",
                //        LogLevel.Error,
                //        e.StackTrace
                //    );
                //}
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                // if object is not present catch the exception and repor the
                // error
                log.Warn(e);

                reportResults(
                      stopOnFaliure,
                      "Check Table " + validationType,
                      "Check Table command failed. Element : " + locator.LogicalName + " [" + locator.ActualLocator + "] not present. Actual Error : " + e.Message,
                      LogLevel.Error,
                      e.StackTrace,
                      customErrorMessage
                );
            }

        }
        //---------------------------------------
        private int ValidateTableRowCount(UITestControl element, string tableName, string expectedValue, bool stopOnFailure, Object customErrorMessage = null)
        {
            int rowCount = 0;
            try
            {

                //rowCount = element.FindElements(By.TagName("tr")).Count;
                string paradigm = element.TechnologyName;
                if (paradigm.Equals("Web", StringComparison.OrdinalIgnoreCase))
                {
                    rowCount = ((HtmlTable)element).RowCount;
                }
                else
                {
                    rowCount = ((WinTable)element).Rows.Count;
                }
                

                if (rowCount == int.Parse(expectedValue))
                {
                    reportResults(
                          "Check Table: Row Count",
                          "Check Table: Row Count command passed. Table : " + tableName + " Input value : "
                          + expectedValue,
                           LogLevel.Success
                      );
                }
                else
                {
                    reportResults(
                        stopOnFailure,
                        "Check Table: Row Count",
                        "Check Table: Row Count command failed. Table : " + tableName + ". Expected : "
                        + expectedValue + ", Actual : " + rowCount,
                        LogLevel.Error,
                        "",
                        customErrorMessage
                     );
                }

            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.Warn(e);
                reportResults(
                     stopOnFailure,
                     "Check Table: Row Count",
                     "Check Table: Row Count command failed. Table : " + tableName + ". Actual Error : " + e.Message,
                     LogLevel.Error,
                     e.StackTrace,
                     customErrorMessage
                );
            }
            return rowCount;
        }
        //---------------------------------------
        private int ValidateTableColCount(UITestControl element, string tableName, string expectedValue, bool stopOnFailure, string customErrorMessage = null)
        {
            //IList<IWebElement> rowElements = null;
            int actualValue = 0;

            try
            {

                //rowElements = element.FindElements(By.TagName("tr"));
                //actualValue = rowElements[1].FindElements(By.TagName("td")).Count();
                string paradigm = element.TechnologyName;
                if (paradigm.Equals("Web", StringComparison.OrdinalIgnoreCase))
                {
                    actualValue = ((HtmlTable)element).ColumnCount;
                }
                else
                {
                    actualValue = ((WinTable)element).ColumnHeaders.Count;
                }

                // actualdValue = CodedUI.getEval(JS);

                if (actualValue == int.Parse(expectedValue))
                {

                    reportResults(
                         "Check Table: Column Count",
                         "Check Table: Column Count command passed. Table : " + tableName + " Input Value : " + expectedValue,
                         LogLevel.Success
                      );

                }
                else
                {
                    reportResults(
                     stopOnFailure,
                     "Check Table: Column Count",
                     "Check Table: Column Count command failed. Table : " + tableName + ". Expected : " + expectedValue + ", Actual : " + actualValue,
                     LogLevel.Error,
                     "",
                     customErrorMessage
                  );
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                log.Warn(e);
                reportResults(
                          stopOnFailure,
                          "Check Table: Column Count",
                          "Check Table: Column Count command failed. Table : " + tableName + ", column count mismatch. Expected : "
                          + expectedValue + ", Actual : " + actualValue + ". Actual Error : " + e.Message,
                          LogLevel.Error,
                          e.StackTrace,
                          customErrorMessage
                       );
            }
            return actualValue;
        }


        //---------------------------------------

        //private IList<string> GetAppTable(IWebElement element)
        //{

        //    IWebElement rowElement;
        //    IList<IWebElement> columnElements;
        //    IList<IWebElement> rowElements;

        //    List<string> htmlTable = new List<string>();

        //    rowElements = element.FindElements(By.TagName("tr"));
        //    int rowNum = rowElements.Count;

        //    if (rowNum > 0)
        //    {

        //        string value = "";
        //        for (int i = 0; i < rowNum; i++)
        //        {

        //            rowElement = rowElements[i];

        //            columnElements = rowElement.FindElements(By.TagName("td"));
        //            if (columnElements == null)
        //            {
        //                columnElements = rowElement.FindElements(By.TagName("th"));
        //            }

        //            int colNum = columnElements.Count;

        //            for (int j = 0; j < colNum; j++)
        //            {

        //                value = columnElements[j].Text;

        //                if (value != null)
        //                {
        //                    htmlTable.Add(value);
        //                }
        //                else
        //                {
        //                    htmlTable.Add("");
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception();

        //    }
        //    return htmlTable;
        //}


        //---------------------------------------
        private void CompareTableData(UITestControl element, string tableName, string expectedValue, bool stopOnFailure, Object customErrorMessage = null)
        {
            IList<string> htmlTable;
            IList<string> inputTable;
            string paradigm = element.TechnologyName;
            try
            {
                //htmlTable = GetAppTable(element);
                if (paradigm.Equals("Web", StringComparison.OrdinalIgnoreCase))
                {
                    HtmlTable actTable = (HtmlTable)element;
                    htmlTable = actTable.GetContent();
                }
                else
                {
                    WinTable actTable = (WinTable)element;
                    htmlTable = actTable.GetContent();
                }


                inputTable = new List<string>(expectedValue.Split(',').ToList<string>());

                List<string> tempInputTable = new List<string>();

                foreach (string inputVal in inputTable)
                {
                    string formattedValue = inputVal.Replace("\\\\", ",");
                    tempInputTable.Add(formattedValue);
                }

                inputTable = tempInputTable;

                string inputTableStr = string.Join("|", inputTable);
                string actualTableStr = string.Join("|", htmlTable);

                if (actualTableStr.Contains(inputTableStr))
                {

                    reportResults("Check Table: Table Data",
                            "Check Table: Table Data command passed. Table : " + tableName,
                            LogLevel.Success);

                }
                else
                {
                    string inputTableString = inputTable.ToString();
                    string htmlTableString = htmlTable.ToString();


                    reportResults(
                           stopOnFailure,
                           "Check Table: Table Data",
                           "Check Table: Table Data command failed. Table : " + tableName + ", table data is not as expected. Expected : "
                           + string.Join(",", inputTableString) + ", Actual : " + string.Join(",", htmlTableString),
                           LogLevel.Error,
                           "",
                           customErrorMessage
                        );
                }

            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                reportResults(
                          stopOnFailure,
                          "Check Table: Table Data",
                          "Check Table: Table Data command failed. Actual Error : " + e.Message,
                          LogLevel.Error,
                          e.StackTrace,
                          customErrorMessage
                       );
            }
        }


        public void ValidateTableOffset(UITestControl element, string objectName, Object expectedValueObj, bool stopOnFailure, Object customErrorMessage = null)
        {
            string expectedValue = CheckNullValue(expectedValueObj, "Validate Table Offset", true);
            List<string> inputStringArray;
            string parentText = "";
            int offset;
            IList<string> htmlTable;

            string inputStringCurrStr = "";
            string result = "";
            string cellText = "";
            //htmlTable = GetAppTable(element);

            string paradigm = element.TechnologyName;
            if (paradigm.Equals("Web", StringComparison.OrdinalIgnoreCase))
            {
                HtmlTable actTable = (HtmlTable)element;
                htmlTable = actTable.GetContent();
            }
            else
            {
                WinTable actTable = (WinTable)element;
                htmlTable = actTable.GetContent();
            }



            StringBuilder resultBuilder = new StringBuilder();

            List<string> inputStringCurrArray = new List<string>();

            inputStringArray = new List<string>(expectedValue.Split('#').ToList<string>());

  			if (expectedValue.EndsWith("#"))
            {
                inputStringArray.RemoveAt(inputStringArray.Count - 1);
            }

            for (int i = 0; i < inputStringArray.Count; i++)
            {

                inputStringCurrStr = @inputStringArray[i];

                var regexSplitter = new Regex(@"(?<!\\),");
                
                
                inputStringCurrArray.Clear();
                inputStringCurrArray.AddRange(regexSplitter.Split(inputStringCurrStr));


                List<string> tempInputTable = new List<string>();

                foreach (string inputVal in inputStringCurrArray)
                {
                    string formattedValue = inputVal.Replace("\\", "");

                    tempInputTable.Add(formattedValue);

                }
                inputStringCurrArray = tempInputTable;

                parentText = inputStringCurrArray[0];

                offset = Int32.Parse(inputStringCurrArray[1]);

                cellText = inputStringCurrArray[2];
                resultBuilder.Append(CheckIfTheTableContainsTheExpectedRelativeValue(htmlTable, parentText, offset, cellText));
            }

            result = resultBuilder.ToString();
            if (result.Length != 0)
            {

                reportResults(
                      stopOnFailure,
                      "Check Table: Relative",
                      "Check Table: Relative command failed. " + objectName + "'s relative validation is not as expected. "
                      + result,
                      LogLevel.Error,
                      "",
                      customErrorMessage
                   );

            }
            else
            {
                reportResults(
                        "Check Table: Relative",
                        "Check Table: Relative command passed. " + objectName + ", Input Value : " + expectedValue
                        + result,
                        LogLevel.Success
                     );
            }

        }

        private String CheckIfTheTableContainsTheExpectedRelativeValue(IList<string> htmlTable, string parentText, int offset, string cellText)
        {            
            int indexParent;
            StringBuilder resultBuilder = new StringBuilder();
            if (htmlTable.Contains(parentText))
            {

                List<int> parentTextIndexList = new List<int>();
                for (int k = 0; k < htmlTable.Count; k++)
                {
                    if (htmlTable[k].Equals(parentText))
                    {
                        parentTextIndexList.Add(k);
                    }
                }
                for (int j = 0; j < parentTextIndexList.Count; j++)
                {
                    indexParent = parentTextIndexList[j];
                    String actualText = "";
                    try
                    {
                        actualText = htmlTable[indexParent + offset];
                        if (!cellText.Equals(actualText))
                        {
                            resultBuilder.Append("|Expected : " + cellText + " Actual :" + actualText + " Base value : " + parentText + "\n");
                        }
                        else
                        {
                            break;
                        }

                    }
                    catch (Exception ex)
                    {
                        resultBuilder.Append("|Expected value : " + cellText
                                        + " cannot be found in the field: "
                                        + (indexParent + offset)
                                        + " in the actual table\n");
                    }
                }
            }
            else
            {
                resultBuilder.Append("|Expected RELATIVE text: " + parentText
                        + " is not present in the actual table \n");
            }
            return resultBuilder.ToString();
        }

        //----------------------------------------------

        private void ValidateCellValue(UITestControl element, string objectName, string expectedValue, bool stopOnFailure, Object customErrorMessage = null)
        {
            List<string> inputStringArray;
            int row = -1;
            int col = -1;
            string cellText = "";
            string result = "";
            IList<string> htmlTable = new List<string>();
            const int inputStringItems = 3;
            string paradigm = element.TechnologyName;

            inputStringArray = new List<string>(expectedValue.Split('#').ToList<string>());

            string inputStringCurrStr;
            List<string> inputStringCurrArray = new List<string>();

            for (int i = 0; i < inputStringArray.Count; i++)
            {
                inputStringCurrArray.Clear();
                inputStringCurrStr = @inputStringArray[i];

                var regexSplitter = new Regex(@"(?<!\\),");
                inputStringCurrArray.AddRange(regexSplitter.Split(inputStringCurrStr));

                List<string> tempInputTable = new List<string>();

                foreach (string inputVal in inputStringCurrArray)
                {
                    string formattedValue = inputVal.Replace("\\", "");
                    tempInputTable.Add(formattedValue);
                }

                if (tempInputTable.Count < inputStringItems)
                {
                    result += "Object : " + objectName + "'s verification data not provided correctly.  \n";
                }
                else
                {
                    row = Int32.Parse(tempInputTable[0]);
                    col = Int32.Parse(tempInputTable[1]);
                    cellText = tempInputTable[2];

                    try
                    {
                        //htmlTable = GetAppTableRow(element, row);
                        if (paradigm.Equals("Web", StringComparison.OrdinalIgnoreCase))
                        {
                            HtmlTable actTable = (HtmlTable)element;
                            HtmlRow actRow = (HtmlRow)actTable.GetRow(row);
                            htmlTable = actRow.GetContent();
                        }
                        else
                        {
                            WinTable actTable = (WinTable)element;
                            WinRow actRow = (WinRow)actTable.GetRow(row);
                            htmlTable = actRow.GetContent();
                        }
                    }
                    catch (Exception ex)
                    {
                        result = result + "|Expected Row : " + row + " cannot be found in the actual table. \n";
                    }
                    result += CheckIfTheCellContainsTheExpectedValue(htmlTable, row, col, cellText);
                }

                if (result.Length > 0)
                {
                    reportResults(
                          stopOnFailure,
                          "Check Table: Table Cell",
                          "Check Table: Table Cell command failed. " + objectName + " : " + result,
                          LogLevel.Error,
                          "",
                          customErrorMessage
                    );
                }
                else
                {
                    reportResults(
                          "Check Table: Table Cell",
                          "Check Table: Table Cell command passed. " + objectName,
                          LogLevel.Success
                    );
                }
            }
        }

        private String CheckIfTheCellContainsTheExpectedValue(IList<string> htmlTable, int row, int col, string cellText)
        {
            StringBuilder resultBuilder = new StringBuilder();

            int verifyIndex = col; // get the sequential index of the value to be
            // verified


            String verifyValue = "";

            try
            {
                verifyValue = htmlTable[verifyIndex].Trim();

                if (!cellText.Equals(verifyValue))
                {
                    //failedOnce = true;
                    resultBuilder.Append("|Expected : " + cellText + " Actual :" + htmlTable[verifyIndex] + ".\n");

                }

            }
            catch (Exception ex)
            {
                //failedOnce = true;
                resultBuilder.Append("|Expected Column : " + verifyIndex + " cannot be found in the actual table \n");
            }

            return resultBuilder.ToString();
        }

        //---------------------------------------

        private IList<string> GetAppTableRow(IWebElement element, int row)
        {

            IList<IWebElement> rowElements;
            IList<IWebElement> colElements;

            IWebElement rowElement;

            List<string> htmlTable = new List<string>();

            rowElements = element.FindElements(By.TagName("tr"));
            rowElement = rowElements[row];

            colElements = rowElement.FindElements(By.TagName("td"));

            //for loop on Ilist rowelements
            //colElements.addAll(rowElement.FindElements(By.TagName("td")));

            int colNum = colElements.Count;

            /* locator = locator.replace("\\\"", "\""); */
            string value = "";
            for (int j = 0; j < colNum; j++)
            {
                value = colElements[j].Text;
                /* value = CodedUI.getTable(locator + "." + row + "." + j); */

                if (value != null)
                {
                    htmlTable.Add(value);
                }
                else
                {
                    htmlTable.Add("");
                }
            }

            return htmlTable;
        }

        //---------------------------------------------



        //-----------------------------------------

        /**
         * Checks if a text in an element is according to a given pattern <br/>
         * . Can be used to check the value of labels, spans, inputs, etc.(Any
         * element which is containing inner text.)<br>
         * 
         * @param locator
         *            : Logical name of the web element assigned by the automation
         *            scripter
         * @param pattern
         * <br>
         * <br>
         *            <b>(Simplified Use)</b><br>
         * <br>
         *            For the pattern following format should be used.<br>
         * <br>
         *            For a uppercase string format should be <b>S</b><br>
         *            For a lowercase string format should be <b>s</b><br>
         *            For a digit format should be <b>d</b><br>
         *            For a special character the character should be entered as it
         *            is.<br>
         * <br>
         *            Ex 1:<br>
         *            For verifying a string like : abc-123#ABC <br>
         *            the pattern should be : sss-ddd#SSS <br>
         * <br>
         * 
         *            Ex 2:<br>
         *            For verifying a date : 12/March/2013 <br>
         *            the pattern should be : dd/Ssss/dddd <br>
         * <br>
         * 
         *            <b>(Advanced Use)</b><br>
         * <br>
         *            For advanced use the pure java regex pattern can be passed for
         *            the pattern. The regex pattern should have a prefix of
         *            'regex='<br>
         * <br>
         * 
         *            Ex 1:<br>
         *            For verifying a string like : abc-123#ABC <br>
         *            An example pattern will be :
         *            regex=[a-z][a-z][a-z]-\d\d\d#[A-Z][A-Z][A-Z]
         * 
         * <br>
         * <br>
         */

        public void CheckPattern(string objectName, Object pattern, bool stopOnFailure, Object customErrorMessage = null)
        {
          
            CheckPattern(objectName, "", pattern, stopOnFailure, customErrorMessage);
        }

        public void CheckPattern(string objectName, string identifier, Object pattern, bool stopOnFailure, Object customErrorMessage = null)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoCheckPattern(locator, pattern, stopOnFailure, customErrorMessage);
        }

        private void DoCheckPattern(ObjectLocator locator, Object patternObj, bool stopOnFailure, Object customErrorMessage = null)
        {
            string pattern = CheckNullValue(patternObj, "Check Pattern", stopOnFailure);
            int counter = retry;
            string returnValue = "";

            string regex = GetRegexPattern(pattern);

            string objectId = locator.ActualLocator;
            try
            {

                //IWebElement element = FindWebElement(objectID);
                UITestControl element = FindCodedUIElement(objectId);

                while (counter > 0)
                {

                    try
                    {
                        counter--;

                        returnValue = element.GetProperty("Text").ToString().Trim();

                        Match match = Regex.Match(returnValue, @regex, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            reportResults(
                                "Check Pattern",
                                "Check Pattern command passed. " + locator.LogicalName + " : Input Pattern : " + pattern,
                                LogLevel.Success
                            );
                            break;
                        }
                        else
                        {
                            reportResults(stopOnFailure,
                              "Check Pattern",
                              "Check Pattern command failed. " + locator.LogicalName + " : Input Pattern : [" + pattern
                              + "] is different from the actual value (" + returnValue + ")",
                              LogLevel.Error,
                              "",
                              customErrorMessage
                            );
                            break;
                        }
                    }
                    catch (AssertFailedException e)
                    {
                        throw e;
                    }
                    catch (StaleElementReferenceException staleElementException)
                    {
                        element = FindCodedUIElement(objectId);
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);

                        if (!(counter > 0))
                        {
                            string objectLogicalName = locator.LogicalName;

                            reportResults(stopOnFailure,
                               "Check Pattern",
                               "Check Pattern command failed. Cannot access element (" + locator.LogicalName + ") [" + locator.ActualLocator + "] Actual Error : " + e.Message,
                               LogLevel.Error,
                               e.StackTrace,
                               customErrorMessage
                            );
                        }
                    }
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                string objectLogicalName = locator.LogicalName;

                reportResults(stopOnFailure,
                    "Check Pattern",
                    "Check Pattern command failed. Element (" + locator.LogicalName + ") [" + locator.ActualLocator + "] not present. Actual Error : " + e.Message,
                    LogLevel.Error,
                    e.StackTrace,
                    customErrorMessage
                );
            }
        }


        private string GetRegexPattern(string patternString)
        {

            string regex = "";
            string pattern = patternString;

            if (pattern.ToLower().StartsWith("regex="))
            {
                pattern = pattern.Substring(pattern.IndexOf('=') + 1, pattern.Length);
                regex = pattern;
            }
            if (pattern.ToLower().StartsWith("regexp:"))
            {
                pattern = pattern.Replace("regexp:", "");
                regex = pattern;
            }
            else
            {
                char[] patternChars = pattern.ToCharArray();
                StringBuilder regexBuilder = new StringBuilder();

                regexBuilder.Append("^");

                for (int strIndex = 0; strIndex < patternChars.Length; strIndex++)
                {

                    if (patternChars[strIndex] == 'S')
                    {
                        regexBuilder.Append("[A-Z]");
                    }
                    else if (patternChars[strIndex] == 's')
                    {
                        regexBuilder.Append("[a-z]");
                    }
                    else if (patternChars[strIndex] == 'd')
                    {
                        regexBuilder.Append("\\d");
                    }
                    else
                    {
                        regexBuilder.Append(patternChars[strIndex]);
                    }
                }
                regexBuilder.Append("$");
                regex = regexBuilder.ToString();
            }
            return regex;
        }


        //----------------------------------------------------------



        public void SelectFrame(string objectName)
        {
            SelectFrame(objectName, "");
        }

        public void SelectFrame(string objectName, string identifier)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoSelectFrame(locator);
        }

        private void DoSelectFrame(ObjectLocator locator)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            string objectIdValue = objectId.ToLower(CultureInfo.CurrentCulture).Trim();
            int counter = retry;
            int frameIndex = -1;
            UITestControl element = FindCodedUIElement(objectId);
            string paradigm = element.TechnologyName;

            while (counter > 0)
            {
                try
                {
                    counter--;


                    if (paradigm.Equals("Web", StringComparison.OrdinalIgnoreCase))
                    {
                        HtmlIFrame frame = new HtmlIFrame(qWin);
                        if (objectIdValue.StartsWith("index="))
                        {
                            frameIndex = int.Parse(objectId.Substring(objectId.IndexOf("=") + 1, objectId.Length).Trim());
                            frame.SetProperty(HtmlIFrame.PropertyNames.TagInstance, frameIndex);
                            frame.SetFocus();
                        }
                        else if ("parent".Equals(objectIdValue, StringComparison.OrdinalIgnoreCase) || ("null".Equals(objectIdValue, StringComparison.OrdinalIgnoreCase)))
                        {
                            //frame.
                        }
                        else
                        {
                            //check for new windows method should implement
                            
                        }

                    }


                    /*
                    ITargetLocator target;
                    if (objectIdValue.StartsWith("index="))
                    {
                        //Should use extensions class substring
                        frameIndex = int.Parse(objectId.Substring(objectId.IndexOf("=") + 1, objectId.Length).Trim());

                        target = driver.SwitchTo();
                        target.DefaultContent();
                        target.Frame(frameIndex);
                    }
                    else if ("parent".Equals(objectIdValue, StringComparison.OrdinalIgnoreCase) || ("null".Equals(objectIdValue, StringComparison.OrdinalIgnoreCase)))
                    {
                        target = driver.SwitchTo();
                        target.DefaultContent();
                    }
                    else
                    {
                        //check for new windows method should implement
                        IWebElement element = FindWebElement(objectId);
                        target = driver.SwitchTo();
                        target.Frame(element);

                    }*/
                    reportResults("Select Frame", "Select Frame command passed. Object " + locator.LogicalName, LogLevel.Success);
                    break;
                }
                catch (Exception e)
                {
                    sleep(retryInterval);
                    if (!(counter > 0))
                    {
                        log.Error(e);
                        if (e.GetType().Equals(typeof(TimeoutException)))
                        {
                            reportResults(true, "Select Frame", "Select Frame command failed. Object " + locator.LogicalName + " cannot be found in the webpage. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
                        }
                        else
                        {
                            reportResults(true, "Select Frame", "Select Frame command cannot access. Object  " + locator.LogicalName + ". Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
                        }
                    }
                }
            }
        }




        public void ClickAt(string objectName, string coordinateString)
        {
            ClickAt(objectName, "", coordinateString);
        }

        public void ClickAt(string objectName, string identifier, string coordinateString)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoclickAt(locator, coordinateString);
        }

        private void DoclickAt(ObjectLocator locator, string coordinateString)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;
            int xOffset = 0;
            int yOffset = 0;
            try
            {
                //IWebElement element = FindWebElement(objectId);
                UITestControl element = FindCodedUIElement(objectId);
                try
                {
                    xOffset = Convert.ToInt32(coordinateString.Split(new char[] { ',' })[0]);
                    yOffset = Convert.ToInt32(coordinateString.Split(new char[] { ',' })[1]);
                }
                catch (Exception e)
                {
                    reportResults(true, "Click At", "Click At command failed. Object " + locator.LogicalName + ". Actual Error : Invalid coordinate string provided for the command ", LogLevel.Error, e.StackTrace);
                }
                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        Mouse.Click(element, new Point(xOffset, yOffset));
                        //Actions clickAt = new Actions(driver);
                        //clickAt.MoveToElement(element, xOffset, yOffset).Click();
                        //clickAt.Build().Perform();
                        reportResults("Click At", "Click At command passed. Object " + locator.LogicalName, LogLevel.Success);
                        break;
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {
                            log.Error(e);
                            reportResults(true, "Click At", "Click At command failed. Object " + locator.LogicalName + ". Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
                        }
                    }
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.Error(e);
                reportResults(true, "Click At", "Click At command failed. Object " + locator.LogicalName + " cannot be found in the webpage. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
            }
        }


       
         
        //EOF
        public void DoubleClickAt(string objectName, string coordinateString)
        {
            DoubleClickAt(objectName, "", coordinateString);
        }

        public void DoubleClickAt(string objectName, string identifier, string coordinateString)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoDoubleClickAt(locator, coordinateString);
        }

        private void DoDoubleClickAt(ObjectLocator locator, string coordinateString)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;
            int xOffset = 0;
            int yOffset = 0;
            try
            {
                //IWebElement element = FindWebElement(objectId);
                UITestControl element = FindCodedUIElement(objectId);
                try
                {
                    xOffset = Convert.ToInt32(coordinateString.Split(new char[] { ',' })[0]);
                    yOffset = Convert.ToInt32(coordinateString.Split(new char[] { ',' })[1]);
                }
                catch (Exception e)
                {
                    reportResults(true, "Double Click At", "Double Click At command failed. Object " + locator.LogicalName + ". Actual Error : Invalid coordinate string provided for the command ", LogLevel.Error, e.StackTrace);
                }
                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        Mouse.DoubleClick(element, new Point(xOffset, yOffset));
                        //Actions doubleClickAt = new Actions(driver);
                        //doubleClickAt.MoveToElement(element, xOffset, yOffset).DoubleClick();
                        //doubleClickAt.Build().Perform();
                        reportResults("Double Click At", "Double Click At command passed. Object " + locator.LogicalName, LogLevel.Success);
                        break;
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {
                            log.Error(e);
                            reportResults(true, "Double Click At", "Double Click At command failed. Object " + locator.LogicalName + ". Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
                        }
                    }
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.Error(e);
                reportResults(true, "Double Click At", "Double Click At command failed. Object " + locator.LogicalName + " cannot be found in the webpage. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
            }
        }
        
        public void GoBack(Object waitTimeObj)
        {
            string waitTime = CheckNullValue(waitTimeObj, "Go Back", true);
            //IWebDriver driver = Driver;
            try
            {
                if (qWin != null)
                {
                    qWin.Back();
                    sleep(Convert.ToInt32(waitTime));
                    reportResults("Go Back", "Go Back command passed. ", LogLevel.Success);
                }
            }
            catch (Exception e)
            {
                reportResults(true, "Go Back", "Go Back command faied. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
            }
        }

        public void Select(string objectName, Object objValue)
        {
            Select(objectName, "", objValue);
        }

        public void Select(string objectName, string identifier, Object objValue)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoSelect(locator, objValue);
        }

        private void DoSelect(ObjectLocator locator, Object objValue)
        {

            string value = CheckNullObject(objValue, "SELECT");
            int counter = retry;
            string objectId = locator.ActualLocator;

            try
            {
                UITestControl element = FindCodedUIElement(objectId);
                
                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        string paradigm = element.TechnologyName;
                        if (paradigm.Equals("Web", StringComparison.OrdinalIgnoreCase)) 
                        {
                            HtmlComboBox selectElement = (HtmlComboBox)element;
                            selectElement.SelectedItem = value;
                        }
                        else
                        {
                            WinComboBox selectElement = (WinComboBox)element;
                            selectElement.SelectedItem = value;
                        }


                        reportResults("Select", "Select command passed. Input value : "+value, LogLevel.Success);
                        break;
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {
                            reportResults(true, "Select", "Select command cannot access. Object " + locator.LogicalName + " . Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
                        }
                    }

                }
                
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                reportResults(true, "Select", "Select command failed. Object " + locator.LogicalName + " cannot be found in the webpage. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
            }
        }
        private List<String> ResolveSelectOptions(SelectElement selectElement, string value)
        {
            List<String> resolvedOptionsList = new List<String>(); ;

            IList<IWebElement> actualElementOptions = selectElement.Options;
            StringBuilder errorString = new StringBuilder();
            String[] actualOptions = new String[actualElementOptions.Count];

            for (int i = 0; i < actualElementOptions.Count; i++)
            {
                actualOptions[i] = actualElementOptions.ElementAt(i).Text;
            }
            string[] expectedValues = value.Split(new char[] { '#' });

            foreach (string expectedValue in expectedValues)
            {
                if (expectedValue.StartsWith("regex"))
                {
                    bool isMatched = false;
                    foreach (string actualOption in actualOptions)
                    {
                        if (expectedValue.StartsWith("regex="))
                        {
                            isMatched = IsMatchingPattern(expectedValue.Substring("=", expectedValue.Length), actualOption);
                        }
                        else
                        {
                            isMatched = IsMatchingPattern(expectedValue.Substring(":", expectedValue.Length), actualOption);
                        }

                        if (isMatched)
                        {
                            resolvedOptionsList.Add(actualOption);
                            break;
                        }
                    }
                    if (!isMatched)
                    {
                        errorString.Append("Cannot find the option : " + expectedValue).Append("\n");
                    }

                }
                else if (expectedValue.StartsWith("index="))
                {
                    string index = "";
                    try
                    {
                        index = expectedValue.Replace("index=", "");
                        resolvedOptionsList.Add(actualOptions[Convert.ToInt32(index)]);

                    }
                    catch
                    {
                        errorString.Append("Cannot find the option index : " + index).Append("\n");
                    }

                }
                else
                {
                    if (actualOptions.Contains(expectedValue))
                    {
                        resolvedOptionsList.Add(expectedValue);
                    }
                    else
                    {
                        errorString.Append("Cannot find the option : " + expectedValue).Append("\n");
                    }

                }

            }

            if (errorString.Length > 0)
            {
                reportResults(true, "Select", "Select command failed. Actual error : Cannot find the option values among actual options : " + string.Join(",", actualOptions)+" Actual error : "+errorString , LogLevel.Error, "");
            }

            return resolvedOptionsList;
        }

        private void SelectOptionFromActualElement(SelectElement selectElement, List<String> selectOptionList)
        {
            foreach (string option in selectOptionList)
            {
                selectElement.SelectByText(option);
            }
        }

        private bool IsMatchingPattern(string patternString, string matcherString)
        {

            Regex pattern = new Regex(patternString);
            return pattern.IsMatch(matcherString);

        }
        public void Pause(Object waitingTimeObj)
        {
            string waitingTime = CheckNullValue(waitingTimeObj, "Pause", true);
            try
            {
                int waitingMilliSeconds = Convert.ToInt32(waitingTime);
                sleep(waitingMilliSeconds);
                reportResults("Pause", "Pause command passed. ", LogLevel.Success);
            }
            catch (Exception e)
            {
                reportResults(true, "Pause", "Pause command failed. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
            }
        }
        public void Fail(Object message)
        {

            reportResults(true, "Fail", "Fail command failed. Actual Error : " + message, LogLevel.Error, "");
            failure(message.ToString());

        }
        public void NavigateToURL(string url, Object waitTime)
        {
            NavigateToURL(url, "", waitTime);
        }
        public void NavigateToURL(string url, string identifier, Object waitTime)
        {
            string actualURL = objectMapParser.getResolvedObjectSearchPath(url, identifier);
            ObjectLocator locator = new ObjectLocator(url, identifier, actualURL);
            DoNavigateToURL(locator, waitTime);
        }
        public void DoNavigateToURL(ObjectLocator locator, Object waitTimeObj)
        {
            string waitTime = CheckNullValue(waitTimeObj, "Navigate To URL", true);
            string url = "";
            //IWebDriver driver = Driver;
            try
            {
                url = locator.ActualLocator;
                //command start time
                if (url.ToLower(CultureInfo.CurrentCulture).StartsWith("openwindow="))
                {
                    String actualUrl = url.Substring("=", url.Length);
                    BrowserWindow.Launch(new System.Uri(actualUrl));

                    /*
                    IList<String> oldWindowHandles = driver.WindowHandles;
                    String actualUrl = url.Substring("=", url.Length);
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("window.open('" + actualUrl + "', '_newWindow');");
                    sleep(Convert.ToInt32(waitTime));
                    IList<String> newWindowHandles = driver.WindowHandles;
                    IList<String> newWindows = newWindowHandles.Except(oldWindowHandles).ToList();
                    driver.SwitchTo().Window(newWindows[0]);*/
                }
                else
                {
                   qWin.NavigateToUrl(new System.Uri(url));
                    /*
                    INavigation navigation = driver.Navigate();
                    navigation.GoToUrl(url);*/
                }
                reportResults("Navigate To URL", "Navigate To URL command passed. ", LogLevel.Success);

            }
            catch (Exception e)
            {
                reportResults(true, "Navigate To URL", "Navigate To URL command failed. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
            }

        }
        //EOF


        //public void SelectWindow(string objectName)
        //{
        //    SelectWindow(objectName, "");
        //}

        //public void SelectWindow(string objectName, string identifier)
        //{
        //    String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
        //    ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
        //    DoSelectWindow(locator);
        //}

        //private void DoSelectWindow(ObjectLocator locator)
        //{
        //    IWebDriver driver = Driver;
        //    string windowId = locator.ActualLocator;
        //    int counter = retry;

        //    while (counter > 0)
        //    {
        //        try
        //        {
        //            counter--;
        //            String windowHandle = GetMatchingWindowFromCurrentWindowHandles(driver, windowId);
        //            if (windowHandle != null)
        //            {
        //                driver.SwitchTo().Window(windowHandle);
        //                driver.Manage().Window.Maximize();
        //                reportResults("Select Window", "Select window command passed. Object " + locator.LogicalName, LogLevel.Success);
        //                break;
        //            }
        //            else
        //            {
        //                reportResults(true, "Select Window", "Select window command faied. Object " + locator.LogicalName + ". Actual Error : Cannot find the window with locator " + windowId, LogLevel.Error, "");
        //            }
        //        }
        //        catch (AssertFailedException e)
        //        {
        //            throw e;
        //        }
        //        catch (Exception e)
        //        {
        //            sleep(retryInterval);
        //            if (!(counter > 0))
        //            {
        //                log.Error(e);
        //                reportResults(true, "Select Window", "Select window command faied. Object " + locator.LogicalName + ". Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
        //            }
        //        }
        //    }
        //}

        //private string GetMatchingWindowFromCurrentWindowHandles(IWebDriver driver, string inputWindowName)
        //{
        //    ReadOnlyCollection<string> currentWinHandles = driver.WindowHandles;
        //    if (inputWindowName.ToLower(CultureInfo.CurrentCulture).StartsWith("index="))
        //    {
        //        return currentWinHandles.ElementAt(Convert.ToInt32(inputWindowName.Replace("index=", "")));
        //    }
        //    else
        //    {
        //        foreach (String windowname in currentWinHandles)
        //        {
        //            if (inputWindowName.ToLower(CultureInfo.CurrentCulture).StartsWith("regexp:")
        //                    || inputWindowName.StartsWith("glob:"))
        //            {

        //                if (Regex.IsMatch(inputWindowName.Substring(":", inputWindowName.Length),
        //                    driver.SwitchTo().Window(windowname).Title))
        //                {
        //                    return windowname;
        //                }

        //            }
        //            else if (driver.SwitchTo().Window(windowname).Title.Equals(inputWindowName))
        //            {
        //                return windowname;
        //            }

        //        }
        //    }
        //    return null;

        //}


          public void FireEvent(string keyFlow, Object waitTimeObj)
        {
            string waitTime = CheckNullValue(waitTimeObj, "Fire Event", true);
            bool testPassed = false;
            try
            {
                if (keyFlow.StartsWith("KEY%"))
                {
                    FireKeyEvent(keyFlow.Replace("KEY%", ""));
                    testPassed = true;
                }
                else if (keyFlow.StartsWith("MOUSE%"))
                {
                    FireMouseEvent(keyFlow.Replace("MOUSE%", ""));
                    testPassed = true;
                }
                else if (keyFlow.StartsWith("VERIFY%"))
                {
                    FireEventVerifyValue(keyFlow.Replace("VERIFY%", ""));
                }
                else
                {
                    reportResults(true, "Fire Event", "Invalid command is passed : " + keyFlow, LogLevel.Error, "");
                }

                if (testPassed)
                {
                    reportResults("Fire Event", "Fire Event command pass : " + keyFlow + " :", LogLevel.Success);
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (InvalidOperationException e)
            {
                reportResults(true, "Fire Event", "Fire event cannot perform the actions specified : " + keyFlow + " Actual error : " + e.Message, LogLevel.Error, e.StackTrace);
            }
            catch (Exception e)
            {
                reportResults(true, "Fire Event", "Fire event cannot perform the actions specified : " + keyFlow + " Actual error : " + e.Message, LogLevel.Error, e.StackTrace);
            }
        }

        private void FireEventVerifyValue(string value)
        {

            InputSimulator.SimulateModifiedKeyStroke(new[] { VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C }, null);
            String clipBoardText = Clipboard.GetText(TextDataFormat.Text);
            if (clipBoardText.Equals(value))
            {
                reportResults("Fire Event", "Fire event verify action passed. Input value : " + value, LogLevel.Success);
            }
            else
            {
                reportResults(true, "Fire Event", "Fire event verify value action failed. Actual value " + clipBoardText + " does not match the expected " + value + " . ", LogLevel.Error, "");
            }
        }

        private void FireMouseEvent(string actionEvents)
        {
            char[] chararr = new[] { '|' };
            String[] commandSet = actionEvents.Split(chararr);

            foreach (String fullCommand in commandSet)
            {
                String command = fullCommand.Substring(0, "=").ToLower(CultureInfo.CurrentCulture);
                String input = fullCommand.Substring("=", fullCommand.Length);
                if ("move".Equals(command))
                {
                    char[] splitarr = new[] { ',' };
                    int startingIndex = 0;
                    String[] coords = input.Split(splitarr);
                    int resolutionWidth = Convert.ToInt32(coords[startingIndex]);
                    int resolutionHeight = Convert.ToInt32(coords[startingIndex + 1]);
                    int x = Convert.ToInt32(coords[startingIndex + 2]);
                    int y = Convert.ToInt32(coords[startingIndex + 3]);

                    InputSimulator.SimulateKeyPress(VirtualKeyCode.F11);

                    Cursor.Position = new Point(x, y);

                    InputSimulator.SimulateKeyPress(VirtualKeyCode.F11);

                }
                else if ("wait".Equals(command))
                {
                    Thread.Sleep(Convert.ToInt32(input));
                }
                else
                {
                    throw new InvalidOperationException("Cannot perform action " + command);
                }
            }
        }


        private void FireKeyEvent(String actionEvents)
        {
            char[] chararr = new[] { '|' };
            String[] commandSet = actionEvents.Split(chararr);

            foreach (String fullCommand in commandSet)
            {
                String command = fullCommand.Substring(0, "=").ToLower(CultureInfo.CurrentCulture);
                String input = fullCommand.Substring("=", fullCommand.Length);
                if ("type".Equals(command))
                {
                    InputSimulator.SimulateTextEntry(input);
                }
                else if ("key".Equals(command))
                {
                    List<VirtualKeyCode> list = CreateCombinationKeys(input);
                    InputSimulator.SimulateModifiedKeyStroke(list, null);
                }
                else if ("wait".Equals(command))
                {
                    Thread.Sleep(Convert.ToInt32(input));
                }
                else
                {
                    throw new InvalidOperationException("Cannot perform action " + command);
                }

            }

        }

        private List<VirtualKeyCode> CreateCombinationKeys(String keyCombination)
        {
            KeyCodes codes = new KeyCodes();
            char[] chararr = new[] { '+' };
            String[] keys = keyCombination.Split(chararr);

            List<VirtualKeyCode> keyCodeList = new List<VirtualKeyCode>();

            foreach (String key in keys)
            {
                keyCodeList.AddRange(codes.GetKey(key));
            }
            return keyCodeList;
        }

        public void HandlePopup(string actionFlow, Object waitTimeObj)
        {
            string waitTime = CheckNullValue(waitTimeObj, "Handle Popup", true);
            try
            {
                int wait = Convert.ToInt32(waitTime);
                HandlePopupTimerTask task = new HandlePopupTimerTask(this, actionFlow, wait);
                task.TimerStart();
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (FormatException e)
            {
                reportResults(true, "Handle Popup", "Handle popup command failed. Actual error : Invalid wait time " + waitTime + " passed for the command.", LogLevel.Error, e.StackTrace);
            }
            catch (OverflowException e)
            {
                reportResults(true, "Handle Popup", "Handle popup command failed. Actual error : Invalid wait time " + waitTime + " passed for the command.", LogLevel.Error, e.StackTrace);
            }
            catch (Exception e)
            {
                reportResults(true, "Handle Popup", "Handle popup command failed. Actual error : " + e.Message, LogLevel.Error, e.StackTrace);
            }
        }

        private StringBuilder HandleJavaScriptPopups(string actions)
        {
            StringBuilder verificationErrorBuilder = new StringBuilder();
            bool isPopupHandled = false;
            String[] commands = actions.Split(new char[] { '|' });
            foreach (String command in commands)
            {
                String commandString = command.ToLower(CultureInfo.CurrentCulture);
                if (commandString.StartsWith("type="))
                {
                    String text =
                            command.Substring("=", command.Length);
                    Driver.SwitchTo().Alert().SendKeys(text);

                }
                else if (commandString.StartsWith("verify="))
                {
                    string actualAlertText = Driver.SwitchTo().Alert().Text;
                    string expectedText =
                            command.Substring("=", command.Length);
                    if (!expectedText.Equals(actualAlertText))
                    {
                        verificationErrorBuilder
                                .Append("Verify alert text failed. Actual : " + ""
                                        + actualAlertText + " Expected : "
                                        + expectedText + " ");
                    }
                }
                else if (commandString.StartsWith("action="))
                {
                    String action =
                             command.Substring("=", command.Length);
                    if ("ok".Equals(action, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Driver.SwitchTo().Alert().Accept();
                        isPopupHandled = true;
                    }
                    else if ("cancel".Equals(action, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Driver.SwitchTo().Alert().Dismiss();
                        isPopupHandled = true;
                    }
                }
                else
                {
                    verificationErrorBuilder
                            .Append("Given input command ("
                                    + command
                                    + ")is not recognized. Supported commands are type, verify, action.");
                }
            }

            if (!isPopupHandled)
            {
                Driver.SwitchTo().Alert().Accept();
            }
            return verificationErrorBuilder;
        }


        private class HandlePopupTimerTask
        {
            static System.Timers.Timer timer;
            CodedUITestBase caller;
            string actions;
            public HandlePopupTimerTask(CodedUITestBase caller, string actionFlow, int timeInterval)
            {
                this.caller = caller;
                this.actions = actionFlow;
                timer = new System.Timers.Timer();
                timer.Interval = timeInterval;
                timer.AutoReset = false;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);

            }

            public void TimerStart()
            {
                timer.Start();
            }

            public void TimerStop()
            {
                timer.Stop();
            }

            private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
            {
                timer.Stop();
                if (actions.StartsWith("FORCE%"))
                {
                    caller.FireKeyEvent(actions.Replace("FORCE%", ""));
                }
                else
                {
                    StringBuilder errorMessages = caller.HandleJavaScriptPopups(actions);
                    if (errorMessages.Length > 0)
                    {
                        caller.reportResults(true, "Handle Popup", "Handle popup command failed. Actual error : " + errorMessages.ToString(), LogLevel.Error, "");
                    }
                    else
                    {
                        caller.reportResults("Handle Popup", "Handle popup command passed. Actions : " + actions, LogLevel.Success);
                    }
                }
            }
        }

        //------------------------------------------------------------


        //------------------------------------
        public void MouseOver(string objectName)
        {
            MouseOver(objectName, "");
        }

        public void MouseOver(string objectName, string identifier)
        {

            string actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoMouseOver(locator);
        }

        public void DoMouseOver(ObjectLocator locator)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;
            try
            {

                //IWebElement element = FindWebElement(objectId);
                UITestControl element = FindCodedUIElement(objectId);

                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        Mouse.Hover(element);
                        //Actions builder = new Actions(driver);
                        //// call for CodedUI web driver command
                        //builder.MoveToElement(element).Build().Perform();
                        // if not exception is called consider and report the result
                        // as passed
                        reportResults(
                             "Mouse Over",
                             "Mouse Over command passed. " + locator.LogicalName,
                             LogLevel.Success
                        );
                        // if the testcase passed move out from the loop
                        break;
                    }
                    /*  catch (AssertFailedException e)
                      {
                          throw e;
                      }*/
                    catch (StaleElementReferenceException staleElementException)
                    {
                        element = FindCodedUIElement(objectId);
                        log.Warn(staleElementException);
                    }

                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {
                            reportResults(
                             true,
                             "Mouse Over",
                             "Mouse Over command failed. Cannot access element : " + locator.LogicalName + " [" + locator.ActualLocator + "]. Actual Error: " + e.Message,
                             LogLevel.Error,
                             e.StackTrace
                             );
                        }
                    }
                }
            }
            /*catch (AssertFailedException e)
            {
                throw e;
            }*/
            catch (Exception e)
            {
                reportResults(
                          true,
                          "Mouse Over",
                          "Mouse Over command failed. Element : " + locator.LogicalName + " [" + locator.ActualLocator + "] not present. Actual Error: " + e.Message,
                          LogLevel.Error,
                          e.StackTrace
                );

            }
        }
        //----------------------------------------

        public void Store(string key, string type, Object objValue)
        {
            string value = CheckNullObject(objValue, "STORE");

            XmlManager xmlWorker = new XmlManager();


            try
            {
                xmlWorker.XmlWrite(key, type, value);

                reportResults(
                       "Store Value",
                       "Store Value command passed.", 
                       LogLevel.Success
                        );
            }
            catch (Exception e)
            {
                reportResults(
                         true,
                         "Store Value",
                         "Store Value command failed. Cannot access project_data.xml. Actual Error: " + e.Message,
                         LogLevel.Error,
                         e.StackTrace);
            }

            CheckStoreValueType(type, value);

        }
      


        private void CheckStoreValueType(string type, string value)
        {
            try
            {
                if ("Int".Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    Int32.Parse(value);
                }
                else if ("Boolean".Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    if ("true".Equals(value, StringComparison.OrdinalIgnoreCase) || "false".Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        Boolean.Parse(value);
                    }
                    else
                    {
                        throw new ArgumentException("Cannot convert to boolean value " + value);
                    }
                }
                else if ("Float".Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    float.Parse(value);
                }
            }
            catch (FormatException e)
            {
                throw new ArgumentException(e.Message, e);
            }
        }


        public string Retrieve(string key, string type)
        {
            XmlManager xmlWorker = new XmlManager();

            try
            {
                string value = xmlWorker.XmlRead(key, type);
                reportResults(
                       "Store Value",
                       "Store Value command passed.",
                       LogLevel.Success
                       );
                return value;
            }
            catch (FileNotFoundException fileNotFoundE)
            {
                reportResults(
                       true,
                       "Store Value",
                       "Store Value command failed. Cannot access project_data.xml. Actual Error: " + fileNotFoundE.Message,
                       LogLevel.Error,
                       fileNotFoundE.StackTrace);
            }
            return null;
        }
      
      
        public string RetrieveString(string key)
        {
            return Retrieve(key, "String");
        }

        //PASS
        public bool RetrieveBoolean(string key)
        {
            string value = Retrieve(key, "Boolean");

            if ("true".Equals(value, StringComparison.OrdinalIgnoreCase) || "false".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return Boolean.Parse(value);
            }
            else
            {
                reportResults(
                       true,
                       "Retrieve Boolean",
                       "Retrieve Boolean command failed. Cannot parse value to boolean.",
                       LogLevel.Error,
                       "");
                return false;
            }
        }
        //PASS
        public double RetrieveDouble(string key)
        {
            string value = Retrieve(key, "Double");
            try
            {
                if (value != null)
                {
                    return Double.Parse(value);
                }
            }
            catch (FormatException e)
            {
                reportResults(
                     true,
                     "Retrieve Double",
                     "Retrieve Double command failed. Actual Error : " + e.Message,
                     LogLevel.Error,
                     e.StackTrace);
            }
            return -1;
        }

        //PASS
        public float RetrieveFloat(string key)
        {
            string value = Retrieve(key, "Float");
            try
            {
                if (value != null)
                {
                    return float.Parse(value);
                }
            }
            catch (FormatException e)
            {
                reportResults(
                       true,
                       "Retrieve Float",
                       "Retrieve Float command failed. Actual Error : " + e.Message,
                       LogLevel.Error,
                       e.StackTrace);
            }
            return -1;
        }

        //PASS
        public int RetrieveInt(string key)
        {
            string value = Retrieve(key, "Int");
            try
            {
                if (value != null)
                {
                    return Int32.Parse(value);
                }
            }
            catch (FormatException e)
            {
                reportResults(
                       true,
                       "Retrieve Int",
                       "Retrieve Int command failed. Actual Error : " + e.Message,
                       LogLevel.Error,
                       e.StackTrace);
            }
            return -1;
        }
        
        //EOF
        public void DoubleClick(string objectName)
        {
            DoubleClick(objectName, "");
        }

        public void DoubleClick(string objectName, string identifier)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoDoubleClick(locator);
        }

        // Anzar
        private void DoDoubleClick(ObjectLocator locator)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;
            try
            {
                //IWebElement element = FindWebElement(objectId);
                UITestControl element = FindCodedUIElement(objectId);

                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        //element.Click();
                        Mouse.DoubleClick(element);
                        //Actions doubleClick = new Actions(driver);
                        //doubleClick.MoveToElement(element).DoubleClick();
                        //doubleClick.Build().Perform();
                        reportResults("Double Click", "Double Click command passed. Object " + locator.LogicalName, LogLevel.Success);
                        break;
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {
                            log.Error(e);
                            reportResults(true, "Double Click", "Double Click command failed. Object " + locator.LogicalName + ". Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
                        }
                    }
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.Error(e);
                reportResults(true, "Double Click", "Double Click command failed. Object " + locator.LogicalName + " cannot be found in the webpage. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
            }
        }
        public void RightClick(string objectName)
        {
            RightClick(objectName, "");
        }

        public void RightClick(string objectName, string identifier)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoRightClick(locator);
        }

        private void DoRightClick(ObjectLocator locator)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;
            try
            {
                //IWebElement element = FindWebElement(objectId);
                UITestControl element = FindCodedUIElement(objectId);

                while (counter > 0)
                {
                    try
                    {
                        counter--;
                        Mouse.Click(element, MouseButtons.Right);
                        //Actions rightClick = new Actions(driver);
                        //rightClick.MoveToElement(element).ContextClick(element);
                        //rightClick.Build().Perform();
                        reportResults("Right Click", "Right Click command passed. Object " + locator.LogicalName, LogLevel.Success);
                        break;
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {
                            log.Error(e);
                            reportResults(true, "Right Click", "Right Click command failed. Object " + locator.LogicalName + ". Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
                        }
                    }
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.Error(e);
                reportResults(true, "Right Click", "Right Click command failed. Object " + locator.LogicalName + " cannot be found in the webpage. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
            }
        }

        /*---------------------------------------
        ****************************************    
        ---------------------------------------*/
        public string GetStringProperty(string objectName, string component)
        {
            return GetStringProperty(objectName, "", component);
        }


        public string GetStringProperty(string objectName, string identifier, string component)
        {
            string actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            return DoGetStringProperty(locator, component);
        }

        private String DoGetStringProperty(ObjectLocator locator, string component)
        {
            String returnValue = "";
            // retrieve the actual object ID from object repository
            String objectID = locator.ActualLocator;

            try
            {
                // Checking whether the element is present
                UITestControl element = FindCodedUIElement(locator.ActualLocator);

                returnValue = GetVarPropertyAttributeValue(locator, element, component);

            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("Attribute",StringComparison.OrdinalIgnoreCase))
                {
                    reportResults(
                         true,
                         "Set Variable Property",
                         "Set Variable Property command failed. Atrribute (" + component + ") of  [" + locator.LogicalName
                         + "] not present. " + "Actual Error : " + e.Message,
                         LogLevel.Error,
                         e.StackTrace);
                }
                else //if (e.Message.StartsWith("Element"))
                {
                    reportResults(
                        true,
                        "Set Variable Property",
                        "Set Variable Property command failed. Element : " + locator.LogicalName
                        + " not present. " + "Actual Error : " + e.Message,
                        LogLevel.Error,
                        e.StackTrace);
                }
            }
            return returnValue;
        }

        

        private string GetVarPropertyAttributeValue(ObjectLocator locator, UITestControl webElement, string component)
        {
            int counter = retry;
            UITestControl element = webElement;
            string returnValue = null;
           
            while (counter > 0) {
                try {
                    counter--;

                    returnValue = ValidateObjectProperty(element,component);

                    reportResults(
                              "Set Variable Property",
                              "Set Variable Property command passed. " + locator.LogicalName + "." + component
                              + "] ",
                              LogLevel.Success);
                    break;
                } 
                catch (StaleElementReferenceException StaleElementException) 
                {
                    element = FindCodedUIElement(locator.ActualLocator);
                    log.Warn(StaleElementException);
                } 
                catch (Exception e) 
                {
                    sleep(retryInterval);
                    /*
                     * after the retry amout, if still the object is not found,
                     * report the failure error will be based on the exception
                     * message, if e contains attribute report attribute failure
                     * else if e contains element, report object not found
                     */
                    if (!(counter > 0)) 
                    {
						throw e;
                    }
                }
            }

            return returnValue;
        }

        public double GetDoubleProperty(string objectName, string component) 
        {
            return GetDoubleProperty(objectName, "", component);
        }

         public double GetDoubleProperty(string objectName, string identifier, string component) 
         {
            string actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            return DoGetDoubleProperty(locator, component);
        }

          private double DoGetDoubleProperty(ObjectLocator locator, string component) {
            string returnValue = "";
            // retrieve the actual object ID from object repository
            
            try {
                // Checking whether the element is present
                UITestControl element = FindCodedUIElement(locator.ActualLocator);

                returnValue = GetVarPropertyAttributeValue(locator, element, component);
            } 
            //put test terminated
            catch (AssertFailedException e)
            {
                throw e;
            }
            /*catch (Exception e) 
            {
                if (e.Message.StartsWith("Attribute")) 
                {
                    reportResults(
                     true,
                     "Set Variable Property",
                     "Set Variable Property command failed. " + locator.LogicalName + "." + component
                     + "] not present. ",
                     LogLevel.Error,
                     e.StackTrace);

                } else if (e.Message.StartsWith("Element")) 
                {

                    reportResults(
                         true,
                         "Set Variable Property",
                         "Set Variable Property command failed. " + locator.LogicalName + "." + component
                         + "] not present. ",
                         LogLevel.Error,
                         e.StackTrace);
                }
            }*/

            double returnval = -1;
            
            try {
                returnval = Double.Parse(returnValue);
            } catch (Exception e) 
            {
                reportResults(
                      true,
                      "Set Variable Property",
                      "Set Variable Property command failed. " + locator.LogicalName + "." + component
                      + ". Input value mismatch with double. User Input : " + returnValue,
                      LogLevel.Error,
                      e.StackTrace);
            }
            return returnval;
        }

        public bool GetBooleanProperty(string objectName, string component) 
        {
                return GetBooleanProperty(objectName, "", component);
        }

        public bool GetBooleanProperty(string objectName, string identifier, string component) 
        {
            string actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            return DoGetBooleanProperty(locator, component);
        }
    

        private bool DoGetBooleanProperty(ObjectLocator locator, string component) 
        {
            string returnValue = "";
            // retrieve the actual object ID from object repository
            string objectId = locator.ActualLocator;

            try {
                // Checking whether the element is present
                UITestControl element = FindCodedUIElement(objectId);
                returnValue = GetVarPropertyAttributeValue(locator, element, component);

            } catch (Exception e) {
                /*
                 * after the retry amout, if still the object is not found, report
                 * the failure error will be based on the exception message, if e
                 * contains attribute report attribute failure else if e contains
                 * element, report object not found
                 */

                if (e.Message.StartsWith("Attribute")) 
                {

                    reportResults(
                        true,
                        "Set Variable Property",
                        "Set Variable Property command failed. Attribute : " + locator.LogicalName + "." + component
                        + " not present. Actual Error : " + e.Message,
                        LogLevel.Error,
                        e.StackTrace);

                } else if (e.Message.StartsWith("Element")) {

                    reportResults(
                        true,
                        "Set Variable Property",
                        "Set Variable Property command failed. Element : " + locator.LogicalName + "." + locator.LogicalName
                        + " not present. Actual Error : " + e.Message,
                        LogLevel.Error,
                        e.StackTrace);
                }
            }

            if ("true".Equals(returnValue,StringComparison.OrdinalIgnoreCase)  ||  "false".Equals(returnValue,StringComparison.OrdinalIgnoreCase)) {

                reportResults(
                    "Set Variable Property",
                    "Set Variable Property command passed. " + locator + " Object Value : " + returnValue, 
                    LogLevel.Success);

            } else {
                reportResults(
                    true,
                    "Set Variable Property",
                    "Set Variable Property command failed. " + locator.LogicalName
                    + " Input value mismatch with boolean. User Input : " + returnValue,
                    LogLevel.Error,
                    "");
            }
            return Boolean.Parse(returnValue);
        }

      


        public int GetIntegerProperty(string objectName, string component) 
        {
            return GetIntegerProperty(objectName, "", component);
        }

        public int GetIntegerProperty(string objectName, string identifier, string component)
        {
            string actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            return DoGetIntegerProperty(locator, component);
        }

        private int DoGetIntegerProperty(ObjectLocator locator, string component) 
        {
            string returnValue = "";
            // retrieve the actual object ID from object repository
            string objectID = locator.ActualLocator;

            try
            {
                UITestControl element = FindCodedUIElement(objectID);
                returnValue = GetVarPropertyAttributeValue(locator, element, component);
            }
            catch (Exception e)
            {
                /*
                 * after the retry amout, if still the object is not found, report
                 * the failure error will be based on the exception message, if e
                 * contains attribute report attribute failure else if e contains
                 * element, report object not found
                 */
                if (e.Message.StartsWith("Attribute", StringComparison.OrdinalIgnoreCase))
                {
                    reportResults(
                          true,
                          "Set Variable Property",
                          "Set Variable Property command failed. Attribute : " + component + " not present."
                          + " Actual Error : " + e.Message,
                          LogLevel.Error,
                          e.StackTrace);
                }
                else if (e.Message.StartsWith("Element", StringComparison.OrdinalIgnoreCase))
                {
                    reportResults(
                          true,
                          "Set Variable Property",
                          "Set Variable Property command failed. Element : " + locator.LogicalName + " not present."
                          + " Actual Error : " + e.Message,
                          LogLevel.Error,
                          e.StackTrace);
                }
            }
            int returnval = 0;
            try 
            {
                returnval = Int32.Parse(returnValue);
            } catch (Exception e) 
            {
                    reportResults(
                          true,
                          "Set Variable Property",
                          "Set Variable Property command failed. " + locator.LogicalName
                          + " Input value mismatch with int. User Input : " + returnValue,
                          LogLevel.Error,
                          e.StackTrace);
            }

            return returnval;
        }



        public void WriteToReport(Object objMessage) 
        {
            string message = CheckNullObject(objMessage, "WRITE TO REPORT");

            
            XmlManager xmlWorker = new XmlManager();

            try
            {
                xmlWorker.XmlReport(objMessage);

                reportResults(
                        "Write to Report",
                        "Write to Report command passed.",
                        LogLevel.Success
                        );
            }
            catch (Exception e)
            {
                reportResults(
                        true,
                        "Write to Report",
                        "Write to Report command failed. report.xml is not accessible.",
                        LogLevel.Error,
                        e.StackTrace);

            }
        }

        public int GetObjectCount(string objectName)
        {
            return GetObjectCount(objectName, "");
        }

        public int GetObjectCount(string objectName, string identifier)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            return DoGetObjectCount(locator);
        }

        // Anzar
        private int DoGetObjectCount(ObjectLocator locator)
        {
            string objectId = locator.ActualLocator;
            int count = 0;
            try
            {
                UITestControl element = FindCodedUIElement(objectId);
                UITestControlCollection allMatchingUiContolCollection = element.FindMatchingControls();
                count = allMatchingUiContolCollection.Count;
                System.Diagnostics.Debug.WriteLine("GetObjectCount Result: " + count);
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.Error(e);
                reportResults(true, "Get Object Count", "Get object count command failed. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
            }

            return count;

        }


        public void CheckTextPresent(string objectName, bool stopOnFailure, Object customErrorMessage = null)
        {
            CheckTextPresent(objectName, "", stopOnFailure, customErrorMessage);
        }

        public void CheckTextPresent(string objectName, string identifier, bool stopOnFailure, Object customErrorMessage = null)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoCheckTextPresent(locator, stopOnFailure, customErrorMessage);
        }

        private void DoCheckTextPresent(ObjectLocator locator, bool stopOnFailure, Object customErrorMessage = null)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;
            bool isTextFound = false;

            if (String.IsNullOrEmpty(objectId))
            {
                objectId = locator.LogicalName;
            }

            while (counter > 0)
            {
                try
                {
                    counter--;
                    UITestControlCollection controlCollection = qWin.CurrentDocumentWindow.GetChildren();
                    string pageSource = (string)controlCollection[0].GetProperty("OuterHtml");
                    //string pageSource = driver.PageSource;
                    if (pageSource.Contains(objectId))
                    {
                        reportResults("Check Text Present", "Check text present command passed. Text : " + objectId + ".", LogLevel.Success);
                        isTextFound = true;
                        break;
                    }
                    
                }
                catch (Exception e)
                {
                    sleep(retryInterval);
                    if (!(counter > 0))
                    {
                        log.Error(e);
                        reportResults(stopOnFailure, "Check Text Present", "Check text present command failed. Text : " + objectId + " was not found in the webpage. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace, customErrorMessage);
                    }
                }
            }

            if(!isTextFound)
            {
                reportResults(stopOnFailure, "Check Text Present", "Check text present command failed. Text : " + objectId + " was not found in the webpage.", LogLevel.Error, "", customErrorMessage);
            }

        }

        //public void CreateUser(string instanceName, string browser, string serverConfig)
        //{
        //    if (WebdriverInstances.ContainsKey(instanceName))
        //    {
        //        reportResults(true, "Create User", "Create user command failed. Webdriver instance with the same name " + instanceName + " already exists.", LogLevel.Error, "");
        //    }
        //    else 
        //    {
        //        try
        //        {
        //            IWebDriver driver = configWebDriverInstance(browser);
        //            WebdriverInstances.Add(instanceName, driver);
        //            manageBrowser(driver);
        //            Driver = driver;
        //            reportResults("Create User", "Create user command passed. Instance name : " + instanceName + ".", LogLevel.Success);
        //        }
        //        catch (InvalidOperationException e)
        //        {
        //            reportResults(true, "Create User", "Create user command failed. Unrecognized browser name " + browser + ". Actual error : " + e.Message, LogLevel.Error, e.StackTrace);
        //        }
        //        catch (Exception e)
        //        {
        //            reportResults(true, "Create User", "Create user command failed. Cannot create new CodedUI instance. Actual error : " + e.Message, LogLevel.Error, e.StackTrace);
        //        }
        //    }
        //}

        //public void SwitchUser(string instanceName)
        //{
        //    if (WebdriverInstances.ContainsKey(instanceName))
        //    {
        //        try
        //        {
        //            IWebDriver driver = WebdriverInstances[instanceName];
        //            Driver = driver;
        //            reportResults("Switch User", "Switch user command passed. Instance name : " + instanceName + ".", LogLevel.Success);
        //        }
        //        catch (KeyNotFoundException e)
        //        {
        //            reportResults(true, "Switch User", "Switch user command failed. Cannot find instance " + instanceName + ". Actual error : " + e.Message, LogLevel.Error, e.StackTrace);
        //        }
        //        catch (Exception e)
        //        {
        //            reportResults(true, "Switch User", "Switch user command failed. Cannot switch to CodedUI instance " + instanceName + ". Actual error : " + e.Message, LogLevel.Error, e.StackTrace);
        //        }
        //    }
        //    else
        //    {
        //        reportResults(true, "Switch User", "Switch user command failed. Cannot find instance " + instanceName + ".", LogLevel.Error, "");
               
        //    }
        //}


        public void KeyPress(string objectName, string value)
        {
            KeyPress(objectName, "", value);
        }

        public void KeyPress(string objectName, string identifier, string value)
        {
            String actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoKeyPress(locator, value);
        }

        private void DoKeyPress(ObjectLocator locator, string value)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            int counter = retry;
            try
            {
             if (value.Contains('|'))
                {
                    reportResults(true,
                        "Key Press",
                        "Key Press command failed. Input value cannot contain a pipeline symbol '|'. Object " + locator.LogicalName + ". Input value : " + value,
                        LogLevel.Error,
                        "");
                }
                char[] delemeters = { '|' };
                String[] valueStringsArr = value.Split(delemeters);
                //IWebElement element = FindWebElement(objectId);
                UITestControl element = FindCodedUIElement(objectId);

                while (counter > 0)
                {
                    try
                    {
                        counter--;

                        //Keyboard.SendKeys(element, "");

                        //element.SendKeys("");
                        //Actions focusAction = new Actions(driver);
                        //focusAction.MoveToElement(element).Build().Perform();

                        element.SetFocus();

                        for (int i = 0; i < valueStringsArr.Length; i++)
                        {
                            sleep(500);
                            List<VirtualKeyCode> list = CreateCombinationKeys(valueStringsArr[i]);
                            InputSimulator.SimulateModifiedKeyStroke(list, null);
                        }

                        reportResults("Key Press", "Key press command passed. Object " + locator.LogicalName + ". Input value : " + value, LogLevel.Success);
                        break;
                    }
                    catch (Exception e)
                    {
                        sleep(retryInterval);
                        if (!(counter > 0))
                        {
                            log.Error(e);
                            reportResults(true, "Key Press", "Key press command failed. Object " + locator.LogicalName + ". Input value : " + value + ". Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
                        }
                    }
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                log.Error(e);
                reportResults(true, "Key Press", "Key press command failed. Object " + locator.LogicalName + " cannot be found in the webpage. Actual Error :  " + e.Message, LogLevel.Error, e.StackTrace);
            }
        }

        public void CheckWindowProperty(string objectName, string propertyname, Object objExpectedvale, bool stopOnFailure, Object customErrorMessage = null)
        {
            CheckWindowProperty(objectName, "", propertyname, objExpectedvale, stopOnFailure, customErrorMessage);
        }

        public void CheckWindowProperty(string objectName, string identifier, string propertyname, Object objExpectedvale, bool stopOnFailure, Object customErrorMessage = null)
        {
            string actualLocator = objectMapParser.getResolvedObjectSearchPath(objectName, identifier);
            ObjectLocator locator = new ObjectLocator(objectName, identifier, actualLocator);
            DoCheckWindowProperty(locator, propertyname, objExpectedvale, stopOnFailure, customErrorMessage);
        }

        private void DoCheckWindowProperty(ObjectLocator locator, string propertyname, Object expectedValueObj, bool stopOnFailure, Object customErrorMessage = null)
        {
            string expectedValue = CheckNullValue(expectedValueObj, "Check Window Property", stopOnFailure);
            if (propertyname.Equals(WindowValidationType.WINDOWPRESENT.ToString()))
            {
                CheckWindowPresent(locator, propertyname, expectedValue, stopOnFailure, customErrorMessage);
            }
        }

        private void CheckWindowPresent(ObjectLocator locator, string propertyname, object objExpectedValue, bool stopOnFailure, Object customErrorMessage)
        {
            //IWebDriver driver = Driver;
            string objectId = locator.ActualLocator;
            UITestControl element = FindCodedUIElement(objectId);
            int counter = retry;
            //while (counter > 0)
            //{
                try
                {
                    counter--;
                    WinWindow windowControl = element as WinWindow;
                    if (windowControl.WaitForControlExist(2000)) // 2 seconds
                    {
                        reportResults("Check Window Property", "Check window property : WindowPresent command passed"
                                                        + locator.LogicalName + ". Window " + objExpectedValue, LogLevel.Success);
                    }
                    else
                    {
                        reportResults(
                                stopOnFailure, "Check Window Property",
                                "Check window property : WindowPresent command failed. Window object " + locator.LogicalName + " with locator "
                                        + objectId + " does not match the expected condition " + objExpectedValue,
                                LogLevel.VerificationError, "", customErrorMessage);
                    }
                    /*
                    string currentWindowHandle = driver.CurrentWindowHandle;
                    string targetWindow = GetMatchingWindowFromCurrentWindowHandles(driver, objectId);
                    driver.SwitchTo().Window(currentWindowHandle);
                    string targetWindowPresentString = (!String.IsNullOrEmpty(targetWindow)).ToString();
                    if (objExpectedValue.ToString().Equals(targetWindowPresentString, StringComparison.OrdinalIgnoreCase))
                    {
                        reportResults("Check Window Property", "Check window property : WindowPresent command passed"
                                + locator.LogicalName + ". Window " + objExpectedValue, LogLevel.Success);

                        break;
                    }
                    else
                    {
                        reportResults(
                                stopOnFailure, "Check Window Property",
                                "Check window property : WindowPresent command failed. Window object " + locator.LogicalName + " with locator "
                                        + objectId + " does not match the expected condition " + objExpectedValue,
                                LogLevel.VerificationError, "", customErrorMessage);

                        break;
                    }*/

                }
                catch (AssertFailedException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    sleep(retryInterval);
                    if (!(counter > 0))
                    {
                        log.Error(e);
                        reportResults(
                                stopOnFailure, "Check Window Property",
                                "Check window property : WindowPresent command failed. Window object " + locator.LogicalName + " with locator "
                                        + objectId + " cannot be accessed. Actual Error : " + e.Message,
                                LogLevel.VerificationError, e.StackTrace, customErrorMessage);
                    }
                }
           // }

        }



        /*---------------------------------------

        ---------------------------------------*/
        /*
        private string GenerateCustomError(Object[] customError) 
        {
            string customErrorMessage = "";
            
            if (customError != null && customError.Length > 0) 
            {
                for (int i = 0; i < customError.Length; i++) 
                {
                    customErrorMessage = customErrorMessage + customError[i].ToString() + ". ";
                }
            }
            return customErrorMessage;
        }*/

        
        public void CreateDBConnection(string databaseType, string instanceName, string url, string username, string password) 
        {
            dict = new Dictionary<string, Object>();
            string completeURL = "";
            DbConnection con = null;

            bool isNewInstance;
            
            if (dict.ContainsKey(instanceName))
            {
                isNewInstance = false;
            }
            else
            {
                isNewInstance = true;
            }

           
                       
            if (isNewInstance)
            {
                try
                {
                    if (databaseType.IndexOf("mysql", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        completeURL = @url + "userid=" + username + ";password=" + password + ";";
                        con = new MySqlConnection(completeURL);
                        con.Open(); 
                        dict.Add(instanceName, con);
                    }
                    if (databaseType.IndexOf("oracle", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        //create object and add to dict
                    }
                    if (databaseType.IndexOf("mssql", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        //create object and add to dict
                    }
                    else if (databaseType.Length == 0)
                    {
                         reportResults(
                               true,
                               "Create DB Connection",
                               "Create DB Connection command failed. Database type not selected.",
                               LogLevel.Error,
                               "");
                    }
                    reportResults(
                           "Create DB Connection",
                           "Create DB Connection command passed. Instance name : " + instanceName + ". DB URL : " + completeURL,
                           LogLevel.Success);
                }
                catch (AssertFailedException e)
                {
                    throw e;
                }
                catch (Exception e) 
                {
                     reportResults(
                                   true,
                                   "Create DB Connection",
                                   "Create DB Connection command failed. Actual error : " + e.Message,
                                   LogLevel.Error,
                                   e.StackTrace);
                }
            }
        }


        private List<Object> GetDBTable(string instanceName, string query) 
        {
            DbConnection con;
            List<Object> arrList = new List<Object>();
            try
            {
                if (dict.ContainsKey(instanceName))
                {
                    con = (DbConnection)dict[instanceName];
                }
                else
                {
                    throw new Exception("Connection instance unavaliable : " + instanceName);
                }

                DbCommand cmd = con.CreateCommand();
                cmd.CommandText = query;
                
                DbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        arrList.Add(reader[i]);
                    }

                }

                if (arrList.Count == 0)
                {
                    throw new NullReferenceException("Empty Result set for the query : " + query);
                }
                
            }
            catch (Exception e)
            {
                reportResults(
                       true,
                       "Get DB Table",
                       "Get DB Table command failed. Actual error : " + e.Message,
                       LogLevel.Error,
                       e.StackTrace);
            }
            
            return arrList;
            
        }


        public string GetStringDBResult(string instanceName, string query) 
        {
            List<Object> arrList;
            string value = null;
            try {
                arrList = GetDBTable(instanceName, query);
                value = arrList[0].ToString();
                
                if (arrList.Count >= 2) 
                {
                    reportResults(
                          true,
                          "Set DB Results",
                          "Set DB Results command failed. For Query : " + query + ". Actual result contains more than one value. Actual Values : " + String.Join(",", arrList)
                          + ". Return Value : " + value,
                          LogLevel.Error,
                          "");
                }
                reportResults(
                   "Set DB Results",
                   "Set DB Results command passed. For Query : " + query,
                   LogLevel.Success);

            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (NullReferenceException e) 
            {
                string errorString = e.Message;

                reportResults(
                        true,
                        "Set DB Results",
                        "Set DB Results command failed. Actual Error : " + e.Message,
                        LogLevel.Error,
                        e.StackTrace);

            } catch (Exception e) {
                
                if (e.Message.StartsWith("Connection instance unavaliable")) 
                {
                    reportResults(
                           true,
                           "Set DB Results",
                           "Set DB Results command failed. Connection : " + instanceName + " Actual error : " + e.Message,
                           LogLevel.Error,
                           e.StackTrace);
                } 
                else 
                {
                    reportResults(
                           true,
                           "Set DB Results",
                           "Set DB Results command failed. Actual error : " + e.Message,
                           LogLevel.Error,
                           e.StackTrace);
                }
            }
            return value;
        }



        public int GetIntDBResult(string instanceName, string query)
        {
            List<Object> arrList;
            int value = 0;
            try
            {
                arrList = GetDBTable(instanceName, query);
                
                if (!(arrList[0] is Int32))
                {
                    throw new FormatException("The value you are trying to retrive ("+ arrList[0].ToString()+ ") is not stored as an interger in the database.");
                }

                value = Int32.Parse(arrList[0].ToString());
              
                reportResults(
                    "Set DB Results",
                    "Set DB Results command passed. For Query : " + query,
                    LogLevel.Success);
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (NullReferenceException e)
            {
                string errorString = e.Message;

                reportResults(
                        true,
                        "Set DB Results",
                        "Set DB Results command failed. Actual Error : " + e.Message,
                        LogLevel.Error,
                        e.StackTrace);

            }
            catch (Exception e)
            {

                if (e.Message.StartsWith("Connection instance unavaliable"))
                {
                    reportResults(
                           true,
                           "Set DB Results",
                           "Set DB Results command failed. Connection : " + instanceName + " is not created. Actual error : " + e.Message,
                           LogLevel.Error,
                           e.StackTrace);
                }
                else
                {
                    reportResults(
                           true,
                           "Set DB Results",
                           "Set DB Results command failed. Actual error : " + e.Message,
                           LogLevel.Error,
                           e.StackTrace);
                }
            }
            return value;
        }

        public bool GetBooleanDBResult(string instanceName, string query)
        {
            List<Object> arrList;
            bool value = false;
            try
            {
                arrList = GetDBTable(instanceName, query);

                if (!(arrList[0] is bool))
                {
                    throw new FormatException("The value you are trying to retrive (" + arrList[0].ToString() + ") is not stored as a boolean in the database.");
                }

                value = Boolean.Parse(arrList[0].ToString());

                reportResults(
                    "Set DB Results",
                    "Set DB Results command passed. For Query : " + query,
                    LogLevel.Success);


            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (NullReferenceException e)
            {
                string errorString = e.Message;

                reportResults(
                        true,
                        "Set DB Results",
                        "Set DB Results command failed. Actual Error : " + e.Message,
                        LogLevel.Error,
                        e.StackTrace);

            }
            catch (Exception e)
            {

                if (e.Message.StartsWith("Connection instance unavaliable"))
                {
                    reportResults(
                           true,
                           "Set DB Results",
                           "Set DB Results command failed. Connection : " + instanceName + " is not created. Actual error : " + e.Message,
                           LogLevel.Error,
                           e.StackTrace);
                }
                else
                {
                    reportResults(
                           true,
                           "Set DB Results",
                           "Set DB Results command failed. Actual error : " + e.Message,
                           LogLevel.Error,
                           e.StackTrace);
                }
            }
            return value;
        }

        public double GetDoubleDBResult(string instanceName, string query)
        {
            List<Object> arrList;
            double value = 0;
            try
            {
                arrList = GetDBTable(instanceName, query);

                if (!(arrList[0] is double))
                {
                    throw new FormatException("The value you are trying to retrive (" + arrList[0].ToString() + ") is not stored as an interger in the database.");
                }

                value = Double.Parse(arrList[0].ToString());

                reportResults(
                    "Set DB Results",
                    "Set DB Results command passed. For Query : " + query,
                    LogLevel.Success);


            }
            catch (AssertFailedException e)
            {
                throw e;
            }

            catch (NullReferenceException e)
            {
                string errorString = e.Message;

                reportResults(
                        true,
                        "Set DB Results",
                        "Set DB Results command failed. Actual Error : " + e.Message,
                        LogLevel.Error,
                        e.StackTrace);

            }
            catch (Exception e)
            {

                if (e.Message.StartsWith("Connection instance unavaliable"))
                {
                    reportResults(
                           true,
                           "Set DB Results",
                           "Set DB Results command failed. Connection : " + instanceName + " is not created. Actual error : " + e.Message,
                           LogLevel.Error,
                           e.StackTrace);
                }
                else
                {
                    reportResults(
                           true,
                           "Set DB Results",
                           "Set DB Results command failed. Actual error : " + e.Message,
                           LogLevel.Error,
                           e.StackTrace);
                }
            }
            return value;
        }



        public void CheckDBResults(string instanceName, string query, Object expectedValueObj, bool stopOnFailure, Object customErrorMessage = null)
        {
            string expectedValue = CheckNullValue(expectedValueObj, "Check DB Results", stopOnFailure);
            List<Object> objArrList;
            List<string> inputTable = new List<string>(); 
            List<string> strArrList = new List<string>(); 
            
            try
            {
                objArrList = GetDBTable(instanceName, query);

                var regexSplitter = new Regex(@"(?<!\\),");
                inputTable.AddRange(regexSplitter.Split(@expectedValue));
                List<String> tempInputTable = new List<String>();
                foreach (string inputVal in inputTable)
                {
                    string formattedValue = inputVal.Replace("\\", "");

                    tempInputTable.Add(formattedValue);                
                }

                inputTable = tempInputTable;
                
                foreach (Object obj in objArrList)
                {
                    strArrList.Add(obj.ToString());
                }

                IEnumerable<string> differenceQuery = inputTable.Except(strArrList, StringComparer.OrdinalIgnoreCase);
                List<string> differenceList = differenceQuery.ToList<string>();


                if (differenceList.Count > 0)
                {
                    reportResults(
                        stopOnFailure,
                        "Check DB Results",
                        "Check DB Results command failed. Table data is not as expected : For Query : " + query + " :EXPECTED Value : " + expectedValue
                        + " :Actual Value : " + String.Join(",", strArrList),
                        LogLevel.Error,
                        "", 
                        customErrorMessage);
                }
                else
                {
                    reportResults(
                       "Check DB Results",
                       "Check DB Results command passed.  For Query : " + query + " :EXPECTED Value " + expectedValue,
                       LogLevel.Success
                       );
                }
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("Connection instance unavaliable"))
                {
                    reportResults(
                          stopOnFailure,
                          "Check DB Results",
                          "Check DB Results command failed. Connection : " + instanceName + " is not created. Actual : " + e.Message,
                          LogLevel.Error,
                          e.StackTrace,
                          customErrorMessage);
                }
                else
                {
                    reportResults(
                         stopOnFailure,
                         "Check DB Results",
                         "Check DB Results command failed. Actual : " + e.Message,
                         LogLevel.Error,
                         e.StackTrace,
                         customErrorMessage);
                }
            }

        }

        public void Screenshot(string imageName)
        {
            {
                try
                {
                    Screenshot ss = ((ITakesScreenshot)Driver).GetScreenshot();


                    string screenshot = ss.AsBase64EncodedString;
                    byte[] screenshotAsByteArray = ss.AsByteArray;
                    ss.SaveAsFile("somescreen.png", System.Drawing.Imaging.ImageFormat.Png);

                    reportResults(
                           "Screenshot",
                           "Screenshot command passed.",
                           LogLevel.Success
                    );
                }
                catch (AssertFailedException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    reportResults(
                        true,
                        "Screenshot",
                        "Screenshot command failed. Actual : " + e.Message,
                        LogLevel.Error,
                        e.StackTrace,
                        "");
                }
            }
        }


        //MOUSE MOVE AND CLICK
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        public static void DoMouseMoveAndClick(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
        }

        public void MouseMoveAndClick(string resolution, string coordinates, Object ObjWaitTime)
        {
            string waitTime = CheckNullObject(ObjWaitTime, "Mouse Move and Click", null);
            int screenWidth;
            int screenHeight;

            int yCoordinate;
            int xCoordinate;

            bool dClick = false;

            try
            {
                String[] resArr = resolution.Split(',');
                screenWidth = Int32.Parse(resArr[0]);
                screenHeight = Int32.Parse(resArr[1]);

                if (coordinates.Length > 6)
                {
                    if (coordinates.Substring(coordinates.Length - 6).Equals("DCLICK", StringComparison.OrdinalIgnoreCase))
                    {
                        //has double click at the end
                        coordinates = coordinates.Substring(0, (coordinates.Length - 6));
                        dClick = true;
                    }
                }

                string[] coorArr = coordinates.Split(',');
                xCoordinate = Int32.Parse(coorArr[0]);
                yCoordinate = Int32.Parse(coorArr[1]);

                //IWebDriver driver = Driver;
                //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(double.Parse(waitTime)));
                int waitingMilliSeconds = Convert.ToInt32(waitTime);
                sleep(waitingMilliSeconds);


                int deskHeight = Screen.PrimaryScreen.Bounds.Height;
                int deskWidth = Screen.PrimaryScreen.Bounds.Width;



                xCoordinate = GetCoordinatesForCurrentScreen(xCoordinate, screenWidth, deskWidth);
                yCoordinate = GetCoordinatesForCurrentScreen(yCoordinate, screenHeight, deskHeight);



                if (dClick)
                {
                    DoMouseMoveAndClick(xCoordinate, yCoordinate);
                    DoMouseMoveAndClick(xCoordinate, yCoordinate);
                }
                else
                {
                    DoMouseMoveAndClick(xCoordinate, yCoordinate);
                }

                reportResults(
                     "Mouse Move and Click",
                     "Mouse Move and Click command passed",
                     LogLevel.Success
                );

            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                reportResults(
                       true,
                       "Mouse Move and Click",
                       "Mouse Move and Click command failed. Invalid input. Actual : " + e.Message,
                       LogLevel.Error,
                       e.StackTrace,
                       "");
            }

        }

        private int GetCoordinatesForCurrentScreen(int coordinate, int oldRes, int newRes)
        {
            double dOldRes = Convert.ToDouble(oldRes);
            double dnewRes = Convert.ToDouble(newRes);
            double dCoordinate = Convert.ToDouble(coordinate);

            double dResult = (dCoordinate / oldRes) * newRes;
            int result = (int)Math.Round(dResult);
            return result;
        }
         
        
        public void CheckFileInfo(string fileName, string verificationType, bool stopOnFailure, Object customError) 
        {
                string root="";
                string thefile="";

                char current;
                int position = 0;

                //getting the file name

                for (int i = fileName.Length-1; i >= 0; i--)
                {
                     current = fileName[i-1];
                  
                     if (current == '\\')
                     {
                        
                         position = i;
                         thefile = fileName.Substring(i, fileName.Length - i);
                         break;
                     }
                }

                root = fileName.Substring(0, position);

                try
                {
                    FileAttributes attr = File.GetAttributes(fileName);

                    switch (attr)
                    {
                        case FileAttributes.Directory:
                            if (Directory.Exists(fileName))
                            {
                                
                                reportResults(
                                    stopOnFailure,
                                    "Check File Information",
                                    "Check File Information command failed. File: " + thefile + " does not exist.",
                                    LogLevel.Error,
                                    "",
                                    customError);
                            }
                            else
                            {
                                reportResults(
                                    stopOnFailure,
                                    "Check File Information",
                                    "Check File Information command failed. File: " + thefile + " does not exist.",
                                    LogLevel.Error,
                                    "",
                                    customError);
                            }
                            break;
                        default:
                            if (File.Exists(fileName))
                            {
                                
                                reportResults(
                                        "Check File Information",
                                        "Check File Information command passed.",
                                        LogLevel.Success
                                        );

                               
                            }
                            else
                            {

                                reportResults(
                                    stopOnFailure,
                                    "Check File Information",
                                    "Check File Information command failed. File: " + thefile + " does not exist.",
                                    LogLevel.Error,
                                    "",
                                    customError);

                            
                            }
                            break;
                    }
                }
                catch (AssertFailedException afe)
                {
                    throw afe;
                }
                catch (Exception e)
                {
                 
                    reportResults(
                          stopOnFailure,
                          "Check File Information",
                          "Check File Information command failed. File: " + thefile + " does not exist.",
                          LogLevel.Error,
                          "",
                          customError);
                }
        }


        public void CheckDocument(String docType, String filePath, String pageNumberRange, String verifyType, String inputString, Boolean stopOnFailure, String customErrorMessage) 
        {
            if (docType.Equals("WORD")) 
            {
                VerifyDocumentFiles(filePath, verifyType, inputString, stopOnFailure, customErrorMessage);
            }
        }

        private void VerifyDocumentFiles(string filePath, string verifyType, string inputString, bool stopOnFailure, string customErrorMessage)
        {
            if (verifyType.Equals("EXISTS")) 
            {
                checkExistsInDocument(filePath, inputString, stopOnFailure, customErrorMessage);
            }

        }

        private void checkExistsInDocument(string filePath, string inputString, bool stopOnFailure, string customErrorMessage)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document doc = new Microsoft.Office.Interop.Word.Document();

            object fileName = filePath;
            // Define an object to pass to the API for missing parameters
            object missing = System.Type.Missing;
            doc = word.Documents.Open(ref fileName,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing);

            String read = string.Empty;
            List<string> data = new List<string>();
            foreach (Microsoft.Office.Interop.Word.Range tmpRange in doc.StoryRanges)
            {
                //read += tmpRange.Text + "<br>";
                data.Add(tmpRange.Text);
            }
            ((Microsoft.Office.Interop.Word._Document)doc).Close();
            ((Microsoft.Office.Interop.Word._Application)word).Quit();
            Boolean isFound = false;
            foreach (String line in data) 
            {
                if (line.Contains(inputString)) 
                {
                    isFound = true;
                    break;
                }
            }

            if (!isFound)
            {
                reportResults(
                         stopOnFailure,
                         "Check Document",
                         "Check Doument Information command failed. File: " + filePath
                         + " does not contain the expected values.",
                         LogLevel.Error,
                         "",
                         customErrorMessage);
            }
            else 
            {
                reportResults("Check Document",
                                        "Check Document Information command passed.",
                                        LogLevel.Success
                                        );
            }

        }

        


    }
}
