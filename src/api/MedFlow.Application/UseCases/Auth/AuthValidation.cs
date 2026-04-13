namespace MedFlow.Application.UseCases.Auth;

internal static class AuthValidation
{
    internal const int MinPasswordLength = 8;

    /// <summary>
    /// Valida e retorna o e-mail normalizado e a senha original (a normalização não se aplica à senha).
    /// </summary>
    internal static (string NormalizedEmail, string Password) ValidateAndNormalize(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail é obrigatório.", nameof(email));

        if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
            throw new ArgumentException($"A senha deve ter pelo menos {MinPasswordLength} caracteres.", nameof(password));

        return (email.Trim().ToLowerInvariant(), password);
    }
}
