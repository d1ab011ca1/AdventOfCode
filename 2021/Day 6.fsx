open System
open System.IO

let realInputText = 
    let inputPath = Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")
    File.ReadAllText(inputPath)

let sampleInputText = """
3,4,3,1,2
"""

let inputs =
    let inputText = realInputText
    //let inputText = sampleInputText

    let lines =
        inputText.Split('\n', StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)

    lines
    |> Array.collect(fun s -> s.Split(',') |> Array.map int)
//printfn "%A" inputs

let part1 () =
    let fish =
        [1..80] |> Seq.fold (fun fish _day -> 
            seq {
                let mutable newFish = 0
                for f in fish do
                    if f = 0 then newFish <- newFish + 1; 6
                    else f - 1
                for _ = 1 to newFish do 8
            }
        ) (inputs |> Seq.ofArray)

    printfn "Part 1: %d" (fish |> Seq.length)

let printFishCount day fish =
    printfn "Day %d: %d" day (fish |> Seq.length)
    fish

let printFish day fish =
    printf "Day %d: " day
    fish |> Seq.iter (printf "%d ")
    printfn "(%d)" (fish |> Seq.length)
    fish

let part2 () =
    let lookup = Collections.Hashtable()
    let rec numZeroes daysRemaining n =
        if daysRemaining <= 0 then 
            0L
        else
            let key = daysRemaining * 10 + n
            match lookup.ContainsKey(key) with
            | true -> lookup.[key] :?> int64
            | _ ->
                let x =
                    if n = 0 then
                        1L + 
                        (0 |> numZeroes (daysRemaining - 7)) + 
                        (0 |> numZeroes (daysRemaining - 9))
                    else
                        0 |> numZeroes (daysRemaining - n)
                lookup.[key] <- x
                x

    let total = 
        (inputs |> Seq.length |> int64) + 
        (inputs |> Seq.sumBy (numZeroes 256))

    printfn "Part 2: %d" total

part1 () //352872
part2 () //1604361182149
