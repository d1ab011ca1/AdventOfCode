// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open FSharpHelpers
open System.Text.RegularExpressions
open System.Collections.Generic


type Monkey =
    { Id: int
      StartingItems: int64[]
      Operation: string * Operand
      Test: int64
      IfTrue: int
      IfFalse: int }

and Operand =
    | Old
    | Value of int64

let parseInput (text: string) =
    let monkeyRE = Regex(@"^Monkey (?<value>\d+):$", RegexOptions.ExplicitCapture)

    let startingItemsRE =
        Regex(@"^Starting items: (?<value>\d+)(, (?<value>\d+))*$", RegexOptions.ExplicitCapture)

    let operationRE =
        Regex(@"^Operation: new = old (?<operator>[\*\+]) (old|(?<value>\d+))$", RegexOptions.ExplicitCapture)

    let testRE =
        Regex(@"^Test: divisible by (?<value>\d+)$", RegexOptions.ExplicitCapture)

    let ifTrueRE =
        Regex(@"^If true: throw to monkey (?<value>\d+)$", RegexOptions.ExplicitCapture)

    let ifFalseRE =
        Regex(@"^If false: throw to monkey (?<value>\d+)$", RegexOptions.ExplicitCapture)

    text
    |> parseInputText
    |> Array.chunkBySize 6
    |> Array.map (fun lines ->
        { Id = monkeyRE.Match(lines[0]).Groups["value"].Value |> int
          StartingItems =
            startingItemsRE.Match(lines[1]).Groups["value"].Captures
            |> Seq.map (fun c -> c.Value |> int64)
            |> Seq.toArray
          Operation =
            let m = operationRE.Match(lines[2])

            m.Groups["operator"].Value,
            match m.Groups["value"].Success with
            | true -> Value(m.Groups["value"].Value |> int64)
            | _ -> Old
          Test = testRE.Match(lines[3]).Groups["value"].Value |> int64
          IfTrue = ifTrueRE.Match(lines[4]).Groups["value"].Value |> int
          IfFalse = ifFalseRE.Match(lines[5]).Groups["value"].Value |> int })

module Queue =
    let tryDequeue (q: Queue<_>) =
        match q.TryDequeue() with
        | true, i -> Some i
        | _ -> None

type MonkeyStats =
    { MonkeyId: int
      Items: Queue<int64>
      mutable ItemsHandled: int }

    static member init monkeys =
        monkeys
        |> Array.map (fun monkey ->
            { MonkeyId = monkey.Id
              Items = Queue monkey.StartingItems
              ItemsHandled = 0 })

    static member print round stats =
        printfn $"== After round {round} =="

        stats
        |> Array.iteri (fun i stat -> printfn $"Monkey {i} inspected items {stat.ItemsHandled} times.")

    static member calcScore stats =
        stats
        |> Seq.map (fun s -> int64 s.ItemsHandled)
        |> Seq.sortDescending
        |> Seq.take 2
        |> Seq.fold (*) 1L

let play monkeys totalRounds worryLevelDivisor =
    let worryLevelDivisor = int64 worryLevelDivisor
    let stats = monkeys |> MonkeyStats.init

    // This is the trick to prevent the worry level from growing out of control...
    // divide the computed worry level by the greatest common factor of all tests
    let gcf = monkeys |> Seq.map (fun m -> m.Test) |> Seq.fold (*) 1L

    let rec nextItem monkey =
        match stats[monkey.Id].Items |> Queue.tryDequeue with
        | None -> () // done
        | Some worryLevel ->
            let worryLevel =
                match monkey.Operation with
                | "+", (Value v) -> worryLevel + v
                | "*", (Value v) -> worryLevel * v
                | "+", Old -> worryLevel + worryLevel
                | "*", Old -> worryLevel * worryLevel
                | _ -> failwith "Unexpected operation"

            let worryLevel = worryLevel % gcf / worryLevelDivisor

            let throwTo =
                match worryLevel % monkey.Test = 0L with
                | true -> monkey.IfTrue
                | _ -> monkey.IfFalse

            stats[ throwTo ].Items.Enqueue(worryLevel)
            stats[monkey.Id].ItemsHandled <- stats[monkey.Id].ItemsHandled + 1
            nextItem monkey

    for round = 1 to totalRounds do
        for monkey in monkeys do
            nextItem monkey
        // if round = 1000 then
        //     stats |> MonkeyStats.print round
        //     OperationCanceledException() |> raise
        
    stats

let part1 (monkeys: Monkey[]) =
    let totalRounds = 20
    let stats = play monkeys totalRounds 3
    stats |> MonkeyStats.print totalRounds
    stats |> MonkeyStats.calcScore

let part2 (monkeys: Monkey[]) =
    let totalRounds = 10000
    let stats = play monkeys totalRounds 1
    stats |> MonkeyStats.print totalRounds
    stats |> MonkeyStats.calcScore

let sampleInputText1 =
    """
Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1
"""

try
    [ sampleInputText1; getInput () ]
    |> Seq.iteri (fun inputNo inputText ->
        let input = inputText |> parseInput
        printfn "Input %d Part 1: %O" inputNo (part1 input) // 61005
        printfn "Input %d Part 2: %O" inputNo (part2 input) // 20567144694
        printfn "")
with
| :? OperationCanceledException -> printfn "*** Cancelled ***"
| _ -> reraise ()
