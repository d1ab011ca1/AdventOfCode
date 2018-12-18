# https://adventofcode.com/2018/day/17
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 32552
    part2 #= 26405
}

class Point : System.IComparable {
    [int] $X
    [int] $Y

    Point() {
    }
    Point([int]$x, [int]$y) {
        $this.X = $x
        $this.Y = $y
    }
    Point([Point]$pt) {
        $this.X = $pt.X
        $this.Y = $pt.Y
    }

    [Point] Offset([Point]$pt) { $this.X += $pt.X; $this.Y += $pt.Y; return $this }
    [Point] Offset([int]$x, [int]$y) { $this.X += $X; $this.Y += $Y; return $this }

    [Point] Clone() { return [Point]::new($this.X, $this.Y) }

    [Point] Above() { return [Point]::new($this.X, $this.Y - 1) }
    [Point] Below() { return [Point]::new($this.X, $this.Y + 1) }
    [Point] Left() { return [Point]::new($this.X - 1, $this.Y) }
    [Point] Right() { return [Point]::new($this.X + 1, $this.Y) }

    static [Point] op_Addition([Point]$a, [Point]$b) { return [Point]::new($a.X + $b.X, $a.Y + $b.Y) }
    static [Point] op_Subtraction([Point]$a, [Point]$b) { return [Point]::new($a.X - $b.X, $a.Y - $b.Y) }

    [int] GetHashCode() {
        return $this.X.GetHashCode() -bxor $this.Y.GetHashCode()
    }
    [bool] Equals([object]$obj) {
        if ($obj.GetType() -ne [Point]) { return $false }
        return $this.CompareTo($obj) -eq 0
    }
    [int] CompareTo([object]$object) {
        if ($null -eq $object) { return 1 }
        [Point]$other = $object
        if ($null -eq $other) { throw [System.ArgumentException]'Object is not a Point' }
        $diff = $this.Y.CompareTo($other.Y)
        if ($diff -eq 0) { $diff = $this.X.CompareTo($other.X) }
        return $diff
    }

    #[string] ToString() {
    #    return '({0},{1})' -f $this.X, $this.Y
    #}
}

class Puzzle {
    [System.Text.StringBuilder[]]$grid
    [Point]$origin
    [int]$width
    [int]$depth
    [int]$minY
    [int]$maxY
    [Point[]] $streams = @()

    Puzzle([object[]]$clay) {
        $minX = ($clay | measure minX -Minimum).Minimum - 1
        $maxX = ($clay | measure maxX -Maximum).Maximum + 1
        $this.minY = ($clay | measure minY -Minimum).Minimum
        $this.maxY = ($clay | measure maxY -Maximum).Maximum
        $this.width = $maxX - $minX + 1
        $this.depth = $this.maxY + 1 + 1

        # shift X-coords to zero to simplify math
        $this.Origin = [Point]::new($minX, 0)
        $maxX -= $minX
        $minX = 0
    
        $this.grid = foreach ($y in 0..($this.depth - 1)) { 
            [System.Text.StringBuilder]::new('.' * $this.width)
        }

        # Insert spring at (500,0)
        $this.streams = @([Point]::new(500, 0) - $this.Origin)
        $this.setGridItem($this.streams[0], 'x')

        # Insert clay...
        foreach ($i in $clay) {
            if ($i.minX -eq $i.maxX) {
                $x = $i.minX - $this.Origin.X
                foreach ($y in ($i.minY - $this.Origin.Y)..($i.maxY - $this.Origin.Y)) {
                    $this.grid[$y][$x] = '#'
                }
            }
            else {
                $line = $this.grid[$i.minY - $this.Origin.Y]
                foreach ($x in ($i.minX - $this.Origin.X)..($i.maxX - $this.Origin.X)) {
                    $line[$x] = '#'
                }
            }
        }
    }

    [char] getGridItem([Point]$pt) {
        return $this.grid[$pt.Y][$pt.X]
    }
    [void] setGridItem([Point]$pt, [char]$c) {
        $this.grid[$pt.Y][$pt.X] = $c
    }
    [void] setGridItem([int]$x, [int]$y, [char]$c) {
        $this.grid[$Y][$X] = $c
    }

    [void] print() {
        $sb100 = [System.Text.StringBuilder]::new(' ' * $this.width)
        $sb10 = [System.Text.StringBuilder]::new(' ' * $this.width)
        $sb1 = [System.Text.StringBuilder]::new(' ' * $this.width)
        foreach ($x in 0..($this.width - 1)) {
            $n = $this.Origin.X + $x
            $sb100[$x] = [char]([int][char]'0' + [int][math]::floor($n / 100))
            $sb10[$x] = [char]([int][char]'0' + [int][math]::floor(($n % 100) / 10))
            $sb1[$x] = [char]([int][char]'0' + ($n % 10))
        }
        Write-Host "   $sb100" -ForegroundColor Gray
        Write-Host "   $sb10" -ForegroundColor Gray
        Write-Host "   $sb1" -ForegroundColor Gray
        
        foreach ($y in 0..($this.depth - 1)) {
            Write-Host ('{0,2} ' -f $y) -NoNewline -ForegroundColor Gray
            Write-Host $this.grid[$y].ToString()
        }
        Write-Host
    }


    [void] Go() {
        while ($this.Streams.Count) {
            # flow each stream until it overflows (createing oneor more new streams) 
            # or goes out of bounds (passes $this.maxY)
            $strms = $this.Streams
            $this.Streams = @()
            foreach ($strm in $strms) {
                $this.Flow($strm)
            }
        }
        $this.Print()
    }

    [void] Flow([point]$strm) {
        while ($true) {
            $next = $strm.Below()
            if ($next.Y -gt $this.maxY) {
                # stream is done
                return
            }

            $c = $this.getGridItem($next)
            if ($c -eq '.') {
                # Flow down
                $this.setGridItem($next, '|')
                $strm = $next
                continue 
            }

            if ($c -eq '|') {
                # We hit an existing stream.
                #      |           |
                #      x           x
                #    ||||||      |||||#
                #    |#~~#|  or  |#####
                #    |####|      |#    
                #
                # This stream merges into the existing one
                return
            }

            # We hit clay or water. It may be the bottom of a pit or a ledge
            #
            #      |         |         |       |        |    |
            #    # | #       x       # x       x      # x #||||
            #    # x #  or  ###  or  ####  or  #  or  #~~~~~~#|
            #    #####      # #         #      #      ########
            #
            # Flow left and right until we hit a pit wall and/or fall over ledge

            # Find the left edge...
            $left = $strm.Left()
            $isPit = $true
            while ($true) {
                if ($this.getGridItem($left) -eq '#') {
                    # pit wall
                    $left = $left.right()
                    break
                }
                if ($this.getGridItem($left.Below()) -eq '.') {
                    # ledge. spawn a new stream
                    $this.streams += @($left)
                    $isPit = $false
                    break
                }
                $left = $left.Left()
            }

            # Find the right edge...
            $right = $strm.right()
            while ($true) {
                if ($this.getGridItem($right) -eq '#') {
                    # pit wall
                    $right = $right.left()
                    break
                }
                if ($this.getGridItem($right.Below()) -eq '.') {
                    # ledge. spawn a new stream
                    $this.streams += @($right)
                    $isPit = $false
                    break
                }
                $right = $right.right()
            }

            if (!$isPit) {
                # We are overflowing. Fill with '|||||'
                foreach ($x in $left.X..$right.X) {
                    $this.setGridItem($x, $left.Y, '|')
                }

                # All done. The spawned streams will take it from here
                return
            }

            # We're in a pit. Fill with '~~~~', backup and try again
            #
            #      |          |    |
            #      | #      # x #||||
            #    # x #  or  #~~~~~~#|
            #    #####      ########
            #
            foreach ($x in $left.X..$right.X) {
                $this.setGridItem($x, $left.Y, '~')
            }

            $strm = $strm.Above()
        }
    }
}

function part1 {
    $in = input
    $puzzle = [Puzzle]::new($in)
    #Write-Host "Width x Depth = $($puzzle.width) x $($puzzle.depth) = $($puzzle.width * $puzzle.depth)"
    $puzzle.print()

    $puzzle.Go()

    $score = 0
    foreach ($y in $puzzle.minY..$puzzle.maxY) {
        $line = $puzzle.grid[$y]
        foreach ($x in 0..($puzzle.width - 1)) {
            if ('~|'.Contains($line[$x])) {
                ++$score
            }
        }
    }

    Write-Host "Part 1: Score = $score"
}    

function part2 {
    $in = input
    $puzzle = [Puzzle]::new($in)

    $puzzle.Go()

    $score = 0
    foreach ($y in $puzzle.minY..$puzzle.maxY) {
        $line = $puzzle.grid[$y]
        foreach ($x in 0..($puzzle.width - 1)) {
            if ('~' -eq $line[$x]) {
                ++$score
            }
        }
    }

    Write-Host "Part 2: Score = $score"
}

function input {

    $source = 
    'x=495, y=2..7
        y=7, x=495..501
        x=501, y=3..7
        x=498, y=2..4
        x=506, y=1..2
        x=498, y=10..13
        x=504, y=10..13
        y=13, x=498..504'

    $source = Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))

    $source -split "`r?`n" | foreach {
        if ($_ -notmatch '([xy])=(\d+), ([xy])=(\d+)\.\.(\d+)') { write-host $_; throw }
        [PSCustomObject]@{
            "min$($matches[1])" = [int]$matches[2]
            "max$($matches[1])" = [int]$matches[2]
            "min$($matches[3])" = [int]$matches[4]
            "max$($matches[3])" = [int]$matches[5]
        }
    }
}
#input
#return

main