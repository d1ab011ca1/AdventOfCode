// https://adventofcode.com/2023/day/11
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type InputData = Input of Grid<char>

[<Literal>]
let Dot = '.'

[<Literal>]
let Hash = '#'

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Grid.fromLines
    // |> tee Grid.printfn
    |> Input

let sample1 =
    parseInput
        """
...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#.....
"""

let sample2 = sample1

let calcSumOfMinDistance galaxies =
    galaxies
    |> Seq.allPairs false
    |> Seq.fold
        (fun m pair ->
            let dist = pair ||> manhattanDistance |> int64

            m
            |> Map.change pair (function
                | None -> Some dist
                | Some d -> Some(min dist d)))
        Map.empty
    // |> echo
    |> Map.values
    |> Seq.sum

let part1 (Input grid) =
    // Grid is small enough that we can expand it in memory...
    let rec expand row (grid: Grid<_>) =
        if row >= grid.Length then
            grid
        elif grid[row] |> Array.forall ((=) Dot) then
            let copy = grid[row] |> Array.copy
            grid |> Array.insertAt (row + 1) copy |> expand (row + 2)
        else
            grid |> expand (row + 1)

    let grid = grid |> expand 0 |> Grid.rotate 90 |> expand 0 |> Grid.rotate -90 // |> tee Grid.printfn

    let galaxies =
        grid
        |> Grid.fold
            (fun g coord c ->
                if c = Hash then
                    g |> List.insertAt 0 (coord |> Point2D.ofTuple)
                else
                    g)
            List.empty
    // |> echo

    galaxies |> calcSumOfMinDistance

let part2 (Input grid) growthRate =
    // Grid is too large to expand in memory. We must expand just the galaxies...

    // find all rows that need expanding, sorted from smallest to largest...
    let rowsToExpand =
        let rec expandRows row acc =
            if row >= grid.Length then
                acc |> List.rev
            elif grid[row] |> Array.forall ((=) Dot) then
                expandRows (row + 1) (row :: acc)
            else
                expandRows (row + 1) acc

        expandRows 0 [] //|> echos "rowsToExpand"

    // find all columns that need expanding, sorted from smallest to largest...
    let columnsToExpand =
        let rec expandColumns col acc =
            if col >= grid[0].Length then
                acc |> List.rev
            elif grid |> Grid.col col |> Seq.forall ((=) Dot) then
                expandColumns (col + 1) (col :: acc)
            else
                expandColumns (col + 1) acc

        expandColumns 0 [] //|> echos "columnsToExpand"

    // find all galaxies and calculate their expanded position...
    let galaxies =
        grid
        |> Grid.fold
            (fun g (x, y) c ->
                if c = Hash then
                    let precedingCols = columnsToExpand |> Seq.takeWhile ((>) x) |> Seq.length
                    let x = x + (precedingCols * (growthRate - 1))

                    let precedingRows = rowsToExpand |> Seq.takeWhile ((>) y) |> Seq.length
                    let y = y + (precedingRows * (growthRate - 1))

                    g |> List.insertAt 0 ((x, y) |> Point2D.ofTuple)
                else
                    g)
            List.empty
    // |> tee (fun gal ->
    //     let w = 1 + (gal |> Seq.map (fun pt -> pt.x) |> Seq.max)
    //     let h = 1 + (gal |> Seq.map (fun pt -> pt.y) |> Seq.max)
    //     let g = Grid.create w h Dot
    //     gal |> List.iter (fun pt -> g |> Grid.set pt.x pt.y Hash)
    //     g |> Grid.printfn)

    galaxies |> calcSumOfMinDistance

let data = getInput () |> parseInput

part1 sample1 |> testEqual "Part 1 sample" 374L
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 9769724L

part2 sample2 2 |> testEqual "Part 2 sample" 374L
part2 sample2 10 |> testEqual "Part 2 sample" 1030L
part2 sample2 100 |> testEqual "Part 2 sample" 8410L

part2 data 1_000_000
|> tee (printfn "Part 2: %A")
|> testEqual "Part 2" 603020563700L
