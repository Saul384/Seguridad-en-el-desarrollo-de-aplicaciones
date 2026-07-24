$response = Invoke-RestMethod -Uri 'http://localhost:3100/loki/api/v1/query?query={category="security", module="authentication"} |= "fallido"' -UseBasicParsing
$response.data.result | ConvertTo-Json -Depth 10
