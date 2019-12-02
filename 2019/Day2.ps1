# https://adventofcode.com/2019/day/2
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function parseInput {
    # $source = Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))

    # "$source = @('$((Get-Clipboard) -join "', '")')" | Set-Clipboard
    # $source = @('1', '2', '3')

    # $source = @'...'@ -split "\r?\n"
    # $source = '1 2 3' -split '\s'

    # $data = foreach ($line in $source.where{$_}) {
    #     if ($line -notmatch '^(\d+)$') { throw $line }
    #     [PSCustomObject]@{
    #         Value = [int]$Matches[1]
    #     }
    # }

    # "$data = @($((Get-Clipboard) -join ", "))" | Set-Clipboard
    $data = @(1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,1,13,19,1,10,19,23,2,9,23,27,1,6,27,31,1,10,31,35,1,35,10,39,1,9,39,43,1,6,43,47,1,10,47,51,1,6,51,55,2,13,55,59,1,6,59,63,1,10,63,67,2,67,9,71,1,71,5,75,1,13,75,79,2,79,13,83,1,83,9,87,2,10,87,91,2,91,6,95,2,13,95,99,1,10,99,103,2,9,103,107,1,107,5,111,2,9,111,115,1,5,115,119,1,9,119,123,2,123,6,127,1,5,127,131,1,10,131,135,1,135,6,139,1,139,5,143,1,143,9,147,1,5,147,151,1,151,13,155,1,5,155,159,1,2,159,163,1,163,6,0,99,2,0,14,0)

    # $data = @(1,9,10,3,2,3,11,0,99,30,40,50)
    return $data.foreach{$_}
}
# parseInput


function part1 {
    $data = [System.Collections.ArrayList]::new()
    $data.AddRange((parseInput))
    $data[1] = 12
    $data[2] = 2;
    for ($i = 0; $i -lt $data.Count; $i += 4) {
        $opcode = $data[$i + 0]
        if ($opcode -eq 99) {
            break
        }
        elseif ($opcode -eq 1) {
            $data[$data[$i + 3]] = $data[$data[$i + 1]] + $data[$data[$i + 2]]
        }
        elseif ($opcode -eq 2) {
            $data[$data[$i + 3]] = $data[$data[$i + 1]] * $data[$data[$i + 2]]
        }
    }
    $data[0]
}
part1 #= 11590668

function part2 {
    $in = parseInput
    foreach ($noun in 0..99) {
        foreach ($verb in 0..99) {
            $data = [System.Collections.ArrayList]::new()
            $data.AddRange($in)
            $data[1] = $noun
            $data[2] = $verb

            for ($i = 0; $i -lt $data.Count; $i += 4) {
                $opcode = $data[$i + 0]
                $a1 = $data[$i + 1]
                $a2 = $data[$i + 2]
                $a3 = $data[$i + 3]

                if ($opcode -eq 99) {
                    if ($data[0] -eq 19690720) {
                        "noun=$noun verb=$verb"
                        100 * $noun + $verb
                        return
                    }
                    break
                }
            
                if ($a1 -gt $data.Count -OR $a2 -gt $data.Count -OR $a3 -gt $data.Count) {
                    break
                }

                if ($opcode -eq 1) {
                    $data[$a3] = $data[$a1] + $data[$a2]
                }
                elseif ($opcode -eq 2) {
                    $data[$a3] = $data[$a1] * $data[$a2]
                }
                else {
                    break
                }
            }
        }
    }
}
part2 #= 2254
