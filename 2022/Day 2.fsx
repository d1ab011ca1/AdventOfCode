// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open FSharpHelpers

let sampleInputText1 =
    """
A Y
B X
C Z
"""

let cookie = IO.File.ReadAllText("cookie.txt")
let inputText = downloadInput cookie
//let inputText = sampleInputText1

let parseInput (text: string) =
    let lines = text |> parseInputText
    lines |> Array.map (fun s -> s[0], s[2])

let inputs = inputText |> parseInput
//printfn "%A" inputs

type RPS =
    | R
    | P
    | S

let (|RPS|) =
    function
    | 'A'
    | 'X' -> R
    | 'B'
    | 'Y' -> P
    | 'C'
    | 'Z' -> S
    | x -> failwithf "unexpected RPS value: %A" x

type Outcome =
    | W
    | L
    | D

let (|Outcome|) =
    function
    | 'X' -> L
    | 'Y' -> D
    | 'Z' -> W
    | x -> failwithf "unexpected outcome value: %A" x

let caclScore opp me =
    match me with
    | R -> 1
    | P -> 2
    | S -> 3
    + match opp, me with
      | R, S
      | S, P
      | P, R -> 0 // loose
      | R, P
      | P, S
      | S, R -> 6 // win
      | _ -> 3 // draw

let part1 () =
    let score = inputs |> Array.fold (fun s (RPS opp, RPS me) -> s + caclScore opp me) 0
    printfn $"Part 1: {score}"

let part2 () =
    let win =
        function
        | R -> P
        | S -> R
        | P -> S

    let lose =
        function
        | R -> S
        | S -> P
        | P -> R

    let score =
        inputs
        |> Array.fold
            (fun s (RPS opp, Outcome outcome) ->
                let me =
                    match outcome with
                    | W -> win opp
                    | L -> lose opp
                    | D -> opp

                s + caclScore opp me)
            0

    printfn $"Part 2: {score}"

part1 () // 15572
part2 () // 16098
