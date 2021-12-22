open System
open System.IO

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
..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

#..#.
#....
##..#
..#..
..###
"""

let lookup, map =
    let inputText = realInputText
    // let inputText = sampleInputText

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines.[0], lines |> Array.tail
// printfn "%A" (lookup,map)

let (|Value|) (c: char) = if c = '#' then 1 else 0

// Code below depends on lookup 0 and 511 being opposites
if lookup.[0b000_000_000] = lookup.[0b111_111_111] then
    failwith "Unexpected lookup values."

let value x y iter (map: string []) =
    if 0 <= x
       && x < map.[0].Length
       && 0 <= y
       && y < map.Length then
        map.[y].[x]
    // values outside the map depend on lookup[0] and the iteration
    elif lookup.[0] = '#' && iter % 2 = 1 then
        '#'
    else
        '.'

let outValue x y iter map =
    seq {
        for y = y - 1 to y + 1 do
            for x = x - 1 to x + 1 do
                value x y iter map
    }
    |> Seq.fold (fun v (Value b) -> (v <<< 1) ||| b) 0

let enhance iterations map =
    Seq.init iterations id
    |> Seq.fold
        (fun (map: string []) iter ->
            [| for y = -1 to map.Length do
                   seq {
                       for x = -1 to map.[0].Length do
                           lookup.[outValue x y iter map]
                   }
                   |> String.Concat |])
        map

let part1 () =

    let map = enhance 2 map
    // for s in map do
    //     printfn "%s" s

    let numWhite = map |> Seq.sumBy (Seq.sumBy (|Value|))

    printfn "Part 1: %d" numWhite


let part2 () =
    let numWhite =
        enhance 50 map |> Seq.sumBy (Seq.sumBy (|Value|))

    printfn "Part 2: %d" numWhite

part1 () // 5379
part2 () // 17917
