// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open FSharpHelpers

type Move =
    | U of int
    | D of int
    | L of int
    | R of int

    static member parse(line: string) =
        let parts = line.Split(' ', 2)

        match parts[0], int parts[1] with
        | "L", n -> L n
        | "R", n -> R n
        | "D", n -> D n
        | "U", n -> U n
        | _ -> failwithf "Unexpected: %s" line

    static member toOffsetAndCount mv =
        match mv with
        | R n -> (+1, 0), n
        | L n -> (-1, 0), n
        | D n -> (0, +1), n
        | U n -> (0, -1), n

let parseInput (text: string) =
    text |> parseInputText |> Array.map Move.parse

let printPath (path: Point2D list) =
    let minX, maxX, minY, maxY =
        path
        |> List.fold
            (fun (x, X, y, Y) pt -> Math.Min(x, pt.x), Math.Max(X, pt.x), Math.Min(y, pt.y), Math.Max(Y, pt.y))
            (0, 0, 0, 0)

    let width = maxX - minX + 1
    let height = maxY - minY + 1
    let grid = Array.init height (fun _ -> Text.StringBuilder().Append('.', width))
    path |> List.iter (fun pt -> grid[pt.y - minY][pt.x - minX] <- '#')
    grid[path[0].y - minY][path[0].x - minX] <- 's'
    grid |> Array.iter (printfn "%O")

let moveOneTail (h: Point2D, t: Point2D, s) onMove =
    let dx = h.x - t.x
    let dy = h.y - t.y

    match Math.Abs(dx), Math.Abs(dy) with
    | 0, 0
    | 1, 0
    | 1, 1 -> h, t, s
    | adx, ady ->
        // Move t to within 1 unit of h by scaling the delta to a unit value and subtract that from h.
        // Ex:
        //  delta: 0,3 -> 0,1
        //  delta: 4,4 -> 1,1
        //  delta: 5,3 -> 1,0

        let maxd = Math.Max(adx, ady)

        let t' =
            { x = h.x - (dx / maxd)
              y = h.y - (dy / maxd) }

        h, t', onMove (t', s)

let move mv hts onMove =
    let ofs, count = mv |> Move.toOffsetAndCount

    let rec moveOne count (h, t, s) =
        let h' = h |> Point2D.offset ofs
        let hts' = moveOneTail (h', t, s) onMove

        match count - 1 with
        | 0 -> hts'
        | remaining -> moveOne remaining hts'

    moveOne count hts

let part1 inputs =
    // printfn "%A" inputs

    let h = Point2D.zero
    let t = h
    let onMove (t', path) = t' :: path

    let h', t', path =
        let path0 = List.singleton t
        inputs |> Array.fold (fun hts mv -> move mv hts onMove) (h, t, path0)

    let path = path |> List.rev
    // printPath path

    printfn "Part 1: %d" (path |> Seq.distinct |> Seq.length)

let part2 inputs =
    let h = Point2D.zero
    let ts = Array.create 9 h
    let tiLast = ts.Length - 1

    let rec onMoveTail (t', ti) =
        ts[ti] <- t'

        if ti < tiLast then
            // move the rest...
            let _, _, ti' = moveOneTail (t', ts[ti + 1], ti + 1) onMoveTail
            ti'
        else
            ti

    let _, _, path =
        let onMoveTail0 (t0', path) =
            let path =
                if onMoveTail (t0', 0) = tiLast then
                    ts[tiLast] :: path
                else
                    path
            path

        let path0 = List.singleton ts[tiLast]
        inputs |> Array.fold (fun htp mv -> move mv htp onMoveTail0) (h, ts[0], path0)

    let path = path |> List.rev
    // printPath path

    printfn "Part 2: %d" (path |> Seq.distinct |> Seq.length)

let inputText1 =
    """
R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2"""

let inputText2 =
    """
R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20"""

[ inputText1
  inputText2
  getInput ()
  //
  ]
// |> Seq.skip 2
|> Seq.iteri (fun inputNo inputText ->
    printfn "Input %d:" inputNo

    let input = inputText |> parseInput

    part1 input //6464
    part2 input //2604

    printfn "")
