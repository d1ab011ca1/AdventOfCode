// https://adventofcode.com/2023/day/14
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type InputData = Input of Grid<char>

[<Literal>]
let Dot = '.'

[<Literal>]
let Cube = '#'

[<Literal>]
let Round = 'O'

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Grid.fromLines
    // |> tee Grid.printfn
    |> Input

let sample1 =
    parseInput
        """
O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#....
"""

let sample2 = sample1

let tryMove dx dy x y grid =
    let rec loop x y =
        match grid |> Grid.tryItem (x + dx) (y + dy) with
        | Some Dot ->
            grid |> Grid.set x y Dot
            grid |> Grid.set (x + dx) (y + dy) Round
            loop (x + dx) (y + dy)
        | _ -> ()

    loop x y

let tryMoveNorth = tryMove 0 -1
let tryMoveSouth = tryMove 0 +1
let tryMoveWest = tryMove -1 0
let tryMoveEast = tryMove +1 0

let rotateNorth grid =
    let (w, h) = grid |> Grid.widthAndHeight

    for y = 0 to h - 1 do
        for x = 0 to w - 1 do
            match grid |> Grid.item x y with
            | Round -> grid |> tryMoveNorth x y
            | _ -> ()

    grid

let rotateSouth grid =
    let (w, h) = grid |> Grid.widthAndHeight

    for y = h - 1 downto 0 do
        for x = 0 to w - 1 do
            match grid |> Grid.item x y with
            | Round -> grid |> tryMoveSouth x y
            | _ -> ()

    grid

let rotateWest grid =
    let (w, h) = grid |> Grid.widthAndHeight

    for x = 0 to w - 1 do
        for y = 0 to h - 1 do
            match grid |> Grid.item x y with
            | Round -> grid |> tryMoveWest x y
            | _ -> ()

    grid

let rotateEast grid =
    let (w, h) = grid |> Grid.widthAndHeight

    for x = w - 1 downto 0 do
        for y = 0 to h - 1 do
            match grid |> Grid.item x y with
            | Round -> grid |> tryMoveEast x y
            | _ -> ()

    grid

let rollGrid = rotateNorth >> rotateWest >> rotateSouth >> rotateEast

let score grid =
    let mutable sum = 0
    let height = grid |> Grid.height

    grid
    |> Grid.iter (fun (_, y) v ->
        match v with
        | Round -> sum <- sum + (height - y)
        | _ -> ())

    sum

let hashGrid =
    let enc = Text.Encoding.Latin1
    let maxRowWidth = 100 // from looking at input
    let buf = Array.zeroCreate (enc.GetMaxByteCount(maxRowWidth))

    let hasher =
        Security.Cryptography.SHA1.Create() :> Security.Cryptography.HashAlgorithm

    fun (grid: Grid<char>) ->
        let (w, h) = grid |> Grid.widthAndHeight

        hasher.Initialize()

        for y = 0 to h - 1 do
            for x = 0 to w - 1 do
                buf[x] <- grid |> Grid.item x y |> byte

            hasher.TransformBlock(buf, 0, w, null, 0) |> ignore

        hasher.TransformFinalBlock(buf, 0, 0) |> ignore
        let hash = Convert.ToBase64String(hasher.Hash.AsSpan())

        hash

let part1 (Input grid) =
    grid
    |> rotateNorth
    // |> tee Grid.printfn
    |> score

let part2 (Input grid) =
    let cycles = 1_000_000_000

    // Find the periodicity of the grid...
    let getPeriod () =
        let cache = Collections.Generic.Dictionary()

        let rec loop n =
            let hash = grid |> rollGrid |> hashGrid

            if cache.TryAdd(hash, n) then
                loop (n + 1)
            else // done. found a repeat
                n, n - cache[hash]

        loop 1

    let (cycle, period) = getPeriod () //|> echos "cycle, period"

    // roll the last few...
    let remainingCycles =
        let rem = cycles - cycle
        rem - (rem / period * period) //|> echos "remainingCycles"

    for _ = 1 to remainingCycles do
        grid |> rollGrid |> ignore

    grid
    // |> tee Grid.printfn
    |> score

let data = getInput () |> parseInput

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 136
executePuzzle "Part 1 finale" (fun () -> part1 data) 106378

executePuzzle "Part 2 sample" (fun () -> part2 sample1) 64
executePuzzle "Part 2 finale" (fun () -> part2 data) 90795
