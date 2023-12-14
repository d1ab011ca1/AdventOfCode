// https://adventofcode.com/2023/day/13
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers
open System.Text.RegularExpressions

type InputData = Input of Grid<char>[]

let parseInput (text: string) : InputData =
    text
    |> String.splitRE (Regex("\r?\n\r?\n"))
    |> Array.map (String.splitAndTrim "\n" >> Grid.fromLines)
    // |> tee (Array.iter (printfn "%A"))
    |> Input

let sample1 =
    parseInput
        """
#.##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.

#...##..#
#....#..#
..##..###
#####.##.
#####.##.
..##..###
#....#..#
"""

let sample2 = sample1

let rec reflection (size: Grid<_> -> int) (accessor: int -> Grid<_> -> char seq) requiredDiffCount (grid: Grid<char>) =
    let size = grid |> size

    //  012345
    //  ------
    //  abcde  size=5
    //      ^  offset = 4 (compares d,e)
    let rec loop center offset diffs =
        if center = size then
            None // no reflection found
        elif center - offset < 0 || center + offset > size then
            if diffs = requiredDiffCount then
                Some center // reflection found
            else
                loop (center + 1) 1 0 // check next center
        else
            let a = grid |> accessor (center - offset)
            let b = grid |> accessor (center + offset - 1)

            let localDiffs = Seq.zip a b |> Seq.sumBy (fun (a, b) -> if a = b then 0 else 1)

            let diffs = diffs + localDiffs

            if diffs <= requiredDiffCount then
                loop center (offset + 1) diffs // check next offset
            else
                loop (center + 1) 1 0 // check next center

    loop 1 1 0

let solve grids requiredDiffs =

    let vertical = reflection Grid.width Grid.col
    let horizontal = reflection Grid.height Grid.row

    grids
    |> Seq.sumBy (fun g ->
        // find vertical or horizontal reflection in g...
        match vertical requiredDiffs g with
        | Some colsLeft -> colsLeft
        | _ ->
            match horizontal requiredDiffs g with
            | Some rowsAbove -> rowsAbove * 100
            | _ -> failwith "No reflection found")


let part1 (Input grids) = solve grids 0

let part2 (Input grids) = solve grids 1

let data = getInput () |> parseInput

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 405
executePuzzle "Part 1 finale" (fun () -> part1 data) 36041

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 400
executePuzzle "Part 2 finale" (fun () -> part2 data) 35915
