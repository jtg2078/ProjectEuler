// Learn more about F# at http://fsharp.net

// Arithmetic Way
let Formula a x = (x / a) * (1 + x / a) * a / 2
let Calculate3sAnd5sUnder1000ArithWay = Formula 3 999 + Formula 5 999 - Formula 15 999


// Brute Force way
let rec brute n =
    if n > 0 then
        if n % 3 = 0 || n % 5 = 0 then
            n + brute (n-1)
        else
            0 + brute (n-1)
    else
        0

// Brute Force found online
// http://blog.matthewdoig.com/?p=63
//        seq declares we will be using a sequence workflow.

//        for n in 1 .. 999 do generates and iterates through number 1 to 999.

//        if n % 3 = 0 or n % 5 = 0 then checks if n a multiple of 3 or 5

//        yield n returns the result 1 number at a time

//        |> Seq.reduce (+) sums up all the numbers in our generated sequence

let result =
    seq {for n in 1 .. 999 do
            if n % 3 = 0 || n % 5 = 0 then
                yield n}
    |> Seq.reduce (+)
    
printfn "%A" result
