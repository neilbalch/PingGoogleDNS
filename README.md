# PingGoogleDNS

Windows Desktop Tray application that checks network status.

## Description

This aplication sits in the notification tray and displays either a success or fail status of a ping to the Google DNS server, (*8.8.8.8*) chosen for it's reliability and consistent performance as a connection metric. The reason for this program's existence is to verify when a network adapter finishes initializing an internet connection, as sometimes even when the Windows built-in network icon shows as connected, internet access isn't established.

The executable looks for the two icon files (*`connected.ico` and `disconnected.ico`*) in the same directory as itself, meaning that the icons (and therefore how the tray icon looks) can be changed without rebuilding the application.

Pings are sent out every second, and the tray icon is changed accordingly based on whether or not the ping response was recieved or there was a transmission error.

## How to setup to run on login

1. Type `Win` + `R` to open the run menu, type `shell:startup` and press enter. This opens the startup folder for the current user.
2. Copy the executable (*`.exe`*) and the two `.ico` icon files in the ["Releases" page](https://github.com/neilbalch/PingGoogleDNS/releases) to the startup folder.
3. Upon the next restart (*or login*) the executable will launch.
