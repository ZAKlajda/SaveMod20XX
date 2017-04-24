# Welcome to the SaveMod20XX wiki!
## Version 3.0 is now available

This application is able to modify the key portions of the 20XX save file to enable/disable items of your choosing to customize your game experience.

### To use:
1. Make sure you have .NET 4.5.2 installed and enabled on your system
2. Either download the executable from /SaveMod20XX/bin/Release/ or compile the source code yourself
3. Running the program will open a user interface (and generate a default **XML settings file** )
4. Enable or disable whatever items you wish using the drop-down menus in the "Availability" column
5. Click the "Perform Save File Modification" button to modify your save file
6. (Optional) Save the settings using the "Save Settings Changes" button to save them for next time

The program will look in the default file location for your save game. If it cannot find it, you can drag-and-drop the save game onto the program exe, or else specify the full file name and path as the first program argument.

> Your 20XX save file is located in \Documents\20XX\. The file is called 20xxalphav64.sav. The tool will automatically make a backup of the old save-file every time it is modified, in case you want to undo any changes you make. Note that it will add an incrementing counter to previous backup every time, so the latest backup will (usually) have the largest number at the end.


In the future, when new items are added or when item flags and locations within the file change, simply adjust the values within the **XML settings file** to continue to use the program. No need to recompile! Any items with the name "Unknown" will not appear in the GUI.


### Special thanks to Steam user Zero for [his helpful guide](http://steamcommunity.com/sharedfiles/filedetails/?id=776854746), without which this probably would never have happened.
