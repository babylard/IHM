# I Hate Microsoft

I Hate Microsoft (or IHM for short) is a crappy Win10 Debloater. Still a WIP.

![IHM_DEDczNSF6a](https://github.com/user-attachments/assets/a0143e97-7fc4-4298-8580-a9d2c5b5ec95)

# Functions
Brief overview of what each option actually does.


## Bloat
Disabling first sets the following registry entries to `0`. First string on each line is Folder location, second is Entry to be modified.
```c#
{ @"HKLM:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo", "Enabled"}, // Windows Feedback experience
{ @"HKCU:\Software\Microsoft\Windows\CurrentVersion\Holographic", "FirstRunSucceeded"},// Mixed reality portal uninstallable
{ @"HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\PushNotifications", "NoTileApplicationNotification"}, // Disable live tiles
{ @"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People", "PeopleBand" }, // People icon in Taskbar
{ @"HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SystemPaneSuggestionsEnabled" } // Start Reccomendations
```
Then sets all of the following Entries in `HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager` to `0` as well
```c#
"ContentDeliveryAllowed",
"SOemPreInstalledAppsEnabled",
"PreInstalledAppsEnabled",
"PreInstalledAppsEverEnabled",
"SilentInstalledAppsEnabled",
"SystemPaneSuggestionsEnabled"
```

Re-Enabling is just this process but set to `1`.

## Telemetry

Modifies the `hosts` file in Windows, which is for mapping hostnames to IP Addresses. The file is written out with common addresses that Microsoft sends Telemetry to even after disabling Optional Data in the settings. They are all mapped to a [loopback address](https://nordvpn.com/cybersecurity/glossary/loopback-address); `127.0.0.1`

May prevent you from accessing certain Bing services like the Widgets and Bing's new tab News Feed. Below is a list of each address.


- telemetry.microsoft.com
- settings-win.data.microsoft.com
- v10.vortex-win.data.microsoft.com
- diagnostics.support.microsoft.com
- a-0001.a-msedge.net
- edge.microsoft.com
- feedback.microsoft.com
- comptex.microsoft.com
- data.microsoft.com
- msftconnecttest.com
- azureedge.net
- activity.windows.com
- bingapis.com
- msedge.net
- assets.msn.com
- scorecardresearch.com
- data.msn.com

Then modifies more Registry Entries relating to Location tracking and Data Collection. When Re-Enabling, `hosts` is just overwritten with the default comments left by Microsoft, and Registry Entries are reverted to `1`.

## Unessesary Services
Disables the following Windows services;

- UsoSvc
- CryptSvc
- WaasMedicSvc
- BITS
- wuauserv

### UsoSvc
Manages Windows Update checks and Installation.

### CrypstSvc
Handles Cryptographic Services as the name implies, which incldes ensuring that software (like Windows Updates) is signed.

### WaasMedicSvc
Attempts to repair corrupt Windows Updates.

### BITS
Downloads updates and other bs while Idle. Also possibly vunreble to dll Injection.

### wuauserv
Main Windows Update service. Tends to use a ton of resources.
