open System
open System.IO

let realInputText = 
    let inputPath = Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")
    File.ReadAllText(inputPath)

let sampleInputText = """
16,1,2,0,4,2,7,1,2,14
"""

let inputs =
    let inputText = realInputText
    //let inputText = sampleInputText

    let lines =
        inputText.Split('\n', StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)

    lines
    |> Array.collect(fun s -> s.Split(',') |> Array.map int)
//printfn "%A" inputs

let min = inputs |> Seq.min
let max = inputs |> Seq.max
let avg = (inputs |> Seq.sum) / inputs.Length
printfn "min=%d, max=%d, avg=%d" min max avg

let distance (a:int) b =
    Math.Abs(a - b)

let part1 () =
    let minCost =
        [min..max]
        |> Seq.map(fun x -> (x, inputs |> Array.sumBy (distance x)))
        |> Seq.minBy snd

    printfn "Part 1: %A" minCost

let part2 () =
    let summatorial x = x * (x + 1) / 2
    let distance2 a b = distance a b |> summatorial
    let minCost =
        [min..max]
        |> Seq.map(fun x -> (x, inputs |> Array.sumBy (distance2 x)))
        |> Seq.minBy snd

    printfn "Part 2: %A" minCost

part1 () //355521
part2 () //100148777
