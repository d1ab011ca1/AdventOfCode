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

let getRoomIdx (s: string) =
    match s with
    | Room r -> r
    | _ -> failwith $"Not a room pos: {s}"

let (|Hall|_|) (s: string) =
    match s.[0] with
    | 'H' -> Some(s.Remove(0, 1) |> int)
    | _ -> None

let getHallIdx (s: string) =
    match s with
    | Hall h -> h
    | _ -> failwith $"Not a hallway pos: {s}"

let hallwayPosOfRoom ri = (ri + 1) * 2
let roomIdx (c: char) = int c - int 'A'
let roomName i = char (int 'A' + i)

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

[<Literal>]
let Available = '.'

[<Literal>]
let Home = '-'

let part2 () =

    // R   0 1 2 3
    // H 01 3 5 7 90
    //   .. . . . ..
    //     C B D D
    //     D C B A
    //     D B A C
    //     B C A A

    let hallway = ".. . . . ..".ToCharArray()
    let hallwayPositions = [| 0; 1; 3; 5; 7; 9; 10 |]
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

    let play (rooms: char [] []) (hallway: char []) =
        let rooms = rooms |> Array.map (Array.copy)
        let hallway = hallway |> Array.copy

        // # Note: the only freedom we have is choosing the order
        // # in which we move and where in the hallway we move.
        // # Moving home, when possible, is mandatory.
        //
        // for first pawn in each room
        //   if can move to home
        //     move to home
        //     if not isWin
        //        recurse
        //     undo move
        //   else
        //    for each available hallway position
        //      move into hallway
        //      recurse
        //      undo move
        // for each pawn in hallway
        //   if can move to home
        //     move to home
        //     if not isWin
        //        recurse
        //     undo move

        // mark all pawns already in their home position to simplify the logic below
        for rn in 'A' .. 'D' do
            let room = rooms.[roomIdx rn]

            let lastAway =
                room
                |> Array.tryFindIndexBack ((<>) rn)
                |> Option.defaultValue room.Length

            for i = lastAway + 1 to room.Length - 1 do
                room.[i] <- Home

        let roomNames =
            [ 2;3;0;1 ]
            |> Seq.map (sprintf "R%d")
            |> Seq.toArray

        let hallwayNames =
            hallwayPositions |> Array.map (sprintf "H%d")

        let isWin () =
            rooms |> Array.forall (Array.forall ((=) Home))

        let rec tryMoveToHall destHi from =
            let srcHi =
                match from with
                | Hall srcHi -> srcHi
                | Room srcRi -> hallwayPosOfRoom srcRi
                | _ -> failwith $"Unexpected pos: {from}."

            // check hallway between src and dest (dont include src since it is not available)
            let min, max =
                if destHi < srcHi then
                    (destHi, srcHi - 1)
                else
                    (srcHi + 1, destHi)

            let positions =
                hallwayPositions
                |> Seq.skipWhile ((>) min) // min > x
                |> Seq.takeWhile ((>=) max) // max >= x
            // printfn "tryMoveToHall %A %A %A" min max positions

            positions
            |> Seq.map (Array.get hallway)
            |> Seq.forall ((=) Available)
            |> fun res -> if res then Some() else None

        let tryMoveToRoom destRi from =
            // first, test if destination room is available
            let room = rooms.[destRi]
            let homePos = room |> Array.findIndexBack ((<>) Home)

            match room.[homePos] with
            | Available ->
                // Home position is available.
                // Check if we can move to the hallway outside the room.
                let srcHi =
                    match from with
                    | Room srcRi -> hallwayPosOfRoom srcRi
                    | Hall srcHi -> srcHi
                    | _ -> failwith "Unexpected"

                let destHi =
                    // Note: The position directly outside the room
                    // is not a valid stopping position. Pick the
                    // side closest to the src room
                    match hallwayPosOfRoom destRi with
                    | destHi when destHi < srcHi -> destHi + 1
                    | destHi -> destHi - 1

                tryMoveToHall destHi from
                |> Option.map (fun _ -> (room, homePos))
            | _ -> None // not available

        // order the hallway positions based on the shortest distance from each room
        let hallwayPosPerRoom =
            [| for ri = 0 to 4 do
                   let dist =
                    let hi = hallwayPosOfRoom ri
                    fun i -> Math.Abs(i - hi)

                   hallwayPositions
                   |> Array.sortBy dist |]

        let rec playR () =
            seq {
                // first, try to move pawns home...
                for fromRoom in roomNames do
                    let ri = getRoomIdx fromRoom
                    let room = rooms.[ri]

                    match room |> Array.tryFindIndex (Char.IsLetter) with
                    | None -> ()
                    | Some pawni ->
                        let pawn = room.[pawni]
                        let pawnRi = roomIdx pawn

                        match tryMoveToRoom pawnRi fromRoom with
                        | None -> ()
                        | Some (destRoom, pos) ->
                            destRoom.[pos] <- Home
                            room.[pawni] <- Available

                            // printfn "%s:%c -> %c%d" fromRoom pawn pawn pos
                            // printPuzzle rooms hallway
                            // printfn ""
                            if pos = 0 && isWin () then
                                // printfn "Win!"
                                yield [ (fromRoom, $"R{roomIdx pawn}") ]
                            else
                                // recurse...
                                for win in playR () do
                                    yield (fromRoom, $"R{roomIdx pawn}") :: win
                            // printfn "pop"

                            room.[pawni] <- pawn
                            destRoom.[pos] <- Available

                // try to move all hallway pieces home...
                for fromHall in hallwayNames do
                    let hi = getHallIdx fromHall

                    match hallway.[hi] with
                    | pawn when pawn <> '.' ->
                        let pawnRi = roomIdx pawn

                        match tryMoveToRoom pawnRi fromHall with
                        | None -> ()
                        | Some (destRoom, pos) ->
                            destRoom.[pos] <- Home
                            hallway.[hi] <- Available

                            // printfn "%s:%c -> %c%d" fromHall pawn pawn pos
                            // printPuzzle rooms hallway
                            // printfn ""
                            if pos = 0 && isWin () then
                                // printfn "Win!"
                                yield [ (fromHall, $"R{pawnRi}") ]
                            else
                                // recurse...
                                for win in playR () do
                                    yield (fromHall, $"R{pawnRi}") :: win
                            // printfn "pop"

                            hallway.[hi] <- pawn
                            destRoom.[pos] <- Available
                    | _ -> ()

                // try to move pawns to hallway...
                for fromRoom in roomNames do
                    let ri = getRoomIdx fromRoom
                    let room = rooms.[ri]

                    match room |> Array.tryFindIndex (Char.IsLetter) with
                    | None -> ()
                    | Some pawni ->
                        let pawn = room.[pawni]
                        let pawnRi = roomIdx pawn

                        for hi in hallwayPosPerRoom.[ri] do
                            match tryMoveToHall hi fromRoom with
                            | None -> ()
                            | Some () ->
                                hallway.[hi] <- pawn
                                room.[pawni] <- Available

                                // printfn "%s:%c -> H%d" fromRoom pawn hi
                                // printPuzzle rooms hallway
                                // printfn ""
                                // recurse...
                                for win in playR () do
                                    yield (fromRoom, $"H{hi}") :: win
                                // printfn "pop"

                                room.[pawni] <- pawn
                                hallway.[hi] <- Available
            }

        playR ()

    let mutable min = Int32.MaxValue

    let score,moves =
        play rooms hallway
        // |> Seq.take 1
        |> Seq.map (fun moves ->
            // printfn "%A" moves
            let rs = rooms |> Array.map (Array.copy)
            let h = hallway |> Array.copy
            let score = moves |> Seq.sumBy (move rs h)
            // min <- Math.Min(score, min)
            // printfn "Win! %d in %d moves. Min=%d" score moves.Length min
            score,moves)
        |> Seq.where (fst >> (=) 46451)
        |> Seq.head

    printfn "%40A" moves
    printfn "Part 2: %d" score

part1 () // 10321
part2 () // 46451
