open System
open System.IO
open System.Collections.Generic

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
[({(<(())[]>[[{[]{<()<>>
[(()[<>])]({[<{<<[]>>(
{([(<{}[<>[]}>{[]{[(<()>
(((({<>}<{<{<>}{[]{[]{}
[[<[([]))<([[{}[[()]]]
[{[{({}]{}}([{[{{{}}([]
{<[[]]>}<{[{[{[]{()[[[]
[<(<(<(<{}))><([]([]()
<{([([[(<>()){}]>(<<{{
<{([{{}}[<[[[<>{}]]]>[]]
"""

let inputs =
    let inputText = realInputText
    // let inputText = sampleInputText

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines
//printfn "%A" inputs

let maybeCorrupted s =
    let stack = Stack()

    seq {
        for c in s do
            if c = '{' || c = '(' || c = '[' || c = '<' then
                stack.Push c
            else
                match stack.Pop(), c with
                | '(', ')'
                | '[', ']'
                | '{', '}'
                | '<', '>' -> ()
                | _ -> c
    }
    |> Seq.tryHead

let part1 () =
    let score =
        inputs
        |> Seq.choose maybeCorrupted
        |> Seq.sumBy (function
            | ')' -> 3L
            | ']' -> 57L
            | '}' -> 1197L
            | '>' -> 25137L
            | _ -> failwith "unexpected")

    printfn "Part 1: %d" score

let part2 () =
    let endings =
        seq {
            for s in
                inputs
                |> Seq.where (maybeCorrupted >> Option.isNone) do
                let stack = Stack()

                for c in s do
                    match c with
                    | '{' -> stack.Push '}'
                    | '(' -> stack.Push ')'
                    | '[' -> stack.Push ']'
                    | '<' -> stack.Push '>'
                    | _ -> stack.Pop() |> ignore

                stack |> String.Concat
        }

    let score ending =
        ending
        |> Seq.fold
            (fun total c ->
                match c with
                | ')' -> total * 5L + 1L
                | ']' -> total * 5L + 2L
                | '}' -> total * 5L + 3L
                | '>' -> total * 5L + 4L
                | _ -> failwith "unexpected")
            0L

    let scores = endings |> Seq.map score |> List.ofSeq

    let middleScore =
        scores
        |> Seq.sort
        |> Seq.skip (scores.Length / 2)
        |> Seq.head

    printfn "Part 2: %d" middleScore

part1 () //358737
part2 () //4329504793
