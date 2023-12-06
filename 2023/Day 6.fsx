// https://adventofcode.com/2023/day/6
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type InputType = TimeAndDistance of time: string * distance: string

let parseInput (text: string) : InputType =
    let input =
        text
        |> String.splitAndTrim "\n"
        |> Array.map (String.splitAndTrim ":" >> Array.item 1)

    TimeAndDistance(input[0], input[1]) // |> tee (printfn "%A")

let sample1 =
    parseInput
        """
Time:      7  15   30
Distance:  9  40  200
"""

let sample2 = sample1

let calcWins (time, distance) =
    // First win algorithm takes advantage of the fact that the win-lose graph is symetric.
    // For example, time=7, distance=9:
    //   LLWWWWLL
    //   01234567 (8 seconds)
    // First win is at n=2 (after 2 losses), so total wins is 8 - 2 - 2.
    let rec firstWin n =
        if n * (time - n) > distance then
            (time + 1L) - n - n
        else
            firstWin (n + 1L)

    firstWin 1L

let part1 (TimeAndDistance(times, distances)) =
    let times = times |> String.splitAndTrim " " |> Array.map Int64.Parse
    let distances = distances |> String.splitAndTrim " " |> Array.map Int64.Parse
    Array.zip times distances |> Seq.map calcWins |> Seq.fold (*) 1L

let part2 (TimeAndDistance(time, distance)) =
    let time = time |> String.replace " " "" |> Int64.Parse
    let distance = distance |> String.replace " " "" |> Int64.Parse
    calcWins (time, distance)

let data = getInput () |> parseInput

part1 sample1 |> testEqual "Part 1 sample" 288L
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 393120L

part2 sample2 |> testEqual "Part 2 sample" 71503L
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 36872656L
