// https://adventofcode.com/2023/day/8
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers
open System.Text.RegularExpressions

type InputData = Input of Instructions: string * Network: Map<string, string * string>

let parseInput (text: string) : InputData =
    let lines = text |> String.splitAndTrim "\n"

    let instructions = lines[0]

    let network =
        lines
        |> Seq.skip 1
        |> Seq.fold
            (fun map s ->
                let m = Regex.Match(s, """^(\w+) = \((\w+), (\w+)\)""").Groups
                map |> Map.add m[1].Value (m[2].Value, m[3].Value))
            Map.empty

    Input(instructions, network) //|> tee (printfn "%A")

let sample1 =
    parseInput
        """
LLR

AAA = (BBB, BBB)
BBB = (AAA, ZZZ)
ZZZ = (ZZZ, ZZZ)
"""

let sample2 =
    parseInput
        """
LR

11A = (11B, XXX)
11B = (XXX, 11Z)
11Z = (11B, XXX)
22A = (22B, XXX)
22B = (22C, 22C)
22C = (22Z, 22Z)
22Z = (22B, 22B)
XXX = (XXX, XXX)
"""

let part1 (Input(instructions, network)) =
    let mutable steps = 0
    let mutable next = "AAA"

    while next <> "ZZZ" do
        next <-
            match instructions[steps % instructions.Length] with
            | 'L' -> fst network[next]
            | 'R' -> snd network[next]
            | _ -> failwith "unexpected"

        steps <- steps + 1

    steps

let part2 (Input(instructions, network)) =
    // For each starting point, count the number of steps to the
    // first ending. Then compute the LCM of all the minimum steps.
    // I'm not sure if this always works, but it does in this case.

    let inline endsWith c (n: string) = n[2] = c

    let rec findFirstEnd step cur =
        if cur |> endsWith 'Z' then
            int64 step
        else
            let next =
                match instructions[step % instructions.Length] with
                | 'L' -> fst network[cur]
                | 'R' -> snd network[cur]
                | _ -> failwith "unexpected"

            findFirstEnd (step + 1) next

    network.Keys
    |> Seq.where (endsWith 'A')
    |> Seq.map (findFirstEnd 0)
    |> Seq.toList
    // |> tee (printfn "%A")
    |> Int64.lcmList

let data = getInput () |> parseInput

part1 sample1 |> testEqual "Part 1 sample" 6
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 16271

part2 sample2 |> testEqual "Part 2 sample" 6L
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 14265111103729L
