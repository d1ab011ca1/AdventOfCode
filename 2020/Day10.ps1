$d = @"
28
33
18
42
31
14
46
20
48
47
24
23
49
45
19
38
39
11
1
32
25
35
8
17
7
9
4
2
34
10
3
"@ -split '\r?\n'
$d = @"
16
10
15
5
1
11
7
19
6
12
4
"@ -split '\r?\n'
$d = Get-Content "$PSScriptRoot/Day10.txt"
$d = $d.ForEach{ [int]$_ }

#$part1 = $d | measure -Maximum
#$part1.Maximum + 3
$d = $d | sort
$p = 0
$x = foreach ($i in $d) {
    $i - $p
    $p = $i
}
$p1 = $x + @(3) | group -AsHashTable
Write-Host Part1 = $p1[1].Count * $p1[3].Count = ($p1[1].Count * $p1[3].Count) # 
#$x

$p = 0
$p2 = for ($i = 1; $i -lt $x.count; ++$i) {
    if ($x[$i] -ne $x[$p]) { if ($x[$p] -eq 1) { ($i - $p); } $p = $i }
}
$p2 += ($i - $p)
#$p2

$n = 1
switch ($p2) {
    2 { $n *= 2 }
    3 { $n *= 4 }
    4 { $n *= 7 }
}
Write-Host Part2 = $n # 16198260678656