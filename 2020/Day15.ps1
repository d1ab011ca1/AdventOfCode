$data = 7, 14, 0, 17, 11, 1, 2

Add-Type -TypeDefinition @"
namespace AOC
{
    using System;
    using System.Collections.Generic;
    public static class Day15 {
        public static int DoIt(int[] data, int iterations)
        {
            var m = new Dictionary<int, int>();
            var speak = 0;
            var turn = 1;
            for (; turn <= data.Length; ++turn) {
                speak = data[turn - 1];
                m[speak] = turn;
            }
            var prev = -1;
            for (; turn <= iterations; ++turn) {
                speak = (prev == -1) ? 0 : turn - 1 - prev;
                prev = (m.ContainsKey(speak)) ? m[speak] : -1;
                m[speak] = turn;
            }
            return speak;
        }
    }
}
"@

Write-Host Part1 = ([AOC.Day15]::DoIt($data, 2020)) # 206
Write-Host Part2 = ([AOC.Day15]::DoIt($data, 30000000)) # 955
