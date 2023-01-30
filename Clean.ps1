# 清理Release、zip
if (Test-Path "./Release") { Remove-Item "./Release" -Recurse -Force } 
if (Test-Path "./**.zip") { Remove-Item "./**.zip" -Recurse -Force } 

# 清理obj, Debug
Write-Host "Cleaning up 'obj, Debug, Release',Not included 'node_modules'";
Get-ChildItem .\ -include bin, obj, Debug, Release -Recurse | ForEach-Object ($_) { 
    if (!$_.fullname.contains("node_modules")) { 
        Write-Host "delete: " $_.fullname;
        remove-item $_.fullname -Force -Recurse;
    } 
}
"Cleanup succeeded,Any key to exit";
# Read-Host | Out-Null ;
# Exit