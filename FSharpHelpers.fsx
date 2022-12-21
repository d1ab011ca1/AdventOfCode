open System

let trace x =
#if INTERACTIVE
    printfn "%A" x
    x
#else
    LINQPad.Extensions.Dump(x)
#endif

/// Computes the sum of positive integers in the range 0..n, inclusive.
let summatorial n = (n * (n + 1)) / 2

/// Returns `(b, a)` if the `condition` is true, otherwise `(a, b)`.
let inline swapIf condition a b = if condition then (b, a) else (a, b)

/// Returns `(b, a)` if the `conditionf` returns true, otherwise `(a, b)`.
let inline swapWhen conditionf a b =
    if conditionf () then (b, a) else (a, b)

/// Converts the given digit character ('0'..'9') to its numeric equivalent (0..9).
let digitToInt (c: char) =
    match int c with
    | n when (int '0') <= n && n <= (int '9') -> n - (int '0')
    | _ -> failwithf "Invalid decimal digit: %A" c

/// Converts the given hexdigit character ('0'..'F') to its numeric equivalent (0..15).
let hexDigitToInt (c: char) =
    match int c with
    | n when (int '0') <= n && n <= (int '9') -> n - (int '0')
    | n when (int 'A') <= n && n <= (int 'F') -> n - (int 'A') + 10
    | n when (int 'a') <= n && n <= (int 'f') -> n - (int 'a') + 10
    | _ -> failwithf "Invalid hex digit: %A" c

let (|DecChar|) = digitToInt
let (|HexChar|) = hexDigitToInt

module String =
    open System.Text.RegularExpressions

    let comparer = StringComparer.Ordinal
    let compareri = StringComparer.OrdinalIgnoreCase

    let inline len (s: string) = s.Length

    let inline isBlank s = String.IsNullOrWhiteSpace s
    let inline isEmpty s = String.IsNullOrEmpty s
    let inline compare a b = comparer.Compare(a, b)
    let inline comparei a b = compareri.Compare(a, b)
    let inline equal a b = compare a b = 0
    let inline equali a b = comparei a b = 0
    let inline startsWith (prefix: string) (s: string) = s.StartsWith(prefix)
    let inline endsWith (suffix: string) (s: string) = s.EndsWith(suffix)
    let inline indexOf (x: string) (s: string) = s.IndexOf(x)
    let inline contains (x: string) (s: string) = s.Contains(x, StringComparison.Ordinal)

    let inline containsi (x: string) (s: string) =
        s.Contains(x, StringComparison.OrdinalIgnoreCase)

    let inline substr idx max (s: string) =
        if 0 <= max && max < s.Length - idx then
            s.Substring(idx, max)
        else
            s.Substring(idx)

    let inline left max (s: string) = substr 0 max s

    let inline right max (s: string) =
        if 0 <= max && max < s.Length then
            s.Substring(s.Length - max)
        else
            s

    let inline toArray (s: string) = s.ToCharArray()
    let inline toSeq (s: string) : char seq = s :> _

    let inline toUpper (s: string) = s.ToUpperInvariant()
    let inline toLower (s: string) = s.ToLowerInvariant()

    let inline trim (s: string) = s.Trim()
    let inline trimL (s: string) = s.TrimStart()
    let inline trimR (s: string) = s.TrimEnd()

    let inline padL c totalWidth (s: string) = s.PadLeft(c, totalWidth)
    let inline padR c totalWidth (s: string) = s.PadRight(c, totalWidth)

    /// Split a string using the given delimiter.
    let inline split (sep: string) (s: string) = s.Split(sep)
    /// Split a string into at most `count` parts.
    let inline splitN (sep: string) count (s: string) = s.Split(sep, count = count)
    /// Split a string using the given `StringSplit` options.
    let inline splitO (sep: string) opts (s: string) = s.Split(sep, options = opts)

    /// Split a string into at most `count` parts using the given `StringSplit` options.
    let inline splitNO (sep: string) count opts (s: string) =
        s.Split(sep, count = count, options = opts)

    /// Split a string using the given regular expression as a delimter.
    let inline splitRE (re: Regex) (s: string) = re.Split(s)

    let inline replace (oldValue: string) newValue (s: string) =
        s.Replace(oldValue, newValue, StringComparison.Ordinal)

    let inline replacei (oldValue: string) newValue (s: string) =
        s.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase)

    let inline replaceRE (re: Regex) replacement (s: string) =
        re.Replace(s, replacement = replacement)

    let inline replaceREWith (re: Regex) evaluator (s: string) =
        re.Replace(s, MatchEvaluator(evaluator))

    let inline tryMatch (re: Regex) (s: string) =
        match re.Match(s) with
        | m when m.Success -> Some m.Groups
        | _ -> None

module StringBuilder =
    open System.Text

    let inline append (x: string) (sb: StringBuilder) = sb.Append(x)

    let inline appendLine (x: string) (sb: StringBuilder) = sb.AppendLine(x)

    let inline appendFmt (fmt: string) (x: string) (sb: StringBuilder) = sb.AppendFormat(fmt, x)

module Int32 =
    let inline toString toBase (num: Int32) = Convert.ToString(num, toBase = toBase)
    let inline fromString fromBase (s: string) = Convert.ToInt32(s, fromBase = fromBase)

module Int64 =
    let inline toString toBase (num: Int64) = Convert.ToString(num, toBase = toBase)
    let inline fromString fromBase (s: string) = Convert.ToInt64(s, fromBase = fromBase)

module Array =
    let inline shuffle a =
        a |> Array.sortBy (fun _ -> Random.Shared.Next(0, a.Length))

let scriptPath =
#if INTERACTIVE
    fsi.CommandLineArgs[0]
#else
    LINQPad.Util.CurrentQueryPath
#endif

/// Downloads puzzle input as a string.
/// Assumes script file is named "./{Year}/Day {day}.fsx" and
/// cookie.txt exists in repo root.
let downloadInput () =
    let fi = IO.FileInfo(scriptPath)
    let year = fi.Directory.Name |> int

    let day =
        IO.Path.GetFileNameWithoutExtension(fi.Name)
        |> String.split " "
        |> Array.last
        |> int

    let cookiePath = IO.Path.Join(fi.Directory.Parent.FullName, "cookie.txt")
    let cookie = IO.File.ReadAllLines(cookiePath)[0]

    use client = new Net.Http.HttpClient()
    client.DefaultRequestHeaders.Add("cookie", cookie.Trim())

    client.GetStringAsync($"https://adventofcode.com/{year}/day/{day}/input")
    |> Async.AwaitTask
    |> Async.RunSynchronously

/// Returns the path of the file containing puzzle input.
let getInputFilePath () =
    IO.Path.ChangeExtension(scriptPath, ".txt")

/// Returns the path of the file containing puzzle input.
let getInput () =
    let inputFilePath = getInputFilePath ()

    if not (IO.File.Exists(inputFilePath)) then
        let input = downloadInput ()
        IO.File.WriteAllText(inputFilePath, input)

    IO.File.ReadAllText(inputFilePath)

/// Splits a string of text into an array of individual lines (delimited by `\n`).
/// All lines are trimmed and empty lines and discarded.
let parseInputText (text: string) =
    text
    |> String.splitO "\n" (StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)

/// Converts a collection of strings into an array of character arrays.
let toCharArrays (strings: string seq) =
    strings |> Seq.map String.toArray |> Seq.toArray

/// Splits a collection of strings into an array of word arrays. Words are
/// delimited by spaces (one or more).
let toWordArrays (strings: string seq) =
    strings
    |> Seq.map (String.splitO " " StringSplitOptions.RemoveEmptyEntries)
    |> Seq.toArray

/// Splits a collection of strings into groups of strings. Each group begins
/// with a string matching the specified prefix.
let toGroups groupPrefix (strings: string[]) =
    if strings.Length > 0 && not (strings[0] |> String.startsWith groupPrefix) then
        failwithf "Unexpected group header: %s" strings[0]

    let mutable idx = 0

    [| while idx < strings.Length do
           let groupName = strings[idx]
           idx <- idx + 1

           groupName,
           [| while idx < strings.Length && not (strings[idx] |> String.startsWith groupPrefix) do
                  strings[idx]
                  idx <- idx + 1 |] |]

/// Simple algorithm for parsing a binary tree
type Tree<'V> =
    | Value of 'V
    | Branch of Tree<'V>[]

module Tree =

    type Token<'V> =
        | Leaf of 'V
        | StartBranch
        | NextBranch
        | EndBranch
        | EOF
        | Unknown

    /// Generates a simple tree.
    /// Here is an example:
    /// ```
    /// let tokenizer (input: string) idx =
    ///     if idx = -1 || idx = input.Length then
    ///         (EOF, -1)
    ///     else
    ///         match input[idx] with
    ///         | '[' -> (StartBranch, idx + 1)
    ///         | ',' -> (NextBranch, idx + 1)
    ///         | ']' -> (EndBranch, idx + 1)
    ///         | c when Char.IsLetter(c) -> (Leaf (string c), idx + 1)
    ///         | _ -> (Unknown, idx + 1)
    ///
    /// let tree = Tree.parse (tokenizer "[a,[b][,c,],]")
    /// ```
    let parse (tokenizer: int -> Token<'V> * int) =
        let rec parse idx starting =
            match tokenizer idx with
            | (Leaf v, idx) -> Some(Value v), idx

            | (StartBranch, idx) ->
                let mutable idx = idx

                let subnodes =
                    [| let mutable more = true

                       while more do
                           match parse idx false with
                           | Some subnode, i ->
                               yield subnode
                               idx <- i
                           | None, i ->
                               more <- false
                               idx <- i |]

                Some(Branch subnodes), idx

            | (EndBranch, _) when starting -> failwith $"Unexpected end at {idx}."
            | (EndBranch, idx) -> None, idx

            | (NextBranch, _) when starting -> failwith $"Unexpected token at {idx}."
            | (NextBranch, idx) -> parse idx false

            | (EOF, idx) when starting -> None, idx
            | (EOF, _) -> failwith $"Unexpected end."

            | (Unknown, idx) -> failwith $"Invalid token at {idx}."

        parse 0 true |> fst |> Option.get

    // Converts a tree to a string.
    let stringize (n: Tree<'V>) =
        let rec stringize sb =
            function
            | Branch(subnodes) ->
                sb |> StringBuilder.append "[" |> ignore

                subnodes
                |> Seq.iteri (fun i n ->
                    if i > 0 then
                        sb |> StringBuilder.append "," |> ignore

                    stringize sb n)

                sb |> StringBuilder.append "]" |> ignore
            | Value v -> sb.Append(sprintf "%A" v) |> ignore

        let sb = Text.StringBuilder()
        stringize sb n
        sb.ToString()

/// A basic 2-dimensional point.
[<StructAttribute>]
type Point2D =
    { x: int
      y: int }

    override this.ToString() = $"({this.x},{this.y})"

    static member zero = { x = 0; y = 0 }
    static member inline ofTuple(x, y) = { x = x; y = y }
    static member inline toTuple pt = (pt.x, pt.y)

    static member inline offset (dx, dy) pt = { x = pt.x + dx; y = pt.y + dy }

/// A basic 3-dimensional point.
[<StructAttribute>]
type Point3D =
    { x: int
      y: int
      z: int }

    override this.ToString() = $"({this.x},{this.y},{this.z})"

    static member zero = { x = 0; y = 0; z = 0 }
    static member inline ofTuple(x, y, z) = { x = x; y = y; z = z }
    static member inline toTuple pt = (pt.x, pt.y, pt.z)

    static member inline offset (dx, dy, dz) pt =
        { x = pt.x + dx
          y = pt.y + dy
          z = pt.z + dz }

/// A rectangle type.
///  p1+-----bottom----+
///    |               |
///   left           right
///    |               |
///    +------top------+p2
[<NoComparison>]
[<StructAttribute>]
type Rect =
    { p1: Point2D // the "smaller" point (bottom-left), inclusive
      p2: Point2D } // the "larger" point (top-right), exclusive

    override this.ToString() = $"[{this.p1}..{this.p2})"

    member this.left = this.p1.x
    member this.right = this.p2.x
    member this.bottom = this.p1.y
    member this.top = this.p2.y
    member this.width = this.right - this.left
    member this.height = this.top - this.bottom

    static member empty = { p1 = Point2D.zero; p2 = Point2D.zero }

    static member inline isEmpty(c: Rect) = c.width = 0 || c.height = 0

    /// Creates a normalized rect with the given points
    static member inline fromPoints p1 p2 = { p1 = p1; p2 = p2 } |> Rect.normalize

    /// Creates a normalized Rect with the given points
    static member inline fromCoords xy1 xy2 =
        Rect.fromPoints (Point2D.ofTuple xy1) (Point2D.ofTuple xy2)

    static member inline dims(c: Rect) = (c.width, c.height)

    /// Returns volume of a rect.
    static member inline volume(c: Rect) =
        Math.Abs(int64 c.width * int64 c.height)

    /// Offsets the rect by offsetting both p1 and p2.
    static member inline offset (dx, dy) c : Rect =
        { c with
            p1 = c.p1 |> Point2D.offset (dx, dy)
            p2 = c.p2 |> Point2D.offset (dx, dy) }

    /// Grows the rect by offsetting p2.
    static member inline grow (dx, dy) c : Rect =
        { c with p2 = c.p2 |> Point2D.offset (dx, dy) }

    /// Returns a Rect with positive size in all dims (p1.xy <= p2.xy).
    static member normalize(c: Rect) =
        let c =
            if c.p1.x > c.p2.x then
                { c with
                    p1 = { c.p1 with x = c.p2.x - 1 }
                    p2 = { c.p2 with x = c.p1.x + 1 } }
            else
                c

        let c =
            if c.p1.y > c.p2.y then
                { c with
                    p1 = { c.p1 with y = c.p2.y - 1 }
                    p2 = { c.p2 with y = c.p1.y + 1 } }
            else
                c

        if Rect.isEmpty c then Rect.empty else c

    /// Checks if point intersects a Rect. Rect must be normalized.
    static member inline contains (pt: Point2D) (c: Rect) =
        c.left <= pt.x && pt.x < c.right && c.bottom <= pt.y && pt.y < c.top

    /// Returns the union of the two rects. Rect must be normalized.
    static member union (c1: Rect) (c2: Rect) =
        let x1, x2 = (c1, c2) ||> swapIf (c2.left < c1.left)

        let y1, y2 = (c1, c2) ||> swapIf (c2.bottom < c1.bottom)

        let inline min a b = if a < b then a else b
        let inline max a b = if a > b then a else b

        { p1 =
            { x = min x1.left x2.left
              y = min y1.bottom y2.bottom }
          p2 =
            { x = max x1.right x2.right
              y = max y1.top y2.top } }

    /// Find intersection of two rects. Rects must be normalized.
    static member intersection (c1: Rect) (c2: Rect) =
        let x1, x2 = (c1, c2) ||> swapIf (c2.left < c1.left)

        let y1, y2 = (c1, c2) ||> swapIf (c2.bottom < c1.bottom)

        let inline min a b = if a < b then a else b

        // must intersect in all 3 dims
        if x2.left < x1.right && y2.bottom < y1.top then
            { p1 = { x = x2.left; y = y2.bottom }
              p2 =
                { x = min x1.right x2.right
                  y = min y1.top y2.top } }
        else
            Rect.empty

    static member inline intersects c1 c2 =
        not (Rect.intersection c1 c2 |> Rect.isEmpty)

    /// Returns `a` minus the intersection of `b`, or `None` if there is no intersection.
    /// Rects must be normalized.
    static member tryDifference a b =
        match Rect.intersection a b with
        | i when i |> Rect.isEmpty -> None
        | i ->
            [ let mutable a = a

              if i.right = b.right && a.right <> b.right then
                  yield { a with p1 = { a.p1 with x = i.right } }
                  a <- { a with p2 = { a.p2 with x = i.right } }

              if i.left = b.left && a.left <> b.left then
                  yield { a with p2 = { a.p2 with x = i.left } }
                  a <- { a with p1 = { a.p1 with x = i.left } }

              if i.top = b.top && a.top <> b.top then
                  yield { a with p1 = { a.p1 with y = i.top } }
                  a <- { a with p2 = { a.p2 with y = i.top } }

              if i.bottom = b.bottom && a.bottom <> b.bottom then
                  yield { a with p2 = { a.p2 with y = i.bottom } }
                  a <- { a with p1 = { a.p1 with y = i.bottom } } ]
            |> Some

/// A cube type.
[<NoComparison>]
[<StructAttribute>]
type Cube =
    { p1: Point3D // the "smaller" point, inclusive
      p2: Point3D } // the "larger" point, exclusive

    override this.ToString() = $"[{this.p1}..{this.p2})"

    member this.left = this.p1.x
    member this.right = this.p2.x
    member this.bottom = this.p1.y
    member this.top = this.p2.y
    member this.back = this.p1.z
    member this.front = this.p2.z
    member this.width = this.right - this.left
    member this.height = this.top - this.bottom
    member this.depth = this.front - this.back

    static member empty = { p1 = Point3D.zero; p2 = Point3D.zero }

    static member inline isEmpty(c: Cube) =
        c.width = 0 || c.height = 0 || c.depth = 0

    /// Creates a normalized cube with the given points
    static member inline fromPoints p1 p2 = { p1 = p1; p2 = p2 } |> Cube.normalize

    /// Creates a normalized cube with the given points
    static member inline fromCoords xyz1 xyz2 =
        Cube.fromPoints (Point3D.ofTuple xyz1) (Point3D.ofTuple xyz2)

    static member inline dims(c: Cube) = (c.width, c.height, c.depth)

    /// Returns volume of a cube.
    static member inline volume(c: Cube) =
        Math.Abs(int64 c.width * int64 c.height * int64 c.depth)

    /// Offsets the cube by offsetting both p1 and p2.
    static member inline offset (dx, dy, dz) c : Cube =
        { c with
            p1 = c.p1 |> Point3D.offset (dx, dy, dz)
            p2 = c.p2 |> Point3D.offset (dx, dy, dz) }

    /// Grows the cube by offsetting p2.
    static member inline grow (dx, dy, dz) c : Cube =
        { c with p2 = c.p2 |> Point3D.offset (dx, dy, dz) }

    /// Returns a Cube with positive size in all 3 dims (p1.xyz <= p2.xyz).
    static member normalize(c: Cube) =
        let c =
            if c.p1.x > c.p2.x then
                { c with
                    p1 = { c.p1 with x = c.p2.x - 1 }
                    p2 = { c.p2 with x = c.p1.x + 1 } }
            else
                c

        let c =
            if c.p1.y > c.p2.y then
                { c with
                    p1 = { c.p1 with y = c.p2.y - 1 }
                    p2 = { c.p2 with y = c.p1.y + 1 } }
            else
                c

        let c =
            if c.p1.z > c.p2.z then
                { c with
                    p1 = { c.p1 with z = c.p2.z - 1 }
                    p2 = { c.p2 with z = c.p1.z + 1 } }
            else
                c

        if Cube.isEmpty c then Cube.empty else c

    /// Checks if point intersects a cube. Cube must be normalized.
    static member inline contains pt (c: Cube) =
        c.left <= pt.x
        && pt.x < c.right
        && c.bottom <= pt.y
        && pt.y < c.top
        && c.back <= pt.z
        && pt.z < c.front

    /// Returns the union of the two cubes. Cube must be normalized.
    static member union (c1: Cube) (c2: Cube) =
        let x1, x2 = (c1, c2) ||> swapIf (c2.left < c1.left)

        let y1, y2 = (c1, c2) ||> swapIf (c2.bottom < c1.bottom)

        let z1, z2 = (c1, c2) ||> swapIf (c2.back < c1.back)

        let inline min a b = if a < b then a else b
        let inline max a b = if a > b then a else b

        { p1 =
            { x = min x1.left x2.left
              y = min y1.bottom y2.bottom
              z = min z1.back z2.back }
          p2 =
            { x = max x1.right x2.right
              y = max y1.top y2.top
              z = max z1.front z2.front } }

    /// Find intersection of two cubes. Cubes must be normalized.
    static member intersection (c1: Cube) (c2: Cube) =
        let x1, x2 = (c1, c2) ||> swapIf (c2.left < c1.left)

        let y1, y2 = (c1, c2) ||> swapIf (c2.bottom < c1.bottom)

        let z1, z2 = (c1, c2) ||> swapIf (c2.back < c1.back)

        let inline min a b = if a < b then a else b

        // must intersect in all 3 dims
        if x2.left < x1.right && y2.bottom < y1.top && z2.back < z1.front then
            { p1 =
                { x = x2.left
                  y = y2.bottom
                  z = z2.back }
              p2 =
                { x = min x1.right x2.right
                  y = min y1.top y2.top
                  z = min z1.front z2.front } }
        else
            Cube.empty

    static member inline intersects c1 c2 =
        not (Cube.intersection c1 c2 |> Cube.isEmpty)

    /// Returns `a` minus the intersection of `b`, or `None` if there is no intersection.
    /// Cubes must be normalized.
    static member tryDifference a b =
        match Cube.intersection a b with
        | i when i |> Cube.isEmpty -> None
        | i ->
            [ let mutable a = a

              if i.right = b.right && a.right <> b.right then
                  yield { a with p1 = { a.p1 with x = i.right } }
                  a <- { a with p2 = { a.p2 with x = i.right } }

              if i.left = b.left && a.left <> b.left then
                  yield { a with p2 = { a.p2 with x = i.left } }
                  a <- { a with p1 = { a.p1 with x = i.left } }

              if i.top = b.top && a.top <> b.top then
                  yield { a with p1 = { a.p1 with y = i.top } }
                  a <- { a with p2 = { a.p2 with y = i.top } }

              if i.bottom = b.bottom && a.bottom <> b.bottom then
                  yield { a with p2 = { a.p2 with y = i.bottom } }
                  a <- { a with p1 = { a.p1 with y = i.bottom } }

              if i.front = b.front && a.front <> b.front then
                  yield { a with p1 = { a.p1 with z = i.front } }
                  a <- { a with p2 = { a.p2 with z = i.front } }

              if i.back = b.back && a.back <> b.back then
                  yield { a with p2 = { a.p2 with z = i.back } }
                  a <- { a with p1 = { a.p1 with z = i.back } } ]
            |> Some

let manhattanDistance (p1: Point2D) (p2: Point2D) =
    Math.Abs(p2.x - p1.x) + Math.Abs(p2.y - p1.y)

let manhattanDistance3D (p1: Point3D) (p2: Point3D) =
    Math.Abs(p2.x - p1.x) + Math.Abs(p2.y - p1.y) + Math.Abs(p2.z - p1.z)
