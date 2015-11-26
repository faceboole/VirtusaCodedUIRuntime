using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspectDotNet;
using CodedUI.Virtusa.Runtime;
using System.Windows.Forms;
using System.ComponentModel;
//using System.Windows.Forms;

namespace CodedUI.Virtusa.Aspects
{
    class AsepctClass : Aspect
    {
        static System.Timers.Timer timer;
        static CodedUITestBase caller;

        //[AspectAction("%before %call *configureTestReport")]
        static public void WarmEngineAction()
        {
            Console.WriteLine("Warming the engine before starting ");
        }

        [AspectAction("%before %call *IWebElement.Click() && %withincode (CodedUI.Virtusa.Runtime.CodedUITestBase.DoClick)")]
        static public void beforeClick()
        {
            caller = CodedUITestBase.Instance;
            adviceInterceptionPoint();
        }


        [AspectAction("%after %call *IWebElement.Click() && %withincode (CodedUI.Virtusa.Runtime.CodedUITestBase.DoClick)")]
        static public void afterClick()
        {
            timer.Stop();
        }

        [AspectAction("%before %call *IWebElement.SendKeys && %withincode (CodedUI.Virtusa.Runtime.CodedUITestBase.DoType)")]
        static public void beforeType()
        {
            caller = CodedUITestBase.Instance;
            adviceInterceptionPoint();
        }


        [AspectAction("%after %call *IWebElement.SendKeys && %withincode (CodedUI.Virtusa.Runtime.CodedUITestBase.DoType)")]
        static public void afterType()
        {
            timer.Stop();
        }

        [AspectAction("%before %call *INavigation.GoToUrl && %withincode (CodedUI.Virtusa.Runtime.CodedUITestBase.DoOpen)")]
        static public void beforeOpen()
        {
            caller = CodedUITestBase.Instance;
            adviceInterceptionPoint();
        }

        [AspectAction("%after %call *INavigation.GoToUrl && %withincode (CodedUI.Virtusa.Runtime.CodedUITestBase.DoOpen)")]
        static public void afterOpen()
        {
            timer.Stop();
        }


        static private void adviceInterceptionPoint()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 300000;
            timer.AutoReset = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            timer.Start();
        }

        static private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            VTAFRecoveryMethods recoveryMethods = new VTAFRecoveryMethods(caller, caller.OnErrorRecoveryScenarios);
            recoveryMethods.runScenarios();
            
        }
    }
}
