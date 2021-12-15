open System
open System.IO

let digitToInt (c: char) =
    match int c with
    | n when (int '0') <= n && n <= (int '9') -> n - (int '0')
    | _ -> failwithf "Invalid decimal digit: %A" c

let hexDigitToInt (c: char) =
    match int c with
    | n when (int '0') <= n && n <= (int '9') -> n - (int '0')
    | n when (int 'A') <= n && n <= (int 'F') -> n - (int 'A') + 10
    | n when (int 'a') <= n && n <= (int 'f') -> n - (int 'a') + 10
    | _ -> failwithf "Invalid hex digit: %A" c

let (|DecChar|) = digitToInt
let (|HexChar|) = hexDigitToInt

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
