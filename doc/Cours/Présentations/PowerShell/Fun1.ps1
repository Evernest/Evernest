Function Add ($x, $y)
{
$Ans = $x + $y
Write-Host "The Answer is $Ans"
}

Function Add { $args[0] + $args[1] }

Function Add
{
Param ([int]$x = 0, [int]$y = 0)
$Ans = $x + $y
Write-Host $Ans
}