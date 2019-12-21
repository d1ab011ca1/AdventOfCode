# https://adventofcode.com/2019/day/8
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function parseInput($source) {
    if (!$source) {
        $source = Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))
    }

    # "`$source = @('$((Get-Clipboard) -join "', '")')" | Set-Clipboard
    # $source = @('1', '2', '3')

    ($source -join '').GetEnumerator().ForEach{[int]$_ - [int][char]'0'}
}
#return parseInput

function part1 {
    $in = parseInput
    $w = 25
    $h = 6
    $ppl = $w * $h

    $results = @{}
    for ($i = 0; $i -lt $in.Count; $i += $ppl) {
        $layer = $in[$i..($i+$ppl-1)]
        $g = $layer | group -AsHashTable
        $results[$g[0].count] = $g[1].count * $g[2].count
    }
    $results.GetEnumerator() | sort Key
}
#part1 #= 1950

function part2 {
    $in = parseInput
    $w,$h = 25,6
    #$in = '0222112222120000'.GetEnumerator().ForEach{[int]$_ - [int][char]'0'}
    #$w,$h = 2,2
    
    function printLayer([int[]]$layer) {
        for ($y = 0; $y -lt $h; $y++) {
            for ($x = 0; $x -lt $w; $x++) {
                $p = $layer[($y * $w) + $x]
                if ($p -eq 2) { Write-Host ' ' -NoNewline }
                elseif ($p -eq 1) { Write-Host ' ' -BackgroundColor 'Green' -NoNewline }
                elseif ($p -eq 0) { Write-Host ' ' -BackgroundColor 'Blue' -NoNewline }
            }
            Write-Host ''
        }
    }
    
    $ppl = $w * $h
    $layers = for ($i = 0; $i -lt $in.count; $i += $ppl) { @(,$in[$i..($i+$ppl-1)]) }
    #return $layers.ForEach{ printLayer $_; Write-Host '' }

    $image = [System.Collections.ArrayList]::new((0..($ppl-1)).ForEach{2})
    for ($i = 0; $i -lt $ppl; $i++) {
        # find first layer with non-transparent pixel
        foreach ($layer in $layers) {
            $pixel = $layer[$i]
            if ($pixel -ne 2) {
                $image[$i] = $pixel
                break
            }
        }
    }
    printLayer $image
}
part2 #= FKAHL
