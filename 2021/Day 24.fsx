open System
open System.IO

let digitToInt (c: char) =
    match int c with
    | n when (int '0') <= n && n <= (int '9') -> n - (int '0')
    | _ -> failwithf "Invalid decimal digit: %A" c

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
inp w
add z w
mod z 2
div w 2
add y w
mod y 2
div w 2
add x w
mod x 2
div w 2
mod w 2
"""

let program =
    let inputText = realInputText
    // let inputText = sampleInputText

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines
    |> Seq.map (fun s -> s.Split(" "))
    |> Seq.map (fun ps -> (ps.[0], ps.[1], (if ps.Length = 3 then ps.[2] else "")))
    |> Seq.toArray

let asRegister (s: string) =
    match s with
    | "w" -> 0
    | "x" -> 1
    | "y" -> 2
    | "z" -> 3
    | _ -> failwith $"Invalid register: {s}"

let (|Literal|Register|) (s: string) =
    match s with
    | "w" -> Choice2Of2 0
    | "x" -> Choice2Of2 1
    | "y" -> Choice2Of2 2
    | "z" -> Choice2Of2 3
    | _ -> Choice1Of2 <| int s

type InputEnum = Collections.Generic.IEnumerator<int>

let compute (registers: int []) (input: InputEnum) (ins, a, b) =
    let getReg n = registers.[n]
    let setReg n v = registers.[n] <- v

    let ra = a |> asRegister

    if ins = "inp" then
        if not <| input.MoveNext() then
            failwith "No more input."

        input.Current |> setReg ra
    else
        let x = ra |> getReg

        let y =
            match b with
            | Register rb -> rb |> getReg
            | Literal n -> n

        match ins with
        | "add" -> x + y |> setReg ra
        | "mul" -> x * y |> setReg ra
        | "div" ->
            match x, y with
            | _, 0 -> failwith "Divide by zero."
            | x, y when (x >= 0 && y >= 0) || (x < 0 && y < 0) -> x / y
            | x, y -> Math.Ceiling(float x / float y) |> int
            |> setReg ra
        | "mod" when x < 0 || y <= 0 -> failwith "Modulo with zero."
        | "mod" -> x % y |> setReg ra
        | "eql" -> (if x = y then 1 else 0) |> setReg ra
        | _ -> failwith $"Unexpected instruction: {ins} {a} {b}."

let part1 () =
    // for (ins, a, b) in program do
    //     match ins with
    //     | "inp" -> printfn $"{a} = next()"
    //     | "add" -> printfn $"{a} = {a} + {b}"
    //     | "mul" -> printfn $"{a} = {a} * {b}"
    //     | "div" -> printfn $"{a} = round({a} / {b})"
    //     | "mod" -> printfn $"{a} = {a} %% {b}"
    //     | "eql" -> printfn $"{a} = {a} == {b}"
    //     | _ -> failwith $"Unexpected instruction: {ins} {a} {b}."

    // w = next()
    // x = x * 0
    // x = x + z
    // x = x % 26
    // z = round(z / A)
    // x = x + B
    // x = x == w
    // x = x == 0
    // y = y * 0
    // y = y + 25
    // y = y * x
    // y = y + 1
    // z = z * y
    // y = y * 0
    // y = y + w
    // y = y + C
    // y = y * x
    // z = z + y

    // z = 0
    // A :=  1, 1, 1, 1, 1, 26, 1,26,26, 1,26, 26,26,26
    // B := 12,11,13,11,14,-10,11,-9,-1,13,-5,-10,-4,-5
    // C :=  4,11, 5,11,14,  7,11, 4, 6, 5, 9, 12,14,14

    // w = [1..9]
    // if (A == 1) then
    //   z = z * 26 + w + C
    // elif (z % 26 == w - B)
    //   z = z / 26
    // else
    //   z = z / 26 * 26 + w + C

    let A =
        [| 1
           1
           1
           1
           1
           26
           1
           26
           26
           1
           26
           26
           26
           26 |]

    let B =
        [| 12
           11
           13
           11
           14
           -10
           11
           -9
           -1
           13
           -5
           -10
           -4
           -5 |]

    let C =
        [| 4
           11
           5
           11
           14
           7
           11
           4
           6
           5
           9
           12
           14
           14 |]

    let numbers =
        seq {
            // !!! WARNING: This enumerator is not thread safe.
            // The 'number' array is returned for each enumeration
            let number = Array.create 14 9
            let mutable pos = number.Length - 1
            number.[0] <- 5

            while number.[0] > 0 do

                yield number

                number.[pos] <- number.[pos] - 1
                while pos > 0 && number.[pos] = 0 do
                    number.[pos] <- 9
                    pos <- pos - 1 // repeat on previous pos
                    number.[pos] <- number.[pos] - 1
                    if number.[pos] > 0 then
                        if pos = 5 && number.[pos] = 9 then
                            printfn "%s" (number |> Seq.map string |> String.Concat)
                        pos <- number.Length - 1


            // let next n =
            //     // count down
            //     number.[n] <- number.[n] - 1
            //     for i = n + 1 to 13 do
            //         number.[i] <- 9

            // number.[0] <- 5

            // for _ = 1 to 2 do
            //     for _ = 1 to 9 do
            //         for _ = 1 to 9 do
            //             for _ = 1 to 9 do
            //                 for _ = 1 to 9 do
            //                     printfn "%s" (number |> Seq.map string |> String.Concat)

            //                     for _ = 1 to 9 do
            //                         for _ = 1 to 9 do
            //                             for _ = 1 to 9 do
            //                                 for _ = 1 to 9 do
            //                                     for _ = 1 to 9 do
            //                                         for _ = 1 to 9 do
            //                                             for _ = 1 to 9 do
            //                                                 for _ = 1 to 9 do
            //                                                     for _ = 1 to 9 do
            //                                                         yield number
            //                                                         next 13

            //                                                     next 12

            //                                                 next 11

            //                                             next 10

            //                                         next 9

            //                                     next 8

            //                                 next 7

            //                             next 6

            //                         next 5

            //                     next 4

            //                 next 3

            //             next 2

            //         next 1

            //     next 0
        }

    let solution =
        seq {
            for number in numbers do
                let mutable z = 0

                for n = 0 to 13 do
                    let w = number.[n]

                    if A.[n] = 1 then
                        z <- z * 26 + w + C.[n]
                    elif z % 26 = w - B.[n] then
                        z <- z / 26
                    else
                        z <- z / 26 * 26 + w + C.[n]

                if z = 0 then yield number
        }
        |> Seq.head

    printfn "Part 1: %A" solution

    let registers = Array.create 4 0

    let input =
        (solution |> Seq.ofArray).GetEnumerator()

    program |> Seq.iter (compute registers input)
    printfn "%A" registers


let part2 () = printfn "Part 2: "

part1 () //
part2 () //
