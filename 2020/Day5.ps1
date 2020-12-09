$d = Get-Content "$PSScriptRoot/Day5.txt"

$seatIds = $d.foreach{

    $row = [convert]::ToInt32(($_.Substring(0, 7) -replace 'F', '0' -replace 'B', '1'), 2)
    $col = [convert]::ToInt32(($_.Substring(7, 3) -replace 'L', '0' -replace 'R', '1'), 2)
    $seatId = $row * 8 + $col

    $seatId
} | sort

Write-Host Part1 = $seatIds[-1]  # 850

$mySeatId = foreach ($i in (1..$seatIds.count)) { if ($seatIds[$i] + 1 -ne $seatIds[$i + 1]) { $seatIds[$i] + 1; break; } }
Write-Host Part2 = $mySeatId # 599
