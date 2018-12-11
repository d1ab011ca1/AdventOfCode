# https://adventofcode.com/2018/day/11
Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 21,37
    part2 #= 236,146,12
}

function RackId($serialNo, $x, $y) {
    $rackId = $x + 10
    [math]::floor(((($rackId * $y + $serialNo) * $rackId) % 1000) / 100) - 5
}

function printRegion([Int32[, ]]$grid, [int]$x = 0, [int]$y = 0, [int]$dx = 3, [int]$dy = 3) {
    $maxX = $x + $dx
    $maxY = $y + $dy
    for (; $y -lt $maxY; $y++) {
        for ($i = $x; $i -lt $maxX; $i++) {
            Write-Host ('{0,2}, ' -f $grid[$i, $y]) -NoNewline
        }
        Write-Host ''
    }
}

function MaxPower([Int32[, ]]$grid, [int]$size) {
    [int]$maxX = -1
    [int]$maxy = -1
    [int]$maxP = [int]::MinValue

    for ($x = 300 - $size; $x -ge 0; $x--) {
        for ($y = 300 - $size; $y -ge 0; $y--) {
            [int]$p = 0
            for ($i = $x + $size - 1; $i -ge $x; $i--) {
                for ($j = $y + $size - 1; $j -ge $y; $j--) {
                    $p += $grid[$i, $j]
                }
            }

            if ($p -gt $maxP) {
                $maxP = $p
                $maxX = $x
                $maxY = $y
                # "$($maxX + 1),$($maxY + 1) = $maxP"
            }
        }
    }
    return $maxX, $maxY, $maxP
}

function part1 {
    #RackId 8 3 5 #= 4
    #RackId 57 122 79 #= -5
    #RackId 39 217 196 #= 0
    #RackId 71 101 153 #= 4

    $serialNo = 8561

    $grid = [array]::CreateInstance([int], 300, 300)
    for ($x = 1; $x -le 300; $x++) {
        for ($y = 1; $y -le 300; $y++) {
            $v = RackId $serialNo $x $y
            $grid[($x - 1), ($y - 1)] = $v
        }
    }

    $maxX, $maxY, $maxP = MaxPower $grid 3
    printRegion $grid $maxX $maxY 3 3
    "$($maxX+1),$($maxY+1) = $maxP"
}

function part2 {
    $serialNo = 8561

    $grid = [array]::CreateInstance([int], 300, 300)
    for ($x = 1; $x -le 300; $x++) {
        for ($y = 1; $y -le 300; $y++) {
            $v = RackId $serialNo $x $y
            $grid[($x - 1), ($y - 1)] = $v
        }
    }

    $maxP = [int]::MinValue
    foreach ($size in (12..12)) {
        $x, $y, $p = MaxPower $grid $size
        if ($p -gt $maxP) {
            $maxP = $p
            printRegion $grid $x $y $size $size
            "$($x + 1),$($y + 1),$size = $maxP"
        }
    }
}

main