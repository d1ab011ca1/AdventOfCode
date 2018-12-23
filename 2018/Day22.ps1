# https://adventofcode.com/2018/day/18
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 5637
    part2 #= 
}

class Row : System.Collections.ArrayList {
    Row() {}
    Row([int] $width) : base($width) {
        foreach ($x in 1..$width) {
            $this.Add(-1L)
        }
    }
}

class Grid : System.Collections.ArrayList {
    Grid() {}
    Grid([int] $width, [int] $height) : base($height) {
        foreach ($y in 1..$height) {
            $this.Add([Row]::new($width))
        }
    }
}

class Puzzle {
    [int] $Depth
    [int] $TargetX
    [int] $TargetY
    [Grid] $Grid  # Contains erosion levels of each area

    Puzzle([int]$depth, [int]$targetX, [int]$targetY) {

        $this.Depth = $depth
        $this.TargetX = $targetX
        $this.TargetY = $targetY
        $this.Grid = [Grid]::new(1000, 1000)

        # Pre-calculate the erosion level of boundary areas
        # A region's erosion level is its geologic index plus the cave system's depth, all modulo 20183.

        # The region at 0,0 (the mouth of the cave) has a geologic index of 0.
        # The region at the coordinates of the target has a geologic index of 0.
        # If the region's Y coordinate is 0, the geologic index is its X coordinate times 16807.
        # If the region's X coordinate is 0, the geologic index is its Y coordinate times 48271.
        # Otherwise, the region's geologic index is the result of multiplying the erosion levels of the regions at X-1,Y and X,Y-1.

        # Assign hardcoded values in reverse priority
        foreach ($y in 1..($this.Grid.Count - 1)) {
            $gi = $y * 48271
            $this.Grid[$y][0] = ($gi + $this.Depth) % 20183
        }
        foreach ($x in 0..($this.Grid[0].Count - 1)) {
            $gi = $x * 16807
            $this.Grid[0][$x] = ($gi + $this.Depth) % 20183
        }
        $this.Grid[$this.TargetX][$this.TargetY] = (0 + $this.Depth) % 20183
        $this.Grid[0][0] = (0 + $this.Depth) % 20183

        # All other will be calculated as needed. See getErosionLevel()
    }

    # A region's erosion level is its geologic index plus the cave system's depth, all modulo 20183. Then:
    #  - If the erosion level modulo 3 is 0, the region's type is rocky.
    #  - If the erosion level modulo 3 is 1, the region's type is wet.
    #  - If the erosion level modulo 3 is 2, the region's type is narrow.
    [long] getErosionLevel([int]$x, [int]$y) {
        # avoid unnecessary calls to PS methods since they are slow and this is recursive!!!
        $el = $this.Grid[$y][$x]
        if ($el -eq -1) {
            $el = $this.calcErosionLevel($x, $y)
        }
        return $el
    }
    [long] calcErosionLevel([int]$x, [int]$y) {
        # A region's erosion level is its geologic index plus the cave system's depth, all modulo 20183.
        # The region's geologic index is the result of multiplying the erosion levels of the regions at X-1,Y and X,Y-1.

        # Avoid unnecessary calls to PS methods since they are slow and this is recursive!!!
        $elA = $this.Grid[$y][$x-1]
        if ($elA -eq -1) {
            $elA = $this.calcErosionLevel($x-1, $y)
        }

        $elB = $this.Grid[$y-1][$x]
        if ($elB -eq -1) {
            $elB = $this.calcErosionLevel($x, $y-1)
        }

        $gi = $elA * $elB
        return $this.Grid[$y][$x] = ($gi + $this.Depth) % 20183
    }

    [void] Print([int]$maxX, [int]$maxY) {
        # Origin: M
        # Target: T
        # Rocky:  .
        # Wet:    =
        # Narrow: |
        $sb = [System.Text.StringBuilder]::new($maxX + 1)
        foreach ($y in 0..$maxY) {
            Write-Host ('{0,2} ' -f $y) -NoNewline -ForegroundColor DarkGray
            $null = $sb.Clear()
            foreach ($x in 0..$maxX) {
                if ($x -eq 0 -AND $y -eq 0) {
                    $null = $sb.Append('M')
                }
                elseif ($x -eq $this.TargetX -AND $y -eq $this.TargetY) {
                    $null = $sb.Append('T')
                }
                else {
                    $null = $sb.Append('.=|'[$this.getErosionLevel($x, $y) % 3])
                }
            }

            Write-Host $sb.ToString()
        }
        Write-Host
    }
    [void] PrintTool([string]$tool, [int]$maxX, [int]$maxY) {
        # Origin: M
        # Target: T
        # Rocky:  .
        # Wet:    =
        # Narrow: |
        if ($tool -eq 'torch') {
            # Torch can be used in rocky or narrow areas
            $allowedAreas = '.|'
            $alternativeTool = 'C-N'
        } elseif ($tool -eq 'climbing gear') {
            # Climbing gear can be used in rocky or wet areas
            $allowedAreas = '.='
            $alternativeTool = 'TN-'
        } elseif ($tool -eq 'neither') {
            # Neither can be used in wet or narrow areas
            $allowedAreas = '=|'
            $alternativeTool = '-CT'
        } else {
            throw 'Unknown tool.'
        }

        Write-Host $tool -ForegroundColor Blue
        $sb = [System.Text.StringBuilder]::new($maxX + 1)
        foreach ($y in 0..$maxY) {
            Write-Host ('{0,2} ' -f $y) -NoNewline -ForegroundColor DarkGray
            $null = $sb.Clear()
            foreach ($x in 0..$maxX) {
                $areaId = $this.getErosionLevel($x, $y) % 3
                $area = '.=|'[$areaId]
                if (!$allowedAreas.Contains($area)) {
                    $null = $sb.Append(' ')
                }
                elseif ($x -eq 0 -AND $y -eq 0) {
                    $null = $sb.Append('M')
                }
                elseif ($x -eq $this.TargetX -AND $y -eq $this.TargetY) {
                    $null = $sb.Append('T')
                }
                else {
                    $null = $sb.Append($alternativeTool[$areaId])
                }
            }

            Write-Host $sb.ToString()
        }
        Write-Host
    }

    [int] ComputeRisk([int]$maxX, [int]$maxY) {
        # Rocky:  0
        # Wet:    1
        # Narrow: 2
        $risk = 0
        foreach ($y in 0..$maxY) {
            foreach ($x in 0..$maxX) {
                if ($x -eq 0 -AND $y -eq 0) {
                    # Origin
                }
                elseif ($x -eq $this.TargetX -AND $y -eq $this.TargetY) {
                    # Target
                }
                else {
                    $risk += $this.getErosionLevel($x, $y) % 3
                }
            }

        }
        return $risk
    }
}

function part1 {
    $depth, $targetX, $targetY = input
    $puzzle = [Puzzle]::new($depth, $targetX, $targetY)

    $risk = $puzzle.ComputeRisk($puzzle.TargetX, $puzzle.TargetY)

    $puzzle.Print($puzzle.TargetX + 1, $puzzle.TargetY + 1)
    Write-Host "Part 1: Risk is $risk."
}    

function part2 {
    $depth, $targetX, $targetY = input
    $puzzle = [Puzzle]::new($depth, $targetX, $targetY)
    $puzzle.Print($puzzle.TargetX + 3, $puzzle.TargetY + 3)
    $puzzle.PrintTool('torch', $puzzle.TargetX + 3, $puzzle.TargetY + 3)
    $puzzle.PrintTool('climbing gear', $puzzle.TargetX + 3, $puzzle.TargetY + 3)
    $puzzle.PrintTool('neither', $puzzle.TargetX + 3, $puzzle.TargetY + 3)

    $elapsed = 0#$puzzle.ComputeQuickestPath($puzzle.TargetX, $puzzle.TargetY)

    Write-Host "Part 2: Fewest minutes to reach target is $elapsed."
}

function input {
    #      depth, targetX targetY
    return   510,   10,      10
    return 11394,    7,     701
}
#input
#return

main