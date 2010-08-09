@echo off
md merged
ilmerge  /out:merged\PandoraMusicBox.MediaPortalPlugin.dll PandoraMusicBox.MediaPortalPlugin.dll PandoraMusicBox.Engine.dll NLog.dll
