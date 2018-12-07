# https://adventofcode.com/2018/day/7
Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= MNOUBYITKXZFHQRJDASGCPEVWL
    part2 #= 893
}

function part1 {
    $post = input
    #$post

    $pre = @{}
    foreach ($values in $post.Values) {
       foreach ($v in $values) {
            ++$pre[$v]
        }
    }
    #$pre

    $a = [System.Collections.SortedList]::new()
    # Add nodes with no predecessors to $a...
    foreach ($key in $post.Keys) {
        if (!$pre.ContainsKey($key)) {
            $a[$key] = $post[$key]
        }
    }
    #$a

    $result = while ($a.Count) {
        # pop off the first node
        $k = $a.GetKey(0)
        $next = $a[$k]
        $a.Remove($k)
        $k

        # decrement the count the following node(s). If any are zero, add them to $a
        foreach ($key in $next) {
            if ((--$pre[$key]) -eq 0) {
                $a[$key] = $post[$key]
            }
        }
    }
    $result -join ''
}

function part2 {
    $numWorkers = 5 #2 #5
    $overhead = 60 #0 #60
    $nodeCount = 26 #6 #26

    $post = input
    $pre = @{}
    foreach ($values in $post.Values) {
       foreach ($v in $values) {
            ++$pre[$v]
        }
    }
    #$pre

    $a = [System.Collections.SortedList]::new()
    # Add nodes with no predecessors to $a...
    foreach ($key in $post.Keys) {
        if (!$pre.ContainsKey($key)) {
            $a[$key] = $post[$key]
        }
    }
    #$a

    class Worker {
        [int]$id
        [nullable[int]]$busyUntil
        [string]$work
        [string[]]$next
        Worker($id) {
            $this.id = $id
        }
        [bool] Available() {
            return $null -eq $this.busyUntil
        }
        [bool] Done([int]$tick) {
            return $null -ne $this.busyUntil -AND $this.busyUntil -le $tick
        }
    }
    $workers = @()
    foreach ($i in 1..$numWorkers) { $workers += [Worker]::new($i) }
    $result = ''
    $tick = -1
    while ($result.Length -lt $nodeCount) {
        ++$tick

        # Find completed work
        foreach ($worker in $workers.Where{$_.Done($tick)}) {
            $result += $worker.work
            
            # decrement the count of the next node(s). If any are zero, add them to $a
            foreach ($key in $worker.next) {
                if ((--$pre[$key]) -eq 0) {
                    $a[$key] = $post[$key]
                }
            }

            $worker.busyUntil = $null
            $worker.work = '.'
            $worker.next = @()
        }

        # Queue new work...
        while ($a.Count) {
            $worker = $workers | where{$_.Available()} | select -First 1
            if (!$worker) { break }

            # pop off the first node
            $k = $a.GetKey(0)
            $worker.work = $k
            $worker.next = $a[$k]
            $worker.busyUntil = $tick + ([int][char]$k - [int][char]'A' + 1) + $overhead
            
            $a.Remove($k)
        }

        Write-Host ('{0,4}' -f $tick) -NoNewline
        foreach ($worker in $workers) {
            Write-Host ('{0,5}' -f $worker.work) -NoNewline
        }
        Write-Host ('   {0}' -f $result)
    }
    $result
    $tick
}

function input {
    $source = `
'Step C must be finished before step A can begin.
Step C must be finished before step F can begin.
Step A must be finished before step B can begin.
Step A must be finished before step D can begin.
Step B must be finished before step E can begin.
Step D must be finished before step E can begin.
Step F must be finished before step E can begin.'

    $source = Get-Content (Join-Path $PSScriptRoot 'Day7.txt')

    $in = @{}
    $source -split "`n" | %{
        if (!($_ -match 'Step (.+) must be finished before step (.+) can begin')){throw $_}
        $in[$Matches[1]] += @($Matches[2])
    }
    $in
}
#input
#return

main