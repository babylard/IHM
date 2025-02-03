using Microsoft.Win32;
using System.ServiceProcess;

internal class IHM
{   
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("[1] Telemetry");
            Console.WriteLine("[2] Bloat");
            Console.WriteLine("[3] Unessasary Services");
            Console.WriteLine("[#] ------------------------ ");
            Console.WriteLine("[4] Disable all");
            Console.WriteLine("[5] Re-Enable all");
            Console.WriteLine("");
            Console.WriteLine("Choose an option: ");
            string userInput1 = Console.ReadLine();
            
            if (userInput1 == "1")
            {
                Console.Clear();
                Console.WriteLine("[1] Disable Telemetry");
                Console.WriteLine("[2] Re-Enable Telemetry");
                Console.WriteLine("");
                Console.WriteLine("Choose an option: ");

                string userInput2 = Console.ReadLine();

                if (userInput2 == "1")
                {
                    DisableSpyware();
                }

                else if (userInput2 == "2")
                {
                    EnableSpyware();
                }
            }

            else if (userInput1 == "2")
            {
                Console.Clear();

                Console.WriteLine("[1] Disable Bloat");
                Console.WriteLine("[2] Re-Enable Bloat");
                Console.WriteLine("");
                Console.WriteLine("Choose an option: ");
                string userInput3 = Console.ReadLine();

                if (userInput3 == "1")
                {
                    DisableBloat();
                }

                else if (userInput3 == "2")
                {
                    EnableBloat();
                }
                
            }

            else if (userInput1 == "3")
            {
                Console.Clear();

                Console.WriteLine("[1] Disable Unessasary Services");
                Console.WriteLine("[2] Enable Unessasary Services");
                Console.WriteLine("");
                Console.WriteLine("Choose an option:");
                string userInput4 = Console.ReadLine();

                if (userInput4 == "1")
                {
                    DisableWindowsUpdates();
                }

                else if (userInput4 == "2")
                {
                    EnableWindowsUpdates();
                }
            }
            
            else if (userInput1 == "4")
            {
                Console.Clear();
                DisableSpyware();
                DisableBloat();
                DisableWindowsUpdates();
            }

            else if (userInput1 == "5")
            {
                Console.Clear();
                EnableSpyware();
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

            // Handle exit and menu return.
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
            SetRegistryValue(path, name, value);
            Console.WriteLine($"Successfully set {name} to {value} in {path}.");
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

            var DataCollection = new Dictionary<string,string>
            {
                { @"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry"}, // Telemetry
                { @"HKLM:\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry"}, // Telemetry
                { @"HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry" }, // Telemetry
                { @"HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}", "SensorPermissionState" }, // Location Tracking
                { @"HKLM:\SYSTEM\CurrentControlSet\Services\lfsvc\Service\Configuration", "LocationConfig" } // Location Tracking
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

            // Disable telemetry in registry.
            foreach(var entry in DataCollection)
            {
                SetRegistryValueSafe(entry.Key, entry.Value, 0);
            }

            Console.WriteLine();
            Console.WriteLine("\nDone!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}\n\n(Is your antivirus interfering?)");
        }
    }
    static void EnableSpyware()
    {
        string hostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
        Console.Clear();
        Console.WriteLine("Overwriting Hosts");
        try
        {
            // Check if the hosts file is writable
            if (!IsFileWritable(hostsFilePath))
            {
                Console.WriteLine("Error: Unable to access hosts file. Make sure you have administrative privileges.");
                return;
            }
            else
            {   // Write out file with default comments.
                File.WriteAllText(hostsFilePath, """
                                        # Copyright (c) 1993-2009 Microsoft Corp. 
                    # 
                    # This is a sample HOSTS file used by Microsoft TCP/IP for Windows. 
                    # 
                    # This file contains the mappings of IP addresses to host names. Each 
                    # entry should be kept on an individual line. The IP address should 
                    # be placed in the first column followed by the corresponding host name. 
                    # The IP address and the host name should be separated by at least one 
                    # space. 
                    # 
                    # Additionally, comments (such as these) may be inserted on individual 
                    # lines or following the machine name denoted by a '#' symbol. 
                    # 
                    # For example: 
                    # 
                    #      102.54.94.97     rhino.acme.com          # source server 
                    #       38.25.63.10     x.acme.com              # x client host 

                    # localhost name resolution is handled within DNS itself. 
                    #	127.0.0.1       localhost 
                    #	::1             localhost
                    """);
            }

            var DataCollection = new Dictionary<string, string>
            {
                { @"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry"}, // Telemetry
                { @"HKLM:\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry"}, // Telemetry
                { @"HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Policies\DataCollection", "AllowTelemetry" }, // Telemetry
                { @"HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}", "SensorPermissionState" }, // Location Tracking
                { @"HKLM:\SYSTEM\CurrentControlSet\Services\lfsvc\Service\Configuration", "LocationConfig" } // Location Tracking
            };

            foreach (var entry in DataCollection)
            {
                SetRegistryValueSafe(entry.Key, entry.Value, 1);
            }

            Console.WriteLine("");
            Console.WriteLine("Done!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}\n\n(Is your antivirus interfering?)");
        }
    }

    // Function to check if a file is writable
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

    static void DisableBloat()
    {
        Console.Clear();

        // Less important modifications
        var registryEntries = new Dictionary<string, string>
            {
                { @"HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo", "Enabled"}, // Windows Feedback experience
                { @"HKCU:\Software\Microsoft\Windows\CurrentVersion\Holographic", "FirstRunSucceeded"},// Mixed reality portal uninstallable
                { @"HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\PushNotifications", "NoTileApplicationNotification"}, // Disable live tiles
                { @"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People", "PeopleBand" }, // People icon in Taskbar
                { @"HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SystemPaneSuggestionsEnabled" } // Start Reccomendations
            };

        // Disable Microshit slop
        string OEM = @"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";

        string[] keys =
        {
            "ContentDeliveryAllowed",
            "SOemPreInstalledAppsEnabled",
            "PreInstalledAppsEnabled",
            "PreInstalledAppsEverEnabled",
            "SilentInstalledAppsEnabled",
            "SystemPaneSuggestionsEnabled"
        };


        foreach (var entry in registryEntries)
        {
            SetRegistryValueSafe(entry.Key, entry.Value, 0);
        }
        foreach(string key in keys)
        {
            SetRegistryValueSafe(OEM, key, 0);
        }
        Console.WriteLine("\nDone!");
    }

    static void EnableBloat()
    {
        Console.Clear();
        void SetRegistryValueSafe(string path, string name, object value)
        {
            try
            {
                SetRegistryValue(path, name, value);
                Console.WriteLine($"Successfully set {name} to {value} in {path}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set {name} in {path}: {ex.Message}");
            }
        }

        // Less important modifications
        var registryEntries = new Dictionary<string, string>
            {
                { @"HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo", "Enabled"}, // Windows Feedback experience
                { @"HKCU:\Software\Microsoft\Windows\CurrentVersion\Holographic", "FirstRunSucceeded"},// Mixed reality portal uninstallable
                { @"HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\PushNotifications", "NoTileApplicationNotification"}, // Disable live tiles
                { @"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People", "PeopleBand" }, // People icon in Taskbar
                { @"HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SystemPaneSuggestionsEnabled" } // Start Reccomendations
            };

        // Disable Microshit slop
        string OEM = @"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";

        string[] keys =
        {
            "ContentDeliveryAllowed",
            "SOemPreInstalledAppsEnabled",
            "PreInstalledAppsEnabled",
            "PreInstalledAppsEverEnabled",
            "SilentInstalledAppsEnabled",
            "SystemPaneSuggestionsEnabled"
        };

        foreach (var entry in registryEntries)
        {
            SetRegistryValue(entry.Key, entry.Value, 1);
        }

        foreach(var key in keys)
        {
            SetRegistryValueSafe(OEM, key, 1);
        }
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
            try
            {
                Console.WriteLine($"Enabling {service}");
                SetRegistryValue(registryPath + service, "Start", 2); // 2 = Auto
                StartService(service);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
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
            try
            {
                Console.WriteLine($"Disabling {service}");
                StopService(service);
                SetRegistryValue(registryPath + service, "Start", 4); // 4 = Disabled
            }
            catch( Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}