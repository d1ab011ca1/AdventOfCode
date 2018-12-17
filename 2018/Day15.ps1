# https://adventofcode.com/2018/day/15
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 81 * 2690 = 217890
    part2 #= 29 * 1505 = 43645 with attack power 25.
}

function stableSort {
    [CmdletBinding(PositionalBinding = $false)]
    param(
        [Parameter(ValueFromPipeline)]
        [object[]]$InputObject,
        [Parameter(Position = 0)]
        [object[]]$Property,
        [switch]$Descending,
        [switch]$CaseSensitive
    )
    begin {
        if ($PSBoundParameters.ContainsKey('Property')) {
            foreach ($p in $PSBoundParameters['Property']) {
                if ($p -isnot [string]) {
                    throw 'Currently only supports Property names.'
                }
            } 
        }

        [object[]]$list = @()
    }
    process {
        $n = $list.Count
        $list += foreach ($o in $InputObject) {
            [PSCustomObject]@{ Object = $o; RelativeOrder = $n++ } 
        }
    }
    end {
        if (!$PSBoundParameters.ContainsKey('Property')) {
            $PSBoundParameters['Property'] = @('Object')
        }
        else {
            $PSBoundParameters['Property'] = @(foreach ($p in $PSBoundParameters['Property']) {
                    [scriptblock]::Create("`$_.Object.$p")
                })
        }
        $PSBoundParameters['Property'] += @('RelativeOrder')

        $null = $PSBoundParameters.Remove('InputObject')
        $list | Sort-Object @PSBoundParameters | select -ExpandProperty 'Object'
    }
}

class Location : System.IComparable {
    [int] $X
    [int] $Y

    Location() {
    }
    Location([int]$x, [int]$y) {
        $this.X = $x
        $this.Y = $y
    }

    [int] ManhattanDistance([Location]$other) {
        return [math]::Abs($this.X - $other.X) + [math]::Abs($this.Y - $other.Y)
    }
    [Location] Copy() { return [Location]::new($this.X, $this.Y) }
    [Location] Above() { return [Location]::new($this.X, $this.Y - 1) }
    [Location] Below() { return [Location]::new($this.X, $this.Y + 1) }
    [Location] Left() { return [Location]::new($this.X - 1, $this.Y) }
    [Location] Right() { return [Location]::new($this.X + 1, $this.Y) }

    #[string] ToString() {
    #    return '({0},{1})' -f $this.X, $this.Y
    #}
    [int] GetHashCode() {
        return $this.X.GetHashCode() -bxor $this.Y.GetHashCode()
    }
    [bool] Equals([object]$obj) {
        if ($obj.GetType() -ne [Location]) { return $false }
        return $this.CompareTo($obj) -eq 0
    }
    [int] CompareTo([object]$object) {
        if ($null -eq $object) { return 1 }
        [Location]$other = $object
        if ($null -eq $other) { throw [System.ArgumentException]'Object is not a Location' }
        $diff = $this.Y.CompareTo($other.Y)
        if ($diff -eq 0) { $diff = $this.X.CompareTo($other.X) }
        return $diff
    }
}

class Unit {
    [char] $Type
    [Location] $Location
    [int] $HitPoints = 200

    Unit([char]$type, [int]$x, [int]$y) {
        $this.Type = $type
        $this.Location = [Location]::new($x, $y)
    }

    [bool] IsElf() {
        return $this.Type -eq 'E'
    }
    [bool] IsGoblin() {
        return $this.Type -eq 'G'
    }

    [bool] IsDead() {
        return $this.HitPoints -le 0
    }

    #[string] ToString() {
    #    return "[Type=$($this.Type), Location=$($this.Location), HitPoints=$($this.HitPoints)]"
    #}
}

class Puzzle {
    [System.Text.StringBuilder[]] $Grid = @()
    [System.Collections.SortedList] $Units = [System.Collections.SortedList]::new()

    static [Puzzle] Parse([string[]]$source) {
        $puzzle = [Puzzle]::new()

        $puzzle.Grid = ($source -split "`r?`n").ForEach{ [System.Text.StringBuilder]::new($_.Trim()) }

        foreach ($y in 0..($puzzle.Grid.Count - 1)) {
            $line = $puzzle.Grid[$y]
            foreach ($x in 0..($line.Length - 1)) {
                if ('EG'.Contains($line[$x])) { 
                    $unit = [Unit]::new($line[$x], $x, $y)
                    $puzzle.Units[$unit.Location] = $unit
                }
            }
        }
        return $puzzle
    }

    [char] getGridItem([Location]$location) {
        return $this.Grid[$location.y][$location.x]
    }
    [void] setGridItem([Location]$location, [char]$c) {
        $this.Grid[$location.y][$location.x] = $c
    }

    # define some constants
    [int] $kAbove = 0
    [int] $kRight = 1
    [int] $kBelow = 2
    [int] $kLeft = 3

    # Returns all adjacent items in clockwise order starting at top
    [char[]] getAdjacentGridItem([Location]$loc) {
        return @(
            $this.Grid[$loc.y - 1][$loc.x] # $kAbove
            $this.Grid[$loc.y][$loc.x + 1] # $kRight
            $this.Grid[$loc.y + 1][$loc.x] # $kBelow
            $this.Grid[$loc.y][$loc.x - 1] # $kLeft
        )
    }

    [void] print() {
        
        $height = $this.Grid.Count
        $width = $this.Grid[0].Length
        
        # units are sorted in reading/printing order
        $x, $y = 0, 0
        $unitsInLine = $null
        foreach ($unit in $this.Units.Values) {
            $unitX = $unit.Location.X
            $unitY = $unit.Location.Y

            # print the area between units
            if ($y -lt $unitY) {
                if ($x -gt 0) {
                    # print end of line
                    Write-Host $this.Grid[$y].ToString($x, $width - $x) -NoNewline
                    Write-Host '    ' -NoNewline
                    Write-Host ($unitsInLine.Foreach{'{0} ({1})' -f $_.Type, $_.HitPoints} -join ', ')
                    $unitsInLine = $null
                    ++$y
                    $x = 0
                }
                # print full lines
                for (; $y -lt $unitY; ++$y) {
                    Write-Host ('{0,3}  ' -f $y) -NoNewline -ForegroundColor DarkGray
                    Write-Host $this.Grid[$y].ToString()
                }
            }
            if ($x -eq 0) { Write-Host ('{0,3}  ' -f $y) -NoNewline -ForegroundColor DarkGray }
            if ($x -lt $unitX) {
                # print beginning of line
                Write-Host $this.Grid[$y].ToString($x, $unitX - $x) -NoNewline
                $x = $unitX
            }

            $color = if ($unit.Type -eq 'E') {'Green'} else {'Red'} 
            Write-Host $unit.Type -NoNewline -ForegroundColor $color
            $unitsInLine += @($unit)
            ++$x
        }
        # print remainder
        if ($x -gt 0) {
            # print end of line
            Write-Host $this.Grid[$y].ToString($x, $width - $x) -NoNewline
            Write-Host '    ' -NoNewline
            Write-Host ($unitsInLine.Foreach{'{0} ({1})' -f $_.Type, $_.HitPoints} -join ', ')
            ++$y
            $x = 0
        }
        for (; $y -lt $height; ++$y) {
            Write-Host ('{0,3}  ' -f $y) -NoNewline -ForegroundColor DarkGray
            Write-Host $this.Grid[$y].ToString()
        }
        Write-Host ''
    }

    [void] play($elfAttackPower) {

        Write-Host ''
        Write-Host "Starting game with Elf attack power ${elfAttackPower}." -ForegroundColor Blue
        $this.print()
        
        $height = $this.Grid.Count
        $width = $this.Grid[0].Length
        
        $round = 0
        while ($true) {
            ++$round

            $players = @($this.Units.Values)
            foreach ($unit in $players) {

                # Ignore dead/killed units
                if ($unit.IsDead()) {
                    continue
                }

                $targetType = if ($unit.IsElf()) {'G'} else {'E'}
                $adjacent = $this.getAdjacentGridItem($unit.Location)
                if ($adjacent -contains $targetType) {
                    # Will attack (see below)
                }
                elseif ($adjacent -contains '.') {
                    # Move

                    # find all targets
                    $targets = $this.Units.Values.where{$_.Type -eq $targetType}

                    # find the open spaces around the targets...
                    $inRange = foreach ($target in $targets) {
                        $candidates = @()
                        $targetAdjacent = $this.getAdjacentGridItem($target.Location)
                        if ($targetAdjacent[$this.kAbove] -eq '.') { $candidates += @($target.Location.Above()) }
                        if ($targetAdjacent[$this.kLeft] -eq '.') { $candidates += @($target.Location.Left()) }
                        if ($targetAdjacent[$this.kRight] -eq '.') { $candidates += @($target.Location.Right()) } 
                        if ($targetAdjacent[$this.kBelow] -eq '.') { $candidates += @($target.Location.Below()) }
                        $candidates.forEach{
                            [PSCustomObject]@{ Location = $_; Distance = $_.ManhattanDistance($unit.Location); } 
                        }
                    }

                    # Find the shorest distance to any cell in range...
                    # This is by far the most expensive part of this algorithm
                    $minDistance = [int]::MaxValue
                    $nextLocation = $null
                    $nextLocations = @($unit.Location.Above(), $unit.Location.Left(), $unit.Location.Right(), $unit.Location.Below())
                    # Sort by distance so we can break out of the loop ASAP.
                    foreach ($cell in $inRange | stableSort Distance) {
                        if ($cell.Distance -gt $minDistance) {
                            break
                        }

                        $distanceMap = $this.getDistanceMap($cell.Location)

                        foreach ($next in $nextLocations) {
                            if ($distanceMap.ContainsKey($next)) {
                                $distance = $distanceMap[$next]
                                if ($distance -lt $minDistance) {
                                    $minDistance = $distance
                                    $nextLocation = $next
                                }
                            }
                        }
                    }

                    if ($null -ne $nextLocation) {
                        # move
                        $this.Units.Remove($unit.Location)
                        $this.setGridItem($unit.Location, '.')
                        $unit.Location = $nextLocation
                        $this.setGridItem($unit.Location, $unit.Type)
                        $this.Units.Add($unit.Location, $unit)
                        
                        #$this.print()

                        $adjacent = $this.getAdjacentGridItem($unit.Location)
                    }
                }    
                
                if ($adjacent -contains $targetType) {
                    # Attack!!!

                    $targets = @()
                    # add targets in reading order in case of tie
                    if ($adjacent[$this.kAbove] -eq $targetType) { $targets += @($this.Units[$unit.Location.Above()]) }
                    if ($adjacent[$this.kLeft] -eq $targetType) { $targets += @($this.Units[$unit.Location.Left()]) }
                    if ($adjacent[$this.kRight] -eq $targetType) { $targets += @($this.Units[$unit.Location.Right()]) }
                    if ($adjacent[$this.kBelow] -eq $targetType) { $targets += @($this.Units[$unit.Location.Below()]) }
                    $target = $targets | stableSort HitPoints | select -First 1

                    # do damage
                    if ($unit.IsElf()) {
                        $target.HitPoints -= $elfAttackPower
                    }
                    else {
                        $target.HitPoints -= 3
                    }
                    if ($target.IsDead()) {

                        if ($target.IsElf() -AND $elfAttackPower -ge 4) {
                            $winner = if ($unit.IsElf()) {'Elves'} else {'Goblins'}
                            $hitpoints = ($this.Units.Values | measure -Sum HitPoints).Sum
                            $this.print()
                            Write-Host "Game over. An elf died during round $round with attack power $elfAttackPower."
                            return
                        }

                        # Remove target from puzzle
                        $null = $this.Units.Remove($target.Location)
                        $this.setGridItem($target.Location, '.')

                        # Was that the last target?
                        if (!$this.Units.Values.where{$_.Type -eq $targetType}) {

                            $fullRounds = $round - 1
                            if ($unit -eq $players[-1]) {
                                ++$fullRounds
                            }

                            $winner = if ($unit.IsElf()) {'Elves'} else {'Goblins'}
                            $hitpoints = ($this.Units.Values | measure -Sum HitPoints).Sum
                            $this.print()
                            Write-Host "Combat ends after $fullRounds full rounds"
                            Write-Host "$winner win with $hitpoints total hit points left"
                            Write-Host "Outcome: $fullRounds * $hitpoints = $($fullRounds * $hitpoints)"

                            if ($unit.IsElf() -AND $elfAttackPower -ge 4) {
                                Write-Host "No Elfs died with attack power $elfAttackPower." -ForegroundColor Green
                                Exit
                            }
                            return
                        }
                        
                        #$this.print()
                    }
                }
            }

            Write-Host "After ${round} rounds:" -ForegroundColor Blue
            $this.print()
        }
    }

    [hashtable] getDistanceMap([Location]$location) {
        $queue = [System.Collections.Queue]::new()
        $distanceMap = @{$location = 0}
        $queue.Enqueue($location)
        while ($queue.Count) {
            [Location] $cell = $queue.Dequeue()
            [int] $dist = $distanceMap[$cell]
            
            foreach ($next in ($cell.Above(), $cell.Left(), $cell.Right(), $cell.Below())) {
                if ($this.getGridItem($next) -eq '.' -AND !$distanceMap.ContainsKey($next)) {
                    $distanceMap[$next] = $dist + 1
                    $queue.Enqueue($next)
                }
            }
        }

        #$print = $false
        #if ($print) {
        #    $height = $this.Grid.Count
        #    $width = $this.Grid[0].Length
        #    foreach ($y in 0..($height - 1)) {
        #        $line = [System.Text.StringBuilder]::new($this.Grid[$y].ToString())
        #        foreach ($x in 0..($width - 1)) {
        #            if ($line[$x] -ne '#') {
        #                $line[$x] = $distanceMap[[Location]::new($x, $y)].ToString().ToCharArray()[-1]
        #            }
        #        }
        #        Write-Host $line.ToString()
        #    }
        #}
    
        return $distanceMap
    }
}   

function part1 {
    $source =
    '#########
    #G..G..G#
    #.......#
    #.......#
    #G..E..G#
    #.......#
    #.......#
    #G..G..G#
    #########'

    $source =
    '#######   
    #.G...#
    #...EG#
    #.#.#G#
    #..G#E#
    #.....#   
    #######'

    $source =
    '#######
    #G..#E#
    #E#E.E#
    #G.##.#
    #...#E#
    #...E.#
    #######'

    $source = 
    '#######
    #E..EG#
    #.#G.E#
    #E.##E#
    #G..#.#
    #..E#.#
    #######'

    $source = 
    '#######
    #E.G#.#
    #.#G..#
    #G.#.G#
    #G..#.#
    #...E.#
    #######'

    $source = 
    '#######
    #.E...#
    #.#..G#
    #.###.#
    #E#G#G#
    #...#G#
    #######'

    $source =
    '#########
    #G......#
    #.E.#...#
    #..##..G#
    #...##..#
    #...#...#
    #.G...G.#
    #.....G.#
    #########'

    $source = Get-Content -Path ([system.io.path]::ChangeExtension($PSCmdlet.MyInvocation.MyCommand, 'txt'))
    $puzzle = [Puzzle]::parse($source)
    $puzzle.play(3)
}

function part2 {
    $source =
    '#######   
    #.G...#
    #...EG#
    #.#.#G#
    #..G#E#
    #.....#   
    #######'

    $source = 
    '#######
    #E..EG#
    #.#G.E#
    #E.##E#
    #G..#.#
    #..E#.#
    #######'

    $source = 
    '#######
    #E.G#.#
    #.#G..#
    #G.#.G#
    #G..#.#
    #...E.#
    #######'

    $source = 
    '#######
    #.E...#
    #.#..G#
    #.###.#
    #E#G#G#
    #...#G#
    #######'

    $source =
    '#########
    #G......#
    #.E.#...#
    #..##..G#
    #...##..#
    #...#...#
    #.G...G.#
    #.....G.#
    #########'

    $source = Get-Content -Path ([system.io.path]::ChangeExtension($PSCmdlet.MyInvocation.MyCommand, 'txt'))
    
    foreach ($elfAttackPower in 25..25) {
        $puzzle = [Puzzle]::parse($source)
        $puzzle.Play($elfAttackPower)
        # will exit script when solution found
    }
}

main
