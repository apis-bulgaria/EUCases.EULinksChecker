@echo off
@cls
::@echo on
set BASE=C:\inetpub\wwwroot\LinkPdf\Content\pdf
set q=-q
set p2h="%BASE%\p2h\bin\p2h.exe" -s %q%
set eulc="%BASE%\eulc-ns.exe"
set css="%BASE%\main.css"
if "%3"=="" (set LANG=en)else (set LANG=%3)
set ocss=--user-style-sheet m.css

set o=--allow . --load-error-handling ignore --enable-local-file-access --enable-external-links --image-quality 100 %ocss% --no-debug-javascript --enable-javascript
set h2p="%BASE%\h2p\bin\h2p.exe" %q% %o%
set m=m

if "%1"=="" goto Usage
if "%1"=="clear" goto Clear
if "%2"=="" goto Usage
set ou=%2
set ouext=%~x2
if "%ouext%"=="" set ou=%ou%.pdf
if "%ou%" == "m.pdf" set m=%m%m

call :Clear
if not "%ocss%"=="" copy /y %css% .>nul 2>nul
del /Q %m%>nul 2>nul
del /Q "%ou%">nul 2>nul
%p2h% "%1" %m%
::@echo %eulc% %m%-html.html r%m%-html.html %LANG%
::@echo %eulc% %CD%\%m%-html.html %CD%\r%m%-html.html %LANG%
%eulc% %CD%\%m%-html.html %CD%\r%m%-html.html %LANG%
%h2p% r%m%-html.html "%ou%"
::
for %%i in (*) do if not "%%i" == "%ou%" del %%i
exit /b 0

:Usage
@echo %~nx0 InFile OutFile [LANG=en|bg|de|fr|it]
@echo     or
@echo %~nx0 clear
exit /b 0

:Clear
del /Q *
exit /b 0
