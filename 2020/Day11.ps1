$d = @"
L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL
"@ -split '\r?\n'
$d = Get-Content "$PSScriptRoot/Day11.txt"

# include blanks around entire thing
$h = $d.Count + 2
$w = $d[0].Length + 2
$g = [Text.StringBuilder]::new($h * $w)
$null = $g.Append(' ' * $w)
$d.foreach{ $null = $g.Append(' ').Append($_).Append(' ') }
$null = $g.Append(' ' * $w)
$begin = $w + 1
$end = ($h - 1) * $w - 1

function printGrid([Text.StringBuilder]$grid) {
    for ($p = 0; $p -lt $grid.Length; $p += $w) {
        Write-Host $grid.ToString($p, $w)
    }
    Write-Host
}
function copyGrid($grid) {
    [Text.StringBuilder]::new($grid.ToString())
}

#printGrid $g

$around = @(
    - $w - 1
    - $w
    - $w + 1
    -1
    #0
    1
    + $w + 1
    + $w
    + $w - 1)

function part1 {
    $changed = $true
    #$changed = $false
    while ($changed) {
        $changed = $false
        $next = copyGrid $g
        for ($p = $begin; $p -lt $end; ++$p) {
            if ($g[$p] -eq 'L') {
                $occupied = $around.ForEach{ $g[($p + $_)] }.where{ $_ -eq '#' }.count
                if ($occupied -eq 0) {
                    $next[$p] = '#'
                    $changed = $true
                }
            }
            elseif ($g[$p] -eq '#') {
                $occupied = $around.ForEach{ $g[($p + $_)] }.where{ $_ -eq '#' }.count
                if ($occupied -ge 4) {
                    $next[$p] = 'L'
                    $changed = $true
                }
            }
        }
        #printGrid $next
        $g = $next
    }
    #printGrid $g
    Write-Host Part1 = $g.ToString().GetEnumerator().where{ $_ -eq '#' }.count #2126
}

function part2 {
    $changed = $true
    while ($changed) {
        $changed = $false
        $next = copyGrid $g
        for ($p = $begin; $p -lt $end; ++$p) {

            if ($g[$p] -eq 'L') {
                $occupied = $around.ForEach{
                    for ($n = $p + $_; $g[$n] -eq '.'; $n += $_) {}
                    if ($g[$n] -eq '#') { 1 }
                }.count
                if (!$occupied) {
                    $next[$p] = '#'
                    $changed = $true
                }
            }
            elseif ($g[$p] -eq '#') {
                $occupied = $around.ForEach{
                    for ($n = $p + $_; $g[$n] -eq '.'; $n += $_) {}
                    if ($g[$n] -eq '#') { 1 }
                }.count
                if ($occupied -ge 5) {
                    $next[$p] = 'L'
                    $changed = $true
                }
            }
        }
        #printGrid $next
        $g = $next
    }
    #printGrid $g
    Write-Host Part2 = $g.ToString().GetEnumerator().where{ $_ -eq '#' }.count #1914
}

#(Measure-Command { part1 }).TotalMinutes
(Measure-Command { part2 }).TotalMinutes
