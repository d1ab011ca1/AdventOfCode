open System
open System.IO

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
#############
#...........#
###B#C#B#D###
  #A#D#C#A#
  #########
"""

let inputs =
    let inputText = realInputText
    // let inputText = sampleInputText

    let lines =
        inputText
            .Trim()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)

    lines
// printfn "%A" inputs

let parseRooms (line: string) =
    let line = line.TrimStart()

    let line =
        if line.[1] = '#' then
            line.Remove(0, 2)
        else
            line

    [| line.[1]
       line.[3]
       line.[5]
       line.[7] |]

let parseMap (map: string []) =
    Seq.zip (parseRooms map.[2]) (parseRooms map.[3])
    |> Seq.map (fun (a, b) -> [| a; b |])
    |> Seq.toArray

let energyOf =
    function
    | 'A' -> 1
    | 'B' -> 10
    | 'C' -> 100
    | 'D' -> 1000
    | unk -> failwithf "Unexpected char: '%c'" unk

let (|Energy|) c = energyOf c

let (|Room|_|) (s: string) =
    match s.[0] with
    | 'R' -> Some(s.Remove(0, 1) |> int)
    | _ -> None

let (|Hall|_|) (s: string) =
    match s.[0] with
    | 'H' -> Some(s.Remove(0, 1) |> int)
    | _ -> None

let hallwayPosOfRoom r = (r + 1) * 2

let printPuzzle (rooms: char [] []) (hallway: char []) =
    printfn "%s" (String.Concat hallway)

    for i = 0 to (rooms.[0].Length - 1) do
        printf " "

        for room in rooms do
            printf " %c" room.[i]

        printfn ""

let move (rooms: char [] []) (hallway: char []) (src, dst) =
    let rec moveR src dst =
        match src, dst with
        | Hall h, Room r ->
            if hallway.[h] = '.' then
                failwith $"Hallway {h} is empty."

            let room = rooms.[r]

            let ri =
                room
                |> Seq.tryFindIndexBack ((=) '.')
                |> Option.defaultWith (fun _ -> failwith $"Room {r} is full.")

            let energy = energyOf hallway.[h]
            let distInHallway = Math.Abs(h - (hallwayPosOfRoom r))
            let distInRoom = ri + 1
            room.[ri] <- hallway.[h]
            hallway.[h] <- '.'
            energy * (distInHallway + distInRoom)

        | Room r, Hall h ->
            let room = rooms.[r]

            let ri =
                room
                |> Seq.tryFindIndex ((<>) '.')
                |> Option.defaultWith (fun _ -> failwith $"Room {r} is empty.")

            if hallway.[h] <> '.' then
                failwith $"Hallway {h} is not empty: {hallway.[h]}."

            let energy = energyOf room.[ri]
            let distInHallway = Math.Abs(h - (hallwayPosOfRoom r))
            let distInRoom = ri + 1
            hallway.[h] <- room.[ri]
            room.[ri] <- '.'
            energy * (distInHallway + distInRoom)

        | Room a, Room b when a <> b ->
            let hallPos = hallwayPosOfRoom a
            let tmp = hallway.[hallPos]
            hallway.[hallPos] <- '.'

            try
                moveR src $"H{hallPos}" + moveR $"H{hallPos}" dst
            finally
                hallway.[hallPos] <- tmp

        | Hall a, Hall b ->
            if hallway.[a] = '.' then
                failwith $"Hallway {a} is empty."

            if hallway.[b] <> '.' then
                failwith $"Hallway {b} is not empty: {hallway.[b]}."

            let energy = energyOf hallway.[a]
            let distance = Math.Abs(a - b)

            if distance > 0 then
                hallway.[b] <- hallway.[a]
                hallway.[a] <- '.'

            energy * distance

        | _ -> failwith $"Unexpected move: {src} -> {dst}"

    moveR src dst

let part1 () =
    let rooms = inputs |> parseMap
    let hallway = ".. . . . ..".ToCharArray()

    // solved manually
    // R   0 1 2 3
    // H 01 3 5 7 90
    //   .. . . . ..
    //     C B D D
    //     B C A A

    let moves =
        [ "R3", "H9" // 2000
          "R3", "H1" //    9
          "H9", "R3" // 3000
          "R2", "R3" // 4000
          "R2", "H7" //    3
          "R0", "R2" //  700
          "R1", "H3" //   20
          "R1", "R2" //  500
          "H3", "R1" //   30
          "R0", "R1" //   50
          "H1", "R0" //    3
          "H7", "R0" ] //  6

    let sum = moves |> Seq.sumBy (move rooms hallway)

    printfn "Part 1: %d" sum
    // printPuzzle rooms hallway
    // printfn ""

let part2 () =

    let hallway = ".. . . . ..".ToCharArray()
    let extraRooms1 = parseRooms "#D#C#B#A#"
    let extraRooms2 = parseRooms "#D#B#A#C#"

    let rooms =
        inputs
        |> parseMap
        |> Array.mapi (fun i r ->
            [| r.[0]
               extraRooms1.[i]
               extraRooms2.[i]
               r.[1] |])

    printPuzzle rooms hallway
    printfn ""

    // R   0 1 2 3
    // H 01 3 5 7 90
    //   .. . . . ..
    //     C B D D
    //     D C B A
    //     D B A C
    //     B C A A

    let moves = []

    let sum = moves |> Seq.sumBy (move rooms hallway)

    printfn "Part 2: %d" sum

part1 () // 10321
part2 () //
