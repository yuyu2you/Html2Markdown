opencover.console -register:user "-target:C:\Program Files\dotnet\dotnet.exe" "-targetargs:test .\test\Html2Markdown.Test\Html2Markdown.Test.csproj" -filter:+[Html2Markdown.Test*]* -output:coverage.xml -oldStyle

pause