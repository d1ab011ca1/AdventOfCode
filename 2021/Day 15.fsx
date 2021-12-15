open System
open System.IO
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

let pathCost (path: Point seq) = path |> Seq.sumBy value

let start: Point = (0, 0)
let endPt: Point = (sizeX - 1, sizeY - 1)

let part1 () =

    let enumPaths () =
        let mutable minCost = Int32.MaxValue
        let mutable stack = [ (start, Set.empty, 0) ]

        seq {
            while stack |> Seq.length > 0 do
                for (pt, path, cost) in stack do
                    if pt = endPt then
                        yield pt :: (path |> Seq.toList) // answer
                        let cost = cost + (value pt)
                        if cost < minCost then minCost <- cost

                stack <-
                    seq {
                        for (pt, path, cost) in stack do
                            let cost = cost + (value pt)

                            if cost < minCost && pt <> endPt then
                                let path = path |> Set.add pt

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
                    // Remove dups
                    |> Seq.distinctBy (fun (pt, _, _) -> pt)
                    // Dont backtrack
                    |> Seq.where (fun (pt, path, _) -> path |> (not << Set.contains pt))
                    // Sort by lowest cost
                    |> Seq.sortBy (fun (pt, _, cost) -> cost + (value pt))
                    |> Seq.toList
        }

    let minPath = enumPaths () |> Seq.head
    let minCost = (minPath |> pathCost) - value start
    printfn "Part 1: %d" minCost

let part2 () = printfn "Part 2: "

part1 () // 613
part2 () //
