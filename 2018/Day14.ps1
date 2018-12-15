# https://adventofcode.com/2018/day/14
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 5115114101
    part2 #= 20310465
}

function printList {
    $node = $list.First
    foreach ($idx in 0..($list.Count-1)) {
        $v = if ($idx -eq $elf1Idx) { '({0})' -f $elf1.Value }
        elseif ($idx -eq $elf2Idx) { '[{0}]' -f $elf2.Value }
        else { ' {0} ' -f $node.Value }
        Write-Host $v -NoNewline

        $node = $node.Next
        ++$idx
    }
    Write-Host ''
}

function part1 {
    $in = Input

    #$recipes = 9 # => 5158916779
    $recipes = 633601

    $list = [System.Collections.Generic.LinkedList[int]]::new()
    $elf1 = $list.AddLast(3)
    $elf1Idx = 0
    $elf2 = $list.AddLast(7)
    $elf2Idx = 1

    printList
    while ($list.Count -lt $recipes + 10) {

        $score = '{0}' -f ($elf1.Value + $elf2.Value)
        foreach ($c in $score.GetEnumerator()) {
            $null = $list.AddLast([int]$c - [int][char]'0')
        }

        foreach ($_ in 0..$elf1.Value) {
            $elf1 = $elf1.Next
            if ($null -ne $elf1) {
                ++$elf1Idx
            } else {
                $elf1 = $list.First
                $elf1Idx = 0
            }
        }
        foreach ($_ in 0..$elf2.Value) {
            $elf2 = $elf2.Next
            if ($null -ne $elf2) {
                ++$elf2Idx
            } else {
                $elf2 = $list.First
                $elf2Idx = 0
            }
        }

        #printList
    }

    -join ($list | select -Skip $recipes -First 10)
}

function part2 {
    $in = Input

    $target = '51589' # => 9
    $target = '01245' # => 5
    $target = '59414' # => 2018
    $target = '633601'

    $list = [System.Collections.Generic.LinkedList[int]]::new()
    $elf1 = $list.AddLast(3)
    $elf1Idx = 0
    $elf2 = $list.AddLast(7)
    $elf2Idx = 1
    $candidateValue = [string]::Empty
    $candidateIdx = -1

    while ($true) {

        $score = '{0}' -f ($elf1.Value + $elf2.Value)
        foreach ($c in $score.GetEnumerator()) {
            $null = $list.AddLast([int]$c - [int][char]'0')

            if ($target[$candidateValue.Length] -ne $c -AND $candidateValue.Length) {
                $candidateValue = [string]::Empty
            }
            if ($target[$candidateValue.Length] -eq $c) {
                if (!$candidateValue.Length) {
                    $candidateIdx = $list.Count - 1
                }
                $candidateValue += $c
                if ($candidateValue.Length -eq $target.Length) {
                    $candidateIdx
                    return
                }
            }
        }


        foreach ($_ in 0..$elf1.Value) {
            $elf1 = $elf1.Next
            if ($null -ne $elf1) {
                ++$elf1Idx
            } else {
                $elf1 = $list.First
                $elf1Idx = 0
            }
        }
        foreach ($_ in 0..$elf2.Value) {
            $elf2 = $elf2.Next
            if ($null -ne $elf2) {
                ++$elf2Idx
            } else {
                $elf2 = $list.First
                $elf2Idx = 0
            }
        }

        #printList
    }
}

function Input {
    $source = `
''
    #$source = Get-Content -Path ([system.io.path]::ChangeExtension($PSCmdlet.MyInvocation.MyCommand, 'txt'))

    $source -split "`r?`n" | %{
        $_
    }
}
#input
#return

main
