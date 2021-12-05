open System
open System.IO

let realInputText = 
    let inputPath = Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")
    File.ReadAllText(inputPath)

let sampleInputText = """
...
"""

let inputs =
    //let inputText = realInputText
    let inputText = sampleInputText

    let lines =
        inputText.Split('\n', StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)

    lines
//printfn "%A" inputs

let part1 () =
    printfn "Part 1: "

let part2 () =
    printfn "Part 2: "

part1 () //
part2 () //
