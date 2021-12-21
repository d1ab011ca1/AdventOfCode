open System
open System.IO
open System.Text.RegularExpressions

let digitToInt (c: char) =
    match int c with
    | n when (int '0') <= n && n <= (int '9') -> n - (int '0')
    | _ -> failwithf "Invalid decimal digit: %A" c

let hexDigitToInt (c: char) =
    match int c with
    | n when (int '0') <= n && n <= (int '9') -> n - (int '0')
    | n when (int 'A') <= n && n <= (int 'F') -> n - (int 'A') + 10
    | n when (int 'a') <= n && n <= (int 'f') -> n - (int 'a') + 10
    | _ -> failwithf "Invalid hex digit: %A" c

let (|DecChar|) = digitToInt
let (|HexChar|) = hexDigitToInt

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
target area: x=20..30, y=-10..-5
"""

let inputs =
    let inputText = realInputText
    let inputText = sampleInputText

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines.[0]

type Size =
    { dx: int
      dy: int }
    static member ofTuple(dx, dy) = { dx = dx; dy = dy }
    static member toTuple s = (s.dx, s.dy)

    static member (+)(s1, s2) =
        { dx = s1.dx + s2.dx
          dy = s1.dy + s2.dy }

    static member (-)(s1, s2) =
        { dx = s1.dx - s2.dx
          dy = s1.dy - s2.dy }

    static member grow (dx, dy) sz = { dx = sz.dx + dx; dy = sz.dy + dy }

let (|Size|) (dx, dy) = Size.ofTuple (dx, dy)

type Point =
    { x: int
      y: int }
    static member ofTuple(x, y) = { x = x; y = y }
    static member toTuple pt = (pt.x, pt.y)
    static member (+)(pt, sz) = { x = pt.x + sz.dx; y = pt.y + sz.dy }
    static member (-)(pt, sz) = { x = pt.x - sz.dx; y = pt.y - sz.dy }
    static member offset (dx, dy) pt = { x = pt.x + dx; y = pt.y + dy }

let (|Point|) (x, y) = Point.ofTuple (x, y)

type Rect =
    { location: Point
      size: Size }
    member this.width = this.size.dx
    member this.height = this.size.dy
    member this.left = this.location.x
    member this.right = this.left + this.width
    member this.bottom = this.location.y
    member this.top = this.right + this.height

    static member offset size r =
        { r with location = r.location |> Point.offset size }

    static member grow size r =
        { r with size = r.size |> Size.grow size }

    static member normalize(r: Rect) =
        // Ensure size is always positive by moving the location
        let r =
            if r.height < 0 then
                { location = { r.location with y = r.right - -r.height }
                  size = { r.size with dy = -r.height } }
            else
                r

        if r.width < 0 then
            { location = { r.location with x = r.left - -r.width }
              size = { r.size with dx = -r.width } }
        else
            r

    static member contains pt (r: Rect) =
        r.left <= pt.x
        && pt.x < r.left + r.width
        && r.right <= pt.y
        && pt.y < r.right + r.height

let parse s =
    let m =
        Regex.Match(s, """x=(.+)\.\.(.+), y=(.+)\.\.(.+)""")

    let x1, x2, y1, y2 =
        m.Groups.[1].Value |> int, m.Groups.[2].Value |> int, m.Groups.[3].Value |> int, m.Groups.[4].Value |> int

    { location = { x = x1; y = y1 }
      size = { dx = x2 - x1; dy = y2 - y1 } }
    |> Rect.normalize
    |> Rect.grow (1, 1)

// Computes the sum of integers [1..x]
let summatorial x = x * (x + 1) / 2

/// Returns the height (delta Y) after so many steps given the initial Y velocity.
let height (v1: int) steps =
    // = v1 + (v1 - 1) + (v1 - 2) + (v1 - 3) + ... + (v1 - (steps - 1))
    // = (v1 * steps) - 0 - 1 - 2 - 3 - ... - (steps - 1)
    // = (v1 * steps) - (1 + 2 + 3 + ... + (steps - 1))
    v1 * steps - (summatorial (steps - 1))

/// Returns the distance (delta X) after so many steps given the initial X velocity.
let rec distance (v1: int) steps =
    if v1 < 0 then
        -(distance (Math.Abs(v1)) steps)
    else
        // velocity goes to zero after v1 steps.
        let steps = if steps > v1 then v1 else steps
        v1 * steps - (summatorial (steps - 1))

/// Returns the maximum number of steps to reach the specified distance.
/// Assumes smallest possible initial X velocity.
let maxStepsX dist =
    // d = s(s + 1)/2
    // 2d = s^2 + s
    // s^2 + s - 2d = 0
    // plug into quadratic formula...
    // s = -1 +- sqrt(1^2 - 4 * 1 * -2d) / 2*1
    let d = float dist
    let r = Math.Sqrt(1. + 8. * d)
    let s1 = (-1. + r) / 2.
    let s2 = (-1. - r) / 2.

    // return the smallest positive value
    if s1 < 0. then s2
    elif s2 < 0. then s1
    else Math.Min(s1, s2) // |> Math.Floor |> int

let target = inputs |> parse
// printfn "%A" target

let part1 () =

    let maxStepsX =
        maxStepsX target.left |> Math.Floor |> int

    let maxY1 = Math.Abs(target.bottom) // assuming positive y1

    let maxY =
        seq {
            // printfn $"btm={target.bottom}, top={target.top}"
            // printfn $"v0=[{1}..{maxY1 - 1}]"

            for y1 = 1 to maxY1 - 1 do
                // assuming positive y1, takes (2 * y1 + 1) steps to return to zero
                // so the next step is the first below zero.
                let mutable s = Math.Max(2 * y1 + 1 + 1, maxStepsX)
                let mutable y = height y1 s

                while y > target.top do
                    s <- s + 1
                    y <- height y1 s

                if target.bottom <= y && y <= target.top then
                    // hit!
                    //printfn $"y1={y1}: s={s} y={y}, MaxY={height y1 (y1 + 1)}"
                    height y1 (y1 + 1)
        }
        |> Seq.max

    printfn "Part 1: %A" (maxY)

let part2 () =

    // 1 step - Can hit every point
    // 1 step - Can hit every point where (n - summatorial(1-1)) / 1 is an integer
    // 1 step - Can hit every point where (n - 0) / 1 is an integer
    let n = target.width * target.height
    printfn $"1: {n}"

    // 2 steps - Can hit every odd (x,y) point
    // 2 steps - Can hit every point where (n - summatorial(2-1)) / 2 is an integer
    // 2 steps - Can hit every point where (n - 1) / 2 is an integer
    let n =
        n
        + (target.width / 2
           + ((target.width % 2) * (target.left % 2)))
          * (target.height / 2
             + ((target.height % 2) * (target.top % 2)))

    printfn $"2: {n}"
    // 3 steps - Can hit point where (n - summatorial(3-1)) / 3 is an integer
    // 3 steps - Can hit point where (n - 3) / 3 is an integer

    // 4 steps - Can hit point where (n - summatorial(4-1)) / 4 is an integer
    // 4 steps - Can hit point where (n - 6) / 4 is an integer

    // 5 steps - Can hit point where (n - summatorial(5-1)) / 5 is an integer
    // 5 steps - Can hit point where (n - 10) / 5 is an integer

    printfn "Part 2: "

part1 () // 4851
part2 () //
