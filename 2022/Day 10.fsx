// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open FSharpHelpers

type Instruction =
    | AddX of int
    | Noop

let parseInput (text: string) =
    text
    |> parseInputText
    |> Array.map (fun line ->
        let parts = line.Split(' ', 2)

        match parts[0] with
        | "addx" -> AddX(int parts[1])
        | "noop" -> Noop
        | _ -> failwithf "Unexpected line: %s" line)

let run tick instructions =
    instructions
    |> Array.fold
        (fun (cycle, x) i ->
            match i with
            | Noop ->
                tick (cycle + 1) x
                cycle + 1, x
            | AddX n ->
                tick (cycle + 1) x
                tick (cycle + 2) x
                cycle + 2, x + n)
        (0, 1)
    |> ignore

let part1 (inputs: Instruction[]) =
    //printfn "%A" inputs

    let mutable sigStrength = 0L

    let tick cycle x =
        match cycle with
        | 20
        | 60
        | 100
        | 140
        | 180
        | 220 -> sigStrength <- sigStrength + int64 (cycle * x)
        | _ -> ()

    inputs |> run tick

    sigStrength

let part2 (inputs: Instruction[]) =
    let crt = Array.init 6 (fun _ -> Text.StringBuilder().Append(' ', 40))

    let tick cycle x =
        let row = (cycle - 1) / 40
        let col = (cycle - 1) % 40
        let sprite = x

        match col - sprite with
        | -1
        | 0
        | 1 -> crt[row][col] <- '#'
        | _ -> ()

    inputs |> run tick
    "\n" + String.Join("\n", crt)

let sampleInputText1 =
    """
noop
addx 3
addx -5
"""

let sampleInputText2 =
    """
addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop
"""

[ sampleInputText1; sampleInputText2; getInput () ]
|> Seq.iteri (fun inputNo inputText ->
    let input = inputText |> parseInput
    printfn "Input %d Part 1: %O" inputNo (part1 input) // 10760
    printfn "Input %d Part 2: %O" inputNo (part2 input) // FPGPHFGH
    printfn "")
