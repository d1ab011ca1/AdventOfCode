// https://adventofcode.com/2023/day/5
#nowarn "57"
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type RangeMapEntry =
    { DstStart: int64
      SrcStart: int64
      Length: int64 }

type RangeMap = RangeMapEntry array

type InputType =
    { Seeds: int64 array
      Maps: RangeMap array }

let inline intersects (start1: int64, len1) (start2, len2) =
    if (start1 <= start2) then
        start2 < start1 + len1
    else
        start1 < start2 + len2

let inline intersection (start1: int64, len1) (start2, len2) =
    let lb = Math.Max(start1, start2)
    let ub = Math.Min(start1 + len1, start2 + len2)
    if lb < ub then (lb, ub - lb) else (-1, 0)

let parseInput (text: string) : InputType =
    let lines = text |> String.splitAndTrim "\n"

    let seeds =
        lines[0]
        |> String.split ": "
        |> Array.item 1
        |> String.split " "
        |> Array.map Int64.Parse

    let mutable idx = 1

    let readSection title =
        assert (lines[idx] |> String.startsWith title)
        idx <- idx + 1

        [| while idx < lines.Length && lines[idx][0] |> Char.IsAsciiDigit do
               let ns = lines[idx] |> String.split " " |> Array.map (Int64.fromString 10)

               { DstStart = ns[0]
                 SrcStart = ns[1]
                 Length = ns[2] }

               idx <- idx + 1 |]
        |> Array.sortBy (fun e -> e.SrcStart)

    let input =
        { Seeds = seeds
          Maps =
            [| readSection "seed-to-soil map"
               readSection "soil-to-fertilizer map"
               readSection "fertilizer-to-water map"
               readSection "water-to-light map"
               readSection "light-to-temperature map"
               readSection "temperature-to-humidity map"
               readSection "humidity-to-location map" |] }

    // verify ranges do not overlap...
    for map in input.Maps do
        map
        |> Seq.pairwise
        |> Seq.iter (fun (e1, e2) -> assert (not <| intersects (e1.SrcStart, e1.Length) (e2.SrcStart, e2.Length)))

        map
        |> Seq.sortBy (fun e -> e.DstStart)
        |> Seq.pairwise
        |> Seq.iter (fun (e1, e2) -> assert (not <| intersects (e1.DstStart, e1.Length) (e2.DstStart, e2.Length)))

    input
// |> tee (printfn "%A")

let sample1 =
    parseInput
        """
seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4
"""

let sample2 = sample1

let mapSrc2Dst src (map: RangeMap) =
    // map
    // |> Seq.tryFind (fun e -> e.SrcStart <= src && src < e.SrcStart + e.Length)
    // |> Option.map (fun e -> e.DstStart + (src - e.SrcStart))
    // |> Option.defaultValue src
    let rec loop n =
        if n >= map.Length then
            src // not found; use default mapping
        else
            let e = map[n]

            if e.SrcStart <= src && src < e.SrcStart + e.Length then
                e.DstStart + (src - e.SrcStart)
            else
                loop (n + 1)

    loop 0

let part1 (input: InputType) =
    input.Seeds
    |> Seq.map (fun s -> input.Maps |> Array.fold mapSrc2Dst s)
    |> Seq.min

let part2 (input: InputType) =
    // Map every seed range into the dest range, being mindful
    //  that the source range may map into multiple destination ranges.
    // Repeat this process for each map.
    // When done, return the smallest starting value.

    // Note: It is important to note that the map ranges
    // are sorted and they never overlap.

    // Create the starting sort range, i.e. the seeds...
    let mutable srcRanges =
        input.Seeds
        |> Seq.chunkBySize 2
        |> Seq.map (fun rg -> rg[0], rg[1])
        |> Seq.sortBy fst
        |> Seq.toList

    // Map the srcRanges through every map...
    for map in input.Maps do
        let mutable dstRanges = List.empty

        // Map each srcRange to one or more dstRanges.
        // Intersections between the source range and the
        // source map will map to a destination range.
        // Non-intersecting ranges will map to identity.
        for (srcStart, srcLength) as srcRange in srcRanges do
            let srcIntersections =
                [ for e in map do
                      let (si, li) = intersection srcRange (e.SrcStart, e.Length)

                      if li > 0 then
                          yield (si, li) ]

            let mutable next = srcStart

            dstRanges <-
                dstRanges
                @ [ for (si, li) in srcIntersections do
                        if next < si then // no intersection; default mapping
                            yield (next, si - next)

                        yield (mapSrc2Dst si map, li)
                        next <- si + li

                    if next < srcStart + srcLength then
                        yield (next, srcStart + srcLength - next) ]

        srcRanges <- dstRanges

    srcRanges |> Seq.map fst |> Seq.sort |> Seq.head


let data = getInput () |> parseInput

part1 sample1 |> testEqual "Part 1 sample" 35L
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 324724204L

part2 sample2 |> testEqual "Part 2 sample" 46L
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 104070862L
