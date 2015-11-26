using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodedUI.Virtusa.Runtime;
using System.Reflection;
using log4net;
using test;

namespace CodedUI.Virtusa.Aspects
{    
    class VTAFRecoveryMethods
    {
        
        private static ILog log;
        private CodedUITestBase caller;
        private string[] recoveryMethodNames;

        public VTAFRecoveryMethods(CodedUITestBase caller, String[] recoveryMethodNames) 
        {
            log = LogManager.GetLogger(typeof(VTAFRecoveryMethods));
            this.caller = caller;
            this.recoveryMethodNames = recoveryMethodNames;
            
        }

        private string getRecoveryTypeNames(string entireName, string mode) 
        {
            string returnName = "";
            try
            {
                char[] delimiteres = { '.' };
                String[] splitted = entireName.Split(delimiteres);

                if("class".Equals(mode))
                {
                    String className = splitted[0] + "." + splitted[1];
                    returnName = className;
                }else if("method".Equals(mode))
                {
                    String methodName = splitted[2];
                    returnName = methodName;
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return returnName;
        }

        public void runScenarios()
        {
            if (recoveryMethodNames != null && recoveryMethodNames.Length > 0) 
            {
                for (int i = 0; i < recoveryMethodNames.Length; i++)
                {                  
                    
                    try
                    {
                        string recoveryMethodName = recoveryMethodNames[i];
                        
                        String className = getRecoveryTypeNames(recoveryMethodName, "class");
                        String methodName = getRecoveryTypeNames(recoveryMethodName, "method");
                        Type type = Type.GetType(className);
                        MethodInfo method = type.GetMethod(methodName);
                        method.Invoke(caller, new object[] { caller });

                    }
                    catch (TargetException e)
                    {
                        log.Error(e);
                    }
                    catch (ArgumentException e)
                    {
                        log.Error(e);
                    }
                    catch (TargetInvocationException e)
                    {
                        log.Error(e);
                    }
                    catch (TargetParameterCountException e)
                    {
                        log.Error(e);

                    }
                    catch (MethodAccessException e)
                    {
                        log.Error(e);
                    }
                    catch (InvalidOperationException e)
                    {
                        log.Error(e);
                    }
                    catch (NotSupportedException e)
                    {
                        log.Error(e);
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                    }
                   
                }
            }
        }

       


    }
}
