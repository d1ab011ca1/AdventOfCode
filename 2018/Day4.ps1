# https://adventofcode.com/2018/day/4
Set-StrictMode -Version Latest

function input {

    # "@($((Get-Clipboard) -join ", "))" | set-clipboard
    # "@('$((Get-Clipboard) -join "', '")')" | set-clipboard

    Get-Content -Path (Join-Path $PSScriptRoot 'day4.txt') | 
    Sort-Object | 
    ForEach-Object {
        if (!($_ -match '^\[([^\]]+)\] (Guard #(\d+)|wakes|falls)')) {throw}
        $time = [datetime]$Matches[1]
        $sleeping = $Matches[2] -eq 'falls'
        if ($Matches[3]) {
            $guard = [int]$Matches[3]
        }
        [PSCustomObject]@{
            guard = $guard
            time = $time
            start = !!$Matches[3]
            sleeping = $sleeping
        }
    }
}
#input
#return

function part1 {
    $groups = input | group Guard
    foreach($group in $groups) {
        $group | Add-Member Sleeping 0
        $in = $group.Group
        for ($i = 0; $i -lt $in.Count; $i++) {
            $cur = $in[$i]
            if ($cur.start) {
                # beginning of shift (awake)
            }elseif (!$cur.sleeping) {
                # was sleeping
                $group.Sleeping += $cur.time - $in[$i-1].time
            }
        }
    }
    $max = $groups | sort Sleeping | select -Last 1

    $minutes = (0..59)|%{0}
    $in = $max.Group
    for ($i = 0; $i -lt $in.Count; $i++) {
        $cur = $in[$i]
        if ($cur.start) {
            # beginning of shift (awake)
        }elseif (!$cur.sleeping) {
            # was sleeping
            $end = $cur.time.Minute
            $begin = $in[$i-1].time.Minute
            ($begin..($end-1)) | %{ $minutes[$_]++ }
        }
    }
    (0..59)|%{ "$_ $($minutes[$_])"}
    $maxCount= ($minutes | measure -Maximum).Maximum
    $minute = (0..59) | ?{$minutes[$_] -eq $maxCount}

    "$($max.Name) * $minute = $([int]$max.Name * $minute)"
}

function part2 {
    $groups = input | group Guard
    foreach($group in $groups) {
        $group | Add-Member Minutes ((0..59)|%{0})
        $in = $group.Group
        for ($i = 0; $i -lt $in.Count; $i++) {
            $cur = $in[$i]
            if ($cur.start) {
                # beginning of shift (awake)
            }elseif (!$cur.sleeping) {
                # was sleeping
                $end = $cur.time.Minute
                $begin = $in[$i-1].time.Minute
                ($begin..($end-1)) | %{ $group.minutes[$_]++ }
            }
        }
    }
    $max = $groups | sort {($_.Minutes | measure -Maximum).Maximum} | select -Last 1

    (0..59)|%{ "$_ $($max.minutes[$_])"}
    $maxCount= ($max.minutes | measure -Maximum).Maximum
    $minute = (0..59) | ?{$max.minutes[$_] -eq $maxCount}

    "$($max.Name) * $minute = $([int]$max.Name * $minute)"
}

Clear-Host
#part1 #= 102688
part2 #= 56901
