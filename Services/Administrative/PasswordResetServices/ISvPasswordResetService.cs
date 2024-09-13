using System.Threading.Tasks;

namespace Services.Administrative.PasswordResetServices
{
    public interface ISvPasswordResetService
    {
        Task<bool> RequestPasswordResetAsync(string email);
    }
}
