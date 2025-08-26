using Microsoft.AspNetCore.Mvc;

namespace Random_Numbers_API.Controllers
{
    [ApiController]
    [Route("random")]
    public class RandomController : ControllerBase
    {
        private static readonly Random _random = new();

        [HttpGet("number")]
        public IActionResult GetRandomNumber([FromQuery] int? min, [FromQuery] int? max)
        {
            if (min.HasValue && max.HasValue)
            {
                if (min > max)
                    return BadRequest("min no puede ser mayor que max");

                return Ok(new { result = _random.Next(min.Value, max.Value + 1) });
            }

            return Ok(new { result = _random.Next() });
        }

        [HttpGet("decimal")]
        public IActionResult GetRandomDecimal()
        {
            return Ok(new { result = _random.NextDouble() });
        }

        [HttpGet("string")]
        public IActionResult GetRandomString([FromQuery] int length = 8)
        {
            if (length < 1 || length > 1024)
                return BadRequest("length debe estar entre 1 y 1024");

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var str = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());

            return Ok(new { result = str });
        }

        [HttpPost("custom")]
        public IActionResult GetCustomRandom([FromBody] CustomRandomRequest request)
        {
            switch (request.Type?.ToLower())
            {
                case "number":
                    if (request.Min == null || request.Max == null || request.Min > request.Max)
                        return BadRequest("min y max inválidos");
                    return Ok(new { result = _random.Next(request.Min.Value, request.Max.Value + 1) });

                case "decimal":
                    int decimals = request.Decimals ?? 2;
                    double number = _random.NextDouble();
                    return Ok(new { result = Math.Round(number, decimals) });

                case "string":
                    int length = request.Length ?? 8;
                    if (length < 1 || length > 1024)
                        return BadRequest("length fuera de rango");

                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var str = new string(Enumerable.Repeat(chars, length)
                        .Select(s => s[_random.Next(s.Length)]).ToArray());
                    return Ok(new { result = str });

                default:
                    return BadRequest("Tipo inválido. Usa 'number', 'decimal' o 'string'.");
            }
        }
    }

    public class CustomRandomRequest
    {
        public string? Type { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public int? Decimals { get; set; }
        public int? Length { get; set; }
    }
}