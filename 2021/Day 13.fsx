open System
open System.IO
open System.Text.RegularExpressions
open System.Text

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
6,10
0,14
9,10
0,3
10,4
4,11
6,0
6,12
4,1
0,13
10,12
3,4
3,0
8,4
1,10
2,14
8,10
9,0

fold along y=7
fold along x=5
"""

let (dots, folds) =
    let inputText = realInputText
    // let inputText = sampleInputText

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    let re = Regex("(\d+,\d+)|fold along (.=\d+)")

    let (dots, folds) =
        lines
        |> Array.map (fun s -> re.Match(s))
        |> Array.where (fun m -> m.Success)
        |> Array.partition (fun m -> m.Groups.[1].Success)

    dots
    |> Array.map (fun m ->
        m.Groups.[1].Value.Split(",")
        |> Seq.map int
        |> Seq.pairwise
        |> Seq.exactlyOne),
    folds
    |> Array.map (fun m ->
        let ss = m.Groups.[2].Value.Split("=")
        (ss.[0], int ss.[1]))
//printfn "%A" (dots, folds)

let fold folds dots =

    let mutable sizeX = 1 + (dots |> Seq.map fst |> Seq.max)
    let mutable sizeY = 1 + (dots |> Seq.map snd |> Seq.max)

    let map =
        [| for _ = 0 to sizeY - 1 do
               StringBuilder(sizeX)
                   .Append(' ', repeatCount = sizeX) |]

    for (x, y) in dots do
        map.[y].[x] <- '#'

    for (axis, n) in folds do
        if axis = "x" then
            for y = 0 to sizeY - 1 do
                for x = n + 1 to sizeX - 1 do
                    if map.[y].[x] = '#' then
                        map.[y].[n - (x - n)] <- '#'

            sizeX <- n
        else
            for y = n + 1 to sizeY - 1 do
                for x = 0 to sizeX - 1 do
                    if map.[y].[x] = '#' then
                        map.[n - (y - n)].[x] <- '#'

            sizeY <- n

    [ for y = 0 to sizeY - 1 do
          map.[y].Length <- sizeX
          map.[y].ToString() ]
    |> Seq.ofList

let part1 () =
    let answer =
        dots
        |> fold (folds |> Seq.take 1)
        |> Seq.sumBy (Seq.where ((=) '#') >> Seq.length)

    printfn "Part 1: %d" answer

let part2 () =
    let map = dots |> fold folds

    printfn "Part 2: "

    for s in map do
        printfn "%s" s

part1 () // 781
part2 () // PERCGJPB
