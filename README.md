# Welcome to the SaveMod20XX wiki!

This application is able to modify the key portions of the 20XX save file to enable/disable items of your choosing to customize your game experience.

### To use:
1. Make sure you have .NET 4.5.2 installed and enabled on your system
2. Either download the executable from /SaveMod20XX/bin/Release/ or compile the source code yourself
3. Run the program once with no arguments to generate a default **XML settings file**
4. Configure the elements you wish to enable/disable in the settings file using a text editor 
     (preferably one with XML code coloring support, such as the fabulous [Notepad++](https://notepad-plus-plus.org/))
5. Drag-and-drop your 20XX **save file** onto the executable (or provide it's full filepath as the singular argument to the program)
> Your 20XX save file is located in \Documents\20XX\. The file is called 20xxalphav64.sav. The tool will automatically make a backup of the last save-file it modified in case you want to undo any changes you make. Note that it will always replace the previous backup every time, so if you want a pre-edit save file for posterity, you'll want to move it somewhere else before running the program a second time.


In the future, when new items are added or when item flags and locations within the file change, simply adjust the values within the settings file to continue to use the program. No need to recompile!
