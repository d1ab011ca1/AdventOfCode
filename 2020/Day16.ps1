$data = @"
class: 0-1 or 4-19
row: 0-5 or 8-19
seat: 0-13 or 16-19

your ticket:
11,12,13

nearby tickets:
3,9,18
15,1,5
5,14,9
"@ -split '\r?\n'
$data = Get-Content "$PSScriptRoot/Day16.txt"

$rules = [ordered]@{}
$tickets = @()
foreach ($d in $data) {
    if ($d -match '^(.+): (\d+)-(\d+) or (\d+)-(\d+)$') {
        $rules[$Matches[1]] = [pscustomobject]@{
            Begin1 = [int]$Matches[2]; End1 = [int]$Matches[3]
            Begin2 = [int]$Matches[4]; End2 = [int]$Matches[5]
        }
    }
    elseif ($d -match '^\d+') { 
        $tickets += @(, ($d -split ',').foreach{ [int]$_ } )
    }
}

#$rules
#$tickets | Write-Host

function part1 {
    $invalid = $tickets.foreach{
        foreach ($v in $_) {
            $matched = $false
            foreach ($r in $rules.Values) {
                if (($v -ge $r.begin1 -AND $v -le $r.end1) -OR ($v -ge $r.begin2 -AND $v -le $r.end2)) {
                    $matched = $true
                    break
                }
            }
            if (!$matched) { $v }
        }
    }
    $sum = ($invalid | measure -sum).Sum
    Write-Host Part1 = $sum # 28884
}

function part2 {
    Add-Type -TypeDefinition @"
namespace AOC
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    public static class Day16a {
        public static void DoIt()
        {
        }
    }
}
"@
    $valid = @()
    $tickets.foreach{
        foreach ($v in $_) {
            $matched = $false
            foreach ($r in $rules.Values) {
                if (($v -ge $r.begin1 -AND $v -le $r.end1) -OR ($v -ge $r.begin2 -AND $v -le $r.end2)) {
                    $matched = $true
                    break
                }
            }
            if (!$matched) { return }
        }
        $valid += @(, $_)
    }
    #$valid | Write-Host

    $myTicket = $valid[0]
    $nearbyTickets = $valid[1..10000]
    $fieldMap = @{}
    $fields = @(0..($myTicket.count - 1))
    #[array]::Reverse($fields)

    :restart while ($fields.count -gt 0) {
        foreach ($field in $fields) {
            # find all rules which match this field
            $matchedRules = @()
            :nextRule foreach ($ruleName in $rules.Keys) {
                $r = $rules[$ruleName]
        
                foreach ($t in $nearbyTickets) {
                    $v = $t[$field]
                    if (($v -ge $r.begin1 -AND $v -le $r.end1) -OR ($v -ge $r.begin2 -AND $v -le $r.end2)) {
                        # rule matches this field
                    }
                    else {
                        # rule does not match this field
                        #Write-Host $field : $v doesnt match rule $ruleName $r.begin1 thru $r.end1 OR $r.begin2 thru $r.end2
                        #Write-Host ('{0}: {1}-{2} or {3}-{4}' -f $ruleName, $r.begin1, $r.end1, $r.begin2, $r.end2)
                        continue nextRule
                    }
                }
                # rule matched this field
                $matchedRules += $ruleName
                if ($matchedRules.count -gt 1) { break }
            }
            if ($matchedRules.count -eq 1) {
                #Write-Host Field $field is "'$matchedRules'"
                $fieldMap[$matchedRules[0]] = $field
                $fields = $fields.where{ $_ -ne $field }
                $rules.Remove($matchedRules[0])
                continue restart
            }
        }
    }

    #$fieldMap | Out-String | Write-Host
    #$fieldMap.Keys.foreach{ Write-Host $_ = $myTicket[$fieldMap[$_]] }
    $mul = 1
    $fieldMap.Keys.where{ $_ -like 'departure*' }.foreach{ $mul *= $myTicket[$fieldMap[$_]] }
    Write-Host Part2 = $mul # 1001849322119
}

part1
part2
