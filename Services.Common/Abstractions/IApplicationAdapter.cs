using Services.Common.Abstractions.Model;

namespace Services.Common.Abstractions.Abstractions;

    public interface IApplicationAdapter
    {
        Task Process(Application application);
    }



    

    

