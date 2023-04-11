#define MyAppName "Genshin Stella Mod"
#define MyAppVersion "6.0.0.0-alpha.0"
#define MyAppPublisher "Sefinek Inc."
#define MyAppURL "https://genshin.sefinek.net"
#define MyAppExeName "Genshin Stella Mod Launcher.exe"
#define MyAppId "5D6E44F3-2141-4EA4-89E3-6C3018583FF7"

[Setup]
AppCopyright=Copyright 2023 © by Sefinek. All Rights Reserved.
AppId={#MyAppId}
AppMutex={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL=https://sefinek.net/genshin-impact-reshade/support
AppUpdatesURL=https://github.com/sefinek24/Genshin-Impact-ReShade/wiki/13.-Changelog-for-v6.x.x
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
MinVersion=6.1sp1
DefaultDirName=C:\Genshin-Impact-ReShade
DisableDirPage=yes
ChangesAssociations=no
DisableProgramGroupPage=yes
InfoBeforeFile=C:\Genshin-Impact-ReShade\data\README.txt
LicenseFile=C:\Genshin-Impact-ReShade\LICENSE
PrivilegesRequired=none
OutputBaseFilename=Genshin Stella Mod Setup
Compression=lzma
SolidCompression=yes
WizardStyle=classic
VersionInfoCompany={#MyAppPublisher}
VersionInfoTextVersion={#MyAppVersion}
VersionInfoVersion={#MyAppVersion}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "armenian"; MessagesFile: "compiler:Languages\Armenian.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "bulgarian"; MessagesFile: "compiler:Languages\Bulgarian.isl"
Name: "catalan"; MessagesFile: "compiler:Languages\Catalan.isl"
Name: "corsican"; MessagesFile: "compiler:Languages\Corsican.isl"
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "hebrew"; MessagesFile: "compiler:Languages\Hebrew.isl"
Name: "icelandic"; MessagesFile: "compiler:Languages\Icelandic.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "slovak"; MessagesFile: "compiler:Languages\Slovak.isl"
Name: "slovenian"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "CreateDesktopIcon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"; Check: not InstViaSetup and not InstViaLauncher and not DesktopIconExists
Name: "RunSfcSCANNOW"; Description: "Scan and repair system files"; GroupDescription: "Other:"; Flags: unchecked

[Files]
Source: "C:\Genshin-Impact-ReShade\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Genshin-Impact-ReShade\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Genshin Impact ReShade Installer\Dependencies\Microsoft.VCLibs.x64.14.00.Desktop.appx"; DestDir: {tmp}; Flags: deleteafterinstall
;Source: "Genshin Impact ReShade Installer\Dependencies\WindowsTerminal_Win10.msixbundle"; DestDir: {tmp}; Flags: deleteafterinstall

[Icons]
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Tasks: CreateDesktopIcon
Name: "{autoprograms}\Genshin Impact Mod Pack\Uninstall mod"; Filename: "{app}\unins000.exe"

[Run]
Filename: "powershell.exe"; Parameters: "Add-AppxPackage -Path {tmp}\Microsoft.VCLibs.x64.14.00.Desktop.appx"; StatusMsg: "Installing Microsoft VCLibs..."; Flags: runhidden
Filename: "cmd.exe"; Parameters: "sfc /SCANNOW"; Flags: runhidden; StatusMsg: "Scanning and reparing system files..."; Tasks: RunSfcSCANNOW

WorkingDir: "{app}"; Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}} Launcher"; Flags: nowait postinstall skipifsilent runascurrentuser

#define public Dependency_NoExampleSetup
#include "CodeDependencies.iss"

[Code]
function InitializeSetup: Boolean;
begin
  Dependency_AddDotNet48;
  Dependency_AddWebView2;

  Dependency_ForceX86 := True;
  Dependency_AddVC2015To2022;
  Dependency_ForceX86 := False;
  Dependency_AddVC2015To2022;

  Result := True;
end;

function CmdLineParamExists(const value: string): Boolean;
var
  i: Integer;
begin
  Result := False;
  for i := 1 to ParamCount do
    if CompareText(ParamStr(i), value) = 0 then
    begin
      Result := True;
      Exit;
    end;
end;

function InstViaSetup(): Boolean;
begin
  Result := CmdLineParamExists('/INSTVIASETUP');
end;

function InstViaLauncher(): Boolean;
begin
  Result := CmdLineParamExists('/INSTVIALAUNCHER');
end;

function DesktopIconExists(): Boolean;
begin
  Result := FileExists(ExpandConstant('{userdesktop}\{#MyAppName}.lnk'));
end;