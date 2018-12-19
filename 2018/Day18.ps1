# https://adventofcode.com/2018/day/18
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    part1 #= Wooded areas (769) * Lumber yards (644) = 495236
    part2 #= 201348
}

class Puzzle {
    [System.Text.StringBuilder[]]$grid
    [System.Text.StringBuilder[]] hidden $nextGrid
    [int]$width
    [int]$height

    Puzzle([string[]]$map) {
        $this.height = $map.Count
        $this.width = $map[0].Length

        # padded all around with 1 space
        $emptyLine = [System.Text.StringBuilder]::new(' ' * ($this.width + 2))
        $this.grid = @($emptyLine) + @($map.foreach{ [System.Text.StringBuilder]::new(' ' + $_ + ' ') }) + @($emptyLine)
        $this.nextGrid = $this.Grid.foreach{ [System.Text.StringBuilder]::new($emptyLine) }
    }

    [void] Print() {
        # print X axis (ignore the padding)
        if ($this.width -gt 10) {
            $line = '   '
            foreach ($x in 0..([math]::floor($this.width / 10) - 1)) { $line += "$x         " }
            Write-Host $line -ForegroundColor Gray
        }
        Write-Host ('   ' + '0123456789' * [math]::floor($this.width / 10) + '0123456789'.Substring(0, $this.width % 10)) -ForegroundColor DarkGray

        # skip the padding
        foreach ($y in 1..$this.height) {
            Write-Host ('{0,2}' -f ($y - 1)) -NoNewline -ForegroundColor DarkGray
            Write-Host $this.grid[$y]
        }
        Write-Host
    }

    [void] Play($rounds) {

        foreach ($round in 1..$rounds) {
            # fill next grid...
            foreach ($y in 1..$this.height) {
                $yAbove = $this.Grid[$y - 1]
                $yCenter = $this.Grid[$y + 0]
                $yBelow = $this.Grid[$y + 1]

                $nextLine = $this.nextGrid[$y]
                foreach ($x in 1..$this.width) {
                    $cur = $this.Grid[$y][$x]
                    $adjacentTrees = $adjacentLumberyards = 0
                    @(
                        $yAbove[$x - 1], $yAbove[$x], $yAbove[$x + 1],
                        $yCenter[$x - 1], $yCenter[$x + 1]
                        $yBelow[$x - 1], $yBelow[$x], $yBelow[$x + 1]
                    ).foreach{
                        if ($_ -eq '|') { ++$adjacentTrees }
                        elseif ($_ -eq '#') { ++$adjacentLumberyards }
                    }
                                
                    $nextLine[$x] = $cur
                    if ($cur -eq '.') {
                        # An open acre will become filled with trees if three or more adjacent acres contained trees. Otherwise, nothing happens.
                        if ($adjacentTrees -ge 3) {
                            $nextLine[$x] = '|'
                        }
                    }
                    elseif ($cur -eq '|') {
                        # An acre filled with trees will become a lumberyard if three or more adjacent acres were lumberyards. Otherwise, nothing happens.
                        if ($adjacentLumberyards -ge 3) {
                            $nextLine[$x] = '#'
                        }
                    }
                    elseif ($cur -eq '#') {
                        # An acre containing a lumberyard will remain a lumberyard if it was adjacent to at least one other lumberyard and at least one acre containing trees. Otherwise, it becomes open.
                        if (!($adjacentLumberyards -ge 1 -and $adjacentTrees -ge 1)) {
                            $nextLine[$x] = '.'
                        }
                    }
                    else {
                        throw "Unexpected character at ($x,$y): '$cur'."
                    }
                }
            }
    
            # update grid
            $oldGrid = $this.Grid
            $this.Grid = $this.nextGrid
            $this.nextGrid = $oldGrid
        }
    }

    [PSCustomObject] Score() {
        $tally = [PSCustomObject]@{ Score = 0; Woods = 0; Lumberyards = 0 }
        foreach ($y in 1..$this.height) {
            $line = $this.Grid[$y]
            foreach ($x in 1..$this.width) {
                if ($line[$x] -eq '|') { ++$tally.Woods }
                elseif ($line[$x] -eq '#') { ++$tally.Lumberyards }
            }
        }
        $tally.Score = $tally.Woods * $tally.Lumberyards
        return $tally
    }
}

function part1 {
    $in = input
    $puzzle = [Puzzle]::new($in)
    $puzzle.Print()

    $puzzle.Play(10)
    $score = $puzzle.Score()

    $puzzle.Print()
    Write-Host "Part 1: Wooded areas ($($score.Woods)) * Lumber yards ($($score.Lumberyards)) = $($score.Score)"
}    

function part2 {
    Write-Host "Part 2"
    $in = input
    $puzzle = [Puzzle]::new($in)

    $results = foreach ($round in 1..600) {
        $puzzle.Play(1)
        $score = $puzzle.Score()
        #Write-Host ('{0,6}: {1,7} {2,4} {3,4}' -f $round, $score.Score, $score.Woods, $score.Lumberyards)
        $score | Add-Member Round $round -PassThru
    }

    $minRepeatCount = 4
    $period = $results | group Score | where Count -ge $minRepeatCount | measure | select -exp Count
    $firstRound = $results | group Score | where Count -gt $minRepeatCount | select -exp Group | sort Round | select -exp Round -f 1
    $scores = $results | where Round -ge $firstRound | select -exp Score -F $period
   
    # Check....
    $results | where Round -ge $firstRound | %{
        if ($_.Score -ne $scores[($_.Round - $firstRound) % $period]) {
            Write-Error "Didnt work"
            exit
        }
    }

    Write-Host ('Part 2: {0}' -f $scores[(1000000000 - $firstRound) % $period])
}

function input {

    $source = 
    '.#.#...|#.
    .....#|##|
    .|..|...#.
    ..|#.....#
    #.#|||#|#|
    ...#.||...
    .|....|...
    ||...#|.#|
    |.||||..|.
    ...#.|..|.'

    $source = Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))

    ($source -split "`r?`n").foreach{$_.Trim()}
}
#input
#return

main