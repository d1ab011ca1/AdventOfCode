// https://adventofcode.com/2023/day/15
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type InputData = Input of string[]

let parseInput (text: string) : InputData =
    text |> String.trim |> String.split "," |> Input //|> tee (printfn "%A")

let computeHashSpan (s: ReadOnlySpan<char>) =
    let mutable hash = 0

    for i = 0 to s.Length - 1 do
        hash <- (((hash + (int s[i])) * 17) % 256)

    hash

let computeHash (s: string) = computeHashSpan (s.AsSpan())

assert (computeHash "HASH" = 52)

let sample1 =
    parseInput
        """
rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
"""

let sample2 = sample1

let part1 (Input data) =
    data |> Array.sumBy (fun s -> computeHash s)

type Instruction =
    | Insert of lens: string * boxIdx: int * focalLen: int
    | Remove of lens: string * boxIdx: int

let part2 (Input data) =

    let instructions =
        data
        |> Array.map (fun s ->
            match s[s.Length - 1] with
            | '-' ->
                let lens = s |> String.left (s.Length - 1)
                Remove(lens, computeHash lens)
            | DecChar focalLen ->
                assert (s[s.Length - 2] = '=')
                let lens = s |> String.left (s.Length - 2)
                Insert(lens, computeHash lens, focalLen))

    let boxes = Array.create 256 List.empty<string * int>

    instructions
    |> Array.iter (function
        | Insert(lens, boxIdx, focalLen) ->
            boxes[boxIdx] <-
                match boxes[boxIdx] |> List.tryFindIndex (fst >> (=) lens) with
                | Some lensIdx ->
                    // replace
                    // boxes[boxIdx] |> List.removeAt lensIdx |> List.insertAt lensIdx (lens, focalLen)
                    boxes[boxIdx]
                    |> List.mapi (fun i v -> if i = lensIdx then (lens, focalLen) else v)
                | None ->
                    // append
                    boxes[boxIdx] @ [ (lens, focalLen) ]

        | Remove(lens, boxIdx) ->
            // remove existing lens (if any)...
            boxes[boxIdx] <-
                match boxes[boxIdx] |> List.tryFindIndex (fst >> (=) lens) with
                | Some idx -> boxes[boxIdx] |> List.removeAt idx
                | _ -> boxes[boxIdx] // do nothing
    )

    boxes
    |> Seq.mapi (fun boxIdx lens ->
        lens
        |> Seq.mapi (fun lensIdx (_, focalLen) -> (boxIdx + 1) * (lensIdx + 1) * focalLen)
        |> Seq.sum)
    |> Seq.sum

let data = getInput () |> parseInput

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 1320
executePuzzle "Part 1 finale" (fun () -> part1 data) 511416

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 145
executePuzzle "Part 2 finale" (fun () -> part2 data) 290779
