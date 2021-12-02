function input {
    Get-Content "$PSScriptRoot\Day 1.txt" |
    ForEach-Object {
        [int]$_
    }
}

function part1 {
    $d = input
    $p = $d[0]
    $count = 0
    $d | select -skip 1 | foreach {
        if ($_ -gt $p) { ++$count }
        $p = $_
    }

    "Part 1: $count"
}

function part2 {
    [int[]]$d = input
    $count = 0
    for ($i = 3; $i -lt $d.Count; $i++) {
        if ($d[$i] -gt $d[$i - 3]) { ++$count }
    }

    "Part 2: $count"
}

part1
part2