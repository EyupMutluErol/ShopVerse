using Microsoft.AspNetCore.Mvc;

namespace ShopVerse.WebUI.Extensions;

public static class MvcConfigExtensions
{
    public static void ConfigureTurkishValidationMessages(this MvcOptions options)
    {
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
            (x, y) => $"Girdiğiniz '{x}' değeri, {y} alanı için geçersizdir.");

        options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(
            (x) => $"Girdiğiniz '{x}' değeri geçersizdir.");

        options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(
            (x) => $"'{x}' alanı için bir değer girilmelidir.");

        options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(
            () => "Gerekli bilgi eksik.");

        options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(
            () => "İstek gövdesi boş olamaz.");

        options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(
            (x) => $"Girdiğiniz '{x}' değeri geçersizdir.");

        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
            (x) => $"Girdiğiniz değer geçersizdir.");

        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
            (x) => $"'{x}' alanı sayısal bir değer olmalıdır.");

        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
            (x) => $"'{x}' alanı boş bırakılamaz.");
    }
}
