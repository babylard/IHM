using System;
using System.IO;

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
    Console.WriteLine("Press Enter to exit... ");
    Console.ReadLine();
}
else if (userInput.ToLower() == "n")
{
    Console.WriteLine("Canceled.");
    Console.WriteLine("Press Enter to exit...");
    Console.ReadLine();
}
else
{
    Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
    Console.ReadLine();
}
