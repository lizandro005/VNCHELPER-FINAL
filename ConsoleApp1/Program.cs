
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
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Lifetime;


namespace ConsoleApp1
{
    internal class Program
    {

        static void Main(string[] args)
        {
            OutputDiskCUsage();
            UptimeChecker uptimeChecker = new UptimeChecker();
            uptimeChecker.CheckUptime();
            CheckRamUsage();
            /*CheckTemperature();*/
            CheckResourceUsage();
            RunProgramAndDeleteFile("C:\\Program Files (x86)\\VirtualDJ\\virtualdj8.exe", "C:\\Users\\Elite\\Desktop\\VNCHELPER.txt");


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


        



            void CheckResourceUsage()
            {
                FileWriting fileWriting = new FileWriting();
                try
                {
                    var processes = Process.GetProcesses();
                    foreach (var process in processes)
                    {
                        if (process.Threads.Count > 0)
                        {
                            var cpuUsage = process.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount / Stopwatch.Frequency;
                            if (cpuUsage > 0.5)
                            {
                                string resourceusage = "Process: " + process.ProcessName + " is using too much CPU resources.";
                                fileWriting.WriteToFile(resourceusage);
                            }
                            else if (process.WorkingSet64 > 8000000)
                            { //8000000 bytes = 8mb
                                fileWriting.WriteToFile("Process: " + process.ProcessName + " is using too much Memory resources.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    fileWriting.WriteToFile("Error: Unable to retrieve process data." + ex.Message);
                }
            }



            /*void CheckTemperature()
            {
                FileWriting fileWriting = new FileWriting();
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string temp = obj["CurrentTemperature"].ToString();
                        string temperature = "Temperature: " + (Convert.ToInt32(temp) - 2732) + "°C";
                        fileWriting.WriteToFile(temperature);
                    }
                }
                catch (ManagementException)
                {
                    fileWriting.WriteToFile("Error: Unable to retrieve temperature data.");
                }
            } */



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
                    // Start the program
                    Process process = new Process();
                    process.StartInfo.FileName = programPath;
                    process.Start();
                    process.WaitForExit();

                    // Delete the file
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error running program or deleting file: " + ex.Message);
                }
            }
        }


    class UptimeChecker
    {
        [DllImport("kernel32.dll")]
        static extern ulong GetTickCount64();

        public void CheckUptime()
        {
            try
            {
                // Get the tick count
                ulong tickCount = GetTickCount64();
                // Convert the tick count to a TimeSpan object
                var uptime = new TimeSpan(0, 0, 0, 0, (int)tickCount);
                // Display the uptime
                Console.WriteLine("Uptime: {0} days {1} hours {2} minutes {3} seconds",
                    uptime.Days, uptime.Hours, uptime.Minutes, uptime.Seconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Unable to retrieve uptime data. " + ex.Message);
            }
        }
    }




    class FileWriting
        {
            public void WriteToFile(string textToWrite)
            {
                try
                {
                    // Open the file to write to
                    using (StreamWriter sw = File.AppendText("C:\\Users\\Elite\\Desktop\\VNCHELPER.txt"))
                    {
                        sw.WriteLine(textToWrite);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error writing to file: " + ex.Message);
                }
            }




        }
    }
  

}
