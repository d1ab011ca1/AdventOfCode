// https://adventofcode.com/2023/day/X
#if INTERACTIVE
#load "../FSharpHelpers.fsx"
#endif

open System
open System.Text.RegularExpressions
open FSharpHelpers

type Part = { x: int; m: int; a: int; s: int }

type Rule =
    { Property: char
      Operator: char
      Comparand: int
      NextWorkflow: string }

    override this.ToString() =
        $"{this.Property}{this.Operator}{this.Comparand}:{this.NextWorkflow}"

type Workflow =
    { Name: string
      Rules: Rule[]
      Default: string }

    override this.ToString() =
        let sb = StringBuilder.create 100

        let prn (r: Rule) sb =
            sb
            |> StringBuilder.append (
                match r.Operator with
                | '>' -> $"{r.Property}:({r.Comparand}..4000]"
                | _ -> $"{r.Property}:[0..{r.Comparand})"
            )

        let prnNot (r: Rule) sb =
            sb
            |> StringBuilder.append (
                match r.Operator with
                | '>' -> $"{r.Property}:[0..{r.Comparand}]"
                | _ -> $"{r.Property}:[{r.Comparand}..4000]"
            )

        for i = 0 to this.Rules.Length - 1 do
            if i > 0 then sb |> StringBuilder.appendLine "" else sb
            |> StringBuilder.append this.Name
            |> StringBuilder.append ": "
            |> prn this.Rules[i]
            |> ignore

            for j = 0 to i - 1 do
                sb |> StringBuilder.append " && " |> prnNot this.Rules[j] |> ignore

            sb
            |> StringBuilder.append " -> "
            |> StringBuilder.append this.Rules[i].NextWorkflow
            |> ignore

        sb
        |> StringBuilder.appendLine ""
        |> StringBuilder.append this.Name
        |> StringBuilder.append ": "
        |> ignore

        for i = 0 to this.Rules.Length - 1 do
            (if i = 0 then sb else sb |> StringBuilder.append " && ")
            |> prnNot this.Rules[i]
            |> ignore

        sb |> StringBuilder.append " -> " |> StringBuilder.append this.Default |> string

type Workflows = Map<string, Workflow>
type InputData = Input of Workflows * Part[]

let parseInput (text: string) : InputData =
    let segments = text |> String.splitRE (Regex "\r?\n\r?\n")

    let workflows =
        segments[0]
        |> String.splitAndTrim "\n"
        |> Array.fold
            (fun map s ->
                let m = Regex.Match(s, """^(\w+){(([xmas])([<>])(\d+):(\w+),)+(\w+)}$""")
                assert m.Success

                map
                |> Map.add
                    m.Groups[1].Value
                    { Name = m.Groups[1].Value
                      Rules =
                        [| for i = 0 to m.Groups[2].Captures.Count - 1 do
                               { Property = m.Groups[3].Captures[i].Value[0]
                                 Operator = m.Groups[4].Captures[i].Value[0]
                                 Comparand = m.Groups[5].Captures[i].Value |> Int32.Parse
                                 NextWorkflow = m.Groups[6].Captures[i].Value } |]
                      Default = m.Groups[7].Value })
            Map.empty

    let parts =
        segments[1]
        |> String.splitAndTrim "\n"
        |> Array.map (fun s ->
            let m = Regex.Match(s, """^{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}$""")
            assert m.Success

            { x = m.Groups[1].Value |> Int32.Parse
              m = m.Groups[2].Value |> Int32.Parse
              a = m.Groups[3].Value |> Int32.Parse
              s = m.Groups[4].Value |> Int32.Parse })

    (workflows, parts) |> Input //|> tee (printfn "%A")

let sample1 =
    parseInput
        """
px{a<2006:qkq,m>2090:A,rfg}
pv{a>1716:R,A}
lnx{m>1548:A,A}
rfg{s<537:gd,x>2440:R,A}
qs{s>3448:A,lnx}
qkq{x<1416:A,crn}
crn{x>2662:A,R}
in{s<1351:px,qqz}
qqz{s>2770:qs,m<1801:hdj,R}
gd{a>3333:R,R}
hdj{m>838:A,pv}

{x=787,m=2655,a=1222,s=2876}
{x=1679,m=44,a=2067,s=496}
{x=2036,m=264,a=79,s=2244}
{x=2461,m=1339,a=466,s=291}
{x=2127,m=1623,a=2188,s=1013}
"""

let sample2 = sample1

let simplify workflows =
    let rec loop workflows =
        let workflows =
            workflows
            |> Map.map (fun _ w ->
                let rec reduce rules =
                    match rules |> Array.tryLast with
                    | Some rule when rule.NextWorkflow = w.Default ->
                        rules |> Array.removeAt (rules.Length - 1) |> reduce
                    | _ -> rules

                { w with Rules = reduce w.Rules })

        // find all zero-length rules
        let simpleWorkflows = workflows |> Map.filter (fun _ w -> w.Rules.Length = 0)

        if simpleWorkflows = Map.empty then
            workflows
        else
            // First remove all simple workflows...
            let workflows =
                simpleWorkflows
                |> Map.fold (fun map _ sw -> map |> Map.remove sw.Name) workflows

            // Now "inline" all simple workflows...
            let workflows =
                workflows
                |> Map.map (fun _ w ->
                    let rules =
                        w.Rules
                        |> Array.map (fun r ->
                            match simpleWorkflows |> Map.tryFind r.NextWorkflow with
                            | Some replacement -> { r with NextWorkflow = replacement.Default }
                            | _ -> r)

                    let def =
                        match simpleWorkflows |> Map.tryFind w.Default with
                        | Some replacement -> replacement.Default
                        | _ -> w.Default

                    { w with Rules = rules; Default = def })

            loop workflows

    loop workflows

let part1 (Input(workflows, parts)) =
    let start = workflows["in"]

    parts
    |> Seq.where (fun p ->
        let rec runWorkflow wf =
            match applyRules wf 0 with
            | "A" -> true
            | "R" -> false
            | next -> runWorkflow workflows[next]

        and applyRules wf idx =
            if idx = wf.Rules.Length then
                wf.Default
            else
                let rule = wf.Rules[idx]

                let prop =
                    match rule.Property with
                    | 'x' -> p.x
                    | 'm' -> p.m
                    | 'a' -> p.a
                    | _ -> p.s

                let op =
                    match rule.Operator with
                    | '>' -> (>)
                    | _ -> (<)

                if op prop rule.Comparand then
                    rule.NextWorkflow
                else
                    applyRules wf (idx + 1)

        runWorkflow start)
    |> Seq.map (fun p -> p.x + p.m + p.a + p.s)
    |> Seq.sum

let part2 (Input(workflows, _)) =
    let workflows = simplify workflows |> tee (Map.values >> Seq.iter (printfn "%O"))

    // * A : all paths from "in" to "A"
    // * R : all paths from "in" to "R"
    // * A(prop) : union of all 'prop' rules in A
    // * R(prop) : union of all 'prop' rules in R
    // * A'(prop): A(prop) - R(prop)
    // * distinct(set): number of unique elements

    // distinct(A'(x)) * distinct(A'(m)) * distinct(A'(a)) * distinct(A'(s))
    1L

let data = getInput () |> parseInput

executePuzzle "Part 1 sample" (fun () -> part1 sample1) 19114
executePuzzle "Part 1 finale" (fun () -> part1 data) 374873

executePuzzle "Part 2 sample" (fun () -> part2 sample2) 167409079868000L
executePuzzle "Part 2 finale" (fun () -> part2 data) 0L
