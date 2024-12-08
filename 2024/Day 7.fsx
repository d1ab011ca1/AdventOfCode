// https://adventofcode.com/2024/day/X
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = int64 list[]

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Array.map (fun s ->
        s.Split([| ':'; ' ' |], StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)
        |> Array.map (Int64.fromString 10)
        |> List.ofArray)
// |> echo

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20
"""

let sample2 = sample1

let data = lazy (getInput () |> parseData)

type Op =
    | Mul
    | Add
    | Concat

let f (operators: int -> seq<array<Op>>) (data: InputData) =
    data
    |> Seq.where (function
        | result :: (head :: tail) ->
            operators tail.Length
            |> Seq.exists (fun ops ->
                let res =
                    tail
                    |> List.fold
                        (fun (res, bit) v ->
                            match ops[bit] with
                            | Add -> res + v
                            | Mul -> res * v
                            | Concat -> $"{res}{v}" |> Int64.fromString 10
                            , bit + 1)
                        (head, 0)
                    |> fst

                res = result)
        | _ -> failwith "Unexpected")
    |> Seq.sumBy List.head

let part1 (data: InputData) =
    let operators count =
        let ops = Array.init count (fun _ -> Mul)

        let rec permuteOps idx =
            seq {
                ops[idx] <- Mul
                if idx = 0 then yield ops else yield! permuteOps (idx - 1)
                ops[idx] <- Add
                if idx = 0 then yield ops else yield! permuteOps (idx - 1)
            }

        permuteOps (count - 1)

    f operators data

let part2 (data: InputData) =
    let operators count =
        let ops = Array.init count (fun _ -> Mul)

        let rec permuteOps idx =
            seq {
                ops[idx] <- Mul
                if idx = 0 then yield ops else yield! permuteOps (idx - 1)
                ops[idx] <- Add
                if idx = 0 then yield ops else yield! permuteOps (idx - 1)
                ops[idx] <- Concat
                if idx = 0 then yield ops else yield! permuteOps (idx - 1)
            }

        permuteOps (count - 1)

    f operators data

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 3749L
executePuzzle "Part 1 finale" (fun () -> part1 data.Value) 945512582195L

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 11387L
executePuzzle "Part 2 finale" (fun () -> part2 data.Value) 271691107779347L
