
#define MyAppName "BUD Sustainable Building Designer"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Vircon, Inc."
#define MyAppURL "http://www.Vircon.com/"
#define MyAppExeName "BUD Sustainable Building Design Verison 1.0.0.exe"

[Setup]
AppId={{AB54848E-36A0-41B5-8ACC-1525088171FE}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=C:\ProgramData\Autodesk\Revit\Addins\2024\BUD
DefaultGroupName={#MyAppName}
AllowNoIcons=no
OutputDir=.\output
OutputBaseFilename={#MyAppName}
DisableDirPage=yes
Compression=lzma
SolidCompression=yes
Uninstallable=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "BUD.Drainage.Revit.Addin\bin\Rvt_24_Release\net48\**"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2024\BUD"; Flags:  ignoreversion recursesubdirs createallsubdirs
Source: "BUD.Drainage.Revit.Addin\BUD.Drainage.addin"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2024"; Flags: ignoreversion
Source: "D:\Gitspace\BUD.FireSafety\assets\revit\*"; DestDir: "C:\ProgramData\Autodesk\Revit\Addins\2024\BUD\assets"; Excludes:"*.rvt"; Flags:  ignoreversion recursesubdirs createallsubdirs

