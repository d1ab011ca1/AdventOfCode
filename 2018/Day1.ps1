# https://adventofcode.com/
Set-StrictMode -Version Latest
Clear-Host

function puzzle1 {
    function input{
        ('2459,2269,190,163,158,150,146') |
        %{ ,@($_ -split ',' | %{[int]$_} | sort -Descending) }
    }

    input | %{
        $i = $_
    }
}
#puzzle1
