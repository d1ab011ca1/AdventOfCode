# https://adventofcode.com/2018/day/24
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

class Point : System.IComparable {
    [long] $W
    [long] $X
    [long] $Y
    [long] $Z

    Point() {
    }
    Point([long]$w, [long]$x, [long]$y, [long]$z) {
        $this.W = $w
        $this.X = $x
        $this.Y = $y
        $this.Z = $z
    }
    Point([Point]$pt) {
        $this.W = $pt.w
        $this.X = $pt.X
        $this.Y = $pt.Y
        $this.Z = $pt.Z
    }

    [Point] Offset([Point]$pt) { $this.W += $pt.W; $this.X += $pt.X; $this.Y += $pt.Y; $this.Z += $pt.Z; return $this }
    [Point] Offset([long]$w, [long]$x, [long]$y, [long]$z) { $this.W += $W; $this.X += $X; $this.Y += $Y; $this.Z += $Z; return $this }

    [long] Distance() {
        return [math]::Abs($this.W) + [math]::Abs($this.X) + [math]::Abs($this.Y) + [math]::Abs($this.Z)
    }
    [long] Distance([Point]$b) {
        return [math]::Abs($this.W - $b.W) + [math]::Abs($this.X - $b.X) + [math]::Abs($this.Y - $b.Y) + [math]::Abs($this.Z - $b.Z)
    }

    [Point] Clone() { return [Point]::new($this) }

    static [Point] op_Addition([Point]$a, [Point]$b) { return [Point]::new($a.W + $b.W, $a.X + $b.X, $a.Y + $b.Y, $a.Z + $b.Z) }
    static [Point] op_Subtraction([Point]$a, [Point]$b) { return [Point]::new($a.W - $b.W, $a.X - $b.X, $a.Y - $b.Y, $a.Z - $b.Z) }

    [int] GetHashCode() {
        return $this.W.GetHashCode() -bxor $this.X.GetHashCode() -bxor $this.Y.GetHashCode() -bxor $this.Z.GetHashCode()
    }
    [bool] Equals([object]$obj) {
        if ($obj.GetType() -ne [Point]) { return $false }
        return $this.CompareTo($obj) -eq 0
    }
    [int] CompareTo([object]$object) {
        if ($null -eq $object) { return 1 }
        [Point]$other = $object
        if ($null -eq $other) { throw [System.ArgumentException]'Object is not a Point' }
        $diff = $this.W.CompareTo($other.W)
        if ($diff -eq 0) {
            $diff = $this.X.CompareTo($other.X)
            if ($diff -eq 0) {
                $diff = $this.Y.CompareTo($other.Y)
                if ($diff -eq 0) {
                    $diff = $this.Z.CompareTo($other.Z)
                }
            }
        }
        return $diff
    }

    [string] Format() {
        return '({0},{1},{2},{3})' -f $this.W, $this.X, $this.Y, $this.Z
    }
    #[string] ToString() {
    #    return $this.Format()
    #}
}

function main {
    #part1 #= 390
    part2 #= 
}

function part1() {
    $pts = parseInput

    $constellations = @{}

    foreach ($i in 0..($pts.Count - 1)) {
        $pt = $pts[$i]

        [System.Collections.ArrayList]$constellation = $null
        if ($constellations.ContainsKey($pt)) {
            # use the existing constellation
            $constellation = $constellations[$pt]
        } else {
            # start a new constellation
            $constellation = [System.Collections.ArrayList]@($pt)
            $constellations[$pt] = $constellation
        }

        if ($i + 1 -eq $pts.Count) {continue}
        foreach ($j in ($i + 1)..($pts.Count - 1)) {
            if ($j -eq $i) {continue}
            $test = $pts[$j]

            #$dist = $test.Distance($pt)
            $dist = [math]::Abs($test.W - $pt.W) + [math]::Abs($test.X - $pt.X) + [math]::Abs($test.Y - $pt.Y) + [math]::Abs($test.Z - $pt.Z)
            if ($dist -le 3) {
                if ($constellations.ContainsKey($test)) {
                    # merge the current constellation into the existing constellation
                    [System.Collections.ArrayList]$existing = $constellations[$test]
                    if (![object]::ReferenceEquals($constellation, $existing)) {
                        $existing.AddRange($constellation)
                        # switch to the existing constellation
                        $constellation.ForEach{$constellations[$_] = $existing}
                        $constellation = $existing
                        #break
                    }
                } else {
                    # add to current constellation
                    $null = $constellation.Add($test)
                    $constellations[$test] = $constellation
                }
            }
        }
    }

    $constellations = $constellations.Values | foreach{$h=@{}}{$h[$_]=1}{$h.Keys}
    #$constellations | foreach { Write-Host ($_).foreach{$_.Format()} }
    Write-Host "Part 1: $($constellations.Count) constellations."
    Write-Host ''
}    

function part2() {
    $pts = parseInput

    $constellations = @{}

    foreach ($i in 0..($pts.Count - 1)) {
        $pt = $pts[$i]

        [System.Collections.ArrayList]$constellation = $null
        if ($constellations.ContainsKey($pt)) {
            # use the existing constellation
            $constellation = $constellations[$pt]
        } else {
            # start a new constellation
            $constellation = [System.Collections.ArrayList]@($pt)
            $constellations[$pt] = $constellation
        }

        if ($i + 1 -eq $pts.Count) {continue}
        foreach ($j in ($i + 1)..($pts.Count - 1)) {
            if ($j -eq $i) {continue}
            $test = $pts[$j]

            #$dist = $test.Distance($pt)
            $dist = [math]::Abs($test.W - $pt.W) + [math]::Abs($test.X - $pt.X) + [math]::Abs($test.Y - $pt.Y) + [math]::Abs($test.Z - $pt.Z)
            if ($dist -le 3) {
                if ($constellations.ContainsKey($test)) {
                    # merge the current constellation into the existing constellation
                    [System.Collections.ArrayList]$existing = $constellations[$test]
                    if (![object]::ReferenceEquals($constellation, $existing)) {
                        $existing.AddRange($constellation)
                        # switch to the existing constellation
                        $constellation.ForEach{$constellations[$_] = $existing}
                        $constellation = $existing
                        #break
                    }
                } else {
                    # add to current constellation
                    $null = $constellation.Add($test)
                    $constellations[$test] = $constellation
                }
            }
        }
    }

    $constellations = $constellations.Values | foreach{$h=@{}}{$h[$_]=1}{$h.Keys}
    #$constellations | foreach { Write-Host ($_).foreach{$_.Format()} }
    Write-Host "Part 2: $($constellations.Count) constellations."
    Write-Host ''
}

function parseInput {

    $source = 
    '0,0,0,0
    3,0,0,0
    0,3,0,0
    0,0,3,0
    0,0,0,3
    0,0,0,6
    9,0,0,0
    12,0,0,0' # 2 constellations

    #$source = 
    #'-1,2,2,0
    #0,0,2,-2
    #0,0,0,-2
    #-1,2,0,0
    #-2,-2,-2,2
    #3,0,2,-1
    #-1,3,2,2
    #-1,0,-1,0
    #0,2,1,-2
    #3,0,0,0' # 4 constellations

    #$source = 
    #'1,-1,0,1
    #2,0,-1,0
    #3,2,-1,0
    #0,0,3,1
    #0,0,-1,-1
    #2,3,-2,0
    #-2,2,0,0
    #2,-2,0,-1
    #1,-1,0,-1
    #3,2,0,2' # 3 constellations

    #$source =
    #'1,-1,-1,-2
    #-2,-2,0,1
    #0,2,1,3
    #-2,3,-2,1
    #0,2,3,-2
    #-1,-1,1,-2
    #0,-2,-1,0
    #-2,2,3,-1
    #1,2,2,0
    #-1,-2,0,-2' # 8 constellations

    $source = Get-Content -Path ([system.io.path]::ChangeExtension((Join-Path $PSScriptRoot (Split-Path $PSCmdlet.MyInvocation.MyCommand -Leaf)), 'txt'))

    $source -split "`r?`n" | foreach {
        $n = [int[]]($_ -split ',')
        [Point]::new($n[0],$n[1],$n[2],$n[3])
    }
}
#parseInput
#return

main