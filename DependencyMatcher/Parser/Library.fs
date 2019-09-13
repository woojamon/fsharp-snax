namespace Parsing

open System.Text.RegularExpressions

module Parser =

    /// A regular expression to match OR patterns of the form A=1||B=2
    let private nonGroupedOrRegex = "(?<left>\w+=-?\w+)\|\|(?<right>\w+=-?\w+)"

    /// A regular expression to match OR patterns of the form (...)||B=2
    let private leftGroupedOrRegex = "(?<left>\(.*\))\|\|(?<right>\w+=-?\w+)"

    /// A regular expression to match OR patterns of the form A=1||(...)
    let private rightGroupedOrRegex = "(?<left>\w+=-?\w+)\|\|(?<right>\(.*\))"

    /// A regular expression to match OR patterns of the form (...)||(...)
    let private bothGroupedOrRegex = "(?<left>\(.*\))\|\|(?<right>\(.*\))"

    /// A regular expression to match OR patterns of any valid form.
    let private completeOrRegex = nonGroupedOrRegex + "|" + leftGroupedOrRegex + "|" + rightGroupedOrRegex + "|" + bothGroupedOrRegex

    /// A regular expression to match any valid OR pattern.
    let OrMatch = Regex(completeOrRegex).Match
   
    /// Removes all white space.
    let private compactify input = 
        Regex.Replace(input, "\s", "")

    /// Replaces occurences of the pattern in the string with the given replacement.
    let private replace (pattern:string) (replacement:string) (str:string) = 
        Regex.Replace(str, pattern, replacement)

    /// Degroups equality expressions, i.e.: (RegionID=8) becomes RegionID=8
    let rec private simplify input =
        let matches = Regex.Matches(input, "\((\w+!?=\w+)\)")

        if (matches.Count > 0)
            then 
                let theMatch = matches.[0]
                let theGroup = theMatch.Groups.[1]
                let simplified = input.Remove(theMatch.Index, theMatch.Length).Insert(theMatch.Index, theGroup.Value)
                simplify simplified
            else input

    /// Converts inequalities to negatives, i.e.: RegionID!=12 to RegionID=-12
    let convertInequalities input = 
        input 
        |> replace "!=" "=-"

    /// Replaces " AND " with " && " and " OR " with " || " to prevent collisions with key names.
    /// Example RegionID = 8 AND OptionID = 23 becomes RegionID = 8 && OptionID = 23
    let private replaceLogicalOperators input = 
        input 
        |> replace " AND " " && " 
        |> replace " OR " " || "

    /// Returns the next match of the OR operator
    let private nextOrMatch input = 
        let nextMatch = OrMatch input

        if (nextMatch.Success)
            then Some nextMatch
            else None 

    /// Expands a dependency filter expression into a table-like format.
    let rec expand (input:string) =
        let cleanedInput = 
            input
            |> replaceLogicalOperators 
            |> compactify
            |> simplify
            |> convertInequalities

        match nextOrMatch cleanedInput with
        | None -> 
            cleanedInput
            |> replace "\(|\)" ""
            |> replace "&&" ","
            |> (fun s -> seq[s])
        | Some m ->
            seq [
                cleanedInput.Remove(m.Index, m.Length).Insert(m.Index, m.Groups.["left"].Value);
                cleanedInput.Remove(m.Index, m.Length).Insert(m.Index, m.Groups.["right"].Value)
                ]
            |> Seq.map expand
            |> Seq.concat
            |> Seq.distinct
