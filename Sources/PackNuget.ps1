
Push-Location Belt

nuget pack -Build -Prop Configuration=Release 

Pop-Location

Push-Location Belt.Serialization.JsonNet

nuget pack -Build -Prop Configuration=Release 

Pop-Location
