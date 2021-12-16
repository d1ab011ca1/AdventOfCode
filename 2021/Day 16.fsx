open System
open System.IO

let digitToInt (c: char) =
    match int c with
    | n when (int '0') <= n && n <= (int '9') -> n - (int '0')
    | _ -> failwithf "Invalid decimal digit: %A" c

let hexDigitToInt (c: char) =
    match int c with
    | n when (int '0') <= n && n <= (int '9') -> n - (int '0')
    | n when (int 'A') <= n && n <= (int 'F') -> n - (int 'A') + 10
    | n when (int 'a') <= n && n <= (int 'f') -> n - (int 'a') + 10
    | _ -> failwithf "Invalid hex digit: %A" c

let (|DecChar|) = digitToInt
let (|HexChar|) = hexDigitToInt

let realInputText =
    let inputPath =
        Path.ChangeExtension(fsi.CommandLineArgs.[0], ".txt")

    File.ReadAllText(inputPath)

let sampleInputText =
    """
A0016C880162017C3686B18A3D4780
"""
// D2FE28  [LiteralValue (6, 2021)]
// 38006F45291200  [Operator (1, [LiteralValue (6, 10); LiteralValue (2, 20)])]
// EE00D40C823060  [Operator (7, [LiteralValue (2, 1); LiteralValue (4, 2); LiteralValue (1, 3)])]

let inputs =
    let inputText = realInputText
    // let inputText = sampleInputText

    let lines =
        inputText.Split(
            '\n',
            StringSplitOptions.TrimEntries
            ||| StringSplitOptions.RemoveEmptyEntries
        )

    lines.[0]
//printfn "%A" inputs

type PacketIdx = int
type Version = int
type TypeId = int

type Packet =
    | EOF
    | LiteralValue of Literal
    | Operator of Operator

and Literal = { version: Version; value: int64 }

and Operator =
    { version: Version
      typeId: TypeId
      packets: Packet list }

let nextStr (packet: string) (pc: PacketIdx) (n: int) : (string * PacketIdx) =
    if n > packet.Length - pc then
        invalidArg "n" $"Packet overflow."

    packet.Substring(pc, n), pc + n

let nextInt32 (packet: string) pc n : (int * PacketIdx) =
    if n > 32 then
        invalidArg "n" $"Too many bits for Int32: {n}."

    let (s, pc) = nextStr packet pc n
    Convert.ToInt32(s, fromBase = 2), pc

let nextUInt32 (packet: string) pc n : (uint * PacketIdx) =
    if n > 32 then
        invalidArg "n" $"Too many bits for UInt32: {n}."

    let (s, pc) = nextStr packet pc n
    Convert.ToUInt32(s, fromBase = 2), pc

let nextLiteral (packet: string) (pc: PacketIdx) : (int64 * PacketIdx) =
    let mutable res = {| pc = pc; value = 0L |}
    let mutable nibble = 0

    while nibble >= 0 do
        if nibble >= 16 then
            failwith $"Invalid numeric literal: {nextStr packet pc (16 * 5)}"

        let (n, pc) = nextInt32 packet res.pc 5

        res <-
            {| res with
                pc = pc
                value = (res.value <<< 4) ||| ((int64 n) &&& 0b1111L) |}

        if n < 0b10000 then
            nibble <- -1
        else
            nibble <- nibble + 1 // break

    res.value, res.pc

let rec nextPacket (packet: string) (pc: PacketIdx) : (Packet * PacketIdx) =
    let startAt = pc
    // Check for trailing zeros in last byte. All packets are greater than a byte
    if packet.Length - pc < 8 then
        match nextInt32 packet pc (packet.Length - pc) with
        | (0, pc) -> EOF, pc
        | _ -> failwith $"Unexpected padding: {nextStr packet pc (packet.Length - pc)}."

    else
        let (version, pc) = nextInt32 packet pc 3
        let (typeId, pc) = nextInt32 packet pc 3

        match typeId with
        | 4 ->
            let (value, pc) = nextLiteral packet pc
            LiteralValue { version = version; value = value }, pc

        | _ -> // operator
            let subpkts, pc =
                match nextStr packet pc 1 with
                | ("0", pc) ->
                    let (subpktLen, subpktStart) = nextInt32 packet pc 15
                    let (subpkt, subpktEnd) = nextStr packet subpktStart subpktLen
                    parsePackets subpkt, subpktEnd

                | (_, pc) ->
                    let (subpktCount, pc) = nextInt32 packet pc 11
                    let mutable pc = pc

                    [ for _ = 1 to subpktCount do
                          let (pkt, nextpc) = nextPacket packet pc
                          pc <- nextpc
                          yield pkt ],
                    pc

            Operator
                { version = version
                  typeId = typeId
                  packets = subpkts },
            pc
// | _ -> failwith $"Unexpected packet type: {typeId}."

and parsePackets (packet: string) : Packet list =
    let mutable pc = 0

    [ while pc < packet.Length do
          let (pkt, nextpc) = nextPacket packet pc

          if pkt = EOF then
              pc <- packet.Length
          else
              pc <- nextpc
              yield pkt ]

let part1 () =
    let binary =
        inputs
        |> Seq.chunkBySize 2
        |> Seq.map (fun cs -> Convert.ToByte(String.Concat(cs), fromBase = 16))
        |> Seq.map (fun b -> Convert.ToString(b, 2).PadLeft(8, '0'))
        |> String.Concat

    // let bits =
    //     binary |> Seq.map ((=) '1') |> Seq.toArray

    let pkts = parsePackets binary

    let rec calcVersionSum pkts =
        pkts
        |> Seq.sumBy (function
            | EOF -> 0
            | LiteralValue { version = v } -> v
            | Operator { version = v; packets = subpkts } -> v + (subpkts |> calcVersionSum))

    let versionSum = pkts |> calcVersionSum
    printfn "%A" pkts
    printfn "Part 1: %A" versionSum

let part2 () = printfn "Part 2: "

part1 () // 860
part2 () //
