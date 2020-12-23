$data = @'
Player 1:
9
2
6
3
1

Player 2:
5
8
4
7
10
'@ -split '\r?\n'
$data = Get-Content "$PSScriptRoot/Day22.txt"

$p1 = @()
$p2 = $null
$data.foreach{
    if ($_ -eq 'Player 2:') { 
        $p2 = @()
    }
    elseif ([char]::IsDigit($_[0])) {
        if ($null -ne $p2) { $p2 += [int]$_ }
        else { $p1 += [int]$_ }
    }
}
Write-Host Player 1 : $p1
Write-Host Player 2 : $p2

function part1 {
    $p1 = [System.Collections.Queue]::new($p1)
    $p2 = [System.Collections.Queue]::new($p2)

    $round = 0
    while ($p1.Count -AND $p1.Count) {
        ++$round
        Write-Host '-- Round $round --'
        #Write-Host "Player 1's deck: $p1"
        #Write-Host "Player 2's deck: $p2"
        $c1 = $p1.Dequeue()
        $c2 = $p2.Dequeue()
        #Write-Host "Player 1 plays: $c1"
        #Write-Host "Player 2 plays: $c2"
        if ($c1 -gt $c2) {
            Write-Host 'Player 1 wins the round!'
            $p1.Enqueue($c1)
            $p1.Enqueue($c2)
        }
        elseif ($c2 -gt $c1) {
            Write-Host 'Player 2 wins the round!'
            $p2.Enqueue($c2)
            $p2.Enqueue($c1)
        }
        Write-Host
    }

    Write-Host '== Post-game results =='
    Write-Host "Player 1's deck: $p1"
    Write-Host "Player 2's deck: $p2"
    if ($p1.Count) { $w = $p1 } else { $w = $p2 }
    $score = 0
    for ($i = $w.Count; $i -gt 0; --$i) {
        $score += $i * $w.Dequeue()
    }
    
    Write-Host Part 1 = $score # 35202
}

function part2 {
    Add-Type -ea Stop -TypeDefinition @"
    namespace AOC
    {
        using System;
        using System.Linq;
        using System.Collections;
        using System.Collections.Generic;
        public static class Day22a {
            static System.Security.Cryptography.HMACSHA256 hash = new System.Security.Cryptography.HMACSHA256(new byte[64]);
            public static string Hash(IEnumerable p1, IEnumerable p2)
            {
                var b1 = p1.Cast<int>().SelectMany(v => BitConverter.GetBytes(v));
                var b2 = p2.Cast<int>().SelectMany(v => BitConverter.GetBytes(v));
            
                hash.Initialize();
                var bytes = hash.ComputeHash(b1.Concat(new byte[1]).Concat(b2).ToArray());
            
                return Convert.ToBase64String(bytes);
            }
        }
    }
"@

    $gameData = @{ 
        GamesPlayed = 0
    }
    function Combat([int[]]$player1, [int[]]$player2) {
        ++$gameData.GamesPlayed
        $gameNo = $gameData.GamesPlayed
    
        #Write-Host
        #Write-Host "=== Game $gameNo ==="
    
        $p1 = [System.Collections.Queue]::new($player1)
        $p2 = [System.Collections.Queue]::new($player2)
        $round = 0
        $history = @{}
        while ($p1.Count -AND $p2.Count) {
            ++$round
            #Write-Host
            #Write-Host "-- Round $round (Game $gameNo) --"
            #Write-Host "Player 1's deck: $p1"
            #Write-Host "Player 2's deck: $p2"
    
            # Before either player deals a card, if there was a previous round in 
            # this game that had exactly the same cards in the same order in the 
            # same players' decks, the game instantly ends in a win for player 1. 
            # Previous rounds from other games are not considered. (This prevents 
            # infinite games of Recursive Combat, which everyone agrees is a bad idea.)
            #$roundId = "[$($p1 -join ',')], [$($p2 -join ',')]"
            $roundId = [AOC.Day22a]::Hash($p1, $p2)
            if ($history.ContainsKey($roundId)) {
                #Write-Host "Recursive Combat!"
                $winner = 1
                break
            }
            $history.Add($roundId, $null)
    
            $c1 = $p1.Dequeue()
            $c2 = $p2.Dequeue()
            #Write-Host "Player 1 plays: $c1"
            #Write-Host "Player 2 plays: $c2"
    
            # If both players have at least as many cards remaining in their deck as the 
            # value of the card they just drew, the winner of the round is determined by 
            # playing a new game of Recursive Combat.
            # Otherwise, at least one player must not have enough cards left in their deck 
            # to recurse; the winner of the round is the player with the higher-value card.
            if ($p1.Count -ge $c1 -and $p2.Count -ge $c2) {
                #Write-Host 'Playing a sub-game to determine the winner...'
                $winner = Combat ($p1 | select -first $c1) ($p2 | select -first $c2)
                #Write-Host "...anyway, back to game $gameNo."
            }
            elseif ($c1 -gt $c2) {
                $winner = 1
            }
            else {
                $winner = 2
            }
            #Write-Host "Player $winner wins round $round of game $gameNo!"
    
            if ($winner -eq 1) {
                $p1.Enqueue($c1)
                $p1.Enqueue($c2)
            }
            else {
                $p2.Enqueue($c2)
                $p2.Enqueue($c1)
            }
        }
    
        #Write-Host "The winner of game $gameNo is player $winner!"
        #Write-Host 
    
        if ($gameNo -eq 1) {
            Write-Host '== Post-game results =='
            Write-Host "The winner is player $winner!"
            Write-Host "Player 1's deck: $p1"
            Write-Host "Player 2's deck: $p2"
            Write-Host "Total games played: $($gameData.GamesPlayed)"
            $score = 0
            if ($winner -eq 1) { $w = $p1 } else { $w = $p2 }
            for ($i = $w.Count; $i -gt 0; --$i) {
                $score += $i * $w.Dequeue()
            }
            return $score
        }

        return $winner
    }
    
    $score = Combat $p1 $p2
    Write-Host Part 2 = $score # 32317
}

#part1
(Measure-Command { part2 }).TotalSeconds
