
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
          //  var handle = GetConsoleWindow();
          //  ShowWindow(handle, SW_HIDE);

            OutputDiskCUsage();
      
            UptimeChecker uptimeChecker = new UptimeChecker();
            uptimeChecker.CheckUptime(); 
            CheckRamUsage();

            
            void CheckRamUsage()
{
                FileWriting fileWriting = new FileWriting();
                try
                {
                    var performanceCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
                    var ramUsagePercentage = performanceCounter.NextValue();
                    string ramUsageString = "RAM Usage: " + ramUsagePercentage + "%";
                    fileWriting.WriteToFile(ramUsageString);

                    // Check if RAM usage is high (e.g., over 90%)
                    if (ramUsagePercentage > 0)
                    {
                        // Prompt user to clean up RAM
                        Console.WriteLine("Your computer's RAM usage is high. Cleaning up RAM may help improve performance, but it may also close some programs.");
                        Console.Write("Do you want to clean up RAM now? (Y/N): ");
                        string answer = Console.ReadLine().ToLower();

                        if (answer == "s" || answer == "sim")
                        {
                            // Terminate Microsoft Edge instances
                            foreach (Process p in Process.GetProcessesByName("Microsoft.Photos.exe"))
                            {
                                p.Kill();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    fileWriting.WriteToFile("Error: Unable to retrieve RAM usage data. " + ex.Message);
                }
            }





            /*
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
            */


            void OutputDiskCUsage()
            {
                FileWriting fileWriting = new FileWriting();
                DriveInfo di = new DriveInfo("C:/");
                double totalSize = di.TotalSize / 1000000000;
                double freeSpace = di.TotalFreeSpace / 1000000000;
                double usedSpace = totalSize - freeSpace;
                string freediskspace = "\n Disk C: Usage: " + usedSpace + " GB out of " + totalSize + " GB";
                fileWriting.WriteToFile(freediskspace);

                double freeSpacePercentage = (freeSpace / totalSize) * 100;
                string teste = freeSpacePercentage.ToString();
                fileWriting.WriteToFile(teste);
                if (freeSpacePercentage < 95)
                {
                    //string tempFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp";
                    string tempFolderPath = ("C:\\Users\\goncalo.o.lizandro\\Desktop\\aiai"); // para testes <-
                    if (Directory.Exists(tempFolderPath))
                    {
                        DirectoryInfo tempDir = new DirectoryInfo(tempFolderPath);
                        foreach (FileInfo file in tempDir.GetFiles())
                        {
                            try
                            {
                                file.Delete();
                            }
                            catch (Exception ex)
                            {
                                fileWriting.WriteToFile("Nada" + ex.Message);
                            }
                        }
                        foreach (DirectoryInfo subDir in tempDir.GetDirectories())
                        {
                            try
                            {
                                subDir.Delete(true);
                            }
                            catch (Exception ex)
                            {
                                fileWriting.WriteToFile("Pastas" + ex.Message);
                            }
                        }
                    }
                }
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
            
                    // Check if uptime is more than 3 days
                    if (uptime.TotalDays > 0)
                    {
                        // Prompt user to restart the machine
                        Console.WriteLine("O seu computador está ligado à mais de 3 dias. É recomendado que reinicie o seu computador para assegurar máxima perfomance.");
                        Console.Write("Quer reiniciar o seu computador agora? (s/n): ");
                        string answer = Console.ReadLine().ToLower();
            
                        if (answer == "s" || answer == "sim")
                        {
                            // Restart the machine
                            Process.Start("shutdown", "/r /t 0");
                        }
                    }
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
  


