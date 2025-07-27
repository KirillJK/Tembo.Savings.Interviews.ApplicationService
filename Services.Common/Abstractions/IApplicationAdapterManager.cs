using Services.Common.Abstractions.Model;

namespace Services.Applications
{
    public interface IApplicationAdapterManager
    {
        ProductPack Get(ProductCode code);
    }
 
}
