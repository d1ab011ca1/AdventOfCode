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

    [void] Execute([Instruction[]]$instructions) {
        $ipreg = $this.IPRegister
        while ($true) {
            $instruction = $instructions[$this.Registers[$ipreg]]
            $A = $instruction.A
            $B = $instruction.B
            $C = $instruction.C

            #$initIP = $this.Registers[$ipreg]
            #$log = "{0,2}: r$C = " -f $initIP
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
                    $this.registers[$C] = if ($this.registers[$A] -eq $this.registers[$B]) {1}else {0}
                    break
                }
            }
            #$log += $this.registers[$C]
            #Write-Host ('{0,-30} ' -f $log) ('[{0,9}, {1,2}, {2,9}, {3,9}, {4,2}, {5,9}]' -f @($this.Registers))
            #if ($initIP -ne $this.Registers[$ipreg]) {
            #    Write-Host "    JMP $($this.registers[$ipreg] + 1)"
            #}
            
            $this.Registers[$ipreg]++
            if ($this.Registers[$ipreg] -ge $instructions.Count) {
                throw [System.InvalidOperationException]'Invalid IP.'
            }
        }
    }    
}

function main {
    #part1 #= 930
    part2 #= 
}

function part1() {
    $in = parseInput

    $processor = [Processor]::new()
    $processor.IPRegister = $in.IPRegister

    try {
        $processor.Execute($in.instructions)
    }
    catch [System.InvalidOperationException] {
        if ($_.Exception.Message -ne 'Invalid IP.') { throw }
    }

    Write-Host "Part 1: R0 = $($processor.Registers[0])."
    Write-Host ''
}    

function part2([object[]]$examples, [object[]]$instructions) {
    $in = parseInput

    $processor = [Processor]::new()
    $processor.IPRegister = $in.IPRegister
    $processor.Registers[0] = 1

    # 10551329
    #$processor.Registers = @(10551330, 10551328, 0, 10551329, 7, 10551329)

    try {
        $processor.Execute($in.instructions)
    }
    catch [System.InvalidOperationException] {
        if ($_.Exception.Message -ne 'Invalid IP.') { throw }
    }

    Write-Host "Part 2: R0 = $($processor.Registers[0])."
    Write-Host ''
}

function parseInput {

    $source = 
    '#ip 0
    seti 5 0 1
    seti 6 0 2
    addi 0 1 0
    addr 1 2 3
    setr 1 0 0
    seti 8 0 4
    seti 9 0 5'

    $source = Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))

    $in = [PSCustomObject]@{
        IPRegister   = 0
        instructions = @()
    }

    $lines = $source -split "`r?`n"

    if ($lines[0] -notmatch '#ip (\d+)') { throw $lines[0] }
    $in.IPRegister = [int]$matches[1]

    $in.instructions = $lines | select -skip 1 | foreach {
        if ($_[0] -eq ';') { return }
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