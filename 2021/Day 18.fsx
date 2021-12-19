open System
open System.IO
open System.Text.RegularExpressions
open System.Text

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
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]
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

// printfn "%A" inputs

type Node =
    | Value of int
    | Tree of Node * Node

module Node =
    let parse s =
        let rec parse (s: string) idx =
            match s.[idx] with
            | '[' ->
                let left, idx = parse s (idx + 1)
                let right, idx = parse s (idx + 1)
                Tree(left, right), idx + 1
            | DecChar (n) -> Value n, idx + 1

        parse s 0 |> fst

    let stringize n =
        let rec stringize (sb: StringBuilder) =
            function
            | Tree (l, r) ->
                sb.Append('[') |> ignore
                stringize sb l
                sb.Append(',') |> ignore
                stringize sb r
                sb.Append(']') |> ignore
            | Value n -> sb.Append(n) |> ignore

        let sb = StringBuilder()
        stringize sb n
        sb.ToString()

    let rec addLeft i =
        function
        | Value n -> Value(n + i)
        | Tree (l, r) -> Tree(addLeft i l, r)

    let rec addRight i =
        function
        | Value n -> Value(n + i)
        | Tree (l, r) -> Tree(l, addRight i r)

    let tryExplode n =
        let rec tryExplodeR depth n =
            match n with
            | Value _ -> None, None, None
            | Tree (l, r) ->
                if depth = 4 then
                    match l, r with
                    | Value addl, Value addr -> Some(Value 0), Some addl, Some addr
                    | _ -> failwith "Expected a value pair."
                else
                    match tryExplodeR (depth + 1) l with
                    | Some newl, addl, addr ->
                        // add to right but only the caller can add to left
                        let newr =
                            match addr with
                            | Some addr -> r |> addLeft addr
                            | None -> r

                        Some(Tree(newl, newr)), addl, None
                    | None, _, _ ->
                        match tryExplodeR (depth + 1) r with
                        | Some newr, addl, addr ->
                            // add to left but only the caller can add to right
                            let newl =
                                match addl with
                                | Some addl -> l |> addRight addl
                                | None -> l

                            Some(Tree(newl, newr)), None, addr
                        | _, _, _ -> None, None, None

        tryExplodeR 0 n |> fun (a, _, _) -> a

    let rec trySplit n =
        match n with
        | Value i when i < 10 -> None
        | Value i -> Some <| Tree(Value(i / 2), Value((i + 1) / 2))
        | Tree (l, r) ->
            match trySplit l with
            | Some newl -> Some <| Tree(newl, r)
            | _ ->
                match trySplit r with
                | Some newr -> Some <| Tree(l, newr)
                | _ -> None

    let rec reduce n =
        match n |> tryExplode with
        | Some exploded -> reduce exploded
        | None ->
            match n |> trySplit with
            | Some split -> reduce split
            | None -> n

    let rec magnitude = function
        | Value i -> i
        | Tree (l, r) -> (3 * (magnitude l)) + (2 * (magnitude r))

    let add (a: Node) (b: Node) = Tree(a, b) |> reduce

let numbers =
    inputs |> Seq.map Node.parse |> Seq.toList

let part1 () =
    let answer =
        numbers.Tail |> Seq.fold Node.add numbers.Head

    printfn "Part 1: %A" (answer |> Node.magnitude)

let part2 () =
    let answer =
        seq {
            for i = 0 to numbers.Length - 1 do
                for j = i + 1 to numbers.Length - 1 do
                    Node.add numbers.[i] numbers.[j]
                    Node.add numbers.[j] numbers.[i]
        }
        |> Seq.map Node.magnitude
        |> Seq.max

    printfn "Part 2: %A" answer

part1 () // 4088
part2 () // 4536
