open System
open System.IO
open System.Text.RegularExpressions
open System.Text
open System.Collections.Generic

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
2199943210
3987894921
9856789892
8767896789
9899965678
"""

let inputs =
    let inputText = realInputText
    // let inputText = sampleInputText

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines
//printfn "%A" inputs

let sizeY = inputs.Length
let sizeX = inputs.[0].Length
//printfn "Size: %A" (sizeX, sizeY)

let value x y =
    if x < 0 || y < 0 || x >= sizeX || y >= sizeY then
        '9'
    else
        inputs.[y].[x]

let part1 () =
    let value x y = int (value x y) - int '0'

    let isMin x y =
        let v = value x y

        v < value (x - 1) y
        && v < value (x + 1) y
        && v < value x (y - 1)
        && v < value x (y + 1)

    let mins =
        [ for x = 0 to sizeX - 1 do
              for y = 0 to sizeY - 1 do
                  if isMin x y then yield (value x y) ]

    let riskLvl = mins |> Seq.sumBy ((+) 1)
    printfn "Part 1: %A" riskLvl

let part2 () =
    // bitmap of ocean floor used to flood-fill basins
    let map = Collections.BitArray(sizeX * sizeY)
    let isSet x y = map.[y * sizeX + x]
    let set x y = map.[y * sizeX + x] <- true

    // mark all the basin edges. we will flood-fill in between
    for y = 0 to sizeY - 1 do
        for x = 0 to sizeX - 1 do
            if value x y = '9' then set x y

    // flood-fill the basin containing point x,y and returns it's size
    let rec fill x y =
        let mutable size = 0

        if not <| isSet x y then
            set x y
            size <- size + 1

            if x < sizeX - 1 then
                size <- size + (fill (x + 1) y)

            if y < sizeY - 1 then
                size <- size + (fill x (y + 1))

            if x > 0 then
                size <- size + (fill (x - 1) y)

            if y > 0 then
                size <- size + (fill x (y - 1))

        size

    // find all basins
    let basins =
        seq {
            for y = 0 to sizeY - 1 do
                for x = 0 to sizeX - 1 do
                    match fill x y with
                    | 0 -> ()
                    | size -> yield size
        }

    let top3basins =
        basins |> Seq.sortDescending |> Seq.take 3

    let answer = top3basins |> Seq.fold (*) 1

    printfn "Part 2: %A" answer

part1 () //575
part2 () //1019700
