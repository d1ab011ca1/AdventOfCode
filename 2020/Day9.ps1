$d = @"
35
20
15
25
47
40
62
55
65
95
102
117
150
182
127
219
299
277
309
576
"@ -split '\r?\n'
$preambleLen = 5
$d = Get-Content "$PSScriptRoot/Day9.txt"
$preambleLen = 25
$d = $d.ForEach{ [long]$_ }

:part1 
for ($i = $preambleLen; $i -lt $d.count; ++$i) {
    $sum = $d[$i]
    for ($j = $i - $preambleLen; $j -lt $i; ++$j) {
        $dj = $d[$j]
        for ($k = $j + 1; $k -lt $i; ++$k) {
            if ($dj + $d[$k] -eq $sum) {
                continue part1
            }
        }
    }
    $val = $sum
    Write-Host Part1 = $sum #29221323
    break
}


$f = 0
$sum = $d[0]
for ($i = 1; $i -lt $d.count; ++$i) {
    $sum += $d[$i]
    while ($sum -gt $val -AND $f -le $i) {
        $sum -= $d[$f]
        $f += 1
    }
    if ($sum -eq $val) {
        $m = $d[$f..$i] | measure -Minimum -Maximum
        $min = $m.Minimum
        $max = $m.Maximum
        Write-Host Part2 $min + $max = ($min + $max) # 1149640 + 3239729 = 4389369
        break
    }
}

