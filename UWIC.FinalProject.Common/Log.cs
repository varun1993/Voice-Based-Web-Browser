using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace UWIC.FinalProject.Common
{
    public class Log
    {
        public static void ErrorLog(Exception ex)
        {
            var errorMessage = ex.Source + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
            CreateFile("ErrorLog", errorMessage);
        }

/*
        private static char HexChar(int value)
        {
            value &= 0xF;
            if (value >= 0 && value <= 9) return (char)('0' + value);
            return (char)('A' + (value - 10));
        }
*/

        public static void MessageLog(string logMessage)
        {
            CreateFile("MessageLog", logMessage);
        }

        public static void ControlLog(string logMessage)
        {
            CreateFile("ControlLog", logMessage);
        }

        public static void InformationLog(string logMessage)
        {
            CreateFile("InformationLog", logMessage);
        }

        public static void CreateLog(string fileName, string logMessage)
        {
            CreateFile(fileName, logMessage);
        }

        public static void MessageLog(object obj)
        {
            string logMessage = SerializeToXml(obj);
            CreateFile("MessageLog", logMessage);
        }

        private static void CreateFile(string fileName, string logMessage)
        {
            var directoryPath = "\\Logs\\" + DateTime.Today.Date.ToString("dd-MM-yyyy") + "\\";

            try
            {
                //Get Application path
                var applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = applicationPath + directoryPath + fileName + " - " +
                               DateTime.Today.Date.ToString("MMM-dd-yyyy") + ".txt";

                if (!File.Exists(filePath))
                {
                    if (!Directory.Exists(applicationPath + directoryPath))
                    {
                        //Create Directory
                        Directory.CreateDirectory(applicationPath + directoryPath);
                    }
                    //Create Log File
                    using (var swLog = File.CreateText(filePath))
                    {
                        swLog.WriteLine("Created on - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        swLog.WriteLine("==================================");
                        swLog.WriteLine();
                    }
                }

                //Following text is added to the file untill the data is changed
                using (var sw = File.AppendText(filePath))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff ") + logMessage);
                    sw.WriteLine();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string SerializeToXml(Object obj)
        {

            MemoryStream memStream = null;

            try
            {
                var xmlDoc = new XmlDocument();
                var serializer = new XmlSerializer(obj.GetType());
                memStream = new MemoryStream();

                serializer.Serialize(memStream, obj);
                memStream.Position = 0;
                xmlDoc.Load(memStream);

                return FormatXml(xmlDoc);

            }
            finally
            {
                if (memStream != null) memStream.Close();
            }
        }

        private static string FormatXml(XmlDocument doc)
        {
            var sb = new StringBuilder();
            var settings =
                new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = @"    ",
                    NewLineChars = Environment.NewLine,
                    NewLineHandling = NewLineHandling.Replace,
                };

            using (var writer = XmlWriter.Create(sb, settings))
            {
                if (doc.ChildNodes[0] is XmlProcessingInstruction)
                {
                    doc.RemoveChild(doc.ChildNodes[0]);
                }

                doc.Save(writer);
                return sb.ToString();
            }
        }
    }
}
