$data = @"
.#.
..#
###
"@ -split '\r?\n'
$data = Get-Content "$PSScriptRoot/Day17.txt"

$maxExtent = $data.Count
$max = [int][math]::Ceiling(($maxExtent / 2) - 1)
$min = $max - $maxExtent + 1
#$min, $max

function part1 {
    Add-Type -TypeDefinition @"
namespace AOC
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    public static class Day17_1e {
        public static Hashtable DoIt(Hashtable s, int maxExtent)
        {
            var max = (int)Math.Ceiling((maxExtent / 2.0) - 1);
            var min = max - maxExtent + 1;
        
            var s2 = new Hashtable(s);
        
            for (var z = min; z <= max; z++)
            {
                for (var y = min; y <= max; y++)
                {
                    for (var x = min; x <= max; x++)
                    {
                        var xyz = Tuple.Create(x, y, z);
                        var active = s.ContainsKey(xyz);
        
                        var activeNeighbors = 0;
                        _ = around
                            .Where(a => s.ContainsKey(Add(a, xyz)))
                            .TakeWhile(_ => ++activeNeighbors <= 3)
                            .Count();
        
                        // If a cube is active and exactly 2 or 3 of its neighbors are also active, 
                        // the cube remains active. Otherwise, the cube becomes inactive.
                        if (active && !(activeNeighbors == 2 || activeNeighbors == 3))
                        {
                            s2.Remove(xyz);
                        }
                        // If a cube is inactive but exactly 3 of its neighbors are active, 
                        // the cube becomes active. Otherwise, the cube remains inactive.
                        else if (!active && activeNeighbors == 3)
                        {
                            s2.Add(xyz, null);
                        }
                    }
                }
            }
        
            return s2;
        }
        
        static Tuple<int, int, int>[] around = 
            Enumerable.Range(0, 3*3*3)
                      .Where(n => n != 1 + 3 + 9) // skip (0,0,0)
                       .Select(n => Tuple.Create((n / 1 % 3) - 1, (n / 3 % 3) - 1, (n / 9 % 3) - 1))
                      .ToArray();
        
        static Tuple<int, int, int> Add(Tuple<int, int, int> t1, Tuple<int, int, int> t2) {
            return Tuple.Create(t1.Item1 + t2.Item1, t1.Item2 + t2.Item2, t1.Item3 + t2.Item3);
        }
    }
}
"@
    
    $s = @{}
    $z = 0
    for ($y = $min; $y -le $max; $y++) {
        $row = $data[$y - $min]
        for ($x = $min; $x -le $max; $x++) {
            if ($row[$x - $min] -eq '#') {
                $s.Add([Tuple]::Create($x, $y, $z), $null)
                #Write-Host "{Tuple.Create($x, $y, $z), null},"
            }
        }
    }

    foreach ($cycle in (1..6)) {
        $maxExtent += 2
        $s = [AOC.Day17_1e]::DoIt($s, $maxExtent)
    }
    Write-Host Part1 = $s.Count # 257
}

function part2 {
    Add-Type -TypeDefinition @"
namespace AOC
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    public static class Day17_2a {
        public static Hashtable DoIt(Hashtable s, int maxExtent)
        {
            var max = (int)Math.Ceiling((maxExtent / 2.0) - 1);
            var min = max - maxExtent + 1;
        
            var s2 = new Hashtable(s);
        
            for (var z = min; z <= max; z++)
            {
                for (var y = min; y <= max; y++)
                {
                    for (var x = min; x <= max; x++)
                    {
                        for (var w = min; w <= max; w++)
                        {
                            var xyz = Tuple.Create(w, x, y, z);
                            var active = s.ContainsKey(xyz);
        
                            var activeNeighbors = 0;
                            _ = around4D
                                .Where(a => s.ContainsKey(Add(a, xyz)))
                                .TakeWhile(_ => ++activeNeighbors <= 3)
                                .Count();
        
                            // If a cube is active and exactly 2 or 3 of its neighbors are also active, 
                            // the cube remains active. Otherwise, the cube becomes inactive.
                            if (active && !(activeNeighbors == 2 || activeNeighbors == 3))
                            {
                                s2.Remove(xyz);
                            }
                            // If a cube is inactive but exactly 3 of its neighbors are active, 
                            // the cube becomes active. Otherwise, the cube remains inactive.
                            else if (!active && activeNeighbors == 3)
                            {
                                s2.Add(xyz, null);
                            }
                        }
                    }
                }
            }
        
            return s2;
        }
        
        static Tuple<int, int, int, int>[] around4D = 
            Enumerable.Range(0, 3*3*3*3)
                      .Where(n => n != 1 + 3 + 9 + 27) // skip (0,0,0,0)
                       .Select(n => Tuple.Create((n / 1 % 3) - 1, (n / 3 % 3) - 1, (n / 9 % 3) - 1, (n / 27 % 3) - 1))
                      .ToArray();
        
        static Tuple<int, int, int, int> Add(Tuple<int, int, int, int> t1, Tuple<int, int, int, int> t2) {
            return Tuple.Create(t1.Item1 + t2.Item1, t1.Item2 + t2.Item2, t1.Item3 + t2.Item3, t1.Item4 + t2.Item4);
        }
    }
}
"@

    $s = @{}
    $z, $w = 0, 0
    for ($y = $min; $y -le $max; $y++) {
        $row = $data[$y - $min]
        for ($x = $min; $x -le $max; $x++) {
            if ($row[$x - $min] -eq '#') {
                $s.Add([Tuple]::Create($w, $x, $y, $z), $null)
                #Write-Host "{Tuple.Create($w, $x, $y, $z), null},"
            }
        }
    }

    foreach ($cycle in (1..6)) {
        $maxExtent += 2
        $s = [AOC.Day17_2a]::DoIt($s, $maxExtent)
    }
    Write-Host Part2 = $s.Count # 2532
}

part1
part2
