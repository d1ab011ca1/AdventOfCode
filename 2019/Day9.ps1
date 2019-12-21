# https://adventofcode.com/2019/day/9
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
        $line.Split(',').where{$_}.foreach{[int]$_}
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
    #$program = @(104,1125899906842624,99)
    #$program = @(1102,34915192,34915192,7,4,7,99,0)
    #$program = @(109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99)
    [IntCodeObj]::new($program).AddInputs(@(1)).Compute().GetAllOutput()
}
part1 #= 3742852857

function part2 {
    $program = parseInput
    [IntCodeObj]::new($program).AddInputs(@(2)).Compute().GetAllOutput()
}
part2 #= 73439
