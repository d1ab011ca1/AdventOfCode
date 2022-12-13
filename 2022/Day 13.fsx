// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type TreeS = Tree<int>

let tokenizer (input: string) idx =
    if idx = -1 || idx = input.Length then
        Tree.EOF, -1
    else
        match input[idx] with
        | '[' -> Tree.StartBranch, idx + 1
        | ',' -> Tree.NextBranch, idx + 1
        | ']' -> Tree.EndBranch, idx + 1
        | c when Char.IsDigit(c) ->
            let mutable len = 1

            while Char.IsDigit(input[idx + len]) do
                len <- len + 1

            Tree.Leaf(Int32.Parse(input.AsSpan(idx, len))), idx + len

        | c -> failwithf "Unexpected input at %d (%c)." (idx + 1) c

let parse a = Tree.parse (tokenizer a)

let parseInput (text: string) =
    text |> parseInputText |> Array.map parse

let rec compare (t1: TreeS) (t2: TreeS) =
    match t1, t2 with
    | Value v1, Value v2 -> v1.CompareTo(v2)
    | Branch t1, Branch t2 -> Array.compareWith compare t1 t2
    | Value v1, Branch t2 -> compare (Branch [| Value v1 |]) (Branch t2)
    | Branch t1, Value v2 -> compare (Branch t1) (Branch [| Value v2 |])

let printTree t =
    let rec printt =
        function
        | Value v -> printf $"{v}"
        | Branch ts ->
            printf "["

            match ts with
            | [||] -> ()
            | ts ->
                printt ts[0]

                for i = 1 to ts.Length - 1 do
                    printf ","
                    printt ts[i]

            printf "]"

    printt t
    printfn ""

let part1 (inputs: TreeS[]) =
    // printfn "%A" inputs

    inputs
    |> Seq.chunkBySize 2
    |> Seq.mapi (fun idx pair -> if compare pair[0] pair[1] < 0 then idx + 1 else 0)
    |> Seq.fold (+) 0

let part2 (inputs: TreeS[]) =
    let t2 = "[[2]]" |> parse
    let t6 = "[[6]]" |> parse
    let inputs = inputs |> Seq.append [ t2; t6 ]

    let ordered = inputs |> Seq.sortWith compare
    // ordered |> Seq.iter printTree

    let i2 = ordered |> Seq.findIndex ((=) t2)
    let i6 = ordered |> Seq.findIndex ((=) t6)

    (i2 + 1) * (i6 + 1)

let sampleInputText1 =
    """
[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]
"""

try
    [ sampleInputText1; getInput () ]
    |> Seq.iteri (fun inputNo inputText ->
        let input = inputText |> parseInput
        printfn "Input %d Part 1: %O" inputNo (part1 input) // 5350
        printfn "Input %d Part 2: %O" inputNo (part2 input) // 19570
        printfn "")
with
| :? OperationCanceledException -> printfn "*** Cancelled ***"
| _ -> reraise ()
