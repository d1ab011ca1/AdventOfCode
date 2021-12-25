#r "nuget: FSharp.Collections.ParallelSeq"

open System
open System.IO
open FSharp.Collections.ParallelSeq
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

let sampleInputText3 =
    """
on x=-5..47,y=-31..22,z=-19..33
on x=-44..5,y=-27..21,z=-14..35
on x=-49..-1,y=-11..42,z=-10..38
on x=-20..34,y=-40..6,z=-44..1
off x=26..39,y=40..50,z=-2..11
on x=-41..5,y=-41..6,z=-36..8
off x=-43..-33,y=-45..-28,z=7..25
on x=-33..15,y=-32..19,z=-34..11
off x=35..47,y=-46..-34,z=-11..5
on x=-14..36,y=-6..44,z=-16..29
on x=-57795..-6158,y=29564..72030,z=20435..90618
on x=36731..105352,y=-21140..28532,z=16094..90401
on x=30999..107136,y=-53464..15513,z=8553..71215
on x=13528..83982,y=-99403..-27377,z=-24141..23996
on x=-72682..-12347,y=18159..111354,z=7391..80950
on x=-1060..80757,y=-65301..-20884,z=-103788..-16709
on x=-83015..-9461,y=-72160..-8347,z=-81239..-26856
on x=-52752..22273,y=-49450..9096,z=54442..119054
on x=-29982..40483,y=-108474..-28371,z=-24328..38471
on x=-4958..62750,y=40422..118853,z=-7672..65583
on x=55694..108686,y=-43367..46958,z=-26781..48729
on x=-98497..-18186,y=-63569..3412,z=1232..88485
on x=-726..56291,y=-62629..13224,z=18033..85226
on x=-110886..-34664,y=-81338..-8658,z=8914..63723
on x=-55829..24974,y=-16897..54165,z=-121762..-28058
on x=-65152..-11147,y=22489..91432,z=-58782..1780
on x=-120100..-32970,y=-46592..27473,z=-11695..61039
on x=-18631..37533,y=-124565..-50804,z=-35667..28308
on x=-57817..18248,y=49321..117703,z=5745..55881
on x=14781..98692,y=-1341..70827,z=15753..70151
on x=-34419..55919,y=-19626..40991,z=39015..114138
on x=-60785..11593,y=-56135..2999,z=-95368..-26915
on x=-32178..58085,y=17647..101866,z=-91405..-8878
on x=-53655..12091,y=50097..105568,z=-75335..-4862
on x=-111166..-40997,y=-71714..2688,z=5609..50954
on x=-16602..70118,y=-98693..-44401,z=5197..76897
on x=16383..101554,y=4615..83635,z=-44907..18747
off x=-95822..-15171,y=-19987..48940,z=10804..104439
on x=-89813..-14614,y=16069..88491,z=-3297..45228
on x=41075..99376,y=-20427..49978,z=-52012..13762
on x=-21330..50085,y=-17944..62733,z=-112280..-30197
on x=-16478..35915,y=36008..118594,z=-7885..47086
off x=-98156..-27851,y=-49952..43171,z=-99005..-8456
off x=2032..69770,y=-71013..4824,z=7471..94418
on x=43670..120875,y=-42068..12382,z=-24787..38892
off x=37514..111226,y=-45862..25743,z=-16714..54663
off x=25699..97951,y=-30668..59918,z=-15349..69697
off x=-44271..17935,y=-9516..60759,z=49131..112598
on x=-61695..-5813,y=40978..94975,z=8655..80240
off x=-101086..-9439,y=-7088..67543,z=33935..83858
off x=18020..114017,y=-48931..32606,z=21474..89843
off x=-77139..10506,y=-89994..-18797,z=-80..59318
off x=8476..79288,y=-75520..11602,z=-96624..-24783
on x=-47488..-1262,y=24338..100707,z=16292..72967
off x=-84341..13987,y=2429..92914,z=-90671..-1318
off x=-37810..49457,y=-71013..-7894,z=-105357..-13188
off x=-27365..46395,y=31009..98017,z=15428..76570
off x=-70369..-16548,y=22648..78696,z=-1892..86821
on x=-53470..21291,y=-120233..-33476,z=-44150..38147
off x=-93533..-4276,y=-16170..68771,z=-104985..-24507
"""

// Shuffle an array
module Array =
    let private rand = new Random()

    let shuffle a =
        a
        |> Array.sortBy (fun _ -> rand.Next(0, a.Length))

let inputs =
    let inputText = realInputText
    // let inputText = sampleInputText
    // let inputText = sampleInputText2
    // let inputText = sampleInputText3

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

    static member zero = { x = 0; y = 0; z = 0 }
    static member ofTuple(x, y, z) = { x = x; y = y; z = z }
    static member toTuple pt = (pt.x, pt.y, pt.z)

    static member offset (dx, dy, dz) pt =
        { x = pt.x + dx
          y = pt.y + dy
          z = pt.z + dz }

type Cube =
    { p1: Point // the "smaller" point
      p2: Point } // the "larger" point
    override this.ToString() = $"[{this.p1}..{this.p2})"

    member this.left = this.p1.x
    member this.right = this.p2.x
    member this.bottom = this.p1.y
    member this.top = this.p2.y
    member this.back = this.p1.z
    member this.front = this.p2.z
    member this.width = this.right - this.left
    member this.height = this.top - this.bottom
    member this.depth = this.front - this.back

    static member empty = { p1 = Point.zero; p2 = Point.zero }

    static member isEmpty(c: Cube) =
        c.width = 0 || c.height = 0 || c.depth = 0

    /// Creates a normalized cube with the given points
    static member fromPoints p1 p2 = { p1 = p1; p2 = p2 } |> Cube.normalize

    /// Creates a normalized cube with the given points
    static member fromCoords xyz1 xyz2 =
        Cube.fromPoints (Point.ofTuple xyz1) (Point.ofTuple xyz2)

    static member dims(c: Cube) = (c.width, c.height, c.depth)

    /// Returns volume of a cube.
    static member volume(c: Cube) =
        Math.Abs(int64 c.width * int64 c.height * int64 c.depth)

    /// Offsets the cube by offsetting both p1 and p2.
    static member offset (dx, dy, dz) c =
        { c with
            p1 = c.p1 |> Point.offset (dx, dy, dz)
            p2 = c.p2 |> Point.offset (dx, dy, dz) }

    /// Grows the cube by offsetting p2.
    static member grow (dx, dy, dz) c =
        { c with p2 = c.p2 |> Point.offset (dx, dy, dz) }

    /// Returns a Cube with positive size in all 3 dims (p1.xyz <= p2.xyz).
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

        let c =
            if c.front < c.back then
                { c with
                    p1 = { c.p1 with z = c.front }
                    p2 = { c.p2 with z = c.back } }
            else
                c

        if Cube.isEmpty c then Cube.empty else c

    /// Checks if point intersects a cube. Cube must be normalized.
    static member contains pt (c: Cube) =
        c.left <= pt.x
        && pt.x < c.right
        && c.bottom <= pt.y
        && pt.y < c.top
        && c.back <= pt.z
        && pt.z < c.front

    /// Returns the union of the two cubes. Cube must be normalized.
    static member union (c1: Cube) (c2: Cube) =
        let x1, x2 = (c1, c2) ||> swapIf (c2.left < c1.left)

        let y1, y2 =
            (c1, c2) ||> swapIf (c2.bottom < c1.bottom)

        let z1, z2 = (c1, c2) ||> swapIf (c2.back < c1.back)

        let min a b = if a < b then a else b
        let max a b = if a > b then a else b

        { p1 =
            { x = min x1.left x2.left
              y = min y1.bottom y2.bottom
              z = min z1.back z2.back }
          p2 =
            { x = max x1.right x2.right
              y = max y1.top y2.top
              z = max z1.front z2.front } }

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

    static member intersects c1 c2 =
        not (Cube.intersection c1 c2 |> Cube.isEmpty)

    /// Returns `a` minus the intersection of `b`, or `None` if there is no intersection.
    /// Cubes must be normalized.
    static member tryDifference a b =
        match Cube.intersection a b with
        | i when i |> Cube.isEmpty -> None
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
                  a <- { a with p1 = { a.p1 with y = i.bottom } }

              if i.front = b.front && a.front <> b.front then
                  yield { a with p1 = { a.p1 with z = i.front } }
                  a <- { a with p2 = { a.p2 with z = i.front } }

              if i.back = b.back && a.back <> b.back then
                  yield { a with p2 = { a.p2 with z = i.back } }
                  a <- { a with p1 = { a.p1 with z = i.back } } ]
            |> Some


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

let initializationRegion =
    Cube.fromCoords (-50, -50, -50) (51, 51, 51)

let part1 () =

    let target = initializationRegion

    let numOn =
        // printfn "Applying cubes to %O" target

        let bits =
            Collections.BitArray(target |> Cube.volume |> int32)

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

        let mutable c = 0L

        for b in bits do
            if b then c <- c + 1L

        c

    printfn "Part 1: %d" numOn

let part2 () =

    let totalOn =
        cubes
        |> Seq.fold
            (fun onCubes ->
                function
                | (true, onCube) ->
                    let rec merge c =
                        match onCubes |> Seq.tryPick (Cube.tryDifference c) with
                        | Some subCubes ->
                            // intersected another 'On' cube. Merge the differences
                            subCubes |> Seq.collect merge |> Seq.toList
                        | None ->
                            // no intersection with other 'On' cubes.
                            [ c ]

                    onCubes @ (onCube |> merge)

                | (false, offCube) ->
                    // Split every 'on' cube which intersects 'offCube'
                    onCubes
                    |> Seq.collect (fun onCube ->
                        Cube.tryDifference onCube offCube
                        |> Option.defaultValue [ onCube ])
                    |> Seq.toList)
            List.empty

        |> Seq.sumBy Cube.volume

    printfn "Part 2: %d" totalOn

part1 () // 607573
part2 () // 1267133912086024
