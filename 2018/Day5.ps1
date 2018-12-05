# https://adventofcode.com/2018/day/5
Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 10878
    part2 #= 6874
}

function part1 {
    $in = input
    $sb = [System.Text.StringBuilder]::new($in)
    do {
        $reacted = $false
        for ($i = 0; $i -lt $sb.Length - 1; $i++) {
            if ($sb[$i] -ieq $sb[$i+1] -AND $sb[$i] -cne $sb[$i+1]) {
                $reacted = $true
                $null = $sb.Remove($i, 2)
                # recheck previous
                if ($i -gt 0) { --$i }
            }
        }
    } while ($reacted)
    $sb.ToString()
    $sb.Length
}

function part2 {
    $in = input
    $min = [int]::MaxValue
    $sb = [System.Text.StringBuilder]::new($in.Length)
    foreach ($ch in ([char]'a'..[char]'z')) {        
        $null = $sb.Clear()
        $null = $sb.Append(($in -ireplace "$([char]$ch)",''))
        do {
            $reacted = $false
            for ($i = 0; $i -lt $sb.Length - 1; $i++) {
                if ($sb[$i] -ieq $sb[$i+1] -AND $sb[$i] -cne $sb[$i+1]) {
                    $reacted = $true
                    $null = $sb.Remove($i, 2)
                    # recheck previous
                    if ($i -gt 0) { --$i }
                }
            }
        } while ($reacted)
        "$([char]$ch) = $($sb.Length)"
        if ($sb.Length -lt $min) {
            $min = $sb.Length
        }
    }
    $min
}

function input {
    #return 'dabAcCaCBAcCcaDA'

    Get-Content (Join-Path $PSScriptRoot 'Day5.txt')
}
#input
#return

main