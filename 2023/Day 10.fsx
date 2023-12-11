// https://adventofcode.com/2023/day/10
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type InputData = Input of Grid<char>

[<Literal>]
let Start = 'S'

[<Literal>]
let Horz = '─'

[<Literal>]
let Vert = '│'

[<Literal>]
let NE = '└'

[<Literal>]
let NW = '┘'

[<Literal>]
let SW = '┐'

[<Literal>]
let SE = '┌'

[<Literal>]
let Dot = '•'

[<Literal>]
let Unmapped = ' '

let parseInput (text: string) : InputData =
    text
        .Replace('|', Vert)
        .Replace('-', Horz)
        .Replace('L', NE)
        .Replace('J', NW)
        .Replace('7', SW)
        .Replace('F', SE)
        .Replace('.', Dot)
    |> String.splitAndTrim "\n"
    |> Grid.fromLines
    // |> tee Grid.printfn
    |> Input

let sample1 =
    parseInput
        """
7-F7-
.FJ|7
SJLL7
|F--J
LJ.LJ
"""

let sample2 =
    parseInput
        """
.F----7F7F7F7F-7....
.|F--7||||||||FJ....
.||.FJ||||||||L7....
FJL7L7LJLJ||LJ.L-7..
L--J.L7...LJS7F-7L7.
....F-J..F7FJ|L7L7L7
....L7.F7||L7|.L7L7|
.....|FJLJ|FJ|F7|.LJ
....FJL-7.||.||||...
....L---J.LJ.LJLJ...
"""

type Direction =
    | North
    | South
    | East
    | West

let (|Delta|) =
    function
    | North -> (0, -1)
    | South -> (0, +1)
    | East -> (+1, 0)
    | West -> (-1, 0)

/// Computes the next direction when approaching a character from
/// a specific direction, or `ValueNone` if the move is illegal
let tryNextDir dir ch =
    match dir, ch with
    | North, Vert -> ValueSome North // keep going North
    | South, Vert -> ValueSome South // keep going South
    | East, Horz -> ValueSome East // keep going East
    | West, Horz -> ValueSome West // keep going West
    | North, SW -> ValueSome West // turn West
    | North, SE -> ValueSome East // turn East
    | South, NW -> ValueSome West // turn West
    | South, NE -> ValueSome East // turn East
    | West, NE -> ValueSome North // turn North
    | West, SE -> ValueSome South // turn South
    | East, NW -> ValueSome North // turn North
    | East, SW -> ValueSome South // turn South
    | _ -> ValueNone

let findPath grid =
    let (w, h) = grid |> Grid.widthAndHeight
    let (startx, starty) = grid |> Grid.find Start //|> echos "start"

    let grid2 = Grid.create w h Unmapped
    grid2 |> Grid.set startx starty Start

    let rec mapPath x y (Delta(dx, dy) as dir) =
        let (x, y) = (x + dx), (y + dy)

        if grid2 |> Grid.itemOrDefault x y Dot = Unmapped then
            let next = grid |> Grid.item x y

            match tryNextDir dir next with
            | ValueSome nextDir ->
                grid2 |> Grid.trySet x y next
                mapPath x y nextDir
            | _ -> ()

    mapPath startx starty North
    mapPath startx starty South
    mapPath startx starty East
    mapPath startx starty West

    grid2, (startx, starty)

let part1 (Input grid) =
    let (grid, _) = grid |> findPath
    // grid |> Grid.printfn

    let pathLength =
        grid
        |> Grid.toSeq
        |> Seq.map (function
            | Vert
            | Horz
            | SW
            | SE
            | NW
            | NE
            | Start -> 1
            | _ -> 0)
        |> Seq.sum

    (pathLength + 1) / 2

let part2 (Input grid) =
    let grid, (startx, starty) = grid |> findPath
    // grid |> Grid.printfn

    let rec walkLoop x y (Delta(dx, dy) as dir) fn =
        let (nextx, nexty) = (x + dx), (y + dy)
        let nextc = grid |> Grid.item nextx nexty
        assert (nextc <> Unmapped)

        fn dir nextx nexty nextc

        match tryNextDir dir nextc with
        | ValueSome nextDir -> walkLoop nextx nexty nextDir fn
        | _ -> assert (nextc = Start)

    // We want to walk the loop such that the inside of the loop is on
    // the right hand side.
    // We dont know which side of the Start node will walk the inside
    // of the loop, so we will simply pick one of the two possibilites
    // and see if the sum of angles equals (corners - 2) * 180. If they do not, then the the other direction
    // will traverse the inside of the loop.
    let startDir =
        let startDirs =
            [ North; South; East; West ]
            |> List.filter (fun (Delta(dx, dy) as dir) ->
                grid
                |> Grid.tryItem (startx + dx) (starty + dy)
                |> Option.exists (fun nextc -> tryNextDir dir nextc <> ValueNone))
        // |> echos "Start directions"

        let mutable corners = 0
        let mutable angles = 0

        // sum up angles on the _right-hand_ side of the path...
        walkLoop startx starty startDirs[0] (fun dir _ _ c ->
            match dir, c with
            | North, SE
            | South, NW
            | East, SW
            | West, NE ->
                // inside corner
                corners <- corners + 1
                angles <- angles + 90
            | North, SW
            | South, NE
            | East, NW
            | West, SE ->
                // outside corner
                corners <- corners + 1
                angles <- angles + 270

            | _, Start when dir <> startDirs[0] ->
                corners <- corners + 1

                angles <-
                    angles
                    + match dir, startDirs[0] with
                      | North, East
                      | East, South
                      | South, West
                      | West, North -> 90 // inside corner
                      | _ -> 270 // outside corner

            | _ -> ())

        // (angles, corners) |> echo |> ignore

        if angles = (corners - 2) * 180 then
            startDirs[0]
        else
            startDirs[1]

    // fill inside the loop...
    let rec flood x y =
        if grid |> Grid.itemOrDefault x y Dot = Unmapped then
            grid |> Grid.set x y Dot
            flood (x + 1) (y)
            flood (x - 1) (y)
            flood (x) (y + 1)
            flood (x) (y - 1)

    // fill the _right-hand_ side of the path...
    walkLoop startx starty startDir (fun dir x y c ->
        let fillDir =
            match dir, c with
            | North, Vert -> ValueSome [ (+1, 0) ]
            | North, SE -> ValueNone // inside corner
            | North, SW -> ValueSome [ (+1, 0); (0, -1); (+1, -1) ]
            | South, Vert -> ValueSome [ (-1, 0) ]
            | South, NW -> ValueNone // inside corner
            | South, NE -> ValueSome [ (-1, 0); (0, +1); (-1, +1) ]
            | East, Horz -> ValueSome [ (0, +1) ]
            | East, SW -> ValueNone // inside corner
            | East, NW -> ValueSome [ (0, +1); (+1, 0); (+1, +1) ]
            | West, Horz -> ValueSome [ (0, -1) ]
            | West, NE -> ValueNone // inside corner
            | West, SE -> ValueSome [ (0, -1); (-1, 0); (-1, -1) ]
            | _ -> ValueNone

        match fillDir with
        | ValueSome deltas -> deltas |> List.iter (fun (dx, dy) -> flood (x + dx) (y + dy))
        | _ -> ())

    // grid |> Grid.printfn
    grid |> Grid.toSeq |> Seq.where ((=) Dot) |> Seq.length

let data = getInput () |> parseInput

part1 sample1 |> testEqual "Part 1 sample" 8
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 6768

part2 sample2 |> testEqual "Part 2 sample" 8
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 351
