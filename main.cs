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
}