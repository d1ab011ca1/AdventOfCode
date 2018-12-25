# https://adventofcode.com/2018/day/23
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

class Point : System.IComparable {
    [long] $X
    [long] $Y
    [long] $Z

    Point() {
    }
    Point([long]$x, [long]$y, [long]$z) {
        $this.X = $x
        $this.Y = $y
        $this.Z = $z
    }
    Point([Point]$pt) {
        $this.X = $pt.X
        $this.Y = $pt.Y
        $this.Z = $pt.Z
    }

    [Point] Offset([Point]$pt) { $this.X += $pt.X; $this.Y += $pt.Y; $this.Z += $pt.Z; return $this }
    [Point] Offset([long]$x, [long]$y, [long]$z) { $this.X += $X; $this.Y += $Y; $this.Z += $Z; return $this }

    [long] Distance() {
        return [math]::Abs($this.X) + [math]::Abs($this.Y) + [math]::Abs($this.Z)
    }
    [long] Distance([Point]$b) {
        return [math]::Abs($this.X - $b.X) + [math]::Abs($this.Y - $b.Y) + [math]::Abs($this.Z - $b.Z)
    }

    [Point] Clone() { return [Point]::new($this) }

    static [Point] op_Addition([Point]$a, [Point]$b) { return [Point]::new($a.X + $b.X, $a.Y + $b.Y, $a.Z + $b.Z) }
    static [Point] op_Subtraction([Point]$a, [Point]$b) { return [Point]::new($a.X - $b.X, $a.Y - $b.Y, $a.Z - $b.Z) }

    [int] GetHashCode() {
        return $this.X.GetHashCode() -bxor $this.Y.GetHashCode() -bxor $this.Z.GetHashCode()
    }
    [bool] Equals([object]$obj) {
        if ($obj.GetType() -ne [Point]) { return $false }
        return $this.CompareTo($obj) -eq 0
    }
    [int] CompareTo([object]$object) {
        if ($null -eq $object) { return 1 }
        [Point]$other = $object
        if ($null -eq $other) { throw [System.ArgumentException]'Object is not a Point' }
        $diff = $this.X.CompareTo($other.X)
        if ($diff -eq 0) {
            $diff = $this.Y.CompareTo($other.Y)
            if ($diff -eq 0) {
                $diff = $this.Z.CompareTo($other.Z)
            }
        }
        return $diff
    }

    [string] Format() {
        return '({0},{1},{2})' -f $this.X, $this.Y, $this.Z
    }
    [string] ToString() {
        return $this.Format()
    }
}

function main {
    #part1 #= 248 nanobots are in range of 16550473,27374441,-19147897.
    part2 #= 
}

function part1() {
    [Bot[]]$bots = parseInput

    $maxBot = $bots[0]
    foreach ($bot in $bots) {
        if ($bot.Radius -gt $maxBot.Radius) {
            $maxBot = $bot
        }
    }

    $inRange = foreach ($bot in $bots) {
        $dist = $bot.Pt.Distance($maxBot.Pt)
        if ($dist -le $maxBot.Radius) {
            $bot
        }
    }

    Write-Host "Part 1: $($inRange.Count) nanobots are in range of $($maxBot.Format())."
    Write-Host ''
}    

function part2() {
    Write-Host "Part 2: Incomplete."
    return

    [Bot[]]$bots = parseInput

    # Find all bots with overlapping ranges
    $overlap = @{}
    foreach ($a in 0..($bots.Count-2)) {
        $botA = $bots[$a]
        $overlap[$botA] = @($botA)
        foreach ($b in 0..($bots.Count-1)) {
            if ($a -eq $b) { continue }
            $botB = $bots[$b]
            $dist = $botA.Pt.Distance($botB.Pt)
            if ($dist -le $botA.Radius + $botB.Radius) {
                $overlap[$botA] += $botB
            }
        }
    }

    # now find the bot group(s) with the most overlap
    $mapOverlap = $overlap.GetEnumerator() | sort {$_.Value.Count} -Descending
    $mapOverlap = $mapOverlap.where{$_.Value.Count -eq $mapOverlap[0].Value.Count}

    # for each group, find the overlapping point closest to 0,0,0
    $mapOverlap.foreach{
        #Write-Host (($_.Value | sort Pt | foreach{$_.ToString()}) -join ' | ')

        $minX = ($_.Value.foreach{$_.Pt.X - $_.Radius} | measure -Maximum).Maximum
        $maxX = ($_.Value.foreach{$_.Pt.X + $_.Radius} | measure -Minimum).Minimum
        $minY = ($_.Value.foreach{$_.Pt.Y - $_.Radius} | measure -Maximum).Maximum
        $maxY = ($_.Value.foreach{$_.Pt.Y + $_.Radius} | measure -Minimum).Minimum
        $minZ = ($_.Value.foreach{$_.Pt.Z - $_.Radius} | measure -Maximum).Maximum
        $maxZ = ($_.Value.foreach{$_.Pt.Z + $_.Radius} | measure -Minimum).Minimum
        $min = [Point]::new($minX, $minY, $minZ)
        $max = [Point]::new($maxX, $maxY, $maxZ)
        Write-Host "[Point]::new$($min.Format()), [Point]::new$($max.Format()), $($min.Distance($max))" 
    }

    Write-Host "Part 2: "
    Write-Host ''
}

class Bot {
    [Point] $Pt
    [long] $Radius

    Bot([long] $X, [long] $Y, [long] $Z, [long] $Radius) {
        $this.Pt = [Point]::new($X, $y, $z)
        $this.Radius = $Radius
    }

    [string] Format() {
        return 'pos=<{0},{1},{2}>, r={3}' -f ($this.Pt.X, $this.Pt.Y, $this.Pt.Z, $this.Radius)
    }
    [string] ToString() {
        return $this.Format()
    }
}

function parseInput {

    $source = 
    'pos=<0,0,0>, r=4
    pos=<1,0,0>, r=1
    pos=<4,0,0>, r=3
    pos=<0,2,0>, r=1
    pos=<0,5,0>, r=3
    pos=<0,0,3>, r=1
    pos=<1,1,1>, r=1
    pos=<1,1,2>, r=1
    pos=<1,3,1>, r=1'

    $source = 
    'pos=<10,12,12>, r=2
    pos=<12,14,12>, r=2
    pos=<16,12,12>, r=4
    pos=<14,14,14>, r=6
    pos=<50,50,50>, r=200
    pos=<10,10,10>, r=5'

    #$source = Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))

    $source -split "`r?`n" | foreach {
        if ($_ -notmatch 'pos=\<(-?\d+),(-?\d+),(-?\d+)\>, r=(\d+)') { throw $_ }
        [Bot]::new([long]$Matches[1], [long]$Matches[2], [long]$Matches[3], [long]$Matches[4])
    }
}
parseInput | sort Pt
return

main