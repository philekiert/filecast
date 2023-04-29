# filecast

Filecast is an aggressively minimalistic audio player, primarily designed for listening through large backlogs of podcast episodes. Imagine every feature you've ever dreamed of in an audio player - Filecast does almost none of them.

It really only serves one purpose; to sequentially play through a folder of audio files in alphabetical order, remembering where it was when closed and resuming when reopened, looking pretty while doing so.

Instructions:
1) Place in a folder with the audio files you want to listen to, and run.
3) Use the hotkeys listed below to operate.
2) When the program is closed, it will generate a .txt file with the current audio file name and position. When reopened, it will resume playing where it left off.

-	Note that this application will currently only play MP3, WAV and WMA files.

<br><br><br>
<p align="center">
<img src="https://user-images.githubusercontent.com/29918840/235314267-71107691-68c8-47bd-92ee-509e1921710a.png">
</p>
<br><br>

## Kotkeys:
- A / D - Skip forward or backward 5 seconds (hold Shift for 1 minute skips)
- Q / E - Skip to previous or subsequent file.
- Space - Play/Pause

- Back & Forward media keys skip 5 seonds (owing to its intended use as a podcast player).
- Play/Pause media key works as expected.

_____________

Makes use of NAudio and NHotkey nuget packages.
