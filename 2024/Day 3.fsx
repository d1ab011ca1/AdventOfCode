// https://adventofcode.com/2024/day/3
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = string

let parseInput (text: string) : InputData = text //|> tee (printfn "%A")

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"

let sample2 =
    "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))"

let data = getInput () |> parseData

let part1 (data: InputData) =
    //
    Regex.Matches(data, @"mul\(([0-9]{1,3}),([0-9]{1,3})\)")
    |> Seq.fold
        (fun s m ->
            s
            + ((m.Groups[1].Value |> Int32.fromString 10)
               * (m.Groups[2].Value |> Int32.fromString 10)))
        0

let part2 (data: InputData) =
    //
    Regex.Matches(data, @"(?<dont>don't\(\))|(?<do>do\(\))|mul\((?<a>[0-9]{1,3}),(?<b>[0-9]{1,3})\)")
    |> Seq.fold
        (fun (s, enabled) m ->
            if m.Groups["do"].Success then
                (s, true)
            elif m.Groups["dont"].Success then
                (s, false)
            elif not enabled then
                (s, false)
            else
                let s =
                    s
                    + ((m.Groups["a"].Value |> Int32.fromString 10)
                       * (m.Groups["b"].Value |> Int32.fromString 10))

                (s, true))
        (0, true)
    |> fst

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 161
executePuzzle "Part 1 finale" (fun () -> part1 data) 171183089

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 48
executePuzzle "Part 2 finale" (fun () -> part2 data) 63866497
