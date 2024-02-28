# I Hate Microsoft

I Hate Microsoft (or IHM for short) is an application that lets you prevent Windows from sending information about you, and your activity to Microsoft, and also has the ability to disable Cortana.

Note: Your Antivirus may flag the software as Malware since it modifies files within the `Windows` folder.

# How does it work?

### Blacklisting data collection

This works by replacing a file in Windows called `hosts` with a modified version of the file. The hosts file is a plain text file used to map host names to IP addresses, so in this case we map the servers that Microsoft uses to send information to loopback IPs, being an IP that sends information back to you, therefore it isn't sent anywhere.

### Disabling Cortana

Literally just modifes Registry values. Nothing much.

# Roadmap

I plan to update this software with options to do much more in the future regarding Optimization, Privacy, and just useful features in general.