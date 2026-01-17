using Microsoft.AspNetCore.Identity;

namespace ShopVerse.WebUI.Utils;

public class TrIdentityErrorDescriber:IdentityErrorDescriber
{
    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"Şifreniz en az {length} karakter olmalıdır."
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "Şifreniz en az bir adet sembol (*, !, ., @ vb.) içermelidir."
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = "Şifreniz en az bir adet küçük harf ('a'-'z') içermelidir."
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "Şifreniz en az bir adet büyük harf ('A'-'Z') içermelidir."
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "Şifreniz en az bir adet rakam (0-9) içermelidir."
        };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateEmail),
            Description = $"'{email}' adresi zaten başka bir kullanıcı tarafından alınmış."
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError
        {
            Code = nameof(DuplicateUserName),
            Description = $"'{userName}' kullanıcı adı zaten kullanılıyor."
        };
    }
}
