[CmdletBinding()]
param ()
cls

$d = Get-Content "$PSScriptRoot/Day4.txt"
$docs = $d | % { $m = @{} } { if (!$_) { $m; $m = @{} } else { ($_ -split ' ').foreach{ $n, $v = $_ -split ':'; $m.$n = $v } } } { $m }

$part1 = $docs.foreach{ 
    foreach ($p in 'byr', 'iyr', 'eyr', 'hgt', 'hcl', 'ecl', 'pid') { 
        if (!$_.ContainsKey($p)) { return }
    }
    $_
}
Write-Host part1 = $part1.Count

$part2 = $docs.foreach{ 
    foreach ($p in 'byr', 'iyr', 'eyr', 'hgt', 'hcl', 'ecl', 'pid') {
        $v = $_.$p
        if (!$v) { return }
        switch ($p) {
            'byr' { if ($v -notmatch '^\d\d\d\d$') { return }; $v = [int]$v; if ($v -lt 1920 -OR $v -gt 2002) { return } }
            'iyr' { if ($v -notmatch '^\d\d\d\d$') { return }; $v = [int]$v; if ($v -lt 2010 -OR $v -gt 2020) { return } }
            'eyr' { if ($v -notmatch '^\d\d\d\d$') { return }; $v = [int]$v; if ($v -lt 2020 -OR $v -gt 2030) { return } }
            'hgt' {
                if ($v -notmatch '^(?:(\d+)cm|(\d+)in)$') { return } 
                $v = [int]($Matches[1] + $Matches[2])
                if ($Matches[1] -AND ($v -lt 150 -OR $v -gt 193)) { return }
                if ($Matches[2] -AND ($v -lt 59 -OR $v -gt 76)) { return } 
            }
            'hcl' { if ($v -notmatch '^#[0-9a-f]{6}$') { return } }
            'ecl' { if ($v -notmatch '^(amb|blu|brn|gry|grn|hzl|oth)$') { return } }
            'pid' { if ($v -notmatch '^\d{9}$') { return } }
        }
    }
    $_ 
}
Write-Host part2 = $part2.Count
