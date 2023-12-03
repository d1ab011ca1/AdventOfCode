// https://adventofcode.com/2023/day/X
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

let parseInput (text: string) =
    text
    |> String.splitO "\n" (StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)
// |> tee (printfn "%A")

let part1 (input) = 0

let part2 (input) = 0

let sample1 =
    parseInput
        """
"""

let sample2 = sample1

let data = getInput () |> parseInput

part1 sample1 |> testEqual "Part 1 sample" 0
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 0

part2 sample2 |> testEqual "Part 2 sample" 0
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 0
