using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;

namespace XMLFormatter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine("XMLFormatter. Reads in a file and converts it to human redable, indented format.");
                Console.WriteLine("Usage: XMLFormatter [/i] filename");
                Console.WriteLine("    One parameter is assumed to be a filename. Output is to the Console");
                Console.WriteLine("    If a /i is used, the file is read in and converted in place.");
                Console.WriteLine("Any XML errors will abort and the error message returned.");
                return;
            }
            bool inPlace = false;
            string inFile = "";
            if (args.Length == 1)
            {
                if (args[0].ToLower() == "/i")
                {
                    Console.WriteLine("Seriously? Specifiy a filename");
                    return;
                }
                else
                {
                    inFile = args[0];
                }
            }
            else
            {
                if (args[0].ToLower() == "/i")
                {
                    inFile = args[1];
                    inPlace = true;
                }
                else if (args[1].ToLower() == "/i")
                {
                    inFile = args[0];
                    inPlace = true;
                }
                else
                {
                    Console.WriteLine("/i and/or a filename. Nothing else.");
                    return;
                }
            }
            XmlDocument doc = new XmlDocument();
            FileInfo fi = null;
            if (TryGetFileName(inFile, out fi))
            {
                doc = new XmlDocument();
                try
                {
                    string xml = File.ReadAllText(fi.FullName);
                    //                    doc.Load(fi.FullName);
                    doc.LoadXml(xml);
                }
                catch (Exception e)
                {
                    Console.WriteLine("XML Error: {0}", e.Message);
                    return;
                }
            }
            StringBuilder builder = new StringBuilder();

            try
            {
                using (XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder)))
                {
                    writer.Formatting = Formatting.Indented;
                    doc.Save(writer);
                }
                if (inPlace)
                {
                    File.WriteAllText(fi.FullName, builder.ToString());
                }
                else
                {
                    Console.WriteLine("{0}", builder.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during write: {0}", e.Message);
            }
        }
        static bool TryGetFileName(string fileName, out FileInfo fi)
        {
            fi = null;
            try
            {
                fi = new FileInfo(fileName);
                if (!fi.Exists)
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                return false;
            }
            return true;
        }
    }
}
