open System
open System.IO

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526
"""

let sampleInputText2 =
    """
11111
19991
19191
19991
11111
"""

let inputs =
    let inputText = realInputText
    // let inputText = sampleInputText
    // let inputText = sampleInputText2

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines
    |> Array.map (Seq.map (fun c -> int c - int '0') >> Array.ofSeq)

//printfn "%A" inputs

let sizeY = inputs.Length
let sizeX = inputs.[0].Length
//printfn "Size: %A" (sizeX, sizeY)

let value x y (xs: int [] []) =
    if x < 0 || y < 0 || x >= sizeX || y >= sizeY then
        -1
    else
        xs.[y].[x]

type State = { cur: int [] []; prev: int [] [] }

let step state =
    let increaseEnergy x y nrg =
        let nrg = (nrg + 1) % 10
        state.cur.[y].[x] <- nrg
        if nrg = 0 then Some(x, y) else None

    let increaseEnergy2 x y =
        match value x y state.cur with
        | nrg when nrg <= 0 -> None
        | nrg -> increaseEnergy x y nrg

    let rec applyFlash (x, y) =
        let flashes =
            seq {
                increaseEnergy2 (x - 1) (y - 1)
                increaseEnergy2 (x - 1) (y + 0)
                increaseEnergy2 (x - 1) (y + 1)
                increaseEnergy2 (x + 0) (y - 1)
                increaseEnergy2 (x + 0) (y + 1)
                increaseEnergy2 (x + 1) (y - 1)
                increaseEnergy2 (x + 1) (y + 0)
                increaseEnergy2 (x + 1) (y + 1)
            }
            |> Seq.choose id
            |> Array.ofSeq

        if flashes.Length > 0 then
            flashes |> Array.iter applyFlash

    seq {
        for y = 0 to sizeY - 1 do
            for x = 0 to sizeX - 1 do
                increaseEnergy x y state.prev.[y].[x]
    }
    |> Seq.choose id
    |> Array.ofSeq
    |> Array.iter applyFlash

let part1 () =

    let mutable state =
        { cur = inputs |> Array.map Array.copy
          prev = inputs |> Array.map Array.copy }

    let numFlashes =
        seq {
            for _ = 1 to 100 do
                state <- { cur = state.prev; prev = state.cur }
                step state

                state.cur
                |> Seq.sumBy (Seq.where ((=) 0) >> Seq.length)
        }
        |> Seq.sum

    printfn "Part 1: %d" numFlashes

let part2 () =

    let mutable state =
        { cur = inputs |> Array.map Array.copy
          prev = inputs |> Array.map Array.copy }

    let mutable numSteps = 0

    while state.cur |> Seq.sumBy Seq.sum > 0 do
        numSteps <- numSteps + 1
        state <- { cur = state.prev; prev = state.cur }
        step state

    printfn "Part 2: %d" numSteps

part1 () //1741
part2 () //440
