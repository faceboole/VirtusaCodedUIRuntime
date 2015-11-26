using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodedUI.Virtusa.Report.Exporter;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodedUI.Virtusa.Report.Reporter
{
    public class Reporter : IReporter
    {

        private ReportBuilder builder;


        public Reporter()
        {
            builder = new ReportBuilder(initReportDirectory());
        }


        private String initReportDirectory()
        {

            DateTime date = DateTime.Now;
            String timestamp = date.ToString("yyyy_MM_dd_HH_mm_sstt");
            String rootFolder = AppDomain.CurrentDomain.BaseDirectory + "/../../../ExecutionReports";
            String reportFolder = rootFolder + "/ExecutionReport" + timestamp;
            try
            {
                if (!File.Exists(reportFolder))
                {
                    Directory.CreateDirectory(reportFolder);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

           copyReportHelperFiles(rootFolder, reportFolder);

            return reportFolder;
        }


        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }


        private void copyReportHelperFiles(String rootFolder, String reportFolderStr)
        {
            String templateFileFolder = "ReportTemplate";
            Copy(templateFileFolder, reportFolderStr);
        }


        void IReporter.addNewTestExecution()
        {
            builder.addNewTestExecution();
        }

        void IReporter.addNewTestSuite(String testSuiteName)
        {
            builder.addNewTestSuite(testSuiteName, "0ms");
        }

        void IReporter.addNewTestCase(String testCaseName)
        {
            builder.addNewTestCase(testCaseName, "0ms");
        }

        void IReporter.reportStepResults(bool isPassed, String category, String message, String loglvl, String stacktrace)
        {
            string messagexml = escpaeXmlCharacters(message);
            string stackxml = escpaeXmlCharacters(stacktrace);
            if (isPassed)
            {
                builder.addNewTestStep(isPassed, category, messagexml, loglvl);
            }
            else
            {

                String screenShot = saveScreenShot(builder.getReportFolderLocation());//"Link";//
                String thumbScreenShot = saveScreenshotThumb(screenShot); //"Link";//
                builder.addNewTestStep(
                        isPassed,
                        category,
                        "images/" + screenShot,
                        thumbScreenShot,
                        messagexml, stackxml, "Error");
            }

        }

        private String saveScreenshotThumb(String screenShotFile)
        {
            String screenShotThumb = "images/" + screenShotFile + "_Thumb.png";
            try
            {
                String screenShotOriginalFile = builder.getReportFolderLocation() + "/images/" + screenShotFile;
                Image image = Image.FromFile(screenShotOriginalFile, true);

                Bitmap thumb = new Bitmap(image, new Size(150, 100));
                thumb.Save(screenShotOriginalFile + "_Thumb.png", ImageFormat.Png);
                thumb.Dispose();
                GC.Collect();
                //BufferedImage img = ImageIO.read(new File(screenShotOriginalFile));
                //BufferedImage thumb = Scalr.resize(img, Method.SPEED, 150, 100, Scalr.OP_ANTIALIAS, Scalr.OP_BRIGHTER);

                //ImageIO.write(thumb, "png", new File(screenShotOriginalFile + "_Thumb.png"));

            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
            return screenShotThumb;
        }

        private String saveScreenShot(string reportFolderLocation)
        {
            DateTime date = DateTime.Now;
            String timestamp = date.ToString("yyyyMMddHHmmssSS");
            String screenShotFile = timestamp + ".png";
            String screenShotImgFolder = reportFolderLocation + "/images";
            try
            {
                GC.Collect();
                Bitmap img = GetScreenShot();
                if (!File.Exists(screenShotImgFolder))
                {
                    Directory.CreateDirectory(screenShotImgFolder);
                }
                img.Save(screenShotImgFolder + "/" + screenShotFile, ImageFormat.Png);
                img.Dispose();
                GC.Collect();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return screenShotFile;
        }

        private static Bitmap GetScreenShot()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }

        void IReporter.endTestReporting()
        {
            builder.setExecutionSummary();
            Generator generator = new Generator();
            generator.generateReport(builder);
        }

        private string escpaeXmlCharacters(string message)
        {
            return SecurityElement.Escape(message);
        }

    }

    
}
