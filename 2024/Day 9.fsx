// https://adventofcode.com/2024/day/9
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type Length = byte
type Free = byte
type InputData = (Length * Free)[]

let parseInput (text: string) : InputData =
    let input =
        text |> String.trim |> String.toArray |> Array.map (fun c -> (byte) (c - '0'))

    input
    |> Array.splitInto ((input.Length + 1) / 2)
    |> Array.map (function
        | [| a; b |] -> (a, b)
        | [| a |] -> (a, 0uy)
        | _ -> failwith "what???")
// |> echo

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if data |> Array.exists (fst >> (=) 0uy) then
        failwith "contains empty file"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
2333133121414131402
"""

let sample2 = sample1

let data = lazy (getInput () |> parseData)

let part1 (data: InputData) =
    let fileLength i = (int) (fst data[i])
    let freeSpace i = (int) (snd data[i])

    let driveIter () =
        seq {
            for fileId = 0 to data.Length - 1 do
                for _ = 1 to (fileLength fileId) do
                    yield fileId |> ValueSome

                for _ = 1 to (freeSpace fileId) do
                    yield ValueNone
        }

    let endIter () =
        seq {
            for fileId = data.Length - 1 downto 0 do
                for _ = 1 to (fileLength fileId) do
                    yield fileId
        }

    let compactedLen = data |> Array.fold (fun len (cnt, _) -> len + (int) cnt) 0

    let compactIter =
        seq {
            let endEnum = (endIter ()).GetEnumerator()

            for x in driveIter () do
                match x with
                | ValueSome fileId -> fileId
                | _ ->
                    if not (endEnum.MoveNext()) then
                        failwith "consumed endIter"

                    endEnum.Current
        }

    compactIter
    |> Seq.take compactedLen
    |> Seq.fold (fun (s, i) id -> s + (int64) (i * id), i + 1) (0L, 0)
    |> fst

let part2 (data: InputData) =
    let data = data |> Array.mapi (fun id (len, free) -> (id, int len, int free))

    let rec tryMove fileId =
        let idxF = data |> Array.findIndex (fun (id, _, _) -> id = fileId)
        let (idF, lenF, freeF) = data[idxF]
        let mutable (idxI, moved) = (0, false)

        while idxI < idxF && not moved do
            let (idI, lenI, freeI) = data[idxI]

            // we can move F into I's free space?
            if freeI >= lenF then
                if idxF - 1 = idxI then
                    // F is next to I
                    // move I's free space to F
                    data[idxI] <- (idI, lenI, 0)
                    data[idxF] <- (idF, lenF, freeF + freeI)
                else
                    // give F's space to its predecessor (P)...
                    let (idP, lenP, freeP) = data[idxF - 1]
                    data[idxF - 1] <- (idP, lenP, freeP + lenF + freeF)
                    // move F to I+1 (shift i+1 thru f-1)
                    // and move I's remaining free space to F...
                    Array.blit data (idxI + 1) data (idxI + 2) (idxF - 1 - idxI)
                    data[idxI] <- (idI, lenI, 0)
                    data[idxI + 1] <- (idF, lenF, freeI - lenF)

                moved <- true

            idxI <- idxI + 1

        // try to move the next file...
        if fileId > 1 then
            tryMove (fileId - 1)

    tryMove (data.Length - 1)

    // compute the checksum...
    data
    |> Array.fold
        (fun (checksum, idx) (id, len, free) ->
            // checksum += sum(id * [idx .. idx+len-1])
            //          += id * sum([idx .. idx+len-1])
            //          += id * ((idx * len) + sum([0 .. len-1]))
            let checksum =
                checksum + (int64) id * (int64) ((idx * len) + (summatorial (len - 1)))

            (checksum, idx + len + free))
        (0L, 0)
    |> fst

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 1928L
executePuzzle "Part 1 finale" (fun () -> part1 data.Value) 6225730762521L

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 2858L
executePuzzle "Part 2 finale" (fun () -> part2 data.Value) 6250605700557L
