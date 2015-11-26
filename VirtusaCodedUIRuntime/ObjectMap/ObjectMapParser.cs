using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;

namespace CodedUI.Virtusa.ObjectMap
{
    class ObjectMapParser : IGetObjectPath
    {

        public string getResolvedObjectSearchPath(string objectPath, string identifier)
        {
            string objectID = getObjectSearchPath(objectPath);

            if (!String.IsNullOrEmpty(identifier))
            {
                objectID = getResolvedIdentifier(objectID, identifier);
            }

            return objectID;
        }

        public string getResolvedIdentifier(string searchPath, string identifier)
        {
            String resolvedSearchPath = searchPath;
            if (!String.IsNullOrEmpty(identifier))
            {
                List<String> parameterValues = getParameterValues(identifier);
                string[] delimiters = new string[] { "_PARAM:" };
                for (int i = 0; i < parameterValues.Count; i++)
                {
                    resolvedSearchPath = resolvedSearchPath.Replace("<" + parameterValues[i].Split(delimiters, StringSplitOptions.None)[0] + ">", parameterValues[i].Split(delimiters, StringSplitOptions.None)[1]);
                }
            }
            return resolvedSearchPath;
        }

        private static List<String> getParameterValues(String parameters)
        {

            List<String> parameterValues = new List<String>();
            string[] delimiters = new string[] { "_PARAM," };

            String[] idefList = parameters.Split(delimiters, StringSplitOptions.None);

            for (int i = 0; i < idefList.Length; i++)
            {

                parameterValues.Add(idefList[i]);

            }
            return parameterValues;

        }

        public string getObjectSearchPath(string objectPath)
        {
            string objectID = "";
            string pageName = getResourceFileName(objectPath);

            if (pageName != null)
            {
                string objectName = getPageObjectName(objectPath);
                try
                {
                    ResXResourceSet resxSet = new ResXResourceSet(pageName);
                    objectID = resxSet.GetString(objectName);
                }
                catch (Exception e)
                {
                    //Reporter.Log(ReportLevel.Warn, "Object not found in the given file location : " + pageName + ". <br/> Actual Error : " + e.Message);
                }
            }
            return objectID;
        }

        private string getResourceFileName(string objectPath)
        {
            string resourceFileName = null;
            char[] delimiterChars = { '.' };
            try
            {
                string page = objectPath.Split(delimiterChars)[0];
                resourceFileName = @"Pages\" + page + ".resx";
            }
            catch (Exception e)
            {
               // Reporter.Log(ReportLevel.Warn, "Invalid format given for the object : " + objectPath + ". Object must be given in the format : Page.Object <br/> Actual Error : " + e.Message);
            }
            return resourceFileName;
        }

        private string getPageObjectName(string objectPath)
        {
            string objectName = null;
            char[] delimiterChars = { '.' };
            try
            {
                objectName = objectPath.Split(delimiterChars)[1];
            }
            catch (Exception e)
            {
               // Reporter.Log(ReportLevel.Warn, "Invalid format given for the object : " + objectPath + ". Object must be given in the format : Page.Object <br/> Actual Error : " + e.Message);
            }
            return objectName;
        }

    }
}
