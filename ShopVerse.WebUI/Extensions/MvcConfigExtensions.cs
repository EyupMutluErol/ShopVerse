using Microsoft.AspNetCore.Mvc;

namespace ShopVerse.WebUI.Extensions;

public static class MvcConfigExtensions
{
    public static void ConfigureTurkishValidationMessages(this MvcOptions options)
    {
        // 1. Değer Geçersizse (Genel)
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
            (x, y) => $"Girdiğiniz '{x}' değeri, {y} alanı için geçersizdir.");

        // 2. Özellik Olmayan Değer Geçersizse
        options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(
            (x) => $"Girdiğiniz '{x}' değeri geçersizdir.");

        // 3. Zorunlu Alan Eksikse (Binding sırasında)
        options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(
            (x) => $"'{x}' alanı için bir değer girilmelidir.");

        // 4. Anahtar veya Değer Eksikse
        options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(
            () => "Gerekli bilgi eksik.");

        // 5. İstek Gövdesi Boşsa
        options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(
            () => "İstek gövdesi boş olamaz.");

        // 6. Bilinmeyen Değer Geçersizse
        options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(
            (x) => $"Girdiğiniz '{x}' değeri geçersizdir.");

        // 7. Değer Geçersizse (En sık karşılaşılan)
        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
            (x) => $"Girdiğiniz değer geçersizdir.");

        // 8. Değer Sayı Olmalıysa
        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
            (x) => $"'{x}' alanı sayısal bir değer olmalıdır.");

        // 9. Değer Boş Olamazsa
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
            (x) => $"'{x}' alanı boş bırakılamaz.");
    }
}
