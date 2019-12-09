# https://adventofcode.com/2019/day/6
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

    foreach ($line in $source) {
        $p = $line.Split(')')
        [PSCustomObject]@{
            Parent = $p[0]
            Child = $p[1]
        }
    }
}
# parseInput

function part1 {
    $in = parseInput # @('COM)B', 'B)C', 'C)D', 'D)E', 'E)F', 'B)G', 'G)H', 'D)I', 'E)J', 'J)K', 'K)L')

    $m = @{}
    foreach($i in $in) {
        $m[$i.Child] = $i.Parent
    }
    # return $m

    $orbits = 0
    foreach($child in $m.Keys) {
        $parent = $m[$child]
        do {
            ++$orbits
            $parent = $m[$parent]
        } while ($parent)
    }
    $orbits
}
part1 #= 358244

function part2 {
    $in = parseInput # @('COM)B', 'B)C', 'C)D', 'D)E', 'E)F', 'B)G', 'G)H', 'D)I', 'E)J', 'J)K', 'K)L', 'K)YOU', 'I)SAN')

    $m = @{}
    foreach($i in $in) {
        $m[$i.Child] = $i.Parent
    }
    # return $m

    function orbits ($child) {
        $path = @()
        $parent = $m[$child]
        do {
            $path += $parent
            $parent = $m[$parent]
        } while ($parent)
        $path
    }

    $path1 = orbits 'YOU'
    $path2 = orbits 'SAN'
    # "you = $path1"
    # "san = $path2"

    # find the first common orbit...
    if ($path1.Length -gt $path2.Length) {
        # swap so we iterate the shortest path
        $temp = $path1
        $path1 = $path2
        $path2 = $temp
    }
    for ($d1 = 0; $d1 -lt $path1.Length; ++$d1) {
        $d2 = $path2.IndexOf($path1[$d1])
        if ($d2 -ge 0) {
            return $d2 + $d1
        }
    }
    throw 'Common orbit not found!'
}
part2 #= 517
