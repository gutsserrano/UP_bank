using Models.DTO;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AddressApiServices
{
    public class MockAddressApi : IAddressApiService
    {
        public async Task<Address?> GetAddress(AddressDTO addressDTO)
        {
            return await CreateAddress(addressDTO);
        }

        public async Task<Address> CreateAddress(AddressDTO addressDTO)
        {
            return new Address
            {
                ZipCode = addressDTO.ZipCode,
                Number = addressDTO.Number,
                Complement = addressDTO.Complement,
                Street = "Rua dos Testes",
                City = "Cidade dos Testes",
                State = "JAVA"
            };
        }
    }
}
