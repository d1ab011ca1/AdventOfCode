$data = @'
mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
trh fvjkl sbzzf mxmxvkd (contains dairy)
sqjhc fvjkl (contains soy)
sqjhc mxmxvkd sbzzf (contains fish)
'@ -split '\r?\n'
$data = Get-Content "$PSScriptRoot/Day21.txt"

$foods = @{} # food id -> food
$alergens = @{} # alergen -> list of food
$ingredients = @{} # ingredient -> list of food
$data = $data.foreach{ 
    if ($_ -notmatch '^(.+) \(contains (.+)\)$') { throw "no match: $_" }
    $food = [pscustomobject]@{
        Id          = $foods.Count
        Ingredients = @($Matches[1].Trim() -split ' ')
        Alergens    = @($Matches[2] -split ', ')
    }
    $foods[$food.id] = $food
    $food.Alergens.foreach{ $alergens[$_] += @($food) }
    $food.Ingredients.foreach{ $ingredients[$_] += @($food) }
}

$candidates = $alergens.Values | % {
    $_.Ingredients | group | ? count -ge $_.Count | select -exp Name
} | group -AsHashTable
$safe = $ingredients.Keys.where{ !$candidates.ContainsKey($_) } | group -AsHashTable

function part1 {
    $p1 = $foods.Values.Ingredients.where{ $safe.ContainsKey($_) }.Count
    Write-Host Part1 = $safe.Keys : $p1 # 2307
}

function part2 {
    foreach ($s in $safe.Keys) {
        foreach ($food in $ingredients[$s]) {
            $food.Ingredients = @($food.Ingredients.where{ $_ -ne $s })
        }
    }
    #$foods.Values | out-string | write-host

    $unsafe = @{}    
    while ($alergens.Count -gt 0) {
        foreach ($a in $alergens.Keys) {
            $fs = $alergens[$a]
            $c = @($fs.Ingredients | group | where Count -eq $fs.Count)
            if ($c.Count -eq 1) {
                $i = $c[0].Name
                $unsafe[$a] = $i
                $alergens.Remove($a)
                foreach ($food in $ingredients[$i]) {
                    $food.Ingredients = @($food.Ingredients.where{ $_ -ne $i })
                }
                break;
            }
        }
    }

    $p2 = $unsafe.Keys | sort | % { $unsafe[$_] }
    Write-Host Part2 = ($p2 -join ',') # cljf,frtfg,vvfjj,qmrps,hvnkk,qnvx,cpxmpc,qsjszn
}

part1
part2
