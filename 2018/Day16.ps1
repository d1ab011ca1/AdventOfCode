# https://adventofcode.com/2018/day/16
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

class Processor {
    static [string[]] $OperationNames = @(
        'addr'
        'addi'
        'mulr'
        'muli'
        'banr'
        'bani'
        'borr'
        'bori'
        'setr'
        'seti'
        'gtir'
        'gtri'
        'gtrr'
        'eqir'
        'eqri'
        'eqrr'
    )
    [int[]] $Registers = @(0, 0, 0, 0)

    [void] Execute([string]$opName, [int[]]$operands) {
        $this.Execute($opName, $operands[0], $operands[1], $operands[2])
    }
    [void] Execute([string]$opName, [int]$A, [int]$B, [int]$C) {
        $this.$opName($A, $B, $C)
    }

    [void] addr([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3 -OR $B -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = $this.registers[$A] + $this.registers[$B]
    }
    [void] addi([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = $this.registers[$A] + $B
    }

    [void] mulr([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3 -OR $B -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = $this.registers[$A] * $this.registers[$B]
    }
    [void] muli([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = $this.registers[$A] * $B
    }

    [void] banr([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3 -OR $B -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = $this.registers[$A] -band $this.registers[$B]
    }
    [void] bani([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = $this.registers[$A] -band $B
    }

    [void] borr([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3 -OR $B -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = $this.registers[$A] -bor $this.registers[$B]
    }
    [void] bori([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = $this.registers[$A] -bor $B
    }

    [void] setr([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = $this.registers[$A]
    }
    [void] seti([int]$A, [int]$B, [int]$C) {
        $this.registers[$C] = $A
    }

    [void] gtir([int]$A, [int]$B, [int]$C) {
        if ($B -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = if ($A -gt $this.registers[$B]) {1}else {0}
    }
    [void] gtri([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = if ($this.registers[$A] -gt $B) {1}else {0}
    }
    [void] gtrr([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3 -OR $B -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = if ($this.registers[$A] -gt $this.registers[$B]) {1}else {0}
    }

    [void] eqir([int]$A, [int]$B, [int]$C) {
        if ($B -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = if ($A -eq $this.registers[$B]) {1}else {0}
    }
    [void] eqri([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = if ($this.registers[$A] -eq $B) {1}else {0}
    }
    [void] eqrr([int]$A, [int]$B, [int]$C) {
        if ($A -gt 3 -OR $B -gt 3) { throw [System.IndexOutOfRangeException]'Invalid register index.' }
        $this.registers[$C] = if ($this.registers[$A] -eq $this.registers[$B]) {1}else {0}
    }
}

function main {
    $in = parseInput
    $examples = $in.examples
    $instructions = $in.instructions

    part1 $examples #= 500
    part2 $examples $instructions #= 533
}

function part1([object[]]$examples) {
    # How many examples match 3 or more operations?
    $count = 0
    $processor = [Processor]::new()
    foreach ($example in $examples) {
        $numberOfMatches = 0
        foreach ($opName in [Processor]::OperationNames) {
            $processor.Registers = $example.initial
            try {
                $processor.Execute($opName, $example.instruction.operands)
            }
            catch [System.IndexOutOfRangeException] {
                continue
            }
    
            #if ($processor.Registers.CompareTo($example.result, [System.Collections.Comparer]::Default) -eq 0) {
            if (($processor.Registers -join ',') -eq ($example.result -join ',')) {
                if (++$numberOfMatches -eq 3) {
                    ++$count
                    break
                }
            }
        }
    }
    Write-Host "Part 1: $count examples match 3 or more operations."
    Write-Host ''
}    

function part2([object[]]$examples, [object[]]$instructions) {

    # Generate the operation id map using the examples...
    $opMap = generateOpMap $examples
    #$opMap

    # Execute instructions...
    $processor = [Processor]::new()
    foreach ($instruction in $instructions) {
        try {
            $opName = $opMap[$instruction.opId]
            $processor.Execute($opName, $instruction.operands)
        }
        catch [System.IndexOutOfRangeException] {
            continue
        }
    }
        
    Write-Host "Part 2: R0 = $($processor.Registers[0])."
    Write-Host ''
}

# Assign ids to operations using the examples
function generateOpMap([object[]]$examples) {
    $processor = [Processor]::new()
    $maybe = @{}
    foreach ($example in $examples) {
        foreach ($opName in [Processor]::OperationNames) {
            $processor.Registers = $example.initial
            try {
                $processor.Execute($opName, $example.instruction.operands)
            }
            catch [System.IndexOutOfRangeException] {
                continue
            }
            
            #if ($processor.Registers.CompareTo($example.result, [System.Collections.Comparer]::Default) -eq 0) {
            if (($processor.Registers -join ',') -eq ($example.result -join ',')) {
                $maybe[$example.instruction.opId] += @($opName)
            }
        }
    }

    # remove duplicates...
    foreach ($opId in @($maybe.Keys)) {
        $maybe[$opId] = @($maybe[$opId] | sort -Unique)
    }

    $opMap = @{}
    while ($maybe.Count) {
        foreach ($opId in $maybe.Keys) {
            # look for opIds corresponding to a single operation...
            if ($maybe[$opId].Count -eq 1) {

                $opName = $maybe[$opId][0]
                $opMap[$opId] = $opName

                # remove this operation from the maybe map
                $maybe.Remove($opId)
                foreach ($k in @($maybe.Keys)) {
                    $opNames = $maybe[$k]
                    if ($opNames -contains $opName) {
                        $maybe[$k] = @($opNames.where{$_ -ne $opName})
                    }
                }
                break
            }
        }
    }
    return $opMap
}

function parseInput {

    $source = Get-Content -Path ([system.io.path]::ChangeExtension($PSCmdlet.MyInvocation.MyCommand, 'txt'))

    $in = [PSCustomObject]@{
        examples     = @()
        instructions = @()
    }

    $lines = $source -split "`r?`n"
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ([string]::IsNullOrWhiteSpace($lines[$i])) {
            continue
        }    

        if ($lines[$i].StartsWith('Before')) {
            $example = @{}
    
            if ($lines[$i++] -notmatch '(\d+), (\d+), (\d+), (\d+)') { throw $lines[$i - 1] }
            $example.initial = @($matches[1..4].foreach{[int]$_})

            if ($lines[$i++] -notmatch '(\d+) (\d+) (\d+) (\d+)') { throw $lines[$i - 1] }
            $example.instruction = [PSCustomObject]@{
                opId     = [int]$matches[1]
                operands = @($matches[2..4].foreach{[int]$_})
            }

            if ($lines[$i++] -notmatch '(\d+), (\d+), (\d+), (\d+)') { throw $lines[$i - 1] }
            $example.result = @($matches[1..4].foreach{[int]$_})
    
            $in.examples += @([PSCustomObject]$example)
        }
        else {
            if ($lines[$i] -notmatch '(\d+) (\d+) (\d+) (\d+)') { throw $lines[$i - 1] }
            $instruction = [PSCustomObject]@{
                opId     = [int]$matches[1]
                operands = @($matches[2..4].foreach{[int]$_})
            }

            $in.instructions += @($instruction)
        }
    }
    return $in
}
#parseInput
#return

main