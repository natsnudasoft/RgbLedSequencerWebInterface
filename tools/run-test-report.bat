@echo off
SET openCoverVersion=4.6.519
SET xunitRunnerVersion=2.2.0
SET reportGeneratorVersion=2.5.8
SET coverallsVersion=1.3.4
IF "%1"=="buildserver" (
    "%UserProfile%\.nuget\packages\OpenCover\%openCoverVersion%\tools\OpenCover.Console.exe" -register:user "-filter:+[*]* -[*Tests]*" -target:"%UserProfile%\.nuget\packages\xunit.runner.console\%xunitRunnerVersion%\tools\xunit.console.x86.exe" -targetargs:"\"%~dp0..\test\unit\RgbLedSequencerWebInterfaceTests\bin\x86\Release\net47\win7-x86\Natsnudasoft.RgbLedSequencerWebInterfaceTests.dll\" -noshadow -appveyor" -excludebyattribute:*.ExcludeFromCodeCoverage* -excludebyfile:*Designer.cs -output:coverage.xml
    "%UserProfile%\.nuget\packages\coveralls.io\%coverallsVersion%\tools\coveralls.net.exe" --opencover coverage.xml
    IF %errorlevel%==0 echo Report generated and sent to coveralls...
) ELSE (
    "%UserProfile%\.nuget\packages\OpenCover\%openCoverVersion%\tools\OpenCover.Console.exe" -register:user "-filter:+[*]* -[*Tests]*" -target:"%UserProfile%\.nuget\packages\xunit.runner.console\%xunitRunnerVersion%\tools\xunit.console.x86.exe" -targetargs:"\"%~dp0..\test\unit\RgbLedSequencerWebInterfaceTests\bin\x86\Debug\net47\win7-x86\Natsnudasoft.RgbLedSequencerWebInterfaceTests.dll\" -noshadow" -excludebyattribute:*.ExcludeFromCodeCoverage* -excludebyfile:*Designer.cs -output:coverage.xml
    "%UserProfile%\.nuget\packages\ReportGenerator\%reportGeneratorVersion%\tools\ReportGenerator.exe" -reports:coverage.xml -targetdir:coverage
    echo Press any key to display report...
    pause >nul
    start coverage\index.htm
)
