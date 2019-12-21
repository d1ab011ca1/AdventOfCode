# https://adventofcode.com/2019/day/7
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
        $line.Split(',').where{$_}
    }
}
# parseInput

class IntCodeObj {
    [System.Collections.ArrayList] $data
    [int] $i
    [System.Collections.Queue] $in
    [System.Collections.Queue] $out
    [bool] $done

    IntCodeObj([int[]] $program) {
        $this.data = [System.Collections.ArrayList]::new($program)
        $this.Reset()
    }

    [IntCodeObj] Reset() {
        $this.i = 0
        $this.in = [System.Collections.Queue]::new()
        $this.out = [System.Collections.Queue]::new()
        $this.done = $false
        return $this
    }

    [IntCodeObj] AddInput([int]$value) {
        $this.in.Enqueue($value)
        return $this
    }
    [IntCodeObj] AddInputs([int[]]$values) {
        $values.ForEach{ $this.in.Enqueue($_) }
        return $this
    }

    [bool] HasOutput() {
        return $this.out.Count -gt 0
    }
    [int] GetOutput() {
        return $this.out.Dequeue()
    }
    [int[]] GetAllOutput() {
        $values = @($this.out)
        $this.out.Clear()
        return $values
    }

    [IntCodeObj] Compute() {
        while ($this.Step()) {}
        return $this
    }

    [bool] Step() {
        if ($this.done) {
            return $false
        }
        $opcode = $this.data[$this.i + 0] % 100
        $modes = ($this.data[$this.i + 0]).ToString().PadLeft(3+2, '0').Substring(0,3)
        switch ($opcode) {
            99 { # Exit
                $this.done = $true
                return $false
            }
            1 { # Addition
                $a1 = $this.data[$this.i + 1]
                $a2 = $this.data[$this.i + 2]
                $a3 = $this.data[$this.i + 3]
                if ($modes[2] -eq '0') { $a1 = $this.data[$a1] }
                if ($modes[1] -eq '0') { $a2 = $this.data[$a2] }
                $this.data[$a3] = $a1 + $a2
                $this.i += 4
                break
            }
            2 { # Multiply
                $a1 = $this.data[$this.i + 1]
                $a2 = $this.data[$this.i + 2]
                $a3 = $this.data[$this.i + 3]
                if ($modes[2] -eq '0') { $a1 = $this.data[$a1] }
                if ($modes[1] -eq '0') { $a2 = $this.data[$a2] }
                $this.data[$a3] = $a1 * $a2
                $this.i += 4
                break
            }
            3 { # Store Input
                $a1 = $this.data[$this.i + 1]
                if ($this.in.Count -eq 0) {
                    # No input. Pause
                    return $false
                }
                $this.data[$a1] = $this.in.Dequeue()
                $this.i += 2
                break
            }
            4 { # Write Output
                $a1 = $this.data[$this.i + 1]
                if ($modes[2] -eq '0') { $a1 = $this.data[$a1] }
                $this.i += 2
                $this.out.Enqueue($a1)
                break
            }
            5 { # jump if true
                $a1 = $this.data[$this.i + 1]
                $a2 = $this.data[$this.i + 2]
                if ($modes[2] -eq '0') { $a1 = $this.data[$a1] }
                if ($modes[1] -eq '0') { $a2 = $this.data[$a2] }
                if ($a1) { $this.i = $a2 }
                else { $this.i += 3 }
                break
            }
            6 { # jump if false
                $a1 = $this.data[$this.i + 1]
                $a2 = $this.data[$this.i + 2]
                if ($modes[2] -eq '0') { $a1 = $this.data[$a1] }
                if ($modes[1] -eq '0') { $a2 = $this.data[$a2] }
                if (!$a1) { $this.i = $a2 } 
                else { $this.i += 3 }
                break
            }
            7 { # less than
                $a1 = $this.data[$this.i + 1]
                $a2 = $this.data[$this.i + 2]
                $a3 = $this.data[$this.i + 3]
                if ($modes[2] -eq '0') { $a1 = $this.data[$a1] }
                if ($modes[1] -eq '0') { $a2 = $this.data[$a2] }
                $this.data[$a3] = [int]($a1 -lt $a2)
                $this.i += 4
                break
            }
            8 { # equals
                $a1 = $this.data[$this.i + 1]
                $a2 = $this.data[$this.i + 2]
                $a3 = $this.data[$this.i + 3]
                if ($modes[2] -eq '0') { $a1 = $this.data[$a1] }
                if ($modes[1] -eq '0') { $a2 = $this.data[$a2] }
                $this.data[$a3] = [int]($a1 -eq $a2)
                $this.i += 4
                break
            }
            Default {
                throw "Unexpected opcode: $opcode"
            }
        }
        return $true
    }
}

function swap([int[]]$a, [int]$i, [int]$j) { 
    $temp = $a[$i]
    $a[$i] = $a[$j] 
    $a[$j] = $temp
    return $a
} 
function permute([int[]]$a, [int]$l = 0, [int]$r = -1) {
    if ($r -eq -1) { $r = $a.Length - 1 }
    if ($l -eq $r) {
        return @(,$a) 
    }
    for ($i = $l; $i -le $r; $i++) { 
        $a = swap $a $l $i 
        permute $a ($l + 1) $r 
        $a = swap $a $l $i 
    }     
}
# permute (1,2,3)

function part1 {
    $program = parseInput
    # $program = parseInput 3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,
#1002,33,7,33,1,33,31,31,1,32,31,31,4,31,99,0,0,0
#'@ -split '\r?\n')
    # return $program
    
    permute @(0,1,2,3,4) | %{
        $phase = $_
        $thrust = 0
        foreach ($i in $phase) {
            $thrust = [IntCodeObj]::new($program).AddInputs(($i, $thrust)).Compute().GetOutput()
        }
        return $thrust
    } | measure -Maximum
}
part1 #= 30940

function part2 {
    $program = parseInput
#    $program = parseInput (@'
#3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,
#27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5
#'@ -split '\r?\n')
    # return $program

    permute @(5,6,7,8,9) | %{
        $phase = $_

        # reset computers
        [IntCodeObj[]]$computers = (1..5).ForEach{[IntCodeObj]::new($program)}
        #$computers.ForEach{$null = $_.Reset()}

        # connect inputs to outputs
        $computers[0].in = $computers[4].out
        for ($i = 1; $i -lt 5; ++$i) { $computers[$i].in = $computers[$i-1].out }

        # init inputs
        for ($i = 0; $i -lt 5; ++$i) { $null = $computers[$i].AddInputs($phase[$i]) }
        $null = $computers[0].AddInput(0)

        # execute until computer[4] is done...
        $thrust = 0
        while ($true) {
            for ($i = 0; $i -lt 5; ++$i) {
                $c = $computers[$i]
                $null = $c.Compute()

                if ($i -eq 4) {
                    if ($c.HasOutput()) {
                        $thrust = @($c.out)[-1]
                    }
                    if ($c.done) {
                        Write-Host "[$($phase -join ',')] => $thrust"
                        return $thrust
                    }
                }
            }
        }
    } | measure -Maximum
}
part2 #= 76211147
