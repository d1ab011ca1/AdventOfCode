// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers
open System.Text
open System.Collections.Generic

type Grid = int[][]
type Coord = Point2D

let parseInput (text: string) =
    let mutable start = Coord.zero
    let mutable goal = Coord.zero

    let grid =
        text
        |> parseInputText
        |> Array.mapi (fun y ->
            Seq.mapi (fun x ->
                function
                | 'S' ->
                    start <- Coord.ofTuple (x, y)
                    0
                | 'E' ->
                    goal <- Coord.ofTuple (x, y)
                    26 // 'z' + 1
                | c -> int c - int 'a')
            >> Seq.toArray)

    grid, start, goal

let printGrid (grid: Grid) start goal =
    let start = start |> Coord.toTuple
    let goal = goal |> Coord.toTuple

    for y = 0 to grid.Length - 1 do
        for x = 0 to grid[0].Length - 1 do
            if (x, y) = start then 'S'
            elif (x, y) = goal then 'E'
            else grid[y][x] + int 'a' |> char
            |> printf "%c"

        printfn ""

let directions, directionSymbols =
    let up = (0, -1), '^'
    let down = (0, +1), 'v'
    let left = (-1, 0), '<'
    let right = (+1, 0), '>'
    [| up; down; left; right |] |> Array.unzip

let printPath (grid: Grid) (start: Coord) (goal: Coord) path =
    let screen =
        Array.init grid.Length (fun _ -> StringBuilder().Append('.', grid[0].Length))

    screen[goal.y][goal.x] <- 'E'
    let mutable pos = start

    for dir in path do
        screen[pos.y][pos.x] <- directionSymbols[dir]
        pos <- pos |> Coord.offset directions[dir]

    screen |> Array.iter (printfn "%O")

let inline value (grid: Grid) (pos: Coord) = grid[pos.y][pos.x]

let breadthFirst pos (grid: Grid) =
    let gridRect = Rect.fromCoords (0, 0) (grid[0].Length, grid.Length)
    let history = HashSet()

    let nextPositions (pos, curpath) =
        [ match history.Add(pos) with
          | false -> () // already tried; dont loop
          | _ ->
              let v = value grid pos

              for dir = 0 to 3 do
                  let next = pos |> Coord.offset directions[dir]

                  match gridRect |> Rect.contains next with
                  | false -> () // outside grid
                  | _ ->
                      match value grid next with
                      | n when n <= v + 1 -> yield (next, dir :: curpath)
                      | _ -> () ] // ignore - blocked

    seq {
        let mutable positions = nextPositions (pos, List.empty)

        while positions <> [] do
            yield! positions
            positions <- positions |> List.collect nextPositions
    }

let play grid start goal =
    grid
    |> breadthFirst start
    |> Seq.tryPick (fun (pos, path) ->
        match pos = goal with
        | true -> Some(path |> List.rev)
        | _ -> None)

let part1 (grid: Grid, start: Coord, goal: Coord) =
    // printfn "start=%O, goal=%O" start goal
    // printGrid grid start goal

    match play grid start goal with
    | Some path ->
        path |> printPath grid start goal
        path |> List.length
    | _ -> failwith "Path not found!"

let part2 (grid: Grid, _: Coord, goal: Coord) =
    let candidates =
        grid
        |> Seq.mapi (fun y ->
            Seq.mapi (fun x ->
                function
                | 0 -> Some((x, y) |> Coord.ofTuple)
                | _ -> None)
            >> Seq.choose id)
        |> Seq.collect id

    let len, (path, start) =
        candidates
        |> Seq.choose (fun start ->
            play grid start goal
            |> Option.map (fun path -> path |> List.length, (path, start)))
        |> Seq.minBy fst

    path |> printPath grid start goal
    len

let sampleInputText1 =
    """
Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi
"""

try
    [ sampleInputText1; getInput () ]
    |> Seq.iteri (fun inputNo inputText ->
        let input = inputText |> parseInput
        printfn "Input %d Part 1: %O" inputNo (part1 input) // 534
        printfn "Input %d Part 2: %O" inputNo (part2 input) // 525
        printfn "")
with
| :? OperationCanceledException -> printfn "*** Cancelled ***"
| _ -> reraise ()
