$d = @"
nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6
"@ -split '\r?\n'
$d = Get-Content "$PSScriptRoot/Day8.txt"
$inst = $d.foreach{ $i, $v = $_ -split ' '; [pscustomobject]@{i = $i; v = [int]$v } }

function run {
    $acc = 0
    $ip = 0
    $m = @{}
    while ($ip -ge 0 -AND $ip -lt $inst.count -AND !$m.ContainsKey($ip)) {
        $m[$ip] = $null
        $i = $inst[$ip]
        switch ($i.i) {
            'nop' { $ip += 1 }
            'acc' { $ip += 1; $acc += $i.v }
            'jmp' { $ip += $i.v }
        }
    }
    return $acc, $ip
}

$acc, $ip = run
Write-Host Part1 = $acc  # 1753

foreach ($x in $inst) {
    if ($x.i -eq 'acc') { continue }
    $x.i = if ($x.i -eq 'nop') { 'jmp' } else { 'nop' }

    $acc, $ip = run
    if ($ip -ge $inst.count) {
        Write-Host Part2 = $acc  # 733
        break
    }

    $x.i = if ($x.i -eq 'nop') { 'jmp' } else { 'nop' }
}
