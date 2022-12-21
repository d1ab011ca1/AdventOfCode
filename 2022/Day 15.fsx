// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open System.Text
open FSharpHelpers
open System.Text.RegularExpressions

type Coord = Point2D
let abs (x: int) = Math.Abs(x)

type Foo =
    { S: Coord
      B: Coord
      Dist: int }

    override this.ToString() =
        sprintf "S=%O, B=%O, Dist=%d" this.S this.B this.Dist

let parseInput (text: string) =
    let re =
        Regex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)")

    text
    |> parseInputText
    |> Array.map (fun line ->
        let m = re.Match(line).Groups
        let s = Coord.ofTuple (int m[1].Value, int m[2].Value)
        let b = Coord.ofTuple (int m[3].Value, int m[4].Value)

        { S = s
          B = b
          Dist = manhattanDistance b s })

type Intersection =
    { x: int
      len: int }

    override this.ToString() = sprintf "(x=%d len=%d)" this.x this.len

    static member compare a b =
        match a.x - b.x with
        | 0 -> b.len - a.len
        | dx -> dx

let rowIntersection rowNum (f: Foo) =
    match f.Dist - abs (rowNum - f.S.y) with
    | dx when dx >= 0 -> Some({ x = f.S.x - dx; len = dx + 1 + dx }, f)
    | _ -> None // no intersection with row

let print (foos: Foo[]) =
    let rect =
        let init = Rect.fromPoints foos[0].B foos[0].B

        foos
        |> Array.fold
            (fun rect f ->
                let p1 = f.S |> Coord.offset (-f.Dist, -f.Dist)
                let p2 = f.S |> Coord.offset (f.Dist + 1, f.Dist + 1)
                Rect.union rect (Rect.fromPoints p1 p2))
            init

    let grid = Array.init rect.height (fun _ -> StringBuilder().Append('.', rect.width))

    let put x y c =
        grid[y - rect.bottom][x - rect.left] <- c

    for f in foos |> Seq.sortByDescending (fun f -> f.Dist) do
        for dx = 0 to f.Dist do
            // let dy = f.Dist - dx
            for dy = 0 to f.Dist - dx do
                if dx + dy = f.Dist then
                    if dx = 0 then
                        put (f.S.x) (f.S.y - dy) '^'
                        put (f.S.x) (f.S.y + dy) 'v'
                    elif dy = 0 then
                        put (f.S.x - dx) (f.S.y) '<'
                        put (f.S.x + dx) (f.S.y) '>'
                    else
                        put (f.S.x - dx) (f.S.y - dy) '/'
                        put (f.S.x + dx) (f.S.y + dy) '/'
                        put (f.S.x - dx) (f.S.y + dy) '\\'
                        put (f.S.x + dx) (f.S.y - dy) '\\'
                else
                    put (f.S.x + dx) (f.S.y + dy) '#'
                    put (f.S.x + dx) (f.S.y - dy) '#'
                    put (f.S.x - dx) (f.S.y + dy) '#'
                    put (f.S.x - dx) (f.S.y - dy) '#'

    for f in foos do
        put f.S.x f.S.y 'S'
        put f.B.x f.B.y 'B'

    let printX10s () =
        printf "    "

        for x = rect.left to rect.right do
            match x % 5 with
            | 0 when x / 10 <> 0 -> printf $"{abs (x / 10)}"
            | _ -> printf " "

        printfn ""

    let printX1s () =
        printf "    "

        for x = rect.left to rect.right do
            match x % 5 with
            | -1 when x <> -1 -> printf "-"
            | 0 -> printf $"{abs (x % 10)}"
            | _ -> printf " "

        printfn ""

    printX10s ()
    printX1s ()
    grid |> Array.iteri (fun i f -> printfn $"{rect.bottom + i % 100, 3} {f}")

let part1 (inputs: Foo[]) (rowNum: int) =
    // printfn "%A" inputs

    let rec merge result rowIntersections =
        // Assumes rowIntersections is sorted according to Intersection.compare
        match rowIntersections with
        | [] -> result
        | [ (head, fh) ] ->
            // printfn "Adding %O" (head, fh)
            head :: result
        | ((head, fh) :: (((next, fn) :: nexttail) as tail)) ->
            match head.x + head.len - next.x with
            | overlapAmount when overlapAmount >= 0 ->
                // merge head with next and repeat
                let merged =
                    if overlapAmount >= next.len then
                        // next is entirely within head. Skip it
                        head
                    else
                        { head with len = head.len + next.len - overlapAmount }

                // printfn "  Merging %O with %O -> %O" head next merged
                merge result ((merged, fh) :: nexttail)
            | _ ->
                // no overlap. Return head, merge tail
                // printfn "Adding %O (no overlap)" (head, fh)
                merge (head :: result) tail

    // inputs |> print

    let is =
        inputs
        |> Seq.choose (rowIntersection rowNum)
        |> Seq.sortWith (fun (a, _) (b, _) -> Intersection.compare a b)

    // is |> Seq.map snd |> Seq.toArray |> print
    // is |> Seq.iter (printfn "%O")

    let length = is |> Seq.toList |> (merge []) |> Seq.fold (fun s i -> s + i.len) 0

    let beaconsInRow =
        inputs
        |> Seq.where (fun f -> f.B.y = rowNum)
        |> Seq.distinctBy (fun f -> f.B)
        |> Seq.length

    length - beaconsInRow

/// A basic 2-dimensional point.
[<StructAttribute>]
type Point =
    { x: float
      y: float }

    override this.ToString() = $"({this.x:f3},{this.y:f3})"
    static member zero = { x = 0; y = 0 }
    static member inline ofTuple(x, y) = { x = x; y = y }
    static member inline toTuple pt = (pt.x, pt.y)
    static member inline offset (dx, dy) pt = { x = pt.x + dx; y = pt.y + dy }

[<NoComparison>]
[<StructAttribute>]
type Rect =
    { p1: Point // the "smaller" point (bottom-left), inclusive
      p2: Point } // the "larger" point (top-right), exclusive

    override this.ToString() = $"[{this.p1}..{this.p2})"

    member this.left = this.p1.x
    member this.right = this.p2.x
    member this.bottom = this.p1.y
    member this.top = this.p2.y
    member this.width = this.right - this.left
    member this.height = this.top - this.bottom

    static member empty = { p1 = Point.zero; p2 = Point.zero }

    static member inline isEmpty(c: Rect) = c.width = 0 || c.height = 0

    /// Creates a normalized rect with the given points
    static member inline fromPoints p1 p2 = { p1 = p1; p2 = p2 }

    /// Checks if point intersects a Rect. Rect must be normalized.
    static member inline contains (pt: Point) (c: Rect) =
        c.left <= pt.x && pt.x < c.right && c.bottom <= pt.y && pt.y < c.top

    /// Returns the union of the two rects. Rect must be normalized.
    static member union (c1: Rect) (c2: Rect) =
        let x1, x2 = (c1, c2) ||> swapIf (c2.left < c1.left)
        let y1, y2 = (c1, c2) ||> swapIf (c2.bottom < c1.bottom)

        let inline min a b = if a < b then a else b
        let inline max a b = if a > b then a else b

        { p1 =
            { x = min x1.left x2.left
              y = min y1.bottom y2.bottom }
          p2 =
            { x = max x1.right x2.right
              y = max y1.top y2.top } }

    /// Find intersection of two rects. Rects must be normalized.
    static member intersection (c1: Rect) (c2: Rect) =
        let x1, x2 = (c1, c2) ||> swapIf (c2.left < c1.left)
        let y1, y2 = (c1, c2) ||> swapIf (c2.bottom < c1.bottom)

        if x2.left < x1.right && y2.bottom < y1.top then
            let inline min a b = if a < b then a else b

            { p1 = { x = x2.left; y = y2.bottom }
              p2 =
                { x = min x1.right x2.right
                  y = min y1.top y2.top } }
        else
            Rect.empty

    /// Returns `a` minus the intersection of `b`, or `None` if there is no intersection.
    /// Rects must be normalized.
    static member tryDifference a b =
        match Rect.intersection a b with
        | i when i |> Rect.isEmpty -> None
        | i ->
            [ let mutable a = a

              if i.right = b.right && a.right <> b.right then
                  yield { a with p1 = { a.p1 with x = i.right } }
                  a <- { a with p2 = { a.p2 with x = i.right } }

              if i.left = b.left && a.left <> b.left then
                  yield { a with p2 = { a.p2 with x = i.left } }
                  a <- { a with p1 = { a.p1 with x = i.left } }

              if i.top = b.top && a.top <> b.top then
                  yield { a with p1 = { a.p1 with y = i.top } }
                  a <- { a with p2 = { a.p2 with y = i.top } }

              if i.bottom = b.bottom && a.bottom <> b.bottom then
                  yield { a with p2 = { a.p2 with y = i.bottom } }
                  a <- { a with p1 = { a.p1 with y = i.bottom } } ]
            |> Some

let print2 (rs: Rect list) =
    let rect =
        let head = rs |> List.head
        rs |> List.fold Rect.union head

    let grid =
        Array.init (int rect.height + 1) (fun _ -> StringBuilder().Append('.', int rect.width + 1))

    let put (x: float) (y: float) c =
        let y = Math.Clamp(y - rect.bottom, 0, rect.height)
        let x = Math.Clamp(x - rect.left, 0, rect.width)
        grid[int y][int x] <- c

    for r in rs |> Seq.sortByDescending (fun f -> f.height * f.width) do
        put r.left r.bottom '┌'
        put r.left r.top '└'
        put r.right r.bottom '┐'
        put r.right r.top '┘'

        for x = (int r.left) + 1 to (int r.right) - 2 do
            put x r.bottom '─'
            put x r.top '─'

        for y = (int r.bottom) + 1 to (int r.top) - 2 do
            put r.left y '│'
            put r.right y '│'

    for r in rs do
        put (r.left + r.width / 2.) (r.bottom + r.height / 2.) 'S'

    grid |> Array.iteri (fun i f -> printfn $"{int rect.bottom + i % 100, 3} {f}")

let part2 (inputs: Foo[]) maxCoord =
    let rects =
        let sin45 = Math.Sin(Math.PI / 4.)
        let cos45 = Math.Cos(Math.PI / 4.)

        let rotate p =
            let x' = p.x * cos45 + p.y * sin45
            let y' = -p.x * sin45 + p.y * cos45
            Point.ofTuple (x', y')

        inputs
        |> Seq.map (fun f ->
            let p1 = Point.ofTuple (float (f.S.x + f.Dist), f.S.y)
            let p2 = Point.ofTuple (float (f.S.x - f.Dist), f.S.y)
            let p1' = p1 |> rotate
            let p2' = p2 |> rotate
            Rect.fromPoints p1' p2')
        |> Seq.toList

    // rects |> Seq.iter (printfn "%O")
    rects |> print2

    let searchSpace = Rect.fromPoints Point.zero (Point.ofTuple (maxCoord, maxCoord))

    let rec sub searchRects rects =
        match searchRects with
        | [] -> failwith "Out of search space."
        | searchRect :: tailsearchRects ->
            let mutable diffs = []

            let remainingRects =
                rects
                |> List.skipWhile (fun rect ->
                    match Rect.tryDifference searchRect rect with
                    | None -> true // keep searching
                    | Some d -> // intersection
                        diffs <- d
                        false)

            match remainingRects with
            | [] -> Some searchRect // no rects intersected
            | [ _ ] -> None // last rect intersected
            | _ :: tailRects -> sub (diffs @ tailsearchRects) tailRects

    rects |> sub [ searchSpace ]

let sampleInputText1 =
    """
Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3
"""

try
    [ sampleInputText1, 10, 20
      // getInput (), 2000000, 4000000
      ]
    |> Seq.iteri (fun inputNo (inputText, rowNum, maxCoord) ->
        let input = inputText |> parseInput
        printfn "Input %d Part 1: %O" inputNo (part1 input rowNum) //
        printfn "Input %d Part 2: %O" inputNo (part2 input maxCoord) //
        printfn "")
with
| :? OperationCanceledException as e -> printfn "*** %s" e.Message
| _ -> reraise ()
