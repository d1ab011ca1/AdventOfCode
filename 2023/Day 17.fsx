// https://adventofcode.com/2023/day/17
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type InputData = Input of Grid<int>

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Grid.fromLines
    |> Grid.map (fun _ c -> digitToInt c)
    // |> tee (Grid.printfn)
    |> Input

let sample1 =
    parseInput
        """
2413432311323
3215453535623
3255245654254
3446585845452
4546657867536
1438598798454
4457876987766
3637877979653
4654967986887
4564679986453
1224686865563
2546548887735
4322674655533
"""

let sample2 =
    parseInput
        """
111111111111
999999999991
999999999991
999999999991
999999999991
"""

[<Struct>]
type Direction =
    | North
    | West
    | South
    | East

module Direction =
    let all = [| North; West; South; East |]

    let delta: Direction -> Grid.Coordinates =
        function
        | West -> (-1, 0)
        | East -> (+1, 0)
        | North -> (0, -1)
        | South -> (0, +1)

    let reverse =
        function
        | North -> South
        | South -> North
        | West -> East
        | East -> West

    let turnLeft =
        function
        | North -> West
        | West -> South
        | South -> East
        | East -> North

    let turnRight =
        function
        | North -> East
        | East -> South
        | South -> West
        | West -> North

    let toInt =
        function
        | North -> 0
        | East -> 1
        | South -> 2
        | West -> 3

    let (|Delta|) = delta

    let (|Reverse|) = reverse

open Direction

let inline move x y (Delta(dx, dy)) = (x + dx, y + dy)

let tryMove g x y dir =
    let (x', y') = move x y dir

    match g |> Grid.tryItem x' y' with
    | None -> None
    | Some v -> Some(x', y', dir, v)

let inline historyKey (x: int) (y: int) (d: Direction) (s: int) =
    // 14..14..2..2
    (y <<< 18) ||| (x <<< 4) ||| (toInt d <<< 2) ||| s

let part1 (Input g) =
    let (w, h) = g |> Grid.widthAndHeight
    let (endX, endY) = w - 1, h - 1

    let mutable minValue = Int32.MaxValue
    let frontier = Collections.Generic.PriorityQueue()
    let history = Collections.Generic.HashSet()
    let tryMove = tryMove g

    let rec loop () =
        match frontier.TryDequeue() with
        | false, _, _ ->
            // search complete
            minValue

        | true, (x, y, _, _), sum when x = endX && y = endY ->
            // found a new minimum
            minValue <- min sum minValue
            loop ()

        | true, (x, y, dir, steps), sum ->
            if steps < 3 then
                match tryMove x y dir with
                | Some(x', y', _, value) when sum + value < minValue ->
                    if history.Add(historyKey x' y' dir (steps + 1)) then
                        frontier.Enqueue((x', y', dir, steps + 1), sum + value)
                | _ -> ()

            match turnLeft dir |> tryMove x y with
            | Some(x1, y1, dir1, value) when sum + value < minValue ->
                if history.Add(historyKey x1 y1 dir1 1) then
                    frontier.Enqueue((x1, y1, dir1, 1), sum + value)
            | _ -> ()

            match turnRight dir |> tryMove x y with
            | Some(x1, y1, dir1, value) when sum + value < minValue ->
                if history.Add(historyKey x1 y1 dir1 1) then
                    frontier.Enqueue((x1, y1, dir1, 1), sum + value)
            | _ -> ()

            loop ()

    // Start in top-left (0,0).
    frontier.Enqueue((0, 0, South, 0), 0)
    frontier.Enqueue((0, 0, East, 0), 0)
    loop ()

let part2 (Input g) =
    let (w, h) = g |> Grid.widthAndHeight
    let (endX, endY) = w - 1, h - 1

    let frontier = Collections.Generic.PriorityQueue()
    let history = Collections.Generic.HashSet()
    let tryMove = tryMove g
    let mutable minValue = Int32.MaxValue

    let rec loop () =
        match frontier.TryDequeue() with
        | false, _, _ ->
            // search complete
            minValue

        | true, (x, y, _, steps), sum when x = endX && y = endY && steps >= 4 ->
            // found a path
            minValue <- min sum minValue
            loop ()

        | true, (x, y, dir, steps), sum ->
            if steps < 10 then
                match tryMove x y dir with
                | Some(x, y, _, value) when sum + value < minValue ->
                    if history.Add(historyKey x y dir (steps + 1)) then
                        frontier.Enqueue((x, y, dir, steps + 1), sum + value)
                | _ -> ()

            if steps >= 4 then
                match turnLeft dir |> tryMove x y with
                | Some(x, y, dir, value) when sum + value < minValue ->
                    if history.Add(historyKey x y dir 1) then
                        frontier.Enqueue((x, y, dir, 1), sum + value)
                | _ -> ()

                match turnRight dir |> tryMove x y with
                | Some(x, y, dir, value) when sum + value < minValue ->
                    if history.Add(historyKey x y dir 1) then
                        frontier.Enqueue((x, y, dir, 1), sum + value)
                | _ -> ()

            loop ()

    // Start in top-left (0,0).
    frontier.Enqueue((0, 0, South, 0), 0)
    frontier.Enqueue((0, 0, East, 0), 0)
    loop ()

let data = getInput () |> parseInput

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 102
executePuzzle "Part 1 finale" (fun () -> part1 data) 1128

executePuzzle "Part 2 sample1" (fun () -> part2 sample1) 94
executePuzzle "Part 2 sample2" (fun () -> part2 sample2) 71
executePuzzle "Part 2 finale" (fun () -> part2 data) 0 // < 1278
