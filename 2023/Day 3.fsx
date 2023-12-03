// https://adventofcode.com/2023/day/3
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

let inline toSymbol (c: char) = -(int c)
let inline isSymbol i = i < 0
let inline isNumber i = i > 0
let dot = 0
let gear = '*' |> toSymbol

let parseInput (text: string) =
    text
    |> String.splitO "\n" (StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)
    |> Array.map (fun s ->
        let row = Array.create s.Length 0
        let mutable n = 0
        let mutable start = -1

        for i = 0 to s.Length - 1 do
            if Char.IsAsciiDigit(s[i]) then
                n <- n * 10 + (s[i] |> digitToInt)

                if start = -1 then
                    start <- i
            else
                if start <> -1 then
                    for ni = start to i - 1 do
                        row[ni] <- n

                    n <- 0
                    start <- -1

                row[i] <- if s[i] = '.' then dot else s[i] |> toSymbol

        if start <> -1 then
            for ni = start to s.Length - 1 do
                row[ni] <- n

        row) // |> tee (printfn "%A")

let part1 (grid: int array array) =
    // For each Number in the Grid, find all Symbols around the number.
    // For each found Symbol, add the Number to the sum.
    let (w, h) = grid |> Grid.widthAndHeight
    let mutable sum = 0

    let item x y = grid |> Grid.itemOrDefault x y 0

    let addToSumIfSymbol n x y =
        if item x y |> isSymbol then
            sum <- sum + n

    for y = 0 to h - 1 do
        let mutable x = 0

        while x < w do
            let n = item x y

            if isNumber n then
                // Left of number
                addToSumIfSymbol n (x - 1) (y - 1)
                addToSumIfSymbol n (x - 1) y
                addToSumIfSymbol n (x - 1) (y + 1)

                // Above and below
                addToSumIfSymbol n x (y - 1)
                addToSumIfSymbol n x (y + 1)
                x <- x + 1

                while item x y = n do
                    addToSumIfSymbol n x (y - 1)
                    addToSumIfSymbol n x (y + 1)
                    x <- x + 1

                // Right of number
                addToSumIfSymbol n x (y - 1)
                addToSumIfSymbol n x y
                addToSumIfSymbol n x (y + 1)

            x <- x + 1

    sum

let part2 (grid: int array array) =
    // For each gear (*) in the grid, find all Numbers around it.
    // If there are exactly 2 numbers, add their Product to the sum.
    let (w, h) = grid |> Grid.widthAndHeight
    let mutable sum = 0L

    let item x y = grid |> Grid.itemOrDefault x y 0

    let mutable numbers = List.empty

    let addNumber x y =
        let n = item x y

        if n |> isNumber then
            numbers <- n :: numbers

    for y = 0 to h - 1 do
        for x = 0 to w - 1 do
            let n = item x y

            if n = gear then
                numbers <- List.empty

                // Left and right of gear
                addNumber (x - 1) y
                addNumber (x + 1) y

                // Above
                let n = item x (y - 1)

                if n |> isNumber then
                    numbers <- n :: numbers
                else
                    addNumber (x - 1) (y - 1)
                    addNumber (x + 1) (y - 1)

                // Below
                let n = item x (y + 1)

                if n |> isNumber then
                    numbers <- n :: numbers
                else
                    addNumber (x - 1) (y + 1)
                    addNumber (x + 1) (y + 1)

                // Add product to sum IFF exactly 2 numbers...
                match numbers with
                | [ a; b ] -> sum <- sum + int64 (a * b)
                | _ -> ()

    sum

let sample1 =
    parseInput
        """
467..114..
...*......
..35..633.
......#...
617*......
.....+.58.
..592.....
......755.
...$.*....
.664.598..
"""

let sample2 = sample1

let data = getInput () |> parseInput

part1 sample1 |> testEqual "Part 1 sample" 4361
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 531561

part2 sample2 |> testEqual "Part 2 sample" 467835L
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 83279367L
