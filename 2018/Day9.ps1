# https://adventofcode.com/2018/day/9
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 384205
    part2 #= 3066307353
}

function DoIt([int]$players, [int]$lastMarble) {
    $scores = (1..$players).ForEach{0}
    $board = [System.Collections.Generic.LinkedList[int]]::new()
    # hardcode first 2 moves...
    $board.Add(0)
    $board.Add(2)
    $board.Add(1)
    $curIdx = 1
    $cur = $board.First.Next
    $m = 3
    $player = 3
    while ($m -le $lastMarble) {
        if ($m % 23 -eq 0) {
            # backward 7
            for ($i = 0; $i -lt 7; $i++) {
                if ($curIdx -gt 0) {
                    $cur = $cur.Previous
                    --$curIdx
                } else {
                    $cur = $board.Last
                    $curIdx = $board.Count - 1
                }
            }
            # remove current
            $rm = $cur
            $cur = $cur.Next
            if (!$cur) {
                $cur = $board.First
                $curIdx = 0
            }
            $board.Remove($rm)
            # update player score
            $scores[$player - 1] += $m + $rm.Value
        }
        else {
            # forward 2
            $curIdx += 2
            if ($curIdx -eq $board.Count) {
                # append
                $cur = $board.AddLast($m)
            } elseif ($curIdx -gt $board.Count) {
                # add after first
                $cur = $board.AddAfter($board.First, $m)
                $curIdx = 1
            } else {
                $cur = $board.AddAfter($cur.Next, $m)
            }
        }

        ++$m
        $player = ($player + 1) % $players
    }

    $max = 0
    for ($i = 0; $i -lt $scores.Count; $i++) {
        if ($scores[$i] -gt $max) {
            $max = $scores[$i]
        }
        Write-Host ('{0,4}: {1,5}' -f ($i+1), $scores[$i])
    }
    $max
}

function part1 {
    #DoIt 9 25 #=32
    #DoIt 10 1618 #=8317
    #DoIt 13 7999 #=146373
    #DoIt 17 1104 #=2764
    #DoIt 21 6111 #=54718
    #DoIt 30 5807 #=37305
    DoIt 476 71431
}

function part2 {
    DoIt 476 7143100
}

main
