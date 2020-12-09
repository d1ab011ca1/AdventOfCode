cls

$d = @"
light red bags contain 1 bright white bag, 2 muted yellow bags.
dark orange bags contain 3 bright white bags, 4 muted yellow bags.
bright white bags contain 1 shiny gold bag.
muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.
shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.
dark olive bags contain 3 faded blue bags, 4 dotted black bags.
vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.
faded blue bags contain no other bags.
dotted black bags contain no other bags.
"@ -split '\r?\n'
$d = Get-Content "$PSScriptRoot/Day7.txt"
$contains = @{}
$contained = @{}
$d.foreach{ 
    if ($_ -notmatch '^(.*) bags contain (no other bags|(.*))\.$') { throw "REGEX MISS 1: $_" }
    $color = $Matches[1]
    if (!$contains.ContainsKey($color)) { $contains.Add($color, @{}) }
    $inner = $contains[$color]
    if ($Matches[3]) {
        ($Matches[3] -split ', ').foreach{ 
            if ($_ -notmatch '(\d+) (.*) bags?') { throw "REGEX MISS 2: $_" }
            if (!$inner.ContainsKey($Matches[2])) { $inner.Add($Matches[2], 0) }
            $inner[$Matches[2]] += [int]$Matches[1]

            if (!$contained.ContainsKey($Matches[2])) { $contained.Add($Matches[2], @()) }
            $contained[$Matches[2]] += $color
        }
    }
}

#$contains
#$contained

$total = @()
$b = $contained['shiny gold']
while ($b) {
    $total += $b
    $b = $b.foreach{ if ($contained.ContainsKey($_)) { $contained[$_] } }
}
$part1 = $total | select -Unique | measure
Write-Host Part1 = $part1.Count # 112

$counts = @{}
function foo($color) {
    if (!$counts.ContainsKey($color)) {
        $counts.Add($color, 0)
        $inner = $contains[$color]
        foreach ($k in $inner.Keys) {
            $b = foo $k
            $counts[$color] += $inner[$k] * (1 + $b)
        }
    }
    $counts[$color]
}
$part2 = foo 'shiny gold'
Write-Host Part2 = $part2 # 6260
