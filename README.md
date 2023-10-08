# filecast

Filecast is an aggressively minimalist audio player, designed for listening through large backlogs of podcast episodes. Imagine every feature you've ever dreamed of in an audio player - Filecast does almost none of them.

What it does do is sequentially play through a folder of audio files in alphabetical order, remembering where it was when closed and resuming when reopened, looking pretty while doing so.

Instructions:
1) Place in a folder with the audio files you want to listen to, and run.
2) Use the hotkeys listed below to operate.
3) When the program is closed, it will generate a .txt file with the current audio file name and position, along with a list of played* files. When reopened, Filecast will resume playing where it left off.
4) The stylus will be blue or pink if the file is unplayed or played <b>*</b> respectively.

<b>*</b> A file is considered to have been played when it automatically rolls over to the following file, or when it is manually set to played by hitting the P key.

-	Note that this application will currently only play MP3, WAV and WMA files, and it relies on these three-letter extensions to filter out non-audio files.

<br><br><br>
<p align="center">
<img src="https://user-images.githubusercontent.com/29918840/235314267-71107691-68c8-47bd-92ee-509e1921710a.png">
</p>
<br><br>

## Hotkeys:
- A / D - Skip forward or backward 5 seconds (hold Shift for 1 minute skips)
- Q / E - Skip to previous or subsequent file.
- Space - Play/Pause
- P - Manually mark a file as played.
- Right-click - Right click anywhere in the window to load the 

- Back & Forward media keys skip 5 seconds (owing to its intended use as a podcast player).
- Play/Pause media key works as expected.
<br>

_____________
<br>
Makes use of NAudio and NHotkey nuget packages.
