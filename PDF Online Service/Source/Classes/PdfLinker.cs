using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace EucasesLinkingService.PdfManipulation.Classes
{
    public class PdfLinker
    {
        public string Link(string runFileName, string workingDirectory, string arguments)
        {
            string error = string.Empty;
            ProcessStartInfo startinfo = new ProcessStartInfo()
            {
                FileName = runFileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };


            var c = startinfo.EnvironmentVariables;
            var values = c.Values;


            using (Process process = Process.Start(startinfo))
            {
                using (StreamReader reader = process.StandardError)
                {
                    error = reader.ReadToEnd();
                }
            }

            return error;
        }
    }
}