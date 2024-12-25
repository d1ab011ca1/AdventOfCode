// https://adventofcode.com/2024/day/23
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

#nowarn "57" // Experimental library feature (Array.Parallel), requires '--langversion:preview'.

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = (string * string)[]

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Array.map (fun s ->
        let p = s |> String.split "-"
        p[0], p[1])
//|> dump

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
kh-tc
qp-kh
de-cg
ka-co
yn-aq
qp-ub
cg-tb
vc-aq
tb-ka
wh-tc
yn-cg
kh-ub
ta-co
de-co
tc-td
tb-wq
wh-td
ta-ka
td-qp
aq-cg
wq-ub
ub-vc
de-ta
wq-aq
wq-vc
wh-yn
ka-de
kh-ta
co-tc
wh-qp
tb-vc
td-yn
"""

let sample2 = sample1

let data =
    let rawData = getInput ()
    lazy (rawData |> parseData)

let (|Graph|) (input: InputData) =
    input
    |> Array.fold
        (fun g (a, b) ->
            g
            |> Map.change a (Option.defaultValue Array.empty >> Array.insertAt 0 b >> Some)
            |> Map.change b (Option.defaultValue Array.empty >> Array.insertAt 0 a >> Some))
        Map.empty

let part1 (Graph graph) =
    // find all triplets where one of the computer names starts with 't'.
    [ for a in graph.Keys |> Seq.where (String.startsWith "t") do
          for b in graph[a] do
              for c in graph[b] do
                  if graph[c] |> Array.contains a then
                      yield [| a; b; c |] ]
    |> Seq.distinctBy Array.sort
    // |> dump
    |> Seq.length

let writeGraphviz name (Graph graph as input: InputData) =
    // find the largest subgraph where all computers in the subgraph
    // are connected to each other.
    let edges =
        // input |> Seq.map (fun (a, b) -> $$"""{{a}} -- {{b}};""")
        graph
        |> Map.toSeq
        |> Seq.map (fun (a, bs) -> $$"""{{a}} -- { {{bs |> String.join " "}} };""")

    IO.File.WriteAllText(
        IO.Path.ChangeExtension(scriptPath, $"{name}.gv"),
        $$"""
graph G {
    fontname="Helvetica,Arial,sans-serif"
	node [fontname="Helvetica,Arial,sans-serif"; ]
	edge [fontname="Helvetica,Arial,sans-serif"; ]
	layout=fdp
    concentrate=true
    {{edges |> String.join "\n    "}}
}
"""
    )

let part2 (Graph graph: InputData) =
    // find the largest subgraph where all computers in the subgraph
    // are connected to each other.
    // Note that the nodes may also be connected to other subgraphs.
    ""

writeGraphviz "sample" sample1
writeGraphviz "" data.Value

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 7
executePuzzle "Part 1 finale" (fun () -> part1 data.Value) 1046

executePuzzle "Part 2 sample" (fun () -> part2 sample1) "co,de,ka,ta"
executePuzzle "Part 2 finale" (fun () -> part2 data.Value) ""
