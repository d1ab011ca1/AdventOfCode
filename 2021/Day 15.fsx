#r "nuget: FSharp.Collections.ParallelSeq"

open System
open System.IO
open FSharp.Collections.ParallelSeq

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
1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581
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
    |> Array.map (Seq.map hexDigitToInt >> Seq.toArray)

// printfn "%A" inputs
let sizeX = inputs.[0].Length
let sizeY = inputs.Length

type Point = (int * int)

let pathCost (inputs: int [] []) (path: Point seq) =
    path |> Seq.sumBy (fun (x, y) -> inputs.[y].[x])

let enumPaths (inputs: int [] []) =
    let sizeX = inputs.[0].Length
    let sizeY = inputs.Length

    let isValidPoint (x, y) =
        (0 <= x && x < sizeX) && (0 <= y && y < sizeY)

    let checkPoint (pt: Point) =
        if isValidPoint pt then
            Some pt
        else
            None

    let (|Up|_|) (x, y) = checkPoint (x, y - 1)
    let (|Down|_|) (x, y) = checkPoint (x, y + 1)
    let (|Left|_|) (x, y) = checkPoint (x - 1, y)
    let (|Right|_|) (x, y) = checkPoint (x + 1, y)

    let tryValue (x, y) =
        if isValidPoint (x, y) then
            Some inputs.[y].[x]
        else
            None

    let value pt =
        tryValue pt
        |> Option.defaultWith (fun _ -> failwithf "value out of range: %A" pt)

    let start: Point = (0, 0)
    let endPt: Point = (sizeX - 1, sizeY - 1)
    let startCost = value start

    let mutable minCost = Int32.MaxValue
    let mutable stack = [ (start, Set.empty, 0) ]

    seq {
        while stack |> Seq.length > 0 do
            for (pt, _, cost) in stack do
                if pt = endPt then
                    let cost = cost + (value pt)

                    if cost < minCost then
                        minCost <- cost
                        yield cost - startCost // answer

            stack <-
                stack
                |> PSeq.collect (fun (pt, path, cost) ->
                    let cost = cost + (value pt)

                    if cost < minCost && pt <> endPt then
                        let path = path |> Set.add pt

                        seq {
                            match pt with
                            | Up next -> yield (next, path, cost)
                            | _ -> ()

                            match pt with
                            | Down next -> yield (next, path, cost)
                            | _ -> ()

                            match pt with
                            | Left next -> yield (next, path, cost)
                            | _ -> ()

                            match pt with
                            | Right next -> yield (next, path, cost)
                            | _ -> ()
                        }
                    else
                        Seq.empty)
                // Sort by lowest cost
                |> PSeq.sortBy (fun (pt, _, cost) -> cost + (value pt))
                // Dont backtrack
                |> PSeq.filter (fun (pt, path, _) -> path |> (not << Set.contains pt))
                // Remove dups
                |> Seq.distinctBy (fun (pt, _, _) -> pt)
                |> Seq.toList
    }

let part1 () =
    let minCost = enumPaths inputs |> Seq.head
    printfn "Part 1: %d" minCost

let part2 () =
    // let inputs = [|[|8|]|]
    // let sizeX = 1
    // let sizeY = 1

    let large =
        Array.init (sizeY * 5) (fun y -> Array.create (sizeX * 5) 0)

    for y = 0 to sizeY - 1 do
        for x = 0 to sizeX - 1 do
            let v = inputs.[y].[x]

            for j in [ 0 .. 4 ] do
                for i in [ 0 .. 4 ] do
                    large.[j * sizeY + y].[i * sizeX + x] <- ((v + j + i - 1) % 9) + 1
    // printfn "%A" large

    let paths =
        enumPaths large
        |> Seq.map (fun x ->
            printfn "%d" x
            x)

    let minCost = paths |> Seq.sort |> Seq.head
    printfn "Part 2: %d" minCost

part1 () // 613
part2 () // 2899
