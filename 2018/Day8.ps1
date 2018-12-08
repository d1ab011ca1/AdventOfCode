# https://adventofcode.com/2018/day/8
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
Clear-Host

function main {
    #part1 #= 40701
    part2 #= 21399
}

class Node {
    [int]$id
    [Node[]]$children = @()
    [int[]]$metadata = @()
    [int] hidden $_value = -1

    [int]Value() {
        if ($this._value -eq -1) {
            $this._value = 0
            if ($this.children) {
                foreach($m in $this.metadata) {
                    if ($m -ge 1 -and $m -le $this.children.Count) {
                        $this._value += $this.children[$m-1].Value()
                    }
                }
            }else{
                foreach($m in $this.metadata) {
                    $this._value += $m
                }
            }
        }
        return $this._value
    }
}

function part1 {
    $nodes = input
    $sum = 0
    while ($nodes) {
        $nodes = foreach ($n in $nodes) {
            foreach($m in $n.metadata) { $sum += $m }
            $n.children
        }
    }

    $sum
}

function part2 {
    $root = input
    $root.Value()
}

function input {
    $source = `
'2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2'

    $source = Get-Content ([system.io.path]::ChangeExtension($PSCmdlet.MyInvocation.InvocationName, 'txt'))

    $info = @{
        in = $source -split " " | %{ [int]$_}
        i = 0
        nextid = 1
    }

    function parse {
        $node = [Node]::new()
        $node.id = $info.nextid++

        $c = $info.in[$info.i++]
        $m = $info.in[$info.i++]
        $node.children += for ($n = 0; $n -lt $c; $n++) {
            parse
        }
        $node.metadata += for ($n = 0; $n -lt $m; $n++) {
            $info.in[$info.i++]
        }
    
        return $node
    }
    parse
}
#input
#return

main
