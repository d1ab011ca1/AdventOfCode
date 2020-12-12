$data = @"
F10
N3
F7
R90
F11
"@ -split '\r?\n'
$data = Get-Content "$PSScriptRoot/Day12.txt"
$data = $data.foreach{ [pscustomobject]@{ inst = $_[0]; n = [int]$_.Substring(1) } }
#$data | group inst | sort count -Descending
#return

function part1 {
    $dir = 'NESW'
    $x, $y, $d = 0, 0, 1
    ForEach ($e in $data) {
        $inst = if ($e.inst -eq 'F') { $dir[$d] } else { $e.inst }
        switch ($inst) {
            'L' { $d = ($d + 4 - [int]($e.n / 90)) % 4; break }
            'R' { $d = ($d + [int]($e.n / 90)) % 4; break }
            'N' { $y += $e.n; break }
            'S' { $y -= $e.n; break }
            'E' { $x += $e.n; break }
            'W' { $x -= $e.n; break }
        }
    }
    Write-Host Part1 = x=$x, y=$y, d=$d, md=$([math]::Abs($x) + [math]::Abs($y)) # md=1482
}

function part2 {
    $x, $y = 0, 0
    $wx, $wy = 10, 1
    ForEach ($e in $data) {
        switch ($e.inst) {
            'F' { $x += $wx * $e.n; $y += $wy * $e.n; break }
            'L' { $wx, $wy = if ($e.n -eq 180) { - $wx, - $wy } elseif ($e.n -eq 90) { - $wy, + $wx } else { + $wy, - $wx }; break }
            'R' { $wx, $wy = if ($e.n -eq 180) { - $wx, - $wy } elseif ($e.n -eq 270) { - $wy, + $wx } else { + $wy, - $wx }; break }
            'N' { $wy += $e.n; break }
            'S' { $wy -= $e.n; break }
            'E' { $wx += $e.n; break }
            'W' { $wx -= $e.n; break }
        }
    }
    Write-Host Part2 = x=$x, y=$y, wx=$wx, wy=$wy, md=$([math]::Abs($x) + [math]::Abs($y)) # md=48739
}

(Measure-Command { part1 }).TotalMinutes
(Measure-Command { part2 }).TotalMinutes
