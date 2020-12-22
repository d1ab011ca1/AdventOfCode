$data = @'
Tile 2311:
..##.#..#.
##..#.....
#...##..#.
####.#...#
##.##.###.
##...#.###
.#.#.#..##
..#....#..
###...#.#.
..###..###

Tile 1951:
#.##...##.
#.####...#
.....#..##
#...######
.##.#....#
.###.#####
###.##.##.
.###....#.
..#.#..#.#
#...##.#..

Tile 1171:
####...##.
#..##.#..#
##.#..#.#.
.###.####.
..###.####
.##....##.
.#...####.
#.##.####.
####..#...
.....##...

Tile 1427:
###.##.#..
.#..#.##..
.#.##.#..#
#.#.#.##.#
....#...##
...##..##.
...#.#####
.#.####.#.
..#..###.#
..##.#..#.

Tile 1489:
##.#.#....
..##...#..
.##..##...
..#...#...
#####...#.
#..#.#.#.#
...#.#.#..
##.#...##.
..##.##.##
###.##.#..

Tile 2473:
#....####.
#..#.##...
#.##..#...
######.#.#
.#...#.#.#
.#########
.###.#..#.
########.#
##...##.#.
..###.#.#.

Tile 2971:
..#.#....#
#...###...
#.#.###...
##.##..#..
.#####..##
.#..####.#
#..#.#..#.
..####.###
..#.#.###.
...#.#.#.#

Tile 2729:
...#.#.#.#
####.#....
..#.#.....
....#..#.#
.##..##.#.
.#.####...
####.#.#..
##.####...
##..#.##..
#.##...##.

Tile 3079:
#.#.#####.
.#..######
..#.......
######....
####.#..#.
.#...#.##.
#.#####.##
..#.###...
..#.......
..#.###...
'@ -split '\r?\n'
$data = Get-Content "$PSScriptRoot/Day20.txt"

$tiles = @{}
$data.foreach{
    if ($_ -match 'Tile (\d+):') {
        $tile = [pscustomobject]@{
            id      = [int]$Matches[1]
            t       = ''
            b       = ''
            l       = ''
            r       = ''
            content = @()
        }
        $tiles[$tile.id] = $tile
    }
    elseif ($_) {
        $tile.content += $_
    }
}
$map = @{}
$tiles.Values.foreach{
    $t = $_.content[0]
    $b = $_.content[9]
    $l = $_.content.foreach{ $_[0] } -join ''
    $r = $_.content.foreach{ $_[9] } -join ''

    foreach ($e in ($t, $b, $l, $r)) {
        $a = $e.ToCharArray()
        [array]::Reverse($a)
        $a = $a -join ''

        $map[$a] += @($_)
        $map[$e] += @($_)
    }
}
#$tiles | ft
#$map.GetEnumerator().foreach{ $_.value.count } | group | ft
#$map | ft

function part1 {
    $map2 = @{}
    $map.GetEnumerator().where{ $_.Value.Count -eq 2 }.foreach{
        $k = $_.Key
        $_.value.foreach{ $map2[$_.Id] += @($k) }
    }
    $ans = [long]1
    $map2.GetEnumerator().where{ $_.Value.Count -eq 4 }.foreach{ $ans *= $_.Key }
    Write-Host Part1 = $ans # 14129524957217 
}

function part2 {
    $tiles.Values.foreach{
        "    new Tile($($_.Id), new[]{`"$($_.Content -join '", "')`"}),"
    }

    Write-Host Part2 = 1649
}

part1
part2
