# https://adventofcode.com/2019/day/13
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function parseInput($source) {
    if (!$source) {
        $source = Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))
    }

    # "`$source = @('$((Get-Clipboard) -join "', '")')" | Set-Clipboard
    # $source = @('1', '2', '3')

    foreach ($line in $source) {
        $line.Split(',').where{$_}.foreach{[long]$_}
    }
}
#return parseInput

class IntCodeObj {
    [long[]] $program
    [long] $i
    [System.Collections.Queue] $in
    [System.Collections.Queue] $out
    [bool] $done
    [long] $relativeBase
    [System.Collections.Generic.Dictionary[long,long]] $data

    IntCodeObj([long[]] $program) {
        $this.program = $program
        $this.Reset()
    }

    [IntCodeObj] Reset() {
        $this.i = 0
        $this.in = [System.Collections.Queue]::new()
        $this.out = [System.Collections.Queue]::new()
        $this.done = $false
        $this.relativeBase = 0
        $this.data = [System.Collections.Generic.Dictionary[long,long]]::new()
        for ($n = 0; $n -lt $this.program.Count; $n++) {
            $this.data[$n] = $this.program[$n]
        }
        return $this
    }

    [IntCodeObj] AddInput([long]$value) {
        $this.in.Enqueue($value)
        return $this
    }
    [IntCodeObj] AddInputs([long[]]$values) {
        $values.ForEach{ $this.in.Enqueue($_) }
        return $this
    }

    [bool] HasOutput() {
        return $this.out.Count -gt 0
    }
    [long] GetOutput() {
        return $this.out.Dequeue()
    }
    [long[]] GetAllOutput() {
        $values = @($this.out)
        $this.out.Clear()
        return $values
    }

    [IntCodeObj] Compute() {
        while ($this.Step()) {}
        return $this
    }

    [long] ReadMem([long]$address) {
        try {
            return $this.data[$address]
        } catch {
            return 0
        }
    }

    [bool] Step() {
        if ($this.done) {
            return $false
        }
        $inst = $this.ReadMem($this.i)
        $opcode = $inst % 100
        $modes = $inst.ToString().PadLeft(3+2, '0').Substring(0,3)
        switch ($opcode) {
            99 { # Exit
                $this.done = $true
                return $false
            }
            1 { # Addition
                $a1 = $this.ReadMem($this.i + 1)
                $a2 = $this.ReadMem($this.i + 2)
                $a3 = $this.ReadMem($this.i + 3)
                if ($modes[2] -eq '0') { $a1 = $this.ReadMem($a1) }
                elseif ($modes[2] -eq '2') { $a1 = $this.ReadMem($this.relativeBase + $a1) }
                if ($modes[1] -eq '0') { $a2 = $this.ReadMem($a2) }
                elseif ($modes[1] -eq '2') { $a2 = $this.ReadMem($this.relativeBase + $a2) }
                if ($modes[0] -eq '2') { $a3 += $this.relativeBase }

                $this.data[$a3] = $a1 + $a2
                $this.i += 4
                break
            }
            2 { # Multiply
                $a1 = $this.ReadMem($this.i + 1)
                $a2 = $this.ReadMem($this.i + 2)
                $a3 = $this.ReadMem($this.i + 3)
                if ($modes[2] -eq '0') { $a1 = $this.ReadMem($a1) }
                elseif ($modes[2] -eq '2') { $a1 = $this.ReadMem($this.relativeBase + $a1) }
                if ($modes[1] -eq '0') { $a2 = $this.ReadMem($a2) }
                elseif ($modes[1] -eq '2') { $a2 = $this.ReadMem($this.relativeBase + $a2) }
                if ($modes[0] -eq '2') { $a3 += $this.relativeBase }

                $this.data[$a3] = $a1 * $a2
                $this.i += 4
                break
            }
            3 { # Store Input
                if ($this.in.Count -eq 0) {
                    # No input. Pause
                    return $false
                }
                $a1 = $this.ReadMem($this.i + 1)
                if ($modes[2] -eq '2') { $a1 += $this.relativeBase }

                $this.data[$a1] = $this.in.Dequeue()
                $this.i += 2
                break
            }
            4 { # Write Output
                $a1 = $this.ReadMem($this.i + 1)
                if ($modes[2] -eq '0') { $a1 = $this.ReadMem($a1) }
                elseif ($modes[2] -eq '2') { $a1 = $this.ReadMem($this.relativeBase + $a1) }

                $this.out.Enqueue($a1)
                $this.i += 2
                break
            }
            5 { # jump if true
                $a1 = $this.ReadMem($this.i + 1)
                $a2 = $this.ReadMem($this.i + 2)
                if ($modes[2] -eq '0') { $a1 = $this.ReadMem($a1) }
                elseif ($modes[2] -eq '2') { $a1 = $this.ReadMem($this.relativeBase + $a1) }
                if ($modes[1] -eq '0') { $a2 = $this.ReadMem($a2) }
                elseif ($modes[1] -eq '2') { $a2 = $this.ReadMem($this.relativeBase + $a2) }

                if ($a1) { $this.i = $a2 }
                else { $this.i += 3 }
                break
            }
            6 { # jump if false
                $a1 = $this.ReadMem($this.i + 1)
                $a2 = $this.ReadMem($this.i + 2)
                if ($modes[2] -eq '0') { $a1 = $this.ReadMem($a1) }
                elseif ($modes[2] -eq '2') { $a1 = $this.ReadMem($this.relativeBase + $a1) }
                if ($modes[1] -eq '0') { $a2 = $this.ReadMem($a2) }
                elseif ($modes[1] -eq '2') { $a2 = $this.ReadMem($this.relativeBase + $a2) }

                if (!$a1) { $this.i = $a2 } 
                else { $this.i += 3 }
                break
            }
            7 { # less than
                $a1 = $this.ReadMem($this.i + 1)
                $a2 = $this.ReadMem($this.i + 2)
                $a3 = $this.ReadMem($this.i + 3)
                if ($modes[2] -eq '0') { $a1 = $this.ReadMem($a1) }
                elseif ($modes[2] -eq '2') { $a1 = $this.ReadMem($this.relativeBase + $a1) }
                if ($modes[1] -eq '0') { $a2 = $this.ReadMem($a2) }
                elseif ($modes[1] -eq '2') { $a2 = $this.ReadMem($this.relativeBase + $a2) }
                if ($modes[0] -eq '2') { $a3 += $this.relativeBase }

                $this.data[$a3] = [long]($a1 -lt $a2)
                $this.i += 4
                break
            }
            8 { # equals
                $a1 = $this.ReadMem($this.i + 1)
                $a2 = $this.ReadMem($this.i + 2)
                $a3 = $this.ReadMem($this.i + 3)
                if ($modes[2] -eq '0') { $a1 = $this.ReadMem($a1) }
                elseif ($modes[2] -eq '2') { $a1 = $this.ReadMem($this.relativeBase + $a1) }
                if ($modes[1] -eq '0') { $a2 = $this.ReadMem($a2) }
                elseif ($modes[1] -eq '2') { $a2 = $this.ReadMem($this.relativeBase + $a2) }
                if ($modes[0] -eq '2') { $a3 += $this.relativeBase }

                $this.data[$a3] = [long]($a1 -eq $a2)
                $this.i += 4
                break
            }
            9 { # Set RelativeBase
                $a1 = $this.ReadMem($this.i + 1)
                if ($modes[2] -eq '0') { $a1 = $this.ReadMem($a1) }
                elseif ($modes[2] -eq '2') { $a1 = $this.ReadMem($this.relativeBase + $a1) }

                $this.relativeBase += $a1
                $this.i += 2
                break
            }
            Default {
                throw "Unexpected opcode: $opcode"
            }
        }
        return $true
    }
}

function part1 {
    $program = parseInput
    $out = [IntCodeObj]::new($program).Compute().GetAllOutput()
    $hull = @{}
    for ($i = 0; $i -lt $out.Count; $i += 3) {
        $x = $out[$i+0]
        $y = $out[$i+1]
        $id = $out[$i+2]
        if ($id -eq 0) {
            $null = $hull.Remove("$x,$y")
        } else {
            $hull["$x,$y"] = $id
        }
    }
    $hull.Values.where{$_ -eq 2}.count
}
#part1 #= 361

function part2 {
    $program = parseInput
    $c = [IntCodeObj]::new($program)
    $c.data[0] = 2;
    $out = $c.Compute().GetAllOutput()
    $board = @{}
    $width,$height = 0,0
    $tiles = @(' ','W','#','_','o')
    for ($i = 0; $i -lt $out.Count; $i += 3) {
        $x = $out[$i+0]
        $y = $out[$i+1]
        $id = $out[$i+2]
        $board["$x,$y"] = $tiles[$id]
        if ($x -ge $width) {$width = $x+1}
        if ($y -ge $height) {$height = $y+1}
    }
    #"width,height = $width,$height"

    $paddle = [pscustomobject]@{X = 0; Y = 0}
    $ball = [pscustomobject]@{X = 0; Y = 0}
    $ballDir = [pscustomobject]@{X = 0; Y = 0}
    $blocks = 0
    
    $output = (1..$height).foreach{[System.Text.StringBuilder]::new(' ' * $width)}
    foreach ($p in $board.GetEnumerator()) {
        $x,$y = $p.Key -split ','
        $x,$y = [int]$x,[int]$y
        $output[$y][$x] = $p.Value

        if ($p.Value -eq 'o') {
            $ball.X,$ball.Y = $x,$y
        } elseif ($p.Value -eq '_') {
            $paddle.X,$paddle.Y = $x,$y
        } elseif ($p.Value -eq '#') {
            ++$blocks
        }
    }
    $board = $output
    
    $score = 0
    $paddleLine = $output[$paddle.Y]

    function printBoard {
        Clear-Host
        "Score: $score"
        $board.foreach{$_.ToString()}
        "Paddle:  {0},{1}" -f $paddle.x,$paddle.Y
        "Ball:    {0},{1}" -f $ball.x,$ball.Y
        "BallDir: {0},{1}" -f $ballDir.x,$ballDir.Y
        sleep -Milliseconds 10
    }

    $moves = @()
    function movePaddle([int]$dir) {
        $null = $c.AddInput($dir)
        # erase paddle
        $paddleLine[$paddle.X] = if ($paddleLine[$paddle.X] -eq 'X') {'o'}else {' '}
        # move paddle
        $paddle.X += $dir
        $moves += $dir
        # draw paddle
        $paddleLine[$paddle.X] = if ($paddleLine[$paddle.X] -eq 'o') {'X'}else {'_'}
    }

    # at this point it is waiting for input
    do {
        #printBoard

        # move paddle
        if ($paddle.X -lt $ball.X) {movePaddle +1}
        elseif ($paddle.X -gt $ball.X) {movePaddle -1}
        else { movePaddle 0}

        # play
        $null = $c.Compute()

        # update board...
        while($c.out.count -ge 3) { 
            $x = $c.out.Dequeue()
            $y = $c.out.Dequeue()
            $id = $c.out.Dequeue()

            if ($x -eq -1 -AND $y -eq 0) {
                if ($score -ne $id) { --$blocks }
                $score = $id
                continue
            }
    
            $tile = $tiles[$id]
            $output[$y][$x] = $tile
            if ($tile -eq 'o') {
                $ball.X,$ball.Y = $x,$y
            }
        }
    } while ($blocks -AND $ball.Y -lt $height-1)
    printBoard
}
part2 #= 17590
