// https://adventofcode.com/2023/day/20
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

[<Literal>]
let BroadcasterName = "broadcaster"

[<Literal>]
let ButtonName = "button"

[<Struct>]
type Pulse =
    | Low
    | High

[<Struct>]
type State =
    | Off
    | On

type Module =
    | Broadcaster of Connections: string[]
    | FlipFlop of Name: string * Connections: string[] * State: State
    | Conjunction of Name: string * Connections: string[] * Inputs: (string * Pulse)[]

module Module =
    let newFlipFlop name connections = FlipFlop(name, connections, Off)

    let newConjunction name connections =
        Conjunction(name, connections, Array.empty)

    let name =
        function
        | Broadcaster _ -> BroadcasterName
        | FlipFlop(n, _, _)
        | Conjunction(n, _, _) -> n

    let connections =
        function
        | Broadcaster(cs)
        | FlipFlop(_, cs, _)
        | Conjunction(_, cs, _) -> cs

    /// Send a source signal to the destination module.
    ///
    /// Returns the updated module and its side effect, if any.
    let signal src pulse dest =
        match dest, pulse with
        | Broadcaster cs, _ ->
            // echo to connected elements
            dest, Some(BroadcasterName, pulse, cs)

        | Conjunction(n, cs, inputs), _ ->
            let idx = inputs |> Array.findIndex (fst >> (=) src)

            let newInputs =
                if inputs[idx] = (src, pulse) then
                    inputs // no change
                else
                    let newInputs = inputs |> Array.copy
                    newInputs[idx] <- (src, pulse)
                    newInputs

            let output =
                if pulse = Low || newInputs |> Array.exists (snd >> (=) Low) then
                    High
                else
                    Low

            Conjunction(n, cs, newInputs), Some(n, output, cs)

        | FlipFlop(n, cs, state), Low ->
            let (newState, output) =
                match state with
                | On -> Off, Low
                | _ -> On, High

            FlipFlop(n, cs, newState), Some(n, output, cs)

        | FlipFlop _, High ->
            // do nothing
            dest, None

let (|Name|) = Module.name
let (|Connections|) = Module.connections

type InputData = Input of Module[]

let parseInput (text: string) : InputData =
    let input =
        text
        |> String.splitAndTrim "\n"
        |> Array.map (fun s ->
            match s |> String.split " -> " with
            | [| BroadcasterName; cs |] -> Broadcaster(cs |> String.split ", ")
            | [| name; cs |] when name[0] = '%' -> Module.newFlipFlop (name.Substring(1)) (cs |> String.split ", ")
            | [| name; cs |] when name[0] = '&' -> Module.newConjunction (name.Substring(1)) (cs |> String.split ", ")
            | _ -> failwithf "Unexpected input: %s" s)

    // patch up the Conjunction state...
    let allSrcDests = input |> Array.map (fun ((Name n) as (Connections cs)) -> (n, cs))

    input
    |> Array.map (fun m ->
        match m with
        | Conjunction(con, cs, _) ->
            let inputs =
                allSrcDests
                |> Seq.where (snd >> Array.contains con)
                |> Seq.map (fun (src, _) -> (src, Low))
                |> Seq.toArray

            Conjunction(con, cs, inputs)
        | _ -> m)
    |> Input
// |> tee (printfn "%A")

let sample1 =
    parseInput
        """
broadcaster -> a, b, c
%a -> b
%b -> c
%c -> inv
&inv -> a
"""

let sample2 =
    parseInput
        """
broadcaster -> a
%a -> inv, con
&inv -> b
%b -> con
&con -> output
"""

let createIndexMap modules =
    modules
    |> Seq.indexed
    |> Seq.fold (fun map (i, (Name n)) -> map |> Map.add n i) Map.empty

let buttonPulse = [ (ButtonName, Low, [| BroadcasterName |]) ]

let inline sendPulses (modules: Module[]) indexMap ([<InlineIfLambda>] onPulse) pulses =
    let rec loop pulses =
        match pulses with
        | [] -> () // done
        | (from, pulse, destinations) :: tail ->
            let next =
                destinations
                |> Seq.choose (fun dest ->
                    onPulse (pulse, dest)

                    match indexMap |> Map.tryFind dest with
                    | Some mi ->
                        let (m, next) = modules[mi] |> Module.signal from pulse
                        modules[mi] <- m
                        next
                    | _ -> None)
                |> Seq.toList

            (tail @ next) |> loop

    loop pulses

let part1 (Input modules) numberOfButtonPresses =
    let indexMap = createIndexMap modules

    let mutable highPulseCount = 0
    let mutable lowPulseCount = 0

    let rec pushButton cnt =
        if cnt <= numberOfButtonPresses then
            buttonPulse
            |> sendPulses modules indexMap (function
                | Low, _ -> lowPulseCount <- lowPulseCount + 1
                | High, _ -> highPulseCount <- highPulseCount + 1)

            pushButton (cnt + 1)
        else
            int64 lowPulseCount * int64 highPulseCount

    pushButton 1

let part2 (Input modules) =
    let indexMap = createIndexMap modules

    let mutable rxLowCount = 0

    let rec pushButton cnt =
        rxLowCount <- 0

        buttonPulse
        |> sendPulses modules indexMap (function
            | Low, "rx" -> rxLowCount <- rxLowCount + 1
            | _ -> ())

        match rxLowCount with
        | 1 -> cnt
        | _ -> pushButton (cnt + 1)

    pushButton 1

let data = getInput () |> parseInput

executePuzzle "Part 1 sample1" (fun () -> part1 sample1 1000) 32000000L
executePuzzle "Part 1 sample2" (fun () -> part1 sample2 1000) 11687500L
executePuzzle "Part 1 finale" (fun () -> part1 data 1000) 899848294L

executePuzzle "Part 2 finale" (fun () -> part2 data) 0
