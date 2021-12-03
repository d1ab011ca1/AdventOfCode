<Query Kind="FSharpProgram">
  <RuntimeVersion>6.0</RuntimeVersion>
</Query>

let realInputText =
    File.ReadAllText(Path.ChangeExtension(LINQPad.Util.CurrentQueryPath, ".txt"))

let sampleInputText = 
    """
    forward 5
    down 5
    forward 8
    up 3
    down 8
    forward 2
    """
    
let input =
    let inputText = realInputText
    //let inputText = sampleInputText

    inputText
        .Trim()
        .Split('\n')
    |> Seq.map(fun s -> s.Trim())
    |> Seq.where(not << String.IsNullOrEmpty)
    |> Seq.map(fun s -> s.Split(' ', 2))
    |> Seq.map(fun ss -> ss[0], ss[1] |> int)
//input.Dump("input")

let part1 () =
    let (x,y) =
        input
        |> Seq.fold (fun (x,y) -> function
            | ("up", c) -> x,y-c
            | ("down", c) -> x,y+c
            | (_, c) -> x+c,y
           ) (0,0)

    {| pos = {| x=x; y=y |}; answer = x * y |}
    
part1().Dump("Part 1")

let part2 () =
    let (x,y,aim) =
        input
        |> Seq.fold (fun (x,y,aim) -> function
            | ("up", c) -> x,y,aim-c
            | ("down", c) -> x,y,aim+c
            | (_, c) -> x+c,y+c*aim,aim
           ) (0,0,0)

    {| pos = {| x=x; y=y; aim=aim |}; answer = x * y |}

part2().Dump("Part 2")
