open System
open System.IO

let realInputText = 
    let inputPath = Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")
    12, File.ReadAllText(inputPath)

let sampleInputText =
    5, """
    00100
    11110
    10110
    10111
    10101
    01111
    00111
    11100
    10000
    11001
    00010
    01010
    """

let bits,inputs =
    let bits,inputText = realInputText
    //let bits,inputText = sampleInputText

    bits,
    inputText.Trim().Split('\n')
    |> Seq.map (fun s -> s.Trim())
    |> Seq.where (not << String.IsNullOrEmpty)
    |> Seq.map (fun s -> Convert.ToInt32(s, 2))
    |> Seq.toArray
//printfn $"{bits}, {input.[0]}"

let part1 () =
    let gamma = 
        [for n in 0..(bits-1) do
             let ones = (inputs |> Array.where(fun i -> i &&& (1 <<< n) <> 0)).Length
             if ones > inputs.Length / 2 then 1 <<< n else 0]
        |> Seq.fold (|||) 0

    let epsilon = ~~~gamma &&& ((1 <<< bits) - 1)
    let gammas = Convert.ToString(gamma, 2).PadLeft(bits, '0')
    let epsilons = Convert.ToString(epsilon, 2).PadLeft(bits, '0')
    printfn $"Part 1: {gamma * epsilon} (gamma={gamma} 0b{gammas}, epsilon={epsilon} 0b{epsilons})"

part1 () //3912944

let part2 () =
    let partition bit ns = 
        let zeros,ones = ns |> Array.partition(fun n -> n &&& (1 <<< bit) = 0)
        if ones.Length >= zeros.Length then ones,zeros else zeros,ones

    let rec calc_o2 bit ns =
        let o2,_ = ns |> partition bit
        if o2.Length = 1 then o2.[0]
        else o2 |> calc_o2 (bit - 1)

    let rec calc_co2 bit ns =
        let _,co2 = ns |> partition bit
        if co2.Length = 1 then co2.[0]
        else co2 |> calc_co2 (bit - 1)

    let o2 = inputs |> calc_o2 (bits - 1)
    let co2 = inputs |> calc_co2 (bits - 1)
    printfn $"Part 1: {o2 * co2} (O2={o2}, CO2={co2})"

part2 () //4996233
