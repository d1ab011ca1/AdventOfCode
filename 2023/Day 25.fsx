// https://adventofcode.com/2023/day/X
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = (string * string[])[]

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Array.map (fun s ->
        let ps = s |> String.split ": "
        (ps[0], ps[1] |> String.split " "))
// |> tee (printfn "%A")

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
jqt: rhn xhk nvd
rsh: frs pzl lsr
xhk: hfx
cmg: qnr nvd lhk bvb
rhn: xhk bvb hfx
bvb: xhk hfx
pzl: lsr hfx nvd
qnr: nvd
ntq: jqt hfx bvb xhk
nvd: lhk
lsr: lhk
rzs: qnr cmg lsr rsh
frs: qnr lhk lsr
"""

let sample2 = sample1

let data = getInput () |> parseData

[<StructuredFormatDisplay("{a}-{b}")>]
[<CustomComparison; CustomEquality>]
type Edge =
    { a: string
      b: string }

    override this.ToString() = $"{this.a}-{this.b}"

    interface IComparable<Edge> with
        /// Compare two edges ignoring the order of the vertices.
        member this.CompareTo(other: Edge) =
            let inline compOther thisA thisB =
                if compare other.a other.b <= 0 then
                    let d = compare thisA other.a
                    if d = 0 then compare thisB other.b else d
                else // swap other
                    let d = compare thisA other.b
                    if d = 0 then compare thisB other.a else d

            if compare this.a this.b <= 0 then
                compOther this.a this.b
            else // swap this
                compOther this.b this.a

    interface IComparable with
        member this.CompareTo(other) =
            match other with
            | null -> 1
            | _ -> (this :> IComparable<Edge>).CompareTo(other :?> Edge)

    interface IEquatable<Edge> with
        member this.Equals(other: Edge) =
            (this :> IComparable<Edge>).CompareTo(other) = 0

    override this.Equals(other) =
        match other with
        | :? Edge as other -> (this :> IEquatable<Edge>).Equals(other)
        | _ -> false

    override this.GetHashCode() =
        if compare this.a this.b <= 0 then
            HashCode.Combine(this.a, this.b)
        else // swap this
            HashCode.Combine(this.b, this.a)

let part1 (data: InputData) connectingEdges =
    let edges =
        data
        |> Seq.collect (fun (s, ds) -> ds |> Seq.map (fun d -> { a = s; b = d }))
        |> Set.ofSeq

    // edges |> Seq.iter (fun e -> printfn "%s -- %s" e.a e.b)
    // printfn ""

    // Using graphviz, _manually_ find and remove the three edges...
    let edges =
        connectingEdges
        |> Seq.fold (fun edges (a, b) -> edges |> Set.remove { a = a; b = b }) edges

    // edges |> Seq.iter (fun e -> printfn "%s -- %s" e.a e.b)
    // printfn ""

    // partition the edges...
    let lookup =
        edges
        |> Seq.fold
            (fun m e ->
                m
                |> Map.change e.a (function
                    | None -> Some([ e.b ])
                    | Some c -> Some(c @ [ e.b ]))
                |> Map.change e.b (function
                    | None -> Some([ e.a ])
                    | Some c -> Some(c @ [ e.a ])))
            Map.empty
    // |> echo

    let rec partitionNodes nodes set1 =
        match nodes with
        | [] -> set1
        | head :: tail ->
            if set1 |> Set.contains head then
                partitionNodes tail set1
            else
                let set1 = set1 |> Set.add head
                partitionNodes (tail @ lookup[head]) set1

    let nodes = lookup |> Map.keys |> Set.ofSeq
    let set1 = partitionNodes [ nodes |> Seq.head ] Set.empty
    let set2 = Set.difference nodes set1
    (set1 |> Set.count) * (set2 |> Set.count)

let part2 (data: InputData) =
    //
    0

executePuzzle "Part 1 sample" (fun () -> part1 sample1 [ ("jqt", "nvd"); ("bvb", "cmg"); ("hfx", "pzl") ]) 54
executePuzzle "Part 1 finale" (fun () -> part1 data [ ("xtx", "njn"); ("tmb", "gpj"); ("mtc", "rhh") ]) 558376

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 0
executePuzzle "Part 2 finale" (fun () -> part2 data) 0
