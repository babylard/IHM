using Microsoft.Win32;

internal class IHM
{
    static void Main(string[] args)
    {
        Console.WriteLine("[1] Blacklist Microsoft data collection");
        Console.WriteLine("[2] Disable Cortana");
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
        Console.WriteLine("Adding Registry keys to prevent bloatware apps from returning...");
        string registryPath = @"HKLM:\SOFTWARE\Policies\Microsoft\Windows\CloudContent";
        string registryOEM = @"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";

        // Check for existing keys
        if (RegistryKey.OpenBaseKey(GetRootKey(registryPath), GetView(registryPath)) == null)
        {
            // Create missing key
            Registry.CurrentUser.CreateSubKey(GetSubKey(registryPath));
        }

        if (RegistryKey.OpenBaseKey(GetRootKey(registryOEM), GetView(registryOEM)) == null)
        {
            // Create missing key
            Registry.CurrentUser.CreateSubKey(GetSubKey(registryOEM));
        }

        Registry.SetValue(registryPath, "DisableWindowsConsumerFeatures", 1, RegistryValueKind.DWord);
        Registry.SetValue(registryOEM, "ContentDeliveryAllowed", 0, RegistryValueKind.DWord);
        Registry.SetValue(registryOEM, "OemPreInstalledAppsEnabled", 0, RegistryValueKind.DWord);
        Registry.SetValue(registryOEM, "PreInstalledAppsEnabled", 0, RegistryValueKind.DWord);
        Registry.SetValue(registryOEM, "PreInstalledAppsEverEnabled", 0, RegistryValueKind.DWord);
        Registry.SetValue(registryOEM, "SilentInstalledAppsEnabled", 0, RegistryValueKind.DWord);
        Registry.SetValue(registryOEM, "SystemPaneSuggestionsEnabled", 0, RegistryValueKind.DWord);

        Console.WriteLine("Registry keys added successfully.");
        Console.WriteLine("Making Mixed Reality Portal uninstallable");
        SetRegistryValue("HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Holographic", "FirstRunSucceeded", 0);
        Console.WriteLine("Disabling Wi-Fi Sense");
        
        string WifiSense1 = "HKLM:\\SOFTWARE\\Microsoft\\PolicyManager\\default\\WiFi\\AllowWiFiHotSpotReporting";
        string WifiSense2 = "HKLM:\\SOFTWARE\\Microsoft\\PolicyManager\\default\\WiFi\\AllowAutoConnectToWiFiSenseHotspots";
        string WifiSense3 = "HKLM:\\SOFTWARE\\Microsoft\\WcmSvc\\wifinetworkmanager\\config";
    }
}