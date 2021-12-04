open System
open System.IO

let realInputText = 
    let inputPath = Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")
    File.ReadAllText(inputPath)

let sampleInputText = """
7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

22 13 17 11  0
 8  2 23  4 24
21  9 14 16  7
 6 10  3 18  5
 1 12 20 15 19

 3 15  0  2 22
 9 18 13 17  5
19  8  7 25 23
20 11 10 24  4
14 21 16 12  6

14 21 17 24  4
10 16 15  9 19
18  8 23 26 20
22 11 13  6  5
 2  0 12  3  7
"""

type Row = int[]
type Board = Row[]
type Scoreboard = Collections.Generic.List<int>

let nums,boards: (int[] * Board[]) =
    let inputText = realInputText
    //let inputText = sampleInputText

    let lines =
        inputText.Split('\n', StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)

    let nums =
        lines 
        |> Array.head
        |> fun x -> x.Split(',') |> Array.map int

    let boards =
        lines 
        |> Array.tail
        |> Array.map(fun x -> x.Split(' ', StringSplitOptions.RemoveEmptyEntries) |> Array.map int)
        |> Array.chunkBySize 5

    let boards =
        boards
        |> Array.map(fun b ->
            b 
            |> Array.transpose
            |> Array.append b)

    nums,
    boards
//printfn "%A" (nums,boards)

let createScoreboard (boards: Board[]) : Scoreboard[] =
    boards
    |> Array.map(fun b -> Scoreboard(Seq.init b.Length (fun _ -> 0)))

let markScore (scoreboard: Scoreboard) ri ci =
    let score = scoreboard.[ri] ||| (1 <<< ci)
    scoreboard.[ri] <- score
    score = 0b11111

let calcScore winningNumber (b: Board) (s: Scoreboard) =
    let mutable sum = 0
    for ri in 0..4 do
        for ci in 0..4 do
            if s.[ri] &&& (1 <<< ci) = 0 then
                sum <- sum + b.[ri].[ci]
    sum * winningNumber

let part1 () =
    let scoreboard = boards |> createScoreboard

    let play n =
        let mutable gameOver = false
        [for bi = 0 to boards.Length-1 do
            let b = boards.[bi]
            for ri = 0 to b.Length-1 do
                for ci in 0..4 do
                    if not gameOver && b.[ri].[ci] = n then
                        if markScore scoreboard.[bi] ri ci then
                            gameOver <- true
                            yield bi,n]

    let bingo ns =
        let mutable gameOver = false
        [for n in ns do 
            if not gameOver then
                match play n with
                | winner :: _ ->
                    gameOver <- true
                    yield winner
                | _ -> ()]
        |> List.head

    let winningB,winningN = bingo nums
    let score = calcScore winningN boards.[winningB] scoreboard.[winningB]

    printfn "Part 1: Score=%d (board=%d, num=%d)" score winningB winningN

let part2 () =
    let scoreboard = boards |> createScoreboard
    let boardsWon = Collections.Generic.Dictionary<int,int>()

    let play n =
        [for bi = 0 to boards.Length-1 do
            if not <| boardsWon.ContainsKey bi then
                let b = boards.[bi]
                for ri = 0 to b.Length-1 do
                    for ci in 0..4 do
                        if b.[ri].[ci] = n then
                            if markScore scoreboard.[bi] ri ci then
                                boardsWon.[bi] <- n
                                if boardsWon.Count = boards.Length then
                                    yield bi,n]

    let bingo ns =
        let mutable gameOver = false
        [for n in ns do
            if not gameOver then
                match play n with
                | looser :: _ ->
                    gameOver <- true
                    yield looser
                | _ -> ()]
        |> List.head

    let loosingB,loosingN = bingo nums
    let score = calcScore loosingN boards.[loosingB] scoreboard.[loosingB]

    printfn "Part 2: Score=%d (board=%d, num=%d)" score loosingB loosingN

part1 () //2496
part2 () //25925
