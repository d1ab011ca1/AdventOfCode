// https://adventofcode.com/2023/day/23
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = Grid<char>

[<Literal>]
let Dot = '.'

[<Literal>]
let Forest = '#'

[<Literal>]
let North = '^'

[<Literal>]
let East = '>'

[<Literal>]
let South = 'v'

[<Literal>]
let West = '<'

[<Literal>]
let Visited = 'O'


type Direction =
    | North = 0
    | South = 1
    | East = 2
    | West = 3

let opposite =
    function
    | Direction.North -> Direction.South
    | Direction.South -> Direction.North
    | Direction.East -> Direction.West
    | Direction.West -> Direction.East
    | _ -> failwith "opposite"

[<return: Struct>]
let (|Slope|_|) =
    function
    | North -> ValueSome Direction.North
    | South -> ValueSome Direction.South
    | East -> ValueSome Direction.East
    | West -> ValueSome Direction.West
    | _ -> ValueNone

let (|Delta|) =
    function
    | Direction.North -> (0, -1)
    | Direction.South -> (0, +1)
    | Direction.East -> (+1, 0)
    | Direction.West -> (-1, 0)
    | _ -> failwith "Delta"

let parseInput (text: string) : InputData =
    text |> String.splitAndTrim "\n" |> Grid.fromLines //|> tee (printfn "%A")

let validateAssumptions (g: InputData) =
    let (w, h) = g |> Grid.widthAndHeight
    let (startX, startY) = (1, 0)
    let (endX, endY) = (w - 2, h - 1)

    if g |> Grid.item startX startY <> Dot then
        failwith "bad assumption"

    if g |> Grid.item endX endY <> Dot then
        failwith "bad assumption"

    // Grid only contains East and South slopes
    if g |> Grid.tryFind West <> None then
        failwith "bad assumption"

    if g |> Grid.tryFind North <> None then
        failwith "bad assumption"

    // All slopes are followed by a Dot
    g
    |> Grid.iter (fun (x, y) ->
        function
        | South ->
            if g |> Grid.item x (y + 1) <> Dot then
                failwith "bad assumption"
        | East ->
            if g |> Grid.item (x + 1) y <> Dot then
                failwith "bad assumption"
        | _ -> ())


let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
#.#####################
#.......#########...###
#######.#########.#.###
###.....#.>.>.###.#.###
###v#####.#v#.###.#.###
###.>...#.#.#.....#...#
###v###.#.#.#########.#
###...#.#.#.......#...#
#####.#.#.#######.#.###
#.....#.#.#.......#...#
#.#####.#.#.#########v#
#.#...#...#...###...>.#
#.#.#v#######v###.###v#
#...#.>.#...>.>.#.###.#
#####v#.#.###v#.#.###.#
#.....#...#...#.#.#...#
#.#########.###.#.#.###
#...###...#...#...#.###
###.###.#.###v#####v###
#...#...#.#.>.>.#.>.###
#.###.###.#.###.#.#v###
#.....###...###...#...#
#####################.#
"""

let sample2 = sample1

let data = getInput () |> parseData

type Vertex =
    { Id: VId
      Edges: Edge list }

    static member create vid = { Id = vid; Edges = List.empty }

    static member addEdge otherVId weight vertex =
        { vertex with Edges = { To = otherVId; Weight = weight } :: vertex.Edges }

and Edge = { To: VId; Weight: int }
and VId = Grid.Coordinates

let makeGraph startX startY grid =
    let visited = grid |> Grid.clone
    let queue = Collections.Generic.Queue()

    let allDirections =
        [| Direction.East; Direction.South; Direction.West; Direction.North |]

    let rec loop graph =
        if queue.Count = 0 then
            graph |> List.toArray
        else
            let mutable vertex = queue.Dequeue()
            let (x, y) = vertex.Id

            let nextDirs =
                match grid |> Grid.item x y with
                | Dot -> allDirections
                | Slope newDir -> newDir |> Array.singleton
                | _ -> failwith "nextDirs"

            nextDirs
            |> Array.iter (fun nextDir ->
                match canMove x y nextDir with
                | ValueSome(x2, y2) ->
                    vertex <- vertex |> Vertex.addEdge (x2, y2) 1

                    if visited |> Grid.item x2 y2 <> Visited then
                        visited |> Grid.set x2 y2 Visited
                        queue.Enqueue(Vertex.create (x2, y2))
                | _ -> ())

            loop (graph @ [ vertex ])

    and canMove x y (Delta(dx, dy) as nextDir) =
        match grid |> Grid.itemOrDefault (x + dx) (y + dy) Forest with
        | Forest -> ValueNone
        | Dot -> ValueSome(x + dx, y + dy)
        | Slope d when d <> opposite nextDir -> ValueSome(x + dx, y + dy)
        | _ -> ValueNone

    visited |> Grid.set startX startY Visited
    queue.Enqueue(Vertex.create (startX, startY))
    loop List.empty

/// Sorts the vertices of a Directed Acyclic Graph (DAG) into an
/// array of vertices such that for every directed edge u->v,
/// vertex u comes before v in the ordering.
let topologicalSort (graph: Vertex[]) =
    let stack = Collections.Generic.Stack(graph.Length)
    let visited = Collections.Generic.HashSet(graph.Length)

    let getVertex =
        if graph.Length <= 100 then
            fun id -> graph |> Array.find (fun v -> v.Id = id)
        else
            let lookup = graph |> Array.fold (fun m v -> m |> Map.add v.Id v) Map.empty
            fun id -> lookup |> Map.find id

    let rec loop v =
        if visited.Add(v.Id) then
            for e in v.Edges do
                e.To |> getVertex |> loop

            stack.Push(v)

    graph |> Array.iter loop
    stack |> Seq.toArray

let printGraph grid graph =
    let (w, h) = grid |> Grid.widthAndHeight
    let g2 = Grid.create w h Forest

    graph
    |> Array.iteri (fun i v ->
        let (x0, y0) = v.Id

        for e in v.Edges do
            let (x1, y1) = e.To
            let c = Dot
            // let c = (char (int '0' + (e.Weight % 10)))
            // let c = (char (int '0' + (i % 10)))

            for y = min y0 y1 to max y0 y1 do
                for x = min x0 x1 to max x0 x1 do
                    g2 |> Grid.set x y c)

    (grid, g2)
    ||> Seq.zip
    |> Seq.iter (fun (r, r2) -> printfn "%s  %s" (String.ofArray r) (String.ofArray r2))

let solve (grid: InputData) =
    let (w, h) = grid |> Grid.widthAndHeight
    let (startX, startY) = (1, 0)
    let (endX, endY) = (w - 2, h - 1)

    let graph =
        grid
        |> makeGraph startX startY
        // |> tee (Array.iter (printfn "%A"))
        |> topologicalSort

    // graph |> printGraph grid
    if graph[0].Id <> (startX, startY) then
        failwith "start vert"

    let maxDist =
        let dict = Collections.Generic.Dictionary(graph.Length)
        graph |> Array.iter (fun v -> dict[v.Id] <- Int32.MinValue)
        dict[(startX, startY)] <- 0
        dict

    let rec loop idx =
        if idx < graph.Length then
            // Update distances of all adjacent vertices
            let v = graph[idx]
            let distV = maxDist[v.Id]

            if distV <> Int32.MinValue then
                for e in v.Edges do
                    if maxDist[e.To] < distV + e.Weight then
                        maxDist[e.To] <- distV + e.Weight

            loop (idx + 1)

    loop 0
    // dist |> Seq.iter (printfn "%A")
    maxDist[(endX, endY)]

let part1 (g: InputData) = solve g

let part2 (g: InputData) =
    // change all slopes to dots...
    g |> Grid.map (fun _ c -> if c = Forest then Forest else Dot) |> solve

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 94
executePuzzle "Part 1 finale" (fun () -> part1 data) 2130

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 154
executePuzzle "Part 2 finale" (fun () -> part2 data) 0 // 5018 < n
