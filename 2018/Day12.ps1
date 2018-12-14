# https://adventofcode.com/2018/day/12
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    part1 #= 3915
    #part2 #= 
}

function DoIt([long]$rounds) {
    $in = input

    [Regex]$regex = $in.Rules.foreach{ '(' + [regex]::Escape($_.Pattern) + ')' } -join '|'
    #$regex.ToString()

    $pots = '....' + $in.Pots + ('.' * 20)
    $begin = 4
    $length = $in.Pots.Length
    Write-Host ('{0,2}: {1}' -f 0, $pots)
    
    $next = [System.Text.StringBuilder]::new(400)
    for ([long]$round = 1; $round -le $rounds; $round++) {
        $null = $next.Clear().Append('.' * (4 + $length + 20))
        $startAt = 0
        while ($startAt -lt $pots.Length) {
            $m = $regex.Match($pots, $startAt)
            if (!$m.Success) {
                break
            }

            # which capture matched?
            for ($i = 1; $i -lt $m.Groups.Count; $i++) {
                if ($m.Groups[$i].Success) {
                    $c = $in.Rules[$i - 1].Next
                    $x = $m.Index + 2
                    $next[$x] = $c
                    #Write-Host $next.ToString()
                    break
                }
            }

            $startAt = $m.Index + 1
        }

        $pots = $next.ToString()
        for ($i = 0; $i -lt $pots.Length; $i++) {
            if ($pots[$i] -eq '#') {
                $begin = $i
                for (; $i -lt $pots.Length; $i++) {
                    if ($pots[$i] -eq '#') {
                        $length = $i - $begin + 1
                    }
                }
                break
            }
        }
        Write-Host ('{0,2}: {1}' -f $round, $pots)
    }

    $sum = 0
    for ($i = 0; $i -lt $pots.Length; $i++) {
        if ($pots[$i] -eq '#') {
            $sum += $i - $begin
        }
    }
    $sum
}

function part1 {
    DoIt 20
}

function part2 {
    DoIt 40
}

function input {
    $source = `
        'initial state: #..#.#..##......###...###

...## => #
..#.. => #
.#... => #
.#.#. => #
.#.## => #
.##.. => #
.#### => #
#.#.# => #
#.### => #
##.#. => #
##.## => #
###.. => #
###.# => #
####. => #'

    #$source = Get-Content ([system.io.path]::ChangeExtension($PSCmdlet.MyInvocation.MyCommand, 'txt'))

    #$source = $source.Replace('.', '_')
    $text = $source -split "`r?`n"
    $pots = $text[0].Substring('initial state: '.Length)
    $rules = $text | select -Skip 2 | % {
        if (!($_ -match '^(.{5}) => (.)')) {throw "'$_'"}
        [PSCustomObject]@{
            Pattern = $Matches[1]
            Next    = $Matches[2]
        }
    }

    [PSCustomObject]@{
        Pots  = $pots
        Rules = $rules
    }
}
#input
#return

main
