@echo off

md tmp
:: add following for .NET 4.0:  /targetplatform:v4,C:\Windows\Microsoft.NET\Framework\v4.0.30319
ilmerge  /out:tmp\pandora.exe pandora.exe PandoraMusicBox.Engine.dll PandoraMusicBox.Player.dll DirectShowLib-2005.dll

IF EXIST pandora_UNMERGED.exe del pandora_UNMERGED.exe
ren pandora.exe pandora_UNMERGED.exe
IF EXIST pandora_UNMERGED.pdb del pandora_UNMERGED.pdb
ren pandora.pdb pandora_UNMERGED.pdb

move tmp\*.* .
rd tmp

