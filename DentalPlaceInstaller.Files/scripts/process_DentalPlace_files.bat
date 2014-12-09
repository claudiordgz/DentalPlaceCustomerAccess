@echo off
SETLOCAL ENABLEDELAYEDEXPANSION
call:AssembleWxsLibraries bin.txt,"..\..\DentalPlaceCustomerControl\bin"
echo.&pause&goto:eof

:AssembleWxsLibraries
SETLOCAL
SET listOfFolders=%~1
SET localPathToDirectories=%~2
for /F "tokens=*" %%A in (%listOfFolders%) do call:AssembleWxsSingular "%localPathToDirectories%" %%A
echo Finished Assembling
ENDLOCAL
goto:eof

:AssembleWxsSingular
SETLOCAL
SET localPathToDirectory=%~1
SET localDirectoryToInstall=%~2
SET flag=%~3
IF [%localDirectoryToInstall%] NEQ [] (
	echo Assembling Wix File for %localDirectoryToInstall%
	call:HeatProcess %localDirectoryToInstall% "%localPathToDirectory%" %flag%
)
ENDLOCAL
goto:eof

:LCase
:UCase
:: Converts to upper/lower case variable contents
:: Syntax: CALL :UCase _VAR1 _VAR2
:: Syntax: CALL :LCase _VAR1 _VAR2
:: _VAR1 = Variable NAME whose VALUE is to be converted to upper/lower case
:: _VAR2 = NAME of variable to hold the converted value
:: Note: Use variable NAMES in the CALL, not values (pass "by reference")

SET _UCase=A B C D E F G H I J K L M N O P Q R S T U V W X Y Z
SET _LCase=a b c d e f g h i j k l m n o p q r s t u v w x y z
SET _Lib_UCase_Tmp=!%1!
IF /I "%0"==":UCase" SET _Abet=%_UCase%
IF /I "%0"==":LCase" SET _Abet=%_LCase%
FOR %%Z IN (%_Abet%) DO SET _Lib_UCase_Tmp=!_Lib_UCase_Tmp:%%Z=%%Z!
SET %2=%_Lib_UCase_Tmp%
GOTO:EOF

:HeatProcess
:: -dr 	Use the -dr flag with the name of one of the directories you
::   	actually wanted to create. That way, the components will be
::		copied into that directory during the installation.
:: -cg 	Add the -cg flag with a name to use for a new
::		ComponentGroup element. Heat will then group the
::		components.
:: -gg 	To have Heat create GUIDs for us, add the -gg flag
:: -g1	To have the GUIDs not have curly brackets, use the -g1 flag.
::		This is just a preference.
:: -sf	By default, Heat puts each component and your directory
::		structure in separate Fragment elements. Adding -sfrag
::		puts these elements into the same Fragment element.
:: -srd	There's not really any reason to harvest the folder where the
::		files are, so add the -srd flag.
:: -var	We can use the -var flag with the name of a preprocessor
::		variable (preceded by var) to insert in place of SourceDir.
::		Later on, we can set the variable from within the project's
::		Properties settings or on the command line.
:: -svb6 Suppress VB6 COM registration entries. When registering a COM 
::		component created in VB6 it adds registry entries that are part 
::		of the VB6 runtime component. This flag is recommend for VB6 
::		components to avoid breaking the VB6 runtime on uninstall.
SETLOCAL
SET localElement=%~1
SET localUpperElement=%~1
SET PathToElement="%~2"
SET FullElement=%~1
SET extraflag=%~3
IF [%PathToElement%] NEQ [] SET FullElement=%PathToElement%\%localElement%
heat.exe dir %FullElement% -srd -dr %localUpperElement%_FOLDER -cg  %localElement%FilesGroup -gg -g1 -sf -template fragment -var "var.FilesPath)$(var.%localElement%" -out ".\%localElement%.wxs" %extraflag% >> log.txt 2>&1
ENDLOCAL
goto:eof