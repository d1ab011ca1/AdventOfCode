# https://adventofcode.com/2019/day/14
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

class Chem {
    [string] $N
    [int] $Q

    [string] ToString() { return "$($this.N):$($this.Q)" }
}

function parseInput($source) {
    if (!$source) {
        $source = Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))
    }

    # "`$source = @('$((Get-Clipboard) -join "', '")')" | Set-Clipboard
    # $source = @('1', '2', '3')

    $map = @{}
    foreach ($line in $source.where{ $_ }) {
        $reagents = foreach ($t in $line.trim() -split ',|=>') {
            if ($t -notmatch '^\s*(\d+) (\w+)\s*$') { throw "Bad line: $t" }
            [Chem]@{Q = [int]$Matches[1]; N = $Matches[2] }
        }

        $map[$reagents[-1].N] = [pscustomobject]@{
            Q      = $reagents[-1].Q
            Inputs = $reagents[0..($reagents.count - 2)]
        }
    }
    #$map.Keys | sort N,Q | %{"$($map[$_]) => $_"}
    $map
}
return parseInput

. "$PSScriptRoot\..\..\powershell-algorithms\src\algorithms\math\least-common-multiple\leastCommonMultiple.ps1"

function part1 {
    $map = parseInput

    $map = parseInput (@'
    10 ORE => 10 A
    1 ORE => 1 B
    7 A, 1 B => 1 C
    7 A, 1 C => 1 D
    7 A, 1 D => 1 E
    7 A, 1 E => 1 FUEL
'@ -split '\r?\n')

    function calc([string]$chem, $quan) {
        $amount = 0
        $inputs = $map[$chem]
        $quan = leastCommonMultiple $quan $inputs.Q
        foreach ($in in $inputs.Inputs) {
            if ($in.N -eq 'ORE') {
                $amount += $in.Q
            }
            else {
                $amount += calc $in.N $in.Q
            }
        }
        $amount * $quan
    }
    calc 'FUEL' 1
}
part1 #= 

function part2 {
    $in = parseInput
}
part2 #= 
