// https://adventofcode.com/2023/day/1
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

let parseInput (text: string) =
    let lines =
        text
        |> String.splitO "\n" StringSplitOptions.TrimEntries
        |> Array.where (not << String.isEmpty)

    lines

let part1 (input: string[]) =
    input
    |> Seq.fold
        (fun sum s ->
            let s = s |> Seq.where Char.IsAsciiDigit |> Seq.toArray
            // printfn "%A" s
            let d0 = s[0] |> digitToInt
            let d1 = s[s.Length - 1] |> digitToInt
            sum + (d0 * 10 + d1))
        0

let part2 (input: string[]) =
    let toDigit (s) =
        match s with
        | "one" -> 1
        | "two" -> 2
        | "three" -> 3
        | "four" -> 4
        | "five" -> 5
        | "six" -> 6
        | "seven" -> 7
        | "eight" -> 8
        | "nine" -> 9
        | _ ->
            assert (s.Length = 1)
            digitToInt s[0]

    let re = Regex("""([0-9]|one|two|three|four|five|six|seven|eight|nine)""")

    input
    |> Seq.map (
        String.replace "twone" "21"
        >> String.replace "oneight" "18"
        >> String.replace "threeight" "38"
        >> String.replace "fiveight" "58"
        >> String.replace "nineight" "98"
        >> String.replace "eightwo" "82"
        >> String.replace "eighthree" "83"
        >> String.replace "sevenine" "79"
    )
    |> Seq.fold
        (fun sum s ->
            // printfn "%A" s
            let m = re.Matches(s)
            let d0 = toDigit (m |> Seq.head).Captures[0].Value
            let d1 = toDigit (m |> Seq.last).Captures[0].Value
            sum + (d0 * 10 + d1))
        0

let data = getInput () |> parseInput

let sample1 =
    parseInput
        """
1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet
"""

let sample2 =
    parseInput
        """
two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen
"""

part1 sample1 |> testEqual "Part 1 sample" 142
part1 data |> tee (printfn "Part 1: %A") |> testEqual "Part 1" 54634

part2 sample2 |> testEqual "Part 2 sample" 281
part2 data |> tee (printfn "Part 2: %A") |> testEqual "Part 2" 53855
