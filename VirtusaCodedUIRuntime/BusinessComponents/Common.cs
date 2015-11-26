using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Virtusa.Runtime;
using System.Windows.Forms;

namespace test
{
	/// <summary>
	/// Libray Common contains the business components 
	/// which can be used in the test cases.
	/// </summary>
		
	class Common
	{
		public static void EnterSerachData(CodedUITestBase caller, string Where,string InDate,string OutDate) {
		    caller.Type("codedUIPage.UIWhereEdit",Where);
		    caller.Type("codedUIPage.UICheckinddmmyyEdit",InDate);
		    caller.Type("codedUIPage.UICheckoutddmmyyEdit",OutDate);	
		}
	}
}