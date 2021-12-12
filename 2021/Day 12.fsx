open System
open System.IO

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
start-A
start-b
A-c
A-b
b-d
A-end
b-end
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
    |> Array.map (fun s -> s.Split('-') |> Array.pairwise |> Array.exactlyOne)
    |> Array.map (fun (x, y) ->
        if y = "start" || x = "end" then
            y, x
        else
            x, y)
//printfn "%A" inputs

let (|Start|_|) =
    function
    | "start" -> Some()
    | _ -> None

let (|End|_|) =
    function
    | "end" -> Some()
    | _ -> None

let (|BigCave|_|) =
    function
    | "start"
    | "end" -> None
    | x when Char.IsUpper(x.[0]) -> Some x
    | _ -> None

let (|SmallCave|_|) =
    function
    | "start"
    | "end" -> None
    | x when Char.IsLower(x.[0]) -> Some x
    | _ -> None

let map =
    inputs
    |> Seq.fold
        (fun map (x, y) ->
            let ys =
                map
                |> Map.tryFind x
                |> Option.defaultValue []
                |> List.append [ y ]

            let xs =
                map
                |> Map.tryFind y
                |> Option.defaultValue []
                |> List.append (
                    match x with
                    | Start
                    | End -> []
                    | _ -> [ x ]
                )

            map |> Map.add x ys |> Map.add y xs)
        Map.empty
    |> Map.map (fun _ vs -> vs |> Seq.sort |> Array.ofSeq)

// printfn "%A" map

type State =
    { visitedSmallCaves: Map<string, int>
      path: string list }

let inititalState =
    { visitedSmallCaves = Map.empty
      path = List.empty }

let rec findPaths state node smallCave : string list seq =
    seq {
        let state = { state with path = node :: state.path }

        for next in map.[node] do
            match next with
            | End ->
                next :: state.path
            | BigCave next -> yield! findPaths state next smallCave
            | SmallCave next ->
                let count =
                    state.visitedSmallCaves
                    |> Map.tryFind next
                    |> Option.defaultValue 0

                if (count = 0) || (count = 1 && next = smallCave) then
                    let state =
                        { state with
                            visitedSmallCaves =
                                state.visitedSmallCaves
                                |> Map.add next (count + 1) }

                    yield! findPaths state next smallCave
            | _ -> failwith "unexpected"
    }

let part1 () =

    let solutions =
        findPaths inititalState "start" ""
        |> Seq.distinct
        //|> Seq.map (List.rev >> String.concat ",")
        //|> Seq.sortWith (fun a b -> String.Compare(a, b))
        //|> Seq.map (printfn "%s")
        |> Seq.length

    printfn "Part 1: %d" solutions

let part2 () =

    let solutions =
        map
        |> Seq.map (fun kv -> kv.Key)
        |> Seq.choose (function
            | SmallCave c -> Some c
            | _ -> None)
        |> Seq.collect (findPaths inititalState "start")
        |> Seq.distinct
        //|> Seq.map (List.rev >> String.concat ",")
        //|> Seq.sortWith (fun a b -> String.Compare(a, b))
        //|> Seq.map (printfn "%s")
        |> Seq.length

    printfn "Part 2: %A" solutions

part1 () // 3497
part2 () // 93686
