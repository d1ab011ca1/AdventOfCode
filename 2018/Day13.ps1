# https://adventofcode.com/2018/day/13
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 83,106
    part2 #= 132,26
}

$Arrows = '^>v<'.ToCharArray()
$Directions = [System.Collections.Generic.Dictionary[char, int]]::new()
$Arrows.foreach{$Directions.Add($_, $Directions.Count)}

class Cart {
    [int]$X
    [int]$Y
    [int]$Dir
    [int]$TurnCount = 0

    Cart([int]$x, [int]$y, [int]$dir) {
        $this.X = $x
        $this.Y = $y
        $this.Dir = $Dir
    }

    [string]Move([string[]]$map) {
        # Move forward
        if ($this.Dir -eq 0) { --$this.Y } # Up
        if ($this.Dir -eq 1) { ++$this.X } # Right
        if ($this.Dir -eq 2) { ++$this.Y } # Down
        if ($this.Dir -eq 3) { --$this.X } # Left

        # Turn if needed
        $track = $map[$this.Y][$this.X]
        if ($track -eq '+') {
            if ($this.TurnCount % 3 -eq 0) { $this.TurnLeft() }
            elseif ($this.TurnCount % 3 -eq 2) { $this.TurnRight() }
            ++$this.TurnCount
        }
        elseif ($track -eq '\') {
            if ($this.Dir % 2 -eq 0) { $this.TurnLeft() } else { $this.TurnRight() }
        }
        elseif ($track -eq '/') {
            if ($this.Dir % 2 -eq 0) { $this.TurnRight() } else { $this.TurnLeft() }
        }

        return $this.getLocation()
    }

    [void]TurnRight() {
        $this.Dir = ($this.Dir + 1) % 4
    }
    [void]TurnLeft() {
        $this.Dir = ($this.Dir + 3) % 4
    }

    [string]getLocation()
    {
         return [Cart]::FormatLocation($this.X, $this.Y)
    }
    static [string]FormatLocation([int]$x, [int]$y)
    {
         return '{0},{1}' -f $x, $y
    }
}

function printMap([string[]]$map, [hashtable]$CartPositions) {
    $width = $map[0].Length
    foreach ($y in 0..($map.Count-1)) {
        Write-Host ('{0,3}  ' -f $y) -NoNewline -ForegroundColor DarkGray
        $last = 0
        foreach ($x in 0..($width-1)) {
            $location = [Cart]::FormatLocation($x, $y)
            if ($CartPositions.Contains($location)) {

                Write-Host $map[$y].Substring($last, $x - $last) -NoNewline
                $last = $x + 1

                $carts = $CartPositions[$location]
                if ($carts.Count -gt 1) {
                    Write-Host 'X' -ForegroundColor Red -NoNewline
                } else {
                    Write-Host $Arrows[$carts[0].Dir] -ForegroundColor Green -NoNewline
                }
            } else {
            }
        }
        Write-Host $map[$y].Substring($last)
    }
}

function part1 {
    $source = `
'/->-\        
|   |  /----\
| /-+--+-\  |
| | |  | v  |
\-+-/  \-+--/
  \------/   '
    
    $source = Get-Content -Path ([system.io.path]::ChangeExtension($PSCmdlet.MyInvocation.MyCommand, 'txt'))
    $in = parseInput $source

    $map = $in.Map
    $carts = $in.Carts
    #$map | Write-Host
    #$carts | Out-String | Write-Host

    $cartPositions = @{}
    foreach ($cart in $carts) {
        $cartPositions[$cart.getLocation()] += @($cart)
    }
    #printMap $map $cartPositions
    
    $moves = 0
    while ($true) {
        ++$moves
        foreach ($cart in $carts | sort Y,X) {
            $cartPositions.Remove($cart.getLocation())

            $newLocation = $cart.Move($map)

            $collisions = $cartPositions[$newLocation] += @($cart)
            if ($collisions.Count -gt 1) {
                # Collision!
                printMap $map $cartPositions
                $collisions
                Write-Host "Collision at $newLocation during move $moves."
                return
            }
        }

        #printMap $map $cartPositions
    }
}

function part2 {
    $source = `
'/>-<\  
|   |  
| /<+-\
| | | v
\>+</ |
  |   ^
  \<->/'
    
    $source = Get-Content -Path ([system.io.path]::ChangeExtension($PSCmdlet.MyInvocation.MyCommand, 'txt'))
    $in = parseInput $source

    $map = $in.Map
    $carts = $in.Carts

    $cartPositions = @{}
    foreach ($cart in $carts) {
        $cartPositions[$cart.getLocation()] += @($cart)
    }
    #printMap $map $cartPositions
    Write-Host "Original number of carts: $($carts.Count)"

    $moves = 0
    while ($carts.Count -gt 1) {
        ++$moves
        $carts = $carts | sort Y,X
        for ($n = 0; $n -lt $carts.Count; $n++) {
            $cart = $carts[$n]
            if ($null -eq $cart) {
                continue # was removed due to collision
            }
            $cartPositions.Remove($cart.getLocation())

            $newLocation = $cart.Move($map)

            $collisions = $cartPositions[$newLocation] += @($cart)
            if ($collisions.Count -gt 1) {
                # Collision!
                #printMap $map $cartPositions
                Write-Host "Collision at $newLocation during move $moves. Removing."

                # remove collisions
                foreach ($otherCart in $collisions) {
                    for ($j = 0; $j -lt $carts.Count; $j++) {
                        if ($carts[$j] -eq $otherCart) {
                            $carts[$j] = $null
                            break
                        }
                    }
                }
                $cartPositions.Remove($newLocation)
            }
        }

        # remove collisions now that we're outside the loop
        $carts = $carts.Where{$null -ne $_}

        #printMap $map $cartPositions
        Write-Host "Move $moves complete. Carts remaining: $($carts.Count)"
    }
    $carts
}

function parseInput([string[]]$source) {

    $map = $source -split "`r?`n"
    $carts = for ($y = 0; $y -lt $map.Count; $y++) {
        $line = $map[$y]
        for ($startAt = 0; $startAt -lt $line.Length; ) {
            $x = $line.IndexOfAny($Arrows, $startAt)
            if ($x -eq -1) { break }

            $dir = $Directions[$line[$x]]
            [Cart]::new($x, $y, $dir)

            # replace the cart with track
            $map[$y] = $line = $line.Substring(0, $x) + '|-|-'[$dir] + $line.Substring($x + 1)
            
            $startAt = $x + 1
        }
    }
    
    [PSCustomObject]@{
        Map = $map
        Carts = $carts
    }
}
#input
#return

main
