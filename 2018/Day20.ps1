# https://adventofcode.com/2018/day/20
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= Furthest room 16,86 is 4344 doors away. Grid is 100 x 100 (WxH)
    part2 #= 8809 room are at least 1000 doors away.
}

function part1 {
    $directions = input
    $directions = $directions.Substring(1, $directions.Length - 2)

    $X = $Y = 0
    $minX = $minY = 0
    $maxX = $maxY = 0
    $stack = [System.Collections.Stack]::new()
    foreach ($c in $directions.GetEnumerator()) {
        if ($c -eq 'N') {if (--$Y -lt $minY) { $minY = $Y }}
        elseif ($c -eq 'S') {if (++$Y -gt $maxY) { $maxY = $Y }}
        elseif ($c -eq 'W') {if (--$X -lt $minX) { $minX = $X }}
        elseif ($c -eq 'E') {if (++$X -gt $maxX) { $maxX = $X }}
        elseif ($c -eq '|') { $X,$Y = $stack.Peek() }
        elseif ($c -eq '(') { $stack.Push(@($X,$Y)) }
        elseif ($c -eq ')') { $X,$Y = $stack.Pop() }
    }
    Write-Host "($minX,$minY)($maxX,$maxY)"

    $width = $maxX-$minX+1
    $height = $maxY-$minY+1
    $x = -$minX
    $y = -$minY
    $doors = 0
    $rooms = @{}
    foreach ($c in $directions.GetEnumerator()) {
        if ($c -eq 'N')     { --$y }
        elseif ($c -eq 'S') { ++$y }
        elseif ($c -eq 'W') { --$x }
        elseif ($c -eq 'E') { ++$x }
        elseif ($c -eq '|') { $x,$y,$doors = $stack.Peek(); continue }
        elseif ($c -eq '(') { $stack.Push(@($x,$y,$doors)); continue }
        elseif ($c -eq ')') { $x,$y,$doors = $stack.Pop(); continue }

        ++$doors
        if (!$rooms.ContainsKey("$x,$y")) {
            $rooms.Add("$x,$y", $doors)
        }
    }

    $furthest = $rooms.GetEnumerator() | sort Value | select -last 1 
    Write-Host "Part 1: Furthest room $($furthest.Key) is $($furthest.Value) doors away. Grid is $width x $height (WxH)."
}

function part2 {
    $directions = input
    $directions = $directions.Substring(1, $directions.Length - 2)

    $x = 0
    $y = 0
    $doors = 0
    $rooms = @{}
    $stack = [System.Collections.Stack]::new()
    foreach ($c in $directions.GetEnumerator()) {
        if ($c -eq 'N')     {--$y; ++$doors }
        elseif ($c -eq 'S') {++$y; ++$doors }
        elseif ($c -eq 'W') {--$x; ++$doors }
        elseif ($c -eq 'E') {++$x; ++$doors }
        elseif ($c -eq '|') { $x,$y,$doors = $stack.Peek(); continue }
        elseif ($c -eq '(') { $stack.Push(@($x,$y,$doors)); continue }
        elseif ($c -eq ')') { $x,$y,$doors = $stack.Pop(); continue }

        $r = "$x,$y"
        if (!$rooms.ContainsKey($r) -OR $rooms[$r] -gt $doors) {
            $rooms[$r] = $doors
        }
    }

    Write-Host "Part 2: $($rooms.Values.where{$_ -ge 1000}.Count) room are at least 1000 doors away."
}

function input {
    #return '^WNE$'
    #return '^ENWWW(NEEE|SSE(EE|N))$'
    #return '^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$'
    #return '^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$'
    #return '^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$'
    return Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))
}
#input
#return

main