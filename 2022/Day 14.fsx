// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers
open System.Text

type Coord = Point2D
type Grid = StringBuilder[]

let printGrid (grid: Grid) = grid |> Array.iter (printfn "%O")

let parseInput (text: string) : Coord[][] =
    text
    |> parseInputText
    |> Array.map (fun line ->
        line.Split(" -> ")
        |> Array.map (fun pair ->
            let p = pair.Split(',')
            Coord.ofTuple (int p[0], int p[1])))

let source = Coord.ofTuple (500, 0)
let down = (0, 1)
let downleft = (-1, 1)
let downright = (1, 1)

let getRect (inputs: Coord[] seq) =
    let minX, maxX, maxY =
        inputs
        |> Seq.collect id
        |> Seq.fold
            (fun (minX, maxX, maxY) p -> Math.Min(minX, p.x), Math.Max(maxX, p.x), Math.Max(maxY, p.y))
            (Int32.MaxValue, 0, 0)

    Rect.fromCoords (minX - 1, 0) (maxX + 2, maxY + 1)

let createGrid (inputs: Coord[] seq) =
    let rect = getRect inputs
    let grid = Array.init rect.height (fun _ -> StringBuilder().Append('.', rect.width))

    inputs
    |> Seq.iter (
        Array.pairwise
        >> Array.iter (fun (p1, p2) ->
            if p1.x = p2.x then
                let n, m = Math.Min(p1.y, p2.y), Math.Max(p1.y, p2.y)

                for y = n to m do
                    grid[y][p1.x - rect.left] <- '#'
            else
                let n, m = Math.Min(p1.x, p2.x), Math.Max(p1.x, p2.x)

                for x = n to m do
                    grid[p1.y][x - rect.left] <- '#'

            grid[source.y][source.x - rect.left] <- '+')
    )

    grid, rect

let drop (grid: Grid, offset: Coord) (pos: Coord) =
    let rec droprec (pos: Coord) =
        let maxY = offset.y + grid.Length - 1
        let get (pt: Coord) = grid[pt.y - offset.y][pt.x - offset.x]

        let set (pt: Coord) v =
            grid[pt.y - offset.y][pt.x - offset.x] <- v

        if pos.y >= maxY then
            ValueNone // done
        else
            let next = pos |> Coord.offset down

            match get next with
            | '.' -> droprec next // continue
            | _ ->
                let next = pos |> Coord.offset downleft

                match get next with
                | '.' -> droprec next // continue
                | _ ->
                    let next = pos |> Coord.offset downright

                    match get next with
                    | '.' -> droprec next // continue
                    | _ ->
                        // dead-end
                        set pos 'o'
                        ValueSome pos

    droprec pos

let part1 (inputs: Coord[][]) =
    // printfn "%A" inputs

    let grid, rect = inputs |> createGrid

    let rec drip () =
        match drop (grid, rect.p1) source with
        | ValueNone -> () // done
        | _ -> drip ()

    drip ()
    grid |> printGrid

    grid
    |> Seq.map (fun row -> row.ToString() |> Seq.where ((=) 'o') |> Seq.length)
    |> Seq.fold (+) 0

let part2 (inputs: Coord[][]) =
    let rect = getRect inputs
    let ht = rect.height + 1

    let floor =
        [| Coord.ofTuple (500 - rect.height - 2, ht); Coord.ofTuple (500 + rect.height + 2, ht) |]

    let grid, rect = Seq.append inputs [floor] |> createGrid
    // grid |> printGrid

    let rec drip () =
        match drop (grid, rect.p1) source with
        | ValueNone -> failwith "Finished unexpectedly"
        | ValueSome p when p.y = 0 -> () // done
        | _ -> drip ()

    drip ()
    grid |> printGrid

    grid
    |> Seq.map (fun row -> row.ToString() |> Seq.where ((=) 'o') |> Seq.length)
    |> Seq.fold (+) 0


let sampleInputText1 =
    """
498,4 -> 498,6 -> 496,6
503,4 -> 502,4 -> 502,9 -> 494,9
"""

try
    [ sampleInputText1
      getInput ()
      ]
    |> Seq.iteri (fun inputNo inputText ->
        let input = inputText |> parseInput
        printfn "Input %d Part 1: %O" inputNo (part1 input) // 672
        printfn "Input %d Part 2: %O" inputNo (part2 input) // 26831
        printfn "")
with
| :? OperationCanceledException -> printfn "*** Cancelled ***"
| _ -> reraise ()
