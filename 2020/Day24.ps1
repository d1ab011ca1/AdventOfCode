$data = @'
sesenwnenenewseeswwswswwnenewsewsw
neeenesenwnwwswnenewnwwsewnenwseswesw
seswneswswsenwwnwse
nwnwneseeswswnenewneswwnewseswneseene
swweswneswnenwsewnwneneseenw
eesenwseswswnenwswnwnwsewwnwsene
sewnenenenesenwsewnenwwwse
wenwwweseeeweswwwnwwe
wsweesenenewnwwnwsenewsenwwsesesenwne
neeswseenwwswnwswswnw
nenwswwsewswnenenewsenwsenwnesesenew
enewnwewneswsewnwswenweswnenwsenwsw
sweneswneswneneenwnewenewwneswswnese
swwesenesewenwneswnwwneseswwne
enesenwswwswneneswsenwnewswseenwsese
wnwnesenesenenwwnenwsewesewsesesew
nenewswnwewswnenesenwnesewesw
eneswnwswnwsenenwnwnwwseeswneewsenese
neswnwewnwnwseenwseesewsenwsweewe
wseweeenwnesenwwwswnew
'@ -split '\r?\n'
$data = Get-Content "$PSScriptRoot/Day24.txt"

$re = [regex]'(e|w|se|sw|ne|nw)'
$around = @{
    e  = [tuple]::Create(+2, 0)
    w  = [tuple]::Create(-2, 0)
    ne = [tuple]::Create(+1, +2)
    nw = [tuple]::Create(-1, +2)
    se = [tuple]::Create(+1, -2)
    sw = [tuple]::Create(-1, -2)
}
$tiles = @{}
$data.foreach{
    $x, $y = 0, 0
    foreach ($m in $re.Matches($_)) {
        $offset = $around[$m.value]
        $x += $offset.Item1
        $y += $offset.Item2
    }
    $loc = [tuple]::Create($x, $y)
    if ($tiles.ContainsKey($loc)) { $tiles.Remove($loc) } else { $tiles.Add($loc, 'B') }
}
#$tiles | ft | Out-String | Write-Host

function part1 {
    Write-Host Part 1 = $tiles.Count # 266
}

function part2 {
    Add-Type -Namespace 'AOC' -Name 'Day24' -UsingNamespace 'System.Linq', 'System.Collections', 'System.Collections.Generic' -ea Stop -MemberDefinition @"
        public static int Helper() {
            return 123;
        }
"@

    $around = $around.Values

    for ($iter = 0; $iter -lt 100; $iter++) {
        Write-Host Iteration $iter : $tiles.Values.where{ $_ -eq 'B' }.Count # 

        # Add in all adjacent white tiles
        foreach ($t in @($tiles.Keys)) {
            $around.foreach{
                $loc = [tuple]::Create($t.Item1 + $_.Item1, $t.Item2 + $_.Item2)
                if (!$tiles.ContainsKey($loc)) { $tiles[$loc] = 'W' }
            }
        }

        Write-Host Checking $tiles.Count tiles
        $next = @{}
        $tiles.GetEnumerator().ForEach{
            $loc = $_.Key
            $color = $_.Value

            # the tiles are all flipped according to the following rules:
            # - Any black tile with zero or more than 2 black tiles immediately 
            #   adjacent to it is flipped to white.
            # - Any white tile with exactly 2 black tiles immediately adjacent 
            #   to it is flipped to black.
            $numBlack = 0
            foreach ($offset in $around) {
                $near = [tuple]::Create($loc.Item1 + $offset.Item1, $loc.Item2 + $offset.Item2)
                if ($tiles[$near] -eq 'B') {
                    ++$numBlack
                    if ($numBlack -ge 3) {
                        break
                    }
                }
            }
        
            if ($color -eq 'B') {
                if ($numBlack -eq 0 -OR $numBlack -gt 2) {
                    # change to white
                }
                else {
                    # stay black
                    $next[$loc] = 'B' 
                }
            }
            else {
                # white
                if ($numBlack -eq 2) {
                    # change to black
                    $next[$loc] = 'B'
                }
            }
        }

        $tiles = $next
    }

    Write-Host Part 2 = $tiles.Values.where{ $_ -eq 'B' }.Count # 3627
}

(Measure-Command { part1 }).TotalSeconds
(Measure-Command { part2 }).TotalSeconds
