// https://adventofcode.com/2024/day/4
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = Grid<char>

let parseInput (text: string) : InputData =
    text |> String.splitAndTrim "\n" |> Grid.fromLines
// |> tee (printfn "%A")

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX
"""

let sample2 = sample1

let data = getInput () |> parseData

let part1 (data: InputData) =
    let searchMAS x y dx dy =
        // data |> Grid.item x y => 'X'
        if data |> Grid.tryItemV (x + dx) (y + dy) = ValueSome 'M' then
            if data |> Grid.tryItemV (x + dx + dx) (y + dy + dy) = ValueSome 'A' then
                if data |> Grid.tryItemV (x + dx + dx + dx) (y + dy + dy + dy) = ValueSome 'S' then
                    1
                else
                    0
            else
                0
        else
            0

    // For every X, search around it for an M A S...
    data
    |> Grid.fold
        (fun count (x, y) ->
            function
            | 'X' ->
                count
                + searchMAS x y 1 0 // right
                + searchMAS x y -1 0 // left
                + searchMAS x y 0 -1 // up
                + searchMAS x y 0 1 // down
                + searchMAS x y 1 1 // right-down
                + searchMAS x y 1 -1 // right-up
                + searchMAS x y -1 -1 // left-up
                + searchMAS x y -1 1 // left-down
            | _ -> count)
        0

let part2 (data: InputData) =
    // For every A, search for M M S S (or a rotated variant) in the corners...
    data
    |> Grid.fold
        (fun count (x, y) ->
            function
            | 'A' ->
                let tl = data |> Grid.itemOrDefault (x - 1) (y - 1) '.' // top-left
                let tr = data |> Grid.itemOrDefault (x + 1) (y - 1) '.' // top-right
                let br = data |> Grid.itemOrDefault (x + 1) (y + 1) '.' // bottom-right
                let bl = data |> Grid.itemOrDefault (x - 1) (y + 1) '.' // bottom-left

                match tl, tr, br, bl with
                | 'M', 'M', 'S', 'S'
                | 'S', 'M', 'M', 'S'
                | 'S', 'S', 'M', 'M'
                | 'M', 'S', 'S', 'M' -> count + 1
                | _ -> count
            | _ -> count)
        0

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 18
executePuzzle "Part 1 finale" (fun () -> part1 data) 2639

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 9
executePuzzle "Part 2 finale" (fun () -> part2 data) 2005
