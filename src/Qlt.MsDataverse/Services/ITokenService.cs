using System.Threading.Tasks;

namespace Qlt.MsDataverse.Services
{
    public interface ITokenService
    {
        Task<string> GetTokenWithClientCredential();
    }
}
