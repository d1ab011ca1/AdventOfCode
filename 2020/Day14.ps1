$data = Get-Content "$PSScriptRoot/Day14.txt"

function part1 {
    $data2 = @"
mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0
"@ -split '\r?\n'

    $mem = @{}
    $mask = 0UL
    $maskval = 0UL
    $data.foreach{ 
        if ($_ -notmatch 'mask = (.*)|mem\[(\d+)\] = (\d+)') { throw "NO MATCH: $_" }
        if ($Matches[1]) {
            $mask = [convert]::ToUInt64((($Matches[1] -replace '1', '0') -replace 'X', '1'), 2)
            $maskval = [convert]::ToUInt64(($Matches[1] -replace 'X', '0'), 2)
        }
        else {
            $addr = [int]$Matches[2]
            $val = [long]$Matches[3]
            $mem[$addr] = $val -band $mask -bor $maskval
        }
    }
    $p1 = $mem.Values | measure -Sum
    Write-Host Part1 = $p1.Sum # 5055782549997
}

function part2 {
    $data2 = @"
mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1
"@ -split '\r?\n'

    function toBase2([long]$v) { [Convert]::ToString($v, 2).PadLeft(36, '0') }
    function bitCount([long]$v) { [Convert]::ToString($v, 2).Replace('0', '').Length }

    $source = @"
namespace AOC
{
    using System;
    using System.Linq;
    public static class Day14a {
        public static string[] GenAddresses(string mask)
        {
            var parts = mask.Split('X');
            var fmt = parts[0] + String.Join("", parts.Skip(1).Select((s, i) => $"{{{i}}}{s}"));
            return Enumerable.Range(0, 1 << parts.Length - 1)
                .Select(i => Convert.ToString(i, 2).PadLeft(parts.Length - 1, '0').ToCharArray())
                .Select(bits => String.Format(fmt, bits.OfType<object>().ToArray()))
                .ToArray();
        }
    }
}
"@
    Add-Type -TypeDefinition $source -Language CSharp

    $inst = $data.foreach{ 
        if ($_ -notmatch 'mask = (.*)|mem\[(\d+)\] = (\d+)') { throw "NO MATCH: $_" }
        if ($Matches[1]) {
            $Amask = [convert]::ToInt64(($Matches[1] -replace 'X', '0'), 2)
            $Xmask = $Matches[1] -replace '1', '0'
            $xs = for ($i = 0; $i -lt $Xmask.Length; $i++) {
                if ($Xmask[$i] -eq 'X') {
                    $i
                }
            }
        }
        else {
            $addr = [long]$Matches[2]
            $val = [long]$Matches[3]
            $addr = toBase2 ($addr -bor $Amask)
            $XAddr = [text.stringbuilder]::new($addr)
            $xs.foreach{ $XAddr[$_] = 'X' }
            [pscustomobject]@{
                XAddr = $XAddr
                Value = $val
            }
        }
    }

    function print {
        $inst | Format-Table | Out-String | Write-Host
    }
    
    $inst = @($inst)
    [array]::Reverse($inst)
    #print

    $mem = @{}
    foreach ($i in $inst) {
        foreach ($addr in [AOC.Day14a]::GenAddresses($i.XAddr)) {
            if (!$mem.ContainsKey($addr)) {
                #Write-Host $addr = $i.Value
                $mem.Add($addr, $i.Value)
            }
        }
    }

    $sum = ($mem.Values | measure -sum).Sum
    Write-Host mem size = $mem.Count
    Write-Host Part2 = $sum # 4795970362286
}

part1
part2
