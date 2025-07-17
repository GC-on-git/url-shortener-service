using FluentValidation;

namespace ShortenURL.utils
{
    public class Validator : AbstractValidator<string>
    {
        public Validator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .WithMessage("URL cannot be empty")
                .Must(BeValidUrl)
                .WithMessage("Invalid URL, please provide a Valid URl")
                .MaximumLength(2048)
                .WithMessage(" URL character limit exceeded");
        }

        private static bool BeValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            
            //checking if URI is valid
            if(!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
            
            //Checking that it is either HTTP or HTTPS
            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }
    }
}
