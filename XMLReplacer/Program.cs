using System;
using System.IO;
using System.Xml;

namespace XMLReplacer
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = GetParameter(args, "-source");
            var xmlPath = GetParameter(args, "-xmlPath");
            var targetFolder = GetParameter(args, "-targetFolder");

            var sourceXML = new XmlDocument();
            sourceXML.Load(path);
            var sourceNode = sourceXML.SelectSingleNode(xmlPath);
            if(sourceNode == null)
                throw new InvalidOperationException($"Could not find xml node '{xmlPath}' in file {path}");

            ReplaceTargetXmls(targetFolder, xmlPath, sourceNode.InnerXml);
        }

        private static void ReplaceTargetXmls(string targetFolder, string xmlPath, string innerXml)
        {
            var d = new DirectoryInfo(targetFolder);
            var configFiles = d.GetFiles("*.config"); //Getting config files
            
            foreach (var fileInfo in configFiles)
            {
                var targetXml = new XmlDocument();
                targetXml.Load(fileInfo.FullName);
                var sourceNode = targetXml.SelectSingleNode(xmlPath);
                if (sourceNode == null)
                    throw new InvalidOperationException($"Could not find xml node '{xmlPath}' in file {fileInfo.FullName}");

                sourceNode.InnerXml = innerXml;
                if(fileInfo.IsReadOnly)
                    File.SetAttributes(fileInfo.FullName, File.GetAttributes(fileInfo.FullName) & ~FileAttributes.ReadOnly);

                targetXml.Save(fileInfo.FullName);
            }

            Console.WriteLine($"Replaced {configFiles.Length} files");
        }

        private static string GetParameter(string[] args, string name)
        {
            var indexSource = Array.IndexOf(args, name);
            if (indexSource < 0)
            {
                throw new ArgumentNullException(name);
            }

            return args[indexSource + 1];
        }
    }
}
