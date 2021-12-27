open System
open System.IO

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
v...>>.vv>
.vv>>.vv..
>>.>v>...v
>>v>>.>.v.
v>v.vv.v..
>.>>..v...
.vv..>.>v.
v.v..>>v.v
....v..v.>
"""

let inputs =
    let inputText = realInputText
    // let inputText = sampleInputText

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines |> Array.map Seq.toArray
//printfn "%A" inputs

type Map = char[][]

let printMap (map: Map) =
    for r in map do
        printfn "%s" (r |> String.Concat)

let tryMove (map: Map) =
    let height = map.Length
    let width = map.[0].Length
    let mutable moved = false

    // try move east '>'
    let res = map |> Array.map Array.copy
    for y = 0 to height - 1 do
        let row = map.[y]
        let resRow = res.[y]
        for x = 0 to width - 1 do
            let nextX = (x + 1) % width
            if row.[x] = '>' && row.[nextX] = '.' then
                resRow.[x] <- '.'
                resRow.[nextX] <- '>'
                moved <- true

    // move south herd 'v'
    let map, res = res, res |> Array.map Array.copy
    for y = 0 to height - 1 do
        let nextY = (y + 1) % height
        let row = map.[y]
        let nextRow = map.[nextY]
        let resRow = res.[y]
        let resNextRow = res.[nextY]
        for x = 0 to width - 1 do
            if row.[x] = 'v' && nextRow.[x] = '.' then
                resRow.[x] <- '.'
                resNextRow.[x] <- 'v'
                moved <- true

    if moved then Some res else None

let part1 () =
    let mutable map = inputs
    let mutable step = 0
    let mutable moved = true

    // printMap map

    while moved do
        step <- step + 1

        match tryMove map with
        | Some res -> map <- res
        | None -> moved <- false

        // printfn $"Step {step}:"
        // printMap map

    printfn $"Part 1: {step}"

part1 () // 329
