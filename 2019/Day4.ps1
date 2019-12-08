# https://adventofcode.com/2019/day/4
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function parseInput {
    # $source = Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))

    # "$source = @('$((Get-Clipboard) -join "', '")')" | Set-Clipboard
    # $source = @('1', '2', '3')

    # $source = @'...'@ -split "\r?\n"
    $source = @('1,2,3')

    $data = foreach ($line in $source) {
        $points = foreach($p in $line -split ',') {
            if ($p -notmatch '^(\d+)$') { throw $p }
            [PSCustomObject]@{
                n = [int]$Matches[1]
            };
        }
        [PSCustomObject]@{
            Points = $points
        }
    }

    return $data
}
# parseInput

function part1 {
    # 240298 < 244444 <= n <= 779999 < 784956 < 788000
    # 
    $count = 0
    $n = '244444'
    while ($true) {
        # look for consecutive pair
        foreach ($i in 4..0) { 
            if ($n[$i] -eq $n[$i+1]) {
                ++$count
                break
            }
        }

        # next
        foreach ($i in 5..0) { if ($n[$i] -ne '9') { break } }
        $next = ([char]([int]$n[$i] + 1)).ToString() * (6 - $i)
        $n = $n.Substring(0, $i) + $next
        if ($n -eq '779999') {
            ++$count
            break 
        }
    }
    return $count
}
part1 #= 1150


function part2 {
    # 240298 < 244444 <= n <= 779999 < 784956 < 788000
    # 
    $count = 0
    $n = '244444'
    while ($true) {
        # look for *lone* consecutive pair
        foreach ($i in 4..0) { 
            if ($n[$i] -eq $n[$i+1]) {
                if ($i -gt 0 -and $n[$i-1] -eq $n[$i]) {
                    continue
                }
                if ($i -lt 4 -and $n[$i+2] -eq $n[$i]) {
                    continue
                }
                ++$count
                break
            }
        }

        # next
        foreach ($i in 5..0) { if ($n[$i] -ne '9') { break } }
        $next = ([char]([int]$n[$i] + 1)).ToString() * (6 - $i)
        $n = $n.Substring(0, $i) + $next
        if ($n -eq '779999') {
            ++$count
            break 
        }
    }
    return $count
}
part2 #= 748
