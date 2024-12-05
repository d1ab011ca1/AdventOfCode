// https://adventofcode.com/2024/day/5
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type Rules = int[]
type Pages = int[]
type InputData = Rules[] * Map<int, Rules> * Pages[] * Pages[]

let parseInput (text: string) : InputData =
    let text = text.Trim() |> String.split "\n" |> Array.map String.trim
    let idx = text |> Array.findIndex (String.IsNullOrWhiteSpace)

    let rules =
        text
        |> Array.take idx
        |> Array.map (String.split "|" >> Array.map (Int32.fromString 10))

    let updates =
        text
        |> Array.skip (idx + 1)
        |> Array.map (String.split "," >> Array.map (Int32.fromString 10))

    // group the rules by index 0...
    let ruleLookup =
        rules
        |> Array.fold
            (fun s t ->
                s
                |> Map.change t[0] (fun a ->
                    let a = a |> Option.defaultWith (fun () -> ResizeArray<int>(Capacity = 8))
                    a.Add(t[1])
                    a |> Some))
            Map.empty
        |> Map.map (fun _ t -> t.ToArray())

    let results =
        updates
        |> Array.groupBy (fun pages ->
            let mutable valid = true
            let mutable i = 0

            while valid && i < pages.Length do
                match ruleLookup.TryGetValue pages[i] with
                | false, _ -> () // ignore
                | true, page_rules ->
                    let mutable j = 0

                    while valid && j < page_rules.Length do
                        let n = Array.IndexOf(pages, page_rules[j])

                        if 0 <= n && n <= i then
                            valid <- false

                        j <- j + 1

                i <- i + 1

            valid)
        |> Map.ofSeq

    rules, ruleLookup, results[true], results[false]
// |> echo

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47
"""

let sample2 = sample1

let data = getInput () |> parseData

let part1 ((_, _, valid, _): InputData) =
    valid |> Seq.sumBy (fun pages -> pages[pages.Length / 2])

let part2 ((rules, ruleLookup, _, invalid): InputData) =
    invalid
    |> Array.map (fun pages ->
        let pages = pages |> Array.map id
        let mutable i = 0

        while i < pages.Length do
            let mutable swapped = false

            match ruleLookup.TryGetValue pages[i] with
            | false, _ -> () // ignore
            | true, page_rules ->
                let mutable j = 0

                while not swapped && j < page_rules.Length do
                    let n = Array.IndexOf(pages, page_rules[j])

                    if 0 <= n && n <= i then
                        let tmp = pages[i]
                        pages[i] <- pages[n]
                        pages[n] <- tmp
                        swapped <- true

                    j <- j + 1

            if swapped then
                i <- 0 // restart the check
            else
                i <- i + 1

        pages)
    |> Seq.sumBy (fun pages -> pages[pages.Length / 2])

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 143
executePuzzle "Part 1 finale" (fun () -> part1 data) 5509

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 123
executePuzzle "Part 2 finale" (fun () -> part2 data) 4407
