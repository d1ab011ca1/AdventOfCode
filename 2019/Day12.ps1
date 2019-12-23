# https://adventofcode.com/2019/day/12
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function parseInput($source) {
    if (!$source) {
        $source = @'
<x=12, y=0, z=-15>
<x=-8, y=-5, z=-10>
<x=7, y=-17, z=1>
<x=2, y=-11, z=-6>
'@
    }
    # "`$source = @('$((Get-Clipboard) -join "', '")')" | Set-Clipboard
    # $source = @('1', '2', '3')

    foreach ($line in $source -split '\r?\n') {
        if ($line -notmatch '^\s*<x=(.+), y=(.+), z=(.+)>') {throw "Parsing error: $line"}
        [PSCustomObject]@{
            posX = [int]$Matches[1]
            posY = [int]$Matches[2]
            posZ = [int]$Matches[3]
            velX = 0
            velY = 0
            velZ = 0
        }
    }
}
#return parseInput

function part1 {
    $steps = 1000
    $moons = parseInput

#    $steps = 10
#    $moons = parseInput @'
#    <x=-1, y=0, z=2>
#    <x=2, y=-10, z=-7>
#    <x=4, y=-8, z=8>
#    <x=3, y=5, z=-1>
#'@

    $moons | Format-Table
    for ($step = 0; $step -lt $steps; $step++) {
        # Apply gravity to all pairs
        for ($im = 0; $im -lt $moons.Count; $im++) {
            for ($in = $im + 1; $in -lt $moons.Count; $in++) {
                $m = $moons[$im]
                $n = $moons[$in]
                if ($n.posX -lt $m.posX) { $n.velX += 1; $m.velX -= 1 }
                elseif ($n.posX -gt $m.posX) { $n.velX -= 1; $m.velX += 1 }
                if ($n.posY -lt $m.posY) { $n.velY += 1; $m.velY -= 1 }
                elseif ($n.posY -gt $m.posY) { $n.velY -= 1; $m.velY += 1 }
                if ($n.posZ -lt $m.posZ) { $n.velZ += 1; $m.velZ -= 1 }
                elseif ($n.posZ -gt $m.posZ) { $n.velZ -= 1; $m.velZ += 1 }
            }
        }

        # Apply velocity
        foreach ($moon in $moons) {
            $moon.posX += $moon.velX
            $moon.posY += $moon.velY
            $moon.posZ += $moon.velZ
        }

        #Write-Host "After step $($step + 1):"
        #$moons | Format-Table
    }

    # calculate energy
    $total = 0
    foreach ($moon in $moons) {
        $pot = [math]::abs($moon.posX) + [math]::abs($moon.posY) + [math]::abs($moon.posZ)
        $kin = [math]::abs($moon.velX) + [math]::abs($moon.velY) + [math]::abs($moon.velZ)
        $total += $pot * $kin
        [PSCustomObject]@{
            pot = $pot
            kin = $kin
            total = $pot * $kin
        }
    }
    $total
}
#part1 #= 7636

function part2 {

    $moons = parseInput @'
    <x=-1, y=0, z=2>
    <x=2, y=-10, z=-7>
    <x=4, y=-8, z=8>
    <x=3, y=5, z=-1>
'@

$moons = parseInput

    $initial = @{
        posX = $moons.posX -join ','
        posY = $moons.posY -join ','
        posZ = $moons.posZ -join ','
    }

    foreach ($axis in ('X','Y','Z')) {
        $pos = 'pos' + $axis
        $vel = 'vel' + $axis

        for ($step = 1; $true; $step++) {
            if ($step % 1000 -eq 0) {"Step $step"}
    
            # Apply gravity to all pairs
            for ($im = 0; $im -lt $moons.Count; $im++) {
                for ($in = $im + 1; $in -lt $moons.Count; $in++) {
                    $m = $moons[$im]
                    $n = $moons[$in]
                    if ($n.$pos -lt $m.$pos) { $n.$vel += 1; $m.$vel -= 1 }
                    elseif ($n.$pos -gt $m.$pos) { $n.$vel -= 1; $m.$vel += 1 }
                }
            }
    
            # Apply velocity
            foreach ($moon in $moons) {
                $moon.$pos += $moon.$vel
            }
    
            if (($moons.$vel -join ',') -eq '0,0,0,0') {
                if (($moons.$pos -join ',') -eq $initial.$pos) {
                    "Axis $axis : $step"
                    $moons | Format-Table
                    $initial."axis$axis" = $step
                    break #next axis
                }
            }
        }
    }

    function LCM($x,$y,$z) {"wolframaplha.com: LCM $x $y $z"}
    LCM $initial.axisX $initial.axisY $initial.axisZ
}
part2 #= LCM 161428 193052 144624 = 281691380235984
