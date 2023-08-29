# Semantic Versioning Prototyp
# Copyright: LieberLieber Software GmbH 2023
# Inputs basemodel and mymodel .eapx or .qeax are supported and LemonTree.Automation.exe Path.
# Uses a special Version of LemonTree.Automation that exports a diffreport.xml file
# Sample LTA Parameters: merge --theirs "performancetest_small_a.eap" --mine "performancetest_small_b.eap" --dryRun "true" --abortOnConflict "True" --diffReport "true"

$BaseModel = ".\..\src\Models\Semantic-Base.eapx"
$TheirsModel = ".\..\src\Models\Semantic-Change.eapx"
$UpdateModel = "Semantic-Update.eapx"

$LTAToolPath = "C:\Source\Apps.LemonTree\src\LemonTree.Automation\bin\Debug\net6.0\LemonTree.Automation.exe"
$ToolsSemantic = ".\..\bin\LemonTree.Pipeline.Tools.Semantic.exe"

&$LTAToolPath merge --theirs $BaseModel --mine $TheirsModel --dryRun 'true' --abortOnConflict 'True' --diffReport 'true'

Copy-Item $TheirsModel $UpdateModel
$latest = Get-ChildItem ".\*.xml" | Sort-Object -Descending -Property LastWriteTime | Select-Object -last 1
$DiffFile = $latest.Name 
&$ToolsSemantic --Model $UpdateModel --DiffFile $DiffFile