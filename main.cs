using Microsoft.Win32;
using System.Management.Automation;

internal class IHM
{
    static void Main(string[] args)
    {
        Console.WriteLine("[1] Blacklist Microsoft data collection");
        Console.WriteLine("[2] Disable Cortana");
        Console.WriteLine("[3] Disable Bloat (Optimization)");
        Console.WriteLine("Choose an option: ");
        var userInput2 = Console.ReadLine();

        if (userInput2.ToLower() == "1")
        {
            DisableSpyware();
        }
        else if (userInput2.ToLower() == "2")
        {
            Console.Clear();
            Console.WriteLine("[1] Disable Cortana");
            Console.WriteLine("[2] Re-Enable Cortana");
            var userinput3 = Console.ReadLine();

            if (userinput3.ToLower() == "1")
            {
                Console.Clear();
                DisableCortana();
            }

            else if (userinput3.ToLower() == "2")
            {
                Console.Clear();
                EnableCortana();
            }
        }
        else if (userInput2.ToLower() == "3")
        {
            DisableBloat();
        }
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }

    static void DisableSpyware()
    {

        string sourceFileName = "hosts";
        string sourcePath = Path.Combine(Environment.CurrentDirectory, "Assets", sourceFileName);
        string destinationPath = @"C:\Windows\System32\drivers\etc\" + sourceFileName;
        Console.WriteLine("Do you want to proceed? (y/n): ");
        var userInput = Console.ReadLine();

        // Check the user's response
        if (userInput.ToLower() == "y")
        {

            Console.Clear();
            try
            {
                // Check if the destination file exists
                if (File.Exists(destinationPath))
                {
                    // If it exists, delete the file
                    Console.WriteLine("Patching...");
                    File.Delete(destinationPath);
                }

                // Copy the file
                File.Copy(sourcePath, destinationPath);

                string DataCollection1 = "HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection";
                string DataCollection2 = "HKLM:\\SOFTWARE\\Policies\\Microsoft\\Windows\\DataCollection";
                string DataCollection3 = "HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection";

                SetRegistryValue(DataCollection1, "AllowTelemetry", 0);
                SetRegistryValue(DataCollection2, "AllowTelemetry", 0);
                SetRegistryValue(DataCollection3, "AllowTelemetry", 0);

                Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}\n\n(Is your antivirus interfering?");
            }
        }
        else if (userInput.ToLower() == "n")
        {
            Console.WriteLine("Canceled.");
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
            Console.ReadLine();
        }
    }

    static void DisableCortana()
    {
        Console.WriteLine("Disabling Cortana...");
        SetRegistryValue("HKCU\\SOFTWARE\\Microsoft\\Personalization\\Settings", "AcceptedPrivacyPolicy", 0);
        SetRegistryValue("HKCU\\SOFTWARE\\Microsoft\\InputPersonalization", "RestrictImplicitTextCollection", 1);
        SetRegistryValue("HKCU\\SOFTWARE\\Microsoft\\InputPersonalization", "RestrictImplicitInkCollection", 1);
        SetRegistryValue("HKCU\\SOFTWARE\\Microsoft\\InputPersonalization\\TrainedDataStore", "HarvestContacts", 0);
        Console.WriteLine("Done.");
    }

    static void EnableCortana()
    {
        Console.WriteLine("Re-enabling Cortana...");
        SetRegistryValue("HKCU\\SOFTWARE\\Microsoft\\Personalization\\Settings", "AcceptedPrivacyPolicy", 1);
        SetRegistryValue("HKCU\\SOFTWARE\\Microsoft\\InputPersonalization", "RestrictImplicitTextCollection", 0);
        SetRegistryValue("HKCU\\SOFTWARE\\Microsoft\\InputPersonalization", "RestrictImplicitInkCollection", 0);
        SetRegistryValue("HKCU\\SOFTWARE\\Microsoft\\InputPersonalization\\TrainedDataStore", "HarvestContacts", 1);
        Console.WriteLine("Done.");
    }

    static void DisableTask(string taskName)
    {
        using (var powerShell = PowerShell.Create())
        {
            powerShell.AddCommand("Get-ScheduledTask");
            powerShell.AddParameter("TaskName", taskName);

            var tasks = powerShell.Invoke();

            if (tasks.Count > 0)
            {
                foreach (var task in tasks)
                {
                    powerShell.Commands.Clear();
                    powerShell.AddCommand("Disable-ScheduledTask");
                    powerShell.AddArgument("InputObject", task);
                    powerShell.Invoke();
                    Console.WriteLine($"Disabled task: {taskName}");
                }
            }
            else
            {
                Console.WriteLine($"Task '{taskName}' not found.");
            }
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
        SetRegistryValue("HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo", "Enabled", 0);
        Console.WriteLine("Disabling Cortana as a part of Windows Search");
        SetRegistryValue("HKLM:\\SOFTWARE\\Policies\\Microsoft\\Windows\\Windows Search", "AllowCortana", 0);
        Console.WriteLine("Disabling Bing Search in Start Menu");
        SetRegistryValue("HKCU:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Search", "BingSearchEnabled", 0);
        Console.WriteLine("Making Mixed Reality Portal uninstallable");
        SetRegistryValue("HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Holographic", "FirstRunSucceeded", 0);
        Console.WriteLine("Disabling live tiles");
        string liveRegistryKey = @"HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\PushNotifications";

        if (Registry.GetValue(liveRegistryKey, null, null) == null)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\PushNotifications");
            key.Close();
        }

        Registry.SetValue(liveRegistryKey, "NoTileApplicationNotification", 1);
        Console.WriteLine("Disabling Location Tracking");
        string SensorState = "HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Sensor\\Overrides\\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}";
        string LocationConfig = "HKLM:\\SYSTEM\\CurrentControlSet\\Services\\lfsvc\\Service\\Configuration";
        SetRegistryValue(SensorState, "SensorPermissionState", 0);
        SetRegistryValue(LocationConfig, "LocationConfig", 0);

        Console.WriteLine("Disabling People icon in Taskbar");
        SetRegistryValue("HKCU:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\People", "PeopleBand", 0);



        Console.WriteLine("\nDone!");
    }
}