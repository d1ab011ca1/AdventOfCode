open System
open System.IO
open System.Text.RegularExpressions

let swapIf condition a b = if condition then (b, a) else (a, b)

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
on x=10..12,y=10..12,z=10..12
on x=11..13,y=11..13,z=11..13
off x=9..11,y=9..11,z=9..11
on x=10..10,y=10..10,z=10..10
"""

let sampleInputText2 =
    """
on x=-20..26,y=-36..17,z=-47..7
on x=-20..33,y=-21..23,z=-26..28
on x=-22..28,y=-29..23,z=-38..16
on x=-46..7,y=-6..46,z=-50..-1
on x=-49..1,y=-3..46,z=-24..28
on x=2..47,y=-22..22,z=-23..27
on x=-27..23,y=-28..26,z=-21..29
on x=-39..5,y=-6..47,z=-3..44
on x=-30..21,y=-8..43,z=-13..34
on x=-22..26,y=-27..20,z=-29..19
off x=-48..-32,y=26..41,z=-47..-37
on x=-12..35,y=6..50,z=-50..-2
off x=-48..-32,y=-32..-16,z=-15..-5
on x=-18..26,y=-33..15,z=-7..46
off x=-40..-22,y=-38..-28,z=23..41
on x=-16..35,y=-41..10,z=-47..6
off x=-32..-23,y=11..30,z=-14..3
on x=-49..-5,y=-3..45,z=-29..18
off x=18..30,y=-20..-8,z=-3..13
on x=-41..9,y=-7..43,z=-33..15
on x=-54112..-39298,y=-85059..-49293,z=-27449..7877
on x=967..23432,y=45373..81175,z=27513..53682
"""

let inputs =
    let inputText = realInputText
    // let inputText = sampleInputText
    // let inputText = sampleInputText2

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines


type Point =
    { x: int
      y: int
      z: int }
    override this.ToString() = $"({this.x},{this.y},{this.z})"
    static member empty = { x = 0; y = 0; z = 0 }
    static member ofTuple(x, y, z) = { x = x; y = y; z = z }
    static member toTuple pt = (pt.x, pt.y, pt.z)

    static member offset (dx, dy, dz) pt =
        { x = pt.x + dx
          y = pt.y + dy
          z = pt.z + dz }

type Cube =
    { p1: Point // the "smaller" point
      p2: Point } // the "larger" point
    override this.ToString() = $"{this.p1}...{this.p2}"
    member this.left = this.p1.x
    member this.right = this.p2.x
    member this.bottom = this.p1.y
    member this.top = this.p2.y
    member this.back = this.p1.z
    member this.front = this.p2.z
    member this.width = this.right - this.left
    member this.height = this.top - this.bottom
    member this.depth = this.front - this.back

    static member empty = { p1 = Point.empty; p2 = Point.empty }

    static member fromPoints p1 p2 = { p1 = p1; p2 = p2 } |> Cube.normalize

    static member fromCoords xyz1 xyz2 =
        Cube.fromPoints (Point.ofTuple xyz1) (Point.ofTuple xyz2)

    static member dims(c: Cube) = (c.width, c.height, c.depth)

    /// Returns volume of a cube.
    static member volume(c: Cube) = Math.Abs(c.width * c.height * c.depth)

    /// Offsets the cube by offsetting both p1 and p2.
    static member offset (dx, dy, dz) c =
        { c with
            p1 = c.p1 |> Point.offset (dx, dy, dz)
            p2 = c.p2 |> Point.offset (dx, dy, dz) }

    /// Grows the cube by offsetting p2.
    static member grow (dx, dy, dz) c =
        { c with p2 = c.p2 |> Point.offset (dx, dy, dz) }

    /// Ensures size is positive/whole number in all 3 dims
    static member normalize(c: Cube) =
        let c =
            if c.right < c.left then
                { c with
                    p1 = { c.p1 with x = c.right }
                    p2 = { c.p2 with x = c.left } }
            else
                c

        let c =
            if c.top < c.bottom then
                { c with
                    p1 = { c.p1 with y = c.top }
                    p2 = { c.p2 with y = c.bottom } }
            else
                c

        if c.front < c.back then
            { c with
                p1 = { c.p1 with z = c.front }
                p2 = { c.p2 with z = c.back } }
        else
            c

    /// Find intersection of two cubes. Cubes must be normalized.
    static member intersection (c1: Cube) (c2: Cube) =
        let x1, x2 = (c1, c2) ||> swapIf (c2.left < c1.left)

        let y1, y2 =
            (c1, c2) ||> swapIf (c2.bottom < c1.bottom)

        let z1, z2 = (c1, c2) ||> swapIf (c2.back < c1.back)

        let min a b = if a < b then a else b

        // must intersect in all 3 dims
        if x2.left < x1.right
           && y2.bottom < y1.top
           && z2.back < z1.front then
            { p1 =
                { x = x2.left
                  y = y2.bottom
                  z = z2.back }
              p2 =
                { x = min x1.right x2.right
                  y = min y1.top y2.top
                  z = min z1.front z2.front } }
        else
            Cube.empty

    /// Checks if point intersects a cube. Cube must be normalized.
    static member contains pt (c: Cube) =
        c.left <= pt.x
        && pt.x < c.right
        && c.bottom <= pt.y
        && pt.y < c.top
        && c.back <= pt.z
        && pt.z < c.front

let parse (ss: string seq) =
    let re =
        Regex("""(on|off) x=(.+)\.\.(.+),y=(.+)\.\.(.+),z=(.+)\.\.(.+)""")

    ss
    |> Seq.map (fun s ->
        let m = re.Match(s)

        if not m.Success then
            failwithf "Bad input: %s" s

        m.Groups.[1].Value = "on",
        Cube.fromCoords
            (int m.Groups.[2].Value, int m.Groups.[4].Value, int m.Groups.[6].Value)
            (int m.Groups.[3].Value, int m.Groups.[5].Value, int m.Groups.[7].Value)
        |> Cube.grow (1, 1, 1) // make inclusive
    )
    |> Seq.toArray

let cubes = inputs |> parse
// printfn "%A" cubes

let part1 () =
    let target =
        Cube.fromCoords (-50, -50, -50) (51, 51, 51)

    let bits =
        Collections.BitArray(target |> Cube.volume)


    for (state, cube) in cubes do
        match Cube.intersection target cube with
        | i when i = target -> bits.SetAll state
        | i ->
            // let bitIndex x y z =
            //     ((z - target.back) * target.width * target.height)
            //     + ((y - target.bottom) * target.width)
            //     + (x - target.left)
            // let setBit x y z v = bits.[bitIndex x y z] <- v
            //
            // for z = i.back to i.front - 1 do
            //     for y = i.bottom to i.top - 1 do
            //         for x = i.left to i.right - 1 do
            //             setBit x y z state
            for z = i.back to i.front - 1 do
                let idx =
                    (z - target.back) * target.width * target.height

                for y = i.bottom to i.top - 1 do
                    let idx =
                        idx + ((y - target.bottom) * target.width)

                    for x = i.left to i.right - 1 do
                        bits.[idx + (x - target.left)] <- state

    let numOn =
        seq {
            for b in bits do
                if b then yield b
        }
        |> Seq.length

    printfn "Part 1: %A" numOn

let part2 () = printfn "Part 2: "

part1 () // 607573
part2 () //
