$data = @'
42: 9 14 | 10 1
9: 14 27 | 1 26
10: 23 14 | 28 1
1: "a"
11: 42 31
5: 1 14 | 15 1
19: 14 1 | 14 14
12: 24 14 | 19 1
16: 15 1 | 14 14
31: 14 17 | 1 13
6: 14 14 | 1 14
2: 1 24 | 14 4
0: 8 11
13: 14 3 | 1 12
15: 1 | 14
17: 14 2 | 1 7
23: 25 1 | 22 14
28: 16 1
4: 1 1
20: 14 14 | 1 15
3: 5 14 | 16 1
27: 1 6 | 14 18
14: "b"
21: 14 1 | 1 14
25: 1 1 | 1 14
22: 14 14
8: 42
26: 14 22 | 1 20
18: 15 15
7: 14 5 | 1 21
24: 14 1

abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa
bbabbbbaabaabba
babbbbaabbbbbabbbbbbaabaaabaaa
aaabbbbbbaaaabaababaabababbabaaabbababababaaa
bbbbbbbaaaabbbbaaabbabaaa
bbbababbbbaaaaaaaabbababaaababaabab
ababaaaaaabaaab
ababaaaaabbbaba
baabbaaaabbaaaababbaababb
abbbbabbbbaaaababbbbbbaaaababb
aaaaabbaabaaaaababaa
aaaabbaaaabbaaa
aaaabbaabbaaaaaaabbbabbbaaabbaabaaa
babaaabbbaaabaababbaabababaaab
aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba
'@ -split '\r?\n'
$data = Get-Content "$PSScriptRoot/Day19.txt"

$rules = @{}
$deps = @{}
$rdeps = @{}
$msgs = $null
$data.foreach{
    if ($_.length -eq 0) { $msgs = @(); return }
    if ($null -ne $msgs) {
        $msgs += $_
    }
    else {
        if ($_ -notmatch '^(\d+): ("([^"]+)"|(.*))$') { throw "No match: $_" }
        $rule = $Matches[1]
        $subrule = $Matches[4]
        $rules.Add($rule, $Matches[3] + $subrule)
        $deps[$rule] = $subrule -split ' \| ' -split ' ' | select -Unique | where { $_ }
        foreach ($sr in $deps[$rule]) {
            $rdeps[$sr] += @($rule)
        }
    }
}
#$rules | ft
#$msgs | ft
#$deps | ft
#$rdeps | ft
#return

function get([string]$rule, [switch]$fullLine) {
    $pattern = $rules[$rule]
    $subrules = $deps[$rule]
    while ($subrules) {
        #Write-Host "'$pattern' : $subrules"
        $subrules = foreach ($sr in $subrules) {
            $p = $rules[$sr]
            $pattern = $pattern -replace ("\b$sr\b", "($p)")
            if ($deps.ContainsKey($sr)) { $deps[$sr] }
        }
    }
    
    $pattern = $pattern.replace('(a)', 'a').replace('(b)', 'b').replace(' ', '')

    if ($fullLine) {
        $pattern = "^($pattern)`$"
    }
    [regex]::new($pattern, 'ExplicitCapture')
}

function part1 {
    $re = get '0' -fullLine
    Write-Host $re

    $ans = $msgs.where{ $re.IsMatch($_) }.Count
    Write-Host Part1 = $ans # 180 
}

function part2 {
    # 0 = 8 11
    # 8 = 42 | 42 8 = 42+
    # 11 = 42 31 | 42 11 31 = 42 (42...31)* 31
    # 0 => 42+ 42 (42...31)* 31
    [regex]$re42 = get '42'
    [regex]$re31 = get '31'
    Write-Host 42 = $re42
    Write-Host 31 = $re31

    $ans = $msgs.where{ 
        $42, $31 = 0, 0
        $start = 0
        while (($m = $re42.Match($_, $start)).Success -AND $m.Index -eq $start) {
            ++$42
            $start = $m.Index + $m.Length
        }
        if ($42 -ge 2) {
            while (($m = $re31.Match($_, $start)).Success -AND $m.Index -eq $start) {
                ++$31
                $start = $m.Index + $m.Length
            }
            if ($start -eq $_.Length -AND $31 -ge 1 -AND $42 -gt $31) {
                Write-Host $_
                return $true
            }
        }
    }.Count
    Write-Host Part2 = $ans # 323
}

part1
part2
