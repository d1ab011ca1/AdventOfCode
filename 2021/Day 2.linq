<Query Kind="FSharpProgram">
  <RuntimeVersion>6.0</RuntimeVersion>
</Query>

let input() =
    File.ReadAllLines(Path.ChangeExtension(LINQPad.Util.CurrentQueryPath, ".txt"))

let sample() =
    let text = """
        forward 5
        down 5
        forward 8
        up 3
        down 8
        forward 2
        """
    text.Split(System.Environment.NewLine)
    |> Seq.where(not << String.IsNullOrWhiteSpace)
    |> Seq.map(fun s -> s.Trim())

let data =
    //sample()
    input()
    |> Seq.map(fun s -> s.Split(" ", 2))
    |> Seq.map(fun ss -> ss[0], ss[1] |> int)

//data.Dump()

let part1 () =
    let (x,y) =
        data
        |> Seq.fold (fun (x,y) -> function
            | ("up", c) -> x,y-c
            | ("down", c) -> x,y+c
            | (_, c) -> x+c,y
           ) (0,0)

    {| pos={|x=x;y=y|}; answer=x * y |}

let part2 () =
    let (x,y,aim) =
        data
        |> Seq.fold (fun (x,y,aim) -> function
            | ("up", c) -> x,y,aim-c
            | ("down", c) -> x,y,aim+c
            | (_, c) -> x+c,y+c*aim,aim
           ) (0,0,0)

    {| pos={|x=x;y=y;aim=aim|}; answer=x * y |}

part1().Dump("Part 1")
part2().Dump("Part 2")
