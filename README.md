# LemonTree.Pipeline.Tools

first 2 are on Nexus:
https://nexus.lieberlieber.com/#browse/browse:lemontree-pipeline-tools

## LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe
https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe

## LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe
https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe

Use in Workflows is very simple:
```
&"C:\Program Files\Git\mingw64\bin\curl.exe" https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe --output LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe

.\LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe .\DemoModel.eapx

```
