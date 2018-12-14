# https://adventofcode.com/2018/day/12
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 3915
    part2 #= 4900000001793
}

function DoIt([long]$rounds) {
    $in = input

    [Regex]$regex = $in.Rules.foreach{ '(' + [regex]::Escape($_.Pattern) + ')' } -join '|'
    #$regex.ToString()

    $pots = '....' + $in.Pots + '....'
    $offset = 4
    Write-Host ('{0,3}: [{1,4}] {2}' -f 0, -$offset, $pots)
    
    $next = [System.Text.StringBuilder]::new(400)
    for ([long]$round = 1; $round -le $rounds; $round++) {
        $null = $next.Clear().Append([char]'.', $pots.Length)

        $startAt = 0
        while ($startAt -lt $pots.Length) {
            $m = $regex.Match($pots, $startAt)
            if (!$m.Success) {
                break
            }

            # which capture group matched?
            for ($i = 1; $i -lt $m.Groups.Count; $i++) {
                if ($m.Groups[$i].Success) {
                    $next[$m.Index + 2] = $in.Rules[$i - 1].Next
                    break
                }
            }

            $startAt = $m.Index + 1
        }

        $first = $last = -1
        for ($i = 0; $i -lt $next.Length; $i++) {
            if ($next[$i] -eq '#') {
                $first = $last = $i
                while (++$i -lt $next.Length) {
                    if ($next[$i] -eq '#') {
                        $last = $i
                    }
                }
                break
            }
        }
        if ($first -eq -1) {
            Write-Host "EMPTY!!!!" -ForegroundColor Green
            break
        }

        $pots = $next.Insert($first, '....').Append('....').ToString($first, $last - $first + 1 + 4 + 4)
        $offset += 4 - $first
        Write-Host ('{0,3}: [{1,4}] {2}' -f $round, -$offset, $pots)
    }

    $sum = 0
    for ($i = 0; $i -lt $pots.Length; $i++) {
        if ($pots[$i] -eq '#') {
            $sum += $i - $offset
        }
    }
    $sum
}

function part1 {
    DoIt 20
}

function part2 {
    [long]$rounds = 50000000000
    #DoIt $rounds

    $pots = '....##.##.##.##....##.##.##.##.##.##.##.##.##....##.##.##.##.##.##.##.##.##....##.##.##....##.##.##.##.##.##....##.##.##.##.##.##.##.##....##.##.##.##.##.##.##.##.##.##....'
    [long]$offset = -($rounds - 68)

    [long]$sum = 0
    for ($i = 0; $i -lt $pots.Length; $i++) {
        if ($pots[$i] -eq '#') {
            $sum += $i - $offset
        }
    }
    $sum
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

    $source = Get-Content ([system.io.path]::ChangeExtension($PSCmdlet.MyInvocation.MyCommand, 'txt'))

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
