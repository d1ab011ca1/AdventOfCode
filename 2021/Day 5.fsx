open System
open System.IO
open System.Text.RegularExpressions
open System.Text

let realInputText = 
    let inputPath = Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")
    File.ReadAllText(inputPath)

let sampleInputText = """
0,9 -> 5,9
8,0 -> 0,8
9,4 -> 3,4
2,2 -> 2,1
7,0 -> 7,4
6,4 -> 2,0
0,9 -> 2,9
3,4 -> 1,4
0,0 -> 8,8
5,5 -> 8,2
"""

type Pt =
    { x:int
      y:int }
type Line = 
    { p1:Pt
      p2:Pt }

let inputs =
    let inputText = realInputText
    //let inputText = sampleInputText

    let lines =
        inputText.Split('\n', StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)

    let re = Regex("""(\d+),(\d+) -> (\d+),(\d+)""", RegexOptions.Compiled)
    lines
    |> Array.map(fun s -> 
        match re.Match(s) with
        | m when m.Success ->
            { p1 = { x = int <| m.Groups.[1].Value; y = int <| m.Groups.[2].Value }
              p2 = { x = int <| m.Groups.[3].Value; y = int <| m.Groups.[4].Value } }
        | _ -> failwith s)
//printfn "%A" inputs.[0]

let max = 
    Array.fold (fun m -> function
    | {Line.p1={Pt.x=x}} | {Line.p2={Pt.x=x}} when x > m.x -> { m with x = x }
    | {Line.p1={Pt.y=y}} | {Line.p2={Pt.y=y}} when y > m.y -> { m with y = y }
    | _ -> m) {x=Int32.MinValue; y=Int32.MinValue} inputs
let min = 
    Array.fold (fun m -> function
    | {Line.p1={Pt.x=x}} | {Line.p2={Pt.x=x}} when x < m.x -> { m with x = x }
    | {Line.p1={Pt.y=y}} | {Line.p2={Pt.y=y}} when y < m.y -> { m with y = y }
    | _ -> m) max inputs
//printfn "min=%A max=%A" min max

type Map = int[][]
let createMap () : Map =
    Array.init (max.x + 1) (fun _ -> Array.zeroCreate (max.y + 1))

let printMap (map: Map) =
    let sb = StringBuilder((max.x - min.x + 1) * (max.y - min.y + 1))
    for y = min.y to max.y do
        for x = min.x to max.x do
            (if map.[x].[y] = 0 then "." else map.[x].[y] |> string) 
            |> sb.Append |> ignore
        sb.AppendLine() |> ignore
    printf $"{sb.ToString()}"

let plotPt (map: Map) (p: Pt) =
    map.[p.x].[p.y] <- map.[p.x].[p.y] + 1

let scoreMap (map: Map) =
    map
    |> Array.sumBy (
        Array.sumBy (fun n -> if n > 1 then 1 else 0))

type Line with 
    static member isStraight line =
        line.p1.x = line.p2.x || 
        line.p1.y = line.p2.y
    static member isDiagonal line =
        Math.Abs(line.p2.x - line.p1.x) = Math.Abs(line.p2.y - line.p1.y)
    static member points line =
        let dx = if line.p1.x > line.p2.x then -1 else +1
        let dy = if line.p1.y > line.p2.y then -1 else +1
        [
            if Line.isStraight line then
                for x in line.p1.x .. dx .. line.p2.x do
                    for y in line.p1.y .. dy .. line.p2.y do
                        yield { x = x; y = y }
            elif Line.isDiagonal line then
                let mutable p = line.p1
                for _ = 0 to Math.Abs(line.p2.x - line.p1.x) do
                    yield p
                    p <- { p with x = p.x + dx; y = p.y + dy;}
        ]

let part1 () =
    let map = createMap()

    inputs
    |> Array.where Line.isStraight
    |> Seq.collect Line.points
    |> Seq.iter (plotPt map)
    //printMap map

    let answer = scoreMap map
    printfn "Part 1: %A" answer

let part2 () =
    let map = createMap()

    inputs
    |> Array.where (fun l -> Line.isStraight l || Line.isDiagonal l)  
    |> Seq.collect Line.points
    |> Seq.iter (plotPt map)
    //printMap map

    let answer = scoreMap map
    printfn "Part 2: %A" answer

part1 () //6841
part2 () //19258
