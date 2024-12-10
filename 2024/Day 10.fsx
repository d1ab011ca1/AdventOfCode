// https://adventofcode.com/2024/day/10
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = Grid<byte>

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Grid.fromLines
    |> Grid.map (fun _ c -> (byte) (c - '0'))
// |> tee Grid.printfn

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732
"""

let sample2 = sample1

let data = lazy (getInput () |> parseData)

let deltas = [| (0, -1); (0, +1); (-1, 0); (+1, 0) |]

let part1 (data: InputData) =
    let pathsTo9 pos =
        let rec loop c coords =
            if c = 9uy then
                coords // done
            else
                // find next moves (c+1)...
                let nextMoves =
                    coords
                    |> Seq.collect (fun (x, y) ->
                        deltas
                        |> Seq.choose (fun (dx, dy) ->
                            let (nx, ny) = (x + dx, y + dy)

                            match data |> Grid.tryItemV nx ny with
                            | ValueSome n when n = (c + 1uy) -> Some(nx, ny)
                            | _ -> None))
                    |> Seq.distinct

                // search next moves
                loop (c + 1uy) nextMoves

        loop 0uy [ pos ] |> Seq.length

    data
    |> Grid.fold
        (fun sum pos c ->
            match c with
            | 0uy -> sum + (pathsTo9 pos)
            | _ -> sum)
        0

let part2 (data: InputData) =
    let pathsTo9 pos =
        let mutable branches = 1

        let rec loop c coords =
            if c = 9uy then
                // force enumeration of sequences...
                coords |> Seq.length |> ignore
                branches // done
            else
                // find next moves (c+1)...
                let nextMoves =
                    coords
                    |> Seq.collect (fun (x, y) ->
                        // assume this branch it is a dead end
                        branches <- branches - 1

                        deltas
                        |> Seq.choose (fun (dx, dy) ->
                            let (nx, ny) = (x + dx, y + dy)

                            match data |> Grid.tryItemV nx ny with
                            | ValueSome n when n = (c + 1uy) ->
                                // not a dead end
                                branches <- branches + 1
                                Some(nx, ny)
                            | _ -> None))

                // search next moves
                loop (c + 1uy) nextMoves

        loop 0uy [ pos ]

    data
    |> Grid.fold
        (fun sum pos c ->
            match c with
            | 0uy -> sum + (pathsTo9 pos)
            | _ -> sum)
        0

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 36
executePuzzle "Part 1 finale" (fun () -> part1 data.Value) 538

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 81
executePuzzle "Part 2 finale" (fun () -> part2 data.Value) 1110
