# https://adventofcode.com/2018/day/15
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    part1 #= 
    #part2 #= 
}

class Location : System.IComparable {
    [int]$X
    [int]$Y

    Location() {
    }
    Location([int]$x, [int]$y) {
        $this.X = $x
        $this.Y = $y
    }

    [Location]Copy() { return [Location]::new($this.X, $this.Y) }
    [Location]Up([int]$amount = 1) { return [Location]::new($this.X, $this.Y - $amount) }
    [Location]Down([int]$amount = 1) { return [Location]::new($this.X, $this.Y + $amount) }
    [Location]Left([int]$amount = 1) { return [Location]::new($this.X - $amount, $this.Y) }
    [Location]Right([int]$amount = 1) { return [Location]::new($this.X + $amount, $this.Y) }

    [string]ToString() {
        return '({0},{1})' -f $this.X, $this.Y
    }
    [int]GetHashCode() {
        return $this.X.GetHashCode() -bxor $this.Y.GetHashCode()
    }
    [bool]Equals([object]$obj) {
        if ($obj.GetType() -ne [Location]) { return $false }
        return $this.CompareTo($obj) -eq 0
    }
    [int]CompareTo([object]$object){
        if ($null -eq $object) { return 1 }
        [Location]$other = $object
        if ($null -eq $other) { throw [System.ArgumentException]'Object is not a Location' }
        $diff = $this.Y.CompareTo($other.Y)
        if ($diff -eq 0) { $diff = $this.X.CompareTo($other.X) }
        return $diff
    }
}

class Unit {
    [Location]$Location
    [bool]$Dead = $false

    Unit([int]$x, [int]$y) {
        $this.Location = [Location]::new($x, $y)
    }
}

class Elf : Unit {
    Elf([int]$x, [int]$y) : base($x, $y) {}
}

class Goblin : Unit {
    Goblin([int]$x, [int]$y) : base($x, $y) {}
}

class Puzzle {
    [string[]]$Map = @()
    [Unit[]]$Units = @()
    [hashtable]$UnitPositions = @{}

    static [Puzzle] Parse([string[]]$source) {
        $puzzle = [Puzzle]::new()

        $puzzle.Map = $source -split "`r?`n"

        foreach ($y in 0..($puzzle.Map.Count-1)) {
            $line = $puzzle.Map[$y]
            for ($startAt = 0; $startAt -lt $line.Length; ) {
                $x = $line.IndexOfAny('EG'.ToCharArray(), $startAt)
                if ($x -eq -1) { break }
    
                if ($line[$x] -eq 'E') {
                    $unit = [Elf]::new($x, $y)
                } else {
                    $unit = [Goblin]::new($x, $y)
                }
                $puzzle.Units += @($unit)
                $puzzle.UnitPositions[$unit.Location] = $unit

                # replace the unit with ground
                $puzzle.Map[$y] = $line = $line.Substring(0, $x) + '.' + $line.Substring($x + 1)
                
                $startAt = $x + 1
            }
        }
        return $puzzle
    }

    [char] getMapItem([Location]$location) {
        return $this.Map[$location.y][$location.x]
    }
    [char] getMapItem([int]$x, [int]$y) {
        return $this.Map[$y][$x]
    }

    [void] printMap() {
        $width = $this.Map[0].Length
        foreach ($y in 0..($this.Map.Count-1)) {
            Write-Host ('{0,3}  ' -f $y) -NoNewline -ForegroundColor DarkGray
            $last = 0
            foreach ($x in 0..($width-1)) {
                $location = [Location]::new($x, $y)
                if ($this.UnitPositions.Contains($location)) {
    
                    Write-Host $this.Map[$y].Substring($last, $x - $last) -NoNewline
                    $last = $x + 1
    
                    if ($this.UnitPositions[$location] -is [Elf]) {
                        Write-Host 'E' -ForegroundColor Green -NoNewline
                    } else {
                        Write-Host 'G' -ForegroundColor Red -NoNewline
                    }
                }
            }
            Write-Host $this.Map[$y].Substring($last)
        }
    }

}    

function part1 {
    $source = `
'#########
#G..G..G#
#.......#
#.......#
#G..E..G#
#.......#
#.......#
#G..G..G#
#########'
    
    #$source = Get-Content -Path ([system.io.path]::ChangeExtension($PSCmdlet.MyInvocation.MyCommand, 'txt'))
    $puzzle = [Puzzle]::parse($source)
    $puzzle.printMap()
return

    $round = 1
    while ($true) {
        $elfs = $puzzle.Units.where{$_ -is [Elf]}
        $goblins = $puzzle.Units.where{$_ -is [Goblin]}
        foreach ($unit in $puzzle.Units) {
            if ($unit.Dead) { continue }
            $isElf = $unit -is [Elf]
            $targets = if ($isElf) { $goblins } else { $elfs }
            if (!$targets) {
                if ($isElf) {'Elfs won!'} else {'Goblins won!'}
                return
            }

            $openUnits = foreach ($target in $targets) {
                $left = $target.Location.Left()
                $right = $target.Location.Right()
                $up = $target.Location.Up()
                $down = $target.Location.Down()
                if ($puzzle.getMapItem($left) -eq '.') { $left }
                if ($puzzle.getMapItem($right) -eq '.') { $right }
                if ($puzzle.getMapItem($up) -eq '.') { $up }
                if ($puzzle.getMapItem($down) -eq '.') { $down }
            }
            $openUnits = $openUnits | sort {}

            $puzzle.UnitPositions.Remove($unit.Location)
            $puzzle.move($unit)
            $puzzle.UnitPositions.Add($unit.Location, $unit)
        }

        $puzzle.Units = $puzzle.Units.where{!$_.Dead} | sort Y,X
        ++$round
        #$puzzle.printMap()
    }
}

function part2 {
}

main
