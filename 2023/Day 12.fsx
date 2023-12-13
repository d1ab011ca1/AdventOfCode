// https://adventofcode.com/2023/day/12
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif
#nowarn "57"

open System
open FSharpHelpers

type Input = (char[] * int[])[]
type InputData = Input of Input

[<Literal>]
let Dot = '.'

[<Literal>]
let Hash = '#'

[<Literal>]
let Q = '?'

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Seq.map (String.splitN " " 3)
    |> Seq.map (fun p -> p[0] |> String.toArray, p[1] |> String.split "," |> Array.map Int32.Parse)
    |> Seq.toArray
    // |> tee (Array.iter (fun (map, records) -> printfn "%s %A" map records))
    |> Input

let sample1 =
    parseInput
        """
???.### 1,1,3
.??..??...?##. 1,1,3
?#?#?#?#?#?#?#? 1,3,1,6
????.#...#... 4,1,1
????.######..#####. 1,6,5
?###???????? 3,2,1
"""

let sample2 = sample1

let safe =
    let m = obj ()
    fun (fn: unit -> unit) -> lock m fn

let printComplete n len cnt =
    // safe (fun () -> printfn "%d/%d complete: %d" n len cnt)
    ()

let joinMaps count (map: _ array) =
    [ for _ = 1 to count - 1 do
          map
          [| Q |]
      map ]
    |> Array.concat

let part1 (Input(data)) multipleCount =
    let mutable sum = 0L

    data
    |> Array.Parallel.iteri (fun n (map, records) ->
        let n = n + 1
        let map = map |> joinMaps multipleCount
        let records = records |> Array.create multipleCount |> Array.concat

        let rec countPossibleMatches mapIdx matches =
            if mapIdx >= map.Length then
                incIfMatch matches // end
            else
                match map[mapIdx] with
                | Hash
                | Dot -> countPossibleMatches (mapIdx + 1) matches
                | _ ->
                    map[mapIdx] <- Hash
                    let matches = countPossibleMatches (mapIdx + 1) matches

                    map[mapIdx] <- Dot
                    let matches = countPossibleMatches (mapIdx + 1) matches

                    map[mapIdx] <- Q
                    matches

        and incIfMatch cnt =
            if checkMatch 0 0 then
                // printfn "%s" (String map)
                cnt + 1
            else
                cnt

        and checkMatch mapIdx recordIdx =
            let (nextMapIdx, hashCount) = nextHashCount (nextHash mapIdx) 0

            if recordIdx = records.Length then (hashCount = 0) // done
            elif records[recordIdx] <> hashCount then false
            else checkMatch nextMapIdx (recordIdx + 1)

        and nextHash mapIdx =
            if mapIdx >= map.Length || map[mapIdx] = Hash then
                mapIdx // done
            else
                nextHash (mapIdx + 1)

        and nextHashCount mapIdx hashCount =
            if mapIdx >= map.Length || map[mapIdx] <> Hash then
                (mapIdx, hashCount) // done
            else
                nextHashCount (mapIdx + 1) (hashCount + 1)

        let matches = countPossibleMatches 0 0
        printComplete n data.Length matches
        Threading.Interlocked.Add(&sum, matches) |> ignore)

    sum

let part2 (Input(data)) multipleCount =
    let mutable sum = 0L

    data
    |> Array.Parallel.iteri (fun n (map, records) ->
        let n = n + 1
        let map = map |> joinMaps multipleCount
        let records = records |> Array.create multipleCount |> Array.concat

        // This is the key to solving Part 2!
        // Cache (mapIdx, recordIdx) -> submatches
        let submatchCache = new Collections.Generic.Dictionary<int * int, int64>()

        let rec countPossibleMatches recordIdx mapIdx dotsToMatch matches =
            if recordIdx = records.Length then
                // Matched all records.
                // Are we at end of map? (map may end with dots)
                if mapIdx <= map.Length && takeDots (map.Length - mapIdx) mapIdx then
                    matches + 1L // match!
                else
                    matches // no match

            elif mapIdx >= map.Length then
                // We have unmatched records
                matches // no match

            // can we match the requested dots and hashes?
            elif takeDots dotsToMatch mapIdx then
                let matches =
                    if takeHashes records[recordIdx] (mapIdx + dotsToMatch) then
                        matches + countSubmatches (mapIdx + dotsToMatch) recordIdx
                    else
                        matches

                // also try with additional dots
                countPossibleMatches recordIdx mapIdx (dotsToMatch + 1) matches
            else
                matches

        and countSubmatches mapIdx recordIdx =
            // check if this submatch has already been computed
            let key = (mapIdx, recordIdx)

            if submatchCache.ContainsKey(key) then
                submatchCache[key]
            else
                let submatches =
                    countPossibleMatches (recordIdx + 1) (mapIdx + records[recordIdx]) 1 0L

                submatchCache[key] <- submatches
                submatches

        and takeDots dotsToTake mapIdx =
            if dotsToTake = 0 then
                true
            elif mapIdx >= map.Length then
                false
            else
                match map[mapIdx] with
                | Dot
                | Q -> takeDots (dotsToTake - 1) (mapIdx + 1)
                | _ -> false

        and takeHashes hashesToTake mapIdx =
            if hashesToTake = 0 then
                true
            elif mapIdx >= map.Length then
                false
            else
                match map[mapIdx] with
                | Hash
                | Q -> takeHashes (hashesToTake - 1) (mapIdx + 1)
                | _ -> false


        let matches = countPossibleMatches 0 0 0 0L
        printComplete n data.Length matches
        Threading.Interlocked.Add(&sum, matches) |> ignore)

    sum

let data = getInput () |> parseInput

executePuzzle "Part 1 sample" (fun () -> part1 sample1 1) 21L
executePuzzle "Part 1 finale" (fun () -> part1 data 1) 8075L

executePuzzle "Part 2 sample x 1" (fun () -> part2 sample1 1) 21L
executePuzzle "Part 2 data x 1" (fun () -> part2 data 1) 8075L

executePuzzle "Part 2 sample" (fun () -> part2 sample2 5) 525152L
executePuzzle "Part 2 finale" (fun () -> part2 data 5) 4232520187524L
