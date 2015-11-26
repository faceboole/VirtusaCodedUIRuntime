
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Collections;
using CodedUI.Virtusa.Runtime;
using MSTestHacks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITesting;
using CodedUI.Virtusa.Utils;

namespace test.TestSuite1
{
    /// <summary>
    /// Description of DataDrivenTestCase1.
    /// </summary>
    [CodedUITest]
    public class DataDrivenTestCase1 : CodedUITestBase
    {
        

        /// <summary>
        /// Contains the actions for the test case "DataDrivenTestCase1".
        /// The "DataDrivenTestCase1" method will be iterated n times where n is the number of data rows associated with the test case.
        /// </summary>        
        public void DataDrivenTestCase1_TestSteps(string SearchDataTable_Identifier, string SearchDataTable_FName, string SearchDataTable_LName, string SearchDataTable_PhoneNo)
        {
        	initTestCase();       
        	
        	Open("http://www.hotelclub.com/","3000");
        	Common.EnterSerachData(this, "Colombo, Sri lanka","12/12/15","25/12/15");
        	Click("codedUIPage.UISearchButton");
        	Click("codedUIPage.UIRamadaColomboHyperlink");
        	Click("codedUIPage.UIContinueBooking2DoubHyperlink");
        	Click("codedUIPage.UIContinuebookingButton");
        	Select("codedUIPage.UIMainPane",SearchDataTable_Identifier);
        	Type("codedUIPage.UIFirstgivennameEdit",SearchDataTable_FName);
        	Type("codedUIPage.UILastnamesurnameEdit",SearchDataTable_LName);
        	Type("codedUIPage.UIPhonenumberWeonlycalEdit",SearchDataTable_PhoneNo);
        	Click("codedUIPage.UIContinuebookingButton");
        }
        
 		[TestMethod]
		[DataSource("test.TestSuite1.DataDrivenTestCase1.DataDrivenTestCase1_DataProvider")]				
		public void DataDrivenTestCase1_TestMethod()
		{
			SettingsCodedUITests.StartTest();// to set the PlayBack settings
				
			Dictionary<string, object> testDataRow = this.TestContext.GetRuntimeDataSourceObject<Dictionary<string, object>>();
			
			string SearchDataTable_Identifier = Convert.ToString(testDataRow["SearchDataTable_Identifier"]);
			string SearchDataTable_FName = Convert.ToString(testDataRow["SearchDataTable_FName"]);
			string SearchDataTable_LName = Convert.ToString(testDataRow["SearchDataTable_LName"]);
			string SearchDataTable_PhoneNo = Convert.ToString(testDataRow["SearchDataTable_PhoneNo"]);
			
			DataDrivenTestCase1_TestSteps(SearchDataTable_Identifier, SearchDataTable_FName, SearchDataTable_LName, SearchDataTable_PhoneNo);
		}

        private void initTestCase()
        {
            configureTestReport("TestSuite1", "DataDrivenTestCase1");
            BusinessRecoveryScenarios = new string[] {};
            OnErrorRecoveryScenarios = new string[] {};

        }
        
        
		private IEnumerable DataDrivenTestCase1_DataProvider
		{
		     get { return GetVirtualDataTable("SearchDataTable"); }
		}
    }
}