// https://adventofcode.com/2023/day/18
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

[<Literal>]
let Left = 'L'

[<Literal>]
let Right = 'R'

[<Literal>]
let Up = 'U'

[<Literal>]
let Down = 'D'


[<Literal>]
let Dot = '.'

[<Literal>]
let Hash = '#'

type Instruction = char * int * string
type InputData = Input of Instruction[]

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Array.map (fun s ->
        match s |> String.split " " with
        | [| d; m; c |] -> (d[0], Int32.Parse(m), c |> String.substr 2 6)
        | _ -> failwithf "Unexpected input: %s" s)
    |> Input
// |> tee (printfn "%A")

let sample1 =
    parseInput
        """
R 6 (#70c710)
D 5 (#0dc571)
L 2 (#5713f0)
D 2 (#d2c081)
R 2 (#59c680)
D 2 (#411b91)
L 5 (#8ceee2)
U 2 (#caa173)
L 1 (#1b58a2)
U 2 (#caa171)
R 2 (#7807d2)
U 3 (#a77fa3)
L 2 (#015232)
U 2 (#7a21e3)
"""

let sample2 = sample1

type Path = Grid.Coordinates list
type Instructions = (char * int) list

let createPath (instructions: Instructions) =
    let rec loop x y path =
        function
        | [] -> path
        | (dir, dist) :: tail ->
            let (dx, dy) =
                match dir with
                | Left -> (-dist, 0)
                | Right -> (+dist, 0)
                | Up -> (0, -dist)
                | Down -> (0, +dist)
                | _ -> failwith "dir"

            tail |> loop (x + dx) (y + dy) ((x + dx, y + dy) :: path)

    instructions |> loop 0 0 List.empty

let getExtents (path: Path) =
    let rec loop minX maxX minY maxY =
        function
        | [] -> minX, maxX, minY, maxY
        | (x, y) :: tail -> tail |> loop (min minX x) (max maxX x) (min minY y) (max maxY y)

    path |> loop 0 0 0 0

let normalizePath path =
    let (minX, maxX, minY, maxY) = getExtents path

    let rec loop res =
        function
        | [] -> res |> List.rev
        | (x, y) :: tail -> tail |> loop ((x - minX, y - minY) :: res)

    let path = path |> loop List.empty
    let (w, h) = maxX - minX + 1, maxY - minY + 1
    path, w, h

let drawLine x1 y1 x2 y2 g =
    for x = min x1 x2 to max x1 x2 do
        for y = min y1 y2 to max y1 y2 do
            g |> Grid.set x y Hash

let drawPath (path: Path) g =
    let rec loop x1 y1 =
        function
        | [] -> (x1, y1)
        | (x2, y2) :: tail ->
            g |> drawLine x1 y1 x2 y2
            loop x2 y2 tail

    match path with
    | (x0, y0) :: tail ->
        let (x1, y1) = loop x0 y0 tail
        g |> drawLine x1 y1 x0 y0
    | [] -> ()

let isClockwise (instructions: Instructions) =
    // walk the instructions. treat clockwise turns as "inside" angles.
    // if the sum of inside angles = (corners - 2) * 180, then
    // the loop is indeed clockwise. Otherwise it is counter-clockwise.

    let add dir1 dir2 corners angles =
        match dir1, dir2 with
        | Up, Right
        | Right, Down
        | Down, Left
        | Left, Up ->
            // inside angle
            (corners + 1), (angles + 90)
        | Up, Left
        | Right, Up
        | Left, Down
        | Down, Right ->
            // outside angle
            (corners + 1), (angles + 270)
        | _ ->
            // not an angle
            corners, angles

    let rec loop dir1 corners angles =
        function
        | [] -> dir1, corners, angles
        | (dir2, _) :: tail ->
            let (corners, angles) = add dir1 dir2 corners angles
            tail |> loop dir2 corners angles

    match instructions with
    | []
    | [ _ ] -> true
    | (dir0, _) :: tail ->
        let (dirN, corners, angles) = tail |> loop dir0 0 0

        // close the loop
        let (corners, angles) = add dirN dir0 corners angles

        // is it clockwise?
        angles = (corners - 2) * 180

let fillPath (path: Path) (instructions: Instructions) g =
    // identify the direction of the loop
    let clockwise = instructions |> isClockwise

    let rec flood x y =
        if g |> Grid.itemOrDefault x y ' ' = Dot then
            g |> Grid.set x y Hash
            flood (x + 1) (y)
            flood (x - 1) (y)
            flood (x) (y + 1)
            flood (x) (y - 1)

    // fill to the right side of the line...
    let rec fill x0 y0 x1 y1 =
        if y0 = y1 then
            if x0 < x1 then
                for x = x0 to x1 do
                    flood x (y0 + 1)
            else
                for x = x0 downto x1 do
                    flood x (y0 - 1)
        elif x0 = x1 then
            if y0 < y1 then
                for y = y0 to y1 do
                    flood (x0 - 1) y
            else
                for y = y0 downto y1 do
                    flood (x0 + 1) y

    let rec loop x1 y1 =
        function
        | [] -> ()
        | (dir, dist) :: tail ->
            let (dx, dy) =
                match dir with
                | Left -> (-dist, 0)
                | Right -> (+dist, 0)
                | Up -> (0, -dist)
                | Down -> (0, +dist)
                | _ -> failwith "dir"

            let (x2, y2) = x1 + dx, y1 + dy
            if clockwise then fill x1 y1 x2 y2 else fill x2 y2 x1 y1
            tail |> loop x2 y2

    let (x0, y0) = path |> List.head
    instructions |> loop x0 y0

let part1 (Input instructions) =
    let instructions =
        instructions |> Seq.map (fun (dir, dist, _) -> (dir, dist)) |> Seq.toList

    let (path, w, h) = createPath instructions |> normalizePath

    let g = Grid.create w h Dot

    g |> drawPath path

    fillPath path instructions g
    // g |> Grid.printfn

    g |> Grid.fold (fun sum _ c -> if c = Hash then sum + 1 else sum) 0

let part2 (Input data) = 0

let data = getInput () |> parseInput

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 62
executePuzzle "Part 1 finale" (fun () -> part1 data) 35991

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 0
executePuzzle "Part 2 finale" (fun () -> part2 data) 0
