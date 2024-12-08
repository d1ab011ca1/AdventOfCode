// https://adventofcode.com/2024/day/X
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = Rect * Map<char, Point2D list>

let parseInput (text: string) : InputData =
    let grid = text |> String.splitAndTrim "\n" |> Grid.fromLines
    let bounds = Rect.fromCoords (0, 0) (grid |> Grid.widthAndHeight)

    let antennas =
        grid
        |> Grid.fold
            (fun antennas (Point2D pt) antenna ->
                if antenna = '.' then
                    antennas
                else
                    antennas
                    |> Map.change antenna (function
                        | Some x -> Some(pt :: x)
                        | _ -> Some [ pt ]))
            Map.empty

    bounds, antennas

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
............
........0...
.....0......
.......0....
....0.......
......A.....
............
............
........A...
.........A..
............
............
"""

let sample2 = sample1

let data = lazy (getInput () |> parseData)

let part1 ((bounds, antennas): InputData) =
    let rec addAntinode offset p s =
        let pn = p |> Point2D.offset offset

        if bounds |> Rect.contains pn then
            (pn :: s) //|> addAntinode offset pn
        else
            s

    let antinodes =
        antennas
        |> Map.fold
            (fun antinodes antenna coords ->
                let rec loop s =
                    function
                    | [] -> s // done
                    | ({ x = x1; y = y1 } as p1) :: tail ->
                        let s =
                            tail
                            |> List.fold
                                (fun s ({ x = x2; y = y2 } as p2) ->
                                    let (dx, dy) = x2 - x1, y2 - y1
                                    s |> addAntinode (-dx, -dy) p1 |> addAntinode (+dx, +dy) p2)
                                s

                        tail |> loop s

                coords |> loop antinodes)
            List.empty
    // |> echo

    antinodes |> List.distinct |> List.length

let part2 ((bounds, antennas): InputData) =
    let rec addAntinode offset p s =
        let pn = p |> Point2D.offset offset

        if bounds |> Rect.contains pn then
            (pn :: s) |> addAntinode offset pn
        else
            s

    let antinodes =
        antennas
        |> Map.fold
            (fun antinodes antenna coords ->
                let rec loop s =
                    function
                    | [] -> s // done
                    | ({ x = x1; y = y1 } as p1) :: tail ->
                        let s =
                            tail
                            |> List.fold
                                (fun s ({ x = x2; y = y2 } as p2) ->
                                    let (dx, dy) = x2 - x1, y2 - y1
                                    s |> addAntinode (-dx, -dy) p1 |> addAntinode (+dx, +dy) p2)
                                s

                        tail |> loop (p1 :: s)

                coords |> loop antinodes)
            List.empty
    // |> echo

    antinodes |> List.distinct |> List.length

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 14
executePuzzle "Part 1 finale" (fun () -> part1 data.Value) 278

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 34
executePuzzle "Part 2 finale" (fun () -> part2 data.Value) 1067
