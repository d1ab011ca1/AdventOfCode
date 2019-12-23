# https://adventofcode.com/2019/day/11
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
    $c = [IntCodeObj]::new($program)
    $hull = @{}
    $x,$y,$d = 0,0,0 # up
    # $d => 0,1,2,3 (up,right,down,left)
    $iter = 0
    while (!$c.done) {
        ++$iter
        try {
            $color = $hull["$x,$y"]
        } catch {
            $color = 0
        }
        $color,$turn = $c.AddInput($color).Compute().GetAllOutput()
        #if ($color -ne 0 -AND $color -ne 1) {throw "Bad color: $color at $iter, $x,$y"}
        #if ($turn -ne 0 -AND $turn -ne 1) {throw "Bad turn: $turn at $iter, $x,$y"}
        
        # paint
        $hull["$x,$y"] = $color
        # "$iter : $x,$y = $color"

        # turn left or right
        if ($turn -eq 1) {
            # right (clockwise)
            $d = ($d + 1) % 4
        } else {
            # left (counter-clockwise)
            $d = ($d + 3) % 4
        }

        # move forward
        if ($d -eq 0) {++$y} #up
        elseif ($d -eq 1) {++$x} #right
        elseif ($d -eq 2) {--$y} #down
        else {--$x} #left
    }

    $hull.count
}
#part1 #= 2539

function part2 {
    $program = parseInput
    $c = [IntCodeObj]::new($program)
    $hull = @{}
    $x,$y,$d = 0,0,0 # up
    # $d => 0,1,2,3 (up,right,down,left)
    $hull["$x,$y"] = 1 # white
    $maxx,$maxy = 0,0
    $minx,$miny = 0,0
    while (!$c.done) {
        try {
            $color = $hull["$x,$y"]
        } catch {
            $color = 0
        }
        $color,$turn = $c.AddInput($color).Compute().GetAllOutput()
        
        # paint
        $hull["$x,$y"] = $color

        # turn left or right
        if ($turn -eq 1) {
            # right (clockwise)
            $d = ($d + 1) % 4
        } else {
            # left (counter-clockwise)
            $d = ($d + 3) % 4
        }

        # move forward
        if ($d -eq 0) { #up
            ++$y
            if ($y -gt $maxy) {$maxy = $y}
        }
        elseif ($d -eq 1) { #right
            ++$x
            if ($x -gt $maxx) {$maxx = $x}
        }
        elseif ($d -eq 2) { #down
            --$y
            if ($y -lt $miny) {$miny = $y}
        }
        else { #left
            --$x
            if ($x -lt $minx) {$minx = $x}
        }
    }

    "x = $maxx..$minx, y = $miny..$maxy"
    $emptyline = ' ' * ($maxx - $minx + 1)
    $output = ($miny..$maxy).foreach{[System.Text.StringBuilder]::new($emptyline)}
    foreach ($p in $hull.GetEnumerator()) {
        if ($p.Value -eq 1) {
            $x,$y = $p.Key -split ','
            $line = $output[$maxy - ([int]$y - $miny) - 1]
            $null = $line[[int]$x - $minx] = '#'
        }
    }
    $output.foreach{$_.ToString()}
}
part2 #= ZLEBKJRA
