module Tests

open Xunit
open Parsing

[<Theory>]
[<InlineData("HomesiteID = 24","HomesiteID=24")>]
[<InlineData("(HomesiteID = 24)","HomesiteID=24")>]
[<InlineData("((HomesiteID = 24))","HomesiteID=24")>]
[<InlineData("HomesiteID = 24 && RegionID = 15","HomesiteID=24,RegionID=15")>]
[<InlineData("(HomesiteID = 24 && RegionID = 15)","HomesiteID=24,RegionID=15")>]
[<InlineData("(HomesiteID = 24) && (RegionID = 15)","HomesiteID=24,RegionID=15")>]
[<InlineData("((HomesiteID = 24) && (RegionID = 15))","HomesiteID=24,RegionID=15")>]
[<InlineData("( HomesiteID = 24 && ( RegionID = 15 && OptionID = 12 ) )","HomesiteID=24,RegionID=15,OptionID=12")>]
[<InlineData("( HomesiteID = 24 && CommunityID = 35 ) AND ( RegionID = 15 && OptionID = 12 )","HomesiteID=24,CommunityID=35,RegionID=15,OptionID=12")>]
[<InlineData("( ( HomesiteID = 24 && CommunityID = 35 ) AND ( RegionID = 15 && OptionID = 12 ) )","HomesiteID=24,CommunityID=35,RegionID=15,OptionID=12")>]
[<InlineData("HomesiteID = 24 OR RegionID = 12","HomesiteID=24;RegionID=12")>]
[<InlineData("( HomesiteID = 24 OR RegionID = 12 )","HomesiteID=24;RegionID=12")>]
[<InlineData("( HomesiteID = 24 ) OR ( RegionID = 12 )","HomesiteID=24;RegionID=12")>]
[<InlineData("( HomesiteID = 24 ) OR RegionID = 12","HomesiteID=24;RegionID=12")>]
[<InlineData("HomesiteID = 24 OR ( RegionID = 12 )","HomesiteID=24;RegionID=12")>]
[<InlineData("HomesiteID = 24 OR ( RegionID = 12 OR PhaseID = 7 )","HomesiteID=24;RegionID=12;PhaseID=7")>]
[<InlineData("( HomesiteID = 24 OR RegionID = 12 ) OR PhaseID = 7","HomesiteID=24;RegionID=12;PhaseID=7")>]
[<InlineData("( HomesiteID = 24 OR RegionID = 12 ) OR ( PhaseID = 7 OR OptionID = 9 )","HomesiteID=24;RegionID=12;PhaseID=7;OptionID=9")>]
[<InlineData("( HomesiteID = 24 OR RegionID = 12 ) AND PhaseID = 7","HomesiteID=24,PhaseID=7;RegionID=12,PhaseID=7")>]
[<InlineData("PhaseID = 7 AND ( HomesiteID = 24 OR RegionID = 12 )","PhaseID=7,HomesiteID=24;PhaseID=7,RegionID=12")>]
[<InlineData("PhaseID = 7 AND ( HomesiteID = 24 OR RegionID = 12 ) AND OptionID = 547","PhaseID=7,HomesiteID=24,OptionID=547;PhaseID=7,RegionID=12,OptionID=547")>]
[<InlineData("( HomesiteID = 24 && OptionID = 7 ) OR ( RegionID = 12 )","HomesiteID=24,OptionID=7;RegionID=12")>]
[<InlineData("( HomesiteID = 24 ) OR ( RegionID = 12 && OptionID = 7 )","HomesiteID=24;RegionID=12,OptionID=7")>]
[<InlineData("( HomesiteID = 24 AND CommunityID = 34 ) OR ( RegionID = 12 AND OptionID = 7 )","HomesiteID=24,CommunityID=34;RegionID=12,OptionID=7")>]
[<InlineData("( ( HomesiteID = 24 ) OR RegionID = 12 ) AND PhaseID = 12","HomesiteID=24,PhaseID=12;RegionID=12,PhaseID=12")>]
[<InlineData("PhaseID = 12 AND ( ( HomesiteID = 24 ) OR RegionID = 12 )","PhaseID=12,HomesiteID=24;PhaseID=12,RegionID=12")>]
[<InlineData("( ( HomesiteID = 24 ) OR ( RegionID = 12 ) ) AND PhaseID = 12","HomesiteID=24,PhaseID=12;RegionID=12,PhaseID=12")>]
[<InlineData("PhaseID = 12 AND ( ( HomesiteID = 24 ) OR ( RegionID = 12 ) )","PhaseID=12,HomesiteID=24;PhaseID=12,RegionID=12")>]
[<InlineData("HomesiteID != 24","HomesiteID=-24")>]
[<InlineData("HomesiteID != 24 OR RegionID = 12", "HomesiteID=-24;RegionID=12" )>]
[<InlineData("HomesiteID = 24 OR RegionID != 12", "HomesiteID=24;RegionID=-12")>]
[<InlineData("HomesiteID != 24 OR RegionID != 12", "HomesiteID=-24;RegionID=-12")>]
[<InlineData("HomesiteID != 12 AND RegionID != 12", "HomesiteID=-12,RegionID=-12")>]
let expand_dependencyFilter_returnsExpectedExpansion (filter, expansion: string) =   
    /// Arrange   
    // Get the expected expansion.   
    let expectedExpansion = expansion.Split(';')   

    /// Act   
    // Get the expansion for the dependency filter.   
    let actualExpansion = Parser.expand filter   

    /// Assert   
    // Verify that the actual expansion matches the expected expansion.   
    Assert.Equal<seq<string>>(expectedExpansion, actualExpansion)