# https://adventofcode.com/2018/day/19
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

class Instruction {
    [string] $Name
    [long] $A
    [long] $B
    [long] $C
}

class Processor {
    [long[]] $Registers = @(0, 0, 0, 0, 0, 0)
    [long] $IPRegister = -1

    [int] Execute([Instruction[]]$instructions, [int]$part = 1) {
        $map = @{}
        $ipreg = $this.IPRegister
        $prev = 0
        $set = @{}
        while ($true) {
            $IP = $this.Registers[$ipreg]
            $instruction = $instructions[$IP]
            $A = $instruction.A
            $B = $instruction.B
            $C = $instruction.C

            #$log = "{0,2}: r$C = " -f $IP
            switch ($instruction.Name) {
                'addr' {
                    #$log += "r$A + r$B = "
                    $this.registers[$C] = $this.registers[$A] + $this.registers[$B]
                    break
                }
                'addi' {
                    #$log += "r$A + $B = "
                    $this.registers[$C] = $this.registers[$A] + $B
                    break
                }
                'mulr' {
                    #$log += "r$A * r$B = "
                    $this.registers[$C] = $this.registers[$A] * $this.registers[$B]
                    break
                }
                'muli' {
                    #$log += "r$A * $B = "
                    $this.registers[$C] = $this.registers[$A] * $B
                    break
                }
                'shri' {
                    #$log += "r$A >> $B = "
                    $this.registers[$C] = $this.registers[$A] -shr $B
                    break
                }
                'banr' {
                    #$log += "r$A & r$B = "
                    $this.registers[$C] = $this.registers[$A] -band $this.registers[$B]
                    break
                }
                'bani' {
                    #$log += "r$A & $B = "
                    $this.registers[$C] = $this.registers[$A] -band $B
                    break
                }
                'borr' {
                    #$log += "r$A | r$B = "
                    $this.registers[$C] = $this.registers[$A] -bor $this.registers[$B]
                    break
                }
                'bori' {
                    #$log += "r$A | $B = "
                    $this.registers[$C] = $this.registers[$A] -bor $B
                    break
                }
                'setr' {
                    #$log += "r$A = "
                    $this.registers[$C] = $this.registers[$A]
                    break
                }
                'seti' {
                    $this.registers[$C] = $A
                    break
                }
                'gtir' {
                    #$log += "$A > r$B = "
                    $this.registers[$C] = if ($A -gt $this.registers[$B]) {1}else {0}
                    break
                }
                'gtri' {
                    #$log += "r$A > $B = "
                    $this.registers[$C] = if ($this.registers[$A] -gt $B) {1}else {0}
                    break
                }
                'gtrr' {
                    #$log += "r$A > r$B = "
                    $this.registers[$C] = if ($this.registers[$A] -gt $this.registers[$B]) {1}else {0}
                    break
                }
                'eqir' {
                    #$log += "$A == r$B = "
                    $this.registers[$C] = if ($A -eq $this.registers[$B]) {1}else {0}
                    break
                }
                'eqri' {
                    #$log += "r$A == $B = "
                    $this.registers[$C] = if ($this.registers[$A] -eq $B) {1}else {0}
                    break
                }
                'eqrr' {
                    #$log += "r$A == r$B = "

                    if ($this.registers[$B] -eq 0) {
                        $r1 = $this.registers[$A]
                        if ($part -eq 1) {
                            return $r1
                        } else {
                            if ($set.ContainsKey($r1)) {
                                return $prev
                            }
                            $set.Add($r1, $null)
                            $prev = $r1
                        }
                    }
    
                    $this.registers[$C] = if ($this.registers[$A] -eq $this.registers[$B]) {1}else {0}
                    break
                }
            }
            #$log += $this.registers[$C]
            #Write-Host ('{0,-30} ' -f $log) ('[{0,9}, {1,2}, {2,9}, {3,9}, {4,2}, {5,9}]' -f @($this.Registers))
            #if ($IP -ne $IP) {
            #    Write-Host "    JMP $($IP + 1)"
            #}
            
            $this.Registers[$ipreg]++
            if ($this.Registers[$ipreg] -ge $instructions.Count) {
                throw [System.InvalidOperationException]'Invalid IP.'
            }
        }
        throw 'Loop terminated unexpectedly.'
        return 0
    }    
}

function main {
    part1 #= 1797184
    part2 #= 11011493
}

function part1() {
    $in = parseInput

    $processor = [Processor]::new()
    $processor.IPRegister = $in.IPRegister

    $result = $processor.Execute($in.instructions, 1)

    Write-Host "Part 1: $result."
    Write-Host ''
}    

function part2([object[]]$examples, [object[]]$instructions) {
    $in = parseInput

    $processor = [Processor]::new()
    $processor.IPRegister = $in.IPRegister

    $result = $processor.Execute($in.instructions, 2)

    Write-Host "Part 2: $result."
    Write-Host ''
}

function parseInput {
    $source = Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))

    $in = [PSCustomObject]@{
        IPRegister   = 0
        instructions = @()
    }

    $lines = $source -split "`r?`n"

    if ($lines[0] -notmatch '#ip (\d+)') { throw $lines[0] }
    $in.IPRegister = [int]$matches[1]

    $in.instructions = $lines | select -skip 1 | foreach {
        if (!$_ -or $_[0] -eq ';') { return }
        if ($_ -notmatch '([a-z]+)\s+(\d+)\s+(\d+)\s+(\d+)') { throw $_ }
        $instruction = [Instruction]::new()
        $instruction.Name = $matches[1]
        $instruction.A = [int]$matches[2]
        $instruction.B = [int]$matches[3]
        $instruction.C = [int]$matches[4]
        $instruction
    }
    return $in
}
#parseInput
#return

main