#define MyAppName "Genshin Stella Mod"
#define MyAppVersion "7.1.0.0"
#define MyAppPublisher "Sefinek Inc."
#define MyAppURL "https://genshin.sefinek.net"
#define MyAppExeName "Genshin Stella Mod.exe"
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
AppUpdatesURL=https://github.com/sefinek24/Genshin-Impact-ReShade/wiki/14.-Changelog-for-v7.x.x
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
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Check: not InstViaSetup and not InstViaLauncher 

[Files]
Source: "C:\Genshin-Impact-ReShade\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Genshin-Impact-ReShade\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Genshin Impact ReShade Installer\Dependencies\Microsoft.VCLibs.x64.14.00.Desktop.appx"; DestDir: {tmp}; Flags: deleteafterinstall

Source: "UninsIS.dll"; Flags: dontcopy

[Icons]
Name: "{autodesktop}\Stella Mod Launcher"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{autoprograms}\Genshin Stella Mod\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"

[Run]
Filename: "powershell.exe"; Parameters: "Add-AppxPackage -Path {tmp}\Microsoft.VCLibs.x64.14.00.Desktop.appx"; StatusMsg: "Installing Microsoft VCLibs..."; Flags: runhidden
Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}} Launcher"; Flags: nowait postinstall skipifsilent runascurrentuser

#define public Dependency_NoExampleSetup
#include "CodeDependencies.iss"

[Code]
// ------------------------------------------- UNINSTALL THE OLD VERSION -------------------------------------------
// Import IsISPackageInstalled() function from UninsIS.dll at setup time
function DLLIsISPackageInstalled(AppId: string; Is64BitInstallMode,
  IsAdminInstallMode: DWORD): DWORD;
  external 'IsISPackageInstalled@files:UninsIS.dll stdcall setuponly';

// Import CompareISPackageVersion() function from UninsIS.dll at setup time
function DLLCompareISPackageVersion(AppId, InstallingVersion: string;
  Is64BitInstallMode, IsAdminInstallMode: DWORD): LongInt;
  external 'CompareISPackageVersion@files:UninsIS.dll stdcall setuponly';

// Import UninstallISPackage() function from UninsIS.dll at setup time
function DLLUninstallISPackage(AppId: string; Is64BitInstallMode,
  IsAdminInstallMode: DWORD): DWORD;
  external 'UninstallISPackage@files:UninsIS.dll stdcall setuponly';

// Wrapper for UninsIS.dll IsISPackageInstalled() function
// Returns true if package is detected as installed, or false otherwise
function IsISPackageInstalled(): Boolean;
begin
  result := DLLIsISPackageInstalled('{#MyAppId}',  // AppId
    DWORD(Is64BitInstallMode()),                   // Is64BitInstallMode
    DWORD(IsAdminInstallMode())) = 1;              // IsAdminInstallMode
  if result then
    Log('UninsIS.dll - Package detected as installed')
  else
    Log('UninsIS.dll - Package not detected as installed');
end;

// Wrapper for UninsIS.dll CompareISPackageVersion() function
// Returns:
// < 0 if version we are installing is < installed version
// 0   if version we are installing is = installed version
// > 0 if version we are installing is > installed version
function CompareISPackageVersion(): LongInt;
begin
  result := DLLCompareISPackageVersion('{#MyAppId}',  // AppId
    '{#MyAppVersion}',                                  // InstallingVersion
    DWORD(Is64BitInstallMode()),                      // Is64BitInstallMode
    DWORD(IsAdminInstallMode()));                     // IsAdminInstallMode
  if result < 0 then
    Log('UninsIS.dll - This version {#MyAppVersion} older than installed version')
  else if result = 0 then
    Log('UninsIS.dll - This version {#MyAppVersion} same as installed version')
  else
    Log('UninsIS.dll - This version {#MyAppVersion} newer than installed version');
end;

// Wrapper for UninsIS.dll UninstallISPackage() function
// Returns 0 for success, non-zero for failure
function UninstallISPackage(): DWORD;
begin
  result := DLLUninstallISPackage('{#MyAppId}',  // AppId
    DWORD(Is64BitInstallMode()),                 // Is64BitInstallMode
    DWORD(IsAdminInstallMode()));                // IsAdminInstallMode
  if result = 0 then
    Log('UninsIS.dll - Installed package uninstall completed successfully')
  else
    Log('UninsIS.dll - installed package uninstall did not complete successfully');
end;

function PrepareToInstall(var NeedsRestart: Boolean): string;
begin
  result := '';
  // If package installed, uninstall it automatically if the version we are
  // installing does not match the installed version; If you want to
  // automatically uninstall only...
  // ...when downgrading: change <> to <
  // ...when upgrading:   change <> to >
  if IsISPackageInstalled() and (CompareISPackageVersion() <> 0) then
    UninstallISPackage();
end;


// ------------------------------------------- INSTALL REQUIRED DEPS -------------------------------------------
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


// ------------------------------------------- PARAMS -------------------------------------------
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
  Result := CmdLineParamExists('/SETUP');
end;

function InstViaLauncher(): Boolean;
begin
  Result := CmdLineParamExists('/UPDATE');
end;