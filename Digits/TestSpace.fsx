 
open System
open System.IO

let rootPath = @"C:\Projects\Workshops\Digits\Digits\"

let parseRowIntoInts (row:string[]) =
    row |> Array.Parallel.map (fun s -> s.Split(',') |> Array.map(fun s2 -> int s2))

type NumberSample = { Label:int; Pixels:int[] }
type Hit = { Predicted:int; Actual:int }

let loadNumberSamples path =
    (File.ReadAllLines path).[1..] 
    |> parseRowIntoInts 
    |> Array.Parallel.map(fun r -> { Label = r.[0]; Pixels = r.[1..] } )

let trainingSamples = loadNumberSamples (rootPath + "trainingsample.csv")

let getPointWeight p1 p2 = 
    (p1 - p2) * (p1 - p2)

let getDistanceForArray (a1:int[]) (a2:int[]) =
    Array.map2(getPointWeight) a1 a2 
    |> Array.sum 
    |> Convert.ToDouble 
    |> sqrt
    
let classify (points:int[]) = 
    trainingSamples
    |> Array.minBy(fun r -> getDistanceForArray r.Pixels points)

let validationSamples = loadNumberSamples (rootPath + "validationsample.csv")

let validate (sample:NumberSample) =
    if classify(sample.Pixels).Label = sample.Label
    then 1
    else 0

let totalHits = validationSamples
                |> Array.Parallel.map(validate)
                |> Array.sum

let hitRate = ((double)totalHits / (double)(Array.length validationSamples)) * (double)100


