// https://adventofcode.com/2024/day/2
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = int[][]

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Array.map (fun s -> s.Split(' ') |> Array.map Int32.Parse)
// |> tee (printfn "%A")

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9
"""

let sample2 = sample1

let data = getInput () |> parseData

let testInc (prev, n) =
    let d = n - prev
    1 <= d && d <= 3

let testDec (prev, n) =
    let d = prev - n
    1 <= d && d <= 3

let part1 (data: InputData) =
    data
    |> Seq.where (fun r ->
        if r[0] < r[1] then
            r |> Seq.pairwise |> Seq.forall testInc
        else
            r |> Seq.pairwise |> Seq.forall testDec)
    |> Seq.length

let part2 (data: InputData) =
    data
    |> Seq.where (fun r ->
        // Find the first valid report by skipping 0 or 1 level...
        // This solution feels "lazy". I tried others and kept coming
        // up 1 short. Still don't know what scenario I was missing.
        let isSafeReport skip =
            let rec loop fn i j =
                if j >= r.Length then true // reached end
                elif i = skip then loop fn (i + 1) j
                elif j = skip || i >= j then loop fn i (j + 1)
                elif fn (r[i], r[j]) then loop fn (i + 1) (j + 1)
                else false // not valid

            loop testInc 0 1 || loop testDec 0 1

        // test when skipping 0 levels...
        let mutable safe = isSafeReport -1
        let mutable idx = 0

        // test when skipping 1 level...
        while not safe && idx < r.Length do
            safe <- isSafeReport idx
            idx <- idx + 1

        safe)
    |> Seq.length

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 2
executePuzzle "Part 1 finale" (fun () -> part1 data) 299

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 4
executePuzzle "Part 2 finale" (fun () -> part2 data) 364
