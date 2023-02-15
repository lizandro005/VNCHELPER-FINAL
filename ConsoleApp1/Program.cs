
using System;
using System.Management;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Security.Policy;
using System.Runtime.Remoting.Lifetime;


namespace ConsoleApp1
{
    internal class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            OutputDiskCUsage();
            UptimeChecker uptimeChecker = new UptimeChecker();
            uptimeChecker.CheckUptime();
            CheckRamUsage();
            RunProgramAndDeleteFile("C:\\Program Files (x86)\\AnyDesk\\AnyDesk.exe", ".\\VNCHELPER.TXT");



            void CheckRamUsage()
            {
                FileWriting fileWriting = new FileWriting();
                try
                {
                    var performanceCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
                    var ramUsagePercentage = performanceCounter.NextValue();
                    string ramusage = "RAM Usage: " + ramUsagePercentage + "%";
                    fileWriting.WriteToFile(ramusage);
                }
                catch (Exception ex)
                {
                    fileWriting.WriteToFile("Error: Unable to retrieve RAM usage data." + ex.Message);
                }
            }


        



            void OutputDiskCUsage()
            {
                FileWriting fileWriting = new FileWriting();
                DriveInfo di = new DriveInfo("C:/");
                double totalSize = di.TotalSize / 1000000000;
                double freeSpace = di.TotalFreeSpace / 1000000000;
                double usedSpace = totalSize - freeSpace;
                string freediskspace = "\n Disk C: Usage: " + usedSpace + " GB out of " + totalSize + " GB";
                fileWriting.WriteToFile(freediskspace);

            }



            void RunProgramAndDeleteFile(string programPath, string filePath)
            {
                try
                {
                    Process process = new Process();
                    process.StartInfo.FileName = programPath;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch (Exception ex)
                {
                    FileWriting fileWriting = new FileWriting();
                    fileWriting.WriteToFile("Error running program or deleting file: " + ex.Message);
                }
            }
        }

        class UptimeChecker
        {

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern ulong GetTickCount64();



            public void CheckUptime()
            {
                try
                {
                    ulong tickCount = GetTickCount64();
                    var uptime = TimeSpan.FromMilliseconds(tickCount);
                    string uptimeString = $"Uptime: {uptime.Days} days {uptime.Hours} hours {uptime.Minutes} minutes {uptime.Seconds} seconds";
                    FileWriting fileWriting = new FileWriting();
                    fileWriting.WriteToFile(uptimeString);
                }
                catch (Exception ex)
                {
                    FileWriting fileWriting = new FileWriting();
                    fileWriting.WriteToFile("Error: Unable to retrieve uptime data. " + ex.Message);
                }
            }
        }

  




        class FileWriting
        {
            public void WriteToFile(string textToWrite)
            {
                try
                {
                    using (StreamWriter sw = File.AppendText(".\\VNCHELPER.TXT"))
                    {
                        sw.WriteLine(textToWrite);
                    }
                }
                catch (Exception ex)
                {
                    FileWriting fileWriting = new FileWriting();
                    fileWriting.WriteToFile("Error writing to file: " + ex.Message);
                }
            }




        }
    }
  

}
