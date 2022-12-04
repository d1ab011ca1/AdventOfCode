// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open System.IO
open FSharpHelpers
// open MathNet.Numerics
// open FSharp.Collections.ParallelSeq

let sampleInputText1 =
    """
vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw
"""

let inputText = File.ReadAllText(getInputFilePath ())
// let inputText = sampleInputText1

let parseInput (text: string) = text |> parseInputText

let inputs = inputText |> parseInput
// printfn "%A" inputs

let score c =
    if c < 'a' then
        (int c - int 'A') + 26 + 1
    else
        (int c - int 'a') + 1


let part1 () =
    let s =
        inputs
        |> Array.fold
            (fun s line ->
                let len = line.Length / 2
                let a, b = line[.. len - 1], line[len..]
                let c = a[a.IndexOfAny(b.ToCharArray())]
                s + score c)
            0

    printfn "Part 1: %A" s

let part2 () =
    let foo m badge =
        Map.change
            badge
            (function
            | Some cnt -> Some(cnt + 1)
            | _ -> Some 1)
            m
    let s =
        inputs
        |> Array.chunkBySize 3
        |> Array.fold
            (fun sum sacks ->
                let badge =
                    sacks 
                    |> Seq.fold (fun m -> Seq.distinct >> Seq.fold foo m) Map.empty
                    |> Seq.find (fun p -> p.Value = 3)
                    |> fun p -> p.Key
                sum + score badge)
            0

    printfn "Part 2: %A" s

part1 () // 8072
part2 () // 2567
