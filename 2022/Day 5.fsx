// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open System.IO
open FSharpHelpers
open System.Text.RegularExpressions
open System.Collections.Generic
// open MathNet.Numerics
// open FSharp.Collections.ParallelSeq

let sampleInputText1 =
    """
    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2
"""

let inputText, depth, width = File.ReadAllText(getInputFilePath ()), 8, 9
// let inputText,depth,width = sampleInputText1,3,3

let parseInput (text: string) =
    let lines =
        text.Trim('\r', '\n') |> String.split "\n" |> Seq.map (fun s -> s.TrimEnd('\r'))

    let stacks = Array.init width (fun _ -> Stack())

    lines
    |> Seq.take depth
    |> Seq.rev
    |> Seq.iter (fun s ->
        for w in 0 .. width - 1 do
            match s[w * 4 + 1] with
            | ' ' -> ()
            | v -> stacks[ w ].Push(v))

    let dirs =
        lines
        |> Seq.skip (depth + 2)
        |> Seq.map (fun s ->
            let m = Regex.Match(s, $"move (\d+) from (\d+) to (\d+)")
            int m.Groups[1].Value, (int m.Groups[2].Value) - 1, (int m.Groups[3].Value) - 1)

    stacks, dirs

let part1 () =
    let stacks, dirs = inputText |> parseInput
    printfn "%A" (stacks, dirs)

    for (n, f, t) in dirs do
        for _ = 1 to n do
            stacks[ t ].Push(stacks[ f ].Pop())

    let top =
        stacks
        |> Seq.fold (fun a s -> s.Pop() :: a) List.Empty
        |> Seq.rev
        |> String.Concat

    printfn "Part 1: %A" top

let part2 () =
    let stacks, dirs = inputText |> parseInput
    printfn "%A" (stacks, dirs)

    for (n, f, t) in dirs do
        [ for _ = 1 to n do
              stacks[ f ].Pop() ]
        |> Seq.rev
        |> Seq.iter (stacks[t].Push)

    let top =
        stacks
        |> Seq.fold (fun a s -> s.Pop() :: a) List.Empty
        |> Seq.rev
        |> String.Concat

    printfn "Part 2: %A" top

part1 () // VQZNJMWTR
part2 () // NLCDCLVMQ
