// https://adventofcode.com/2022
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#r "nuget: MathNet.Numerics.FSharp" // https://numerics.mathdotnet.com/api/
#r "nuget: FSharp.Collections.ParallelSeq"
#endif

open System
open FSharpHelpers
open System.Collections.Generic
// open MathNet.Numerics
// open FSharp.Collections.ParallelSeq

let sampleInputText1 =
    """
$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k
"""

let cookie = IO.File.ReadAllText("cookie.txt")
let inputText = downloadInput cookie
// let inputText = sampleInputText1

type Op =
    | CD of string
    | LS
    | Dir of string
    | File of string * int64

    static member parse(s: string) =
        match s with
        | "$ ls" -> LS
        | _ when s.StartsWith("$ cd ") -> CD(s.Substring(5))
        | _ when s.StartsWith("dir ") -> Dir(s.Substring(4))
        | _ ->
            let p = s.Split(' ', 2)
            File(p[1], int64 p[0])

let parseInput (text: string) =
    text |> parseInputText |> Array.map Op.parse

let inputs = inputText |> parseInput
//printfn "%A" inputs

type Tree =
    | File of string * int64
    | Subdir of string * ResizeArray<Tree>

    static member print(dir: ResizeArray<Tree>) =
        let rec print1 pad (dir: ResizeArray<Tree>) idx =
            match idx < dir.Count with
            | false -> ()
            | true ->
                match dir[idx] with
                | File(name, size) -> printfn "%s- %s (file, size=%d)" pad name size
                | Subdir(name, subdir) ->
                    printfn "%s- %s (dir)" pad name
                    print1 (pad + "  ") subdir 0

                print1 pad dir (idx + 1)

        print1 "" dir 0

let build (inputs: Op array) =

    let addSubDir subname (curDir: ResizeArray<Tree>) =
        curDir
        |> Seq.tryPick (function
            | Subdir(name, subdir) when name = subname -> Some subdir
            | _ -> None)
        |> Option.defaultWith (fun _ ->
            let subdir = ResizeArray<_>()
            curDir.Add(Subdir(subname, subdir))
            subdir)

    let addFile filename filesize (curDir: ResizeArray<Tree>) =
        curDir
        |> Seq.tryPick (function
            | File(name, _) when name = filename -> Some()
            | _ -> None)
        |> Option.defaultWith (fun _ -> curDir.Add(File(filename, filesize)))

    let rec buildTree idx (curDir: ResizeArray<Tree>) : int =
        match idx >= inputs.Length with
        | true -> idx // done
        | _ ->
            match inputs[idx] with
            | CD ".." -> idx + 1
            | CD dir ->
                let subdir = curDir |> addSubDir dir
                let nextIdx = buildTree (idx + 1) subdir
                buildTree nextIdx curDir
            | LS -> buildTree (idx + 1) curDir
            | Op.Dir subname ->
                curDir |> addSubDir subname |> ignore
                buildTree (idx + 1) curDir
            | Op.File(filename, filesize) ->
                curDir |> addFile filename filesize
                buildTree (idx + 1) curDir

    let allFolders = Dictionary<_, _>()

    let rec calcSize (sum: int64) path (dir: ResizeArray<Tree>) idx =
        match idx >= dir.Count with
        | true ->
            // done
            if path <> "" then allFolders[path] <- sum
            sum
        | _ ->
            let size =
                match dir[idx] with
                | File(_, s) -> s
                | Subdir(subname, subdir) ->
                    let path = 
                        match path with
                        | "" -> subname 
                        | "/" -> $"/{subname}"
                        | _ -> $"{path}/{subname}" 
                    calcSize 0L path subdir 0

            calcSize (sum + size) path dir (idx + 1)

    let tree = ResizeArray()
    buildTree 0 tree |> ignore
    calcSize 0L "" tree 0 |> ignore

    tree, allFolders

let tree, allFolders = build inputs
// tree |> Tree.print
// allFolders |> Seq.iter (fun p -> printfn "%10d %s" p.Value p.Key)

let part1 () =
    let answer =
        allFolders.Values |> Seq.where (fun n -> n < 100000L) |> Seq.fold (+) 0L

    printfn "Part 1: %A" answer

let part2 () =
    let maxSize = 40000000L
    let curSize = allFolders["/"]
    let spaceNeeded = curSize - maxSize
    let folder =
        allFolders
        |> Seq.where (fun p -> p.Value >= spaceNeeded)
        |> Seq.sortBy (fun p -> p.Value)
        |> Seq.head

    printfn "Part 2: %d" folder.Value

part1 () // 1778099
part2 () // 1623571
