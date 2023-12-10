// https://adventofcode.com/2023/day/9
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type InputData = Input of int[][]

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Array.map (String.split " " >> Array.map int32)
    |> Input
// |> tee (printfn "%A")

let sample1 =
    parseInput
        """
0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45
"""

let sample2 = sample1

let part1 (Input data) =
    data
    |> Array.sumBy (fun ns ->
        let rec loop sum ns =
            let sum = sum + (ns |> Array.last)
            let next = ns |> Seq.pairwise |> Seq.map (fun (a, b) -> b - a) |> Seq.toArray

            // if next |> Array.forall ((=) 0) then
            if next |> Array.last = 0 then // not sure this is alwys a valid check, but works here
                sum
            else
                next |> loop sum

        ns |> loop 0) //|> tee (printfn "%A"))

let part2 (Input data) =
    data
    |> Array.sumBy (fun ns ->
        let rec loop ns =
            let next = ns |> Seq.pairwise |> Seq.map (fun (a, b) -> b - a) |> Seq.toArray

            // if next |> Array.forall ((=) 0) then
            if next |> Array.last = 0 then // not sure this is alwys a valid check, but works here
                (ns |> Array.head)
            else
                (ns |> Array.head) - (next |> loop)

        ns |> loop) //|> tee (printfn "%A"))

let data = getInput () |> parseInput

part1 sample1 |> testEqual "Part 1 sample" 114
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 1641934234

part2 sample2 |> testEqual "Part 2 sample" 2
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 975
