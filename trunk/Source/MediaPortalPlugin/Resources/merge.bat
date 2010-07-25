@echo off

md tmp
ilmerge  /out:tmp\PandoraMusicBox.MediaPortalPlugin.dll PandoraMusicBox.MediaPortalPlugin.dll PandoraMusicBox.Engine.dll NLog.dll

IF EXIST PandoraMusicBox.MediaPortalPlugin_UNMERGED.exe del PandoraMusicBox.MediaPortalPlugin_UNMERGED.exe
ren PandoraMusicBox.MediaPortalPlugin.exe PandoraMusicBox.MediaPortalPlugin_UNMERGED.exe
IF EXIST PandoraMusicBox.MediaPortalPlugin_UNMERGED.pdb del PandoraMusicBox.MediaPortalPlugin_UNMERGED.pdb
ren PandoraMusicBox.MediaPortalPlugin.pdb PandoraMusicBox.MediaPortalPlugin_UNMERGED.pdb

move tmp\*.* .
rd tmp

