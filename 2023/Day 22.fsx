// https://adventofcode.com/2023/day/22
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type InputData = Cube[]

[<Literal>]
let Ground = 0

let parseInput (text: string) : InputData =
    text
    |> String.splitAndTrim "\n"
    |> Array.map (fun s ->
        let p = Regex.Split(s, """[,~]""")

        // !IMPORTANT! The Cube height is in the _Y_ dimension!

        // Make the bricks 3D cubes by expanding left, back, and bottom by -1.
        // This causes bottom bricks to rest on the ground at z=0.
        Cube.fromCoords
            (Int32.Parse(p[0]) - 1, Int32.Parse(p[2]) - 1, Int32.Parse(p[1]) - 1)
            (Int32.Parse(p[3]), Int32.Parse(p[5]), Int32.Parse(p[4])))
// |> tee (printfn "%A")

let validateAssumptions (data: InputData) =
    // verify that every brick varies in exactly one dimension
    // and that the first point <= second point...
    data
    |> Array.iter (fun b ->
        match b.width, b.height, b.depth with
        | _, 1, 1 when b.left <= b.right -> ()
        | 1, _, 1 when b.bottom <= b.top -> ()
        | 1, 1, _ when b.back <= b.front -> ()
        | _ -> failwithf "%A" b)

let parseData s = parseInput s |> tee validateAssumptions

let sample1 =
    parseData
        """
1,0,1~1,2,1
0,0,2~2,0,2
0,2,3~2,2,3
0,0,4~0,2,4
2,0,5~2,2,5
0,1,6~2,1,6
1,1,8~1,1,9
"""

let sample2 = sample1

let data = getInput () |> parseData

let print bricks =
    let union = bricks |> Seq.fold (fun u c -> Cube.union u c) Cube.empty

    let front = Grid.create union.width (union.height + 1) '.' // varies in X
    let side = Grid.create union.depth (union.height + 1) '.' // varies in Z

    for x = 0 to union.width - 1 do
        front |> Grid.set x union.height '-'

    for z = 0 to union.depth - 1 do
        side |> Grid.set z union.height '-'

    bricks
    |> Seq.iteri (fun i b ->
        let label =
            // skip '?' (63) and unprintable chars 127..160, 173
            match i with
            | _ when i < 62 -> char (int 'A' + i - 0)
            | _ when i < 92 -> char (int '!' + i - 62)
            | _ when i < 104 -> char (161 + i - 92)
            | _ -> char (172 + i - 104)

        for y = b.bottom - union.bottom to b.top - union.bottom - 1 do
            let y = union.height - 1 - y // from bottom up

            for x = b.left - union.left to b.right - union.left - 1 do
                match front |> Grid.item x y with
                | '.' -> front |> Grid.set x y label
                | _ -> front |> Grid.set x y '?'

            for z = b.back - union.back to b.front - union.back - 1 do
                match side |> Grid.item z y with
                | '.' -> side |> Grid.set z y label
                | _ -> side |> Grid.set z y '?')

    printfn "%d x %d x %d" union.width union.depth union.height
    printfn "%*cx" (union.width / 2 - 1) ' '
    front |> Grid.printfn
    printfn ""
    printfn "%*cz" (union.depth / 2 - 1) ' '
    side |> Grid.printfn
    printfn ""

let tryFindStableBottom (stableBricks: Cube list) (brick: Cube) =
    // find all stable bricks below `b` and take `max(_.top)`
    let bricksBelow =
        let brickRect = Rect.fromCoords (brick.left, brick.back) (brick.right, brick.front)

        stableBricks
        |> List.where (fun b -> Rect.fromCoords (b.left, b.back) (b.right, b.front) |> Rect.intersects brickRect)

    let newBottom =
        match bricksBelow with
        | [] -> Ground
        | _ -> (bricksBelow |> List.maxBy (fun x -> x.top)).top

    if newBottom <> brick.bottom then Some newBottom else None

let compareBrick (a: Cube) (b: Cube) =
    if a.bottom <> b.bottom then
        a.bottom - b.bottom
    else
        a.top - b.top

let stabilizeBricks (bricks: Cube array) =
    bricks
    |> Array.sortWith compareBrick
    |> Array.fold
        (fun stableBricks brick ->
            match brick |> tryFindStableBottom stableBricks with
            | None -> stableBricks @ [ brick ] // stable
            | Some newBottom -> stableBricks @ [ brick |> Cube.offset (0, -brick.bottom + newBottom, 0) ])
        List.empty
    |> List.sortWith compareBrick
    |> List.toArray

let part1 (bricks: InputData) =
    // print bricks

    // stabilize bricks...
    let bricks = stabilizeBricks bricks
    // print bricks

    // find all bricks which are _NOT_ redundant.
    // Do this by removing each brick one at a time,
    // then check if any bricks become unstable. If
    // they do, then it is NOT a redundant brick.
    let countRedundantBricks () =
        let rec loop index stableBricks redundantCount =
            if index = bricks.Length then
                redundantCount
            else
                let brickToCheck = bricks[index]

                let (unstable, _) =
                    seq {
                        for i = index + 1 to bricks.Length - 1 do
                            if bricks[i].bottom <= brickToCheck.top then
                                bricks[i]
                    }
                    |> Seq.fold
                        (fun (unstable, stableBricks) brick ->
                            if unstable then
                                (unstable, stableBricks)
                            else
                                match brick |> tryFindStableBottom stableBricks with
                                | None -> false, stableBricks @ [ brick ] // stable
                                | Some _ -> true, stableBricks)
                        (false, stableBricks)

                let redundantCount =
                    match unstable with
                    | true -> redundantCount // brick is NOT redundnat
                    | _ -> redundantCount + 1 // brick is redundnat

                loop (index + 1) (stableBricks @ [ brickToCheck ]) redundantCount

        loop 0 List.empty 0

    countRedundantBricks ()

let part2 (bricks: InputData) =
    // print bricks

    // stabilize bricks...
    let bricks = stabilizeBricks bricks
    // print bricks

    // find all bricks which are _NOT_ redundant.
    // Do this by removing each brick one at a time,
    // then check if any bricks become unstable. If
    // they do, then it is NOT a redundant brick.
    let countFallingBricks () =
        let rec loop index stableBricks totalUnstableCount =
            if index = bricks.Length then
                totalUnstableCount
            else
                let brickToCheck = bricks[index]

                let (unstableCount, _) =
                    seq {
                        for i = index + 1 to bricks.Length - 1 do
                            bricks[i]
                    }
                    |> Seq.fold
                        (fun (unstableCount, stableBricks) brick ->
                            // if unstableCount = 0 && brick.bottom > brickToCheck.top then
                            //     (unstableCount, stableBricks)
                            // else
                            match brick |> tryFindStableBottom stableBricks with
                            | None -> unstableCount, stableBricks @ [ brick ] // stable
                            | Some newBottom ->
                                unstableCount + 1,
                                stableBricks @ [ brick |> Cube.offset (0, -brick.bottom + newBottom, 0) ])
                        (0, stableBricks)

                // if unstableCount > 0 then
                //     printfn "i=%d: %d" index unstableCount

                loop (index + 1) (stableBricks @ [ brickToCheck ]) (totalUnstableCount + unstableCount)

        loop 0 List.empty 0

    countFallingBricks ()


executePuzzle "Part 1 sample" (fun () -> part1 sample1) 5
executePuzzle "Part 1 finale" (fun () -> part1 data) 389

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 7
executePuzzle "Part 2 finale" (fun () -> part2 data) 70609
