// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open FSharpHelpers

let sampleInputText1 =
    """
30373
25512
65332
33549
35390
"""

let cookie = IO.File.ReadAllLines("cookie.txt") |> Array.head
let inputText = downloadInput cookie
// let inputText = sampleInputText1

let parseInput (text: string) =
    text |> parseInputText |> Array.map (Seq.map id >> Seq.toArray)

let inputs = inputText |> parseInput
let height = inputs.Length
let width = inputs[0].Length
//printfn "%A" inputs

let part1 () =
    let bm = Array.init height (fun _ -> Array.create width '.')

    for y = 0 to height - 1 do
        bm[y][0] <- '#'
        bm[y][width - 1] <- '#'

    for x = 1 to width - 2 do
        bm[0][x] <- '#'
        bm[height - 1][x] <- '#'

    for y = 1 to height - 2 do
        let mutable max = inputs[y][0]

        for x = 1 to width - 2 do
            if inputs[y][x] > max then
                bm[y][x] <- '#'
                max <- inputs[y][x]

        max <- inputs[y][width - 1]

        for x = width - 2 downto 1 do
            if inputs[y][x] > max then
                bm[y][x] <- '#'
                max <- inputs[y][x]

    for x = 1 to width - 2 do
        let mutable max = inputs[0][x]

        for y = 1 to height - 2 do
            if inputs[y][x] > max then
                bm[y][x] <- '#'
                max <- inputs[y][x]

        max <- inputs[height - 1][x]

        for y = height - 2 downto 1 do
            if inputs[y][x] > max then
                bm[y][x] <- '#'
                max <- inputs[y][x]

    // for y = 0 to height - 1 do
    //     for x = 0 to width - 1 do
    //         printf "%c" (bm[y][x])
    //     printfn ""

    let mutable count = 0
    for y = 0 to height - 1 do
        for x = 0 to width - 1 do
            if bm[y][x] = '#' then
                count <- count + 1

    printfn "Part 1: %d" count

let part2 () =
    let mutable maxscore = -1

    for y = 1 to height - 2 do
        for x = 1 to width - 2 do
            let max = inputs[y][x]

            // up (y-1..0)
            let mutable y' = y - 1
            while y' >= 0 && inputs[y'][x] < max do
                y' <- y' - 1
            let up = y - Math.Max(y', 0)

            // left (x-1..0)
            let mutable x' = x - 1
            while x' >= 0 && inputs[y][x'] < max do
                x' <- x' - 1
            let left = x - Math.Max(x', 0)

            // down (y+1..height]
            let mutable y' = y + 1
            while y' < height && inputs[y'][x] < max do
                y' <- y' + 1
            let down = Math.Min(y', height - 1) - y

            // right (x+1..width]
            let mutable x' = x + 1
            while x' < width && inputs[y][x'] < max do
                x' <- x' + 1
            let right = Math.Min(x', width - 1) - x

            let score = down * up * left * right
            if score > maxscore then
                //printfn $"{(x,y)}: {score} (u={up}, l={left}, d={down}, r={right})"
                maxscore <- score

    printfn "Part 2: %A" maxscore

part1 () // 1787
part2 () // 440640
