﻿using Microsoft.PowerShell.Commands;
using Microsoft.Win32;
using System.ServiceProcess;

internal class IHM
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("[1] Disable Telemetry");
            Console.WriteLine("[2] Disable Bloat");
            Console.WriteLine("[3] Re-enable Bloat");
            Console.WriteLine("[4] Disable Windows Update Services");
            Console.WriteLine("[5] Enable Windows Update Services");
            Console.WriteLine("[6] Disable all");
            Console.WriteLine("[7] Re-Enable all");
            Console.WriteLine("");
            Console.WriteLine("Choose an option: ");
            var userInput1 = Console.ReadLine();

            if (userInput1.ToLower() == "1")
            {
                DisableSpyware();
            }
            else if (userInput1.ToLower() == "2")
            {
                DisableBloat();
            }

            else if (userInput1.ToLower() == "3")
            {
                EnableBloat();
            }

            else if (userInput1.ToLower() == "4")
            {
                DisableWindowsUpdates();
            }

            else if (userInput1.ToLower() == "5")
            {
                EnableWindowsUpdates();
            }

            else if (userInput1.ToLower() == "6")
            {
                DisableBloat();
                DisableSpyware();
                DisableWindowsUpdates();
            }

            else if (userInput1.ToLower() == "7")
            {
                EnableBloat();
                EnableWindowsUpdates();
            }

            else if (string.IsNullOrEmpty(userInput1))
            {
                return; // Exit the program
            }
            else
            {
                Console.WriteLine("Invalid option. Press enter to return to Menu.");
                Console.ReadLine();
                Console.Clear();
                continue; // Skip to the next iteration of the loop
            }

            // Handle exit or menu return
            while (true)
            {
                Console.WriteLine("Press Enter to exit, or M to return to Menu");
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    return;
                }
                else if (input.ToUpper() == "M")
                {
                    Console.Clear();
                    break; // Return to the main menu
                }
                else
                {
                    Console.WriteLine("Invalid input. Please press Enter or M.");
                }
            }
        }
    }

    static void SetRegistryValueSafe(string path, string name, object value)
    {
        try
        {
            using (var registryKey = Registry.LocalMachine.OpenSubKey(path, true))
            {
                if (registryKey != null)
                {
                    registryKey.SetValue(name, value, RegistryValueKind.DWord);
                    Console.WriteLine($"Successfully set {name} to {value} in {path}.");
                }
                else
                {
                    Console.WriteLine($"Registry path {path} not found.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to set {name} in {path}: {ex.Message}");
        }
    }
    static void DisableSpyware()
    {
        string hostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";

        Console.Clear();
        try
        {
            Console.WriteLine("Modifying hosts");
            // Create an array of entries to block telemetry and tracking
            string[] telemetryEntries = new string[]
            {
            "telemetry.microsoft.com",
            "settings-win.data.microsoft.com",
            "v10.vortex-win.data.microsoft.com",
            "diagnostics.support.microsoft.com",
            "a-0001.a-msedge.net",
            "edge.microsoft.com",
            "feedback.microsoft.com",
            "comptex.microsoft.com",
            "data.microsoft.com",
            "msftconnecttest.com",
            "azureedge.net",
            "activity.windows.com",
            "bingapis.com",
            "msedge.net",
            "assets.msn.com",
            "scorecardresearch.com",
            "data.msn.com"
            };

            // Check if the hosts file is writable
            if (!IsFileWritable(hostsFilePath))
            {
                Console.WriteLine("Error: Unable to access hosts file. Make sure you have administrative privileges.");
                return;
            }

            // Read existing lines from the hosts file
            var lines = File.ReadAllLines(hostsFilePath).ToList();
            var existingEntries = new HashSet<string>(lines
                .Where(line => !line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]));

            // Prepare to add new entries
            bool modified = false;
            foreach (string domain in telemetryEntries)
            {
                if (!existingEntries.Contains(domain))
                {
                    // Find the last comment or empty line to insert new entries before it
                    int insertIndex = lines.Count; // Default to end
                    for (int i = lines.Count - 1; i >= 0; i--)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i]) || lines[i].StartsWith("#"))
                        {
                            insertIndex = i;
                        }
                        else
                        {
                            break;
                        }
                    }
                    lines.Insert(insertIndex, $"127.0.0.1 {domain}");
                    Console.WriteLine($"Added: 127.0.0.1 {domain}");
                    modified = true;
                }
                else
                {
                    Console.WriteLine($"Already exists: {domain}");
                }
            }

            // Write back only if modified
            if (modified)
            {
                File.WriteAllLines(hostsFilePath, lines);
                Console.WriteLine("Hosts file updated successfully.");
            }
            else
            {
                Console.WriteLine("No changes made to the hosts file.");
            }

            // Disable telemetry in the registry
            string DataCollection1 = "HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection";
            string DataCollection2 = "HKLM:\\SOFTWARE\\Policies\\Microsoft\\Windows\\DataCollection";
            string DataCollection3 = "HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection";

            Console.WriteLine("Disabling Telemetry");
            SetRegistryValueSafe(DataCollection1, "AllowTelemetry", 0);
            SetRegistryValueSafe(DataCollection2, "AllowTelemetry", 0);
            SetRegistryValueSafe(DataCollection3, "AllowTelemetry", 0);

            Console.WriteLine("Disabling Location tracking");
            string SensorState = "HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Sensor\\Overrides\\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}";
            string LocationConfig = "HKLM:\\SYSTEM\\CurrentControlSet\\Services\\lfsvc\\Service\\Configuration";

            SetRegistryValueSafe(SensorState, "SensorPermissionState", 0);
            SetRegistryValueSafe(LocationConfig, "Status", 0);

            Console.WriteLine("Disabling DoSvc");
            SetRegistryValueSafe("HKLM:\\SYSTEM\\CurrentControlSet\\Services\\DoSvc", "Start", 4);
            // 4 = Disabled
            // 2 = Enabled

            Console.WriteLine("Done!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}\n\n(Is your antivirus interfering?)");
        }
    }

    // Helper function to check if a file is writable
    static bool IsFileWritable(string path)
    {
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                // If we can open the file for writing, it's writable
                return true;
            }
        }
        catch
        {
            return false; // If there's an exception, the file isn't writable
        }
    }

    static void SetRegistryValue(string keyPath, string valueName, object value)

    {
        try
        {
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (key != null)
                {
                    key.SetValue(valueName, value);
                }
                else
                {
                    Console.WriteLine($"Error: Unable to create or open registry key {keyPath}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void CreateRegistryEntry(string keyPath, string entryName, object value)
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (key != null)
                {
                    key.SetValue(entryName, value);
                }
                else
                {
                    Console.WriteLine("Error: Unable to create registry entry.");
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static bool RegistryKeyExists(string registryPath)
    {
        try
        {
            // Open the registry key in read-only mode
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath, false);

            // If the key is null, it doesn't exist
            return key != null;
        }
        catch (Exception ex)
        {
            // Handle any exceptions that may occur during registry access
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    // Helper methods to extract root key, view, and subkey from paths
    static RegistryHive GetRootKey(string path)
    {
        string rootKeyString = path.Split('\\')[0].Substring(2); // Extract root key string (e.g., "HKLM")
        return (RegistryHive)Enum.Parse(typeof(RegistryHive), rootKeyString);
    }

    static RegistryView GetView(string path)
    {
        return RegistryView.Registry64; // Use 64-bit view for compatibility
    }

    static string GetSubKey(string path)
    {
        return path.Split('\\')[1]; // Extract subkey name from path
    }

    static void DisableBloat()
    {
        Console.Clear();

        Console.WriteLine("Disabling Windows Feedback Experience");
        SetRegistryValueSafe("HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo", "Enabled", 0);

        Console.WriteLine("Making Mixed Reality Portal uninstallable");
        SetRegistryValueSafe("HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Holographic", "FirstRunSucceeded", 0);

        Console.WriteLine("Disabling live tiles");
        string liveRegistryKey = "HKCU:\\SOFTWARE\\Policies\\Microsoft\\Windows\\CurrentVersion\\PushNotifications";

        try
        {
            if (Registry.GetValue(liveRegistryKey, null, null) == null)
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\CurrentVersion\\PushNotifications"))
                {
                    // Key is created if it doesn't exist
                }
            }
            Registry.SetValue(liveRegistryKey, "NoTileApplicationNotification", 1);
            Console.WriteLine("Successfully disabled live tiles.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to disable live tiles: {ex.Message}");
        }

        Console.WriteLine("Disabling Location Tracking");
        string SensorState = "HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Sensor\\Overrides\\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}";
        string LocationConfig = "HKLM:\\SYSTEM\\CurrentControlSet\\Services\\lfsvc\\Service\\Configuration";

        SetRegistryValueSafe(SensorState, "SensorPermissionState", 0);
        SetRegistryValueSafe(LocationConfig, "LocationConfig", 0);

        Console.WriteLine("Disabling People icon in Taskbar");
        SetRegistryValueSafe("HKCU:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\People", "PeopleBand", 0);

        Console.WriteLine("Disabling start menu reccomendations");
        SetRegistryValueSafe("HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SystemPaneSuggestionsEnabled", 0);

        Console.WriteLine();

        Console.WriteLine("\nDone!");
    }

    static void EnableBloat()
    {
        Console.Clear();

        Console.WriteLine("Enabling Windows Feedback Experience");
        SetRegistryValueSafe("HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo", "Enabled", 1);

        Console.WriteLine("Preventing Mixed Reality Portal from being uninstallable");
        SetRegistryValueSafe("HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Holographic", "FirstRunSucceeded", 1);

        Console.WriteLine("Disabling live tiles");
        string liveRegistryKey = "HKCU:\\SOFTWARE\\Policies\\Microsoft\\Windows\\CurrentVersion\\PushNotifications";

        try
        {
            if (Registry.GetValue(liveRegistryKey, null, null) == null)
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\PushNotifications"));
                
                // Key is created if it doesn't exist
                
            }
            Registry.SetValue(liveRegistryKey, "NoTileApplicationNotification", 0);
            Console.WriteLine("Successfully Enabled live tiles.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to enable live tiles: {ex.Message}");
        }

        Console.WriteLine("Enabling Location Tracking");
        string SensorState = "HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Sensor\\Overrides\\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}";
        string LocationConfig = "HKLM:\\SYSTEM\\CurrentControlSet\\Services\\lfsvc\\Service\\Configuration";

        SetRegistryValueSafe(SensorState, "SensorPermissionState", 1);
        SetRegistryValueSafe(LocationConfig, "LocationConfig", 1);

        Console.WriteLine("Enabling People icon in Taskbar");
        SetRegistryValueSafe("HKCU:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\People", "PeopleBand", 1);

        Console.WriteLine("Enabling start menu reccomendations");
        SetRegistryValue("HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SystemPaneSuggestionsEnabled", 0);

        Console.WriteLine();

        Console.WriteLine("\nDone!");
    }

    static void EnableWindowsUpdates()
    {
        Console.Clear();

        // Services and Reg entries to modify
        var servicesToEnable = new[]
        {
        "UsoSvc",
        "CryptSvc",
        "WaaSMedicSvc",
        "BITS",
        "wuauserv"
    };

        // Helper method to start services
        void StartService(string serviceName)
        {
            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        Console.WriteLine($"Starting {serviceName}...");
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running);
                        Console.WriteLine($"{serviceName} started successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"{serviceName} is already running.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start {serviceName}: {ex.Message}");
            }
        }

        string registryPath = "SYSTEM\\CurrentControlSet\\Services\\";

        foreach (var service in servicesToEnable)
        {
            Console.WriteLine($"Enabling {service}");
            SetRegistryValueSafe(registryPath + service, "Start", 2); // 2 = Auto
            StartService(service);
        }
    }

    static void DisableWindowsUpdates()
    {
        Console.Clear();

        // Services and Reg entries to modify
        var servicesToDisable = new[]
        {
        "UsoSvc",
        "CryptSvc",
        "WaaSMedicSvc",
        "BITS",
        "wuauserv"
    };

        // Helper method to stop services
        void StopService(string serviceName)
        {
            try
            {
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    if (sc.Status != ServiceControllerStatus.Stopped)
                    {
                        Console.WriteLine($"Stopping {serviceName}...");
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                        Console.WriteLine($"{serviceName} stopped successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"{serviceName} is already stopped.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to stop {serviceName}: {ex.Message}");
            }
        }

        string registryPath = "SYSTEM\\CurrentControlSet\\Services\\";

        foreach (var service in servicesToDisable)
        {
            Console.WriteLine($"Disabling {service}");
            StopService(service);
            SetRegistryValueSafe(registryPath + service, "Start", 4); // 4 = Disabled
        }
    }
}