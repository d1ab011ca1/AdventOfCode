// https://adventofcode.com/2024/day/X
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

#nowarn "57" // Experimental library feature (Array.Parallel), requires '--langversion:preview'.

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = Grid<char>[]

let parseInput ((TextLines lines) as text) : InputData =
    // find the indexes of the blank lines
    let blanks =
        lines |> Seq.indexed |> Seq.where (snd >> String.isEmpty) |> Seq.map fst

    // split the lines into grids
    Seq.append blanks [ lines.Length ]
    |> Seq.fold
        (fun (grids, startIdx) blankIdx ->
            ((lines[startIdx .. (blankIdx - 1)] |> Grid.fromLines) :: grids, blankIdx + 1))
        ([], 0)
    |> fst
    |> List.rev
    |> List.toArray
// |> dump

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
#####
.####
.####
.####
.#.#.
.#...
.....

#####
##.##
.#.##
...##
...#.
...#.
.....

.....
#....
#....
#...#
#.#.#
#.###
#####

.....
.....
#.#..
###..
###.#
###.#
#####

.....
.....
.....
#....
#.#..
#.#.#
#####
"""

let data =
    let rawData = getInput ()
    lazy (rawData |> parseData)

let part1 (data: InputData) =
    let (locks, keys) = data |> Array.partition (Grid.item 0 0 >> (=) '#') //|> dump

    let toHeights g =
        [| for x = 0 to 4 do
               (g |> Grid.col x |> Seq.where ((=) '#') |> Seq.length) - 1 |]

    let locks = locks |> Array.map toHeights
    let keys = keys |> Array.map toHeights

    seq {
        for k in keys do
            for l in locks do
                if not (Seq.zip k l |> Seq.exists (fun (k, l) -> k + l > 5)) then
                    yield 1
    }
    |> Seq.sum

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 3
executePuzzle "Part 1 finale" (fun () -> part1 data.Value) 3090
