// https://adventofcode.com/2023/day/21
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

[<Literal>]
let Rock = '#'

[<Literal>]
let Garden = '.'

[<Literal>]
let Start = 'S' // also a Garden


type InputData = Input of Grid<char>

let parseInput (text: string) : InputData =
    text |> String.splitAndTrim "\n" |> Grid.fromLines |> Input
// |> tee (printfn "%A")

let sample1 =
    parseInput
        """
...........
.....###.#.
.###.##..#.
..#.#...#..
....#.#....
.##..S####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
...........
"""

let sample2 = sample1

let moves = [| (-1, 0); (+1, 0); (0, -1); (0, +1) |]

let part1 (Input grid) totalSteps =
    let rec loop step frontier =
        if step > totalSteps then
            frontier |> Seq.length
        else
            let next = Collections.Generic.HashSet()

            for (x, y) in frontier do
                for (dx, dy) in moves do
                    let (x2, y2) = (x + dx), (y + dy)

                    match grid |> Grid.tryItem (x2) (y2) with
                    | Some(Garden | Start) -> next.Add((x2, y2)) |> ignore
                    | _ -> ()

            next |> loop (step + 1)

    Collections.Generic.HashSet([ grid |> Grid.find Start ]) |> loop 1

let part2 (Input grid) totalSteps =
    // Part 2 cannot be solved with simulation.
    //
    // Known properties:
    //  - The reached area resembles a checkerboard pattern
    //    which flips colors on each step.
    //  - Once a garden has been reached, it will toggle between
    //    unreached and reached on each subsequent step.
    //    - If a garden is reached on an odd step, it will be
    //      reached on all future odd steps.
    //    - If a garden is reached on an even step, it will be
    //      reached on all future even steps.
    //  - The number of reachable gardens in a _covered_ grid
    //    stabilizes to one of two values: one for the
    //    even steps, and another for odd steps.
    //
    // If I knew exactly how the frontier expands, I could "just"
    // compute the value for those grids intersecting the frontier,
    // since all grids in the middle will have one of the two
    // stable values. My problems are:
    // - I dont know how to predict the location of the frontier
    //   for a given step.
    //   - I suppect it roughly expands in a diamond pattern due
    //     to the unobstructed edges of the grid.
    //   - We do know that the grids on the periphery will be reached
    //     from their inside corner.
    // - I dont know how to predict the number of reached
    //   gardens in the middle grid. It is a function of the step
    //   and the grid dimensions - if the width is even, then
    //   the left and right grids would have the same value (due to
    //   the global checkerboard pattern), otherwise they would
    //   have alternating values.
    //
    // I could _probably_ answer these questions if I was to
    // animate the output for a reasonably large area, but I am
    // not going to do that.

    let (w, h) = grid |> Grid.widthAndHeight

    let inline item x y =
        let inline rem n d =
            match n % d with
            | m when m < 0 -> d + m
            | m -> m

        let x' = rem x w
        let y' = rem y h
        grid |> Grid.item x' y'

    let rec loop step (frontier: Collections.Generic.HashSet<_>) =

        if step > totalSteps then
            frontier |> Seq.length
        else
            if step > w then
                // The number of reachable gardens in a covered grid stabilizes pretty quickly
                frontier
                |> Seq.where (fun (x, y) -> x >= 0 && x < w && y >= 0 && y < h)
                |> Seq.length
                |> printfn "%d: %d" step

            let next = Collections.Generic.HashSet()

            for (x, y) in frontier do
                for mi = 0 to moves.Length - 1 do
                    let (dx, dy) = moves[mi]
                    let (x2, y2) as coords = (x + dx), (y + dy)

                    match item x2 y2 with
                    | Garden
                    | Start -> next.Add(coords) |> ignore
                    | _ -> ()

            // printfn "%d: %d, delta=%d" step (frontier.Count) (next.Count - frontier.Count)

            next |> loop (step + 1)

    Collections.Generic.HashSet([ grid |> Grid.find Start ]) |> loop 1

let data = getInput () |> parseInput

executePuzzle "Part 1 sample" (fun () -> part1 sample1 6) 16
executePuzzle "Part 1 finale" (fun () -> part1 data 64) 3770

executePuzzle "Part 2 sample x6" (fun () -> part2 sample2 6) 16
executePuzzle "Part 2 sample x10" (fun () -> part2 sample2 10) 50
executePuzzle "Part 2 sample x100" (fun () -> part2 sample2 100) 6536
executePuzzle "Part 2 sample x1000" (fun () -> part2 sample2 1000) 668697
executePuzzle "Part 2 sample x5000" (fun () -> part2 sample2 5000) 16733044
executePuzzle "Part 2 finale" (fun () -> part2 data 26501365) 0
