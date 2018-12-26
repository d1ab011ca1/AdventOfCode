# https://adventofcode.com/2018/day/18
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 5637
    part2 #= 965 (wrong answer!!!)
}

class Row : System.Collections.ArrayList {
    Row() {}
    Row([int] $width) : base($width) {
        foreach ($x in 1..$width) {
            $this.Add(-1L)
        }
    }
}

class Grid : System.Collections.ArrayList {
    Grid() {}
    Grid([int] $width, [int] $height) : base($height) {
        foreach ($y in 1..$height) {
            $this.Add([Row]::new($width))
        }
    }
}

class Puzzle {
    [int] $Depth
    [int] $TargetX
    [int] $TargetY
    [Grid] $Grid  # Contains erosion levels of each area

    Puzzle([int]$depth, [int]$targetX, [int]$targetY) {

        $this.Depth = $depth
        $this.TargetX = $targetX
        $this.TargetY = $targetY
        $this.Grid = [Grid]::new(1000, 1000)

        # Pre-calculate the erosion level of boundary areas
        # A region's erosion level is its geologic index plus the cave system's depth, all modulo 20183.

        # The region at 0,0 (the mouth of the cave) has a geologic index of 0.
        # The region at the coordinates of the target has a geologic index of 0.
        # If the region's Y coordinate is 0, the geologic index is its X coordinate times 16807.
        # If the region's X coordinate is 0, the geologic index is its Y coordinate times 48271.
        # Otherwise, the region's geologic index is the result of multiplying the erosion levels of the regions at X-1,Y and X,Y-1.

        # Assign hardcoded values in reverse priority
        foreach ($y in 1..($this.Grid.Count - 1)) {
            $gi = $y * 48271
            $this.Grid[$y][0] = ($gi + $this.Depth) % 20183
        }
        foreach ($x in 0..($this.Grid[0].Count - 1)) {
            $gi = $x * 16807
            $this.Grid[0][$x] = ($gi + $this.Depth) % 20183
        }
        $this.Grid[$this.TargetX][$this.TargetY] = (0 + $this.Depth) % 20183
        $this.Grid[0][0] = (0 + $this.Depth) % 20183

        # All other will be calculated as needed. See getErosionLevel()
    }

    # A region's erosion level is its geologic index plus the cave system's depth, all modulo 20183. Then:
    #  - If the erosion level modulo 3 is 0, the region's type is rocky.
    #  - If the erosion level modulo 3 is 1, the region's type is wet.
    #  - If the erosion level modulo 3 is 2, the region's type is narrow.
    [long] getErosionLevel([int]$x, [int]$y) {
        # avoid unnecessary calls to PS methods since they are slow and this is recursive!!!
        $el = $this.Grid[$y][$x]
        if ($el -eq -1) {
            $el = $this.calcErosionLevel($x, $y)
        }
        return $el
    }
    [long] calcErosionLevel([int]$x, [int]$y) {
        # A region's erosion level is its geologic index plus the cave system's depth, all modulo 20183.
        # The region's geologic index is the result of multiplying the erosion levels of the regions at X-1,Y and X,Y-1.

        # Avoid unnecessary calls to PS methods since they are slow and this is recursive!!!
        $elA = $this.Grid[$y][$x-1]
        if ($elA -eq -1) {
            $elA = $this.calcErosionLevel($x-1, $y)
        }

        $elB = $this.Grid[$y-1][$x]
        if ($elB -eq -1) {
            $elB = $this.calcErosionLevel($x, $y-1)
        }

        $gi = $elA * $elB
        return $this.Grid[$y][$x] = ($gi + $this.Depth) % 20183
    }

    [void] Print([int]$maxX, [int]$maxY) {
        # Origin: M
        # Target: T
        # Rocky:  .
        # Wet:    =
        # Narrow: |
        $sb = [System.Text.StringBuilder]::new($maxX + 1)
        foreach ($y in 0..$maxY) {
            Write-Host ('{0,2} ' -f $y) -NoNewline -ForegroundColor DarkGray
            $null = $sb.Clear()
            foreach ($x in 0..$maxX) {
                if ($x -eq 0 -AND $y -eq 0) {
                    $null = $sb.Append('M')
                }
                elseif ($x -eq $this.TargetX -AND $y -eq $this.TargetY) {
                    $null = $sb.Append('T')
                }
                else {
                    $null = $sb.Append('.=|'[$this.getErosionLevel($x, $y) % 3])
                }
            }

            Write-Host $sb.ToString()
        }
        Write-Host
    }
    [void] PrintTool([string]$tool, [int]$maxX, [int]$maxY) {
        # Origin: M
        # Target: T
        # Rocky:  .
        # Wet:    =
        # Narrow: |
        if ($tool -eq 'torch') {
            # Torch can be used in rocky or narrow areas
            $allowedAreas = '.|'
            $alternativeTool = 'C-N'
        } elseif ($tool -eq 'climbing gear') {
            # Climbing gear can be used in rocky or wet areas
            $allowedAreas = '.='
            $alternativeTool = 'TN-'
        } elseif ($tool -eq 'neither') {
            # Neither can be used in wet or narrow areas
            $allowedAreas = '=|'
            $alternativeTool = '-CT'
        } else {
            throw 'Unknown tool.'
        }

        Write-Host $tool -ForegroundColor Blue
        $sb = [System.Text.StringBuilder]::new($maxX + 1)
        foreach ($y in 0..$maxY) {
            Write-Host ('{0,2} ' -f $y) -NoNewline -ForegroundColor DarkGray
            $null = $sb.Clear()
            foreach ($x in 0..$maxX) {
                $areaId = $this.getErosionLevel($x, $y) % 3
                $area = '.=|'[$areaId]
                if (!$allowedAreas.Contains($area)) {
                    $null = $sb.Append(' ')
                }
                elseif ($x -eq 0 -AND $y -eq 0) {
                    $null = $sb.Append('M')
                }
                elseif ($x -eq $this.TargetX -AND $y -eq $this.TargetY) {
                    $null = $sb.Append('T')
                }
                else {
                    $null = $sb.Append($alternativeTool[$areaId])
                }
            }

            Write-Host $sb.ToString()
        }
        Write-Host
    }

    [int] ComputeRisk([int]$maxX, [int]$maxY) {
        # Rocky:  0
        # Wet:    1
        # Narrow: 2
        $risk = 0
        foreach ($y in 0..$maxY) {
            foreach ($x in 0..$maxX) {
                if ($x -eq 0 -AND $y -eq 0) {
                    # Origin
                }
                elseif ($x -eq $this.TargetX -AND $y -eq $this.TargetY) {
                    # Target
                }
                else {
                    $risk += $this.getErosionLevel($x, $y) % 3
                }
            }

        }
        return $risk
    }
}

function part1 {
    $depth, $targetX, $targetY = input
    $puzzle = [Puzzle]::new($depth, $targetX, $targetY)

    $risk = $puzzle.ComputeRisk($puzzle.TargetX, $puzzle.TargetY)

    $puzzle.Print($puzzle.TargetX + 1, $puzzle.TargetY + 1)
    Write-Host "Part 1: Risk is $risk."
}    

Add-Type -TypeDefinition '
public class Node : System.IComparable {
    public int X;
    public int Y;
    public char Tool; // in (T, C, N)
    public int Duration = int.MaxValue;

    public int CompareTo(object obj) {
        // Assume other is a non-null Node
        var other = (Node)obj;
        int diff = this.X.CompareTo(other.X);
        if (diff == 0) {
            diff = this.Y.CompareTo(other.Y);
            if (diff == 0) {
                diff = this.Tool.CompareTo(other.Tool);

                // ignore Duration
            }
        }
        return diff;
    }
    public override bool Equals(object other) {
        return this.CompareTo(other) == 0;
    }
    public override int GetHashCode() {
        return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Tool.GetHashCode();
    }
}
public class NodeDurationComparer : System.Collections.Generic.Comparer<Node> {
    public override int Compare(Node objA, Node objB) {
        // Assume both valid
        int diff = ((Node)objA).Duration.CompareTo(((Node)objB).Duration);
        if (diff == 0) {
            diff = ((Node)objA).CompareTo((Node)objB);
        }
        return diff;
    }
}
public class PriorityQueue
{
    private System.Collections.Generic.SortedSet<Node> queue = new System.Collections.Generic.SortedSet<Node>(new NodeDurationComparer());

    public int Count { get { return queue.Count; } }
   
	public void Push(Node n) {
		queue.Add(n);
	}
	public Node Pop() {
		var front = queue.Min;
		queue.Remove(front);
		return front;
	}
	public void Remove(Node n) {
		queue.Remove(n);
	}
}
public class NodeHashSet
{
    private System.Collections.Generic.HashSet<Node> set = new System.Collections.Generic.HashSet<Node>();

	public int Count { get { return set.Count; } }

	public bool Add(Node n) {
		return set.Add(n);
	}   

	public bool Contains(Node n) {
		return set.Contains(n);
	}   
	public Node Get(Node n) {
		Node o;
		if (!set.TryGetValue(n, out o))
			throw new System.Collections.Generic.KeyNotFoundException();
		return o;
	}   
	public Node TryGet(Node n) {
		Node o;
		if (set.TryGetValue(n, out o)) 
			return o;
		return null;
	}   
}
'

function part2 {
    $depth, $targetX, $targetY = input
    $puzzle = [Puzzle]::new($depth, $targetX, $targetY)

    # We're assuming the path never veers too far from a direct path
    [int]$maxX = $puzzle.TargetX + 100
    [int]$maxY = $puzzle.TargetY + 100
    $null = $puzzle.ComputeRisk($maxX, $maxY)

    [Node]$initialNode = @{x=0; y=0; tool='T'; duration=0}
    [Node]$targetC = @{x=$puzzle.TargetX; y=$puzzle.TargetY; tool='C'}
    [Node]$targetT = @{x=$puzzle.TargetX; y=$puzzle.TargetY; tool='T'}

    [NodeHashSet]$nodes = [NodeHashSet]::new()
    [NodeHashSet]$visited = [NodeHashSet]::new()
    [PriorityQueue]$queue = [PriorityQueue]::new()

    $null = $nodes.Add($initialNode)
    $queue.Push($initialNode)

    while ($queue.Count) {
        # Select the node with the minimum duration (queue is sorted by duration)
        [Node]$cur = $queue.Pop()
        $null = $visited.Add($cur)

        # break once we find the Torch solution. The Camping Gear solution takes an additional 7 seconds
        # thus it may not be the shortest.
        if ($cur -eq $targetT -or $cur -eq $targetC) {
            break
        }

        $neighbors = @(@(($cur.X-1), $cur.Y), @(($cur.X+1), $cur.Y), @($cur.X, ($cur.Y+1)), @($cur.X, ($cur.Y-1)))
        foreach($neighbor in $neighbors) {
            [int]$x = $neighbor[0]
            [int]$y = $neighbor[1]
            if ($x -lt 0 -or $x -gt $maxX) {continue}
            if ($y -lt 0 -or $y -gt $maxY) {continue}

            # Rocky area (0) can use Torch or Climbing gear
            # Wet area (1) can use Climbing gear or Neither
            # Narrow area (2) can use Torch or Neither
            [string]$allowedTools = @('TC', 'CN', 'TN')[$puzzle.getErosionLevel($x, $y) % 3]
            foreach ($tool in $allowedTools.GetEnumerator()) {

                [Node]$next = @{x=$x; y=$y; tool=$tool} 
                if ($visited.Contains($next)) {
                    continue
                }

                # 1 second to move
                $next.Duration = $cur.Duration + 1

                # 7 seconds to switch tools
                if ($next.Tool -ne $cur.Tool) {
                    $next.Duration += 7
                }

                # The Camping Gear solution always takes an additional
                # 7 seconds to switch back to Torch
                if ($next -eq $targetC) {
                    $next.Duration += 7
                }
                
                if (!$nodes.Add($next)) {
                    # already exists
                    [Node]$existing = $nodes.Get($next)
                    if ($next.Duration -lt $existing.Duration) {
                        # queue is sorted by duration, so remove before modifying. we'll reinsert it later.
                        $null = $queue.Remove($existing)
                        $existing.Duration = $next.Duration
                    }
                    $next = $existing
                }

                $queue.Push($next)
            }
        }
    }

    $minDuration = [int]::MaxValue
    $targetT = $visited.TryGet($targetT)
    if ($targetT -and $targetT.Duration -lt $minDuration) {
        $minDuration = $targetT.Duration
    }

    $targetC = $visited.TryGet($targetC)
    if ($targetC -and $targetC.Duration -lt $minDuration) {
        $minDuration = $targetC.Duration
    }
    
    Write-Host "Part 2: Fewest minutes to reach target is $minDuration."
}

function input {
    #      depth, targetX targetY
    #return   510,   10,      10
    return 11394,    7,     701
}
#input
#return

main