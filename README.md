# LemonTree.Pipeline.Tools

They are availble for downloard from the LieberLieber Nexus:\
https://nexus.lieberlieber.com/#browse/browse:lemontree-pipeline-tools

## LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe
If the Enterprise Architect Models are "poluted" with DIAGRAMIMAGEMAPS - you can use this little commandline Tool to delete those! (.eap(x) only)\
https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe

```
&"C:\Program Files\Git\mingw64\bin\curl.exe" https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe --output LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe

.\LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe .\DemoModel.eapx
```

## LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe
Used to set "Filters" in LemonTree Single Session Files in the Pipeline.\
https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe

```
&"C:\Program Files\Git\mingw64\bin\curl.exe" https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe --output LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe

.\LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe SampleCompareSession.ltsfs "#Conflicted" "$HideGraphicalChanges
```
## LemonTree.Pipeline.Tools.ModelCheck.exe
Used to check Models for LemonTree Readiness.
https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.ModelCheck.exe

...

&"C:\Program Files\Git\mingw64\bin\curl.exe" https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.ModelCheck.exe --output LemonTree.Pipeline.Tools.ModelCheck.exe

LemonTree.Pipeline.Tools.ModelCheck.exe  --model "model.eapx" --out ".\output.md" --FailOnErrors --FailOnWarnings
echo "finished validation with $LASTEXITCODE"
echo  Exit codes of LemonTree.Pipeline.Tools.ModelCheck.exe:
echo  * -2 = other runtime exception occurred
echo * -1 = CLI argument parsing error occurred
echo *  0 = model is valid (no error, no warning, no runtime exception)
echo  *  1 = model has at least one warning (only if --FailOnWarnings or --FailOnErrors)
echo *  2 = model has at least one error  (only if --FailOnErrors)

...
