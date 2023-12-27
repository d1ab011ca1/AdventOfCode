// https://adventofcode.com/2023/day/X
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

[<StructuredFormatDisplay("({x}, {y}, {z} @ {dx}, {dy}, {dz})")>]
type Data =
    { x: float
      y: float
      z: float
      dx: float
      dy: float
      dz: float }

type InputData = Data[]

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Array.map (fun s ->
        let ns = s |> String.splitRE (Regex("""\s*[,@]\s*"""))

        { x = ns[0] |> Int64.Parse |> float
          y = ns[1] |> Int64.Parse |> float
          z = ns[2] |> Int64.Parse |> float
          dx = ns[3] |> Int32.Parse |> float
          dy = ns[4] |> Int32.Parse |> float
          dz = ns[5] |> Int32.Parse |> float })
// |> tee (printfn "%A")

let validateAssumptions (data: InputData) =
    if data |> Array.exists (fun a -> a.dx = 0 || a.dy = 0 || a.dz = 0) then
        failwith "unexpected zero delta"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
19, 13, 30 @ -2,  1, -2
18, 19, 22 @ -1, -1, -2
20, 25, 34 @ -2, -2, -4
12, 31, 28 @ -1, -2, -1
20, 19, 15 @  1, -5, -3
"""

let sample2 = sample1

let data = getInput () |> parseData

let pointIntersection l1 l2 =

    // Given two lines in standard form (ax + by + c = 0), then
    // the point of intersection is given by:
    //   (x, y) = ((c2*b1 - c1*b2)/(a1*b2 - a2*b1), (c1*a2 - c2*a1)/(a1*b2 - a2*b1))

    // Given a line in point-slope form ((y - y0) = m (x - x0)),
    // where m, x0, and y0 are constant, the standard form becomes:
    //   y - y0 - m (x - x0) = 0
    //   y - m*x + m*x0 - y0 = 0
    //   -m*x + 1y + (m*x0 - y0) = 0
    // Thus:
    //   a = -m
    //   b = 1
    //   c = m * x0 - y0

    let m1 = l1.dy / l1.dx
    let m2 = l2.dy / l2.dx

    if abs (m1 - m2) < 0.0000000001 then
        // lines are parallel, no intersection
        None
    else
        let (a1, c1) = -m1, m1 * l1.x - l1.y
        let (a2, c2) = -m2, m2 * l2.x - l2.y

        let x = ((c2 * 1.0) - (c1 * 1.0)) / (a1 * 1.0 - a2 * 1.0)
        let y = ((c1 * a2) - (c2 * a1)) / (a1 * 1.0 - a2 * 1.0)
        Some(x, y)

let part1 (data: InputData) minXY maxXY =
    seq {
        for i = 0 to data.Length - 1 do
            for j = i + 1 to data.Length - 1 do
                (data[i], data[j])
    }
    |> Seq.choose (fun (a, b) ->
        pointIntersection a b
        |> Option.filter (fun (x, y) -> minXY <= x && x <= maxXY && minXY <= y && y <= maxXY)
        |> Option.filter (fun (x, _) ->
            // do hailstones intersect this point in the future or past?
            let ta = (x - a.x) * a.dx
            let tb = (x - b.x) * b.dx
            ta >= 0 && tb >= 0))
    |> Seq.length

let part2 (data: InputData) =
    //
    0

executePuzzle "Part 1 sample" (fun () -> part1 sample1 7. 27.) 2
executePuzzle "Part 1 finale" (fun () -> part1 data 200000000000000. 400000000000000.) 16050

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 0
executePuzzle "Part 2 finale" (fun () -> part2 data) 0
