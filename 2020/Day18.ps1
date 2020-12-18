$data = @"
2 * 3 + (4 * 5)
5 + (8 * 3 + 9 + 3 * 4 * 3)
5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))
((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2
"@ -split '\r?\n'
$data = Get-Content "$PSScriptRoot/Day18.txt"

function part1 {
    function eval {
        $x = $tokens.Dequeue()
        if ($x -eq '(') {
            $x = eval
            $null = $tokens.Dequeue() # right paren
        }
        else {
            $x = [int]$x
        }
    
        while ($tokens.Count -AND $tokens.Peek() -ne ')') {
            $op = $tokens.Dequeue()

            $y = $tokens.Dequeue()
            if ($y -eq '(') {
                $y = eval
                $null = $tokens.Dequeue() # right paren
            }
            else {
                $y = [int]$y
            }
    
            if ($op -eq '+') { 
                $x = $x + $y
            }
            elseif ($op -eq '*') { 
                $x = $x * $y
            }
        }
    
        $x
    }

    $p1 = 0
    $tokens = [System.Collections.Queue]::new()
    foreach ($expr in $data) {
        $expr.foreach{ $_ -split ' |\b|\B' }.where{ $_ }.foreach{ $tokens.Enqueue($_) }
        $p1 += eval
        if ($tokens.Count) { throw 'Not all tokens consumed' }
    }
    Write-Host Part1 = $p1 # 
}

function part2 {
    function eval {
        $x = $tokens.Dequeue()
        if ($x -eq '(') {
            $x = eval
            $null = $tokens.Dequeue() # right paren
        }
        else {
            $x = [long]$x
        }
    
        while ($tokens.Count -AND $tokens.Peek() -ne ')') {
            $op = $tokens.Dequeue()
            if ($op -eq '+') { 
                $y = $tokens.Dequeue()
                if ($y -eq '(') {
                    $y = eval
                    $null = $tokens.Dequeue() # right paren
                }
                else {
                    $y = [long]$y
                }

                $x = $x + $y
            }
            else { 
                $y = eval
                $x = $x * $y
            }
        }
    
        $x
    }

    $p2 = 0L
    $tokens = [System.Collections.Queue]::new()
    foreach ($expr in $data) {
        $expr.foreach{ $_ -split ' |\b|\B' }.where{ $_ }.foreach{ $tokens.Enqueue($_) }
        $x = eval
        if ($tokens.Count) { throw 'Not all tokens consumed' }
        #Write-Host $expr = $x
        $p2 += $x
    }
    Write-Host Part2 = $p2 # 
}

part1
part2
