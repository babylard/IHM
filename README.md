# I Hate Microsoft

I Hate Microsoft (or IHM for short) is an simple application that prevents Windows from sending information about you, and your activity to Microsoft.

# How does it work?

This works by replacing a file in Windows called `hosts` with a modified version of the file. The hosts file is a plain text file used to map host names to IP addresses, so in this case we map the servers that Microsoft uses to send information to loopback IPs, being an IP that sends information back to you, therefore it isn't sent anywhere.

# Roadmap

I plan to update this software with options to do much more in the future regarding Optimization, Privacy, and just useful features in general.