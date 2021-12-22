open System
open System.IO
open System.Text.RegularExpressions
open System.Collections.Generic

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

let sampleInputText1 =
    """
--- scanner 0 ---
-1,-1,1
-2,-2,2
-3,-3,3
-2,-3,1
5,6,-4
8,0,7
"""

let sampleInputText2 =
    """
--- scanner 0 ---
404,-588,-901
528,-643,409
-838,591,734
390,-675,-793
-537,-823,-458
-485,-357,347
-345,-311,381
-661,-816,-575
-876,649,763
-618,-824,-621
553,345,-567
474,580,667
-447,-329,318
-584,868,-557
544,-627,-890
564,392,-477
455,729,728
-892,524,684
-689,845,-530
423,-701,434
7,-33,-71
630,319,-379
443,580,662
-789,900,-551
459,-707,401

--- scanner 1 ---
686,422,578
605,423,415
515,917,-361
-336,658,858
95,138,22
-476,619,847
-340,-569,-846
567,-361,727
-460,603,-452
669,-402,600
729,430,532
-500,-761,534
-322,571,750
-466,-666,-811
-429,-592,574
-355,545,-477
703,-491,-529
-328,-685,520
413,935,-424
-391,539,-444
586,-435,557
-364,-763,-893
807,-499,-711
755,-354,-619
553,889,-390

--- scanner 2 ---
649,640,665
682,-795,504
-784,533,-524
-644,584,-595
-588,-843,648
-30,6,44
-674,560,763
500,723,-460
609,671,-379
-555,-800,653
-675,-892,-343
697,-426,-610
578,704,681
493,664,-388
-671,-858,530
-667,343,800
571,-461,-707
-138,-166,112
-889,563,-600
646,-828,498
640,759,510
-630,509,768
-681,-892,-333
673,-379,-804
-742,-814,-386
577,-820,562

--- scanner 3 ---
-589,542,597
605,-692,669
-500,565,-823
-660,373,557
-458,-679,-417
-488,449,543
-626,468,-788
338,-750,-386
528,-832,-391
562,-778,733
-938,-730,414
543,643,-506
-524,371,-870
407,773,750
-104,29,83
378,-903,-323
-778,-728,485
426,699,580
-438,-605,-362
-469,-447,-387
509,732,623
647,635,-688
-868,-804,481
614,-800,639
595,780,-596

--- scanner 4 ---
727,592,562
-293,-554,779
441,611,-461
-714,465,-776
-743,427,-804
-660,-479,-426
832,-632,460
927,-485,-438
408,393,-506
466,436,-512
110,16,151
-258,-428,682
-393,719,612
-211,-452,876
808,-476,-593
-575,615,604
-485,667,467
-680,325,-822
-627,-443,-432
872,-547,-609
833,512,582
807,604,487
839,-516,451
891,-625,532
-652,-548,-490
30,-46,-14
"""

type Point =
    { x: int
      y: int
      z: int }
    override this.ToString() = $"({this.x},{this.y},{this.z})"

let inputs =
    let inputText = realInputText
    // let inputText = sampleInputText1
    // let inputText = sampleInputText2

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    [ let mutable idx = 0

      while idx < lines.Length do
          if not <| lines.[idx].StartsWith("--- scanner ") then
              failwith "Unexpected line"

          idx <- idx + 1

          [ while idx < lines.Length
                  && not <| lines.[idx].StartsWith("--- scanner ") do
                let m =
                    Regex.Match(lines.[idx], """(.+),(.+),(.+)""")

                { x = m.Groups.[1].Value |> int
                  y = m.Groups.[2].Value |> int
                  z = m.Groups.[3].Value |> int }

                idx <- idx + 1 ] ]

// printfn "%A" inputs

let flip orientation p =
    match orientation with
    | 0 -> p
    // rotate X around Y-axis
    | 1
    | -3 -> { p with x = p.z; z = -p.x } // x -> -z, z -> +x
    | 2
    | -2 -> { p with x = -p.x; z = -p.z } // x -> -x, z -> -z
    | 3
    | -1 -> { p with x = -p.z; z = p.x } // x -> +z, z -> -x
    // rotate X around Z-axis
    | 4
    | -5 -> { p with x = -p.y; y = p.x } // x -> +y, y -> -x
    | 5
    | -4 -> { p with x = p.y; y = -p.x } // x -> -y, y -> +x
    | _ -> invalidArg (nameof orientation) "Must be between 0 and 4"


let spin orientation p =
    // spin X-axis
    match orientation with
    | 0 -> p
    | 1
    | -3 -> { p with y = p.z; z = -p.y } // y -> -z, z -> +y
    | 2
    | -2 -> { p with y = -p.y; z = -p.z } // y -> -y, z -> -z
    | 3
    | -1 -> { p with y = -p.z; z = p.y } // y -> +z, z -> -y
    | _ -> invalidArg (nameof orientation) "Must be between 0 and 3"

let rotateAxis orientation p =
    if orientation < 0 then
        p
        |> spin (orientation % 4) // -1..-3
        |> flip (orientation / 4) // -1..-5
    else
        p
        |> flip (orientation / 4) // 0..5
        |> spin (orientation % 4) // 0..3

let rotatePoints orientation pts = pts |> Seq.map (rotateAxis orientation)

let toPoint (x, y, z) = { x = x; y = y; z = z }

let offset (dx, dy, dz) pt =
    { x = pt.x + dx
      y = pt.y + dy
      z = pt.z + dz }

let delta pt1 pt2 =
    let dx = pt2.x - pt1.x
    let dy = pt2.y - pt1.y
    let dz = pt2.z - pt1.z
    dx, dy, dz

let distance pt1 pt2 =
    let dx, dy, dz = delta pt1 pt2
    Math.Abs(dx) + Math.Abs(dy) + Math.Abs(dz)

let print pts =
    for pt in pts do
        printfn $"{pt}"

let mutable orientationMap = Map [ 0, inputs.[0] ]
let mutable scannerMap = Map [ 0, { x = 0; y = 0; z = 0 } ]
let mutable searchNext = [ 0 ]

while not searchNext.IsEmpty do
    let ais = searchNext
    searchNext <- []

    for ai in ais do
        let a = orientationMap.[ai]

        for bi = 1 to inputs.Length - 1 do
            if orientationMap.ContainsKey bi then
                Seq.empty // already oriented
            else
                seq {
                    let b = inputs.[bi]

                    // check which rotation of B results in 12+ points being equidistant from 12+ points in A:
                    for r = 0 to 23 do
                        let b = b |> rotatePoints r |> Seq.toList

                        // get the delta of two arbitrary points
                        for i = 0 to a.Length - 11 do
                            for j = 0 to b.Length - 11 do
                                let d = delta a.[i] b.[j]

                                let equiD =
                                    [ for pa in a do
                                          for pb in b do
                                              if delta pa pb = d then pa ]

                                if equiD.Length >= 12 then
                                    let dx, dy, dz = d
                                    let b = b |> List.map (offset (-dx, -dy, -dz))
                                    let scannerPos = { x = dx; y = dy; z = dz }

                                    orientationMap <- orientationMap |> Map.add bi b
                                    scannerMap <- scannerMap |> Map.add bi scannerPos
                                    searchNext <- bi :: searchNext
                                    yield r // (bi + 1, r, d |> toPoint |> rotateAxis -r, pas)
                }
            |> Seq.tryHead
            |> ignore

let part1 () =


    let probeCount =
        orientationMap
        |> Seq.collect (fun kv -> kv.Value)
        |> Seq.distinct
        |> Seq.length

    printfn "Part 1: %A" probeCount

let part2 () =
    let maxDist =
        seq {
            for i = 0 to scannerMap.Count - 2 do
                let si = scannerMap.[i]

                for j = i + 1 to scannerMap.Count - 1 do
                    let sj = scannerMap.[j]
                    distance si sj
        }
        |> Seq.max

    printfn "Part 2: %d" maxDist

part1 () // 362
part2 () // 12204
