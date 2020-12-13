$data = @"
939
7,13,x,x,59,x,31,19
"@ -split '\r?\n'
$data = Get-Content "$PSScriptRoot/Day13.txt"

$ts = [long]$data[0]
$ids = $data[1] -split ','

function part1 {
    $x = $ids.where{ $_ -ne 'x' }.foreach{ [int]$_ }.foreach{ [pscustomobject]@{ id = $_; next = ([math]::Floor($ts / $_) + 1) * $_ } } 
    $p1 = $x | sort next | select -f 1
    Write-Host Part1 = '(' $p1.next - $ts ')' * $p1.id = (($p1.next - $ts) * $p1.id) # 
}

function part2 {
    $x = for ($i = 0; $i -lt $ids.Count; $i++) {
        if ($ids[$i] -ne 'x') {
            [pscustomobject]@{ id = [int]$ids[$i]; idx = $i; } 
        }
    }
    $first = $x[0].id
    $mult = $x.where{ $_.idx % $first -eq 0 }.id
    $source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
namespace AOC
{
    public static class Day13d {
        public static long Part2() {
            var result = new ConcurrentBag<long>();
            Parallel.For(0, 9, i =>
            {
                long start = i * 100000000000000L;
                long end = start + 100000000000000L;
                long inc = $($mult -join ' * ');
                for (long x = (start / inc) * inc; x < end; x += inc)
                {
                    if ($(
                        ($x | sort id -Descending | ? id -notin $mult | foreach { 
                            "(x + $($_.idx) - $first) % $($_.id) == 0"
                        }) -join " && `n"))
                    {
                        result.Add(x - $first);
                        return;
                    }
                }
            });
            return result.OrderBy(x=>x).First();
        }
    }
}
"@
    $source
    Add-Type -TypeDefinition $source -Language CSharp
    Write-Host Part2 = ([AOC.Day13d]::Part2()) # 626670513163231
}

part1
part2
