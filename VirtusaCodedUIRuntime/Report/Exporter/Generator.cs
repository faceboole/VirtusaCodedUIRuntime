using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodedUI.Virtusa.Report.Reporter;
using Antlr3.ST;
using System.IO;

namespace CodedUI.Virtusa.Report.Exporter
{
    class Generator
    {
        public Generator()
        {
        }

        public void generateReport(ReportBuilder reportBuilder)
        {
            // String templateFolderRoot = "";          

            String targetHtmlDataFile = reportBuilder.getReportFolderLocation();
            try
            {
                StringReader reader = getTemplateStringReader("Libraries" + Path.DirectorySeparatorChar + "Report.stg");
                Console.WriteLine(reader.ToString());
                createContent("report", reportBuilder,
                       reader, targetHtmlDataFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        private void createContent(String varName, Object objectToPass, StringReader reader, String outputFileName)
        {

            Dictionary<String, Object> map = new Dictionary<String, Object>();
            map.Add(varName, objectToPass);

            String outputContent = null;
            StringTemplateGroup group = new StringTemplateGroup(reader);
            var contentTemplate = group.GetInstanceOf("Content");
            contentTemplate.Attributes = map;
            outputContent = contentTemplate.ToString();
            //StringBuilder sb = new StringBuilder(outputContent);
            StringWriter writer = new StringWriter(new StringBuilder(outputContent));
            writer.Flush();
            StreamWriter fileWriter = null;
            try
            {
                fileWriter = new StreamWriter(outputFileName + "/report.html.data");
                fileWriter.Write(writer.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            writer.Close();
            fileWriter.Close();
        }

        private StringReader getTemplateStringReader(String filePath)
        {

            string absoluteSkinRootDirectoryName = Path.Combine(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName, filePath);
            TextReader reader = File.OpenText(absoluteSkinRootDirectoryName);
            String readString = reader.ReadToEnd();
            Console.WriteLine("Tepmlate Group Read OK!!");
            return new StringReader(readString);
        }

    }
}
