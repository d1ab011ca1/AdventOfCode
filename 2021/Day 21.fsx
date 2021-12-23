open System
open System.IO

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
Player 1 starting position: 4
Player 2 starting position: 8
"""

let inputs =
    let inputText = realInputText
    let inputText = sampleInputText

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines.[0].Split(": ").[1] |> int, lines.[1].Split(": ").[1] |> int
// printfn "%A" inputs

type Player = { pos: int; score: int }

let move player spaces =
    let pos = (player.pos + spaces) % 10

    { pos = pos
      score = player.score + pos + 1 }

let players =
    { pos = (inputs |> fst) - 1; score = 0 }, { pos = (inputs |> snd) - 1; score = 0 }

let part1 () =
    let rollDeterministicDie3Times rollNum = rollNum * 3 + 6

    let minScore = 1000
    let mutable p1, p2 = players
    let mutable rollNum = 0

    while p1.score < minScore && p2.score < minScore do
        p1 <- rollDeterministicDie3Times rollNum |> move p1
        rollNum <- rollNum + 3

        if p1.score < minScore then
            p2 <- rollDeterministicDie3Times rollNum |> move p2
            rollNum <- rollNum + 3

        // printfn $"R{rollNum/6}: {p1.pos+1,2},{p2.pos+1,-2} {p1.score,4},{p2.score,-4}"

    // printfn "Winner: %s in %d rolls" (if p1.score > p2.score then "P1" else "P2") rollNum

    printfn "Part 1: %d" (rollNum * Math.Min(p1.score, p2.score))

let part2 () =
    let rollProb =
        // Propability of rolling:
        Map [ 3, 1 // in 27
              4, 3 // in 27
              5, 6 // in 27
              6, 7 // in 27
              7, 6 // in 27
              8, 3 // in 27
              9, 1 ] // in 27

    let minScore = 21
    let die = Random()

    // while true do
    //     let mutable p1, p2 = players
    //     let mutable rollNum = 0

    //     let rollDiracDie3Times () =
    //         die.Next(1, 4)
    //         + die.Next(1, 4)
    //         + die.Next(1, 4)

    //     while p1.score < minScore && p2.score < minScore do
    //         p1 <- rollDiracDie3Times () |> move p1
    //         rollNum <- rollNum + 3

    //         if p1.score < minScore then
    //             p2 <- rollDiracDie3Times () |> move p2
    //             rollNum <- rollNum + 3

    //     printfn
    //         $"R{rollNum / 6}: {p1.pos + 1, 2},{p2.pos + 1, -2} {p1.score, 4},{p2.score, -4} P{if p1.score > p2.score then 1 else 2}"

    printfn "Part 2: "

part1 () // 897798
part2 () //
