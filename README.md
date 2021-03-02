# DungeonsModMerger
 Tool to merge Minecraft: Dungeons custom level mods (.levelmod files) into one single .pak file.
 
# Prerequisites
  - Python 3.7+ 
  - .NET 5.0 Runtime ([x64](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-desktop-5.0.2-windows-x64-installer) | [x86](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-desktop-5.0.2-windows-x86-installer))
 
# How to use this Tool?
  1. Download the latest release from the [Releases](https://github.com/LukeFZ/DungeonsModMerger/releases/latest) page and unpack it into a new folder.
  2. Start the "ModMerger.exe" and follow the instructions in the window.
  
  This creates a MergedMods.pak file that contains all custom level mods you merged, and the matching "customlevels.json" for the [Dungeons Level Loader](https://github.com/LukeFZ/DungeonsLevelLoader).
  
  
  # For developers
   - How to create .levelmod files?   
    .levelmod files are renamed .zip archives, that contain a "Dungeons" folder with the assets for the given level   
    (like a normal .pak mod), but also a "Game.json" with the custom strings you want to use for your level.   
    They should follow this naming scheme: "*modName\*.levelmod".  
    **Note: An example .levelmod file is provided [here](https://cdn.discordapp.com/attachments/768927253722955819/808340880154034176/ExampleMod.levelmod). (Level created by Azuran_SHadow)** 
    
   - I want to create my own custom levels:   
     If you want to make your own custom levels for Minecraft: Dungeons you need to follow a few basic guidelines:
     
     1. Do not replace any official assets, only add your own.
        This is to ensure mod compatability between all level mods.
        
     2. ~Do not change the level-id to anything other than "creeperwoods".
       This is needed to ensure that the custom strings you are using are going to be loaded by the game.~  
       (**Game update 1.8.0.0 changed this to allow for custom lighting, you can now choose whichever level id you want.  
       You just need to set the "loctable-id" to "creeperwoods", to allow for the custom strings to be loaded.**
       
      The [Dokucraft Mod Kit](https://github.com/Dokucraft/Dungeons-Mod-Kit) and [Level Format Documentation](https://github.com/Dokucraft/Dungeons-Level-Format) are also going to be very helpful resources in your journey.
     
   - I have more questions that were not answered here:
     You can reach out to me on the [Dungeoneer's Hideout](https://discord.gg/S7gKeh5FR2) discord server. 
