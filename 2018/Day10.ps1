# https://adventofcode.com/2018/day/10
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    part1 #= ERCXLAJL
    #part2 #= 10813
}

function print($in) {

    $Xbounds = $in | measure x -Minimum -Maximum
    $Ybounds = $in | measure y -Minimum -Maximum
    $Xsize = [int]($Xbounds.Maximum - $Xbounds.Minimum) + 1
    $Ysize = [int]($Ybounds.Maximum - $Ybounds.Minimum) + 1
    [System.Text.StringBuilder[]]$sky = (1..$Ysize).ForEach{
        [System.Text.StringBuilder]::new($Xsize).Append([char]' ', $Xsize)
    }

    foreach ($i in $in) {
        $sky[$i.y - $Ybounds.Minimum][$i.x - $Xbounds.Minimum] = '#'
    }

    foreach ($y in $sky) {
        Write-Host $y.ToString()
    }
}

function part1 {
    $in = input

    $step = 0
    $lastHeight = [int]::MaxValue
    while ($true) {
        
        $YBounds = $in | measure y -Minimum -Maximum
        $height = [int]($Ybounds.Maximum - $Ybounds.Minimum)
        if ($height -gt $lastHeight) {
            foreach ($i in $in) {
                $i.TickBack()
            }
            --$step

            print $in
            Write-Host $step seconds
            return
        }
        $lastHeight = $height
        
        foreach ($i in $in) {
            $i.Tick()
        }
        ++$step
    }
}

class Foo {
    [int]$x
    [int]$y
    [int]$dx
    [int]$dy

    Foo([int]$x, [int]$y, [int]$dx, [int]$dy) {
        $this.x = $x
        $this.y = $y
        $this.dx = $dx
        $this.dy = $dy
    }

    [void] Tick() {
        $this.x += $this.dx        
        $this.y += $this.dy
    }
    [void] TickBack() {
        $this.x -= $this.dx        
        $this.y -= $this.dy
    }
    [void] Step($step) {
        $this.x += $this.dx * $step
        $this.y += $this.dy * $step
    }
}

function input {
    $source = `
        'position=< 9,  1> velocity=< 0,  2>
position=< 7,  0> velocity=<-1,  0>
position=< 3, -2> velocity=<-1,  1>
position=< 6, 10> velocity=<-2, -1>
position=< 2, -4> velocity=< 2,  2>
position=<-6, 10> velocity=< 2, -2>
position=< 1,  8> velocity=< 1, -1>
position=< 1,  7> velocity=< 1,  0>
position=<-3, 11> velocity=< 1, -2>
position=< 7,  6> velocity=<-1, -1>
position=<-2,  3> velocity=< 1,  0>
position=<-4,  3> velocity=< 2,  0>
position=<10, -3> velocity=<-1,  1>
position=< 5, 11> velocity=< 1, -2>
position=< 4,  7> velocity=< 0, -1>
position=< 8, -2> velocity=< 0,  1>
position=<15,  0> velocity=<-2,  0>
position=< 1,  6> velocity=< 1,  0>
position=< 8,  9> velocity=< 0, -1>
position=< 3,  3> velocity=<-1,  1>
position=< 0,  5> velocity=< 0, -1>
position=<-2,  2> velocity=< 2,  0>
position=< 5, -2> velocity=< 1,  2>
position=< 1,  4> velocity=< 2,  1>
position=<-2,  7> velocity=< 2, -2>
position=< 3,  6> velocity=<-1, -1>
position=< 5,  0> velocity=< 1,  0>
position=<-6,  0> velocity=< 2,  0>
position=< 5,  9> velocity=< 1, -2>
position=<14,  7> velocity=<-2,  0>
position=<-3,  6> velocity=< 2, -1>'

    $source = Get-Content ([system#.io.path]::ChangeExtension($PSCmdlet.MyInvocation.MyCommand, 'txt'))

    $source -split "`n" | % {
        if (!($_ -match 'position=<([^,]+), ([^>]+)> velocity=<([^,]+), ([^>]+)>')) {throw $_}
        [Foo]::new([int]$Matches[1], [int]$Matches[2], [int]$Matches[3], [int]$Matches[4])
    }
}
#input
#return

main
