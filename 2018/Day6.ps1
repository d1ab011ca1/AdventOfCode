# https://adventofcode.com/2018/day/6
Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 3620 (#31)
    part2 #= 39930
}

function distance([System.Drawing.Point]$point, [int]$x, [int]$y) {
    return [math]::Abs($point.X - $x) + [math]::Abs($point.Y - $y)
}

function part1 {
    $in = input
    $Xbounds = $in | measure X -min -max
    $Ybounds = $in | measure Y -min -max
    'x=({0}..{1}) y=({2}..{3})' -f $Xbounds.Minimum, $Xbounds.Maximum, $Ybounds.Minimum, $Ybounds.Maximum
    $Xmin = [int]$Xbounds.Minimum - 1
    $Xmax = [int]$Xbounds.Maximum + 1
    $Ymin = [int]$Ybounds.Minimum - 1
    $Ymax = [int]$Ybounds.Maximum + 1
    $XSize = $XMax - $XMin + 1
    $YSize = $YMax - $YMin + 1

    $grid = [array]::CreateInstance([nullable[int]], $XSize, $YSize)
    for ([int]$y = $YMin; $y -le $YMax; $y++) {
        for ([int]$x = $XMin; $x -le $XMax; $x++) {
            $minD = 10000
            $closest = $null
            for ([int]$pointNo = 0; $pointNo -lt $in.Count; $pointNo++) {
                # $d = distance $in[$pointNo] $x $y
                $d = [math]::Abs($in[$pointNo].X - $x) + [math]::Abs($in[$pointNo].Y - $y)

                if ($d -lt $minD) {$minD = $d; $closest = $pointNo; $tie = $false}
                elseif ($d -eq $minD) {$tie = $true}
            }
            if (!$tie) { $grid[($x - $XMin), ($y - $YMin)] = $closest }
        }
    }
    #for ([int]$y = $YMin; $y -le $YMax; $y++) {
    #    Write-Host ('{0,3}: ' -f $y) -NoNewline
    #    #for ([int]$x = $XMin; $x -le $XMax; $x++) {
    #    for ([int]$x = $Xmin + 0; $x -le $Xmin + 50; $x++) {
    #        $v = $grid[($x - $XMin), ($y - $YMin)]
    #        if ($null -eq $v) {
    #            $color = 'Gray'
    #            $v = '.. '
    #        } else {
    #            $color = $v % 6 + 1
    #            $v = '{0,2:D2} ' -f $v
    #        }
    #        if ($y -lt $Ybounds.Minimum -or $y -gt $Ybounds.MAximum -or $x -lt $Xbounds.Minimum -or $x -gt $Xbounds.Maximum) {
    #            $color = 'DarkGray'
    #        }
    #        Write-Host $v -NoNewline -ForegroundColor $color
    #    }
    #    Write-Host ''
    #}

    $ignore = @{}
    for ($x = 0; $x -lt $XSize; $x++) {
        if ($null -ne $grid[$x, 0]) { $ignore[$grid[$x, 0]] = 1 }
        if ($null -ne $grid[$x, ($YSize - 1)]) { $ignore[$grid[$x, ($YSize - 1)]] = 1 }
    }
    for ($y = 0; $y -lt $YSize; $y++) {
        if ($null -ne $grid[0, $y]) { $ignore[$grid[0, $y]] = 1 }
        if ($null -ne $grid[($XSize - 1), $y]) { $ignore[$grid[($XSize - 1), $y]] = 1 }
    }
    
    $fin = @{}
    for ($x = 1; $x -lt $XSize-1; $x++) {
        for ($y = 1; $y -lt $YSize-1; $y++) {
            $v = $grid[$x, $y]
            if ($null -eq $v -or $ignore.ContainsKey($v)) {
                continue
            }
            $fin[$v]++
        }
    }
    $fin.GetEnumerator() | sort Value | select -last 1
}

function part2 {
    $in = input
    $Xbounds = $in | measure X -min -max
    $Ybounds = $in | measure Y -min -max
    'x=({0}..{1}) y=({2}..{3})' -f $Xbounds.Minimum, $Xbounds.Maximum, $Ybounds.Minimum, $Ybounds.Maximum
    $Xmin = [int]$Xbounds.Minimum
    $Xmax = [int]$Xbounds.Maximum
    $Ymin = [int]$Ybounds.Minimum
    $Ymax = [int]$Ybounds.Maximum

    $region = 0
    for ($x = $XMin; $x -le $XMax; $x++) {
        :YLoop for ($y = $YMin; $y -le $YMax; $y++) {
            $d = 0
            for ([int]$pointNo = 0; $pointNo -lt $in.Count; $pointNo++) {
                # $d = distance $in[$pointNo] $x $y
                $d += [math]::Abs($in[$pointNo].X - $x) + [math]::Abs($in[$pointNo].Y - $y)

                if ($d -ge 10000) {
                    continue YLoop
                }
            }
            ++$region
        }
    }
    $region
}

function input {
    $source = `
'1, 1
1, 6
8, 3
3, 4
5, 5
8, 9'

    $source = Get-Content (Join-Path $PSScriptRoot 'Day6.txt')

    Add-Type -AssemblyName 'System.Drawing'
    $source -split "`n" | %{ if ($_ -match '(\d+), (\d+)') { [System.Drawing.Point]::new([int]$Matches[1], [int]$Matches[2]) } }
}
#input
#return

main