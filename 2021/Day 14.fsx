open System
open System.IO
open System.Collections.Generic

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
NNCB

CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C
"""

let (template, rules) =
    let inputText = realInputText
    let inputText = sampleInputText

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines |> Seq.head,
    lines
    |> Seq.skip 1
    |> Seq.map (fun s ->
        s.Split(" -> ")
        |> Seq.pairwise
        |> Seq.exactlyOne)
    |> Map.ofSeq
//printfn "%A" (template, rules)

let rec xform n (rules: Map<string, string>) (cs: char seq) =
    let cs =
        seq {
            yield cs |> Seq.head

            yield!
                cs
                |> Seq.pairwise
                |> Seq.collect (fun (c1, c2) ->
                    match rules |> Map.tryFind $"{c1}{c2}" with
                    | Some r -> (r.ToCharArray() |> List.ofArray) @ [ c2 ]
                    | _ -> [ c2 ])
        }

    if n > 1 then
        cs |> xform (n - 1) rules
    else
        cs

let (|KeyValue|) (kv: KeyValuePair<_, _>) = (kv.Key, kv.Value)
let (|Key|) (kv: KeyValuePair<_, _>) = kv.Key
let (|Value|) (kv: KeyValuePair<_, _>) = kv.Value

let getFreq (cs: char seq) =
    cs
    |> Seq.groupBy id
    |> Seq.map (fun (c, cs) -> c, cs |> Seq.length)
    |> List.ofSeq

let part1 () =
    let result = template |> xform 10 rules

    let freq =
        result
        |> getFreq
        |> Seq.map snd

    printfn "Part 1: %A" ((freq |> Seq.max) - (freq |> Seq.min))

let part2 () =
    // let rules =
    //     rules |> Map.map (fun k v -> $"{k.[0]}{v}{k.[1]}")

    // let substituteValue map (termKey:string) (termValue:string) =
    //     map
    //     |> Map.map (fun _ (v: string) -> v.Replace(termKey, termValue))

    // let rec bar (rules: Map<string, string>) =
    //     let looping, terminal =
    //         rules |> Map.partition (fun k v -> v.Contains k)

    //     if terminal.IsEmpty then
    //         looping
    //     else
    //         terminal
    //         |> Map.fold substituteValue looping
    //         |> bar

    // let foo =
    //     bar rules
    // let foo =
    //     foo |> Map.fold substituteValue rules

    // let countOccurances (s: string) (sub: string) =
    //     let rec loop n idx =
    //         match s.IndexOf(sub, startIndex = idx) with
    //         | -1 -> n
    //         | next -> loop (n + 1) (next + 1)
    //     loop 0 0

    // printfn "Part 2:"
    // for KeyValue (k,v) in rules do
    //     printfn "  %s -> %s (%d)" k v (countOccurances v k)

    // let result = template |> xform 10 foo

    // let freq =
    //     result
    //     |> Seq.groupBy id
    //     |> Seq.map (fun (_, cs) -> cs |> Seq.length)
    //     |> Array.ofSeq

    // printfn "Part 2: %A" ((freq |> Seq.max) - (freq |> Seq.min))

    for r in rules |> Seq.map (|Key|) do
        for n = 1 to 6 do
            r
            |> xform n rules
            |> String.Concat //getFreq
            |> printfn "%d: %s: %A" n r

part1 () // 2590
part2 () //

//   BB -> BNB (0)
//   BC -> BBC (1)
//   BH -> BHH (1)
//   BN -> BBN (1)
//   CB -> CHB (0)
//   CC -> CNC (0)
//   CH -> CBH (0)
//   CN -> CCN (1)
//   HB -> HCB (0)
//   HC -> HBC (0)
//   HH -> HNH (0)
//   HN -> HCN (0)
//   NB -> NBB (1)
//   NC -> NBC (0)
//   NH -> NCH (0)
//   NN -> NCN (0)

//   BB -> BNB
//   BC -> BBC* -> BNBBC*
//   BH -> BH* HH
//   BN -> BB  BN*
//   CB -> CH  HB
//   CC -> CN  NC
//   CH -> CB  BH
//   CN -> CC  CN*
//   HB -> HC  CB
//   HC -> HB  BC
//   HH -> HN  NH
//   HN -> HC  CN
//   NB -> NB* BB
//   NC -> NB  BC
//   NH -> NC  CH
//   NN -> NC  CN
