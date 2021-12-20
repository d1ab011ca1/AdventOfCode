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
    member this.bottom = this.location.y
    member this.top = this.location.y + this.size.dy
    member this.left = this.location.x
    member this.right = this.location.x + this.size.dx

    static member offset size r =
        { r with location = r.location |> Point.offset size }

    static member grow size r =
        { r with size = r.size |> Size.grow size }

    static member normalize r =
        // Ensure size is always positive by moving the location
        let r =
            if r.size.dy < 0 then
                { location = { r.location with y = r.location.y - -r.size.dy }
                  size = { r.size with dy = -r.size.dy } }
            else
                r

        if r.size.dx < 0 then
            { location = { r.location with x = r.location.x - -r.size.dx }
              size = { r.size with dx = -r.size.dx } }
        else
            r

    static member contains pt r =
        r.location.x <= pt.x
        && pt.x <= r.location.x + r.size.dx
        && r.location.y <= pt.y
        && pt.y <= r.location.y + r.size.dy

let inputs =
    let inputText = realInputText
    let inputText = sampleInputText

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    let m =
        Regex.Match(lines.[0], """x=(.+)\.\.(.+), y=(.+)\.\.(.+)""")

    let x1, x2, y1, y2 =
        m.Groups.[1].Value |> int, m.Groups.[2].Value |> int, m.Groups.[3].Value |> int, m.Groups.[4].Value |> int

    { location = { x = x1; y = y1 }
      size = { dx = x2 - x1; dy = y2 - y1 } }
    |> Rect.normalize

// printfn "%A" inputs

let summatorial x = x * (x + 1) / 2

let height vy step =
    // = (v - 0) + (v - 1) + (v - 2) + (v - 3) + ...
    vy * step - (summatorial (step - 1))

let distance vx step =
    // = (v - 0) + (v - 1) + (v - 2) + (v - 3) + ...
    // velocity goes to zero after `vx` steps.
    let step = if step > vx then vx else step
    vx * step - (summatorial (step - 1))

/// returns the minimum number of steps to reach the specified distance with velocity 0
let minSteps dist =
    // d = s(s + 1)/2
    // 2d = s^2 + s
    // s^2 + s - 2d = 0
    // plug into quadratic formula...
    // s = -1 +- sqrt(1^2 - 4 * 1 * -2d) / 2*1
    let d = float dist
    let r = Math.Sqrt(1. + 8. * d)
    let s1 = (-1. + r) / 2.
    let s2 = (-1. - r) / 2.
    Math.Max(s1, s2) |> Math.Floor |> int

/// returns the minimum X velocity needed to reach the specified distance. Any vx less
/// than the returned value will never reach
let minV dist =
    // velocity goes to zero after `s` steps thus minV is s.
    let s = minSteps dist
    if distance s s < dist then s + 1 else s

// printfn "%d,%d" (distance 8 20) (height 0 4)

let part1 () =
    let target = inputs

    let minSteps = minSteps target.left
    let maxY1 = Math.Abs(target.bottom) // assuming positive y1

    let maxY =
        seq {
            // printfn $"btm={target.bottom}, top={target.top}"
            // printfn $"v0=[{1}..{maxY1 - 1}]"

            for y1 = 1 to maxY1 - 1 do
                // assuming positive y1, takes (2 * y1 + 1) steps to return to zero
                // so the next step is the first below zero.
                let mutable s = Math.Max(2 * y1 + 1 + 1, minSteps)
                let mutable y = height y1 s
                while y > target.top do
                    s <- s + 1
                    y <- height y1 s
                if target.bottom <= y && y <= target.top then
                    // hit!
                    printfn $"y1={y1}: s={s} y={y}, MaxY={height y1 (y1 + 1)}"
                    height y1 (y1 + 1)
        }
        |> Seq.max

    printfn "Part 1: %A" (maxY)

let part2 () = printfn "Part 2: "

part1 () // 4851
part2 () //
