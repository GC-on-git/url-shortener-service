using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShortenURL.DataBase;
using ShortenURL.Models;
using ShortenURL.utils;

namespace ShortenURL.Controllers
{
    

    [ApiController]
    [Route("/")]
    public class ShortenUrlController : ControllerBase
    {
        private readonly UrlDbContext _context;

        public ShortenUrlController(UrlDbContext context)
        {
            _context = context;
        }

        [HttpPost("shorten")] 
        public async Task<IActionResult> ShortenUrl(
            [FromBody] string urlRequest) 
        {
            var validator = new Validator();
            var validationResult = await validator.ValidateAsync(urlRequest);

            if (validationResult.IsValid)
            {
                try
                {
                    var longUrl = Uri.UnescapeDataString(urlRequest);

                    var shortCode = Encoder.Encode(longUrl); 

                    // check already existing 
                    var exists = await _context.UrlMappings.FirstOrDefaultAsync(x => x.ShortCode == shortCode);
                    if (exists == null)
                    {
                        var mapping = new UrlMapping
                        {
                            ShortCode = shortCode,
                            LongUrl = longUrl
                        };
                        _context.UrlMappings.Add(mapping);
                        await _context.SaveChangesAsync();
                    }

                    var shortUrl = $"{Request.Scheme}://{Request.Host}{shortCode}";

                    return Ok(new { shortUrl });
                }

                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest($"Errors during validating URL: {validationResult.Errors.Select(e=>e.ErrorMessage).ToList()}");
        }
        

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> GetOriginalUrl([FromRoute] string shortCode)
        {
            try
            {
                var shortUrl = Uri.UnescapeDataString(shortCode);
                var mapping = await _context.UrlMappings.FirstOrDefaultAsync(x => x.ShortCode == shortCode);

                if (mapping == null)
                {
                    return NotFound();
                }
                return Redirect(mapping.LongUrl);

            }
            catch (Exception)
            {
                return NotFound("Short URL not found");
            }
        }


    }
}