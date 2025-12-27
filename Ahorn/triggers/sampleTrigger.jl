module MyNewCelesteModSampleTrigger

using ..Ahorn, Maple

@mapdef Trigger "MyNewCelesteMod/SampleTrigger" SampleTrigger(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight,
    sampleProperty::Integer=0
)

const placements = Ahorn.PlacementDict(
    "Sample Trigger (MyNewCelesteMod)" => Ahorn.EntityPlacement(
        SampleTrigger,
        "rectangle",
    )
)

end