// https://adventofcode.com/2024/day/24
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

#nowarn "57" // Experimental library feature (Array.Parallel), requires '--langversion:preview'.

open System
open System.Text.RegularExpressions
open FSharpHelpers
open System.Collections.Generic

type Wire = string

type Gate =
    | OR of inA: Wire * inB: Wire * out: Wire
    | XOR of inA: Wire * inB: Wire * out: Wire
    | AND of inA: Wire * inB: Wire * out: Wire

type InputData = (Wire * bool)[] * Gate[]

let parseInput ((TextLines lines) as text) : InputData =
    let (wires, gates) = lines |> Array.split ((=) "")
    let gates = gates |> Array.skip 1

    let wires =
        wires
        |> Array.map (fun s ->
            match s |> Regex.matchGroups @"^(\S+): (0|1)$" with
            | Some gs -> (gs[1].Value, gs[2].Value = "1")
            | _ -> failwithf "Bad input: %A" s)

    let gates =
        gates
        |> Array.map (fun s ->
            match s |> Regex.matchGroups @"^(\S+) (OR|XOR|AND) (\S+) -> (\S+)$" with
            | Some gs ->
                match gs[2].Value with
                | "OR" -> OR(gs[1].Value, gs[3].Value, gs[4].Value)
                | "XOR" -> XOR(gs[1].Value, gs[3].Value, gs[4].Value)
                | "AND" -> AND(gs[1].Value, gs[3].Value, gs[4].Value)
                | _ -> failwithf "Bad input: %A" s
            | _ -> failwithf "Bad input: %A" s)

    (wires, gates) //|> dump

let validateAssumptions (data: InputData) =
    // Note: `assert` does not work in FSI, so must throw exception
    if false then
        failwith "bad assumption"

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
x00: 1
x01: 1
x02: 1
y00: 0
y01: 1
y02: 0

x00 AND y00 -> z00
x01 XOR y01 -> z01
x02 OR y02 -> z02
"""

let sample2 =
    parseData
        """
x00: 1
x01: 0
x02: 1
x03: 1
x04: 0
y00: 1
y01: 1
y02: 1
y03: 1
y04: 1

ntg XOR fgs -> mjb
y02 OR x01 -> tnw
kwq OR kpj -> z05
x00 OR x03 -> fst
tgd XOR rvg -> z01
vdt OR tnw -> bfw
bfw AND frj -> z10
ffh OR nrd -> bqk
y00 AND y03 -> djm
y03 OR y00 -> psh
bqk OR frj -> z08
tnw OR fst -> frj
gnj AND tgd -> z11
bfw XOR mjb -> z00
x03 OR x00 -> vdt
gnj AND wpb -> z02
x04 AND y00 -> kjc
djm OR pbm -> qhw
nrd AND vdt -> hwm
kjc AND fst -> rvg
y04 OR y02 -> fgs
y01 AND x02 -> pbm
ntg OR kjc -> kwq
psh XOR fgs -> tgd
qhw XOR tgd -> z09
pbm OR djm -> kpj
x03 XOR y03 -> ffh
x00 XOR y04 -> ntg
bfw OR bqk -> z06
nrd XOR fgs -> wpb
frj XOR qhw -> z04
bqk OR frj -> z07
y03 OR x01 -> nrd
hwm AND bqk -> z03
tgd XOR rvg -> z12
tnw OR pbm -> gnj
"""

let data =
    let rawData = getInput ()
    lazy (rawData |> parseData)

type WireMap = IDictionary<Wire, unit -> bool>

module Gate =
    let exec (wires: WireMap) gate =
        match gate with
        | OR(inA, inB, _) -> wires[inA]() || wires[inB]()
        | XOR(inA, inB, _) -> wires[inA]() <> wires[inB]()
        | AND(inA, inB, _) -> wires[inA]() && wires[inB]()

let part1 ((wires, gates): InputData) =
    let mutable gateMap = Dictionary()

    for (w, v) in wires do
        gateMap.Add(w, fun () -> v)

    for g in gates do
        match g with
        | OR(_, _, out)
        | XOR(_, _, out)
        | AND(_, _, out) ->
            if gateMap.ContainsKey out then
                failwithf "Duplicate wire: %s" out

            gateMap.Add(out, fun () -> Gate.exec gateMap g)

    gateMap.Keys
    |> Seq.filter (fun k -> k.StartsWith "z")
    |> Seq.fold
        (fun i zN ->
            match gateMap[zN]() with
            | false -> i
            | true -> i ||| (1L <<< (zN.Substring 1 |> int)))
        0L

let part2 ((wires, gates): InputData) =
    //
    0

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 4L
executePuzzle "Part 1 sample" (fun () -> part1 sample2) 2024L
executePuzzle "Part 1 finale" (fun () -> part1 data.Value) 42049478636360L

executePuzzle "Part 2 sample" (fun () -> part2 sample1) 0
executePuzzle "Part 2 finale" (fun () -> part2 data.Value) 0
