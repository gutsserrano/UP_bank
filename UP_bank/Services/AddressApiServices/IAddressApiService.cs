using Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.AddressApiServices
{
    public interface IAddressApiService
    {
        Task<Address?> GetAddress(AddressDTO addressDTO);

        Task<Address?> CreateAddress(AddressDTO addressDTO);
    }
}
