; NSIS AddPinyin Installer
; Include
!include MUI.nsh
!include LogicLib.nsh
!include x64.nsh

; General
!define dllname "Release\AddPinyin.dll"
!define dbname "Release\romanization.db"
!define dllname6 "Release 6\AddPinyin.dll"
!define displayname "AddPinyin Plugin for MarcEdit"
!define menginedll "C:\Program Files\MarcEdit 6\mengine60.dll"
!define menginedll86 "C:\Program Files (x86)\MarcEdit 6\mengine60.dll"
!define menginedll7 "C:\Program Files\MarcEdit 7\mengine7.dll"
!define menginedll7_86 "C:\Program Files (x86)\MarcEdit 7\mengine7.dll"
!define menginedll75 "C:\Program Files\Terry Reese\MarcEdit 7.5\mengine7.dll"
!define menginedll75_86 "C:\Program Files (x86)\Terry Reese\MarcEdit 7.5\mengine7.dll"
!define menginedll7user "$APPDATA\MarcEdit 7\mengine7.dll"
!define menginedll75user "$APPDATA\Terry Reese"
!define installDir7 "$APPDATA\marcedit7\plugins"
!define installDir75 "$APPDATA\marcedit75\plugins75"

Name "${displayname}"
OutFile "InstallAddPinyin.exe"
InstallDir "$APPDATA\marcedit\plugins"

; Interface Settings
!define MUI_ABORTWARNING

; Installer Pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

; Languages
!insertmacro MUI_LANGUAGE "English"

; Prerequisites Section
Section "-Prerequisites"

IfFileExists "${menginedll}" MarcEditInstalled 0
IfFileExists "${menginedll86}" MarcEditInstalled 0
IfFileExists "${menginedll7}" MarcEditInstalled 0
IfFileExists "${menginedll7_86}" MarcEditInstalled 0
IfFileExists "${menginedll7user}" MarcEditInstalled 0
IfFileExists "${menginedll75}" MarcEditInstalled 0
IfFileExists "${menginedll75_86}" MarcEditInstalled 0
IfFileExists "${menginedll75user}" MarcEditInstalled 0
	MessageBox MB_OK "You must install MarcEdit before installing this plugin."
	Quit

MarcEditInstalled:

SectionEnd

; Installer Section
Section "-Install"

IfFileExists "${installDir75}" install75 0
Goto version7check

install75:
SetOutPath "${installDir75}"
File "${dllname}"
File "${dbname}"

version7check:
IfFileExists "${installDir7}" install7 0
Goto version6check

install7:
SetOutPath "${installDir7}"
File "${dllname}"
File "${dbname}"

version6check:
IfFileExists "$INSTDIR" install6 0
Goto installend

install6:
SetOutPath "$INSTDIR"
File "${dllname6}"
File "${dbname}"

installend:

SectionEnd

