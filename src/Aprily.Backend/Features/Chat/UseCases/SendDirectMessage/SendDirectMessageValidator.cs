using FluentValidation;

namespace Aprily.Backend.Features.Chat.UseCases.SendDirectMessage;

public sealed class SendDirectMessageValidator : AbstractValidator<SendDirectMessageCommand>
{
    public SendDirectMessageValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();

        RuleFor(x => x)
            .Must(command =>
                !string.IsNullOrWhiteSpace(command.Content) || command.Images.Count > 0)
            .WithErrorCode("chat.message_empty")
            .WithMessage("A message must contain text or at least one image");

        RuleFor(x => x.Content)
            .MaximumLength(4000)
            .WithErrorCode("chat.message_too_long")
            .WithMessage("Message content must be 4000 characters or fewer")
            .When(x => x.Content is not null);

        RuleFor(x => x.Images)
            .Must(images => images.Count <= ChatImageUploadRules.MaxImageCount)
            .WithErrorCode("chat.too_many_images")
            .WithMessage($"A message can contain at most {ChatImageUploadRules.MaxImageCount} images");

        RuleForEach(x => x.Images)
            .Must(image => image.Length > 0 && image.Length <= ChatImageUploadRules.MaxImageSize)
            .WithErrorCode("chat.invalid_image_size")
            .WithMessage("Each image must be non-empty and no larger than 10 MB")
            .Must(image => ChatImageUploadRules.IsAllowedContentType(image.ContentType))
            .WithErrorCode("chat.invalid_image_type")
            .WithMessage("Images must be JPG, PNG, WEBP, or GIF files");
    }
}
