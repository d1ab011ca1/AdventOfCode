open System
open System.IO
open System.Collections.Generic

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText1 =
    """
acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf
"""

let sampleInputText2 =
    """
be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce
"""

let sortStringChars (s: string) = s.ToCharArray() |> Array.sort |> String
let (|KeyValue|) (pair: KeyValuePair<_, _>) = (pair.Key, pair.Value)
let (|Key|) (pair: KeyValuePair<_, _>) = pair.Key
let (|Value|) (pair: KeyValuePair<_, _>) = pair.Value

module Map =
    let lookup table key = Map.find key table

let inputs =
    let inputText = realInputText
    // let inputText = sampleInputText1
    // let inputText = sampleInputText2

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines
    |> Array.map (fun l ->
        l.Split(" | ", 2)
        |> Array.map (fun s -> s.Split(' ') |> Array.map sortStringChars)
        |> Array.pairwise
        |> Array.head)
//printfn "%A" (inputs.[0].[1])

let part1 () =
    let f =
        inputs
        |> Seq.collect snd
        |> Seq.where (fun x ->
            match x.Length with
            | 2
            | 4
            | 3
            | 7 -> true
            | _ -> false)
        |> Seq.length

    printfn "Part 1: %d" f

let part2 () =
    let decode (signals: string []) =
        let freqMap =
            signals
            |> Seq.collect (fun s -> s.ToCharArray())
            |> Seq.groupBy id
            |> Seq.map (fun (c, cs) -> cs |> Seq.length, c)
            |> Seq.distinctBy fst
            |> Map

        let b = freqMap.[6]
        let e = freqMap.[4]
        let f = freqMap.[9]

        let sortedSignals = signals |> Array.sortBy Seq.length
        let n1 = sortedSignals.[0]
        let n7 = sortedSignals.[1]
        let n4 = sortedSignals.[2]
        let n8 = sortedSignals.[9]
        let a = n7 |> Seq.except n1 |> Seq.head
        let c = n1 |> Seq.except [ f ] |> Seq.head
        let d = n4 |> Seq.except [ b; c; f ] |> Seq.head

        let g =
            n8 |> Seq.except [ a; b; c; d; e; f ] |> Seq.head
        //printfn "segments: %A" (String.Concat([a;b;c;d;e;f;g;]))

        let segmentMap =
            Map [ 'a', a
                  'b', b
                  'c', c
                  'd', d
                  'e', e
                  'f', f
                  'g', g ]

        let digits =
            [ '0', "abcefg"
              '1', "cf"
              '2', "acdeg"
              '3', "acdfg"
              '4', "bcdf"
              '5', "abdfg"
              '6', "abdefg"
              '7', "acf"
              '8', "abcdefg"
              '9', "abcdfg" ]

        digits
        |> Seq.map (fun (digit, segments) ->
            segments
            |> Seq.map (Map.lookup segmentMap)
            |> String.Concat
            |> sortStringChars,
            digit)
        |> Map.ofSeq

    let solve (signals: string [], outputs: string []) =
        let map = decode signals
        let digits = outputs |> Seq.map (Map.lookup map)
        digits |> String.Concat |> int

    printfn "Part 2: %d" (inputs |> Seq.map solve |> Seq.sum)

part1 () //342
part2 () //1068933
