# https://adventofcode.com/2019/day/9
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

class Point {
    [double] $x
    [double] $y

    Point([double] $x, [double] $y) {
        $this.x = $x
        $this.y = $y
    }
}

function parseInput($source) {
    if (!$source) {
        $source = @'
        ###..#########.#####.
        .####.#####..####.#.#
        .###.#.#.#####.##..##
        ##.####.#.###########
        ###...#.####.#.#.####
        #.##..###.########...
        #.#######.##.#######.
        .#..#.#..###...####.#
        #######.##.##.###..##
        #.#......#....#.#.#..
        ######.###.#.#.##...#
        ####.#...#.#######.#.
        .######.#####.#######
        ##.##.##.#####.##.#.#
        ###.#######..##.#....
        ###.##.##..##.#####.#
        ##.########.#.#.#####
        .##....##..###.#...#.
        #..#.####.######..###
        ..#.####.############
        ..##...###..#########
'@
    }
    
    $y = 0
    foreach ($line in $source -split '\r?\n') {
        $line = $line.Trim().where{$_}
        for ($x = 0; $x -lt $line.Length; $x++) {
            if ($line.Chars($x) -eq '#') {
                [Point]::New($x,$y)
            }
        }
        ++$y
    }
}
#return parseInput

function part1 {
    $in = parseInput @'
    .#....#####...#..
##...##.#####..##
##...#...#.#####.
..#.....X...###..
..#.#.....#....##
'@
    $in = parseInput
    $output = foreach ($a in $in) {
        $vectors = foreach ($b in $in) {
            if ($a -eq $b) {continue}
            $dx = $a.x - $b.x
            $dy = $a.y - $b.y
            if ($dy -eq 0) {
                $ratio = [double]::MaxValue
            } else {
                $ratio = [math]::Round([math]::Abs($dx / $dy), 6)
            }
            "$([math]::Sign($dx)),$([math]::Sign($dy)),$ratio"
        }

        [PSCustomObject]@{
            Name = "$($a.x),$($a.y)"
            Count = ($vectors | select -Unique).Count
        }
    }
    $output | sort Count | select -Last 3
}
#part1 #= 221 at 11,11

function part2 {
    $in = parseInput @'
    .#....#####...#..
    ##...##.#####..##
    ##...#...#.#####.
    ..#.....#...###..
    ..#.#.....#....##
'@
    $x,$y = 8,3

    $in = parseInput @'
    .#..##.###...#######
    ##.############..##.
    .#.######.########.#
    .###.#######.####.#.
    #####.##.#.##.###.##
    ..#####..#.#########
    ####################
    #.####....###.#.#.##
    ##.#################
    #####.##.###..####..
    ..######..##.#######
    ####.##.####...##..#
    .#####..#.######.###
    ##...#.##########...
    #.##########.#######
    .####.#.###.###.#.##
    ....##.##.###..#####
    .#.#.###########.###
    #.#.#.#####.####.###
    ###.##.####.##.#..##
'@
    $x,$y = 11,13


    $in = parseInput
    $x,$y = 11,11

    $a = [Point]::new($x,$y)
    $vectors = foreach ($b in $in) {
        $dx = $b.x - $a.x
        $dy = $b.y - $a.y
        $dist = $dx*$dx + $dy*$dy
        if ($dist -eq 0) {continue}
        if ($dx -eq 0) {
            $angle = if ($dy -lt 0){0.0}else{180.0}
        } else {
            $angle = [math]::Atan([math]::Round($dy / $dx, 6)) * 180 / [math]::PI
            $angle += 90.0 * (1 - [math]::Sign($dx)) * [math]::Sign(0.5 + $dy)
            # angle is -180..180 clockwise with 0 at (+dx,0)
            # convert to 0..360 with 0 at (0,-dy)
            $angle = ($angle + 90 + 360) % 360.0
        }
        [PSCustomObject]@{
            Name = "$($b.x),$($b.y)"
            Angle = $angle
            Dist = $dist
        }
    }

    $groups = $vectors | sort Angle,Dist | Group Angle | %{@(,$_.group)}
    $n = 0
    do {
        $done = $true
        for ($i = 0; $i -lt $groups.Count; $i++) {
            $g = $groups[$i]
            if ($g) {
                ++$n
                #[PSCustomObject]@{
                #    N = $n
                #    Name = $g[0].Name
                #}
                if ($n -eq 200) {
                    return $g[0].Name
                }
                if ($g.count -eq 1) {
                    $groups[$i] = @()
                } else {
                    $groups[$i] = $g[1..($g.count - 1)]
                    $done = $false
                }
            }
        }
    } while(!$done)
}
part2 #= 806
