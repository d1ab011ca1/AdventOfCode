// https://adventofcode.com/2023/day/16
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type InputData = Input of Grid<char>

[<Literal>]
let Undefined = ' '

[<Literal>]
let Dot = '.'

[<Literal>]
let Backslash = '\\'

[<Literal>]
let FwdSlash = '/'

[<Literal>]
let Vertical = '|'

[<Literal>]
let Horizontal = '-'

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Grid.fromLines
    // |> tee Grid.printfn
    |> Input

let sample1 =
    parseInput
        """
.|...\....
|.-.\.....
.....|-...
........|.
..........
.........\
..../.\\..
.-.-/..|..
.|....-|.\
..//.|....
"""

let sample2 = sample1

[<Struct>]
type Direction =
    | Left
    | Right
    | Up
    | Down

let simulate dir x y g =
    let energized = Collections.Generic.HashSet<_>()
    let path = Collections.Generic.HashSet<_>()

    let rec loop dir x y =
        match g |> Grid.tryItem x y with
        | None -> () // out of bounds
        | Some c ->
            if path.Add(ValueTuple.Create(x, y, dir)) then
                energized.Add(ValueTuple.Create(x, y)) |> ignore

                match dir, c with
                | Right, FwdSlash -> move x y Up |||> loop
                | Up, FwdSlash -> move x y Right |||> loop
                | Left, FwdSlash -> move x y Down |||> loop
                | Down, FwdSlash -> move x y Left |||> loop
                | Right, Backslash -> move x y Down |||> loop
                | Down, Backslash -> move x y Right |||> loop
                | Left, Backslash -> move x y Up |||> loop
                | Up, Backslash -> move x y Left |||> loop
                | (Up | Down), Horizontal -> split x y Left Right
                | (Left | Right), Vertical -> split x y Up Down
                | _ -> move x y dir |||> loop

    and move x y =
        function
        | Left -> Left, (x - 1), y
        | Right -> Right, (x + 1), y
        | Up -> Up, x, (y - 1)
        | Down -> Down, x, (y + 1)

    and split x y dir1 dir2 =
        move x y dir1 |||> loop
        move x y dir2 |||> loop

    loop dir x y

    energized.Count

let part1 (Input g) = g |> simulate Right 0 0

let part2 (Input g) =
    let mutable maximum = 0
    let (w, h) = g |> Grid.widthAndHeight

    for y = 0 to h - 1 do
        maximum <- g |> simulate Right 0 y |> max maximum
        maximum <- g |> simulate Left (w - 1) y |> max maximum

    for x = 0 to w - 1 do
        maximum <- g |> simulate Down x 0 |> max maximum
        maximum <- g |> simulate Up x (h - 1) |> max maximum

    maximum

let data = getInput () |> parseInput

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 46
executePuzzle "Part 1 finale" (fun () -> part1 data) 7482

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 51
executePuzzle "Part 2 finale" (fun () -> part2 data) 7896
