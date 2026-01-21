# LemonTree.Pipeline.Tools

They are availble for downloard from the LieberLieber Nexus:\
https://nexus.lieberlieber.com/#browse/browse:lemontree-pipeline-tools

## LemonTree.Pipeline.Tools.ModelCheck.exe
Used to check Models for LemonTree Readiness.
https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.ModelCheck.exe

### Commandline Reference

```
  --Out               File to output .md e.g.: 'out.md'

  --NoCompact         If set the Checks that compact the Model are not run!

  --FailOnErrors      If set the Exitcode will be 2 if there is at least on Check of Status Error!

  --FailOnWarnings    If set the Exitcode will be 1 if there is at least on Check of Status Warning!

  --TableSize         If set the size of the tables in the database will be reported!

  --Orphans           If set Model Orphans will be reported!

  --ChecksConfig      Path to JSON file with checks configuration. If not set, hardcoded checks will be used.

  --Model             Required. The  'Model' used for the operation.

  --help              Display this help screen.

  --version           Display version information.
```

### Custom Checks Configuration

ModelCheck supports loading checks from a JSON configuration file using the `--ChecksConfig` parameter. If no configuration file is specified, the tool uses hardcoded default checks.

To use a custom checks configuration:

```
LemonTree.Pipeline.Tools.ModelCheck.exe --model "model.eapx" --ChecksConfig "./checks-config.json" --out ".\output.md"
```

The JSON configuration file should follow this structure:

```json
{
  "checks": [
    {
      "id": "check-id",
      "query": "SELECT COUNT(*) FROM [table] WHERE [condition]",
      "passedTitle": "Title when check passes",
      "failedTitle": "Title when check fails",
      "passedDetail": "Details when check passes",
      "failedDetail": "Details when check fails",
      "passedLevel": "Info",
      "failedLevel": "Error",
      "includeCountInTitle": true
    }
  ]
}
```

See the [sample checks-config.json](src/LemonTree.Pipeline.Tools.ModelCheck/checks-config.json) in the repository for a complete example with all default checks.

### Powershell Example:
```
&"C:\Program Files\Git\mingw64\bin\curl.exe" https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.ModelCheck.exe --output LemonTree.Pipeline.Tools.ModelCheck.exe

LemonTree.Pipeline.Tools.ModelCheck.exe  --model "model.eapx" --out ".\output.md" --FailOnErrors --FailOnWarnings
echo "finished validation with $LASTEXITCODE"
echo "Exit codes of LemonTree.Pipeline.Tools.ModelCheck.exe:"
echo "* -2 = other runtime exception occurred"
echo "* -1 = CLI argument parsing error occurred"
echo "*  0 = model is valid "
echo "*  1 = model has at least one warning (only if --FailOnWarnings or --FailOnErrors)"
echo "*  2 = model has at least one error  (only if --FailOnErrors)"
```

### Sample Output:

# LemonTree ModelCheck results
| | Severity | Issue | Message |
|----------|----------|---------|---------|
|:red_circle:|Error|Auditing is enabled.|Auditing is not helpful or required if you manage a model inside a VCS with LemonTree.|
|:red_circle:|Error|Model has 12 Audit Entries|Audits are not helpful or required if you manage a model inside a VCS with LemonTree.|
|:red_circle:|Error|Model has 1 Resource Allocation Entries|Resource Allocations are not supported when using LemonTree.|
|:orange_circle:|Warning|Model has 1 VCS enabled Packages|Models with Package based VCS  are not a supported scenario.|
|:green_circle:|Passed|No DIAGRAMIMAGEMAP entries in the model||
|:green_circle:|Passed|No Baseline entries in the model||
|:green_circle:|Passed|User Security not enabled in the Model||
|:green_circle:|Passed|No embedded binary images or document entries in the model||
|:green_circle:|Passed|No ModelDocument entries in the model||
|:green_circle:|Passed|No t_image entries in the model||
|:green_circle:|Passed|Model size before compact 1.45 MB after 1.45 MB|If you run compact on the model you can reduce the size to 99.46%|
|:green_circle:|Passed|Model size before strip and compact 1.45 MB after 1.43 MB|If you strip the model from binary content and run compact on the model you can reduce the size to 98.66%|

# Project Statistics
|Count|Measure|
|-------|-------|
|0|Attributes in Elements|
|0|Constraints on Elements|
|0|Efforts on Elements|
|0|Element Testing|
|10|Elements in Diagrams|
|0|File on Elements|
|0|Metrics on Elements|
|3|Note|
|0|Opertions in Elements|
|7|Package|
|0|Parameters in Operations|
|1|Part|
|0|Requirements in Elements|
|1|Resources Allocated to Elements|
|0|Risks on Elements|
|2|Signal|
|0|Total Connectors|
|4|Total Diagrams|
|13|Total Elements|
|8|Total Packages|
|1|Total Root Packages|




## LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe
If the Enterprise Architect Models are "poluted" with DIAGRAMIMAGEMAPS - you can use this little commandline Tool to delete those! (.eap(x) and .qea(x) only)\
https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe

### Powershell Example:
```
&"C:\Program Files\Git\mingw64\bin\curl.exe" https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe --output LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe

.\LemonTree.Pipeline.Tools.RemovePrerenderedDiagrams.exe .\DemoModel.eapx
```

## LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe
Used to set "Filters" in LemonTree Single Session Files in the Pipeline.\
https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe

### Powershell Example:
```
&"C:\Program Files\Git\mingw64\bin\curl.exe" https://nexus.lieberlieber.com/repository/lemontree-pipeline-tools/LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe --output LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe

.\LemonTree.Pipeline.Tools.SetFilterInSessionFile.exe SampleCompareSession.ltsfs "#Conflicted" "$HideGraphicalChanges
```
