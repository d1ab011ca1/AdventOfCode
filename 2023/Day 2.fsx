// https://adventofcode.com/2023/day/2
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open FSharpHelpers

type Game =
    { Id: int
      Infos: Info array array }

    override x.ToString() =
        let infos =
            x.Infos
            |> Array.map (Array.map string >> String.concat ", ")
            |> String.concat "; "

        sprintf "Game %d: %O" x.Id infos

    static member parse idx s =
        { Id = idx + 1
          Infos =
            s
            |> String.split ": "
            |> Array.item 1
            |> String.split "; "
            |> Array.map (fun grab -> grab |> String.split ", " |> Array.map (Info.parse)) }

and Info =
    { Color: Color
      Count: int }

    override x.ToString() = sprintf "%d %O" x.Count x.Color

    static member parse s =
        match s |> String.split (" ") with
        | [| count; color |] ->
            { Color = Color.Parse(color, true)
              Count = Int32.fromString 10 count }
        | _ -> failwithf "Unexpected: '%s'" s

and Color = ConsoleColor

let parseInput (text: string) =
    text
    |> String.splitO "\n" StringSplitOptions.TrimEntries
    |> Seq.where (not << String.isEmpty)
    |> Seq.mapi (Game.parse)
    // |> tee (Seq.iter (printfn "%O"))
    |> Seq.toArray

let part1 (games: Game array) =
    // ignore games containing
    // - more than 12 red
    // - OR more than 13 green
    // - OR more than and 14 blue
    // then return the sum of the ids
    games
    |> Seq.where (fun g ->
        g.Infos
        |> (not
            << Seq.exists (fun i ->
                i
                |> Seq.exists (function
                    | { Color = Color.Red; Count = cnt } when cnt > 12 -> true
                    | { Color = Color.Green; Count = cnt } when cnt > 13 -> true
                    | { Color = Color.Blue; Count = cnt } when cnt > 14 -> true
                    | _ -> false))))
    |> Seq.fold (fun sum g -> sum + g.Id) 0

let part2 (games: Game array) =
    games
    |> Seq.map (fun g ->
        g.Infos
        |> Seq.fold
            (fun m info ->
                info
                |> Seq.fold
                    (fun m cc ->
                        m
                        |> Map.change cc.Color (function
                            | None -> Some cc.Count
                            | Some n -> Some(Math.Max(n, cc.Count))))
                    m)
            Map.empty)
    |> Seq.map (tee (printfn "%A"))
    |> Seq.map (Map.values >> Seq.fold (*) 1)
    |> Seq.fold (+) 0

let data = getInput () |> parseInput

let sample1 =
    parseInput
        """
Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
"""

let sample2 = sample1

part1 sample1 |> testEqual "Part 1 sample" 8
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 2416

part2 sample2 |> testEqual "Part 2 sample" 2286
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 63307
