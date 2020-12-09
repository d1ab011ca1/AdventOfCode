cls

$d = Get-Content "$PSScriptRoot/Day6.txt"
#$d = 'abc
#
#a
#b
#c
#
#ab
#ac
#
#a
#a
#a
#a
#
#b' -split '\r?\n'
$d = $d | % { $g = @() } { if (!$_) { @{g = $g }; $g = @() } else { $g += $_ } } { @{g = $g } }

$part1 = $d.foreach{ (($_.g -join '').GetEnumerator() | select -unique).Count } | measure -sum
Write-Host Part1 = $part1.Sum  # 6630

$part2 = $d.foreach{ @(($_.g -join '').GetEnumerator() | group | ? Count -eq $_.g.count).Count } | measure -sum
Write-Host Part2 = $part2.Sum  # 3437
