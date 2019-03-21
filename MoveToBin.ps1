param (
	[string] $p1
)

$sourceDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath("$p1");
$targetDir = (Join-Path $sourceDir "bin\");

# Create the bin folder
New-Item "$targetDir" -ItemType Directory -Force;

# Move all files to the bin folder, relative to their current path
Get-ChildItem $sourceDir -Include "*.dll", "*.pdb", "*.xml" -Recurse -File | `
  foreach { 
    $sourceFile = $_.FullName;
    $relativeFile = $sourceFile.SubString($sourceDir.Length);
    
    if (!($relativeFile.StartsWith("bin\"))) {
      $targetFile = $targetDir + $relativeFile;
      $targetFileDir = (Split-Path $targetFile -Parent);
      New-item $targetFileDir -ItemType Directory -Force;
      Move-Item $sourceFile -Destination $targetFile;
    }
  };

# Recursively delete all empty directories
# https://stackoverflow.com/a/28631669/7517185
do {
  $dirs = gci $sourceDir -Directory -Recurse | Where { (gci $_.fullName -Force).Count -eq 0 } | Select -ExpandProperty FullName
  $dirs | Foreach-Object { Remove-Item $_ -Force }
} while ($dirs.count -gt 0)
