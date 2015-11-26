
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
    /// Description of NonDataDrivenTestCase1.
    /// </summary>
    [CodedUITest]
    public class NonDataDrivenTestCase1 : CodedUITestBase
    {
        

        /// <summary>
        /// Contains the actions for the test case "NonDataDrivenTestCase1".
        /// The "NonDataDrivenTestCase1" method will be iterated n times where n is the number of data rows associated with the test case.
        /// </summary>        
        public void NonDataDrivenTestCase1_TestSteps()
        {
        	initTestCase();       
        	
        	Open("http://www.hotelclub.com/","3000");
        	Common.EnterSerachData(this, "Colombo, Sri lanka","12/12/15","25/12/15");
        	Click("codedUIPage.UISearchButton");
        	Click("codedUIPage.UIRamadaColomboHyperlink");
        	Click("codedUIPage.UIContinueBooking2DoubHyperlink");
        	Click("codedUIPage.UIContinuebookingButton");
        }
        
 		[TestMethod]
		
		public void NonDataDrivenTestCase1_TestMethod()
		{
			SettingsCodedUITests.StartTest();// to set the PlayBack settings
			NonDataDrivenTestCase1_TestSteps();
		}

        private void initTestCase()
        {
            configureTestReport("TestSuite1", "NonDataDrivenTestCase1");
            BusinessRecoveryScenarios = new string[] {};
            OnErrorRecoveryScenarios = new string[] {};

        }
        
        
    }
}